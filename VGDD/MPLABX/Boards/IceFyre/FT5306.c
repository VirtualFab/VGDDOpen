/********************************************************************
 FileName:    	FT5306.c
 Dependencies:
 Hardware:	IceFyre BETA
 Complier:  	Microchip XC32
 Company:	Vinagrón Digital
 Author:	Juan Carlos Orozco Gutierrez

 Software License Agreement:

 This software has been licensed under the GNU General Public
 License is intended to guarantee your freedom to share and change free
 software--to make sure the software is free for all its users.

********************************************************************
 File Description:
    FT5306 capacitive touch panel driver file


 Change History:
  Rev   Description                                 Modified by:
  ----  -----------------------------------------  --------------
  1.0   Initial release                             JCOG

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/
#include "HardwareProfile.h"
#include "FT5306.h"

/* Function prototypes */
void I2C_Init       (void);
void I2C_WriteByte  (BYTE deviceID, BYTE offset, BYTE value);
BYTE I2C_WriteBlock (BYTE deviceID, BYTE offset, BYTE *buffer, WORD length);

//------------------------------------------------------------------------------
//FT5306 Start
//------------------------------------------------------------------------------
static void FT5306_Reset(void)
{
    PCT_RST_TRIS = 0;
    
    PCT_RST_LAT = 0;    //reset FT5306 PCT controller
    DelayMs(500);
    PCT_RST_LAT = 1;
}

void FT5306_Init(void)
{
    FT5306_Reset();

    I2C_Init();

    PCT_INT_TRIS        = 1;    //interrupt pin must be an input
    PCT_INT_ENABLE      = 0;    //disable interrupt
    PCT_INT_TRIGGER     = 1;    //rising edge
    PCT_INT_PRIORITY    = 4;
    PCT_INT_FLAG        = 0;    //clear interrupt flag
    PCT_INT_ENABLE      = 1;    //enable interrupt

    DelayMs(10);
    //Send the normal mode command (0) to FT5306
    FT5306_Write(FT_REG_DEVICE_MODE,0);
}

//------------------------------------------------------------------------------
//I2C Start
//------------------------------------------------------------------------------
void I2C_Init(void)
{
    UINT32 actualClock = 0;
    I2C2CONbits.ON = 0;

    // Set the I2C baudrate
    actualClock = I2CSetFrequency(FT5306_I2C_BUS, GetPeripheralClock(), FT5306_I2C_CLOCK_FREQ);

    #if (FT5306_I2C_CLOCK_FREQ != 400000)
        #error "I2C frequency must be 400kHz for optimum performance"
    #endif
//    if ( abs(actualClock-FT5306_I2C_CLOCK_FREQ) > FT5306_I2C_CLOCK_FREQ/10 )
//    {
//        __asm__ volatile (" sdbbp 0");
//    }

    I2C2CONbits.ON = 1; //start the module

    DelayMs(500);  //Added to stablilize PCAP I2C Reading
}

/*******************************************************************************
  Function: BOOL StartTransfer( BOOL restart )
  Summary:  Starts (or restarts) a transfer to/from the EEPROM.
  Description:
    This routine starts (or restarts) a transfer to/from the I2C, waiting (in
    a blocking loop) until the start (or re-start) condition has completed.

  Precondition:The I2C module must have been initialized.
  Parameters:
    restart - If FALSE, send a "Start" condition
            - If TRUE, send a "Restart" condition
  Returns:
    TRUE    - If successful
    FALSE   - If a collision occured during Start signaling
  Remarks:
    This is a blocking routine that waits for the bus to be idle and the Start
    (or Restart) signal to complete.
  *****************************************************************************/
static BOOL StartTransfer( BOOL restart )
{
    I2C_STATUS  status;

    // Send the Start (or Restart) signal
    if(restart)
    {
        I2CRepeatStart(FT5306_I2C_BUS);
    }
    else
    {
        // Wait for the bus to be idle, then start the transfer
        while( !I2CBusIsIdle(FT5306_I2C_BUS) );

        if(I2CStart(FT5306_I2C_BUS) != I2C_SUCCESS)
        {
            return FALSE;
        }
    }

    // Wait for the signal to complete
    do
    {
        status = I2CGetStatus(FT5306_I2C_BUS);

    } while ( !(status & I2C_START) );

    return TRUE;
}


/*******************************************************************************
  Function:BOOL TransmitOneByte( UINT8 data )
  Summary:      This transmits one byte to the I2C bus.
  Precondition: The transfer must have been previously started.
  Parameters:   data    - Data byte to transmit
  Returns:
    TRUE    - Data was sent successfully
    FALSE   - A bus collision occured
  Remarks:
    This is a blocking routine that waits for the transmission to complete.
  *****************************************************************************/
BOOL TransmitOneByte(UINT8 data)
{
    // Wait for the transmitter to be ready
    while(!I2CTransmitterIsReady(FT5306_I2C_BUS));

    // Transmit the byte
    if(I2CSendByte(FT5306_I2C_BUS, data) == I2C_MASTER_BUS_COLLISION){
        return FALSE;
    }

    // Wait for the transmission to finish
    while(!I2CTransmissionHasCompleted(FT5306_I2C_BUS));

    return TRUE;
}

/*******************************************************************************
  Function: void StopTransfer( void )
  Summary:  Stops a transfer to/from the EEPROM.
  Description:
    This routine Stops a transfer to/from the bus, waiting (in a
    blocking loop) until the Stop condition has completed.

  Precondition: The I2C module must have been initialized & a transfer started.
  Remarks:
    This is a blocking routine that waits for the Stop signal to complete.
***************************************************************************/
static void StopTransfer( void )
{
    I2C_STATUS  status;

    // Send the Stop signal
    I2CStop(FT5306_I2C_BUS);

    // Wait for the signal to complete
    do{
        status = I2CGetStatus(FT5306_I2C_BUS);
    }while(!(status & I2C_STOP));
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

//------------------------------------------------------------------------------
// Function: I2C_WriteBlock
// Description: Write a block of bytes to the spacifed I2C slave address at the specified offset.
//              The offset address is one byte (8 bit offset only).
//              The return code is always 0.  There is no indication in case of error.
//------------------------------------------------------------------------------
BYTE I2C_WriteBlock(BYTE deviceID, BYTE offset, BYTE *buffer, WORD length)
{
    BYTE write_buffer[256];
    WORD count;
    BOOL Success = TRUE;

    memset(write_buffer, 0, sizeof(write_buffer));
    write_buffer[0] = deviceID;
    write_buffer[1] = offset;

    for(count=2 ; count<(length+2) ; count++){
        write_buffer[count] = *buffer;
        buffer++;
    }

    // Start the transfer to write data to the EEPROM
    if(!StartTransfer(FALSE)){
        while(1);
    }

    // Transmit all data
    count = 0;

    while(Success && (count < (length+2)))
    {
        // Transmit a byte
        TransmitOneByte(write_buffer[count]);

        // Advance to the next byte
        count++;

        // Verify that the byte was acknowledged
        if(!I2CByteWasAcknowledged(FT5306_I2C_BUS))
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
    BYTE write_buffer[2]    = {0x00};
    BYTE count              = 0;
    BOOL Success            = TRUE;

    write_buffer[0] = deviceID;
    write_buffer[1] = offset;

    I2C2CONbits.ACKDT = 0;  //ACK is sent JCOG comment

    // Start the transfer to write data to the EEPROM
    if(!StartTransfer(FALSE))   //FALSE -> no bus restart
    {
        while(1);   //a collision ocurred:(
    }

   // Transmit the address with the READ bit set
   deviceID |= 0x01;

   TransmitOneByte(deviceID);

    // Verify that the byte was acknowledged
    if(!I2CByteWasAcknowledged(FT5306_I2C_BUS))
    {
        Success = FALSE;    //it wasn´t acknowledged, fail
    }

    for(count=0 ; count<length ; count++){
        // Read the data from the desired address
        if(Success)
        {
            if(I2CReceiverEnable(FT5306_I2C_BUS, TRUE) == I2C_RECEIVE_OVERFLOW)
            {
                Success = FALSE;
            }
            else
            {
                while(!I2CReceivedDataIsAvailable(FT5306_I2C_BUS));
                *buffer++ = I2C2RCV;

                if(count == (length -1))
                {
                    I2C2CONbits.ACKDT = 1;  //nack is sent
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
