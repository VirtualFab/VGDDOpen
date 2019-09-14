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
 
 This module decodes communication packets like the ones sent from the PC 
 utility, external_memory_programmer.  However, it is not limited to only
 one PC utility or device.  Any device may choose to use this style of
 communication medium.  More details of the communicaiton protocol can
 be found in the extern memory programmer help file documentation.  This
 module is abstracted from the communication medium used and can support
 multiple communicaiton mediums being serviced at one time.
 
 This module is used by serval different Graphics demos and is part of the 
 common directory under the Graphics demo.
 *****************************************************************************/
#ifndef COMM_PKT_HEADER_FILE
#define COMM_PKT_HEADER_FILE

/*****************************************************************************
 * Section: Includes
 *****************************************************************************/
#include <stdint.h>
#include <stdbool.h>

/*****************************************************************************
 * Section: Command Defines
 *****************************************************************************/
#define COMM_PKT_ECHO                        (0x3F)
#define COMM_PKT_MEMORY_ERASE                (0x01)
#define COMM_PKT_MEMORY_SECTOR_ERASE         (0x02)
#define COMM_PKT_MEMORY_WRITE                (0x03)
#define COMM_PKT_MEMORY_READ                 (0x04)
#define COMM_PKT_MAX_PAYLOAD_SIZE            (0x05)
#define COMM_PKT_MEMORY_VERIFY               (0x06)
#define COMM_PKT_MEMORY_DONE                 (0x07)

#define COMM_PKT_REPLY                      1
#define COMM_PKT_ACK                        1
#define COMM_PKT_NACK                       0
/*****************************************************************************
 * Section: Structures
 *****************************************************************************/
typedef struct
{
    uint8_t cmd:   6;
    uint8_t reply: 1;
    uint8_t ack:   1;
    uint8_t check_sum;
    uint16_t length;
}COMM_PKT_HDR;

typedef enum
{
    COMM_PKT_MEDIA_SERIAL_PORT,
    COMM_PKT_MEDIA_USB
}COMM_PKT_MEDIA;

/*****************************************************************************
 * Section: Function Prototypes
 *****************************************************************************/
/*****************************************************************************
 * COMM_PKT_Init
 *****************************************************************************/
void COMM_PKT_Init(void);

/*****************************************************************************
 * COMM_PKT_Update
 *****************************************************************************/
void COMM_PKT_Update(COMM_PKT_MEDIA media);

/*****************************************************************************
 * COMM_PKT_RxPacketAvailable
 *****************************************************************************/
bool COMM_PKT_RxPacketAvailable(void);

/*****************************************************************************
 * COMM_PKT_GetRxPacket
 *****************************************************************************/
uint8_t *COMM_PKT_GetRxPacket(void);

/*****************************************************************************
 * COMM_PKT_IsPacketValid
 *****************************************************************************/
bool COMM_PKT_IsPacketValid(
                                uint8_t *packet);

/*****************************************************************************
 * COMM_PKT_SendCommand
 *****************************************************************************/
void COMM_PKT_SendCommand(
                                COMM_PKT_MEDIA media,
                                uint8_t cmd,
                                uint8_t *payload,
                                uint16_t payloadSize);

/*****************************************************************************
 * COMM_PKT_SendReply
 *****************************************************************************/
void COMM_PKT_SendReply(
                                COMM_PKT_MEDIA media,
                                uint8_t cmd,
                                uint8_t ack,
                                uint8_t *payload,
                                uint16_t payloadSize);

#endif
