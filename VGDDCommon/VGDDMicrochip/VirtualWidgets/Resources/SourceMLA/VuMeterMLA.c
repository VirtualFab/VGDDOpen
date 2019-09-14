// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// VuMeter
// *****************************************************************************
// FileName:        vumeter.c
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
//  2013/10/14	Initial release
// *****************************************************************************
#include "vumeter.h"

/* Internal Functions */

/*********************************************************************
 * Function: VUMETER  *VuCreate(uint16_t ID, int16_t left, int16_t top, int16_t right,
 *		  int16_t bottom, uint16_t state, int16_t value,
 *		  int16_t minValue, int16_t maxValue, XCHAR *pDialText,
 *		  GFX_GOL_OBJ_SCHEME *pScheme)
 *
 *
 * Notes: Creates a VUMETER object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
 *
 ********************************************************************/

VUMETER *VuCreate(
                  uint16_t ID,
                  int16_t left,
                  int16_t top,
                  int16_t right,
                  int16_t bottom,
                  uint16_t state,
                  void *Params,
                  void *pBitmap,
                  GFX_GOL_OBJ_SCHEME *pScheme
                  ) {
    VUMETER *pVuMeter = NULL;

    uint8_t *p=Params,i,cs=0;
    for(i=0;i<*(uint8_t *)Params;i++) cs ^=*p++;
    if(cs!=*p) return NULL;
    p=Params;

    pVuMeter = (VUMETER *) GFX_malloc(sizeof (VUMETER));
    if (pVuMeter == NULL)
        return (NULL);
    pVuMeter->hdr.ID = ID; // unique id assigned for referencing
    pVuMeter->hdr.pNxtObj = NULL; // initialize pointer to NULL
    pVuMeter->hdr.type = OBJ_VUMETER; // set object type
    pVuMeter->hdr.left = left; // left,top coordinate
    pVuMeter->hdr.top = top; //
    pVuMeter->hdr.right = right; // right,bottom coordinate
    pVuMeter->hdr.bottom = bottom; //
    pVuMeter->PointerType = (uint8_t) *(p+7); //PointerType;
    pVuMeter->AngleFrom = (int16_t)(*(p+8)<<8)+*(p+9); // AngleFrom;
    pVuMeter->AngleTo = (int16_t)(*(p+10)<<8)+*(p+11);  // AngleTo;
    pVuMeter->minValue = (int16_t)(*(p+3)<<8)+*(p+4); // minValue;
    pVuMeter->maxValue = (int16_t)(*(p+5)<<8)+*(p+6); // maxValue;
    pVuMeter->currentValue = -1;
    pVuMeter->newValue = (int16_t)(*(p+1)<<8)+*(p+2); // value;
    pVuMeter->previousValue = 0xffff;
    pVuMeter->hdr.state = state; // state
    pVuMeter->PointerCenterOffsetX=(int16_t)(*(p+12)<<8)+*(p+13); // PointerCenterOffsetX;
    pVuMeter->PointerCenterOffsetY=(int16_t)(*(p+14)<<8)+*(p+15); //PointerCenterOffsetY;
    pVuMeter->PointerLength=(int16_t)(*(p+16)<<8)+*(p+17); //PointerLength;
    pVuMeter->PointerWidth=(uint8_t)*(p+18); //PointerWidth;
    pVuMeter->PointerSpeed=(uint8_t)*(p+19); //PointerSpeed;
    pVuMeter->PointerStart=(int16_t)(*(p+21)<<8)+*(p+22); //PointerStart;
    pVuMeter->pBitmap=pBitmap;
    pVuMeter->hdr.DrawObj = VuDraw; // draw function
    pVuMeter->hdr.actionGet = GFX_VuActionGet; // message function
    pVuMeter->hdr.actionSet = GFX_VuActionSet; // default message function
    pVuMeter->hdr.FreeObj = NULL; // free function
    pVuMeter->BorderWidth=0;
    pVuMeter->state = VU_STATE_IDLE;

    // Set the color scheme to be used
    if (pScheme == NULL)
        return (NULL);
    pVuMeter->hdr.pGolScheme = pScheme;

    // calculate dimensions of the VUMETER
    VuCalcDimensions(pVuMeter);

    GFX_GOL_ObjectAdd((GFX_GOL_OBJ_HEADER *) pVuMeter);

    return (pVuMeter);
}

/*********************************************************************
 * Function: VuMeterCalcDimensions(void)
 *
 * Notes: Calculates the dimension of the VUMETER. Dependent on the
 *        VUMETER type set.
 *
 ********************************************************************/
void VuCalcDimensions(VUMETER *pVuMeter) {
      pVuMeter->RectImgWidth = pVuMeter->hdr.right - pVuMeter->hdr.left - 1 - (pVuMeter->BorderWidth << 1);
      pVuMeter->RectImgHeight = pVuMeter->hdr.bottom - pVuMeter->hdr.top - 1 - (pVuMeter->BorderWidth << 1);
      pVuMeter->Xcenter = pVuMeter->RectImgWidth * pVuMeter->PointerCenterOffsetX / 100;
      pVuMeter->Ycenter = pVuMeter->RectImgHeight - pVuMeter->RectImgHeight * pVuMeter->PointerCenterOffsetY / 100;
}

/*********************************************************************
 * Function: VuSetVal(VUMETER *pVuMeter, int16_t newVal)
 *
 * Notes: Sets the value of the VUMETER to newVal. If newVal is less
 *		 than 0, 0 is assigned. If newVal is greater than range,
 *		 range is assigned.
 *
 ********************************************************************/
void VuSetVal(VUMETER *pVuMeter, int16_t newVal) {
    if (newVal < pVuMeter->minValue) {
        pVuMeter->newValue = pVuMeter->minValue;
        return;
    }

    if (newVal > pVuMeter->maxValue) {
        pVuMeter->newValue = pVuMeter->maxValue;
        return;
    }

    pVuMeter->newValue = newVal;
    if(pVuMeter->PointerSpeed==0)
        pVuMeter->currentValue=pVuMeter->newValue;
}

/*********************************************************************
 * Function: VuMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 * Notes: This the default operation to change the state of the VUMETER.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void GFX_VuActionSet(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg) {
    VUMETER *pVuMeter;

    pVuMeter = (VUMETER *) pObj;

    if (translatedMsg == VU_MSG_SET) {
        VuSetVal(pVuMeter, pMsg->param2); // set the value
        GFX_GOL_ObjectStateSet(pVuMeter, VU_DRAW_UPDATE); // update the VUMETER
    }
}

/*********************************************************************
 * Function: uint16_t VuTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Notes: Evaluates the message if the object will be affected by the
 *		 message or not.
 *
 ********************************************************************/
uint16_t GFX_VuActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg) {
    VUMETER *pVuMeter;

    pVuMeter = (VUMETER *) pObj;

#ifdef USE_TOUCHSCREEN
    if (pMsg->type == TYPE_TOUCHSCREEN) {

        // Check if it falls in the VuMeter's face
        if ((pVuMeter->hdr.left < pMsg->param1) &&
                (pVuMeter->hdr.right > pMsg->param1) &&
                (pVuMeter->hdr.top < pMsg->param2) &&
                (pVuMeter->hdr.bottom > pMsg->param2)) {
            return (VU_MSG_TOUCHSCREEN);
        }
        return (GFX_GOL_OBJECT_ACTION_INVALID);
    }

#endif

    // Evaluate if the message is for the VUMETER
    // Check if disabled first
    if (GFX_GOL_ObjectStateGet(pVuMeter, VU_DISABLED))
        return (GFX_GOL_OBJECT_ACTION_INVALID);

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pVuMeter->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (VU_MSG_SET);
            }
        }
    }

    return (GFX_GOL_OBJECT_ACTION_INVALID);
}

/*********************************************************************
 * Function: void VuCheckCoords(void *pObj, int16_t x, int16_t y)
 *
 * Notes: Normalizes computed vertex coordinates and
 *        keeps track of the largest rectangular area used by the pointer,
 *        for subsequent partial restoring of the background bitmap
 *
 ********************************************************************/
void VuCheckCoords(void *pObj, int16_t *x, int16_t *y) {
    const uint8_t REDRAW_BORDER=8;
    VUMETER *pVuMeter = (VUMETER *) pObj;
    if(*x<0) *x=0;
    if(*y<0) *y=0;
    if(*x>pVuMeter->RectImgWidth) *x=pVuMeter->RectImgWidth;
    if(*y>pVuMeter->RectImgHeight) *y=pVuMeter->RectImgHeight;
    if (pVuMeter->xo1 > *x) pVuMeter->xo1 = *x - REDRAW_BORDER;
    if (pVuMeter->xo2 < *x) pVuMeter->xo2 = *x + REDRAW_BORDER;
    if (pVuMeter->yo1 > *y ) pVuMeter->yo1 = *y - REDRAW_BORDER;
    if (pVuMeter->yo2 < *y ) pVuMeter->yo2 = *y + REDRAW_BORDER;
    if(pVuMeter->xo1<0) pVuMeter->xo1=0;
    if(pVuMeter->xo2<0) pVuMeter->xo2=0;
    if(pVuMeter->yo1<0) pVuMeter->yo1=0;
    if(pVuMeter->yo2<0) pVuMeter->yo2=0;
    if(pVuMeter->xo1>pVuMeter->RectImgWidth) pVuMeter->xo1=pVuMeter->RectImgWidth;
    if(pVuMeter->xo2>pVuMeter->RectImgWidth) pVuMeter->xo2=pVuMeter->RectImgWidth;
    if(pVuMeter->yo1>pVuMeter->RectImgHeight) pVuMeter->yo1=pVuMeter->RectImgHeight;
    if(pVuMeter->yo2>pVuMeter->RectImgHeight) pVuMeter->yo2=pVuMeter->RectImgHeight;

}

/*********************************************************************
 * Function: uint16_t VuMeterDraw(void *pObj)
 *
 * Notes: This is the state machine to draw the VUMETER.
 *
 ********************************************************************/
uint16_t VuDraw(void *pObj) {
    VUMETER *pVuMeter;
    int16_t  Xn2, Yn2, Xn3, Yn3;
    int16_t k = 0;

    pVuMeter = (VUMETER *) pObj;

    if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
        return(GFX_STATUS_FAILURE);

    switch (pVuMeter->state) {
        case VU_STATE_IDLE:
            if (GFX_GOL_ObjectStateGet(pVuMeter, VU_HIDE)) { // Hide the VUMETER (remove from screen)
                GFX_ColorSet(pVuMeter->hdr.pGolScheme->CommonBkColor);
                if (!GFX_BarDraw(pVuMeter->hdr.left, pVuMeter->hdr.top, pVuMeter->hdr.right, pVuMeter->hdr.bottom)) // TODO: sostituire GFX_BarDraw con Bevel per hiding
                    return(GFX_STATUS_FAILURE);
                return (1); // Finished!
            } else if (GFX_GOL_ObjectStateGet(pVuMeter, VU_DRAWALL)) { // Check if we need to draw the whole object
                pVuMeter->state = VU_STATE_BACKGROUND_DRAW;
            } else if (GFX_GOL_ObjectStateGet(pVuMeter, VU_DRAW_UPDATE)) { // Or only the pointer
                pVuMeter->state = VU_STATE_POINTER_ERASE;
                goto pointer_erase_here;
            } else {
                return (1); // Nothing to do here...
                break;
            }

        case VU_STATE_BACKGROUND_DRAW:
            if(pVuMeter->pBitmap != NULL) {
                if(GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
                    return(GFX_STATUS_FAILURE);
                if(!GFX_ImagePartialDraw(pVuMeter->hdr.left, pVuMeter->hdr.top
                        , 0
                        , 0
                        , pVuMeter->hdr.right-pVuMeter->hdr.left
                        , pVuMeter->hdr.bottom-pVuMeter->hdr.top
                        , pVuMeter->pBitmap))
                    return(GFX_STATUS_FAILURE);
            }
            if (GFX_GOL_ObjectStateGet(pVuMeter, VU_FRAME)) {
                // Draw frame if specified to be shown
                GFX_LineStyleSet(GFX_LINE_STYLE_THIN_SOLID);
                if (!GFX_GOL_ObjectStateGet(pVuMeter, VU_DISABLED)) {
                    // Use enabled color
                    GFX_ColorSet(pVuMeter->hdr.pGolScheme->Color1);
                } else {
                    // Use disabled color
                    GFX_ColorSet(pVuMeter->hdr.pGolScheme->ColorDisabled);
                }
                if(GFX_RectangleDraw(pVuMeter->hdr.left, pVuMeter->hdr.top, pVuMeter->hdr.right, pVuMeter->hdr.bottom) == 0)
                    return(GFX_STATUS_FAILURE);
            }
            pVuMeter->state = VU_STATE_POINTER_DRAW;
            goto pointer_draw_here;

        case VU_STATE_POINTER_ERASE:
            pointer_erase_here :
            if (pVuMeter->xo2 != -1 && pVuMeter->pBitmap != NULL &&
                    (GFX_GOL_ObjectStateGet(pVuMeter, VU_DRAWALL) || GFX_GOL_ObjectStateGet(pVuMeter, VU_DRAW_UPDATE))) {
                if(GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
                    return(GFX_STATUS_FAILURE);
                if(!GFX_ImagePartialDraw(pVuMeter->hdr.left+pVuMeter->xo1
                        , pVuMeter->hdr.top+pVuMeter->yo1
                        , pVuMeter->xo1
                        , pVuMeter->yo1
                        , pVuMeter->xo2-pVuMeter->xo1+1
                        , pVuMeter->yo2-pVuMeter->yo1+1
                        , pVuMeter->pBitmap))
                    return(GFX_STATUS_FAILURE);
#if 0
                SetLineType(SOLID_LINE);
                GFX_LineStyleSet(GFX_LINE_STYLE_THIN_SOLID);
                GFX_ColorSet(rand() % 0x8000 + 0x4000);
                GFX_RectangleDraw(pVuMeter->hdr.left+pVuMeter->xo1, pVuMeter->hdr.top+pVuMeter->yo1,
                          pVuMeter->hdr.left+pVuMeter->xo2, pVuMeter->hdr.top+pVuMeter->yo2);
#endif
            }
            pVuMeter->state = VU_STATE_POINTER_DRAW;

        case VU_STATE_POINTER_DRAW: // Draw Pointer
            pointer_draw_here :
            if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
                return(GFX_STATUS_FAILURE);

            pVuMeter->xo1 = pVuMeter->RectImgWidth;
            pVuMeter->xo2 = 0;
            pVuMeter->yo1 = pVuMeter->RectImgHeight;
            pVuMeter->yo2 = 0;

            // Compute the pointer's vertex coordinates
            pVuMeter->degAngle = (pVuMeter->currentValue - pVuMeter->minValue) * 100 / (pVuMeter->maxValue - pVuMeter->minValue);
            pVuMeter->degAngle = pVuMeter->AngleFrom + (int32_t) ((pVuMeter->AngleTo - pVuMeter->AngleFrom)) * pVuMeter->degAngle / 100;
            GFX_CirclePointGet(pVuMeter->PointerLength, pVuMeter->degAngle % 360, &pVuMeter->xStart, &pVuMeter->yStart);
            pVuMeter->xStart += pVuMeter->Xcenter;
            pVuMeter->yStart += pVuMeter->Ycenter;
            VuCheckCoords(pVuMeter, &pVuMeter->xStart, &pVuMeter->yStart);
            pVuMeter->xStart += pVuMeter->hdr.left;
            pVuMeter->yStart += pVuMeter->hdr.top;

            GFX_LineStyleSet(GFX_GOL_ObjectStateGet(pVuMeter, VU_POINTER_THICK) ? GFX_LINE_STYLE_THICK_SOLID : GFX_LINE_STYLE_THIN_SOLID);

            switch (pVuMeter->PointerType) {
                case VU_POINTER_NORMAL:
                case VU_POINTER_3D:
                    for (k = 1; k<=(pVuMeter->PointerWidth >> 1); k++) { //GFX_GOL_ObjectStateGet(pVuMeter, VU_POINTER_THICK) ?  2 :  1) {
                        GFX_CirclePointGet(pVuMeter->PointerStart, (pVuMeter->degAngle - k) % 360, &Xn2, &Yn2);
                        GFX_CirclePointGet(pVuMeter->PointerStart, (pVuMeter->degAngle + k) % 360, &Xn3, &Yn3);
                        Xn2 += pVuMeter->Xcenter;
                        Yn2 += pVuMeter->Ycenter;
                        Xn3 += pVuMeter->Xcenter;
                        Yn3 += pVuMeter->Ycenter;
                        VuCheckCoords(pVuMeter, &Xn2, &Yn2);
                        VuCheckCoords(pVuMeter, &Xn3, &Yn3);
                        Xn2 += pVuMeter->hdr.left;
                        Yn2 += pVuMeter->hdr.top;
                        Xn3 += pVuMeter->hdr.left;
                        Yn3 += pVuMeter->hdr.top;
                        GFX_ColorSet(pVuMeter->PointerType == VU_POINTER_3D ? pVuMeter->hdr.pGolScheme->EmbossLtColor : pVuMeter->hdr.pGolScheme->EmbossDkColor);
                        if (!GFX_LineDraw(pVuMeter->xStart, pVuMeter->yStart, Xn2, Yn2))
                            return(GFX_STATUS_FAILURE);

                        GFX_ColorSet(pVuMeter->hdr.pGolScheme->EmbossDkColor);
                        if (!GFX_LineDraw(pVuMeter->xStart, pVuMeter->yStart, Xn3, Yn3))
                            return(GFX_STATUS_FAILURE);
                    }
                    break;

                case VU_POINTER_WIREFRAME:
                    GFX_CirclePointGet(pVuMeter->PointerStart, pVuMeter->degAngle - (pVuMeter->PointerWidth >> 1) % 360, &Xn2, &Yn2);
                    GFX_CirclePointGet(pVuMeter->PointerStart, pVuMeter->degAngle + (pVuMeter->PointerWidth >> 1) % 360, &Xn3, &Yn3);
                    Xn2 += pVuMeter->Xcenter;
                    Yn2 += pVuMeter->Ycenter;
                    Xn3 += pVuMeter->Xcenter;
                    Yn3 += pVuMeter->Ycenter;
                    VuCheckCoords(pVuMeter, &Xn2, &Yn2);
                    VuCheckCoords(pVuMeter, &Xn3, &Yn3);
                    Xn2 += pVuMeter->hdr.left;
                    Yn2 += pVuMeter->hdr.top;
                    Xn3 += pVuMeter->hdr.left;
                    Yn3 += pVuMeter->hdr.top;

                    GFX_ColorSet(pVuMeter->hdr.pGolScheme->EmbossDkColor);
                    if (!GFX_LineDraw(pVuMeter->xStart, pVuMeter->yStart, Xn2, Yn2))
                        return(GFX_STATUS_FAILURE);
                    if (!GFX_LineDraw(Xn2, Yn2, Xn3, Yn3))
                        return(GFX_STATUS_FAILURE);
                    if (!GFX_LineDraw(pVuMeter->xStart, pVuMeter->yStart, Xn3, Yn3))
                        return(GFX_STATUS_FAILURE);
                    break;

                case VU_POINTER_NEEDLE:
                    GFX_ColorSet(pVuMeter->hdr.pGolScheme->EmbossDkColor);
                    GFX_CirclePointGet(pVuMeter->PointerStart, pVuMeter->degAngle % 360, &Xn2, &Yn2);
                    Xn2 += pVuMeter->Xcenter;
                    Yn2 += pVuMeter->Ycenter;
                    VuCheckCoords(pVuMeter, &Xn2, &Yn2);
                    Xn2 += pVuMeter->hdr.left;
                    Yn2 += pVuMeter->hdr.top;
                    if (!GFX_LineDraw(pVuMeter->xStart, pVuMeter->yStart, Xn2, Yn2))
                        return(GFX_STATUS_FAILURE);
                    break;
            }


            GFX_LineStyleSet(GFX_LINE_STYLE_THIN_SOLID);
            pVuMeter->state = VU_STATE_IDLE;
            if (pVuMeter->currentValue == pVuMeter->newValue) {
                GFX_GOL_ObjectStateClear(pVuMeter, VU_DRAW_UPDATE | VU_DRAWALL | VU_DRAW_ANIMATING);
                return (1); // Finished updating!
            } else if(pVuMeter->PointerSpeed==0) {
                pVuMeter->currentValue = pVuMeter->newValue;
            } else {
                int16_t AnimationIncrement;
                if (pVuMeter->currentValue < pVuMeter->newValue) {
                    AnimationIncrement = ((pVuMeter->newValue - pVuMeter->currentValue) >> pVuMeter->PointerSpeed);
                    if (AnimationIncrement == 0)
                        AnimationIncrement = 1;
                } else if (pVuMeter->currentValue > pVuMeter->newValue) {
                    AnimationIncrement = -((pVuMeter->currentValue - pVuMeter->newValue) >> pVuMeter->PointerSpeed);
                    if (AnimationIncrement == 0)
                        AnimationIncrement = -1;
                }
                pVuMeter->currentValue += AnimationIncrement;
            }
            GFX_GOL_ObjectStateSet(pVuMeter, VU_DRAW_ANIMATING);
    }

    return (1);
}

