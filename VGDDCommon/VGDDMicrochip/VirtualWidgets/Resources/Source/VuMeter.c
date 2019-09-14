// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// VuMeter
// *****************************************************************************
// FileName:        VuMeter.c
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
//  2013/12/31  Added different inertia for up/down (PointerSpeedDelay)
// *****************************************************************************
#include "Graphics/Graphics.h"

#ifdef USE_VUMETER
#include "VuMeter.h"

/* Internal Functions */

/*********************************************************************
 * Function: VUMETER  *VuCreate(WORD ID, INT16 left, INT16 top, INT16 right,
 *		  INT16 bottom, WORD state, INT16 value,
 *		  INT16 minValue, INT16 maxValue, XCHAR *pDialText,
 *		  GOL_SCHEME *pScheme)
 *
 *
 * Notes: Creates a VUMETER object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
 *
 ********************************************************************/

VUMETER *VuCreate(
                  WORD ID,
                  INT16 left,
                  INT16 top,
                  INT16 right,
                  INT16 bottom,
                  WORD state,
                  void *Params,
                  void *pBitmap,
                  GOL_SCHEME *pScheme
                  ) {
    VUMETER *pVuMeter = NULL;

    BYTE *p=Params,i,cs=0;
    for(i=0;i<*(BYTE *)Params;i++) cs ^=*p++;
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
    pVuMeter->PointerType = (BYTE) *(p+7); //PointerType;
    pVuMeter->AngleFrom = (INT16)(*(p+8)<<8)+*(p+9); // AngleFrom;
    pVuMeter->AngleTo = (INT16)(*(p+10)<<8)+*(p+11);  // AngleTo;
    pVuMeter->minValue = (INT16)(*(p+3)<<8)+*(p+4); // minValue;
    pVuMeter->maxValue = (INT16)(*(p+5)<<8)+*(p+6); // maxValue;
    pVuMeter->currentValue = -1;
    pVuMeter->newValue = (INT16)(*(p+1)<<8)+*(p+2); // value;
    pVuMeter->previousValue = 0xffff;
    pVuMeter->hdr.state = state; // state
    pVuMeter->PointerCenterOffsetX=(INT16)(*(p+12)<<8)+*(p+13); // PointerCenterOffsetX;
    pVuMeter->PointerCenterOffsetY=(INT16)(*(p+14)<<8)+*(p+15); //PointerCenterOffsetY;
    pVuMeter->PointerLength=(INT16)(*(p+16)<<8)+*(p+17); //PointerLength;
    pVuMeter->PointerWidth=(BYTE)*(p+18); //PointerWidth;
    pVuMeter->PointerSpeed=(BYTE)*(p+19); //PointerSpeed;
    pVuMeter->PointerSpeedDecay=(BYTE)*(p+20); //PointerSpeed;
    pVuMeter->PointerStart=(INT16)(*(p+21)<<8)+*(p+22); //PointerStart;
    pVuMeter->pBitmap=pBitmap;
    pVuMeter->hdr.DrawObj = VuDraw; // draw function
    pVuMeter->hdr.MsgObj = VuTranslateMsg; // message function
    pVuMeter->hdr.MsgDefaultObj = VuMsgDefault; // default message function
    pVuMeter->hdr.FreeObj = NULL; // free function
    pVuMeter->BorderWidth=0;
    pVuMeter->state = VU_STATE_IDLE;

    // Set the color scheme to be used
    if (pScheme == NULL)
        pVuMeter->hdr.pGolScheme = _pDefaultGolScheme;
    else
        pVuMeter->hdr.pGolScheme = pScheme;

    // calculate dimensions of the VUMETER
    VuCalcDimensions(pVuMeter);

    GOLAddObject((OBJ_HEADER *) pVuMeter);

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
 * Function: VuSetVal(VUMETER *pVuMeter, INT16 newVal)
 *
 * Notes: Sets the value of the VUMETER to newVal. If newVal is less
 *		 than 0, 0 is assigned. If newVal is greater than range,
 *		 range is assigned.
 *
 ********************************************************************/
void VuSetVal(VUMETER *pVuMeter, INT16 newVal) {
    if (newVal < pVuMeter->minValue) {
        pVuMeter->newValue = pVuMeter->minValue;
        return;
    }

    if (newVal > pVuMeter->maxValue) {
        pVuMeter->newValue = pVuMeter->maxValue;
        return;
    }

    pVuMeter->newValue = newVal;
    if((pVuMeter->currentValue<newVal && pVuMeter->PointerSpeed==0) ||
       (pVuMeter->currentValue>newVal && pVuMeter->PointerSpeedDecay==0))
        pVuMeter->currentValue=pVuMeter->newValue;

}

/*********************************************************************
 * Function: VuMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG* pMsg)
 *
 * Notes: This the default operation to change the state of the VUMETER.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void VuMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG *pMsg) {
    VUMETER *pVuMeter;

    pVuMeter = (VUMETER *) pObj;

    if (translatedMsg == VU_MSG_SET) {
        VuSetVal(pVuMeter, pMsg->param2); // set the value
        SetState(pVuMeter, VU_DRAW_UPDATE); // update the VUMETER
    }
}

/*********************************************************************
 * Function: WORD VuTranslateMsg(void *pObj, GOL_MSG *pMsg)
 *
 * Notes: Evaluates the message if the object will be affected by the
 *		 message or not.
 *
 ********************************************************************/
WORD VuTranslateMsg(void *pObj, GOL_MSG *pMsg) {
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
        return (OBJ_MSG_INVALID);
    }

#endif

    // Evaluate if the message is for the VUMETER
    // Check if disabled first
    if (GetState(pVuMeter, VU_DISABLED))
        return (OBJ_MSG_INVALID);

    if (pMsg->type == TYPE_SYSTEM) {
        if (pMsg->param1 == pVuMeter->hdr.ID) {
            if (pMsg->uiEvent == EVENT_SET) {
                return (VU_MSG_SET);
            }
        }
    }

    return (OBJ_MSG_INVALID);
}

/*********************************************************************
 * Function: void VuCheckCoords(void *pObj, INT16 x, INT16 y)
 *
 * Notes: Normalizes computed vertex coordinates and
 *        keeps track of the largest rectangular area used by the pointer,
 *        for subsequent partial restoring of the background bitmap
 *
 ********************************************************************/
void VuCheckCoords(void *pObj, INT16 *x, INT16 *y) {
    const BYTE REDRAW_BORDER=8;
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
 * Function: WORD VuMeterDraw(void *pObj)
 *
 * Notes: This is the state machine to draw the VUMETER.
 *
 ********************************************************************/
WORD VuDraw(void *pObj) {
    VUMETER *pVuMeter;
    INT16  Xn2, Yn2, Xn3, Yn3;
    INT16 k = 0;

    pVuMeter = (VUMETER *) pObj;

    if (IsDeviceBusy())
        return (0);

    switch (pVuMeter->state) {
        case VU_STATE_IDLE:
            if (GetState(pVuMeter, VU_HIDE)) { // Hide the VUMETER (remove from screen)
                SetColor(pVuMeter->hdr.pGolScheme->CommonBkColor);
                if (!Bar(pVuMeter->hdr.left, pVuMeter->hdr.top, pVuMeter->hdr.right, pVuMeter->hdr.bottom)) // TODO: sostituire Bar con Bevel per hiding
                    return (0);
                return (1); // Finished!
            } else if (GetState(pVuMeter, VU_DRAWALL)) { // Check if we need to draw the whole object
                pVuMeter->state = VU_STATE_BACKGROUND_DRAW;
            } else if (GetState(pVuMeter, VU_DRAW_UPDATE)) { // Or only the pointer
                pVuMeter->state = VU_STATE_POINTER_ERASE;
                goto pointer_erase_here;
            } else {
                return (1); // Nothing to do here...
                break;
            }

        case VU_STATE_BACKGROUND_DRAW:
            if(pVuMeter->pBitmap != NULL) {
                if(IsDeviceBusy())
                    return (0);
                if(!PutImagePartial(pVuMeter->hdr.left, pVuMeter->hdr.top, pVuMeter->pBitmap, 1,
                    0, 0, pVuMeter->hdr.right-pVuMeter->hdr.left,pVuMeter->hdr.bottom-pVuMeter->hdr.top))
                    return (0);
            }
            if (GetState(pVuMeter, VU_FRAME)) {
                // Draw frame if specified to be shown
                SetLineType(SOLID_LINE);
                SetLineThickness(NORMAL_LINE);
                if (!GetState(pVuMeter, VU_DISABLED)) {
                    // Use enabled color
                    SetColor(pVuMeter->hdr.pGolScheme->Color1);
                } else {
                    // Use disabled color
                    SetColor(pVuMeter->hdr.pGolScheme->ColorDisabled);
                }
                if(Rectangle(pVuMeter->hdr.left, pVuMeter->hdr.top, pVuMeter->hdr.right, pVuMeter->hdr.bottom) == 0)
                    return (0);
            }
            pVuMeter->state = VU_STATE_POINTER_DRAW;
            goto pointer_draw_here;

        case VU_STATE_POINTER_ERASE:
            pointer_erase_here :
            if (pVuMeter->xo2 != -1 && pVuMeter->pBitmap != NULL &&
                    (GetState(pVuMeter, VU_DRAWALL) || GetState(pVuMeter, VU_DRAW_UPDATE))) {
                if(IsDeviceBusy())
                    return (0);
                if(!PutImagePartial(pVuMeter->hdr.left+pVuMeter->xo1, pVuMeter->hdr.top+pVuMeter->yo1, pVuMeter->pBitmap, 1,
                     pVuMeter->xo1, pVuMeter->yo1, pVuMeter->xo2-pVuMeter->xo1+1, pVuMeter->yo2-pVuMeter->yo1+1))
                    return (0);
#if 0
                SetLineType(SOLID_LINE);
                SetLineThickness(NORMAL_LINE);
                SetColor(rand() % 0x8000 + 0x4000);
                Rectangle(pVuMeter->hdr.left+pVuMeter->xo1, pVuMeter->hdr.top+pVuMeter->yo1,
                          pVuMeter->hdr.left+pVuMeter->xo2, pVuMeter->hdr.top+pVuMeter->yo2);
#endif
            }
            pVuMeter->state = VU_STATE_POINTER_DRAW;

        case VU_STATE_POINTER_DRAW: // Draw Pointer
            pointer_draw_here :
            if (IsDeviceBusy())
                return (0);

            pVuMeter->xo1 = pVuMeter->RectImgWidth;
            pVuMeter->xo2 = 0;
            pVuMeter->yo1 = pVuMeter->RectImgHeight;
            pVuMeter->yo2 = 0;

            // Compute the pointer's vertex coordinates
            pVuMeter->degAngle = (pVuMeter->currentValue - pVuMeter->minValue) * 100 / (pVuMeter->maxValue - pVuMeter->minValue);
            pVuMeter->degAngle = pVuMeter->AngleFrom + (INT32) ((pVuMeter->AngleTo - pVuMeter->AngleFrom)) * pVuMeter->degAngle / 100;
            GetCirclePoint(pVuMeter->PointerLength, pVuMeter->degAngle % 360, &pVuMeter->xStart, &pVuMeter->yStart);
            pVuMeter->xStart += pVuMeter->Xcenter;
            pVuMeter->yStart += pVuMeter->Ycenter;
            VuCheckCoords(pVuMeter, &pVuMeter->xStart, &pVuMeter->yStart);
            pVuMeter->xStart += pVuMeter->hdr.left;
            pVuMeter->yStart += pVuMeter->hdr.top;

            SetLineType(SOLID_LINE);
            SetLineThickness(GetState(pVuMeter, VU_POINTER_THICK) ? THICK_LINE : NORMAL_LINE);

            switch (pVuMeter->PointerType) {
                case VU_POINTER_NORMAL:
                case VU_POINTER_3D:
                    for (k = 1; k<=(pVuMeter->PointerWidth >> 1); k++) { //GetState(pVuMeter, VU_POINTER_THICK) ?  2 :  1) {
                        GetCirclePoint(pVuMeter->PointerStart, (pVuMeter->degAngle - k) % 360, &Xn2, &Yn2);
                        GetCirclePoint(pVuMeter->PointerStart, (pVuMeter->degAngle + k) % 360, &Xn3, &Yn3);
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
                        SetColor(pVuMeter->PointerType == VU_POINTER_3D ? pVuMeter->hdr.pGolScheme->EmbossLtColor : pVuMeter->hdr.pGolScheme->EmbossDkColor);
                        if (!Line(pVuMeter->xStart, pVuMeter->yStart, Xn2, Yn2))
                            return (0);

                        SetColor(pVuMeter->hdr.pGolScheme->EmbossDkColor);
                        if (!Line(pVuMeter->xStart, pVuMeter->yStart, Xn3, Yn3))
                            return (0);
                    }
                    break;

                case VU_POINTER_WIREFRAME:
                    GetCirclePoint(pVuMeter->PointerStart, pVuMeter->degAngle - (pVuMeter->PointerWidth >> 1) % 360, &Xn2, &Yn2);
                    GetCirclePoint(pVuMeter->PointerStart, pVuMeter->degAngle + (pVuMeter->PointerWidth >> 1) % 360, &Xn3, &Yn3);
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

                    SetColor(pVuMeter->hdr.pGolScheme->EmbossDkColor);
                    if (!Line(pVuMeter->xStart, pVuMeter->yStart, Xn2, Yn2))
                        return (0);
                    if (!Line(Xn2, Yn2, Xn3, Yn3))
                        return (0);
                    if (!Line(pVuMeter->xStart, pVuMeter->yStart, Xn3, Yn3))
                        return (0);
                    break;

                case VU_POINTER_NEEDLE:
                    SetColor(pVuMeter->hdr.pGolScheme->EmbossDkColor);
                    GetCirclePoint(pVuMeter->PointerStart, pVuMeter->degAngle % 360, &Xn2, &Yn2);
                    Xn2 += pVuMeter->Xcenter;
                    Yn2 += pVuMeter->Ycenter;
                    VuCheckCoords(pVuMeter, &Xn2, &Yn2);
                    Xn2 += pVuMeter->hdr.left;
                    Yn2 += pVuMeter->hdr.top;
                    if (!Line(pVuMeter->xStart, pVuMeter->yStart, Xn2, Yn2))
                        return (0);
                    break;
            }


            SetLineThickness(NORMAL_LINE);
            pVuMeter->state = VU_STATE_IDLE;
            if (pVuMeter->currentValue == pVuMeter->newValue) {
                ClrState(pVuMeter, VU_DRAW_UPDATE | VU_DRAWALL | VU_DRAW_ANIMATING);
                return (1); // Finished updating!
            } else {
                INT16 AnimationIncrement;
                if (pVuMeter->currentValue < pVuMeter->newValue) {
                    if(pVuMeter->PointerSpeed==0) {
                        pVuMeter->currentValue = pVuMeter->newValue;
                    } else {
                        AnimationIncrement = ((pVuMeter->newValue - pVuMeter->currentValue) >> pVuMeter->PointerSpeed);
                        if (AnimationIncrement == 0)
                            AnimationIncrement = 1;
                        pVuMeter->currentValue += AnimationIncrement;
                    }
                } else if (pVuMeter->currentValue > pVuMeter->newValue) {
                    if(pVuMeter->PointerSpeedDecay==0) {
                        pVuMeter->currentValue = pVuMeter->newValue;
                    } else {
                        AnimationIncrement = -((pVuMeter->currentValue - pVuMeter->newValue) >> pVuMeter->PointerSpeedDecay);
                        if (AnimationIncrement == 0)
                            AnimationIncrement = -1;
                        pVuMeter->currentValue += AnimationIncrement;
                    }
                }
            }
            SetState(pVuMeter, VU_DRAW_ANIMATING);
    }

    return (1);
}

#endif // USE_VUMETER
