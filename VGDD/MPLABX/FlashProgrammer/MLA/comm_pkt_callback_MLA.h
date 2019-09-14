/********************************************************************
 Software License Agreement:

 The software supplied herewith by Microchip Technology Incorporated
 (the "Company") for its PIC(R) Microcontroller is intended and
 supplied to you, the Company's customer, for use solely and
 exclusively on Microchip PIC Microcontroller products. The
 software is owned by the Company and/or its supplier, and is
 protected under applicable copyright laws. All rights are reserved.
 Any use in violation of the foregoing restrictions may subject the
 user to criminal sanctions under applicable laws, as well as to
 civil liability for the breach of the terms and conditions of this
 license.

 THIS SOFTWARE IS PROVIDED IN AN "AS IS" CONDITION. NO WARRANTIES,
 WHETHER EXPRESS, IMPLIED OR STATUTORY, INCLUDING, BUT NOT LIMITED
 TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 PARTICULAR PURPOSE APPLY TO THIS SOFTWARE. THE COMPANY SHALL NOT,
 IN ANY CIRCUMSTANCES, BE LIABLE FOR SPECIAL, INCIDENTAL OR
 CONSEQUENTIAL DAMAGES, FOR ANY REASON WHATSOEVER.
 *******************************************************************/
/*****************************************************************************
 Section: Description
 
 This module handles the callback routines used by the module, comm_pkt.  
 The module interfaces with the hardware, in this case serial and/or USB,
 to receive and transmitt data.  The comm_pkt module is abstracted from all
 communication mediums.  The user can add/subtract communication mediums
 as needed per application's requirements.  
 
 This module is used by serval different Graphics demos and is part of the 
 common directory under the Graphics demo.
 *****************************************************************************/
#ifndef COMM_PKT_CALLBACK_H_FILE
#define COMM_PKT_CALLBACK_H_FILE

/*****************************************************************************
 * Section: Includes
 *****************************************************************************/
#include "comm_pkt.h"


/*****************************************************************************
 * Section: Function Prototypes
 *****************************************************************************/
/*****************************************************************************
 * bool COMM_PKT_DataAvailable( 
                                COMM_PKT_MEDIA media)
 
 *****************************************************************************/
bool COMM_PKT_DataAvailable(
                                COMM_PKT_MEDIA media);

/*****************************************************************************
 * uint16_t COMM_PKT_GetData(
                                COMM_PKT_MEDIA media,
                                uint8_t *buffer,
                                uint16_t offset)

 *****************************************************************************/
uint16_t COMM_PKT_GetData(
                                COMM_PKT_MEDIA media,
                                uint8_t *buffer,
                                uint16_t offset);

/*****************************************************************************
 * void COMM_PKT_SendData(
                                COMM_PKT_MEDIA media,
                                uint8_t *data,
                                uint16_t size)

 *****************************************************************************/
void COMM_PKT_SendData(
                                COMM_PKT_MEDIA media,
                                uint8_t *data,
                                uint16_t size);

#endif
