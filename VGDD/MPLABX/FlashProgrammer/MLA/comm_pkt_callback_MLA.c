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

 This module handles the callback routines used by the module, comm_pkt.  The
 module interfaces with the hardware, in this case serial and/or USB, to 
 receive and transmitt data.  The comm_pkt module is abstracted from all
 communication mediums.  The user can add/subtract communication mediums
 as needed per application's requirements.  

 This module is used by serval different Graphics demos and is part of the 
 common directory under the Graphics demo.
 *****************************************************************************/

/*****************************************************************************
 * Section: Includes
 *****************************************************************************/
#include <stdint.h>
#include <stdbool.h>
#include "system.h"
#include "comm_pkt.h"
#ifdef USE_COMM_PKT_MEDIA_SERIAL_PORT
#include "driver/uart/drv_uart2.h"
#endif
#ifdef USE_COMM_PKT_MEDIA_USB
#include "usb/usb.h"
#include "usb/usb_device_generic.h"
#endif

/*****************************************************************************
 * Section: Externs
 *****************************************************************************/
#ifdef USE_COMM_PKT_MEDIA_USB
extern unsigned char OUTPacket[64];	//User application buffer for receiving and holding OUT packets sent from the host
extern unsigned char INPacket[64];		//User application buffer for sending IN packets to the host
extern USB_HANDLE USBGenericOutHandle;
extern USB_HANDLE USBGenericInHandle;
#endif

/*****************************************************************************
 * bool COMM_PKT_DataAvailable(COMM_PKT_MEDIA media)
 *****************************************************************************/
bool COMM_PKT_DataAvailable(COMM_PKT_MEDIA media)
{

#ifdef USE_COMM_PKT_MEDIA_SERIAL_PORT
    DRV_UART2_TRANSFER_STATUS status;
    if(media == COMM_PKT_MEDIA_SERIAL_PORT)
    {
        // check if data is present
        DRV_UART2_TasksRX();
        status = DRV_UART2_TransferStatus();
        if (status & DRV_UART2_TRANSFER_STATUS_RX_DATA_PRESENT)
            return true;
    }
#endif

#ifdef USE_COMM_PKT_MEDIA_USB
    if(media == COMM_PKT_MEDIA_USB)
    {
        if((USBDeviceState < CONFIGURED_STATE)||(USBSuspendControl==1)) 
            return false;

        return !USBHandleBusy(USBGenericOutHandle);
    }
#endif

    return false;
}
/*****************************************************************************
 * uint16_t COMM_PKT_GetData(COMM_PKT_MEDIA media, uint8_t *buffer, uint16_t offset)
 *****************************************************************************/
uint16_t COMM_PKT_GetData(COMM_PKT_MEDIA media, uint8_t *buffer, uint16_t offset)
{
    if(offset >= (COMM_PKT_RX_MAX_SIZE + sizeof(COMM_PKT_HDR)))
        return offset;

#ifdef USE_COMM_PKT_MEDIA_SERIAL_PORT
    DRV_UART2_TRANSFER_STATUS status = 0;

    if(media == COMM_PKT_MEDIA_SERIAL_PORT)
    {
        status = DRV_UART2_TransferStatus();

        while (status & DRV_UART2_TRANSFER_STATUS_RX_DATA_PRESENT)
        {
            buffer[offset] = DRV_UART2_ReadByte() ;
            offset++;
            status = DRV_UART2_TransferStatus();
        }

    }
#endif

#ifdef USE_COMM_PKT_MEDIA_USB
    if(media == COMM_PKT_MEDIA_USB)
    {
        while(!USBHandleBusy(USBGenericOutHandle))
        {
            uint16_t len = USBHandleGetLength(USBGenericOutHandle);
    
            if(len > 0)
            {
                memcpy(&buffer[offset], OUTPacket, len);
                offset += len;
            }

            USBGenericOutHandle = USBGenRead(USBGEN_EP_NUM,(uint8_t*)&OUTPacket,USBGEN_EP_SIZE);
        }
    }
#endif

    return offset;
}
/*****************************************************************************
 * void COMM_PKT_SendData(COMM_PKT_MEDIA media, uint8_t *data, uint16_t size)
 *****************************************************************************/
void COMM_PKT_SendData(COMM_PKT_MEDIA media, uint8_t *data, uint16_t size)
{
#ifdef USE_COMM_PKT_MEDIA_SERIAL_PORT
    DRV_UART2_TRANSFER_STATUS status;

    if(media == COMM_PKT_MEDIA_SERIAL_PORT)
    {
        while(size)
        {
            // write the reply one byte at a time
            DRV_UART2_WriteByte(*data);
            status = 0;
            while((status & DRV_UART2_TRANSFER_STATUS_TX_EMPTY) == 0)
            {
                DRV_UART2_TasksTX();
                status = DRV_UART2_TransferStatus();
            }
            data++;
            size--;
        }
    }
#endif

#ifdef USE_COMM_PKT_MEDIA_USB
    if(media == COMM_PKT_MEDIA_USB)
    {
        while(size)
        {
            uint16_t len = size;

            while(USBHandleBusy(USBGenericInHandle))
                ;

            if(len > USBGEN_EP_SIZE)
                len = USBGEN_EP_SIZE;

            memcpy(INPacket, data, len);
            USBGenericInHandle = USBGenWrite(USBGEN_EP_NUM,(uint8_t*)&INPacket,len);

            data += len;
            size -= len;
        }
    }
#endif
}
