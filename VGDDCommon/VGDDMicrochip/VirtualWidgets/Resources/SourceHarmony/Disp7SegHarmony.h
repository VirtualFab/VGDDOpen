// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// Disp7Seg - Harmony version
// *****************************************************************************
// FileName:        disp7seg.h
// Processor:       PIC32
// Compiler:        MPLAB XC32
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
//  2012/02/26	Start of Developing
//  2016/04/01  MHC version
// *****************************************************************************
#ifndef _DISP7SEG_H
#define _DISP7SEG_H

#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>
#include "gfx/gfx.h"
#include "system_config.h"
#include "system_definitions.h"

/* User should change this value depending on the number of digits he wants to display */
#define D7_WIDTH    0x0A        // This value should be more than the no of digits displayed

/*********************************************************************
 * Object States Definition:
 *********************************************************************/
#define D7_DISABLED         0x0002  // Bit for disabled state - no clear will be done.
#define D7_RIGHT_ALIGN      0x0004  // Bit to indicate value is left aligned.
#define D7_FRAME            0x0010  // Bit to indicate frame is displayed.
#define D7_DRAWPOLY         0x0020  // Bit to indicate digits are to be drawn using PolyLine
#define D7_DRAW             0x4000  // Bit to indicate object must be redrawn.
#define D7_UPDATE           0x2000  // Bit to indicate that only text must be redrawn.
#define D7_HIDE             0x8000  // Bit to remove object from screen.

#define OBJ_DISP7SEG GFX_GOL_UNKNOWN_TYPE+1010
#define D7_MSG_SET GFX_GOL_OBJECT_ACTION_PASSIVE+1010
#define D7_MSG_TOUCHED D7_MSG_SET+1

/*********************************************************************
 * Overview: Defines the parameters required for a Disp7Seg Object.
 *           Depending on the type selected the Disp7Seg is drawn with
 *           the defined shape parameters and values set on the
 *           given fields.
 *
 *********************************************************************/
typedef struct {
    GFX_GOL_OBJ_HEADER  hdr;       // Generic header for all Objects (see GFX_GOL_OBJ_HEADER).
    uint32_t       CurrentValue;   // Current value
    uint32_t       PreviousValue;  // Previous value
    uint8_t        NoOfDigits;     // Number of digits to be displayed
    uint8_t        DigitHeight;    // Height for the digits - based on object's height
    uint8_t        DigitWidth;     // Width for the digits - based on object's Width / NoOfDigits
    uint8_t        Thickness;      // Thickness for the drawing of segments
    uint8_t        DotPos;         // Position of decimal point
} DISP7SEG;

/*********************************************************************
 * Function: Disp7Seg  *D7Create(
 *                      uint16_t ID,int16_t left,int16_t top,int16_t right,int16_t bottom,
 *                      uint16_t state,uint32_t Value,uint8_t NoOfDigits,uint8_t DotPos,uint8_t Thickness,GFX_GOL_SCHEME *pScheme
 *
 * Overview: This function creates a Disp7Seg object with the parameters given.
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
 *       value - Initial value set to the Disp7Seg.
 *    minValue - The minimum value the Disp7Seg will display.
 *    maxValue - The maximum value the Disp7Seg will display.
 *       pText - Pointer to the text label of the Disp7Seg.
 *     pScheme - Pointer to the style scheme used.
 *
 * Output: Returns the pointer to the object created.
 *
 * Side Effects: none
 *
 ********************************************************************/
DISP7SEG *D7Create(
        uint16_t ID,
        int16_t left,
        int16_t top,
        int16_t right,
        int16_t bottom,
        uint16_t state,
        uint32_t Value,
        uint8_t NoOfDigits,
        uint8_t DotPos,
        uint8_t Thickness,
        GFX_GOL_OBJ_SCHEME *pScheme
);

/*********************************************************************
 * Function: uint16_t D7TranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Overview: This function evaluates the message from a user if the
 *	     message will affect the object or not. The table below enumerates the translated
 *	     messages for each event of the touch screen and keyboard inputs.
 *
 *	<TABLE>
 *    	Translated Message   Input Source  Events         	Description
 *     	##################   ############  ######         	###########
 *     	MTR_MSG_SET          System        EVENT_SET            If event set occurs and the Disp7Seg ID is sent in parameter 1.
 *	OBJ_MSG_INVALID	     Any           Any	                If the message did not affect the object.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: pDisp7Seg  - The pointer to the o                SizeRecalc()
bject where the message will be
 *				 evaluated to check if the message will affect the object.
 *        pMsg  - Pointer to the message struct containing the message from
 *        		 the user interface.
 *
 * Output: Returns the translated message depending on the received GOL message:
 *		  - MTR_MSG_SET - Disp7Seg ID is given in parameter 1 for a TYPE_SYSTEM message.
 *         - OBJ_MSG_INVALID - Disp7Seg is not affected.
 *
 * Side Effects: none
 *
 ********************************************************************/
GFX_GOL_TRANSLATED_ACTION GFX_D7ActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
 * Function: SgMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 * Overview: This function performs the actual state change
 *			based on the translated message given. Disp7Seg value is set
 *			based on parameter 2 of the message given. The following state changes
 *			are supported:
 *	<TABLE>
 *    	Translated Message   Input Source  Set/Clear State Bit		Description
 *     	##################   ############  ######     				###########
 *     	MTR_MSG_SET          System  	   Set MTR_DRAW_UPDATE  	Disp7Seg will be redrawn to update the needle position and value displayed.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: translatedMsg - The translated message.
 *        pDisp7Seg          - The pointer to the object whose state will be modified.
 *        pMsg          - The pointer to the GOL message.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void GFX_D7ActionSet(GFX_GOL_TRANSLATED_ACTION translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
 * Macros:  SgGetVal(pDisp7Seg)
 *
 * Overview: This macro returns the current value of the Disp7Seg.
 *			Value is always in the minValue-maxValue range inclusive.
 *
 * PreCondition: none
 *
 * Input: pDisp7Seg - Pointer to the object.
 *
 * Output: Returns current value of the Disp7Seg.
 *
 * Side Effects: none
 *
 ********************************************************************/
#define D7GetVal(pDisp7Seg) ((pDisp7Seg)->value)

/*********************************************************************
 * Function: SgSetVal(Disp7Seg *pDisp7Seg, int16_t newVal)
 *
 * Overview: This function sets the value of the Disp7Seg to the passed
 *			newVal. newVal is checked to be in the minValue-maxValue
 *			range inclusive. If newVal is not in the range, minValue
 *			maxValue is assigned depending on the given newVal
 *			if less than minValue or above maxValue.
 *
 * PreCondition: none
 *
 * Input: pDisp7Seg   - The pointer to the object.
 *        newVal - New value to be set for the Disp7Seg.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void D7SetVal(DISP7SEG *pDisp7Seg, int16_t newVal);

/*********************************************************************
 * Macros:  D7IncVal(pDisp7Seg, deltaValue)
 *
 * Overview: This macro is used to directly increment the value.
 *
 * PreCondition: none
 *
 * Input: pDisp7Seg - Pointer to the object.
 *		 deltaValue - Number to be added to the current Disp7Seg value.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#define D7IncVal(pDisp7Seg, deltaValue)  D7SetVal(pDisp7Seg, ((pDisp7Seg)->value + deltaValue))

/*********************************************************************
 * Macros:  D7DecVal(pDisp7Seg, deltaValue)
 *
 * Overview: This macro is used to directly decrement the value.
 *
 * PreCondition: none
 *
 * Input: pDisp7Seg - Pointer to the object.
 *        deltaValue - Number to be subtracted to the current Disp7Seg value.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#define D7DecVal(pDisp7Seg, deltaValue)  MtrSetVal(pDisp7Seg, ((pDisp7Seg)->value - deltaValue))

GFX_STATUS D7Draw(void *pObj);


#endif // _Disp7Seg_H
