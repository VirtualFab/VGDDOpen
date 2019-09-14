// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// Indicator - MLA Version
// *****************************************************************************
// FileName:        indicator.c
// Processor:       PIC24F, PIC24H, dsPIC, PIC32
// Compiler:        MPLAB C30, MPLAB C32
// Company:         VirtualFab, parts from Microchip Technology Incorporated
//
// VirtualFab Software License Agreement:
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
//  2012/03/17	Start of Developing
//  2014/09/07  MLA4 Version
// *****************************************************************************

#include "indicator.h"

/* Internal Functions */

/*********************************************************************
 * Function: INDICATOR  *IndCreate(uint16_t ID, int16_t left, int16_t top, int16_t right,
 *		  int16_t bottom, uint16_t state, int16_t value,
 *		  uint8_t Style, uint16_t IndicatorColour,GFX_XCHAR *pText,
 *		  GFX_GOL_OBJ_SCHEME *pScheme)
 *
 *
 * Notes: Creates an INDICATOR object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
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
        ) {
    INDICATOR *pIndicator = NULL;

    pIndicator = (INDICATOR *) GFX_malloc(sizeof (INDICATOR));
    if (pIndicator == NULL)
        return (NULL);

    pIndicator->hdr.ID = ID; // unique id assigned for referencing
    pIndicator->hdr.pNxtObj = NULL; // initialize pointer to NULL
    pIndicator->hdr.type = OBJ_INDICATOR; // set object type
    pIndicator->hdr.left = left; // left,top coordinate
    pIndicator->hdr.top = top; //
    pIndicator->hdr.right = right; // right,bottom coordinate
    pIndicator->hdr.bottom = bottom; //
    pIndicator->hdr.pGolScheme = pScheme; //
    pIndicator->Style = Style;
    pIndicator->Value = Value;
    pIndicator->IndicatorColour = IndicatorColour;
    pIndicator->pText = pText;
    pIndicator->hdr.state = state; // state
    pIndicator->hdr.DrawObj = IndDraw; // draw function
    pIndicator->hdr.actionGet = IndTranslateMsg; // message function
    pIndicator->hdr.actionSet = IndMsgDefault; // default message function
    pIndicator->hdr.FreeObj = NULL; // free function

    GFX_GOL_ObjectAdd(GFX_INDEX_0, (GFX_GOL_OBJ_HEADER *) pIndicator);

    return (pIndicator);
}

/*********************************************************************
 * Function: IndSetVal(INDICATOR *pIndicator, int16_t newVal)
 *
 * Notes: Sets the value of the INDICATOR to newVal.
 *
 ********************************************************************/
void IndSetVal(INDICATOR *pIndicator, int16_t newVal) {
    pIndicator->Value = newVal;
}

/*********************************************************************
 * Function: IndSetColour(INDICATOR *pIndicator, uint16_t newColour)
 *
 * Notes: Sets the colour of the INDICATOR to newColour.
 *
 ********************************************************************/
void IndSetColour(INDICATOR *pIndicator, uint16_t newColour) {
    pIndicator->IndicatorColour = newColour;
}

/*********************************************************************
 * Function: IndMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 * Notes: This the default operation to change the state of the INDICATOR.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void IndMsgDefault(GFX_GOL_TRANSLATED_ACTION translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg) {
    INDICATOR *pIndicator;

    pIndicator = (INDICATOR *) pObj;

    if (translatedMsg == IND_MSG_SET) {
        IndSetVal(pIndicator, pMsg->param2); // set the value
        GFX_GOL_ObjectStateSet(pIndicator, IND_UPDATE); // update the INDICATOR
    }
}

/*********************************************************************
 * Function: uint16_t IndTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Notes: Evaluates the message if the object will be affected by the
 *		 message or not.
 *
 ********************************************************************/
GFX_GOL_TRANSLATED_ACTION IndTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg) {
    INDICATOR *pIndicator;

    pIndicator = (INDICATOR *) pObj;

    // Evaluate if the message is for the INDICATOR
    // Check if disabled first
    if (GFX_GOL_ObjectStateGet(pIndicator, IND_DISABLED))
        return (GFX_GOL_OBJECT_ACTION_INVALID);

#ifdef USE_TOUCHSCREEN
    if (pMsg->type == TYPE_TOUCHSCREEN) {

        // Check if it falls in the indicator's face
        if ((pIndicator->hdr.left < pMsg->param1) &&
                (pIndicator->hdr.right > pMsg->param1) &&
                (pIndicator->hdr.top < pMsg->param2) &&
                (pIndicator->hdr.bottom > pMsg->param2)) {
            return (IND_MSG_TOUCHSCREEN);
        }
        return (GFX_GOL_OBJECT_ACTION_INVALID);
    }

#endif

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pIndicator->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (IND_MSG_SET);
            }
        }
    }

    return (GFX_GOL_OBJECT_ACTION_INVALID);
}

/*********************************************************************
 * Function: uint16_t IndDraw(void *pObj)
 *
 * Notes: This is the state machine to draw the INDICATOR.
 *
 ********************************************************************/
GFX_STATUS IndDraw(void *pObj) {

    typedef enum {
        IND_STATE_IDLE,
        IND_STATE_FRAME,
        IND_STATE_DRAWIND,
        IND_STATE_SETALIGN,
        IND_STATE_DRAWTEXT
    } IND_DRAW_STATES;

    volatile static INDICATOR *pInd = NULL;
    static IND_DRAW_STATES state = IND_STATE_IDLE;
    static uint16_t PosX, PosY;
    uint16_t radius, textWidth;
    GFX_XCHAR ch = 0;
    static int16_t charCtr = 0;

    pInd = (INDICATOR *) pObj;

    if (GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT)
        return (0);

    switch (state) {
        case IND_STATE_IDLE:
            if (GFX_GOL_ObjectStateGet(pInd, IND_DRAW)) {
                GFX_ColorSet(GFX_INDEX_0, pInd->hdr.pGolScheme->CommonBkColor);
                if (!GFX_BarDraw(GFX_INDEX_0, pInd->hdr.left, pInd->hdr.top, pInd->hdr.right, pInd->hdr.bottom))
                    return (0);
            }
            // if the draw state was to hide then state is still IDLE STATE so no need to change state
            if (GFX_GOL_ObjectStateGet(pInd, IND_HIDE))
                return (1);
            state = IND_STATE_FRAME;

        case IND_STATE_FRAME:
            if (GFX_GOL_ObjectStateGet(pInd, (IND_DRAW | IND_FRAME)) == (IND_DRAW | IND_FRAME)) {
                // show frame if specified to be shown
                GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_SOLID);
                if (GFX_GOL_ObjectStateGet(pInd, IND_DISABLED))
                    GFX_ColorSet(GFX_INDEX_0, pInd->hdr.pGolScheme->ColorDisabled); // show disabled color
                else
                    GFX_ColorSet(GFX_INDEX_0, pInd->hdr.pGolScheme->Color1); // show enabled color
                if (GFX_RectangleDraw(GFX_INDEX_0, pInd->hdr.left, pInd->hdr.top, pInd->hdr.right, pInd->hdr.bottom) == 0)
                    return (0);
            }

            // set clipping area, text will only appear inside the static text area.
            //SetClip(CLIP_ENABLE);
            //SetClipRgn(pInd->hdr.left + IND_INDENT, pInd->hdr.top, pInd->hdr.right - IND_INDENT, pInd->hdr.bottom);
            state = IND_STATE_DRAWIND;

        case IND_STATE_DRAWIND:
            GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_SOLID);
            GFX_ColorSet(GFX_INDEX_0, pInd->IndicatorColour);
            switch (pInd->Style) {
                case INDSTYLE_CIRCLE:
                    radius = (pInd->hdr.bottom - 2 - (pInd->hdr.top + 2)) / 2 + 1;
                    PosY = pInd->hdr.top + 2 + radius;
                    PosX = pInd->hdr.left + 2 + radius;
                    if (pInd->Value == 0) {
                        GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_DOTTED);
                        while (!GFX_CircleDraw(GFX_INDEX_0, PosX, PosY, radius));
                    } else {
                        GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_SOLID);
                        while (!GFX_CircleFillDraw(GFX_INDEX_0, PosX, PosY, radius));
                    }
                    break;

                case INDSTYLE_SQUARE:
                    PosX = pInd->hdr.left + pInd->hdr.bottom - pInd->hdr.top - 2;
                    if (pInd->Value == 0) {
                        GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_DOTTED);
                        while (!GFX_RectangleDraw(GFX_INDEX_0, pInd->hdr.left + 1, pInd->hdr.top + 1, PosX, pInd->hdr.bottom - 1));
                    } else {
                        GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_SOLID);
                        while (!GFX_BarDraw(GFX_INDEX_0, pInd->hdr.left + 1, pInd->hdr.top + 1, PosX, pInd->hdr.bottom - 1));
                    }
                    break;
            }
            state = IND_STATE_SETALIGN;

        case IND_STATE_SETALIGN:
            textWidth = GFX_TextStringWidthGet(pInd->pText, pInd->hdr.pGolScheme->pFont);

            if (GFX_GOL_ObjectStateGet(pInd, (IND_CENTER_ALIGN))) {
                PosX = pInd->hdr.left + (radius)+((pInd->hdr.right - pInd->hdr.left - textWidth) >> 1);
            } else if (GFX_GOL_ObjectStateGet(pInd, (IND_RIGHT_ALIGN))) {
                PosX = (pInd->hdr.right - textWidth);
            } else {
                PosX = (pInd->hdr.left + ((radius+4) << 1));
            }
            PosY = pInd->hdr.top + 1;
            GFX_TextCursorPositionSet(GFX_INDEX_0, PosX, PosY);
            // use the font specified in the object
            GFX_FontSet(GFX_INDEX_0, pInd->hdr.pGolScheme->pFont);
            GFX_ColorSet(GFX_INDEX_0, pInd->hdr.pGolScheme->TextColor0);

            state = IND_STATE_DRAWTEXT;

        case IND_STATE_DRAWTEXT:
            if (GFX_GOL_ObjectStateGet(pInd, IND_DRAW) || GFX_GOL_ObjectStateGet(pInd, IND_UPDATE)) {
                ch = *(pInd->pText + charCtr);
                // output one character at time until a newline character or a NULL character is sampled
                while ((0x0000 != ch) && (0x000A != ch)) {
                    if (!GFX_TextCharDraw(GFX_INDEX_0, ch))
                        return (0); // render the character
                    charCtr++; // update to next character
                    ch = *(pInd->pText + charCtr);
                }
            }
            charCtr = 0;
            state = IND_STATE_IDLE;
    }
    return (1);
}
//#endif // USE_INDICATOR
