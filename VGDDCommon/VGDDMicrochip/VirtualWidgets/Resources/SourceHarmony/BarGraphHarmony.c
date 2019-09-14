// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// BarGraph - Harmony version
// *****************************************************************************
// FileName:        bargraph.c
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
// Copyright 2013 Microchip Technology Inc.  All rights reserved.
// Microchip licenses to you the right to use, modif(y, copy and distribute
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
// 2013/09/29   Fabio Violino - Initial release
// 2014/07/15   MLA version
// 2016/04/04   Added myitoa
// *****************************************************************************
#include "bargraph.h"

/* Internal Functions */

/*********************************************************************
 * Function: BARGRAPH  *BgCreate(uint16_t ID, int16_t left, int16_t top, int16_t right,
 *		  int16_t bottom, uint16_t state, int16_t value,
 *		  int16_t minValue, int16_t maxValue, GFX_XCHAR *pDialText,
 *		  GFX_GOL_OBJ_SCHEME *pScheme)
 *
 *
 * Notes: Creates a BARGRAPH object and adds it to the current active list.
 *        if the creation is successful, the pointer to the created Object
 *        is returned. if not successful, NULL is returned.
 *
 ********************************************************************/

BARGRAPH *BgCreate(
        uint16_t ID,
        int16_t left,
        int16_t top,
        int16_t right,
        int16_t bottom,
        uint16_t state,
        uint8_t SegmentsCount,
        void *Segments,
        void *Params,
        GFX_GOL_OBJ_SCHEME *pScheme
        ) {
    BARGRAPH *pBG = NULL;

    uint8_t *p = Params, i, cs = 0;
    for (i = 0; i<*(uint8_t *) Params; i++) cs ^= *p++;
    if (cs != *p) return NULL;
    p = Params;

    pBG = (BARGRAPH *) GFX_malloc(sizeof (BARGRAPH));
    if (pBG == NULL)
        return (NULL);

    pBG->hdr.ID = ID; // unique id assigned for referencing
    pBG->hdr.pNxtObj = NULL; // initialize pointer to NULL
    pBG->hdr.type = OBJ_BARGRAPH; // set object type
    pBG->hdr.left = left; // left,top coordinate
    pBG->hdr.top = top; //
    pBG->hdr.right = right; // right,bottom coordinate
    pBG->hdr.bottom = bottom; //
    pBG->BarSpeed = (uint8_t) *(p + 7); // BarSpeed;
    pBG->Style = (uint8_t) *(p + 8); // Style;
    pBG->Divisions = (int16_t) (*(p + 9) << 8)+*(p + 10); // Divisions;
    pBG->ScaleDivisions = (int16_t) (*(p + 11) << 8)+*(p + 12); // ScaleDivisions;
    pBG->SegmentsCount = SegmentsCount;
    pBG->Segments = Segments;
    pBG->minValue = (int16_t) (*(p + 3) << 8)+*(p + 4); // minValue
    pBG->maxValue = (int16_t) (*(p + 5) << 8)+*(p + 6); // maxValue
    pBG->currentValue = -1;
    pBG->newValue = (int16_t) (*(p + 1) << 8)+*(p + 2); //value;
    pBG->previousValue = 0xffff;
    pBG->hdr.state = state; // state
    pBG->hdr.DrawObj = BgDraw; // draw function
    pBG->hdr.actionGet = BgTranslateMsg; // message function
    pBG->hdr.actionSet = BgMsgDefault; // default message function
    pBG->hdr.FreeObj = NULL; // free function
    pBG->state = BG_STATE_IDLE;

    // Set the color scheme to be used
    pBG->hdr.pGolScheme = pScheme;

    // calculate dimensions of the BARGRAPH
    BgCalcDimensions(pBG);

    GFX_GOL_ObjectAdd(GFX_INDEX_0, (GFX_GOL_OBJ_HEADER *) pBG);

    return (pBG);
}

#define BORDER 1

/*********************************************************************
 * Function: BgCalcDimensions(void)
 *
 * Notes: Calculates the dimension of the BARGRAPH. Dependent on the
 *        BARGRAPH type set.
 *
 ********************************************************************/
void BgCalcDimensions(BARGRAPH *pBG) {
    pBG->RectImgWidth = pBG->hdr.right - pBG->hdr.left - 1 - (BORDER << 1);
    pBG->RectImgHeight = pBG->hdr.bottom - pBG->hdr.top - 1 - (BORDER << 1);
    if (pBG->ScaleDivisions > 0) {
        pBG->intScaleInterval = (pBG->maxValue - pBG->minValue) / pBG->ScaleDivisions;
    }

    if (GFX_GOL_ObjectStateGet(pBG, BG_VERTICAL)) {
        if (pBG->hdr.pGolScheme->pFont != NULL && pBG->ScaleDivisions > 0) {
            pBG->intScaleWorH = (((int32_t) (pBG->RectImgHeight) - (BORDER << 1) - GFX_TextStringHeightGet(pBG->hdr.pGolScheme->pFont)) << 8) / pBG->ScaleDivisions;
        }
        pBG->intBarRectWorH = pBG->RectImgHeight - BORDER;
    } else {
        if (pBG->hdr.pGolScheme->pFont != NULL && pBG->ScaleDivisions > 0) {
            pBG->intScaleWorH = ((int32_t) (pBG->RectImgWidth) << 8) / pBG->ScaleDivisions;
        }
        pBG->intBarRectWorH = pBG->RectImgWidth;
    }
    if (pBG->Style == BARGRPHSTYLE_SOLID) {
        pBG->intBarRectWorH -= (BORDER << 1);
    }
    pBG->intBarRectWorH = ((int32_t) (pBG->intBarRectWorH) << 8) / pBG->Divisions;
    pBG->intBarInterval = (((int32_t) (pBG->maxValue) - pBG->minValue) << 8) / pBG->Divisions;

}

/*********************************************************************
 * Function: BgSetVal(BARGRAPH *pBG, int16_t newVal)
 *
 * Notes: Sets the value of the BARGRAPH to newVal. if newVal is less
 *		 than 0, 0 is assigned. if newVal is greater than range,
 *		 range is assigned.
 *
 ********************************************************************/
void BgSetVal(BARGRAPH *pBG, int16_t newVal) {
    if (newVal < pBG->minValue) {
        pBG->newValue = pBG->minValue;
        return;
    }

    if (newVal > pBG->maxValue) {
        pBG->newValue = pBG->maxValue;
        return;
    }

    pBG->newValue = newVal;
}

/*********************************************************************
 * Function: BgMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 * Notes: This the default operation to change the state of the BARGRAPH.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void BgMsgDefault(GFX_GOL_TRANSLATED_ACTION translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg) {
    BARGRAPH *pBG;

    pBG = (BARGRAPH *) pObj;

    if (translatedMsg == BG_MSG_SET) {
        BgSetVal(pBG, pMsg->param2); // set the value
        GFX_GOL_ObjectStateSet(pBG, BG_DRAW_UPDATE); // update the BARGRAPH
    }
}

/*********************************************************************
 * Function: uint16_t BgTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Notes: Evaluates the message if the object will be affected by the
 *		 message or not.
 *
 ********************************************************************/
GFX_GOL_TRANSLATED_ACTION BgTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg) {
    BARGRAPH *pBG;

    pBG = (BARGRAPH *) pObj;

#ifdef USE_TOUCHSCREEN
    if (pMsg->type == TYPE_TOUCHSCREEN) {

        // Check if it falls in the BarGraph's face
        if ((pBG->hdr.left < pMsg->param1) &&
                (pBG->hdr.right > pMsg->param1) &&
                (pBG->hdr.top < pMsg->param2) &&
                (pBG->hdr.bottom > pMsg->param2)) {
            return (BG_MSG_TOUCHSCREEN);
        }
        return (GFX_GOL_OBJECT_ACTION_INVALID);
    }

#endif

    // Evaluate if the message is for the BARGRAPH
    // Check if disabled first
    if (GFX_GOL_ObjectStateGet(pBG, BG_DISABLED))
        return (GFX_GOL_OBJECT_ACTION_INVALID);

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pBG->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (BG_MSG_SET);
            }
        }
    }

    return (GFX_GOL_OBJECT_ACTION_INVALID);
}

char *myitoa(char *buffer, int value, char base) {
//    static char buffer[12]; // 12 bytes is big enough for an INT32
    int original = value; // save original value

    int c = sizeof (buffer) - 1;

    buffer[c] = 0; // write trailing null in last byte of buffer    

    if (value < 0) // if it's negative, note that and take the absolute value
        value = -value;

    do // write least significant digit of value that's left
    {
        buffer[--c] = (value % base) + '0';
        value /= base;
    } while (value);

    if (original < 0)
        buffer[--c] = '-';

    return &buffer[c];
}
/*********************************************************************
 * Function: uint16_t BgDraw(void *pObj)
 *
 * Notes: This is the state machine to draw the BARGRAPH.
 *
 ********************************************************************/
GFX_STATUS BgDraw(void *pObj) {
    volatile BARGRAPH *pBG;
    int16_t intXorY, intBarValue, intBarWorH;
    int16_t Xn1, Yn1, Xn2, Yn2;
    uint8_t i, j;
    char ScaleText[10];

    pBG = (BARGRAPH *) pObj;

    if (GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT)
        return (0);

    switch (pBG->state) {
        case BG_STATE_IDLE:
            if (GFX_GOL_ObjectStateGet(pBG, BG_HIDE)) { // Hide the BARGRAPH (remove from screen)
                GFX_ColorSet(GFX_INDEX_0, pBG->hdr.pGolScheme->CommonBkColor);
                if (!GFX_BarDraw(GFX_INDEX_0, pBG->hdr.left, pBG->hdr.top, pBG->hdr.right, pBG->hdr.bottom)) // TODO: sostituire GFX_BarDraw con Bevel per hiding
                    return (0);
                return (1); // Finished!
            } else if (GFX_GOL_ObjectStateGet(pBG, BG_DRAWALL)) { // Check if we need to draw the whole object
                pBG->state = BG_STATE_DRAW_BACKGROUND;
            } else if (GFX_GOL_ObjectStateGet(pBG, BG_DRAW_UPDATE)) { // Or only the blocks
                pBG->state = BG_STATE_DRAW_BLOCKS;
                goto blocks_erase_here;
            } else {
                return (1); // Nothing to do here...
                break;
            }

        case BG_STATE_DRAW_BACKGROUND:
            GFX_ColorSet(GFX_INDEX_0, pBG->hdr.pGolScheme->CommonBkColor);
            if (GFX_BarDraw(GFX_INDEX_0, pBG->hdr.left, pBG->hdr.top, pBG->hdr.right, pBG->hdr.bottom) == 0)
                return (0);
            if (GFX_GOL_ObjectStateGet(pBG, BG_FRAME)) {
                // Draw frame if specif(ied to be shown
                GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_SOLID);
                if (!GFX_GOL_ObjectStateGet(pBG, BG_DISABLED)) {
                    // Use enabled color
                    GFX_ColorSet(GFX_INDEX_0, pBG->hdr.pGolScheme->Color1);
                } else {
                    // Use disabled color
                    GFX_ColorSet(GFX_INDEX_0, pBG->hdr.pGolScheme->ColorDisabled);
                }
                if (GFX_RectangleDraw(GFX_INDEX_0, pBG->hdr.left, pBG->hdr.top, pBG->hdr.right, pBG->hdr.bottom) == 0)
                    return (0);
            }
            pBG->state = BG_STATE_DRAW_SCALE;

        case BG_STATE_DRAW_SCALE:
            if (pBG->ScaleDivisions > 0) {
                //Draw Scale Texts
                GFX_ColorSet(GFX_INDEX_0, pBG->hdr.pGolScheme->TextColor0);
                GFX_FontSet(GFX_INDEX_0, pBG->hdr.pGolScheme->pFont);
                for (i = 0; i < pBG->ScaleDivisions + 1; i++) {
                    myitoa(ScaleText, pBG->minValue + i * pBG->intScaleInterval, 10);
                    pBG->intTextHeight = GFX_TextStringHeightGet(pBG->hdr.pGolScheme->pFont);
                    pBG->intTextWidth = GFX_TextStringWidthGet(ScaleText, pBG->hdr.pGolScheme->pFont);
                    if (GFX_GOL_ObjectStateGet(pBG, BG_VERTICAL)) {
                        // Vertical BarGraph
                        intXorY = pBG->hdr.bottom - (((int32_t) (pBG->intScaleWorH) * i) >> 8) - BORDER - pBG->intTextHeight;
                        while(GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT); // Wait for previous drawing call to complete. Blocking!
                        GFX_TextStringDraw(GFX_INDEX_0, pBG->hdr.right - BORDER - pBG->intTextWidth, intXorY, ScaleText,0);
                    } else {
                        // Horizontal BarGraph
                        if (i == 0)
                            intXorY = pBG->hdr.left + BORDER;
                        else if (i == pBG->ScaleDivisions)
                            intXorY = pBG->hdr.right - pBG->intTextWidth - BORDER;
                        else
                            intXorY = pBG->hdr.left + (((int32_t) (pBG->intScaleWorH) * i) >> 8) + (BORDER << 1) - (pBG->intTextWidth >> 1);
                        while(GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT); // Wait for previous drawing call to complete. Blocking!
                        GFX_TextStringDraw(GFX_INDEX_0, intXorY, pBG->hdr.top + BORDER, ScaleText,0);
                    }
                }
            } else {
                pBG->intTextHeight = 1;
                pBG->intTextWidth = 1;
            }
            pBG->state = BG_STATE_ERASE_BLOCKS;

        case BG_STATE_ERASE_BLOCKS:
            blocks_erase_here :
                    pBG->state = BG_STATE_DRAW_BLOCKS;

        case BG_STATE_DRAW_BLOCKS:
            if (GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT)
                return (0);

            GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_SOLID);

            //Draw BarGraph
            for (i = 0; i < pBG->Divisions; i++) {
                intBarValue = ((int32_t) (pBG->intBarInterval) * i) >> 8;
                if ((pBG->previousValue < pBG->currentValue && intBarValue >= pBG->previousValue) ||
                        (pBG->previousValue > pBG->currentValue && intBarValue > pBG->currentValue)) {
                    if (intBarValue > pBG->currentValue) {
                        GFX_ColorSet(GFX_INDEX_0, pBG->hdr.pGolScheme->CommonBkColor);
                    } else {
                        BgSegment *Segment;
                        for (j = 0; j < pBG->SegmentsCount; j++) {
                            Segment = (void *) ((pBG->Segments)+(sizeof (BgSegment)) * j);
                            if (intBarValue >= Segment->StartValue) {
                                GFX_ColorSet(GFX_INDEX_0, Segment->SegmentColour);
                            }
                        }
                    }
                    if (GFX_GOL_ObjectStateGet(pBG, BG_VERTICAL)) {
                        // Vertical BarGraph
                        intXorY = pBG->hdr.bottom - (((int32_t) (pBG->intBarRectWorH) * i) >> 8) - (BORDER << 1) - 1;
                        intBarWorH = ((int32_t) (pBG->intBarRectWorH) >> 8) + 1;
                        if (pBG->Style == BARGRPHSTYLE_BLOCK || pBG->Style == BARGRPHSTYLE_WIREFRAME) {
                            intBarWorH = intBarWorH >> 1;
                        } else {
                            intXorY -= (BORDER << 1);
                        }
                        if (intBarWorH == 0)
                            intBarWorH = 1;
                        Xn1 = pBG->hdr.left + (BORDER << 1);
                        Yn2 = intXorY;
                        Xn2 = pBG->hdr.right - BORDER - pBG->intTextWidth;
                        Yn1 = Yn2 - intBarWorH;
                    } else {
                        // Horizontal BarGraph
                        intXorY = (((int32_t) (pBG->intBarRectWorH) * i) >> 8) + (BORDER << 1);
                        intBarWorH = ((int32_t) (pBG->intBarRectWorH) >> 8) + 1;
                        if (pBG->Style == BARGRPHSTYLE_BLOCK || pBG->Style == BARGRPHSTYLE_WIREFRAME) {
                            intBarWorH = intBarWorH >> 1;
                        }
                        if (intBarWorH == 0)
                            intBarWorH = 1;
                        Xn1 = pBG->hdr.left + intXorY;
                        Yn1 = pBG->hdr.top + BORDER + pBG->intTextHeight;
                        Xn2 = Xn1 + intBarWorH;
                        Yn2 = pBG->hdr.bottom - (BORDER << 1);
                    }
                    while(GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT); // Wait for previous drawing call to complete. Blocking!
                    switch (pBG->Style) {
                        case BARGRPHSTYLE_BLOCK:
                        case BARGRPHSTYLE_SOLID:
                            GFX_BarDraw(GFX_INDEX_0, Xn1, Yn1, Xn2, Yn2);
                            break;
                        case BARGRPHSTYLE_WIREFRAME:
                            GFX_RectangleDraw(GFX_INDEX_0, Xn1, Yn1, Xn2, Yn2);
                            break;
                    }
                }
            }

            GFX_LineStyleSet(GFX_INDEX_0, GFX_LINE_STYLE_THIN_SOLID);
            pBG->state = BG_STATE_IDLE;
            pBG->previousValue = pBG->currentValue;
            GFX_GOL_ObjectStateClear(pBG, BG_DRAW_UPDATE | BG_DRAWALL);
            if (pBG->currentValue == pBG->newValue) {
                GFX_GOL_ObjectStateClear(pBG, BG_DRAW_ANIMATING);
                return (1); // Finished updating!
            } else if (pBG->BarSpeed == 0) {
                pBG->currentValue = pBG->newValue;
            } else {
                int16_t AnimationIncrement;
                if (pBG->currentValue < pBG->newValue) {
                    AnimationIncrement = ((pBG->newValue - pBG->currentValue) >> pBG->BarSpeed);
                    if (AnimationIncrement == 0)
                        AnimationIncrement = 1;
                } else if (pBG->currentValue > pBG->newValue) {
                    AnimationIncrement = -((pBG->currentValue - pBG->newValue) >> pBG->BarSpeed);
                    if (AnimationIncrement == 0)
                        AnimationIncrement = -1;
                }
                pBG->currentValue += AnimationIncrement;
            }
            GFX_GOL_ObjectStateSet(pBG, BG_DRAW_ANIMATING);
    }

    return (1);
}

