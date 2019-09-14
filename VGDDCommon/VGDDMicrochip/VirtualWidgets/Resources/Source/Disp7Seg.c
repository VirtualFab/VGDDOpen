// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// Disp7Seg
// *****************************************************************************
// FileName:        Disp7Seg.c
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
//  2012/03/15	Start of Developing
// *****************************************************************************
//#include "Graphics/Graphics.h"
//#include <math.h>
//#include <stdio.h>
#include <stdlib.h>

//#ifdef USE_DISP7SEG
#include "Disp7Seg.h"
#include "FontLed7Seg.h"

/* Internal Functions */
//void    MtrCalcDimensions(DISP7SEG *pDisp7Seg); // used to calculate the DISP7SEG dimensions

/*********************************************************************
 * Function: DISP7SEG  *D7Create(WORD ID, INT16 left, INT16 top, INT16 right,
 *		  INT16 bottom, WORD state, INT16 value,
 *		  INT16 minValue, INT16 maxValue, XCHAR *pDialText,
 *		  GOL_SCHEME *pScheme)
 *
 *
 * Notes: Creates a DISP7SEG object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
 *
 ********************************************************************/

DISP7SEG *D7Create(
        WORD ID,
        SHORT left,
        SHORT top,
        SHORT right,
        SHORT bottom,
        WORD state,
        DWORD Value,
        BYTE NoOfDigits,
        BYTE DotPos,
        BYTE Thickness,
        GOL_SCHEME *pScheme
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
    pDisp7Seg->hdr.MsgObj = D7TranslateMsg; // message function
    pDisp7Seg->hdr.MsgDefaultObj = D7MsgDefault; // default message function
    pDisp7Seg->hdr.FreeObj = NULL; // free function
    pDisp7Seg->hdr.state = state; // state

    GOLAddObject((OBJ_HEADER *) pDisp7Seg);

    return (pDisp7Seg);
}

/*********************************************************************
 * Function: D7SetVal(DISP7SEG *pDisp7Seg, INT16 newVal)
 *
 * Notes: Sets the value of the DISP7SEG to newVal. If newVal is less
 *		 than 0, 0 is assigned. If newVal is greater than range,
 *		 range is assigned.
 *
 ********************************************************************/
void D7SetVal(DISP7SEG *pDisp7Seg, INT16 newVal) {
    pDisp7Seg->CurrentValue = newVal;
}

/*********************************************************************
 * Function: D7MsgDefault(WORD translatedMsg, void *pObj, GOL_MSG* pMsg)
 *
 * Notes: This the default operation to change the state of the DISP7SEG.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void D7MsgDefault(WORD translatedMsg, void *pObj, GOL_MSG *pMsg) {
    DISP7SEG *pDisp7Seg;

    pDisp7Seg = (DISP7SEG *) pObj;

    if (translatedMsg == D7_MSG_SET) {
        D7SetVal(pDisp7Seg, pMsg->param2); // set the value
        SetState(pDisp7Seg, D7_UPDATE); // update the DISP7SEG
    }
}

/*********************************************************************
 * Function: WORD D7TranslateMsg(void *pObj, GOL_MSG *pMsg)
 *
 * Notes: Evaluates the message if the object will be affected by the
 *		 message or not.
 *
 ********************************************************************/
WORD D7TranslateMsg(void *pObj, GOL_MSG *pMsg) {
    DISP7SEG *pDisp7Seg;

    pDisp7Seg = (DISP7SEG *) pObj;

    // Evaluate if the message is for the DISP7SEG
    // Check if disabled first
    if (GetState(pDisp7Seg, D7_DISABLED))
        return (OBJ_MSG_INVALID);

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pDisp7Seg->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (D7_MSG_SET);
            }
        }
    }

    return (OBJ_MSG_INVALID);
}

/*********************************************************************
 * Function: WORD D7Draw(void *pObj)
 *
 * Notes: This is the state machine to draw the DISP7SEG.
 *
 ********************************************************************/
WORD D7Draw(void *pObj) {

    typedef enum {
        D7_STATE_IDLE,
        D7_STATE_FRAME,
        D7_STATE_DRAWTEXT
    } D7_DRAW_STATES;

    DISP7SEG *pD7 = NULL;
    static D7_DRAW_STATES state = D7_STATE_IDLE;
    static UINT16 PosX, PosY;

    pD7 = (DISP7SEG *) pObj;

    if (IsDeviceBusy())
        return (0);

    switch (state) {
        case D7_STATE_IDLE:
            SetClip(CLIP_DISABLE);

            if (GetState(pD7, D7_DRAW)) {
                SetColor(pD7->hdr.pGolScheme->CommonBkColor);
                if (Bar(pD7->hdr.left, pD7->hdr.top, pD7->hdr.right, pD7->hdr.bottom) == 0)
                    return (0);
            }
            // if the draw state was to hide then state is still IDLE STATE so no need to change state
            if (GetState(pD7, D7_HIDE))
                return (1);
            state = D7_STATE_FRAME;

        case D7_STATE_FRAME:
            if (GetState(pD7, D7_DRAW) && GetState(pD7, D7_FRAME)) {
                // show frame if specified to be shown
                SetLineType(SOLID_LINE);
                SetLineThickness(NORMAL_LINE);
                if (!GetState(pD7, D7_DISABLED)) {
                    // show enabled color
                    SetColor(pD7->hdr.pGolScheme->Color1);
                    if (Rectangle(pD7->hdr.left, pD7->hdr.top, pD7->hdr.right, pD7->hdr.bottom) == 0)
                        return (0);
                } else {
                    // show disabled color
                    SetColor(pD7->hdr.pGolScheme->ColorDisabled);
                    if (Rectangle(pD7->hdr.left, pD7->hdr.top, pD7->hdr.right, pD7->hdr.bottom) == 0)
                        return (0);
                }
            }
            state = D7_STATE_DRAWTEXT;

        case D7_STATE_DRAWTEXT:
            PosX = pD7->hdr.left+pD7->Thickness;
            PosY = pD7->hdr.top+pD7->Thickness;
            if (GetState(pD7, D7_DRAW) || GetState(pD7, D7_UPDATE)) {
                FontLed7SegSetSize(pD7->DigitHeight, pD7->DigitWidth, pD7->Thickness, GetState(pD7, D7_DRAWPOLY) ? FontLed7SegPoly : FontLed7SegBar);
                if(!FontLed7SegPrintValue(pD7->CurrentValue, pD7->PreviousValue, PosX, PosY,
                    pD7->NoOfDigits, pD7->Thickness, pD7->hdr.pGolScheme->CommonBkColor, pD7->hdr.pGolScheme->TextColor0, GetState(pD7, D7_UPDATE)))
                    return(0);
            }
    }

    //SetClip(CLIP_DISABLE); // remove clipping
    state = D7_STATE_IDLE; // go back to IDLE state
    return (1);
}
//#endif // USE_DISP7SEG
