// ----------------------------------------------------------------------------
// VGDD Main Skeleton - Header file
// This program is the starting point to build a VGDD-enabled GOL Application
// ----------------------------------------------------------------------------
// FileName:            vgdd_main.h
// Dependencies:        See INCLUDES section below
// Compiler:            [COMPILER]
// Development Board:   [DEVBOARD]
// Expansion Board:     [EXPANSIONBOARD]
// Company:             VirtualFab, parts from Microchip Technology Incorporated
//
// VirtualFab Software License Agreement:
// Copyright 2012-2016 Virtualfab - All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
// Redistributions of source code must retain the above and following copyright notices,
// this list of conditions and the following disclaimer.
// Neither the name of the Fabio Violino or Virtualfab may be used to endorse or promote products
// derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR “AS IS” AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
// OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// Microchip's Software License Agreement:
//
// Copyright © 2012 Microchip Technology Inc.  All rights reserved.
// Microchip licenses to you the right to use, modify, copy and distribute
// Software only when embedded on a Microchip microcontroller or digital
// signal controller, which is integrated into your product or third party
// product (pursuant to the sublicense terms in the accompanying license
// agreement).
//
// You should refer to the license agreement accompanying this Software
// for additional information regarding your rights and obligations.
//
// SOFTWARE AND DOCUMENTATION ARE PROVIDED “AS IS” WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY
// OF MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR
// PURPOSE. IN NO EVENT SHALL MICROCHIP OR ITS LICENSORS BE LIABLE OR
// OBLIGATED UNDER CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION,
// BREACH OF WARRANTY, OR OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT
// DAMAGES OR EXPENSES INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL,
// INDIRECT, PUNITIVE OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
// COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY
// CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF),
// OR OTHER SIMILAR COSTS.
//
// Author               Date        Comment
//
// VirtualFab        2012/01/29     First MPLAB X Wizard implementation 
// ----------------------------------------------------------------------------
#ifndef _VGDDMAIN_H
#define _VGDDMAIN_H

#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
#include <gfx_gol.h>
#include "system.h"

bool APP_ObjectMessageCallback( GFX_GOL_TRANSLATED_ACTION objectMessage,
                                GFX_GOL_OBJ_HEADER *pObject,
                                GFX_GOL_MESSAGE *pMessage);
bool APP_ObjectDrawCallback(void);

// <editor-fold defaultstate="collapsed" desc="Generated Code">
// VGDD_MPLABX_WIZARD_START_SECTION: MainHeader *** DO NOT DELETE THIS LINE! ***
// These lines will be replaced by VGDD MPLAB X Wizard with lines for the MainHeader Section
// Don't delete the starting and ending markers!
// VGDD_MPLABX_WIZARD_END_SECTION *** DO NOT DELETE THIS LINE! ***
// </editor-fold>

// --------------------------------------------------------------------
//                            APPLICATION STATES
// --------------------------------------------------------------------

typedef enum {
// Put here your manually defined screens states

#include "vgdd_[PROJECT_NAME]_screen_states.h"

} SCREEN_STATES;

volatile extern SCREEN_STATES screenState;
extern uint32_t tick;

#endif // _VGDDMAIN_H
