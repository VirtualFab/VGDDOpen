// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// BarGraph
// *****************************************************************************
// FileName:        bargraph.h
// Processor:       PIC24F, PIC24H, dsPIC, PIC32
// Compiler:        MPLAB C30, MPLAB C32
// Company:         VirtualFab, parts from Microchip Technology Incorporated
//
// VirtualFab Software License Agreement:
// Copyright 2013-2016 Virtualfab - All rights reserved.
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
// Copyright 2013 Microchip Technology Inc.  All rights reserved.
// Microchip licenses to you the right to use, modify, copy and distribute
// Software only when embedded on a Microchip microcontroller or digital
// signal controller, which is integrated into your product or third party
// product (pursuant to the sublicense terms in the accompanying license
// agreement).
//
// You should refer to the license agreement accompanying this Software
// for additional information regarding your rights and obligations.
//
// SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
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
//  2013/09/25	Initial Release
// *****************************************************************************
#ifndef _BARGRAPH_H
#define _BARGRAPH_H

//#include "Graphics/GOL.h"
//#include "GenericTypeDefs.h"
//#include "Graphics/DisplayDriver.h"

/*********************************************************************
 * Object States Definition:
 *********************************************************************/
#define BG_DISABLED       0x0002      // Bit for disabled state.

#define BG_VERTICAL       0x0004      // Bit to indicate vertical BarGraph. Default: horizontal

#define BG_FRAME          0x0010      // Bit to indicate frame is to be drawn around the BarGraph.
#define BG_DRAW_ANIMATING 0x0100      // Bit to indicate that the BarGraph is being drawn one block after the other
#define BG_DRAW_UPDATE    0x1000      // Bit to indicate an update. Only blocks are drawn.
#define BG_DRAWALL        0x4000      // Bit to indicate object must be completely redrawn.
#define BG_HIDE           0x8000      // Bit to indicate object must be removed from screen.

// BarGraph types
typedef enum {
	BARGRPHSTYLE_SOLID,
	BARGRPHSTYLE_BLOCK,
	BARGRPHSTYLE_WIREFRAME
} BARGRAPHSTYLE;

// State machine states
typedef enum {
    BG_STATE_IDLE,
    BG_STATE_DRAW_BACKGROUND,
    BG_STATE_DRAW_SCALE,
    BG_STATE_ERASE_BLOCKS,
    BG_STATE_DRAW_BLOCKS
} BG_DRAW_STATES;

// BarGraph Segment structure
typedef struct {
    WORD StartValue;
    WORD EndValue;
    WORD SegmentColour;
} BgSegment;

#define OBJ_BARGRAPH OBJ_UNKNOWN+1011
#define BG_MSG_SET OBJ_MSG_PASSIVE+1011
#define BG_MSG_TOUCHSCREEN BG_MSG_SET+1

/*********************************************************************
 * Overview: Defines the parameters required for a BarGraph Object.
 *           Depending on the type selected the BarGraph is drawn with
 *           the defined shape parameters and values set on the
 *           given fields.
 *
 *********************************************************************/
typedef struct {
    OBJ_HEADER hdr;       // Generic header for all Objects (see OBJ_HEADER).
    INT16 currentValue;   // Current value of the BarGraph.
    INT16 minValue;       // minimum value the BarGraph can display
    INT16 maxValue;       // maximum value the BarGraph can display (range is maxValue - minValue)
    INT16 newValue;       // Value to reach
    INT16 previousValue;  // Last Value displayed
    BYTE BarSpeed;        // Bar speed
    BARGRAPHSTYLE Style;  // One of the BARGRAPHSTYLE styles available
    INT16 Divisions;      // Number of blocks to draw
    INT16 ScaleDivisions; // Number of divisions for the scale
    BYTE SegmentsCount;   // Number of coloured segments used
    void *Segments;       // Pointer to a BgSegment[SegmentsCount] array for Coloured Segments Information

    BG_DRAW_STATES state; // used to store each BarGraph's state

    INT16 RectImgWidth;
    INT16 RectImgHeight;
    INT16 intTextHeight;
    INT16 intTextWidth;
    INT16 intBarRectWorH;
    INT16 intScaleWorH;
    INT16 intScaleInterval;
    INT16 intBarInterval;

} BARGRAPH;


/*********************************************************************
 * Function: BarGraph  *BgCreate
 *
 * Overview: This function creates a BarGraph object with the parameters given.
 *           It automatically attaches the new object into a global linked list of
 *           objects and returns the address of the object.
 *
 * PreCondition: none
 *
 * Output: Returns the pointer to the object created.
 *
 * Side Effects: none
 *
 ********************************************************************/
BARGRAPH *BgCreate(
        WORD ID,                    // Unique user defined ID for the object instance
        INT16 left,                 // Left most position of the object
        INT16 top,                  // Top most position of the object
        INT16 right,                // Right most position of the object
        INT16 bottom,               // Bottom most position of the object
        WORD state,                 // Sets the initial state of the object
	BYTE SegmentsCount,         // Number of coloured segments used
        void *Segments,             // Pointer to a BgSegment[SegmentsCount] array for Coloured Segments Information
        void *Params,               // Rest of the widget's parameters
        GOL_SCHEME *pScheme         // Pointer to the style scheme
        );

void BgCalcDimensions(BARGRAPH *pBarGraph);

/*********************************************************************
 * Function: WORD BgTranslateMsg(void *pObj, GOL_MSG *pMsg)
 *
 * Overview: This function evaluates the message from a user if the
 *	     message will affect the object or not. The table below enumerates the translated
 *	     messages for each event of the touch screen and keyboard inputs.
 *
 *	<TABLE>
 *    	Translated Message   Input Source  Events         	Description
 *     	##################   ############  ######         	###########
 *     	BG_MSG_SET           System        EVENT_SET        If event set occurs and the BarGraph ID is sent in parameter 1.
 *      OBJ_MSG_INVALID      Any           Any              If the message did not affect the object.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: pBarGraph  - The pointer to the object where the message will be
 *                    evaluated to check if the message will affect the object.
 *        pMsg      - Pointer to the message struct containing the message from
 *                    the user interface.
 *
 * Output: Returns the translated message depending on the received GOL message:
 *         - BG_MSG_SET - BarGraph ID is given in parameter 1 for a TYPE_SYSTEM message.
 *         - OBJ_MSG_INVALID - BarGraph is not affected.
 *
 * Side Effects: none
 *
 ********************************************************************/
WORD BgTranslateMsg(void *pObj, GOL_MSG *pMsg);

/*********************************************************************
 * Function: BgMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG* pMsg)
 *
 * Overview: This function performs the actual state change
 *           based on the translated message given. BarGraph value is set
 *           based on parameter 2 of the message given. The following state changes
 *           are supported:
 *	<TABLE>
 *    	Translated Message   Input Source  Set/Clear State Bit		Description
 *     	##################   ############  ######                   ###########
 *     	BG_MSG_SET           System        Set BG_DRAW_UPDATE       BarGraph will be redrawn to update blocks.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: translatedMsg - The translated message.
 *        pBarGraph      - The pointer to the object whose state will be modified.
 *        pMsg          - The pointer to the GOL message.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void BgMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG *pMsg);

/*********************************************************************
 * Macros:  BgGetVal(pBarGraph)
 *
 * Overview: This macro returns the current value of the BarGraph.
 *			Value is always in the minValue-maxValue range inclusive.
 *
 * PreCondition: none
 *
 * Input: pBarGraph - Pointer to the object.
 *
 * Output: Returns current value of the BarGraph.
 *
 * Side Effects: none
 *
 ********************************************************************/
#define BgGetVal(pBarGraph) ((pBarGraph)->value)

/*********************************************************************
 * Function: BgSetVal(BarGraph *pBarGraph, SHORT newVal)
 *
 * Overview: This function sets the value of the BarGraph to the passed
 *			newVal. newVal is checked to be in the minValue-maxValue
 *			range inclusive. If newVal is not in the range, minValue
 *			maxValue is assigned depending on the given newVal
 *			if less than minValue or above maxValue.
 *
 * PreCondition: none
 *
 * Input: pBarGraph   - The pointer to the object.
 *        newVal - New value to be set for the BarGraph.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void BgSetVal(BARGRAPH *pBarGraph, SHORT newVal);

/*********************************************************************
 * Macros:  BgIncVal(pBarGraph, deltaValue)
 *
 * Overview: This macro is used to directly increment the value.
 *
 * PreCondition: none
 *
 * Input: pBarGraph - Pointer to the object.
 *		 deltaValue - Number to be added to the current BarGraph value.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#define BgIncVal(pBarGraph, deltaValue)  VuSetVal(pBarGraph, ((pBarGraph)->value + deltaValue))

/*********************************************************************
 * Macros:  BgDecVal(pBarGraph, deltaValue)
 *
 * Overview: This macro is used to directly decrement the value.
 *
 * PreCondition: none
 *
 * Input: pBarGraph - Pointer to the object.
 *        deltaValue - Number to be subtracted to the current BarGraph value.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#define BgDecVal(pBarGraph, deltaValue)  VuSetVal(pBarGraph, ((pBarGraph)->value - deltaValue))


/*********************************************************************
 * Function: WORD BgDraw(void *pObj)
 *
 * Overview: This function renders the object on the screen using
 * 			the current parameter settings. Location of the object is
 *			determined by the left, top, right and bottom parameters.
 *			The colors used are dependent on the state of the object.
 *			The font used is determined by the style scheme set.
 *
 *			Depending on the defined settings, value of the BarGraph
 *			will displayed or hidden. Displaying the value will require
 *			a little bit more rendering time depending on the size
 *			of the BarGraph and font used.
 *
 *			When rendering objects of the same type, each object
 *			must be rendered completely before the rendering of the
 *			next object is started. This is to avoid incomplete
 *			object rendering.
 *
 * PreCondition: Object must be created before this function is called.
 *
 * Input: pBarGraph - Pointer to the object to be rendered.
 *
 * Output: Returns the status of the drawing
 *		  - 1 - If the rendering was completed and
 *		  - 0 - If the rendering is not yet finished.
 *		  Next call to the function will resume the
 *		  rendering on the pending drawing state.
 *
 * Example:
 *	See BgCreate() example.
 *
 * Side Effects: none
 *
 ********************************************************************/
WORD BgDraw(void *pObj);


#endif // _BARGRAPH_H
