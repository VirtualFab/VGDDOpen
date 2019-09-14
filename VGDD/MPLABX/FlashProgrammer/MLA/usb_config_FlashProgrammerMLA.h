/*
********************************************************************************
                                                                                
Software License Agreement                                                      
                                                                                
Copyright (c) 2007-2008 Microchip Technology Inc.  All rights reserved.           
                                                                                
Microchip licenses to you the right to use, modify, copy and distribute Software
only when embedded on a Microchip microcontroller or digital signal controller  
that is integrated into your product or third party product (pursuant to the    
sublicense terms in the accompanying license agreement).                        
                                                                                
You should refer to the license agreement accompanying this Software for        
additional information regarding your rights and obligations.                   
                                                                                
SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,   
EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY OF        
MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.  
IN NO EVENT SHALL MICROCHIP OR ITS LICENSORS BE LIABLE OR OBLIGATED UNDER       
CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION, BREACH OF WARRANTY, OR    
OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT DAMAGES OR EXPENSES         
INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL, INDIRECT, PUNITIVE OR     
CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, COST OF PROCUREMENT OF        
SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY CLAIMS BY THIRD PARTIES          
(INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF), OR OTHER SIMILAR COSTS.     
                                                                                
********************************************************************************
*/

// VGDD usb_config.h for support of DEVICE functionality for External Memory Programmer support

#ifndef USBCFG_H
#define USBCFG_H

/** DEFINITIONS ****************************************************/
#define USB_EP0_BUFF_SIZE 8     // Valid Options: 8, 16, 32, or 64 bytes.
                                // Using larger options take more SRAM, but
                                // does not provide much advantage in most types
                                // of applications.  Exceptions to this, are applications
                                // that use EP0 IN or OUT for sending large amounts of
                                // application related data.
									
#define USB_MAX_NUM_INT     1   // For tracking Alternate Setting
#define USB_MAX_EP_NUMBER   1

#define USB_USER_DEVICE_DESCRIPTOR &device_dsc
#define USB_USER_DEVICE_DESCRIPTOR_INCLUDE extern const USB_DEVICE_DESCRIPTOR device_dsc

#define USB_USER_CONFIG_DESCRIPTOR USB_CD_Ptr
#define USB_USER_CONFIG_DESCRIPTOR_INCLUDE extern const uint8_t *const USB_CD_Ptr[]
#define USB_PING_PONG_MODE  USB_PING_PONG__FULL_PING_PONG
#define USB_INTERRUPT

#define USB_PULLUP_OPTION USB_PULLUP_ENABLE


#define USB_TRANSCEIVER_OPTION USB_INTERNAL_TRANSCEIVER

#define USB_SPEED_OPTION USB_FULL_SPEED

#define USB_ENABLE_STATUS_STAGE_TIMEOUTS    //Comment this out to disable this feature.  

#define USB_STATUS_STAGE_TIMEOUT     (uint8_t)45   //Approximate timeout in milliseconds, except when
                                                //USB_POLLING mode is used, and USBDeviceTasks() is called at < 1kHz
                                                //In this special case, the timeout becomes approximately:
//Timeout(in milliseconds) = ((1000 * (USB_STATUS_STAGE_TIMEOUT - 1)) / (USBDeviceTasks() polling frequency in Hz))
#define USB_SUPPORT_DEVICE

#define USB_NUM_STRING_DESCRIPTORS 4

#define USB_ENABLE_ALL_HANDLERS
/** DEVICE CLASS USAGE *********************************************/
#define USB_USE_GEN

/** ENDPOINTS ALLOCATION *******************************************/

/* Generic */
#define USBGEN_EP_SIZE          64
#define USBGEN_EP_NUM            1

/** DEFINITIONS ****************************************************/

#endif //USBCFG_H
