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
// VirtualFab        2014/02/10     First implementation for MLA GFX 4.0.0
// ----------------------------------------------------------------------------

#ifndef __SYSTEM_H
#define __SYSTEM_H

/*********************************************************************
* PIC Device Specific includes
*********************************************************************/
#include <stdlib.h>
#include <stdbool.h>
#include <xc.h>
#include "system_config.h"
#include <gfx_gol.h>

// <editor-fold defaultstate="collapsed" desc="Generated Code">
// VGDD_MPLABX_WIZARD_START_SECTION: SystemHead *** DO NOT DELETE THIS LINE! ***
// These lines will be replaced by VGDD MPLAB X Wizard with lines for the SystemHead Section
// Don't delete the starting and ending markers!
// VGDD_MPLABX_WIZARD_END_SECTION *** DO NOT DELETE THIS LINE! ***
// </editor-fold>
#include <libpic30.h>

void SYSTEM_InitializeBoard(void);
void ErrorTrap(GFX_XCHAR *message);
void TickTouchInit(void);

// <editor-fold defaultstate="collapsed" desc="Generated Code">
// VGDD_MPLABX_WIZARD_START_SECTION: System *** DO NOT DELETE THIS LINE! ***
// These lines will be replaced by VGDD MPLAB X Wizard with lines for the System Section
// Don't delete the starting and ending markers!
// VGDD_MPLABX_WIZARD_END_SECTION *** DO NOT DELETE THIS LINE! ***
// </editor-fold>

#endif 




