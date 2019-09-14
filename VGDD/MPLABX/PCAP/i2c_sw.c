/***********************************************************************************/
/*  Copyright (c) 2002-2009, Silicon Image, Inc.  All rights reserved.             */
/*  No part of this work may be reproduced, modified, distributed, transmitted,    */
/*  transcribed, or translated into any language or computer format, in any form   */
/*  or by any means without written permission of: Silicon Image, Inc.,            */
/*  1060 East Arques Avenue, Sunnyvale, California 94085                           */
/***********************************************************************************/
#include "Compiler.h"
#include "string.h"
#include "TouchScreenCapacitive.h"

#define SIL9292_I2C_BUS              I2C2


void I2C_Init(void)
{
    I2C2CONbits.ON =0;

    //I2C2BRG = 26; // 500 khz Baudrate
    I2C2BRG = 43; // lower Baudrate for reliable operations. Thanks Dave!

    I2C2CONbits.ON =1;

    DelayMs(500);  //Added to stablilize PCAP I2C Reading

}

/*******************************************************************************
  Function:
    BOOL StartTransfer( BOOL restart )

  Summary:
    Starts (or restarts) a transfer to/from the EEPROM.

  Description:
    This routine starts (or restarts) a transfer to/from the EEPROM, waiting (in
    a blocking loop) until the start (or re-start) condition has completed.

  Precondition:
    The I2C module must have been initialized.

  Parameters:
    restart - If FALSE, send a "Start" condition
            - If TRUE, send a "Restart" condition
    
  Returns:
    TRUE    - If successful
    FALSE   - If a collision occured during Start signaling
    
  Example:
    <code>
    StartTransfer(FALSE);
    </code>

  Remarks:
    This is a blocking routine that waits for the bus to be idle and the Start
    (or Restart) signal to complete.
  *****************************************************************************/

BOOL StartTransfer( BOOL restart )
{
    I2C_STATUS  status;

    // Send the Start (or Restart) signal
    if(restart)
    {
        I2CRepeatStart(SIL9292_I2C_BUS);
    }
    else
    {
        // Wait for the bus to be idle, then start the transfer
        while( !I2CBusIsIdle(SIL9292_I2C_BUS) );

        if(I2CStart(SIL9292_I2C_BUS) != I2C_SUCCESS)
        {
            return FALSE;
        }
    }

    // Wait for the signal to complete
    do
    {
        status = I2CGetStatus(SIL9292_I2C_BUS);

    } while ( !(status & I2C_START) );

    return TRUE;
}


/*******************************************************************************
  Function:
    BOOL TransmitOneByte( UINT8 data )

  Summary:
    This transmits one byte to the EEPROM.

  Description:
    This transmits one byte to the EEPROM, and reports errors for any bus
    collisions.

  Precondition:
    The transfer must have been previously started.

  Parameters:
    data    - Data byte to transmit

  Returns:
    TRUE    - Data was sent successfully
    FALSE   - A bus collision occured

  Example:
    <code>
    TransmitOneByte(0xAA);
    </code>

  Remarks:
    This is a blocking routine that waits for the transmission to complete.
  *****************************************************************************/

BOOL TransmitOneByte( UINT8 data )
{
    // Wait for the transmitter to be ready
    while(!I2CTransmitterIsReady(SIL9292_I2C_BUS));

    // Transmit the byte
    if(I2CSendByte(SIL9292_I2C_BUS, data) == I2C_MASTER_BUS_COLLISION)
    {
        return FALSE;
    }

    // Wait for the transmission to finish
    while(!I2CTransmissionHasCompleted(SIL9292_I2C_BUS));

    return TRUE;
}

/*******************************************************************************
  Function:
    void StopTransfer( void )

  Summary:
    Stops a transfer to/from the EEPROM.

  Description:
    This routine Stops a transfer to/from the EEPROM, waiting (in a 
    blocking loop) until the Stop condition has completed.

  Precondition:
    The I2C module must have been initialized & a transfer started.

  Parameters:
    None.
    
  Returns:
    None.
    
  Example:
    <code>
    StopTransfer();
    </code>

  Remarks:
    This is a blocking routine that waits for the Stop signal to complete.
***************************************************************************/

void StopTransfer( void )
{
    I2C_STATUS  status;

    // Send the Stop signal
    I2CStop(SIL9292_I2C_BUS);

    // Wait for the signal to complete
    do
    {
        status = I2CGetStatus(SIL9292_I2C_BUS);

    } while ( !(status & I2C_STOP) );
}

//------------------------------------------------------------------------------
// Function: I2C_WriteByte
// Description: Write one byte to the spacifed I2C slave address at the specified offset.
//              The offset address is one byte (8 bit offset only).
//              No error information is returned.
//------------------------------------------------------------------------------
void I2C_WriteByte(BYTE deviceID, BYTE offset, BYTE value)
{
    I2C_WriteBlock( deviceID, offset, &value, 1 );
}


//------------------------------------------------------------------------------
// Function: I2C_ReadByte
// Description: Read one byte from the spacifed I2C slave address at the specified offset.
//              The offset address is one byte (8 bit offset only).
//              The read data is returned.  There is no indication in case of error.
//------------------------------------------------------------------------------
BYTE I2C_ReadByte(BYTE deviceID, BYTE offset)
{
    BYTE returnData = 0;

    I2C_ReadBlock( deviceID, offset, &returnData, 1 );
	
    return( returnData );
}




//#ifdef USE_DEBUG_CMD_HANDLER   //Uused only when debug command handler is enabled

//------------------------------------------------------------------------------
// Function: I2C_WriteBlock
// Description: Write a block of bytes to the spacifed I2C slave address at the specified offset.
//              The offset address is one byte (8 bit offset only).
//              The return code is always 0.  There is no indication in case of error.
//------------------------------------------------------------------------------
BYTE I2C_WriteBlock(BYTE deviceID, BYTE offset, BYTE *buffer, WORD length)
{
	BYTE write_buffer[256];
	memset(write_buffer, 0, sizeof(write_buffer));
	WORD count;
    BOOL                Success = TRUE;
	
	write_buffer[0] = deviceID;
    write_buffer[1] = offset;

	for (count = 2; count < (length + 2); count++)
	{
		write_buffer[count] = *buffer;
		buffer++;
	}

    // Start the transfer to write data to the EEPROM
    if( !StartTransfer(FALSE) )
    {
        while(1);
    }

    // Transmit all data
    count = 0;

    while( Success && (count < (length + 2)) )
    {
        // Transmit a byte
        TransmitOneByte(write_buffer[count]);
        
            // Advance to the next byte
            count++;

            // Verify that the byte was acknowledged
            if(!I2CByteWasAcknowledged(SIL9292_I2C_BUS))
            {
                Success = FALSE;
            }
    }

    // End the transfer (hang here if an error occured)
    StopTransfer();

    if(!Success)
    {
        while(1);
    }
	
    return(0);	
}


//------------------------------------------------------------------------------
// Function: I2C_ReadBlock
// Description: Read a block of bytes from the spacifed I2C slave address at the specified offset.
//              The offset address is one byte (8 bit offset only).
//              The return code is always 0.  There is no indication in case of error.
//------------------------------------------------------------------------------
BYTE I2C_ReadBlock(BYTE deviceID, BYTE offset, BYTE *buffer, WORD length)
{
    BYTE write_buffer[2] = {0x00};
    BYTE count =0;
    BOOL                Success = TRUE;

    write_buffer[0] = deviceID;
    write_buffer[1] = offset;

    I2C2CONbits.ACKDT = 0;

    // Start the transfer to write data to the EEPROM
    if(!StartTransfer(FALSE) )
    {
        while(1);
    }

   // Transmit the address with the READ bit set
   deviceID |= 0x01;

   TransmitOneByte(deviceID);
        
    // Verify that the byte was acknowledged
    if(!I2CByteWasAcknowledged(SIL9292_I2C_BUS))
    {
        Success = FALSE;
    }

    for(count=0;count<length;count++)
    {

    // Read the data from the desired address
    if(Success)
    {
        if(I2CReceiverEnable(SIL9292_I2C_BUS, TRUE) == I2C_RECEIVE_OVERFLOW)
        {
            Success = FALSE;
        }
        else
        {
            while(!I2CReceivedDataIsAvailable(SIL9292_I2C_BUS));
            *buffer++ = I2C2RCV;

          if(count == (length -1)) 
          {
            I2C2CONbits.ACKDT = 1;
          }
            I2C2CONbits.ACKEN = 1;		// initiate bus acknowledge sequence
            while(I2C2CONbits.ACKEN);
        }

    }
    }

    // End the transfer (stop here if an error occured)
    StopTransfer();

    return(0);	
}

//------------------------------------------------------------------------------
// Function: I2C_ReadBlock
// Description: Read a block of bytes from the spacifed I2C slave address at the specified offset.
//              The offset address is one byte (8 bit offset only).
//              The return code is always 0.  There is no indication in case of error.
//------------------------------------------------------------------------------
BYTE I2C_ReadBlock_PCAP(BYTE deviceID, BYTE offset, BYTE *buffer, WORD length)
{
	BYTE write_buffer[2] = {0x00};
    BYTE count =0;
    BOOL                Success = TRUE;

    write_buffer[0] = deviceID;
    write_buffer[1] = offset;

    // Start the transfer to write data to the EEPROM
    if(!StartTransfer(FALSE) )
    {
        while(1);
    }

    // Transmit all data
    count = 0;

    while( Success && (count < (2)) )
    {
        // Transmit a byte
        TransmitOneByte(write_buffer[count]);
        
            // Advance to the next byte
            count++;

            // Verify that the byte was acknowledged
            if(!I2CByteWasAcknowledged(SIL9292_I2C_BUS))
            {
                Success = FALSE;
            }
    }

    // Start the transfer to read data
    StartTransfer(TRUE);

   // Transmit the address with the READ bit set
   deviceID |= 0x01;

   TransmitOneByte(deviceID);
        
    // Verify that the byte was acknowledged
    if(!I2CByteWasAcknowledged(SIL9292_I2C_BUS))
    {
        Success = FALSE;
    }

    for(count=0;count<length;count++)
    {

    // Read the data from the desired address
    if(Success)
    {
        if(I2CReceiverEnable(SIL9292_I2C_BUS, TRUE) == I2C_RECEIVE_OVERFLOW)
        {
            Success = FALSE;
        }
        else
        {
            while(!I2CReceivedDataIsAvailable(SIL9292_I2C_BUS));
            *buffer++ = I2CGetByte(SIL9292_I2C_BUS);
        }

    }
    }
    // End the transfer (stop here if an error occured)
    StopTransfer();

    return(0);	
}

//#endif //Used only when debug command handler is enabled
