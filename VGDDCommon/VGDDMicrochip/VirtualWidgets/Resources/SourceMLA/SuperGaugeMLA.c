// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// SuperGauge - MLA version
// *****************************************************************************
// FileName:        supergauge.c
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
#include "supergauge.h"
#include "fontled7seg.h"
GFX_STATUS GFX_BevelFillDraw(
                                uint16_t x1,
                                uint16_t y1,
                                uint16_t x2,
                                uint16_t y2,
                                uint16_t rad);
/*********************************************************************
 * Function: SuperGauge  *SgCreate(
 *              uint16_t ID, int16_t left, int16_t top, int16_t right, int16_t bottom,
 *              uint16_t state, void *Params,
 *              void *pDialScaleFont, GFX_XCHAR *pDialText,
 *              void *pSegments, GFX_GOL_OBJ_SCHEME *pScheme)
 *
 * Notes: Creates a SUPERGAUGE object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
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
        uint8_t SegmentsCount,
        void *pSegments,
        void *Params,
        GFX_GOL_OBJ_SCHEME *pScheme
        ) {
    SUPERGAUGE *pSG = NULL;

    uint8_t *p = Params, i, cs = 0;
    for (i = 0; i<*(uint8_t *) Params; i++) cs ^= *p++;
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

    pSG->newValue = (int16_t) (*(p + 1) << 8)+*(p + 2); // value;
    pSG->minValue = (int16_t) (*(p + 3) << 8)+*(p + 4); // minValue;
    pSG->maxValue = (int16_t) (*(p + 5) << 8)+*(p + 6); // maxValue;
    pSG->GaugeType = (uint8_t) *(p + 7); // GaugeType;
    pSG->PointerType = (uint8_t) *(p + 8); // PointerType;
    pSG->AngleFrom = (int16_t) (*(p + 9) << 8)+*(p + 10); // AngleFrom;
    pSG->AngleTo = (int16_t) (*(p + 11) << 8)+*(p + 12); // AngleTo;
    pSG->DialScaleNumDivisions = (uint8_t) *(p + 13); // DialScaleNumDivisions;
    pSG->DialScaleNumSubDivisions = (uint8_t) *(p + 14); // DialScaleNumSubDivisions;
    pSG->DialTextOffsetX = (int16_t) (*(p + 15) << 8)+*(p + 16); // DialTextOffsetX;
    pSG->DialTextOffsetY = (int16_t) (*(p + 17) << 8)+*(p + 18); // DialTextOffsetY;
    pSG->PointerSize = (int16_t) (*(p + 19) << 8)+*(p + 20); // PointerSize;
    pSG->PointerCenterSize = (uint8_t) *(p + 21); // PointerCenterSize;
    pSG->DigitsNumber = (uint8_t) *(p + 22); // DigitsNumber;
    pSG->DigitsSizeX = (uint8_t) *(p + 23); // DigitsSizeX;
    pSG->DigitsSizeY = (uint8_t) *(p + 24); // DigitsSizeY;
    pSG->DigitsOffsetX = (int16_t) (*(p + 25) << 8)+*(p + 26); // DigitsOffsetX;
    pSG->DigitsOffsetY = (int16_t) (*(p + 27) << 8)+*(p + 28); // DigitsOffsetY;

    pSG->value = -1;
    pSG->lastValue = 0xffff;
    pSG->SegmentsCount = SegmentsCount;
    pSG->Segments = pSegments;

    pSG->hdr.state = state; // state
    pSG->pDialText = pDialText;
    pSG->hdr.DrawObj = SgDraw; // draw function
    pSG->hdr.actionGet = GFX_SgActionGet; // message function
    pSG->hdr.actionSet = GFX_SgActionSet; // default message function
    pSG->hdr.FreeObj = NULL; // free function

    pSG->state= SG_STATE_IDLE;

    // Set the color scheme to be used
    if (pScheme == NULL)
        return (NULL);
    pSG->hdr.pGolScheme = pScheme;

    // Set the Title Font to be used
    if (pDialScaleFont == NULL)
        pSG->pDialScaleFont = (void *) &DRV_TOUCHSCREEN_FONT;
    else
        pSG->pDialScaleFont = pDialScaleFont;

    // calculate dimensions of the SUPERGAUGE
    SgCalcDimensions(pSG);

    GFX_GOL_ObjectAdd((GFX_GOL_OBJ_HEADER *) pSG);

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
    int16_t tempHeight, tempWidth;
    int16_t left, top, right, bottom, width, height;
    GFX_XCHAR tempChar[2] = {'8', 0};

    left = pSGauge->hdr.left;
    right = pSGauge->hdr.right;
    top = pSGauge->hdr.top;
    bottom = pSGauge->hdr.bottom;

    // get the text width reference. This is used to scale the SUPERGAUGE
    if (pSGauge->pDialText != NULL) {
        tempHeight = (pSGauge->hdr.pGolScheme->EmbossSize << 1) + GFX_TextStringHeightGet(pSGauge->hdr.pGolScheme->pFont);
    } else {
        tempHeight = (pSGauge->hdr.pGolScheme->EmbossSize << 1);
    }

    tempWidth = (pSGauge->hdr.pGolScheme->EmbossSize << 1) + (GFX_TextStringWidthGet(tempChar, pSGauge->hdr.pGolScheme->pFont) * SCALECHARCOUNT);

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
            if ((right - left - tempWidth) > (bottom - top - tempHeight - GFX_TextStringHeightGet(pSGauge->pDialScaleFont))) {
                pSGauge->radius = ((bottom - top - tempHeight - GFX_TextStringHeightGet(pSGauge->pDialScaleFont)) >> 1) - ((tempHeight + bottom - top) >> 3);
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
                    (bottom - top - (GFX_TextStringHeightGet(pSGauge->pDialScaleFont) + GFX_TextStringHeightGet(pSGauge->hdr.pGolScheme->pFont))) -
                    (GOL_EMBOSS_SIZE << 1)
                    ) {
                pSGauge->radius = bottom - top - (GFX_TextStringHeightGet(pSGauge->pDialScaleFont) + GFX_TextStringHeightGet(pSGauge->hdr.pGolScheme->pFont) + (GOL_EMBOSS_SIZE << 1));
            } else {
                pSGauge->radius = right -
                        left -
                        (GFX_TextStringWidthGet(tempChar, pSGauge->hdr.pGolScheme->pFont) * (SCALECHARCOUNT + 1)) -
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
    pSGauge->PointerWidth =  ((int32_t)pSGauge->RectImgWidth * pSGauge->PointerSize) / 400+1;
    if (GFX_GOL_ObjectStateGet(pSGauge, SG_POINTER_THICK))
        pSGauge->DrawStep = 4; //(pSGauge->RectImgWidth >>7) + 1; // 7
    else
        pSGauge->DrawStep = 1;
    pSGauge->DrawRadius = (pSGauge->RectImgWidth * (pSGauge->PointerCenterSize + 1) / 100);
    //pSGauge->RectImgWidth * pSGauge->PointerCenterSize / 100 + 1;

}

// *********************************************************************
// * Function: SgSetVal(SUPERGAUGE *pSGauge, int16_t newVal)
// *
// * Notes: Sets the value of the SUPERGAUGE to newVal. If newVal is less
// *		 than 0, 0 is assigned. If newVal is greater than range,
// *		 range is assigned.
// *
// *********************************************************************
void SgSetVal(SUPERGAUGE *pSGauge, int16_t newVal) {
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
// * Function: SgMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
// *
// * Notes: This the default operation to change the state of the SUPERGAUGE.
// *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
// *
// *********************************************************************
void GFX_SgActionSet(GFX_GOL_TRANSLATED_ACTION translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg) {
    SUPERGAUGE *pSGauge;

    pSGauge = (SUPERGAUGE *) pObj;

    if (translatedMsg == SG_MSG_SET) {
        SgSetVal(pSGauge, pMsg->param2); // set the value
        GFX_GOL_ObjectStateSet(pSGauge, SG_DRAW_UPDATE); // update the SUPERGAUGE
    }
}

// *********************************************************************
// * Function: uint16_t SgTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
// *
// * Notes: Evaluates the message if the object will be affected by the
// *		 message or not.
// *
// *********************************************************************
GFX_GOL_TRANSLATED_ACTION GFX_SgActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg) {
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
        return (GFX_GOL_OBJECT_ACTION_INVALID);
    }

#endif

    // Evaluate if the message is for the SUPERGAUGE
    // Check if disabled first
    if (GFX_GOL_ObjectStateGet(pSGauge, SG_DISABLED))
        return (GFX_GOL_OBJECT_ACTION_INVALID);

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pSGauge->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (SG_MSG_SET);
            }
        }
    }

    return (GFX_GOL_OBJECT_ACTION_INVALID);
}

// *********************************************************************
// * Function: uint8_t SgDrawPointerSUPERGAUGE *pSGauge, uint16_t cColor1, uint16_t cColor2, BOOL Erasing)
// *
// * Notes: Draws the pointer in new position
// *
// *********************************************************************
uint8_t SgDrawPointer(SUPERGAUGE *pSGauge, uint16_t cColor1, uint16_t cColor2, bool Erasing) {
    int16_t k = 0;
    int16_t x1, y1, x2, y2;

    if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
        return (GFX_STATUS_FAILURE);
    switch (pSGauge->PointerType) {
        case SG_POINTER_NORMAL:
        case SG_POINTER_3D:
            for (k = pSGauge->PointerWidth; k > 0; k -= pSGauge->DrawStep) {
                GFX_CirclePointGet(pSGauge->DrawRadius, (pSGauge->degAngle + k - 1) % 360, &x1, &y1);
                GFX_ColorSet(GFX_GOL_ObjectStateGet(pSGauge, SG_POINTER_NORMAL) ? cColor1 : cColor2);
                if (!GFX_LineDraw(x1 + pSGauge->xCenter, y1 + pSGauge->yCenter,
                        pSGauge->xLastPos, pSGauge->yLastPos))
                    return(GFX_STATUS_FAILURE);

                GFX_CirclePointGet(pSGauge->DrawRadius, (pSGauge->degAngle - k + 1) % 360, &x1, &y1);
                GFX_ColorSet(cColor1);
                if (!GFX_LineDraw(x1 + pSGauge->xCenter, y1 + pSGauge->yCenter,
                        pSGauge->xLastPos, pSGauge->yLastPos))
                    return(GFX_STATUS_FAILURE);
            }
            break;

        case SG_POINTER_NEEDLE:
            GFX_ColorSet(cColor1);
            GFX_CirclePointGet(pSGauge->DrawRadius, pSGauge->degAngle % 360, &x1, &y1);
            if (!GFX_LineDraw(x1 + pSGauge->xCenter, y1 + pSGauge->yCenter, pSGauge->xLastPos, pSGauge->yLastPos))
                return(GFX_STATUS_FAILURE);
            break;

        case SG_POINTER_WIREFRAME:
            GFX_ColorSet(cColor1);
            GFX_CirclePointGet(pSGauge->DrawRadius, (pSGauge->degAngle + pSGauge->PointerWidth) % 360, &x1, &y1);
            if (!GFX_LineDraw(x1 + pSGauge->xCenter, y1 + pSGauge->yCenter, pSGauge->xLastPos, pSGauge->yLastPos))
                return(GFX_STATUS_FAILURE);
            GFX_CirclePointGet(pSGauge->DrawRadius, (pSGauge->degAngle - pSGauge->PointerWidth) % 360, &x2, &y2);
            if (!GFX_LineDraw(x2 + pSGauge->xCenter, y2 + pSGauge->yCenter, pSGauge->xLastPos, pSGauge->yLastPos))
                return(GFX_STATUS_FAILURE);
            break;

        case SG_POINTER_PIE:
        case SG_POINTER_COLOUREDPIE: // TODO: Implement Pie and ColouredPie
            if (Erasing || GFX_GOL_ObjectStateGet(pSGauge, SG_POINTER_PIE)) {
                GFX_ColorSet(cColor1);
            }
            break;
        case SG_POINTER_FLOATINGBIT: // TODO: Implement FloatingBit
            break;
    }

    return (1);
}

// *********************************************************************
// * Function: uint16_t SgDraw(void *pObj)
// *
// * Notes: This is the state machine to draw the SUPERGAUGE.
// *
// *********************************************************************
uint16_t SgDraw(void *pObj) {
    static int16_t x1, y1, x2, y2;
    static int16_t temp, j;
    static int16_t CurrentDivision;
    static GFX_XCHAR strVal[SCALECHARCOUNT + 1]; // add one more space here for the NULL character
    //static GFX_XCHAR tempXchar[2] = {'8', 0}; // NULL is pre-defined here
    //static float radian;
    //static DWORD_VAL dTemp, dRes;
    static int16_t rulerValue = 0;
    static int16_t currentScaleDegAngle = 0;
    static int16_t ArcAngleFrom;
    static int16_t ArcAngleTo;
    static uint8_t CurrentSegment;

    SUPERGAUGE *pSG;
    SgSegment *Seg;
    int16_t subDegAngle;
    int16_t textSizeWidth;

    pSG = (SUPERGAUGE *) pObj;

    if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
        return (GFX_STATUS_FAILURE);

    switch (pSG->state) {
        case SG_STATE_IDLE:
            if (GFX_GOL_ObjectStateGet(pSG, SG_HIDE)) { // Hide the SUPERGAUGE (remove from screen)
                GFX_ColorSet(pSG->hdr.pGolScheme->CommonBkColor);
                if (!GFX_BarDraw(pSG->hdr.left, pSG->hdr.top, pSG->hdr.right, pSG->hdr.bottom)) // TODO: sostituire GFX_BarDraw con GFX_BevelDraw per hiding
                    return(GFX_STATUS_FAILURE);
                return (1);
            }

            // Check if we need to draw the whole object
            GFX_LineStyleSet(GFX_LINE_STYLE_THIN_SOLID);
            if (GFX_GOL_ObjectStateGet(pSG, SG_DRAW)) {
                pSG->state = SG_STATE_DIAL_DRAW;
            } else {
                pSG->state = SG_STATE_POINTER_ERASE;
                goto pointer_draw_here;
            }

        case SG_STATE_DIAL_DRAW:
            if (GFX_GOL_ObjectStateGet(pSG, SG_NOPANEL) == 0) {
                GFX_ColorSet(pSG->hdr.pGolScheme->CommonBkColor);
                switch (pSG->GaugeType) {
                    case SUPERGAUGE_FULL360:
                        if (!GFX_BevelFillDraw(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius - pSG->BorderWidth))
                            return(GFX_STATUS_FAILURE);
                        break;
                    case SUPERGAUGE_HALF180DOWN:
                    case SUPERGAUGE_HALF180UP:
                        if (!FreeArc(pSG->xCenter, pSG->yCenter, 1, pSG->radius - pSG->BorderWidth, pSG->MainAngleFrom, pSG->MainAngleTo))
                            return(GFX_STATUS_FAILURE);
                        break;
                }
            }
            pSG->state = SG_STATE_RIM_DRAW;

        case SG_STATE_RIM_DRAW:
            if (GFX_GOL_ObjectStateGet(pSG, SG_NOPANEL) == 0) {
            GFX_ColorSet(pSG->hdr.pGolScheme->Color1);
            //if (!GFX_BevelDraw(pSGauge->xCenter, pSGauge->yCenter, pSGauge->xCenter, pSGauge->yCenter, pSGauge->radius - j))
            if (!FreeArc(pSG->xCenter, pSG->yCenter, pSG->radius, pSG->radius - pSG->BorderWidth, pSG->MainAngleFrom, pSG->MainAngleTo))
                return(GFX_STATUS_FAILURE);
            //            GFX_LineStyleSet(GFX_LINE_STYLE_THICK_SOLID);
            //            if (!GFX_BevelDraw(pSGauge->xCenter, pSGauge->yCenter, pSGauge->xCenter, pSGauge->yCenter, pSGauge->radius - pSGauge->BorderWidth + 1))
            //                return(GFX_STATUS_FAILURE);
            }
            pSG->state = SG_STATE_SEGMENTS_DRAW_SETUP;

        case SG_STATE_SEGMENTS_DRAW_SETUP:
            pSG->radius = (pSG->RectImgVirtualWidth >> 1) - (pSG->RectImgVirtualWidth * 10 / 100);
            CurrentSegment = 0;
            pSG->state = SG_STATE_SEGMENTS_DRAW;

        case SG_STATE_SEGMENTS_DRAW:
            if (CurrentSegment < pSG->SegmentsCount) {
                Seg = (void *) ((pSG->Segments)+(sizeof (SgSegment)) * CurrentSegment);
                GFX_ColorSet(Seg->SegmentColour);
                ArcAngleFrom = (Seg->StartValue - pSG->minValue) * 100 / (pSG->maxValue - pSG->minValue);
                ArcAngleFrom = pSG->AngleFrom + (int32_t) ((pSG->AngleTo - pSG->AngleFrom) * ArcAngleFrom) / 100;

                ArcAngleTo = (Seg->EndValue - pSG->minValue) * 100 / (pSG->maxValue - pSG->minValue);
                ArcAngleTo = pSG->AngleFrom + (int32_t) ((pSG->AngleTo - pSG->AngleFrom) * ArcAngleTo) / 100;
                pSG->state = SG_STATE_SEGMENT_DRAW;
                goto draw_segment_here;
                return 0;
            } else {
                pSG->state = SG_STATE_SCALE_COMPUTE;
                goto scale_compute_here;
            }

        case SG_STATE_SEGMENT_DRAW:
            draw_segment_here :
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
                pSG->subIncrDeg = ((((uint32_t)(pSG->AngleTo - pSG->AngleFrom)) << 8) / pSG->DialScaleNumDivisions) / pSG->DialScaleNumSubDivisions;
            pSG->subDivHeight = pSG->RectImgWidth >> 6;
            CurrentDivision = 0;
            //const GFX_XCHAR strMeasureString[] = {'0', 0};
            for (j = 0; j < SCALECHARCOUNT; j++) {
                strVal[j] = (GFX_XCHAR) ' ';
            }
            strVal[SCALECHARCOUNT] = 0;
            pSG->state = SG_STATE_SCALE_DRAW;
            return(GFX_STATUS_FAILURE);

        case SG_STATE_SCALE_DRAW:
//            scale_draw_here :
            if (pSG->DialScaleNumDivisions==0 || CurrentDivision > pSG->DialScaleNumDivisions) {
                pSG->state = SG_STATE_CENTER_DRAW;
                return(GFX_STATUS_FAILURE);
            }
            currentScaleDegAngle = (pSG->AngleFrom + (int32_t) ((pSG->AngleTo - pSG->AngleFrom) * CurrentDivision) / pSG->DialScaleNumDivisions) % 360;

            // Draw Thick Line
            GFX_CirclePointGet(pSG->radius, currentScaleDegAngle, &x1, &y1);
            GFX_CirclePointGet(pSG->radius - pSG->RectImgWidth / 20, currentScaleDegAngle, &x2, &y2);
            GFX_ColorSet(pSG->hdr.pGolScheme->Color0);
            GFX_LineStyleSet(GFX_LINE_STYLE_THICK_SOLID);
            if (!GFX_LineDraw(x1 + pSG->xCenter, y1 + pSG->yCenter, x2 + pSG->xCenter, y2 + pSG->yCenter))
                return(GFX_STATUS_FAILURE);

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
            GFX_FontSet(pSG->hdr.pGolScheme->pFont);
            GFX_ColorSet(pSG->hdr.pGolScheme->TextColor0); // TODO: Textcolordisabled
            GFX_LineStyleSet(GFX_LINE_STYLE_THICK_SOLID);
            GFX_CirclePointGet(pSG->radius + (pSG->RectImgWidth>>5), currentScaleDegAngle, &x1, &y1);
            textSizeWidth = GFX_TextStringWidthGet(&strVal[SCALECHARCOUNT - j], pSG->hdr.pGolScheme->pFont);
            int16_t textSizeHeight=GFX_TextStringHeightGet(pSG->hdr.pGolScheme->pFont);
            if(x1<0) x1-=(textSizeWidth);
            if(x1==0) x1-=(textSizeWidth>>1);
            if(y1<0) y1-=(textSizeHeight>>2);
            if (GFX_TextStringDraw(x1 + pSG->xCenter
                    , y1 + pSG->yCenter-(textSizeHeight>>1)
                    , &strVal[SCALECHARCOUNT - j]
                    ,0)==GFX_STATUS_FAILURE)
                return (GFX_STATUS_FAILURE);
            rulerValue += (pSG->maxValue - pSG->minValue) / pSG->DialScaleNumDivisions;
            if (++CurrentDivision >= pSG->DialScaleNumDivisions) {
                return(GFX_STATUS_FAILURE);
            }

            //Draw thin lines
            if (pSG->DialScaleNumSubDivisions) {
                for (j = 1; j <= pSG->DialScaleNumSubDivisions - 1; j++) {
                    subDegAngle = currentScaleDegAngle + ((((uint32_t)pSG->subIncrDeg) * j) >> 8);
                    GFX_CirclePointGet(pSG->radius, subDegAngle, &x1, &y1);
                    GFX_CirclePointGet(pSG->radius - pSG->subDivHeight, subDegAngle, &x2, &y2);
                    GFX_ColorSet(pSG->hdr.pGolScheme->Color1); // TODO: _Scheme.Textcolordisabled
                    GFX_LineStyleSet(GFX_LINE_STYLE_THICK_SOLID);
                    if (!GFX_LineDraw(x1 + pSG->xCenter, y1 + pSG->yCenter, x2 + pSG->xCenter, y2 + pSG->yCenter))
                        return(GFX_STATUS_FAILURE);
                }
            }
            return(GFX_STATUS_FAILURE);


        case SG_STATE_CENTER_DRAW: // Draws the center point
            pSG->radius = pSG->RectImgWidth * pSG->PointerCenterSize / 100;
            GFX_ColorSet(pSG->hdr.pGolScheme->EmbossDkColor); // TODO: _Scheme.Textcolordisabled
            if (!GFX_BevelFillDraw(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius))
                return(GFX_STATUS_FAILURE);
            pSG->radius = pSG->RectImgWidth * pSG->PointerCenterSize / 200;
            GFX_ColorSet(pSG->hdr.pGolScheme->EmbossLtColor); // TODO: _Scheme.Textcolordisabled
            if (pSG->radius > 10) {
                GFX_LineStyleSet(GFX_LINE_STYLE_THICK_SOLID);
                if (!GFX_BevelFillDraw(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius))
                    return(GFX_STATUS_FAILURE);
            } else {
                if (!GFX_BevelDraw(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius))
                    return(GFX_STATUS_FAILURE);
            }
            pSG->state = SG_STATE_TEXT_DRAW;

        case SG_STATE_TEXT_DRAW:
            // draw the SUPERGAUGE title
            GFX_ColorSet(pSG->hdr.pGolScheme->TextColor0);
            GFX_FontSet(pSG->pDialScaleFont);
            temp = GFX_TextStringWidthGet(pSG->pDialText, pSG->pDialScaleFont);
            if (GFX_TextStringDraw(pSG->xCenter - (temp >> 1)+(pSG->DialTextOffsetX * pSG->RectImgWidth / 100)
                    , pSG->yCenter + (pSG->DialTextOffsetY * pSG->RectImgHeight / 100)
                    , pSG->pDialText
                    ,0) == GFX_STATUS_FAILURE)
                return (GFX_STATUS_FAILURE);
            pSG->state = SG_STATE_POINTER_ERASE;

        case SG_STATE_POINTER_ERASE:
            pointer_draw_here :
            if (GFX_GOL_ObjectStateGet(pSG, SG_DRAW_UPDATE)) {
                // to update the pointer, redraw the old position with background color
                GFX_LineStyleSet(GFX_LINE_STYLE_THICK_SOLID);
                if (!SgDrawPointer(pSG, pSG->hdr.pGolScheme->CommonBkColor, pSG->hdr.pGolScheme->CommonBkColor, true))
                    return (GFX_STATUS_FAILURE);
            }

            pSG->radius = (pSG->RectImgWidth >> 1) - ((uint32_t) (pSG->RectImgWidth * 16) / 100);
            //pSGauge->radius = (pSGauge->RectImgWidth >> 1) - (pSGauge->RectImgWidth * pSGauge->PointerCenterSize / 100);
            pSG->degAngle = (pSG->value - pSG->minValue) * 100 / (pSG->maxValue - pSG->minValue);
            pSG->degAngle = pSG->AngleFrom + (int32_t) ((pSG->AngleTo - pSG->AngleFrom) * pSG->degAngle) / 100;
            if (pSG->value != pSG->newValue)
                pSG->state = SG_STATE_POINTER_DRAW;
            else
                pSG->state = SG_STATE_VALUE_DRAW;
            return(GFX_STATUS_FAILURE);

        case SG_STATE_VALUE_DRAW: // display the current value
//            value_draw_here :
            if (pSG->DigitsNumber & (pSG->value != pSG->lastValue)) {
                if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
                    return (GFX_STATUS_FAILURE);
                // The first time (or anytime, if other SegDigits objects are used) let's calculate segment sizes
                //            if (FontLed7SegCurrentSizeX != pSGauge->DigitsSizeX || FontLed7SegCurrentSizeY != pSGauge->DigitsSizeY) {
                FontLed7SegSetSize(pSG->RectImgHeight * pSG->DigitsSizeY / 100,
                        pSG->RectImgHeight * pSG->DigitsSizeX / 100, 3, FontLed7SegBar);
                //            }

                temp = pSG->DigitsSizeX / 8 + 3; // gap
                x1 = pSG->xCenter - (((FontLed7SegCurrentSizeX + temp) * pSG->DigitsNumber) >> 1) + (pSG->RectImgWidth * pSG->DigitsOffsetX / 100);
                y1 = ((int32_t) (pSG->RectImgHeight * pSG->DigitsOffsetY) / 100);
                y1 = pSG->yCenter + y1;
                GFX_LineStyleSet(GFX_LINE_STYLE_THICK_SOLID);
                FontLed7SegPrintValue(pSG->value, pSG->lastValue, x1, y1, pSG->DigitsNumber, temp, pSG->hdr.pGolScheme->CommonBkColor, pSG->hdr.pGolScheme->TextColor1, true);
            }
            pSG->lastValue = pSG->value;
            pSG->state = SG_STATE_POINTER_DRAW;

            //return (1);

        case SG_STATE_POINTER_DRAW: // Draw Pointer
            GFX_CirclePointGet(pSG->radius, pSG->degAngle % 360, &pSG->xLastPos, &pSG->yLastPos);
            pSG->xLastPos += pSG->xCenter;
            pSG->yLastPos += pSG->yCenter;
            GFX_LineStyleSet(GFX_GOL_ObjectStateGet(pSG, SG_POINTER_THICK) ? GFX_LINE_STYLE_THICK_SOLID : GFX_LINE_STYLE_THIN_SOLID);
            if (!SgDrawPointer(pSG, pSG->hdr.pGolScheme->EmbossDkColor, pSG->hdr.pGolScheme->EmbossLtColor, false))
                return(GFX_STATUS_FAILURE);

            // Redraw Center circle
            pSG->radius = pSG->RectImgWidth * pSG->PointerCenterSize / 100;
            GFX_ColorSet(pSG->hdr.pGolScheme->EmbossDkColor); // TODO: _Scheme.Textcolordisabled
            if (!GFX_BevelDraw(pSG->xCenter, pSG->yCenter, pSG->xCenter, pSG->yCenter, pSG->radius))
                return(GFX_STATUS_FAILURE);

            if (pSG->value == pSG->newValue) {
                pSG->state = SG_STATE_IDLE;
                GFX_GOL_ObjectStateClear(pSG, SG_DRAW_UPDATE);
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

            GFX_LineStyleSet(GFX_LINE_STYLE_THIN_SOLID);
            GFX_GOL_ObjectStateSet(pSG, SG_DRAW_UPDATE);
            pSG->state = SG_STATE_POINTER_ERASE;
            return(GFX_STATUS_FAILURE);

    }

    return(GFX_STATUS_FAILURE);
}

/*********************************************************************
 * Function: uint16_t FreeArc(int16_t xL, int16_t yT, int16_t xR, int16_t yB, int16_t r1, int16_t r2, int16_t AngleFrom, int16_t AngleTo);
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
uint16_t FreeArc(int16_t xc, int16_t yc, int16_t r1, int16_t r2, int16_t AngleFrom, int16_t AngleTo) {

    typedef enum {
        BEGIN,
        DRAWARC,
    } FREEARC_STATES;

    int16_t x1, y1, x2, y2;
    static int16_t xo1, yo1, xo2, yo2;
    static FREEARC_STATES state = BEGIN;
    static int16_t currentDegAngle;
    static int16_t currentSubArc;
//    uint8_t i;

    if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
        return (GFX_STATUS_FAILURE);
    switch (state) {
        case BEGIN:
            currentDegAngle = AngleFrom;
            GFX_LineStyleSet(GFX_LINE_STYLE_THICK_SOLID);
            xo1=0;
            yo1=0;
            xo2=0;
            yo2=0;
            state = DRAWARC;

        case DRAWARC:
            for(currentSubArc=0;currentSubArc < 10;currentSubArc++) {
                GFX_CirclePointGet(r1, currentDegAngle % 360, &x1, &y1);
                GFX_CirclePointGet(r2, currentDegAngle % 360, &x2, &y2);
                x1+=xc;
                y1+=yc;
                x2+=xc;
                y2+=yc;
                if (!GFX_LineDraw(x1, y1, x2, y2))
                    return(GFX_STATUS_FAILURE);
                if(xo1>0 && yo1>0) {
                    if (!GFX_LineDraw(xo1, yo1, x1, y1))
                        return(GFX_STATUS_FAILURE);
                    if (!GFX_LineDraw(xo1, yo1, x2, y2))
                        return(GFX_STATUS_FAILURE);
                    if (!GFX_LineDraw(xo2, yo2, x2, y2))
                        return(GFX_STATUS_FAILURE);
                    if (!GFX_LineDraw(xo2, yo2, x1, y1))
                        return(GFX_STATUS_FAILURE);
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
