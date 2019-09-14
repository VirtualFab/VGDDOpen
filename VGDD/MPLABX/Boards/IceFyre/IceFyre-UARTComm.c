/********************************************************************
 FileName:    	UARTComm.c
 Hardware:	IceFyre BETA, IceFyre RC1
 Complier:  	Microchip XC32
 Company:	Vinagrón Digital
 Author:	Juan Carlos Orozco Gutierrez

 Software License Agreement:

 This software has been licensed under the GNU General Public
 License is intended to guarantee your freedom to share and change free
 software--to make sure the software is free for all its users.

********************************************************************
 File Description:
    RS232 & RS486 main driver file for IceFyre


 Change History:
  Rev   Description                                 Modified by:
  ----  -----------------------------------------  --------------
  1.0   Initial release                             JCOG

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/
#include "HardwareProfile.h"




//------------------------------------------------------------------------------
// Library Start
//------------------------------------------------------------------------------
void UART_Init(unsigned int initVal)	//initializes the UART module
{
    //Config pins
    UART_RXD_TRIS = 1;
    UART_TXD_TRIS = 0;

    if(initVal > 115200){
        initVal = 19200;
    }

    UARTConfigure(UART_MODULE_ID, UART_ENABLE_PINS_TX_RX_ONLY);
    UARTSetFifoMode(UART_MODULE_ID, UART_INTERRUPT_ON_TX_NOT_FULL | UART_INTERRUPT_ON_RX_NOT_EMPTY);
    UARTSetLineControl(UART_MODULE_ID, UART_DATA_SIZE_8_BITS | UART_PARITY_NONE | UART_STOP_BITS_1);
    UARTSetDataRate(UART_MODULE_ID, GetPeripheralClock(), initVal);
    UARTEnable(UART_MODULE_ID, UART_ENABLE_FLAGS(UART_PERIPHERAL | UART_RX | UART_TX));
}

void UART_SendByte(BYTE data)
{
    while(!UARTTransmitterIsReady(UART_MODULE_ID));

    UARTSendDataByte(UART_MODULE_ID, data);
    
    while(!UARTTransmissionHasCompleted(UART_MODULE_ID));
}

void UART_SendBuffer(const char *buffer, UINT32 size)
{
    while(size){
        while(!UARTTransmitterIsReady(UART_MODULE_ID));

        UARTSendDataByte(UART_MODULE_ID, *buffer);

        buffer++;
        size--;
    }

    while(!UARTTransmissionHasCompleted(UART_MODULE_ID));
}

BYTE UART_ReadByte(void)
{
    while(!UARTReceivedDataIsAvailable(UART_MODULE_ID));

    return UARTGetDataByte(UART_MODULE_ID);
}

UINT32 UART_ReadBuffer(char *buffer, UINT32 max_size)
{
    UINT32 num_char;

    num_char = 0;

    while(num_char < max_size)
    {
        UINT8 character;

        while(!UARTReceivedDataIsAvailable(UART_MODULE_ID));

        character = UARTGetDataByte(UART_MODULE_ID);

        if(character == '\r')
            break;

        *buffer = character;

        buffer++;
        num_char++;
    }

    return num_char;
}
