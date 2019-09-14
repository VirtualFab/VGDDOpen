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
  1.0   Ported from OLIMEX source                   JCOG
  1.1   Cleaned & teaked code                       JCOG
  1.2   Added extra functions                       JCOG

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/
#ifndef _RS232_H
#define _RS232_H


void UART_Init(unsigned int initVal);	//initializes the UART module
void UART_SendByte(BYTE data);
void UART_SendBuffer(const char *buffer, UINT32 size);
BYTE UART_ReadByte(void);
UINT32 UART_ReadBuffer(char *buffer, UINT32 max_size);


#endif // _RS232_H
