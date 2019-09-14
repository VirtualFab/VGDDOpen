// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// SuperGauge
// *****************************************************************************
// FileName:        supergauge.h
// Processor:       PIC24F, PIC24H, dsPIC, PIC32
// Compiler:        MPLAB C30/XC16, MPLAB C32/XC32
// Company:         VirtualFab, parts from Microchip Technology Incorporated
//
// VirtualFab's Software License Agreement:
// Copyright 2013-2016 Virtualfab - All rights reserved.
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
// *****************************************************************************
#ifndef _SUPERGAUGE_H
#define _SUPERGAUGE_H

#include "gfx/gfx_gol.h"
#include "system.h"
#include <stdlib.h>
#include <stdint.h>

/*********************************************************************
 * Object States Definition:
 *********************************************************************/
#define SG_DISABLED         0x0002      // Bit for disabled state.

#define SG_POINTER_THICK    0x0004      // Pointer line drawn as THICK_LINE
#define SG_NOPANEL          0x0020      // Bit to indicate bacground panel is disabled.

#define SG_DRAW_UPDATE      0x1000      // Bit to indicate an update only.
#define SG_DRAW             0x4000      // Bit to indicate object must be redrawn.
#define SG_HIDE             0x8000      // Bit to indicate object must be removed from screen.

/*********************************************************************
 * Used Constants
 *********************************************************************/
//#define RADIAN      1144            // Radian definition. Equivalent to sine(1) * 2^16.
//#define PIIOVER2    102944          // The constant Pii divided by two (pii/2).

// SuperGauge types
typedef enum {
    SUPERGAUGE_FULL360,
    SUPERGAUGE_HALF180UP,
    SUPERGAUGE_HALF180DOWN,
} GAUGETYPE;

// SuperGauge Pointer types
typedef enum {
    SG_POINTER_NORMAL, // Pointer filled with colour
    SG_POINTER_3D, // Pointer filled with 2 colours, 3D effect
    SG_POINTER_WIREFRAME, // Pointer only contoured
    SG_POINTER_NEEDLE, // Pointer as needle
    SG_POINTER_PIE, // Draw Pie instead of pointer
    SG_POINTER_COLOUREDPIE, // Draw Pie instead of pointer, coloured with segments colours
    SG_POINTER_FLOATINGBIT // Pointer as Arc bit near scale, no pointer actually
} SGPOINTERTYPE;

// SuperGauge state machine statuses
typedef enum {
    SG_STATE_IDLE,
    SG_STATE_DIAL_DRAW,
    SG_STATE_RIM_DRAW,
    SG_STATE_SEGMENTS_DRAW_SETUP,
    SG_STATE_SEGMENTS_DRAW,
    SG_STATE_SEGMENT_DRAW,
    SG_STATE_SCALE_COMPUTE,
    //SG_STATE_SCALE_LABEL_DRAW,
    SG_STATE_SCALE_DRAW,
    SG_STATE_CENTER_DRAW,
    SG_STATE_TEXT_DRAW,
    //SG_STATE_TEXT_DRAW_RUN,
    SG_STATE_POINTER_ERASE,
    SG_STATE_POINTER_DRAW,
    //SG_STATE_VALUE_ERASE,
    SG_STATE_VALUE_DRAW,
    //SG_STATE_VALUE_DRAW_RUN,
} SG_DRAW_STATES;

#define OBJ_SUPERGAUGE GFX_GOL_UNKNOWN_TYPE+1000
#define SG_MSG_SET GFX_GOL_OBJECT_ACTION_INVALID+1000
#define SG_MSG_TOUCHSCREEN SG_MSG_SET+1
#define SCALECHARCOUNT  4           // Defines how many characters will be allocated for the
                                    // scale labels. Use this define in accordance to
                                    // the maxValue-minValue. Example: if maxValue-minValue = 500, SCALECHARCOUNT
                                    // should be 3. if maxValue-minValue = 90, SCALECHARCOUNT = 2
                                    // You must include the decimal point if this
                                    // feature is enabled (see MTR_ACCURACY state bit).

// *********************************************************************
// * Overview: Defines the parameters required for a SuperGauge Object.
// *           Depending on the type selected the SuperGauge is drawn with
// *           the defined shape parameters and values set on the
// *           given fields.
// *
// **********************************************************************
typedef struct {
    GFX_GOL_OBJ_HEADER hdr; // Generic header for all Objects (see GFX_GOL_OBJ_HEADER).
    GFX_XCHAR *pDialText; // The text label of the SuperGauge.
    int16_t value; // Current value of the SuperGauge.
    int16_t minValue; // minimum value the SuperGauge can display
    int16_t maxValue; // maximum value the SuperGauge can display (range is maxValue - minValue)
    int16_t newValue; // Value to reach
    int16_t lastValue; // Last Value displayed
    GAUGETYPE GaugeType;
    SGPOINTERTYPE PointerType;
    int16_t MainAngleFrom;
    int16_t MainAngleTo;
    int16_t AngleFrom;
    int16_t AngleTo;
    uint8_t DialScaleNumDivisions;
    uint8_t DialScaleNumSubDivisions;
    int16_t DialTextOffsetX;
    int16_t DialTextOffsetY;
    int16_t PointerSize;
    uint8_t PointerCenterSize;
    uint8_t DigitsNumber;
    uint8_t DigitsSizeX;
    uint8_t DigitsSizeY;
    int16_t DigitsOffsetX;
    int16_t DigitsOffsetY;
    int16_t xCenter; // The x coordinate center position. This is computed automatically.
    int16_t yCenter; // The y coordinate center position. This is computed automatically.
    int16_t radius; // Radius of the SuperGauge, also defines the needle length.
    uint8_t BorderWidth;

    int16_t xLastPos; // The current x position of the needle. This is computed automatically.
    int16_t yLastPos; // The current y position of the needle. This is computed automatically.
    uint8_t SegmentsCount; // Number of segments used
    void *Segments; // Coloured Segment Information

    // The following three points define three fonts used in SuperGauge widget, they can be different from the scheme font.
    // Note that the sizes of these fonts are not checked with the SuperGauge dimension. In cases where font sizes are
    // larger than the SuperGauge dimension, some overlaps will occur.
    //    void *pTitleFont; // Pointer to the font used in the title of the SuperGauge
    void *pDialScaleFont; // Pointer to the font used in the current reading (if displayed) of the SuperGauge

    int16_t OuterAngleFrom;
    int16_t OuterAngleTo;
    int16_t RectImgWidth;
    int16_t RectImgHeight;
    int16_t RectImgVirtualWidth;
    int16_t RectImgVirtualHeight;
    int16_t PointerWidth;
    int16_t DrawStep;
    int16_t DrawRadius;
    int16_t degAngle;
    int16_t subDivHeight;
    int16_t subIncrDeg;
    SG_DRAW_STATES state;
} SUPERGAUGE;

typedef struct {
    uint16_t StartValue;
    uint16_t EndValue;
    uint16_t SegmentColour;
} SgSegment;


/*********************************************************************
 * Function: SuperGauge  *SgCreate(
 *              uint16_t ID, int16_t left, int16_t top, int16_t right, int16_t bottom,
 *              uint16_t state, void *Params,
 *              void *pDialScaleFont, GFX_XCHAR *pDialText,
 *              void *pSegments, GFX_GOL_OBJ_SCHEME *pScheme)
 *
 * Overview: This function creates a SuperGauge object with the parameters given.
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
 *       pText - Pointer to the text label of the SuperGauge.
 *     pScheme - Pointer to the style scheme used.
 *
 * Output: Returns the pointer to the object created.
 *
 * Side Effects: none
 *
 ********************************************************************/
SUPERGAUGE *SgCreate(
        uint16_t ID,
        int16_t left,
        int16_t top,
        int16_t right,
        int16_t bottom,
        uint16_t state,
        void *pDialScaleFont,
        GFX_XCHAR *pDialText,
        uint8_t SegmentsNum,
        void *pSegments,
        void *Params,
        GFX_GOL_OBJ_SCHEME *pScheme
        );

void SgCalcDimensions(SUPERGAUGE *pSGauge);

/*********************************************************************
 * Function: uint16_t SgTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Overview: This function evaluates the message from a user if the
 *	     message will affect the object or not. The table below enumerates the translated
 *	     messages for each event of the touch screen and keyboard inputs.
 *
 *	<TABLE>
 *    	Translated Message   Input Source  Events         	Description
 *     	##################   ############  ######         	###########
 *     	MTR_MSG_SET          System        EVENT_SET            If event set occurs and the SuperGauge ID is sent in parameter 1.
 *	OBJ_MSG_INVALID	     Any           Any	                If the message did not affect the object.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: pSGauge  - The pointer to the object where the message will be
 *				 evaluated to check if the message will affect the object.
 *        pMsg  - Pointer to the message struct containing the message from
 *        		 the user interface.
 *
 * Output: Returns the translated message depending on the received GOL message:
 *		  - MTR_MSG_SET - SuperGauge ID is given in parameter 1 for a TYPE_SYSTEM message.
 *         - OBJ_MSG_INVALID - SuperGauge is not affected.
 *
 * Side Effects: none
 *
 ********************************************************************/
uint16_t GFX_SgActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
 * Function: SgMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 * Overview: This function performs the actual state change
 *			based on the translated message given. SuperGauge value is set
 *			based on parameter 2 of the message given. The following state changes
 *			are supported:
 *	<TABLE>
 *    	Translated Message   Input Source  Set/Clear State Bit		Description
 *     	##################   ############  ######     				###########
 *     	MTR_MSG_SET          System  	   Set MTR_DRAW_UPDATE  	SuperGauge will be redrawn to update the needle position and value displayed.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: translatedMsg - The translated message.
 *        pSGauge          - The pointer to the object whose state will be modified.
 *        pMsg          - The pointer to the GOL message.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void GFX_SgActionSet(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
 * Macros:  SgGetVal(pSGauge)
 *
 * Overview: This macro returns the current value of the SuperGauge.
 *			Value is always in the minValue-maxValue range inclusive.
 *
 * PreCondition: none
 *
 * Input: pSGauge - Pointer to the object.
 *
 * Output: Returns current value of the SuperGauge.
 *
 * Side Effects: none
 *
 ********************************************************************/
#define SgGetVal(pSGauge) ((pSGauge)->value)

/*********************************************************************
 * Function: SgSetVal(SuperGauge *pSGauge, int16_t newVal)
 *
 * Overview: This function sets the value of the SuperGauge to the passed
 *			newVal. newVal is checked to be in the minValue-maxValue
 *			range inclusive. If newVal is not in the range, minValue
 *			maxValue is assigned depending on the given newVal
 *			if less than minValue or above maxValue.
 *
 * PreCondition: none
 *
 * Input: pSGauge   - The pointer to the object.
 *        newVal - New value to be set for the SuperGauge.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void SgSetVal(SUPERGAUGE *pSGauge, int16_t newVal);

/*********************************************************************
 * Macros:  SgIncVal(pSGauge, deltaValue)
 *
 * Overview: This macro is used to directly increment the value.
 *
 * PreCondition: none
 *
 * Input: pSGauge - Pointer to the object.
 *		 deltaValue - Number to be added to the current SuperGauge value.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#define SgIncVal(pSGauge, deltaValue)  SgSetVal(pSGauge, ((pSGauge)->value + deltaValue))

/*********************************************************************
 * Macros:  SgDecVal(pSGauge, deltaValue)
 *
 * Overview: This macro is used to directly decrement the value.
 *
 * PreCondition: none
 *
 * Input: pSGauge - Pointer to the object.
 *        deltaValue - Number to be subtracted to the current SuperGauge value.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#define SgDecVal(pSGauge, deltaValue)  SgSetVal(pSGauge, ((pSGauge)->value - deltaValue))

/*********************************************************************
 * Macros:  MtrSetScaleColors(pSGauge, arc1, arc2, arc3, arc4, arc5, arc6)
 *								{	pSGauge->arcColor6=arc6;
 *									pSGauge->arcColor5=arc5;
 *									pSGauge->arcColor4=arc4;
 *									pSGauge->arcColor3=arc3;
 *									pSGauge->arcColor2=arc2;
 *									pSGauge->arcColor1=arc1;	}
 *
 * Overview: Scale colors can be used to highlight values of the SuperGauge.
 *           User can set these colors to define the arc colors and scale colors.
 *			This also sets the color of the SuperGauge value when displayed. Limitation is that
 *			color settings are set to the following angles:
 *			Color Boundaries		Type Whole		Type Half		Type Quarter
 *			Arc 6					225  to 180		not used		not used
 *			Arc 5					179  to 135		179  to 135 	not used
 *			Arc 4					134  to  90		134  to  90		not used
 *			Arc 3					 89  to  45		 89  to  45		89  to  45
 *			Arc 2					 44  to   0		 44  to   0		44  to   0
 *			Arc 1					-45  to  -1		not used 		not used
 *			As the SuperGauge is drawn colors are changed depending on the
 *			angle of the scale and label being drawn.
 *
 * PreCondition: The object must be created (using MtrCreate()) before
 *				a call to this macro is performed.
 *
 * Input: pSGauge - Pointer to the object.
 *		 arc1 - color for arc 1.
 *		 arc2 - color for arc 2.
 *		 arc3 - color for arc 3.
 *		 arc4 - color for arc 4.
 *		 arc5 - color for arc 5.
 *		 arc6 - color for arc 6.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
/*#define MtrSetScaleColors(pSGauge, arc1, arc2, arc3, arc4, arc5, arc6) \
    {                                                                   \
        pSGauge->arcColor6 = arc6;                                         \
        pSGauge->arcColor5 = arc5;                                         \
        pSGauge->arcColor4 = arc4;                                         \
        pSGauge->arcColor3 = arc3;                                         \
        pSGauge->arcColor2 = arc2;                                         \
        pSGauge->arcColor1 = arc1;                                         \
    }*/

/*********************************************************************
 * Function: uint16_t SuperGaugeDraw(void *pObj)
 *
 * Overview: This function renders the object on the screen using
 * 			the current parameter settings. Location of the object is
 *			determined by the left, top, right and bottom parameters.
 *			The colors used are dependent on the state of the object.
 *			The font used is determined by the style scheme set.
 *
 *			Depending on the defined settings, value of the SuperGauge
 *			will displayed or hidden. Displaying the value will require
 *			a little bit more rendering time depending on the size
 *			of the SuperGauge and font used.
 *
 *			When rendering objects of the same type, each object
 *			must be rendered completely before the rendering of the
 *			next object is started. This is to avoid incomplete
 *			object rendering.
 *
 * PreCondition: Object must be created before this function is called.
 *
 * Input: pSGauge - Pointer to the object to be rendered.
 *
 * Output: Returns the status of the drawing
 *		  - 1 - If the rendering was completed and
 *		  - 0 - If the rendering is not yet finished.
 *		  Next call to the function will resume the
 *		  rendering on the pending drawing state.
 *
 * Example:
 *	See MtrCreate() example.
 *
 * Side Effects: none
 *
 ********************************************************************/
uint16_t SgDraw(void *pObj);

/*********************************************************************
 * Macro: SuperGaugeSetTitleFont(pSGauge, pNewFont)   (((SUPERGAUGE*)pSGauge)->pTitleFont = pNewFont)
 *
 * Overview: This function sets the font of title.
 *
 * PreCondition: Font must be created before this function is called.
 *
 * Input: pSGauge - Pointer to the object.
 *		 pNewFont - Pointer to the new font used for the title.
 *
 * Output: N/A
 *
 * Side Effects: none
 *
 ********************************************************************/
#define SuperGaugeSetTitleFont(pSGauge, pNewFont) (((SUPERGAUGE *)pSGauge)->pTitleFont = pNewFont)

/*********************************************************************
 * Macro: SgSetValueFont(pSGauge, pNewFont)   (((SUPERGAUGE*)pSGauge)->pValueFont = pNewFont)
 *
 * Overview: This function sets the font of value.
 *
 * PreCondition: Font must be created before this function is called.
 *
 * Input: pSGauge - Pointer to the object.
 *		 pNewFont - Pointer to the new font used for the value.
 *
 * Output: N/A
 *
 * Side Effects: none
 *
 ********************************************************************/
#define SgSetValueFont(pSGauge, pNewFont) (((SUPERGAUGE *)pSGauge)->pValueFont = pNewFont)
uint16_t FreeArc(int16_t xc, int16_t yc, int16_t r1, int16_t r2, int16_t AngleFrom, int16_t AngleTo);

#endif // _SuperGauge_H
