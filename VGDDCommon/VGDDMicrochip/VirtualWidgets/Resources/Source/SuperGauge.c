// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// SuperGauge
// *****************************************************************************
// FileName:        SuperGauge.c
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
//  2012/02/26	Start of Developing
// *****************************************************************************
#include "Graphics/Graphics.h"
#include <stdlib.h>

#ifdef USE_SUPERGAUGE
#include "SuperGauge.h"
#include "FontLed7Seg.h"

/*********************************************************************
 * Function: SuperGauge  *SgCreate(
 *              WORD ID, INT16 left, INT16 top, INT16 right, INT16 bottom,
 *              WORD state, void *Params,
 *              void *pDialScaleFont, XCHAR *pDialText,
 *              void *pSegments, GOL_SCHEME *pScheme)
 *
 * Notes: Creates a SUPERGAUGE object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
 *
 ********************************************************************/

SUPERGAUGE *SgCreate(
        WORD ID,
        INT16 left,
        INT16 top,
        INT16 right,
        INT16 bottom,
        WORD state,
        void *pDialScaleFont,
        XCHAR *pDialText,
        BYTE SegmentsCount,
        void *pSegments,
        void *Params,
        GOL_SCHEME *pScheme
        ) {
    SUPERGAUGE *pSG = NULL;

    BYTE *p = Params, i, cs = 0;
    for (i = 0; i<*(BYTE *) Params; i++) cs ^= *p++;
    if (cs != *p) return NULL;
    p = Params;

    pSG = (SUPERGAUGE *) GFX_malloc(sizeof (SUPERGAUGE));
    if (pSG == NULL)
        return (NULL);

    pSG->hdr.ID = ID; // unique id assigned for referencing
    pSG->hdr.pNxtObj = NULL; // initialize pointer to NULL
    pSG->hdr.type = OBJ_SUPERGAUGE; // set object type
    pSG->hdr.left = left; // left,top coordinate
    pSG->hdr.top = top;
    pSG->hdr.right = right; // right,bottom coordinate
    pSG->hdr.bottom = bottom;

    pSG->newValue = (INT16) (*(p + 1) << 8)+*(p + 2); // value;
    pSG->minValue = (INT16) (*(p + 3) << 8)+*(p + 4); // minValue;
    pSG->maxValue = (INT16) (*(p + 5) << 8)+*(p + 6); // maxValue;
    pSG->GaugeType = (BYTE) *(p + 7); // GaugeType;
    pSG->PointerType = (BYTE) *(p + 8); // PointerType;
    pSG->AngleFrom = (INT16) (*(p + 9) << 8)+*(p + 10); // AngleFrom;
    pSG->AngleTo = (INT16) (*(p + 11) << 8)+*(p + 12); // AngleTo;
    pSG->DialScaleNumDivisions = (BYTE) *(p + 13); // DialScaleNumDivisions;
    pSG->DialScaleNumSubDivisions = (BYTE) *(p + 14); // DialScaleNumSubDivisions;
    pSG->DialTextOffsetX = (INT16) (*(p + 15) << 8)+*(p + 16); // DialTextOffsetX;
    pSG->DialTextOffsetY = (INT16) (*(p + 17) << 8)+*(p + 18); // DialTextOffsetY;
    pSG->PointerSize = (INT16) (*(p + 19) << 8)+*(p + 20); // PointerSize;
    pSG->PointerCenterSize = (BYTE) *(p + 21); // PointerCenterSize;
    pSG->DigitsNumber = (BYTE) *(p + 22); // DigitsNumber;
    pSG->DigitsSizeX = (BYTE) *(p + 23); // DigitsSizeX;
    pSG->DigitsSizeY = (BYTE) *(p + 24); // DigitsSizeY;
    pSG->DigitsOffsetX = (INT16) (*(p + 25) << 8)+*(p + 26); // DigitsOffsetX;
    pSG->DigitsOffsetY = (INT16) (*(p + 27) << 8)+*(p + 28); // DigitsOffsetY;

    pSG->value = -1;
    pSG->lastValue = 0xffff;
    pSG->SegmentsCount = SegmentsCount;
    pSG->Segments = pSegments;

    pSG->hdr.state = state; // state
    pSG->pDialText = pDialText;
    pSG->hdr.DrawObj = SgDraw; // draw function
    pSG->hdr.MsgObj = SgTranslateMsg; // message function
    pSG->hdr.MsgDefaultObj = SgMsgDefault; // default message function
    pSG->hdr.FreeObj = NULL; // free function

    pSG->state= SG_STATE_IDLE;

    // Set the color scheme to be used
    if (pScheme == NULL)
        pSG->hdr.pGolScheme = _pDefaultGolScheme;
    else
        pSG->hdr.pGolScheme = pScheme;

    // Set the Title Font to be used
    if (pDialScaleFont == NULL)
        pSG->pDialScaleFont = (void *) &FONTDEFAULT;
    else
        pSG->pDialScaleFont = pDialScaleFont;

    // calculate dimensions of the SUPERGAUGE
    SgCalcDimensions(pSG);
    // Thanks Wolli:
    GetCirclePoint(pSG->radius, pSG->degAngle % 360, &pSG->xLastPos, &pSG->yLastPos);
    pSG->xLastPos += pSG->xCenter;
    pSG->yLastPos += pSG->yCenter;

    GOLAddObject((OBJ_HEADER *) pSG);

    return (pSG);
}

// *********************************************************************
// * Function: SuperGaugeCalcDimensions(void)
// *
// * Notes: Calculates the dimension of the SUPERGAUGE. Dependent on the
// *        SUPERGAUGE type set.
// *
// *********************************************************************
void SgCalcDimensions(SUPERGAUGE *pSGauge) {
    INT16 tempHeight, tempWidth;
    INT16 left, top, right, bottom, width, height;
    XCHAR tempChar[2] = {'8', 0};

    left = pSGauge->hdr.left;
    right = pSGauge->hdr.right;
    top = pSGauge->hdr.top;
    bottom = pSGauge->hdr.bottom;

    // get the text width reference. This is used to scale the SUPERGAUGE
    if (pSGauge->pDialText != NULL) {
        tempHeight = (GOL_EMBOSS_SIZE << 1) + GetTextHeight(pSGauge->hdr.pGolScheme->pFont);
    } else {
        tempHeight = (GOL_EMBOSS_SIZE << 1);
    }

    tempWidth = (GOL_EMBOSS_SIZE << 1) + (GetTextWidth(tempChar, pSGauge->hdr.pGolScheme->pFont) * SCALECHARCOUNT);

    width = right - left;
    height = bottom - top;
    pSGauge->BorderWidth = width / 50;
    // SUPERGAUGE size is dependent on the width or height.
    // The radius is also adjusted to add space for the scales
    switch (pSGauge->GaugeType) {
        case SUPERGAUGE_FULL360:
            if (width != height) {
                pSGauge->hdr.bottom = top + width;
                height = width;
            }
            pSGauge->MainAngleFrom = 0;
            pSGauge->MainAngleTo = 360;
            pSGauge->xCenter = (width >> 1) + left; // + pSGauge->BorderWidth;
            pSGauge->yCenter = (width >> 1) + top; // + pSGauge->BorderWidth;
            pSGauge->OuterAngleFrom = 0;
            pSGauge->OuterAngleTo = 360;
            pSGauge->RectImgVirtualWidth = width; //- (pSGauge->BorderWidth << 1);
            pSGauge->RectImgVirtualHeight = height; // - (pSGauge->BorderWidth << 1);
            pSGauge->radius = (pSGauge->RectImgVirtualWidth >> 1);
            /*// choose the radius
            if ((right - left - tempWidth) > (bottom - top - tempHeight - GetTextHeight(pSGauge->pDialScaleFont))) {
                pSGauge->radius = ((bottom - top - tempHeight - GetTextHeight(pSGauge->pDialScaleFont)) >> 1) - ((tempHeight + bottom - top) >> 3);
            } else
                pSGauge->radius = ((right - left) >> 1) - (tempWidth + ((right - left) >> 3));

            // center the SUPERGAUGE on the given dimensions
            pSGauge->xCenter = (left + right) >> 1;
            pSGauge->yCenter = ((bottom + top) >> 1) - (tempHeight >> 1);*/
            break;

        case SUPERGAUGE_HALF180UP:
            if (width != height * 2) {
                width = height * 2;
                pSGauge->hdr.right = left + width;
            }
            pSGauge->MainAngleFrom = 180;
            pSGauge->MainAngleTo = 360;
            pSGauge->xCenter = (width >> 1) + left; // + pSGauge->BorderWidth;
            pSGauge->yCenter = (width >> 1) + top; // + (pSGauge->BorderWidth);
            //pSGauge->yCenter = height + top + (pSGauge->BorderWidth << 1);
            pSGauge->OuterAngleFrom = 180;
            pSGauge->OuterAngleTo = 360;
            pSGauge->RectImgVirtualWidth = width; // - (pSGauge->BorderWidth << 1);
            pSGauge->RectImgVirtualHeight = height; //(height - pSGauge->BorderWidth) << 1;
            pSGauge->radius = (pSGauge->RectImgVirtualWidth >> 1);
            /*// choose the radius
            if ((right - left) >> 1 > (bottom - top)) {
                pSGauge->radius = (bottom - top) - ((tempHeight << 1) + ((bottom - top) >> 3));
                pSGauge->yCenter = ((bottom + top) >> 1) + ((pSGauge->radius + ((bottom - top) >> 3)) >> 1);
            } else {
                pSGauge->radius = ((right - left) >> 1) - (tempWidth + ((right - left) >> 3));
                pSGauge->yCenter = ((bottom + top) >> 1) + ((pSGauge->radius + ((right - left) >> 3)) >> 1);
            }
            // center the SUPERGAUGE on the given dimensions
            pSGauge->xCenter = (left + right) >> 1;*/
            break;

        case SUPERGAUGE_HALF180DOWN:
            pSGauge->MainAngleFrom = 0;
            pSGauge->MainAngleTo = 180;
            /*// choose the radius
            if ( (right - left - tempWidth) >
                    (bottom - top - (GetTextHeight(pSGauge->pDialScaleFont) + GetTextHeight(pSGauge->hdr.pGolScheme->pFont))) -
                    (GOL_EMBOSS_SIZE << 1)
                    ) {
                pSGauge->radius = bottom - top - (GetTextHeight(pSGauge->pDialScaleFont) + GetTextHeight(pSGauge->hdr.pGolScheme->pFont) + (GOL_EMBOSS_SIZE << 1));
            } else {
                pSGauge->radius = right -
                        left -
                        (GetTextWidth(tempChar, pSGauge->hdr.pGolScheme->pFont) * (SCALECHARCOUNT + 1)) -
                        GOL_EMBOSS_SIZE;
            }
            pSGauge->radius -= (((pSGauge->radius) >> 2) + GOL_EMBOSS_SIZE);
            // center the SUPERGAUGE on the given dimensions
            pSGauge->xCenter = ((left + right) >> 1) - ((pSGauge->radius + tempWidth + (pSGauge->radius >> 2)) >> 1);
            pSGauge->yCenter = ((top + bottom) >> 1) + ((pSGauge->radius + (pSGauge->radius >> 2)) >> 1);*/
            break;
    }
    pSGauge->RectImgWidth = width - (pSGauge->BorderWidth << 1);
    pSGauge->RectImgHeight = height - (pSGauge->BorderWidth << 1);
    pSGauge->PointerWidth =  ((INT32)pSGauge->RectImgWidth * pSGauge->PointerSize) / 400+1;
    if (GetState(pSGauge, SG_POINTER_THICK))
        pSGauge->DrawStep = 4; //(pSGauge->RectImgWidth >>7) + 1; // 7
    else
        pSGauge->DrawStep = 1;
    pSGauge->DrawRadius = (pSGauge->RectImgWidth * (pSGauge->PointerCenterSize + 1) / 100);
    //pSGauge->RectImgWidth * pSGauge->PointerCenterSize / 100 + 1;

}

// *********************************************************************
// * Function: SgSetVal(SUPERGAUGE *pSGauge, INT16 newVal)
// *
// * Notes: Sets the value of the SUPERGAUGE to newVal. If newVal is less
// *		 than 0, 0 is assigned. If newVal is greater than range,
// *		 range is assigned.
// *
// *********************************************************************
void SgSetVal(SUPERGAUGE *pSGauge, INT16 newVal) {
    if ((newVal < 0) || (newVal < pSGauge->minValue)) {
        pSGauge->newValue = pSGauge->minValue;
        return;
    }

    if (newVal > pSGauge->maxValue) {
        pSGauge->newValue = pSGauge->maxValue;
        return;
    }

    pSGauge->newValue = newVal;
}

// *********************************************************************
// * Function: SgMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG* pMsg)
// *
// * Notes: This the default operation to change the state of the SUPERGAUGE.
// *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
// *
// *********************************************************************
void SgMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG *pMsg) {
    SUPERGAUGE *pSGauge;

    pSGauge = (SUPERGAUGE *) pObj;

    if (translatedMsg == SG_MSG_SET) {
        SgSetVal(pSGauge, pMsg->param2); // set the value
        SetState(pSGauge, SG_DRAW_UPDATE); // update the SUPERGAUGE
    }
}

// *********************************************************************
// * Function: WORD SgTranslateMsg(void *pObj, GOL_MSG *pMsg)
// *
// * Notes: Evaluates the message if the object will be affected by the
// *		 message or not.
// *
// *********************************************************************
WORD SgTranslateMsg(void *pObj, GOL_MSG *pMsg) {
    SUPERGAUGE *pSGauge;

    pSGauge = (SUPERGAUGE *) pObj;

#ifdef USE_TOUCHSCREEN
    if (pMsg->type == TYPE_TOUCHSCREEN) {

        // Check if it falls in the SuperGauge's face
        if ((pSGauge->hdr.left < pMsg->param1) &&
                (pSGauge->hdr.right > pMsg->param1) &&
                (pSGauge->hdr.top < pMsg->param2) &&
                (pSGauge->hdr.bottom > pMsg->param2)) {
            return (SG_MSG_TOUCHSCREEN);
        }
        return (OBJ_MSG_INVALID);
    }

#endif

    // Evaluate if the message is for the SUPERGAUGE
    // Check if disabled first
    if (GetState(pSGauge, SG_DISABLED))
        return (OBJ_MSG_INVALID);

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pSGauge->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (SG_MSG_SET);
            }
        }
    }

    return (OBJ_MSG_INVALID);
}

// *********************************************************************
// * Function: BYTE SgDrawPointerSUPERGAUGE *pSGauge, WORD cColor1, WORD cColor2, BOOL Erasing)
// *
// * Notes: Draws the pointer in new position
// *
// *********************************************************************
BYTE SgDrawPointer(SUPERGAUGE *pSGauge, WORD cColor1, WORD cColor2, BOOL Erasing) {
    INT16 k = 0;
    INT16 x1, y1, x2, y2;

    if (IsDeviceBusy())
        return (0);
    switch (pSGauge->PointerType) {
        case SG_POINTER_NORMAL:
        case SG_POINTER_3D:
            for (k = pSGauge->PointerWidth; k > 0; k -= pSGauge->DrawStep) {
                GetCirclePoint(pSGauge->DrawRadius, (pSGauge->degAngle + k - 1) % 360, &x1, &y1);
                SetColor(GetState(pSGauge, SG_POINTER_NORMAL) ? cColor1 : cColor2);
                if (!Line(x1 + pSGauge->xCenter, y1 + pSGauge->yCenter,
                        pSGauge->xLastPos, pSGauge->yLastPos))
                    return (0);

                GetCirclePoint(pSGauge->DrawRadius, (pSGauge->degAngle - k + 1) % 360, &x1, &y1);
                SetColor(cColor1);
                if (!Line(x1 + pSGauge->xCenter, y1 + pSGauge->yCenter,
                        pSGauge->xLastPos, pSGauge->yLastPos))
                    return (0);
            }
            break;

        case SG_POINTER_NEEDLE:
            SetColor(cColor1);
            GetCirclePoint(pSGauge->DrawRadius, pSGauge->degAngle % 360, &x1, &y1);
            if (!Line(x1 + pSGauge->xCenter, y1 + pSGauge->yCenter, pSGauge->xLastPos, pSGauge->yLastPos))
                return (0);
            break;

        case SG_POINTER_WIREFRAME:
            SetColor(cColor1);
            GetCirclePoint(pSGauge->DrawRadius, (pSGauge->degAngle + pSGauge->PointerWidth) % 360, &x1, &y1);
            if (!Line(x1 + pSGauge->xCenter, y1 + pSGauge->yCenter, pSGauge->xLastPos, pSGauge->yLastPos))
                return (0);
            GetCirclePoint(pSGauge->DrawRadius, (pSGauge->degAngle - pSGauge->PointerWidth) % 360, &x2, &y2);
            if (!Line(x2 + pSGauge->xCenter, y2 + pSGauge->yCenter, pSGauge->xLastPos, pSGauge->yLastPos))
                return (0);
            break;

        case SG_POINTER_PIE:
        case SG_POINTER_COLOUREDPIE: // TODO: Implement Pie and ColouredPie
            if (Erasing || GetState(pSGauge, SG_POINTER_PIE)) {
                SetColor(cColor1);
            }
            break;
        case SG_POINTER_FLOATINGBIT: // TODO: Implement FloatingBit
            break;
    }

    return (1);
}

// *********************************************************************
// * Function: WORD SgDraw(void *pObj)
// *
// * Notes: This is the state machine to draw the SUPERGAUGE.
// *
// *********************************************************************
WORD SgDraw(void *pObj) {
    static INT16 x1, y1, x2, y2;
    static INT16 temp, j;
    static INT16 CurrentDivision;
    static XCHAR strVal[SCALECHARCOUNT + 1]; // add one more space here for the NULL character
    //static XCHAR tempXchar[2] = {'8', 0}; // NULL is pre-defined here
    //static float radian;
    //static DWORD_VAL dTemp, dRes;
    static INT16 rulerValue = 0;
    static INT16 currentScaleDegAngle = 0;
    static INT16 ArcAngleFrom;
    static INT16 ArcAngleTo;
    static BYTE CurrentSegment;

    SUPERGAUGE *pSG;
    static SgSegment *Seg;
    INT16 subDegAngle;
    INT16 textSizeWidth;

    pSG = (SUPERGAUGE *) pObj;

    if (IsDeviceBusy())
        return (0);

    switch (pSG->state) {
        case SG_STATE_IDLE:
            if (GetState(pSG, SG_HIDE)) { // Hide the SUPERGAUGE (remove from screen)
                SetColor(pSG->hdr.pGolScheme->CommonBkColor);
                if (!Bar(pSG->hdr.left, pSG->hdr.top, pSG->hdr.right, pSG->hdr.bottom)) // TODO: sostituire Bar con Bevel per hiding
                    return (0);
                return (1);
            }

            // Check if we need to draw the whole object
            SetLineThickness(NORMAL_LINE);
            SetLineType(SOLID_LINE);
            if (GetState(pSG, SG_DRAW)) {
                pSG->state = SG_STATE_DIAL_DRAW;
            } else {
                pSG->state = SG_STATE_POINTER_ERASE;
                goto pointer_draw_here;
            }

        case SG_STATE_DIAL_DRAW:
            if (GetState(pSG, SG_NOPANEL) == 0) {
                SetColor(pSG->hdr.pGolScheme->CommonBkColor);
                switch (pSG->GaugeType) {
                    case SUPERGAUGE_FULL360:
                        if (!FillBevel(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius - pSG->BorderWidth))
                            return (0);
                        break;
                    case SUPERGAUGE_HALF180DOWN:
                    case SUPERGAUGE_HALF180UP:
                        if (!FreeArc(pSG->xCenter, pSG->yCenter, 1, pSG->radius - pSG->BorderWidth, pSG->MainAngleFrom, pSG->MainAngleTo))
                            return (0);
                        break;
                }
            }
            pSG->state = SG_STATE_RIM_DRAW;

        case SG_STATE_RIM_DRAW:
            if (GetState(pSG, SG_NOPANEL) == 0) {
            SetColor(pSG->hdr.pGolScheme->Color1);
            //if (!Bevel(pSGauge->xCenter, pSGauge->yCenter, pSGauge->xCenter, pSGauge->yCenter, pSGauge->radius - j))
            if (!FreeArc(pSG->xCenter, pSG->yCenter, pSG->radius, pSG->radius - pSG->BorderWidth, pSG->MainAngleFrom, pSG->MainAngleTo))
                return (0);
            //            SetLineThickness(THICK_LINE);
            //            if (!Bevel(pSGauge->xCenter, pSGauge->yCenter, pSGauge->xCenter, pSGauge->yCenter, pSGauge->radius - pSGauge->BorderWidth + 1))
            //                return (0);
            }
            pSG->state = SG_STATE_SEGMENTS_DRAW_SETUP;

        case SG_STATE_SEGMENTS_DRAW_SETUP:
            pSG->radius = (pSG->RectImgVirtualWidth >> 1) - (pSG->RectImgVirtualWidth * 10 / 100);
            CurrentSegment = 0;
            pSG->state = SG_STATE_SEGMENTS_DRAW;

        case SG_STATE_SEGMENTS_DRAW:
            if (CurrentSegment < pSG->SegmentsCount) {
                Seg = (void *) ((pSG->Segments)+(sizeof (SgSegment)) * CurrentSegment);
                ArcAngleFrom = (Seg->StartValue - pSG->minValue) * 100 / (pSG->maxValue - pSG->minValue);
                ArcAngleFrom = pSG->AngleFrom + (INT32) ((pSG->AngleTo - pSG->AngleFrom) * ArcAngleFrom) / 100;

                ArcAngleTo = (Seg->EndValue - pSG->minValue) * 100 / (pSG->maxValue - pSG->minValue);
                ArcAngleTo = pSG->AngleFrom + (INT32) ((pSG->AngleTo - pSG->AngleFrom) * ArcAngleTo) / 100;
                pSG->state = SG_STATE_SEGMENT_DRAW;
                goto draw_segment_here;
                return 0;
            } else {
                pSG->state = SG_STATE_SCALE_COMPUTE;
                goto scale_compute_here;
            }

        case SG_STATE_SEGMENT_DRAW:
            draw_segment_here :
            SetColor(Seg->SegmentColour);
            if (FreeArc(pSG->xCenter, pSG->yCenter, pSG->radius, pSG->radius - pSG->RectImgWidth / 20,
                    ArcAngleFrom, ArcAngleTo)) {
                CurrentSegment++;
                pSG->state = SG_STATE_SEGMENTS_DRAW;
            }
            return 0;

        case SG_STATE_SCALE_COMPUTE:
            scale_compute_here :

            rulerValue = pSG->minValue;
            if (pSG->DialScaleNumDivisions && pSG->DialScaleNumSubDivisions)
                pSG->subIncrDeg = ((((UINT32)(pSG->AngleTo - pSG->AngleFrom)) << 8) / pSG->DialScaleNumDivisions) / pSG->DialScaleNumSubDivisions;
            pSG->subDivHeight = pSG->RectImgWidth >> 6;
            CurrentDivision = 0;
            //const XCHAR strMeasureString[] = {'0', 0};
            for (j = 0; j < SCALECHARCOUNT; j++) {
                strVal[j] = (XCHAR) ' ';
            }
            strVal[SCALECHARCOUNT] = 0;
            pSG->state = SG_STATE_SCALE_DRAW;
            return (0);

        case SG_STATE_SCALE_DRAW:
//            scale_draw_here :
            if (pSG->DialScaleNumDivisions==0 || CurrentDivision > pSG->DialScaleNumDivisions) {
                pSG->state = SG_STATE_CENTER_DRAW;
                return (0);
            }
            currentScaleDegAngle = (pSG->AngleFrom + (INT32) ((pSG->AngleTo - pSG->AngleFrom) * CurrentDivision) / pSG->DialScaleNumDivisions) % 360;

            // Draw Thick Line
            GetCirclePoint(pSG->radius, currentScaleDegAngle, &x1, &y1);
            GetCirclePoint(pSG->radius - pSG->RectImgWidth / 20, currentScaleDegAngle, &x2, &y2);
            SetColor(pSG->hdr.pGolScheme->Color0);
            SetLineThickness(THICK_LINE);
            if (!Line(x1 + pSG->xCenter, y1 + pSG->yCenter, x2 + pSG->xCenter, y2 + pSG->yCenter))
                return (0);

            //Draw Strings
            // this implements sprintf(strVal, "%d", temp); faster
            // note that this is just for values >= 0, while sprintf covers negative values.
            j = 1;
            temp = rulerValue;
            do {
                strVal[SCALECHARCOUNT - j] = (temp % 10) + '0';
                if (((temp /= 10) == 0) || (j >= SCALECHARCOUNT))
                    break;
                j++;
            } while (j <= SCALECHARCOUNT);

            // the (&strVal[SCALECHARCOUNT-j]) removes the leading zeros.
            // if leading zeroes will be printed change (&strVal[SCALECHARCOUNT-j])
            // to simply strVal and remove the break statement above in the do-while loop
            SetFont(pSG->hdr.pGolScheme->pFont);
            SetColor(pSG->hdr.pGolScheme->TextColor0); // TODO: Textcolordisabled
            SetLineThickness(THICK_LINE);
            GetCirclePoint(pSG->radius + (pSG->RectImgWidth>>5), currentScaleDegAngle, &x1, &y1);
            textSizeWidth = GetTextWidth(&strVal[SCALECHARCOUNT - j], pSG->hdr.pGolScheme->pFont);
            INT16 textSizeHeight=GetTextHeight(pSG->hdr.pGolScheme->pFont);
            if(x1<0) x1-=(textSizeWidth);
            if(x1==0) x1-=(textSizeWidth>>1);
            if(y1<0) y1-=(textSizeHeight>>2);
            MoveTo(x1 + pSG->xCenter, y1 + pSG->yCenter-(textSizeHeight>>1));
            if (!OutText(&strVal[SCALECHARCOUNT - j]))
                return (0);
            rulerValue += (pSG->maxValue - pSG->minValue) / pSG->DialScaleNumDivisions;
            if (++CurrentDivision >= pSG->DialScaleNumDivisions) {
                return (0);
            }

            //Draw thin lines
            if (pSG->DialScaleNumSubDivisions) {
                for (j = 1; j <= pSG->DialScaleNumSubDivisions - 1; j++) {
                    subDegAngle = currentScaleDegAngle + ((((UINT32)pSG->subIncrDeg) * j) >> 8);
                    GetCirclePoint(pSG->radius, subDegAngle, &x1, &y1);
                    GetCirclePoint(pSG->radius - pSG->subDivHeight, subDegAngle, &x2, &y2);
                    SetColor(pSG->hdr.pGolScheme->Color1); // TODO: _Scheme.Textcolordisabled
                    SetLineThickness(THICK_LINE);
                    if (!Line(x1 + pSG->xCenter, y1 + pSG->yCenter, x2 + pSG->xCenter, y2 + pSG->yCenter))
                        return (0);
                }
            }
            return (0);


        case SG_STATE_CENTER_DRAW: // Draws the center point
            pSG->radius = pSG->RectImgWidth * pSG->PointerCenterSize / 100;
            SetColor(pSG->hdr.pGolScheme->EmbossDkColor); // TODO: _Scheme.Textcolordisabled
            if (!FillBevel(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius))
                return (0);
            pSG->radius = pSG->RectImgWidth * pSG->PointerCenterSize / 200;
            SetColor(pSG->hdr.pGolScheme->EmbossLtColor); // TODO: _Scheme.Textcolordisabled
            if (pSG->radius > 10) {
                SetLineThickness(THICK_LINE);
                if (!FillBevel(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius))
                    return (0);
            } else {
                if (!Bevel(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius))
                    return (0);
            }
            pSG->state = SG_STATE_TEXT_DRAW;

        case SG_STATE_TEXT_DRAW:
            // draw the SUPERGAUGE title
            SetColor(pSG->hdr.pGolScheme->TextColor0);
            SetFont(pSG->pDialScaleFont);
            temp = GetTextWidth(pSG->pDialText, pSG->pDialScaleFont);
            MoveTo(pSG->xCenter - (temp >> 1)+(pSG->DialTextOffsetX * pSG->RectImgWidth / 100),
                    pSG->yCenter + (pSG->DialTextOffsetY * pSG->RectImgHeight / 100));
            pSG->state = SG_STATE_TEXT_DRAW_RUN;

        case SG_STATE_TEXT_DRAW_RUN:
            if (!OutText(pSG->pDialText))
                return (0);
            pSG->state = SG_STATE_POINTER_ERASE;

        case SG_STATE_POINTER_ERASE:
            pointer_draw_here :
            if (GetState(pSG, SG_DRAW_UPDATE)) {
                // to update the pointer, redraw the old position with background color
                SetLineThickness(THICK_LINE);
                if (!SgDrawPointer(pSG, pSG->hdr.pGolScheme->CommonBkColor, pSG->hdr.pGolScheme->CommonBkColor, TRUE))
                    return (0);
            }

            pSG->radius = (pSG->RectImgWidth >> 1) - ((UINT32) (pSG->RectImgWidth * 16) / 100);
            //pSGauge->radius = (pSGauge->RectImgWidth >> 1) - (pSGauge->RectImgWidth * pSGauge->PointerCenterSize / 100);
            pSG->degAngle = (pSG->value - pSG->minValue) * 100 / (pSG->maxValue - pSG->minValue);
            pSG->degAngle = pSG->AngleFrom + (INT32) ((pSG->AngleTo - pSG->AngleFrom) * pSG->degAngle) / 100;
            if (pSG->value != pSG->newValue)
                pSG->state = SG_STATE_POINTER_DRAW;
            else
                pSG->state = SG_STATE_VALUE_DRAW;
            return (0);

        case SG_STATE_VALUE_DRAW: // display the current value
//            value_draw_here :
            if (pSG->DigitsNumber & (pSG->value != pSG->lastValue)) {
                if (IsDeviceBusy())
                    return (0);
                // The first time (or anytime, if other SegDigits objects are used) let's calculate segment sizes
                //            if (FontLed7SegCurrentSizeX != pSGauge->DigitsSizeX || FontLed7SegCurrentSizeY != pSGauge->DigitsSizeY) {
                FontLed7SegSetSize(pSG->RectImgHeight * pSG->DigitsSizeY / 100,
                        pSG->RectImgHeight * pSG->DigitsSizeX / 100, 0, FontLed7SegPoly);
                //            }

                temp = pSG->DigitsSizeX / 10; // gap
                x1 = pSG->xCenter - (((FontLed7SegCurrentSizeX + temp) * pSG->DigitsNumber) >> 1) + (pSG->RectImgWidth * pSG->DigitsOffsetX / 100);
                y1 = ((INT32) (pSG->RectImgHeight * pSG->DigitsOffsetY) / 100);
                y1 = pSG->yCenter + y1;
                SetLineThickness(THICK_LINE);
                FontLed7SegPrintValue(pSG->value, pSG->lastValue, x1, y1, pSG->DigitsNumber, temp, pSG->hdr.pGolScheme->CommonBkColor, pSG->hdr.pGolScheme->TextColor1, TRUE);
            }
            pSG->lastValue = pSG->value;
            pSG->state = SG_STATE_POINTER_DRAW;

            //return (1);

        case SG_STATE_POINTER_DRAW: // Draw Pointer
            GetCirclePoint(pSG->radius, pSG->degAngle % 360, &pSG->xLastPos, &pSG->yLastPos);
            pSG->xLastPos += pSG->xCenter;
            pSG->yLastPos += pSG->yCenter;
            SetLineThickness(GetState(pSG, SG_POINTER_THICK) ? THICK_LINE : NORMAL_LINE);
            if (!SgDrawPointer(pSG, pSG->hdr.pGolScheme->EmbossDkColor, pSG->hdr.pGolScheme->EmbossLtColor, FALSE))
                return (0);

            // Redraw Center circle
            pSG->radius = pSG->RectImgWidth * pSG->PointerCenterSize / 100;
            SetColor(pSG->hdr.pGolScheme->EmbossDkColor); // TODO: _Scheme.Textcolordisabled
            if (!Bevel(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius))
                return (0);

            if (pSG->value == pSG->newValue) {
                pSG->state = SG_STATE_IDLE;
                ClrState(pSG, SG_DRAW_UPDATE);
                //goto value_draw_here;
                return (1);
            } else {
                if (pSG->value < pSG->newValue) {
                    temp = ((pSG->newValue - pSG->value) >> 2);
                    if (temp == 0)
                        temp = 1;
                } else {
                    temp = -((pSG->value - pSG->newValue) >> 2);
                    if (temp == 0)
                        temp = -1;
                }
                pSG->value += temp;
            }

            SetLineThickness(NORMAL_LINE);
            SetState(pSG, SG_DRAW_UPDATE);
            pSG->state = SG_STATE_POINTER_ERASE;
            return (0);

    }

    return (0);
}

/*********************************************************************
 * Function: WORD FreeArc(INT16 xL, INT16 yT, INT16 xR, INT16 yB, INT16 r1, INT16 r2, INT16 AngleFrom, INT16 AngleTo);
 *
 * PreCondition: none
 *
 * Input: xL, yT - location of the upper left center in the x,y coordinate
 *		 xc, yc - center x,y coordinate
 *		 r1, r2 - the two concentric circle radii, r1 as the radius
 *                         of the smaller circle and and r2 as the radius of the larger circle.
 *             AngleFrom - Angle in degrees to start drawing from
 *               AngleTo - Angle in degrees to end drawing
 *
 * Output: - Returns 0 when device is busy and the shape is not yet completely drawn.
 *         - Returns 1 when the shape is completely drawn.
 *
 * Side Effects: none
 *
 * Overview: Draws the arc of a beveled figure with given centers, radii, start and ending angles.
 *	    When r1 is zero and r2 has some value, a filled circle is drawn;
 *           When the radii have values, an arc of thickness (r2-r1) is drawn;
 *
 * Note: none
 *
 ********************************************************************/
WORD FreeArc(INT16 xc, INT16 yc, INT16 r1, INT16 r2, INT16 AngleFrom, INT16 AngleTo) {

    typedef enum {
        BEGIN,
        DRAWARC,
    } FREEARC_STATES;

    INT16 x1, y1, x2, y2;
    static INT16 xo1, yo1, xo2, yo2;
    static FREEARC_STATES state = BEGIN;
    static INT16 currentDegAngle;
    static INT16 currentSubArc;

    if (IsDeviceBusy())
        return (0);
    switch (state) {
        case BEGIN:
            currentDegAngle = AngleFrom;
            SetLineType(SOLID_LINE);
            SetLineThickness(THICK_LINE);
            xo1=0;
            yo1=0;
            xo2=0;
            yo2=0;
            state = DRAWARC;

        case DRAWARC:
            for(currentSubArc=0;currentSubArc < 10;currentSubArc++) {
                GetCirclePoint(r1, currentDegAngle % 360, &x1, &y1);
                GetCirclePoint(r2, currentDegAngle % 360, &x2, &y2);
                x1+=xc;
                y1+=yc;
                x2+=xc;
                y2+=yc;
                if (!Line(x1, y1, x2, y2))
                    return (0);
                if(xo1>0 && yo1>0) {
                    if (!Line(xo1, yo1, x1, y1))
                        return (0);
                    if (!Line(xo1, yo1, x2, y2))
                        return (0);
                    if (!Line(xo2, yo2, x2, y2))
                        return (0);
                    if (!Line(xo2, yo2, x1, y1))
                        return (0);
                }
                xo1=x1;
                yo1=y1;
                xo2=x2;
                yo2=y2;
                currentDegAngle += 1;
                if (currentDegAngle > AngleTo) {
                    state = BEGIN;
                    return 1;
                }
            }
            break;
    } // end of switch
    return 0;
}
#endif // USE_SUPERGAUGE
