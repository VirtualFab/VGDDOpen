// ----------------------------------------------------------------------------
// VGDD Skeleton
// Hardware specific definitions
// ----------------------------------------------------------------------------
// FileName:        system_config.h for VGDD Project [PROJECT_NAME]
// Dependencies:    none
// Processor:       Generated for [PROCESSOR] by VGDD [VGDDVERSION] MPLAB X Wizard
// Compiler:        [COMPILER]
// Company:         VirtualFab, parts from Microchip Technology Incorporated
//
// VirtualFab Software License Agreement:
//
// Copyright 2012-2016 Virtualfab - All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
// Redistributions of source code must retain the above and following copyright 
// notices, this list of conditions and the following disclaimer.
// Neither the name of Fabio Violino or Virtualfab may be used to endorse or promote 
// products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR “AS IS” AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF 
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//
// Microchip's Software License Agreement:
//
// Copyright (c) 2013 released Microchip Technology Inc.  All rights reserved.
//
// Microchip licenses to you the right to use, modify, copy and distribute
// Software only when embedded on a Microchip microcontroller or digital signal
// controller that is integrated into your product or third party product
// (pursuant to the sublicense terms in the accompanying license agreement).
// 
// You should refer to the license agreement accompanying this Software for
// additional information regarding your rights and obligations.
//
// SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY OF
// MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
// IN NO EVENT SHALL MICROCHIP OR ITS LICENSORS BE LIABLE OR OBLIGATED UNDER
// CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION, BREACH OF WARRANTY, OR
// OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT DAMAGES OR EXPENSES
// INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL, INDIRECT, PUNITIVE OR
// CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, COST OF PROCUREMENT OF
// SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY CLAIMS BY THIRD PARTIES
// (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF), OR OTHER SIMILAR COSTS.
//
// Author               Date        Comment
//
// VirtualFab        2014/03/30     First implementation for Harmony v070b
// VirtualFab        2014/09/05     Harmony v1.00
// VirtualFab        2015/01/29     Harmony v1.02
// ----------------------------------------------------------------------------

//Removes PLIB Unsupported Warnings
#ifndef _PLIB_UNSUPPORTED
    #define _PLIB_UNSUPPORTED
#endif

#ifndef _SYSTEM_CONFIG_H
#define _SYSTEM_CONFIG_H

#include <stdlib.h>  // for malloc() declaration
#include <xc.h> 
#include "gfx_config.h"
#include "peripheral/ports/plib_ports.h"
//#include "system/msg/sys_msg.h"

// <editor-fold defaultstate="collapsed" desc="Generated Code">
// VGDD_MPLABX_WIZARD_START_SECTION: HardwareProfileHead *** DO NOT DELETE THIS LINE! ***
// These lines will be replaced by VGDD MPLAB X Wizard with lines for the HardwareProfileHead Section
// Don't delete the starting and ending markers!
// VGDD_MPLABX_WIZARD_END_SECTION *** DO NOT DELETE THIS LINE! ***
// </editor-fold>

#define DRV_TOUCHSCREEN_FONT GOLFontDefault

// <editor-fold defaultstate="collapsed" desc="Generated Code">
// VGDD_MPLABX_WIZARD_START_SECTION: HardwareProfile *** DO NOT DELETE THIS LINE! ***
// These lines will be replaced by VGDD MPLAB X Wizard with lines for the HardwareProfile Section
// Don't delete the starting and ending markers!
// VGDD_MPLABX_WIZARD_END_SECTION *** DO NOT DELETE THIS LINE! ***
// </editor-fold>

//Display
//#define GFX_USE_DISPLAY_PANEL_TFT_G240320LTSW_118W_E
//#define GFX_USE_DISPLAY_PANEL_PH480272T_005_I11Q

//Graphics Library
#define USE_TOUCHSCREEN
#define USE_FONT_FLASH
#define USE_BITMAP_FLASH

#define GFX_CONFIG_FONT_EXTERNAL_DISABLE
#define GFX_CONFIG_FONT_ANTIALIASED_DISABLE
#define GFX_CONFIG_IMAGE_EXTERNAL_DISABLE
#define GFX_malloc(size)                                malloc(size)
#define GFX_free(pObj)                                  free(pObj)
#define GFX_GOL_FOCUS_LINE                              2
#define GFX_GOL_EMBOSS_SIZE                             3

// INTERRUPT
#define INT_IRQ_MAX  5

// --------------------------------------------------------------------
// RTCC DEFAULT INITIALIZATION (these are values to initialize the RTCC)
// --------------------------------------------------------------------
#define RTCC_DEFAULT_DAY        15      // 15st
#define RTCC_DEFAULT_MONTH       9      // September
#define RTCC_DEFAULT_YEAR       15      // 2015
#define RTCC_DEFAULT_WEEKDAY    02      // Tuesday
#define RTCC_DEFAULT_HOUR       10      // 10:20:30
#define RTCC_DEFAULT_MINUTE     20
#define RTCC_DEFAULT_SECOND     30

#endif // _SYSTEM_CONFIG_H
