// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// Indicator - MLA version
// *****************************************************************************
// FileName:        indicator.h
// Processor:       PIC24F, PIC24H, dsPIC, PIC32
// Compiler:        MPLAB C30, MPLAB C32
// Company:         VirtualFab, parts from Microchip Technology Incorporated
//
// VirtualFab's Software License Agreement:
// Copyright 2012-2016 Virtualfab - All rights reserved.
// VirtualFab licenses to you the right to use, modify, copy and distribute
// this software only in the event that you purchased at least one license of the VirtualFab's
// Visual Graphics Display Designer (VGDD) software.
//
// Usage of this software without owning a License for VGDD is explicitly forbidden.
//
// The Demo version of VGDD, from which this source may come, doesn't allow you to use it
// in any projects other than those created for test purposes, even if the code is manually created.
//
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
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
// Copyright 2012 Microchip Technology Inc.  All rights reserved.
// Microchip licenses to you the right to use, modify, copy and distribute
// Software only when embedded on a Microchip microcontroller or digital
// signal controller, which is integrated into your product or third party
// product (pursuant to the sublicense terms in the accompanying license
// agreement).
//
// You should refer to the license agreement accompanying this Software
// for additional information regarding your rights and obligations.
//
// SOFTWARE AND DOCUMENTATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
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
// Date         Comment
// *****************************************************************************
//  2012/03/17	Start of Developing
//  2014/09/07  MLA version
//  2016/04/01  MHC version
// *****************************************************************************
#ifndef _INDICATOR_H
#define _INDICATOR_H

#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>
#include "gfx/gfx.h"
#include "system_config.h"
#include "system_definitions.h"

typedef enum {
    INDSTYLE_CIRCLE,
    INDSTYLE_SQUARE
} INDICATORSTYLE;

/*********************************************************************
 * Object States Definition:
 *********************************************************************/
#define IND_DISABLED         0x0002  // Bit for disabled state - no clear will be done.
#define IND_RIGHT_ALIGN      0x0004  // Bit to indicate text is left aligned.
#define IND_CENTER_ALIGN     0x0008  // Bit to indicate text is center aligned.
#define IND_FRAME            0x0010  // Bit to indicate frame is displayed.
#define IND_DRAW             0x4000  // Bit to indicate object must be redrawn.
#define IND_UPDATE           0x2000  // Bit to indicate that only text must be redrawn.
#define IND_HIDE             0x8000  // Bit to remove object from screen.

#define OBJ_INDICATOR GFX_GOL_UNKNOWN_TYPE+1020
#define IND_MSG_SET GFX_GOL_OBJECT_ACTION_PASSIVE+1020
#define IND_MSG_TOUCHSCREEN IND_MSG_SET+1

/*********************************************************************
 * Overview: Defines the parameters required for a Indicator Object.
 *           Depending on the type selected the Indicator is drawn with
 *           the defined shape parameters and values set on the
 *           given fields.
 *
 *********************************************************************/
typedef struct {
    GFX_GOL_OBJ_HEADER  hdr;            // Generic header for all Objects (see GFX_GOL_OBJ_HEADER).
    uint32_t            Value;          // Current value
    uint8_t             Style;          // Style
    uint16_t            IndicatorColour;// Colour
    GFX_XCHAR           *pText;         // The pointer to text used.
} INDICATOR;

/*********************************************************************
 * Function: Indicator  *IndCreate(
 *                      uint16_t ID,int16_t left,int16_t top,int16_t right,int16_t bottom,
 *                      uint16_t state,uint32_t Value,uint8_t NoOfDigits,uint8_t DotPos,uint8_t Thickness,GFX_GOL_OBJ_SCHEME *pScheme
 *
 * Overview: This function creates a Indicator object with the parameters given.
 *           It automatically attaches the new object into a global linked list of
 *           objects and returns the address of the object.
 *
 * PreCondition: none
 *
 * Input: ID - Unique user defined ID for the object instance.
 *        left - Left most position of the object.
 * 	   top - Top most position of the object.
 *	 right - Right most position of the object.
 *	bottom - Bottom most position of the object.
 *       state - Sets the initial state of the object.
 *       value - Initial value set to the Indicator.
 *    minValue - The minimum value the Indicator will display.
 *    maxValue - The maximum value the Indicator will display.
 *       pText - Pointer to the text label of the Indicator.
 *     pScheme - Pointer to the style scheme used.
 *
 * Output: Returns the pointer to the object created.
 *
 * Side Effects: none
 *
 ********************************************************************/
INDICATOR *IndCreate(
        uint16_t ID,
        int16_t left,
        int16_t top,
        int16_t right,
        int16_t bottom,
        uint16_t state,
        uint32_t Value,
        uint8_t Style,
        uint16_t IndicatorColour,
        GFX_XCHAR *pText,
        GFX_GOL_OBJ_SCHEME *pScheme
);

/*********************************************************************
 * Function: uint16_t IndTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Overview: This function evaluates the message from a user if the
 *	     message will affect the object or not. The table below enumerates the translated
 *	     messages for each event of the touch screen and keyboard inputs.
 *
 *	<TABLE>
 *    	Translated Message   Input Source  Events         	Description
 *     	##################   ############  ######         	###########
 *     	MTR_MSG_SET          System        EVENT_SET            If event set occurs and the Indicator ID is sent in parameter 1.
 *	GFX_GOL_OBJECT_ACTION_INVALID	     Any           Any	                If the message did not affect the object.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: pIndicator  - The pointer to the object where the message will be
 *				 evaluated to check if the message will affect the object.
 *        pMsg  - Pointer to the message struct containing the message from
 *        		 the user interface.
 *
 * Output: Returns the translated message depending on the received GOL message:
 *		  - MTR_MSG_SET - Indicator ID is given in parameter 1 for a TYPE_SYSTEM message.
 *         - GFX_GOL_OBJECT_ACTION_INVALID - Indicator is not affected.
 *
 * Side Effects: none
 *
 ********************************************************************/
GFX_GOL_TRANSLATED_ACTION IndTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
 * Function: IndMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 * Overview: This function performs the actual state change
 *			based on the translated message given. Indicator value is set
 *			based on parameter 2 of the message given. The following state changes
 *			are supported:
 *	<TABLE>
 *    	Translated Message   Input Source  Set/Clear State Bit		Description
 *     	##################   ############  ######     				###########
 *     	IND_MSG_SET          System  	   Set IND_DRAW  	Indicator will be redrawn to update value.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: translatedMsg - The translated message.
 *        pIndicator          - The pointer to the object whose state will be modified.
 *        pMsg          - The pointer to the GOL message.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void IndMsgDefault(GFX_GOL_TRANSLATED_ACTION translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
 * Macros:  IndGetVal(pIndicator)
 *
 * Overview: This macro returns the current value of the Indicator.
 *
 * PreCondition: none
 *
 * Input: pIndicator - Pointer to the object.
 *
 * Output: Returns current value of the Indicator.
 *
 * Side Effects: none
 *
 ********************************************************************/
#define IndGetVal(pIndicator) ((pIndicator)->value)

/*********************************************************************
 * Function: IndSetVal(INDICATOR *pIndicator, int16_t newVal)
 *
 * Overview: This function sets the value of the Indicator to the passed
 *			newVal.
 *
 * PreCondition: none
 *
 * Input: pIndicator   - The pointer to the object.
 *        newVal - New value to be set for the Indicator.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void IndSetVal(INDICATOR *pIndicator, int16_t newVal);

/*********************************************************************
 * Function: IndSetColour(INDICATOR *pIndicator, int16_t newColour)
 *
 * Overview: This function sets the colour of the Indicator to the passed
 *			newColour.
 *
 * PreCondition: none
 *
 * Input: pIndicator   - The pointer to the object.
 *        newColour - New colour to be set for the Indicator.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void IndSetColour(INDICATOR *pIndicator, uint16_t newColour);

GFX_STATUS IndDraw(void *pObj);

#endif // _Indicator_H
