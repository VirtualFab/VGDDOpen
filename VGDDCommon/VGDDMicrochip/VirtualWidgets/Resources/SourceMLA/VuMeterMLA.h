// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// VuMeter
// *****************************************************************************
// FileName:        vumeter.h
// Processor:       PIC24F, PIC24H, dsPIC, PIC32
// Compiler:        MPLAB C30, MPLAB C32
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
//  2013/08/18	Start of Developing
// *****************************************************************************
#ifndef _VUMETER_H
#define _VUMETER_H

#include "gfx/gfx_gol.h"
#include <stdlib.h>
#include <stdint.h>

/*********************************************************************
 * Object States Definition:
 *********************************************************************/
#define VU_DISABLED    0x0002      // Bit for disabled state.

#define VU_POINTER_THICK        0x0004  // Pointer line drawn as THICK_LINE

#define VU_FRAME        0x0010      // Bit to indicate frame is to be drawn around the VuMeter.
#define VU_DRAW_ANIMATING 0x0100      // Bit to indicate that needle is being animated and object needs a VU_DRAW_UPDATE.
#define VU_DRAW_UPDATE  0x1000      // Bit to indicate an update only.
#define VU_DRAWALL      0x4000      // Bit to indicate object must be redrawn.
#define VU_HIDE         0x8000      // Bit to indicate object must be removed from screen.

// VuMeter types
typedef enum {
    VU_POINTER_NORMAL, // Pointer filled with colour
    VU_POINTER_3D, // Pointer filled with 2 colours, 3D effect
    VU_POINTER_WIREFRAME, // Pointer only contoured
    VU_POINTER_NEEDLE, // Pointer as needle
} POINTERTYPE;

// State machine states
typedef enum {
    VU_STATE_IDLE,
    VU_STATE_BACKGROUND_DRAW,
    VU_STATE_POINTER_ERASE,
    VU_STATE_POINTER_DRAW
} VU_DRAW_STATES;

#define OBJ_VUMETER GFX_GOL_UNKNOWN_TYPE+1010
#define VU_MSG_SET GFX_GOL_OBJECT_ACTION_INVALID+1010
#define VU_MSG_TOUCHSCREEN VU_MSG_SET+1

/*********************************************************************
 * Overview: Defines the parameters required for a VuMeter Object.
 *           Depending on the type selected the VuMeter is drawn with
 *           the defined shape parameters and values set on the
 *           given fields.
 *
 *********************************************************************/
typedef struct {
    GFX_GOL_OBJ_HEADER hdr; // Generic header for all Objects (see GFX_GOL_OBJ_HEADER).
    int16_t currentValue; // Current value of the VuMeter.
    int16_t minValue; // minimum value the VuMeter can display
    int16_t maxValue; // maximum value the VuMeter can display (range is maxValue - minValue)
    int16_t newValue; // Value to reach
    int16_t previousValue; // Last Value displayed
    POINTERTYPE PointerType;
    int16_t AngleFrom; // Angle (0->360?) for minValue
    int16_t AngleTo; // Angle (AngleFrom->720?) for maxValue
    void  *pBitmap;   // Pointer to the bitmap for background scale
    int16_t PointerWidth; // Width of the pointer in pixels (not applicable for VU_POINTER_NEEDLE)
    int16_t PointerLength; // Length of the needle in pixels
    int16_t PointerStart; // Pixels from center where pointer starts
    int16_t PointerCenterOffsetX; // Pointer center X offset point (% of height). Use Negative values to put it outside the left edge.
    int16_t PointerCenterOffsetY; // Pointer center Y offset point (% of height). Use Negative values to put it below bottom edge.
    uint8_t PointerSpeed; // Speed of the pointer 0:realtime 8:slowest
    uint8_t BorderWidth;

    VU_DRAW_STATES state; // used to store each VUMeter's state
    int16_t Xcenter, Ycenter; // Used to store computed center position. Computed automatically.
    int16_t xo1, xo2, yo1, yo2; // Used to store last MaxArea, for background partial repainting

    int16_t xStart; // The current x position of the needle's vertex. Computed automatically
    int16_t yStart; // The current y position of the needle's vertex. Computed automatically
    int16_t AnimationIncrement; // The increment for needle movement. Computed automatically

    int16_t RectImgWidth;
    int16_t RectImgHeight;
    int16_t degAngle;

} VUMETER;


/*********************************************************************
 * Function: VuMeter  *VuCreate
 *
 * Overview: This function creates a VuMeter object with the parameters given.
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
VUMETER *VuCreate(
        uint16_t ID,                    // Unique user defined ID for the object instance
        int16_t left,                 // Left most position of the object
        int16_t top,                  // Top most position of the object
        int16_t right,                // Right most position of the object
        int16_t bottom,               // Bottom most position of the object
        uint16_t state,                 // Sets the initial state of the object
        void *Params,               // Rest of the widget's parameters
        void *pBitmap,              // Pointer to the bitmap that will be used as scale background
        GFX_GOL_OBJ_SCHEME *pScheme         // Pointer to the style scheme
        );

void VuCalcDimensions(VUMETER *pVuMeter);

/*********************************************************************
 * Function: uint16_t VuTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Overview: This function evaluates the message from a user if the
 *	     message will affect the object or not. The table below enumerates the translated
 *	     messages for each event of the touch screen and keyboard inputs.
 *
 *	<TABLE>
 *    	Translated Message   Input Source  Events         	Description
 *     	##################   ############  ######         	###########
 *     	VU_MSG_SET           System        EVENT_SET        If event set occurs and the VuMeter ID is sent in parameter 1.
 *      OBJ_MSG_INVALID      Any           Any              If the message did not affect the object.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: pVuMeter  - The pointer to the object where the message will be
 *                    evaluated to check if the message will affect the object.
 *        pMsg      - Pointer to the message struct containing the message from
 *                    the user interface.
 *
 * Output: Returns the translated message depending on the received GOL message:
 *         - VU_MSG_SET - VuMeter ID is given in parameter 1 for a TYPE_SYSTEM message.
 *         - OBJ_MSG_INVALID - VuMeter is not affected.
 *
 * Side Effects: none
 *
 ********************************************************************/
uint16_t GFX_VuActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
 * Function: VuMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 * Overview: This function performs the actual state change
 *           based on the translated message given. VuMeter value is set
 *           based on parameter 2 of the message given. The following state changes
 *           are supported:
 *	<TABLE>
 *    	Translated Message   Input Source  Set/Clear State Bit		Description
 *     	##################   ############  ######                       ###########
 *     	VU_MSG_SET           System        Set VU_DRAW_UPDATE           VuMeter will be redrawn to update the needle position.
 *	</TABLE>
 *
 * PreCondition: none
 *
 * Input: translatedMsg - The translated message.
 *        pVuMeter      - The pointer to the object whose state will be modified.
 *        pMsg          - The pointer to the GOL message.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void GFX_VuActionSet(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
 * Macros:  VuGetVal(pVuMeter)
 *
 * Overview: This macro returns the current value of the VuMeter.
 *			Value is always in the minValue-maxValue range inclusive.
 *
 * PreCondition: none
 *
 * Input: pVuMeter - Pointer to the object.
 *
 * Output: Returns current value of the VuMeter.
 *
 * Side Effects: none
 *
 ********************************************************************/
#define VuGetVal(pVuMeter) ((pVuMeter)->value)

/*********************************************************************
 * Function: VuSetVal(VuMeter *pVuMeter, int16_t newVal)
 *
 * Overview: This function sets the value of the VuMeter to the passed
 *			newVal. newVal is checked to be in the minValue-maxValue
 *			range inclusive. If newVal is not in the range, minValue
 *			maxValue is assigned depending on the given newVal
 *			if less than minValue or above maxValue.
 *
 * PreCondition: none
 *
 * Input: pVuMeter   - The pointer to the object.
 *        newVal - New value to be set for the VuMeter.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void VuSetVal(VUMETER *pVuMeter, int16_t newVal);

/*********************************************************************
 * Macros:  VuIncVal(pVuMeter, deltaValue)
 *
 * Overview: This macro is used to directly increment the value.
 *
 * PreCondition: none
 *
 * Input: pVuMeter - Pointer to the object.
 *		 deltaValue - Number to be added to the current VuMeter value.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#define VuIncVal(pVuMeter, deltaValue)  VuSetVal(pVuMeter, ((pVuMeter)->value + deltaValue))

/*********************************************************************
 * Macros:  VuDecVal(pVuMeter, deltaValue)
 *
 * Overview: This macro is used to directly decrement the value.
 *
 * PreCondition: none
 *
 * Input: pVuMeter - Pointer to the object.
 *        deltaValue - Number to be subtracted to the current VuMeter value.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#define VuDecVal(pVuMeter, deltaValue)  VuSetVal(pVuMeter, ((pVuMeter)->value - deltaValue))


/*********************************************************************
 * Function: uint16_t VuDraw(void *pObj)
 *
 * Overview: This function renders the object on the screen using
 * 			the current parameter settings. Location of the object is
 *			determined by the left, top, right and bottom parameters.
 *			The colors used are dependent on the state of the object.
 *			The font used is determined by the style scheme set.
 *
 *			Depending on the defined settings, value of the VuMeter
 *			will displayed or hidden. Displaying the value will require
 *			a little bit more rendering time depending on the size
 *			of the VuMeter and font used.
 *
 *			When rendering objects of the same type, each object
 *			must be rendered completely before the rendering of the
 *			next object is started. This is to avoid incomplete
 *			object rendering.
 *
 * PreCondition: Object must be created before this function is called.
 *
 * Input: pVuMeter - Pointer to the object to be rendered.
 *
 * Output: Returns the status of the drawing
 *		  - 1 - If the rendering was completed and
 *		  - 0 - If the rendering is not yet finished.
 *		  Next call to the function will resume the
 *		  rendering on the pending drawing state.
 *
 * Example:
 *	See VuCreate() example.
 *
 * Side Effects: none
 *
 ********************************************************************/
uint16_t VuDraw(void *pObj);


#endif // _VUMETER_H
