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

/*****************************************************************************
 * Section: Includes
 *****************************************************************************/
#include <stdint.h>
#include <stdbool.h>
#include "comm_pkt.h"
#include "comm_pkt_callback.h"
#include "system.h"

/*****************************************************************************
 * Section: Variables
 *****************************************************************************/
uint8_t rxPacket[COMM_PKT_RX_MAX_SIZE + sizeof(COMM_PKT_HDR) + 64] __attribute__((aligned(4)));
uint16_t rxPacketIdx;

/*****************************************************************************
 * Section: Function Prototypes
 *****************************************************************************/
uint8_t COMM_PKT_GenerateCheckSum(
                                uint8_t *payload,
                                uint16_t payloadLength);

/*****************************************************************************
 * void COMM_PKT_Init(void)
 *****************************************************************************/
void COMM_PKT_Init(void)
{
    rxPacketIdx = 0;
}
/*****************************************************************************
 * void COMM_PKT_Update(COMM_PKT_MEDIA media)
 *****************************************************************************/
void COMM_PKT_Update(COMM_PKT_MEDIA media)
{
    if(COMM_PKT_DataAvailable(media))
    {
        rxPacketIdx = COMM_PKT_GetData(media, rxPacket, rxPacketIdx);
    }
}
/*****************************************************************************
 * bool COMM_PKT_RxPacketAvailable(void)
 *****************************************************************************/
bool COMM_PKT_RxPacketAvailable(void)
{
    COMM_PKT_HDR *hdr;

    if(rxPacketIdx < sizeof(COMM_PKT_HDR))
        return false;

    hdr = (COMM_PKT_HDR *)rxPacket;

    if(rxPacketIdx < (sizeof(COMM_PKT_HDR) + hdr->length))
        return false;

    if(rxPacketIdx > (sizeof(COMM_PKT_HDR) + hdr->length))
        return false;

    return true;
}
/*****************************************************************************
 * uint8_t *COMM_PKT_GetRxPacket(void)
 *****************************************************************************/
uint8_t *COMM_PKT_GetRxPacket(void)
{
    rxPacketIdx = 0;

    return rxPacket;
}
/*****************************************************************************
 * bool COMM_PKT_IsPacketValid(
                                uint8_t *packet)

 *****************************************************************************/
bool COMM_PKT_IsPacketValid(
                                uint8_t *packet)
{
    COMM_PKT_HDR *hdr;

    hdr = (COMM_PKT_HDR *)packet;

    if(hdr->length > (COMM_PKT_RX_MAX_SIZE))
        return false;

    if(hdr->check_sum != COMM_PKT_GenerateCheckSum(
                                    packet +
                                    sizeof(COMM_PKT_HDR),
                                    hdr->length))
    {
        Nop();
        Nop();
        return false;
    }

    return true;
}
/*****************************************************************************
 * void COMM_PKT_SendCommand(
                                COMM_PKT_MEDIA media,
                                uint8_t cmd,
                                uint8_t *payload,
                                uint16_t payloadSize)

 *****************************************************************************/
void COMM_PKT_SendCommand(COMM_PKT_MEDIA media,
                                uint8_t cmd,
                                uint8_t *payload,
                                uint16_t payloadSize)
{
    COMM_PKT_HDR hdr;

    hdr.cmd = cmd;
    hdr.reply = 0;
    hdr.ack = 0;
    hdr.length = payloadSize;
    hdr.check_sum = COMM_PKT_GenerateCheckSum(payload, payloadSize);

    COMM_PKT_SendData(media, (uint8_t *)&hdr, sizeof(hdr));
    COMM_PKT_SendData(media, payload, payloadSize);
}
/*****************************************************************************
 * void COMM_PKT_SendReply(
                                COMM_PKT_MEDIA media,
                                uint8_t cmd,
                                uint8_t ack,
                                uint8_t *payload,
                                uint16_t payloadSize)

 *****************************************************************************/
void COMM_PKT_SendReply(
                                COMM_PKT_MEDIA media,
                                uint8_t cmd,
                                uint8_t ack,
                                uint8_t *payload,
                                uint16_t payloadSize)
{
    COMM_PKT_HDR hdr;

    hdr.cmd = cmd;
    hdr.reply = COMM_PKT_REPLY;
    hdr.ack = ack;
    hdr.length = payloadSize;
    hdr.check_sum = COMM_PKT_GenerateCheckSum(payload, payloadSize);

    COMM_PKT_SendData(media, (uint8_t *)&hdr, sizeof(hdr));
    COMM_PKT_SendData(media, payload, payloadSize);
}
/*****************************************************************************
 * uint8_t COMM_PKT_GenerateCheckSum(
                                uint8_t *payload,
                                uint16_t payloadLength)

 *****************************************************************************/
uint8_t COMM_PKT_GenerateCheckSum(
                                uint8_t *payload,
                                uint16_t payloadLength)
{
    uint16_t i;
    uint8_t check_sum = 0xFF;

    for(i = 0; i < payloadLength; i++)
    {
        check_sum += payload[i];    
    }

    check_sum *= -1;

    return check_sum;
}
