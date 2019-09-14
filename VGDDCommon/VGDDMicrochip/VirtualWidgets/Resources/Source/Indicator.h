// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// Indicator
// *****************************************************************************
// FileName:        Indicator.h
// Processor:       PIC24F, PIC24H, dsPIC, PIC32
// Compiler:        MPLAB C30, MPLAB C32
// Company:         VirtualFab, parts from Microchip Technology Incorporated
//
// VirtualFab Software License Agreement:
// Copyright 2012 Virtualfab - All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, is not permitted
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
// *****************************************************************************
#ifndef _INDICATOR_H
#define _INDICATOR_H

#include "Graphics/GOL.h"
#include "GenericTypeDefs.h"
#include "Graphics/DisplayDriver.h"

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

#define OBJ_INDICATOR OBJ_UNKNOWN+1020
#define IND_MSG_SET OBJ_MSG_PASSIVE+1020
#define IND_MSG_TOUCHSCREEN IND_MSG_SET+1

/*********************************************************************
 * Overview: Defines the parameters required for a Indicator Object.
 *           Depending on the type selected the Indicator is drawn with
 *           the defined shape parameters and values set on the
 *           given fields.
 *
 *********************************************************************/
typedef struct {
    OBJ_HEADER  hdr;            // Generic header for all Objects (see OBJ_HEADER).
    DWORD       Value;          // Current value
    BYTE        Style;          // Style
    WORD        IndicatorColour;// Colour
    XCHAR       *pText;         // The pointer to text used.
} INDICATOR;

/*********************************************************************
 * Function: Indicator  *IndCreate(
 *                      WORD ID,SHORT left,SHORT top,SHORT right,SHORT bottom,
 *                      WORD state,DWORD Value,BYTE NoOfDigits,BYTE DotPos,BYTE Thickness,GOL_SCHEME *pScheme
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
        WORD ID,
        SHORT left,
        SHORT top,
        SHORT right,
        SHORT bottom,
        WORD state,
        DWORD Value,
        BYTE Style,
        WORD IndicatorColour,
        XCHAR *pText,
        GOL_SCHEME *pScheme
);

/*********************************************************************
 * Function: WORD IndTranslateMsg(void *pObj, GOL_MSG *pMsg)
 *
 * Overview: This function evaluates the message from a user if the
 *	     message will affect the object or not. The table below enumerates the translated
 *	     messages for each event of the touch screen and keyboard inputs.
 *
 *	<TABLE>
 *    	Translated Message   Input Source  Events         	Description
 *     	##################   ############  ######         	###########
 *     	MTR_MSG_SET          System        EVENT_SET            If event set occurs and the Indicator ID is sent in parameter 1.
 *	OBJ_MSG_INVALID	     Any           Any	                If the message did not affect the object.
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
 *         - OBJ_MSG_INVALID - Indicator is not affected.
 *
 * Side Effects: none
 *
 ********************************************************************/
WORD IndTranslateMsg(void *pObj, GOL_MSG *pMsg);

/*********************************************************************
 * Function: IndMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG* pMsg)
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
void IndMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG *pMsg);

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
 * Function: IndSetVal(INDICATOR *pIndicator, SHORT newVal)
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
void IndSetVal(INDICATOR *pIndicator, SHORT newVal);

/*********************************************************************
 * Function: IndSetColour(INDICATOR *pIndicator, SHORT newColour)
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
void IndSetColour(INDICATOR *pIndicator, WORD newColour);

WORD IndDraw(void *pObj);

#endif // _Indicator_H
