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

#ifndef _FLASHPROGRAMMER_H
#define _FLASHPROGRAMMER_H

/*****************************************************************************
 * Section: Includes
 *****************************************************************************/
#include <stdlib.h>

// typedef for initiazation function pointer
typedef void (*FLASH_INIT_FUNC     )(DRV_SPI_INIT_DATA *);

// typedef for read function pointer
typedef void (*FLASH_READ_FUNC     )(uint32_t, uint8_t*, uint16_t);

// typedef for write function pointer
typedef uint8_t (*FLASH_WRITE_FUNC    )(uint32_t, uint8_t*, uint16_t);

// typedef for sector erase function pointer
typedef void (*FLASH_CHIPERASE_FUNC)(void);

/*****************************************************************************
 * Section: Function Prototypes
 *****************************************************************************/
/*****************************************************************************
 * int ProgramExternalMemory(
                                FLASH_READ_FUNC pReadFunc,
                                FLASH_WRITE_FUNC pWriteFunc,
                                FLASH_CHIPERASE_FUNC pChipErase)
 *****************************************************************************/
int ProgramExternalMemory(
                                FLASH_READ_FUNC pReadFunc,
                                FLASH_WRITE_FUNC pWriteFunc,
                                FLASH_CHIPERASE_FUNC pChipErase);
    
    
    
#endif // _FLASHPROGRAMMER_H
