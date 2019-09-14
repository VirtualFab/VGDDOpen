// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// Indicator
// *****************************************************************************
// FileName:        Indicator.c
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
// *****************************************************************************
//#include "Graphics/Graphics.h"
//#include <math.h>
//#include <stdio.h>
#include <stdlib.h>

//#ifdef USE_INDICATOR
#include "Indicator.h"

/* Internal Functions */

/*********************************************************************
 * Function: INDICATOR  *IndCreate(WORD ID, INT16 left, INT16 top, INT16 right,
 *		  INT16 bottom, WORD state, INT16 value,
 *		  BYTE Style, WORD IndicatorColour,XCHAR *pText,
 *		  GOL_SCHEME *pScheme)
 *
 *
 * Notes: Creates an INDICATOR object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
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
    pIndicator->hdr.MsgObj = IndTranslateMsg; // message function
    pIndicator->hdr.MsgDefaultObj = IndMsgDefault; // default message function
    pIndicator->hdr.FreeObj = NULL; // free function

    GOLAddObject((OBJ_HEADER *) pIndicator);

    return (pIndicator);
}

/*********************************************************************
 * Function: IndSetVal(INDICATOR *pIndicator, INT16 newVal)
 *
 * Notes: Sets the value of the INDICATOR to newVal.
 *
 ********************************************************************/
void IndSetVal(INDICATOR *pIndicator, INT16 newVal) {
    pIndicator->Value = newVal;
}

/*********************************************************************
 * Function: IndSetColour(INDICATOR *pIndicator, WORD newColour)
 *
 * Notes: Sets the colour of the INDICATOR to newColour.
 *
 ********************************************************************/
void IndSetColour(INDICATOR *pIndicator, WORD newColour) {
    pIndicator->IndicatorColour = newColour;
}

/*********************************************************************
 * Function: IndMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG* pMsg)
 *
 * Notes: This the default operation to change the state of the INDICATOR.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void IndMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG *pMsg) {
    INDICATOR *pIndicator;

    pIndicator = (INDICATOR *) pObj;

    if (translatedMsg == IND_MSG_SET) {
        IndSetVal(pIndicator, pMsg->param2); // set the value
        SetState(pIndicator, IND_UPDATE); // update the INDICATOR
    }
}

/*********************************************************************
 * Function: WORD IndTranslateMsg(void *pObj, GOL_MSG *pMsg)
 *
 * Notes: Evaluates the message if the object will be affected by the
 *		 message or not.
 *
 ********************************************************************/
WORD IndTranslateMsg(void *pObj, GOL_MSG *pMsg) {
    INDICATOR *pIndicator;

    pIndicator = (INDICATOR *) pObj;

    // Evaluate if the message is for the INDICATOR
    // Check if disabled first
    if (GetState(pIndicator, IND_DISABLED))
        return (OBJ_MSG_INVALID);

#ifdef USE_TOUCHSCREEN
    if (pMsg->type == TYPE_TOUCHSCREEN) {

        // Check if it falls in the indicator's face
        if ((pIndicator->hdr.left < pMsg->param1) &&
                (pIndicator->hdr.right > pMsg->param1) &&
                (pIndicator->hdr.top < pMsg->param2) &&
                (pIndicator->hdr.bottom > pMsg->param2)) {
            return (IND_MSG_TOUCHSCREEN);
        }
        return (OBJ_MSG_INVALID);
    }

#endif

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pIndicator->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (IND_MSG_SET);
            }
        }
    }

    return (OBJ_MSG_INVALID);
}

/*********************************************************************
 * Function: WORD IndDraw(void *pObj)
 *
 * Notes: This is the state machine to draw the INDICATOR.
 *
 ********************************************************************/
WORD IndDraw(void *pObj) {

    typedef enum {
        IND_STATE_IDLE,
        IND_STATE_FRAME,
        IND_STATE_DRAWIND,
        IND_STATE_SETALIGN,
        IND_STATE_DRAWTEXT
    } IND_DRAW_STATES;

    volatile static INDICATOR *pInd = NULL;
    static IND_DRAW_STATES state = IND_STATE_IDLE;
    static UINT16 PosX, PosY;
    UINT16 radius=0, textWidth;
    XCHAR ch = 0;
    static SHORT charCtr = 0;

    pInd = (INDICATOR *) pObj;

    if (IsDeviceBusy())
        return (0);

    switch (state) {
        case IND_STATE_IDLE:
            SetClip(CLIP_DISABLE);

            if (GetState(pInd, IND_DRAW)) {
                SetColor(pInd->hdr.pGolScheme->CommonBkColor);
                if (!Bar(pInd->hdr.left, pInd->hdr.top, pInd->hdr.right, pInd->hdr.bottom))
                    return (0);
            }
            // if the draw state was to hide then state is still IDLE STATE so no need to change state
            if (GetState(pInd, IND_HIDE))
                return (1);
            state = IND_STATE_FRAME;

        case IND_STATE_FRAME:
            if (GetState(pInd, IND_DRAW | IND_FRAME) == (IND_DRAW | IND_FRAME)) {
                // show frame if specified to be shown
                SetLineType(SOLID_LINE);
                SetLineThickness(NORMAL_LINE);
                if (GetState(pInd, IND_DISABLED))
                    SetColor(pInd->hdr.pGolScheme->ColorDisabled); // show disabled color
                else
                    SetColor(pInd->hdr.pGolScheme->Color1); // show enabled color
                if (Rectangle(pInd->hdr.left, pInd->hdr.top, pInd->hdr.right, pInd->hdr.bottom) == 0)
                    return (0);
            }

            // set clipping area, text will only appear inside the static text area.
            //SetClip(CLIP_ENABLE);
            //SetClipRgn(pInd->hdr.left + IND_INDENT, pInd->hdr.top, pInd->hdr.right - IND_INDENT, pInd->hdr.bottom);
            state = IND_STATE_DRAWIND;

        case IND_STATE_DRAWIND:
            SetLineThickness(NORMAL_LINE);
            SetColor(pInd->IndicatorColour);
            switch (pInd->Style) {
                case INDSTYLE_CIRCLE:
                    radius = (pInd->hdr.bottom - 2 - (pInd->hdr.top + 2)) / 2 + 1;
                    PosY = pInd->hdr.top + 2 + radius;
                    PosX = pInd->hdr.left + 2 + radius;
                    if (pInd->Value == 0) {
                        SetLineType(DOTTED_LINE);
                        while (!Circle(PosX, PosY, radius));
                    } else {
                        SetLineType(SOLID_LINE);
                        while (!FillCircle(PosX, PosY, radius));
                    }
                    break;

                case INDSTYLE_SQUARE:
                    PosX = pInd->hdr.left + pInd->hdr.bottom - pInd->hdr.top - 2;
                    if (pInd->Value == 0) {
                        SetLineType(DOTTED_LINE);
                        while (!Rectangle(pInd->hdr.left + 1, pInd->hdr.top + 1, PosX, pInd->hdr.bottom - 1));
                    } else {
                        SetLineType(SOLID_LINE);
                        while (!Bar(pInd->hdr.left + 1, pInd->hdr.top + 1, PosX, pInd->hdr.bottom - 1));
                    }
                    break;
            }
            state = IND_STATE_SETALIGN;

        case IND_STATE_SETALIGN:
            textWidth = GetTextWidth(pInd->pText, pInd->hdr.pGolScheme->pFont);

            if (GetState(pInd, (IND_CENTER_ALIGN))) {
                PosX = pInd->hdr.left + (radius)+((pInd->hdr.right - pInd->hdr.left - textWidth) >> 1);
            } else if (GetState(pInd, (IND_RIGHT_ALIGN))) {
                PosX = (pInd->hdr.right - textWidth);
            } else {
                PosX = (pInd->hdr.left + ((radius+4) << 1));
            }
            PosY = pInd->hdr.top + 1;
            MoveTo(PosX, PosY);
            // use the font specified in the object
            SetFont(pInd->hdr.pGolScheme->pFont);
            SetColor(pInd->hdr.pGolScheme->TextColor0);

            state = IND_STATE_DRAWTEXT;

        case IND_STATE_DRAWTEXT:
            if (GetState(pInd, IND_DRAW) || GetState(pInd, IND_UPDATE)) {
                ch = *(pInd->pText + charCtr);
                // output one character at time until a newline character or a NULL character is sampled
                while ((0x0000 != ch) && (0x000A != ch)) {
                    if (!OutChar(ch))
                        return (0); // render the character
                    charCtr++; // update to next character
                    ch = *(pInd->pText + charCtr);
                }
            }
            charCtr = 0;
            SetClip(CLIP_DISABLE); // remove clipping
            state = IND_STATE_IDLE;
    }
    return (1);
}
//#endif // USE_INDICATOR
