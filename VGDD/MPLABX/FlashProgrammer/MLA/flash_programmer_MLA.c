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

 This module is used to program an external memory source.  The data is sent
 via the PC utility external_memory_programmer.  Using the comm_pkt protocol, 
 the data is parsed and programed to the external memory.  

 This module is used by serval different Graphics demos and is part of the 
 common directory under the Graphics demo.
 *****************************************************************************/

/*****************************************************************************
 * Section: Includes
 *****************************************************************************/
#include "comm_pkt.h"
#include "system.h"
#include "flash_programmer.h"

#ifdef USE_COMM_PKT_MEDIA_SERIAL_PORT
#include "driver/uart/drv_uart2.h"
#endif
#ifdef USE_COMM_PKT_MEDIA_USB
#include "usb/usb.h"
#include "usb/usb_device_generic.h"
#endif

/*****************************************************************************
 * Section: Module Defines
 *****************************************************************************/
#ifdef USE_COMM_PKT_MEDIA_USB
#define FLASH_PROGRAMMER_COMMUNICATION_MEDIUM (COMM_PKT_MEDIA_USB)
#else
#define FLASH_PROGRAMMER_COMMUNICATION_MEDIUM (COMM_PKT_MEDIA_SERIAL_PORT)
#endif

/*****************************************************************************
 * Section: Function Prototypes
 *****************************************************************************/
void     BinaryMemoryUpload(void);
bool     BinaryHandlePacket(void);
uint32_t CalculateCheckSum(uint32_t addr, uint8_t *buffer, uint32_t range);

/*****************************************************************************
 * Section: Function Pointers
 *****************************************************************************/
FLASH_INIT_FUNC           pFlashInitFunc     = NULL; // function pointer to initialization
FLASH_READ_FUNC           pDataReadFunc      = NULL; // function pointer to data read
FLASH_WRITE_FUNC          pDataWriteFunc     = NULL; // function pointer to data write
FLASH_CHIPERASE_FUNC      pDataChipEraseFunc = NULL; // function pointer to erase chip

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
 * Section: Module Structures
 *****************************************************************************/
typedef struct
{
    uint32_t addr __attribute__((packed));
    uint8_t data;
}COMM_PKT_MEMORY_PAYLOAD;

typedef struct
{
    uint32_t addr __attribute__((packed));
    uint32_t range __attribute__((packed));
}COMM_PKT_VERIFY_PAYLOAD;

/*****************************************************************************
 * Section: Functions
 *****************************************************************************/


/*****************************************************************************
    int ProgramExternalMemory(
                                FLASH_READ_FUNC pReadFunc,
                                FLASH_WRITE_FUNC pWriteFunc,
                                FLASH_CHIPERASE_FUNC pChipErase)
 *****************************************************************************/
int ProgramExternalMemory(
                                FLASH_READ_FUNC pReadFunc,
                                FLASH_WRITE_FUNC pWriteFunc,
                                FLASH_CHIPERASE_FUNC pChipErase)

{

    pDataReadFunc       = pReadFunc;
    pDataWriteFunc      = pWriteFunc;
    pDataChipEraseFunc  = pChipErase;

#ifdef USE_COMM_PKT_MEDIA_USB
    USBGenericOutHandle = 0;
    USBGenericInHandle  = 0;

// The USB specifications require that USB peripheral devices must never source
// current onto the Vbus pin.  Additionally, USB peripherals should not source
// current on D+ or D- when the host/hub is not actively powering the Vbus line.
// When designing a self powered (as opposed to bus powered) USB peripheral
// device, the firmware should make sure not to turn on the USB module and D+
// or D- pull up resistor unless Vbus is actively powered.  Therefore, the
// firmware needs some means to detect when Vbus is being powered by the host.
// A 5V tolerant I/O pin can be connected to Vbus (through a resistor), and
// can be used to detect when Vbus is high (host actively powering), or low
// (host is shut down or otherwise not supplying power).  The USB firmware
// can then periodically poll this I/O pin to know when it is okay to turn on
// the USB module/D+/D- pull up resistor.  When designing a purely bus powered
// peripheral device, it is not possible to source current on D+ or D- when the
// host is not actively providing power on Vbus. Therefore, implementing this
// bus sense feature is optional.  This firmware can be made to use this bus
// sense feature by making sure "USE_USB_BUS_SENSE_IO" has been defined in the
// system_config.h file.
#if defined(USE_USB_BUS_SENSE_IO)
    tris_usb_bus_sense = INPUT_PIN;
#endif

// If the host PC sends a GetStatus (device) request, the firmware must respond
// and let the host know if the USB peripheral device is currently bus powered
// or self powered.  See chapter 9 in the official USB specifications for details
// regarding this request.  If the peripheral device is capable of being both
// self and bus powered, it should not return a hard coded value for this request.
// Instead, firmware should check if it is currently self or bus powered, and
// respond accordingly.  If the hardware has been configured like demonstrated
// on the PICDEM FS USB Demo Board, an I/O pin can be polled to determine the
// currently selected power source.  On the PICDEM FS USB Demo Board, "RA2"
// is used for	this purpose.  If using this feature, make sure "USE_SELF_POWER_SENSE_IO"
// has been defined in system_config.h, and that an appropriate I/O pin has been mapped
// to it in system_config.h.
#if defined(USE_SELF_POWER_SENSE_IO)
    tris_self_power = INPUT_PIN;
#endif

    //usb_device.c.  Initializes USB module SFRs and firmware
    USBDeviceInit();

#endif
    
#ifdef USE_COMM_PKT_MEDIA_SERIAL_PORT
    DRV_UART2_InitializerDefault();
#endif

    //HardwareButtonInit();
    BinaryMemoryUpload();

#ifdef USE_COMM_PKT_MEDIA_USB
    // check if USB has sent reply to host then do a detach
    while(!USBHandleBusy(USBGenericInHandle));
    USBDeviceDetach();
#endif

#ifdef USE_COMM_PKT_MEDIA_SERIAL_PORT
    // check if UART is still busy sending replies to the host
    // poll until the transmit buffer in UART is empty
    while((DRV_UART2_Status() & DRV_UART2_TX_COMPLETE) == 0);
#endif

    return (0);
}

/*****************************************************************************
 * void BinaryMemoryUpload(void)
 *****************************************************************************/
void BinaryMemoryUpload(void)
{
#ifdef USE_COMM_PKT_MEDIA_USB
#if defined(USB_INTERRUPT)
        USBDeviceAttach();
#endif
#endif
        
    COMM_PKT_Init();

    while(!BinaryHandlePacket())
    {
        COMM_PKT_Update(FLASH_PROGRAMMER_COMMUNICATION_MEDIUM);

#ifdef USE_COMM_PKT_MEDIA_USB
#if defined(USB_POLLING)
	// Check bus status and service USB interrupts.
        
        // Polling method.  If using polling, must call
        // this function periodically.  This function will take care
        // of processing and responding to SETUP transactions
        // (such as during the enumeration process when you first
        // plug in).  USB hosts require that USB devices should accept
        // and process SETUP packets in a timely fashion.  Therefore,
        // when using polling, this function should be called
        // regularly (such as once every 1.8ms or faster** [see
        // inline code comments in usb_device.c for explanation when
        // "or faster" applies])  In most cases, the USBDeviceTasks()
        // function does not take very long to execute (ex: <100
        // instruction cycles) before it returns.
        USBDeviceTasks();
#endif
#endif
    }
}

/*****************************************************************************
 * bool BinaryHandlePacket(void)
 *****************************************************************************/
bool BinaryHandlePacket(void)
{
    uint8_t *packet;
    uint8_t *payload;
    COMM_PKT_HDR *hdr;
    uint8_t ack_nack;
    bool result = false;

    if(COMM_PKT_RxPacketAvailable() == false)
        return result; 

    packet = COMM_PKT_GetRxPacket();
    
    if(COMM_PKT_IsPacketValid(packet) == false)
        return result;

    hdr = (COMM_PKT_HDR *)packet;
    payload = packet + sizeof(COMM_PKT_HDR);

    ack_nack = COMM_PKT_ACK;
    
    switch(hdr->cmd)
    {

    case COMM_PKT_ECHO:
        break;

    case COMM_PKT_MAX_PAYLOAD_SIZE:
        *((uint16_t *)payload) = COMM_PKT_RX_MAX_SIZE;
        hdr->length = 2;
        break;

    case COMM_PKT_MEMORY_ERASE:
    	pDataChipEraseFunc();
        break;

    case COMM_PKT_MEMORY_WRITE:
        {    
            COMM_PKT_MEMORY_PAYLOAD *memPayload = (COMM_PKT_MEMORY_PAYLOAD *)payload;

            if(!pDataWriteFunc(memPayload->addr, (uint8_t *)&memPayload->data, hdr->length - 4))
                ack_nack = COMM_PKT_NACK;

            hdr->length = 4;
        }
        break;

    case COMM_PKT_MEMORY_READ:
        {
            COMM_PKT_MEMORY_PAYLOAD *memPayload = (COMM_PKT_MEMORY_PAYLOAD *)payload;

            hdr->length = *((uint16_t *)(payload + 4));
            pDataReadFunc(memPayload->addr, (uint8_t *)&memPayload->data, hdr->length);
            hdr->length += 4;
        }
        break;

    case COMM_PKT_MEMORY_VERIFY:
        {
            COMM_PKT_VERIFY_PAYLOAD *verifyPayload = (COMM_PKT_VERIFY_PAYLOAD *)payload;

            *((uint32_t *)payload) = CalculateCheckSum(verifyPayload->addr, payload, verifyPayload->range);
            hdr->length = 4;
        }
        break;

    case COMM_PKT_MEMORY_DONE:
        result = true;
        break;

    default:
        ack_nack = COMM_PKT_NACK;
        break;
    }

    COMM_PKT_SendReply( FLASH_PROGRAMMER_COMMUNICATION_MEDIUM,
                        hdr->cmd,
                        ack_nack,
                        payload,
                        hdr->length);

    return result;
}

/*****************************************************************************
 * uint32_t CalculateCheckSum(uint32_t addr, uint8_t *buffer, uint32_t range)
 *****************************************************************************/
uint32_t CalculateCheckSum(uint32_t addr, uint8_t *buffer, uint32_t range)
{
    uint32_t   checksum = 0xFFFFFFFF;
    
    while(range > 0)
    {
        uint16_t arraySize = COMM_PKT_RX_MAX_SIZE;
        uint16_t    i;

        if(arraySize > range)
            arraySize = range;

        pDataReadFunc(addr, buffer, arraySize);

        for(i = 0; i < arraySize; i++)
        {
            checksum += (uint32_t)buffer[i];
        }

        range -= arraySize;
        addr += (uint32_t)arraySize;
    }

    checksum *= -1;

    return checksum;

}
