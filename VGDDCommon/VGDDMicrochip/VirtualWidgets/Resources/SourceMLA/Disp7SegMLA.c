// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// Disp7Seg - MLA version
// *****************************************************************************
// FileName:        disp7seg.c
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
// SOFTWARE AND DOCUMENTATION ARE PROVIDED ?AS IS? WITHOUT WARRANTY OF ANY
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
//  2012/03/15	Start of Developing
//  2014/09/06  MLA Version
// *****************************************************************************

#include "disp7seg.h"
#include "fontled7seg.h"

/* Internal Functions */
//void    MtrCalcDimensions(DISP7SEG *pDisp7Seg); // used to calculate the DISP7SEG dimensions

/*********************************************************************
 * Function: DISP7SEG  *D7Create(uint16_t ID, int16_t left, int16_t top, int16_t right,
 *		  int16_t bottom, uint16_t state, int16_t value,
 *		  int16_t minValue, int16_t maxValue, GFX_XCHAR *pDialText,
 *		  GFX_GOL_OBJ_SCHEME *pScheme)
 *
 *
 * Notes: Creates a DISP7SEG object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
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
        GFX_GOL_OBJ_SCHEME    *pScheme
        ) {
    DISP7SEG *pDisp7Seg = NULL;

    pDisp7Seg = (DISP7SEG *) GFX_malloc(sizeof (DISP7SEG));
    if (pDisp7Seg == NULL)
        return (NULL);

    pDisp7Seg->hdr.ID = ID; // unique id assigned for referencing
    pDisp7Seg->hdr.pNxtObj = NULL; // initialize pointer to NULL
    pDisp7Seg->hdr.type = OBJ_DISP7SEG; // set object type
    pDisp7Seg->hdr.left = left; // left,top coordinate
    pDisp7Seg->hdr.top = top; //
    pDisp7Seg->hdr.right = right; // right,bottom coordinate
    pDisp7Seg->hdr.bottom = bottom; //
    pDisp7Seg->hdr.pGolScheme = pScheme; //
    pDisp7Seg->NoOfDigits = NoOfDigits;
    pDisp7Seg->CurrentValue = Value;
    pDisp7Seg->DotPos = DotPos;
    pDisp7Seg->Thickness = Thickness;

    pDisp7Seg->DigitWidth = (right-left-Thickness)/NoOfDigits-Thickness;
    pDisp7Seg->DigitHeight=bottom-top-(Thickness<<1);

    pDisp7Seg->hdr.DrawObj = D7Draw; // draw function
    pDisp7Seg->hdr.actionGet = GFX_D7ActionGet; // message function
    pDisp7Seg->hdr.actionSet = GFX_D7ActionSet; // default message function
    pDisp7Seg->hdr.FreeObj = NULL; // free function
    pDisp7Seg->hdr.state = state; // state

    GFX_GOL_ObjectAdd((GFX_GOL_OBJ_HEADER *) pDisp7Seg);

    return (pDisp7Seg);
}

/*********************************************************************
 * Function: D7SetVal(DISP7SEG *pDisp7Seg, int16_t newVal)
 *
 * Notes: Sets the value of the DISP7SEG to newVal. If newVal is less
 *		 than 0, 0 is assigned. If newVal is greater than range,
 *		 range is assigned.
 *
 ********************************************************************/
void D7SetVal(DISP7SEG *pDisp7Seg, int16_t newVal) {
    pDisp7Seg->CurrentValue = newVal;
}

/*********************************************************************
 * Function: GFX_D7ActionSet(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 * Notes: This the default operation to change the state of the DISP7SEG.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void GFX_D7ActionSet(GFX_GOL_TRANSLATED_ACTION translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg) {
    DISP7SEG *pDisp7Seg;

    pDisp7Seg = (DISP7SEG *) pObj;

    if (translatedMsg == D7_MSG_SET) {
        D7SetVal(pDisp7Seg, pMsg->param2); // set the value
        GFX_GOL_ObjectStateSet(pDisp7Seg, D7_UPDATE); // update the DISP7SEG
    }
}

/*********************************************************************
 * Function: GFX_GOL_TRANSLATED_ACTION GFX_D7ActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Notes: Evaluates the message if the object will be affected by the
 *		 message or not.
 *
 ********************************************************************/
GFX_GOL_TRANSLATED_ACTION GFX_D7ActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg) {
    DISP7SEG *pDisp7Seg;

    pDisp7Seg = (DISP7SEG *) pObj;

    // Evaluate if the message is for the DISP7SEG
    // Check if disabled first
    if (GFX_GOL_ObjectStateGet(pDisp7Seg, D7_DISABLED))
        return (GFX_GOL_OBJECT_ACTION_INVALID);

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pDisp7Seg->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (D7_MSG_SET);
            }
        }
    }

    return (GFX_GOL_OBJECT_ACTION_INVALID);
}

/*********************************************************************
 * Function: uint16_t D7Draw(void *pObj)
 *
 * Notes: This is the state machine to draw the DISP7SEG.
 *
 ********************************************************************/
GFX_STATUS D7Draw(void *pObj) {

    typedef enum {
        D7_STATE_IDLE,
        D7_STATE_FRAME,
        D7_STATE_DRAWTEXT
    } D7_DRAW_STATES;

    DISP7SEG *pD7 = NULL;
    static D7_DRAW_STATES state = D7_STATE_IDLE;
    static uint16_t PosX, PosY;

    pD7 = (DISP7SEG *) pObj;

    if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
        return (0);

    switch (state) {
        case D7_STATE_IDLE:
            if (GFX_GOL_ObjectStateGet(pD7, D7_DRAW)) {
                GFX_ColorSet(pD7->hdr.pGolScheme->CommonBkColor);
                if (GFX_BarDraw(pD7->hdr.left, pD7->hdr.top, pD7->hdr.right, pD7->hdr.bottom) == 0)
                    return (0);
            }
            // if the draw state was to hide then state is still IDLE STATE so no need to change state
            if (GFX_GOL_ObjectStateGet(pD7, D7_HIDE))
                return (1);
            state = D7_STATE_FRAME;

        case D7_STATE_FRAME:
            if (GFX_GOL_ObjectStateGet(pD7, D7_DRAW) && GFX_GOL_ObjectStateGet(pD7, D7_FRAME)) {
                // show frame if specified to be shown
                GFX_LineStyleSet(GFX_LINE_STYLE_THIN_SOLID);
                if (!GFX_GOL_ObjectStateGet(pD7, D7_DISABLED)) {
                    // show enabled color
                    GFX_ColorSet(pD7->hdr.pGolScheme->Color1);
                    if (GFX_RectangleDraw(pD7->hdr.left, pD7->hdr.top, pD7->hdr.right, pD7->hdr.bottom) == 0)
                        return (0);
                } else {
                    // show disabled color
                    GFX_ColorSet(pD7->hdr.pGolScheme->ColorDisabled);
                    if (GFX_RectangleDraw(pD7->hdr.left, pD7->hdr.top, pD7->hdr.right, pD7->hdr.bottom) == 0)
                        return (0);
                }
            }
            state = D7_STATE_DRAWTEXT;

        case D7_STATE_DRAWTEXT:
            PosX = pD7->hdr.left+pD7->Thickness;
            PosY = pD7->hdr.top+pD7->Thickness;
            if (GFX_GOL_ObjectStateGet(pD7, D7_DRAW) || GFX_GOL_ObjectStateGet(pD7, D7_UPDATE)) {
                FontLed7SegSetSize(pD7->DigitHeight, pD7->DigitWidth, pD7->Thickness, GFX_GOL_ObjectStateGet(pD7, D7_DRAWPOLY) ? FontLed7SegPoly : FontLed7SegBar);
                if(!FontLed7SegPrintValue(pD7->CurrentValue, pD7->PreviousValue, PosX, PosY,
                    pD7->NoOfDigits, pD7->Thickness, pD7->hdr.pGolScheme->CommonBkColor, pD7->hdr.pGolScheme->TextColor0, GFX_GOL_ObjectStateGet(pD7, D7_UPDATE)))
                    return(0);
            }
    }

    //SetClip(CLIP_DISABLE); // remove clipping
    state = D7_STATE_IDLE; // go back to IDLE state
    return (1);
}
//#endif // USE_DISP7SEG
