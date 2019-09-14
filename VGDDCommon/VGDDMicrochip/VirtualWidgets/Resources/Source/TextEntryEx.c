// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// TextEntryEx
// *****************************************************************************
// FileName:        TextEntryEx.c
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
//  2013/10/20	Initial release
//  2014/10/19  Fixed TeExTranslateMsg bug with capacitive touchscreen
// *****************************************************************************

#include "Graphics/Graphics.h"
#include "TextEntryEx.h"

#ifdef USE_TEXTENTRYEX

/*********************************************************************
 * Function: TEXTENTRYEX *TeExCreate(WORD ID, SHORT left, SHORT top, SHORT right, SHORT bottom, WORD state
 *					SHORT horizontalKeys, SHORT verticalKeys, XCHAR *pText[],
 *					void *pBuffer, WORD bufferLength, void *pDisplayFont,
 *					GOL_SCHEME *pScheme)
 *
 *
 * Notes:
 *
 ********************************************************************/
TEXTENTRYEX *TeExCreate (
        WORD        ID,                      // Unique ID for the Widget
        SHORT       left,                    // Left
        SHORT       top,                     // Top
        SHORT       right,                   // Right
        SHORT       bottom,                  // Bottom
        WORD        state,                   // Initial state for the Widget - TEEX_DRAW to simply draw it
        XCHAR       *pText[],                // Array for keys texts
        XCHAR       *pTextAlternate[],       // Array for alternate keys texts
        XCHAR       *pTextShift[],           // Array for shift keys texts
        XCHAR       *pTextShiftAlternate[],  // Array for shift keys texts
        SHORT       aCommandKeys[],          // Array of command key indexes
        void        *pBuffer,                // Buffer where to store typed text - output of the Widget
        void        *pBitmapReleasedKey,     // Bitmap to draw for the released key
        void        *pBitmapPressedKey,      // Bitmap to draw for the pressed key
        void        *pDisplayFont,           // Font for displaying typed text
        void        *Params,                 // Rest of the widget's parameters
        GOL_SCHEME  *pScheme                 // GOL scheme for the rest of the Widget
        ) {
    TEXTENTRYEX *pTeEx = NULL; //TextEntryEx

    BYTE *p = Params, i, cs = 0;
    for (i = 0; i<*(BYTE *) Params; i++) cs ^= *p++;
    if (cs != *p) return NULL;
    p = Params;

    pTeEx = (TEXTENTRYEX *) GFX_malloc(sizeof (TEXTENTRYEX));
    if (pTeEx == NULL)
        return (NULL);

    pTeEx->hdr.ID = ID;
    pTeEx->hdr.pNxtObj = NULL;
    pTeEx->hdr.type = OBJ_TEXTENTRY; // set object type - set to OBJ_TEXTENTRY for GOL to recognize it as a focusable object (see GOLCanBeFocused() in GOL.c)
    pTeEx->hdr.left = left; // left parameter of the text-entry
    pTeEx->hdr.top = top; // top parameter of the text-entry
    pTeEx->hdr.right = right; // right parameter of the text-entry
    pTeEx->hdr.bottom = bottom; // bottom parameter of the text-entry
    pTeEx->hdr.state = state; // State of the Text-Entry

    pTeEx->radius = (INT16) ((p[1]) << 8) + p[2]; // Radius for the keys buttons
    pTeEx->totalKeys = (INT16) (p[3] << 8) + p[4]; // Total number of keys
    pTeEx->totalKeysAlternate = (INT16) (p[5] << 8) + p[6]; // Total number of alternate keys
    pTeEx->totalKeysShift = (INT16) (p[7] << 8) + p[8]; // Total number of shift keys
    pTeEx->totalKeysShiftAlternate = (INT16) (p[9] << 8) + p[10]; // Total number of shift keys
    pTeEx->horizontalKeys = (INT16) (p[11] << 8) + p[12]; // number of horizontal keys
    pTeEx->verticalKeys = (INT16) (p[13] << 8) + p[14]; // number of vertical keys
    pTeEx->VerticalKeySpacing = (INT16) (p[17] << 8) + p[18]; // Vertical spacing (in pixels) between keys and from widget's edges
    pTeEx->HorizontalKeySpacing = (INT16) (p[19] << 8) + p[20]; // Horizontal spacing (in pixels) between keys and from widget's edges

    pTeEx->CurrentLength = 0; // current length of text
    pTeEx->pHeadOfList = NULL;
    TeExSetBuffer(pTeEx, pBuffer, (INT16) (p[15] << 8) + p[16]); // set the text to be displayed buffer length is also initialized in this call
            pTeEx->pActiveKey = NULL;
    pTeEx->hdr.DrawObj = TeExDraw; // draw function
    pTeEx->hdr.MsgObj = TeExTranslateMsg; // message function
    pTeEx->hdr.MsgDefaultObj = TeExMsgDefault; // default message function
    pTeEx->hdr.FreeObj = TeExDelKeyMembers; // free function

    // Set the color scheme to be used
    if (pScheme == NULL)
        pTeEx->hdr.pGolScheme = _pDefaultGolScheme;
    else
        pTeEx->hdr.pGolScheme = (GOL_SCHEME *) pScheme;

    // Set the font to be used
    if (pDisplayFont == NULL)
        pTeEx->pDisplayFont = (void *) &FONTDEFAULT;
    else
        pTeEx->pDisplayFont = pDisplayFont;

    // Set the ReleasedKey and PressedKey bitmaps
    pTeEx->pBitmapReleasedKey = pBitmapReleasedKey;
    pTeEx->pBitmapPressedKey = pBitmapPressedKey;
    if(pTeEx->pBitmapReleasedKey != NULL) {
        pTeEx->bitmapWidth = GetImageWidth((void *) pTeEx->pBitmapReleasedKey);
        pTeEx->bitmapHeight = GetImageHeight((void *) pTeEx->pBitmapReleasedKey);
    }

    //check if either values of horizontal keys and vertical keys are equal to zero
    if (pTeEx->totalKeys != 0) {
        //create the key members, return null if not successful
        if (TeExCreateKeyMembers(pTeEx, pText, pTextAlternate, pTextShift, pTextShiftAlternate, aCommandKeys) == NULL) {
            TeExDelKeyMembers(pTeEx);
            GFX_free(pTeEx);
            return (NULL);
        }
    }

    //Add this new widget object to the GOL list
    GOLAddObject((OBJ_HEADER *) pTeEx);
    return (pTeEx);
} //end TeCreate()

INT16 xPolyPathLeft, yPolyPathTop;
INT16 xPolyPathSize, yPolyPathSize;
INT16 xPolyPathFirst, yPolyPathFirst;
INT16 xPolyPathLast, yPolyPathLast;

void DrawPolyPathInit(INT16 left, INT16 top, INT16 right, INT16 bottom, INT16 xStart, INT16 yStart) {
    xPolyPathLeft=left;
    yPolyPathTop=top;
    xPolyPathSize = right - left;
    yPolyPathSize = bottom - top;
    xPolyPathFirst = left + ((INT32)(xPolyPathSize * xStart)>>7);
    yPolyPathFirst = top + ((INT32)(yPolyPathSize * yStart)>>7);
    xPolyPathLast=xStart;
    yPolyPathLast=yStart;
}

void DrawPolyPath(INT16 xEnd, INT16 yEnd) {
    INT16 x1,y1,x2,y2;
    x1 = xPolyPathLeft+((INT32)(xPolyPathSize * (xPolyPathLast))>>7);
    y1 = yPolyPathTop+((INT32)(yPolyPathSize * (yPolyPathLast))>>7);
    x2 = xPolyPathLeft+((INT32)(xPolyPathSize * (xPolyPathLast+xEnd))>>7);
    y2 = yPolyPathTop+((INT32)(yPolyPathSize * (yPolyPathLast+yEnd))>>7);
    while (!Line(x1, y1, x2, y2));
    xPolyPathLast += xEnd;
    yPolyPathLast += yEnd;
}

void DrawPolyPathClose(void) {
    while (!Line(xPolyPathLeft+((INT32)(xPolyPathSize * (xPolyPathLast))>>7), yPolyPathTop+((INT32)(yPolyPathSize * (yPolyPathLast))>>7), xPolyPathFirst, yPolyPathFirst));
}

/*********************************************************************
 * Function: WORD TeExDraw(void *pObj)
 *
 * Notes: This function draws the keys with their appropriate text
 *
 ********************************************************************/
WORD TeExDraw(void *pObj) {
    static GFX_COLOR faceClr, embossLtClr, embossDkClr;
    static WORD xText, yText;
    static XCHAR XcharTmp;
    static TEEX_KEYMEMBER *pKeyTemp = NULL;

    static WORD CountOfKeys = 0;
    static WORD counter = 0;
    static XCHAR hideChar[2] = {0x2A, 0x00};
    static WORD bitmapLeft,bitmapTop;
    XCHAR *KeyText;

    GFX_COLOR color1, color2;

    typedef enum {
        TEEX_START,
        TEEX_HIDE_WIDGET,
        TEEX_DRAW_PANEL,
        TEEX_INIT_DRAW_EDITBOX,
        TEEX_DRAW_EDITBOX,
        TEEX_DRAW_KEY_INIT,
        TEEX_DRAW_KEY_SET_PANEL,
        TEEX_DRAW_KEY_DRAW_PANEL,
        TEEX_DRAW_KEY_TEXT,
        TEEX_DRAW_KEY_UPDATE,
        TEEX_UPDATE_STRING_INIT,
        TEEX_UPDATE_STRING,
        TEEX_WAIT_ERASE_EBOX_AREA,
        TEEX_UPDATE_CHARACTERS,
    } TEEX_DRAW_STATES;

    static TEEX_DRAW_STATES state = TEEX_START;
    TEXTENTRYEX *pTeEx;

    pTeEx = (TEXTENTRYEX *) pObj;

    while (1) {
        if (IsDeviceBusy())
            return (0);

        switch (state) {
            case TEEX_START:

                if (GetState(pTeEx, TEEX_HIDE)) {
                    SetColor(pTeEx->hdr.pGolScheme->CommonBkColor);
                    state = TEEX_HIDE_WIDGET;
                    // no break here so it falls through to the TEEX_HIDE_WIDGET state.
                } else {
                    if (GetState(pTeEx, TEEX_DRAW)) {
                        if (pTeEx->radius == 0) {
                            color1 = pTeEx->hdr.pGolScheme->EmbossDkColor;
                            color2 = pTeEx->hdr.pGolScheme->EmbossLtColor;
                        } else {
                            color1 = pTeEx->hdr.pGolScheme->Color0;
                            color2 = pTeEx->hdr.pGolScheme->Color0;
                        }

                        /************DRAW THE WIDGET PANEL*****************************/
                        GOLPanelDraw
                                (
                                pTeEx->hdr.left,
                                pTeEx->hdr.top,
                                pTeEx->hdr.right,
                                pTeEx->hdr.bottom,
                                0,
                                pTeEx->hdr.pGolScheme->Color0, //face color of panel
                                color1, //emboss dark color
                                color2, //emboss light color
                                NULL,
                                GOL_EMBOSS_SIZE
                                );
                        state = TEEX_DRAW_PANEL;
                        break;
                    }
                        // update the keys (if TEEX_UPDATEEX_TEXT is also set it will also be redrawn)
                        // at the states after the keys are updated
                    else if (GetState(pTeEx, TEEX_DRAW_UPDATE)) {
                        ClrState(pTeEx, TEEX_KEY_PRESSED);
                        state = TEEX_DRAW_KEY_INIT;
                        break;
                    }
                    else if (GetState(pTeEx, TEEX_UPDATE_KEY)) {
                        state = TEEX_DRAW_KEY_INIT;
                        break;
                    }

                        // check if updating only the text displayed
                    else if (GetState(pTeEx, TEEX_UPDATE_TEXT)) {
                        state = TEEX_UPDATE_STRING_INIT;
                        break;
                    }
                }

                /*hide the widget*/
            case TEEX_HIDE_WIDGET:
                // this state only gets entered if IsDeviceBusy() immediately after while(1) returns a 0.
                if (!Bar(pTeEx->hdr.left, pTeEx->hdr.top, pTeEx->hdr.right, pTeEx->hdr.bottom))
                    return (0);
                else {
                    state = TEEX_START;
                    return (1);
                }

                /*Draw the widget of the Text-Entry*/
            case TEEX_DRAW_PANEL:
                if (!GOLPanelDrawTsk())
                    return (0);
                state = TEEX_INIT_DRAW_EDITBOX;

            case TEEX_INIT_DRAW_EDITBOX:

                //Draw the editbox
                GOLPanelDraw
                        (
                        pTeEx->hdr.left,
                        pTeEx->hdr.top,
                        pTeEx->hdr.right,
                        pTeEx->hdr.top + GetTextHeight(pTeEx->pDisplayFont) + GOL_EMBOSS_SIZE,
                        0,
                        pTeEx->hdr.pGolScheme->Color1;,
                        pTeEx->hdr.pGolScheme->EmbossDkColor,
                        pTeEx->hdr.pGolScheme->EmbossLtColor,
                        NULL,
                        GOL_EMBOSS_SIZE
                        );

                state = TEEX_DRAW_EDITBOX;

            case TEEX_DRAW_EDITBOX:
                if (!GOLPanelDrawTsk())
                    return (0);
                state = TEEX_DRAW_KEY_INIT;

                /* ********************************************************************* */
                /*                  Update the keys                                      */
                /* ********************************************************************* */
            case TEEX_DRAW_KEY_INIT:
                embossLtClr = pTeEx->hdr.pGolScheme->EmbossLtColor;
                embossDkClr = pTeEx->hdr.pGolScheme->EmbossDkColor;
                faceClr = pTeEx->hdr.pGolScheme->Color0;

                if (GetState(pTeEx, TEEX_DRAW_UPDATE)) {
                    CountOfKeys = 0;
                    pKeyTemp = pTeEx->pHeadOfList;
                } else if ((GetState(pTeEx, TEEX_DRAW) != TEEX_DRAW) && (pTeEx->pActiveKey->update == TRUE)) {
                    // if the active key update flag is set, only one needs to be redrawn
                    CountOfKeys = pTeEx->totalKeys  - 1;
                    pKeyTemp = pTeEx->pActiveKey;
                } else {
                    CountOfKeys = 0;
                    pKeyTemp = pTeEx->pHeadOfList;
                }

                state = TEEX_DRAW_KEY_SET_PANEL;

            case TEEX_DRAW_KEY_SET_PANEL:
                if (CountOfKeys < pTeEx->totalKeys) {
                    bitmapLeft=((pKeyTemp->right-pKeyTemp->left)-pTeEx->bitmapWidth)>>1;
                    bitmapTop=((pKeyTemp->bottom-pKeyTemp->top)-pTeEx->bitmapHeight)>>1;
                    // check if we need to draw the panel
                    if (GetState(pTeEx, TEEX_DRAW) != TEEX_DRAW && GetState(pTeEx, TEEX_DRAW_UPDATE) != TEEX_DRAW_UPDATE) {
                        if (pKeyTemp->update == TRUE || GetState(pTeEx, TEEX_DRAW_UPDATE)) {
                            // set the colors needed
                            if (GetState(pTeEx, TEEX_KEY_PRESSED)) {
                                // If a bitmap has been specified, then draw it and skip Panel settings
                                if(pTeEx->pBitmapPressedKey!=NULL){
                                    if(!PutImage(pKeyTemp->left+bitmapLeft, pKeyTemp->top+bitmapTop,pTeEx->pBitmapPressedKey,1))
                                        return (0);
                                    state = TEEX_DRAW_KEY_DRAW_PANEL;
                                    break;
                                }
                                embossLtClr = pTeEx->hdr.pGolScheme->EmbossDkColor;
                                embossDkClr = pTeEx->hdr.pGolScheme->EmbossLtColor;
                                faceClr = pTeEx->hdr.pGolScheme->Color1;
                            } else {
                                // If a bitmap has been specified, then draw it and skip Panel settings
                                if(pTeEx->pBitmapReleasedKey!=NULL){
                                    if(!PutImage(pKeyTemp->left+bitmapLeft, pKeyTemp->top+bitmapTop,pTeEx->pBitmapReleasedKey,1))
                                        return (0);
                                    state = TEEX_DRAW_KEY_DRAW_PANEL;
                                    break;
                                }
                                embossLtClr = pTeEx->hdr.pGolScheme->EmbossLtColor;
                                embossDkClr = pTeEx->hdr.pGolScheme->EmbossDkColor;
                                faceClr = pTeEx->hdr.pGolScheme->Color0;
                            }
                        } else {
                            state = TEEX_DRAW_KEY_UPDATE;
                            break;
                        }
                    }

                    // If a bitmap has been specified as ReleasedKey, then draw it and skip Panel settings
                    if(pTeEx->pBitmapReleasedKey!=NULL){
                        if(!PutImage(pKeyTemp->left+bitmapLeft, pKeyTemp->top+bitmapTop,pTeEx->pBitmapReleasedKey,1))
                            return (0);
                        state = TEEX_DRAW_KEY_DRAW_PANEL;
                        break;
                    }

                    if (GetState(pTeEx, TEEX_DISABLED) == TEEX_DISABLED) {
                        faceClr = SetColor(pTeEx->hdr.pGolScheme->ColorDisabled);
                    }

#ifdef USE_GRADIENT
                    // set the gradient parameters
                    SetGOLPanelGradient(pTeEx->hdr.pGolScheme);
    #endif

                    // set up the panel
                    GOLPanelDraw
                            (
                            pKeyTemp->left + pTeEx->radius,
                            pKeyTemp->top + pTeEx->radius,
                            pKeyTemp->right - pTeEx->radius,
                            pKeyTemp->bottom - pTeEx->radius,
                            pTeEx->radius,
                            faceClr,
                            embossLtClr,
                            embossDkClr,
                            NULL,
                            GOL_EMBOSS_SIZE
                            );

                    state = TEEX_DRAW_KEY_DRAW_PANEL;
                } else { // End of key drawing
                    TeExDrawCapsLock(pTeEx);
                    ClrState(pTeEx, TEEX_DRAW_UPDATE);
                    state = TEEX_UPDATE_STRING_INIT;
                    break;
                }

            case TEEX_DRAW_KEY_DRAW_PANEL:
                // Don't call PanelDrawTask if bitmaps have been specified
                if(pTeEx->pBitmapReleasedKey==NULL){
                    if (!GOLPanelDrawTsk())
                        return (0);
                }

                // reset the update flag since the key panel is already redrawn
                pKeyTemp->update = FALSE;

                //set the text coordinates of the drawn key
                SHORT textWidth;
                textWidth=pKeyTemp->textWidth;
                if (GetState(pTeEx, TEEX_ALT_ACTIVE)) {
                    if (GetState(pTeEx, TEEX_SHIFT_ACTIVE) && *(pKeyTemp->pKeyNameShiftAlternate) != 0)
                        textWidth = pKeyTemp->textWidthShiftAlternate;
                    else if (*(pKeyTemp->pKeyNameAlternate) != 0)
                        textWidth = pKeyTemp->textWidthAlternate;
                } else if (GetState(pTeEx, TEEX_SHIFT_ACTIVE) && *(pKeyTemp->pKeyNameShift) != 0) {
                    textWidth = pKeyTemp->textWidthShift;
                }

                xText = ((pKeyTemp->left) + (pKeyTemp->right) - (textWidth)) >> 1;
                yText = ((pKeyTemp->bottom) + (pKeyTemp->top) - (pKeyTemp->textHeight)) >> 1;

                //set color of text
                // if the object is disabled, draw the disabled colors
                if (GetState(pTeEx, TEEX_DISABLED) == TEEX_DISABLED) {
                    SetColor(pTeEx->hdr.pGolScheme->TextColorDisabled);
                } else {
                    if ((GetState(pTeEx, TEEX_DRAW) != TEEX_DRAW) && (GetState(pTeEx, TEEX_KEY_PRESSED)) == TEEX_KEY_PRESSED) {
                        SetColor(pTeEx->hdr.pGolScheme->TextColor1);
                    } else {
                        SetColor(pTeEx->hdr.pGolScheme->TextColor0);
                    }
                }

                //output the text
                MoveTo(xText, yText);

                // set the font to be used
                SetFont(pTeEx->hdr.pGolScheme->pFont);

                state = TEEX_DRAW_KEY_TEXT;

            case TEEX_DRAW_KEY_TEXT:
                switch (pKeyTemp->command) {
                    case TEEX_BKSP_COM:
                        KeyText = NULLSTRING;
                        // d="m 90,20 20,-20 80,0 0,40 -80,0 z"
                        SetLineThickness(THICK_LINE);
                        DrawPolyPathInit(pKeyTemp->left,pKeyTemp->top,pKeyTemp->right,pKeyTemp->bottom,14,64);
                        DrawPolyPath(24,-24);
                        DrawPolyPath(68,0);
                        DrawPolyPath(0,48);
                        DrawPolyPath(-68,0);
                        DrawPolyPath(-24,-24);
                        DrawPolyPathClose();

                        DrawPolyPathInit(pKeyTemp->left,pKeyTemp->top,pKeyTemp->right,pKeyTemp->bottom,70,53);
                        DrawPolyPath(22,22);

                        DrawPolyPathInit(pKeyTemp->left,pKeyTemp->top,pKeyTemp->right,pKeyTemp->bottom,70,75);
                        DrawPolyPath(22,-22);
                        SetLineThickness(NORMAL_LINE);

                        break;
                    case TEEX_SPACE_COM:
                        KeyText = SPACESTRING;
                        break;
                    case TEEX_ENTER_COM:
                        KeyText = OKSTRING;
                        break;
                    case TEEX_SHIFT_COM:
                        KeyText = NULLSTRING;
                        // m 30,0 -30,40 20,0 0,30 20,0 0,-30 20,0 z
                        SetLineThickness(THICK_LINE);
                        DrawPolyPathInit(pKeyTemp->left,pKeyTemp->top,pKeyTemp->right,pKeyTemp->bottom,64,30);
                        DrawPolyPath(-30,40);
                        DrawPolyPath(20,0);
                        DrawPolyPath(0,30);
                        DrawPolyPath(20,0);
                        DrawPolyPath(0,-30);
                        DrawPolyPath(20,0);
                        DrawPolyPathClose();
                        SetLineThickness(NORMAL_LINE);

                        break;
                    case TEEX_ALT_COM:
                        if (GetState(pTeEx, TEEX_ALT_ACTIVE))
                            KeyText = NORMALSTRING;
                        else
                            KeyText = ALTERNATESTRING;
                        break;
                    default: {
                        KeyText = pKeyTemp->pKeyName;
                        if (GetState(pTeEx, TEEX_ALT_ACTIVE)) {
                            if (GetState(pTeEx, TEEX_SHIFT_ACTIVE) && *(pKeyTemp->pKeyNameShiftAlternate) != 0)
                                KeyText = pKeyTemp->pKeyNameShiftAlternate;
                            else if (*(pKeyTemp->pKeyNameAlternate) != 0)
                                KeyText = pKeyTemp->pKeyNameAlternate;
                        } else if (GetState(pTeEx, TEEX_SHIFT_ACTIVE) && *(pKeyTemp->pKeyNameShift) != 0) {
                            KeyText = pKeyTemp->pKeyNameShift;
                        }
                    }
                }
                if (!OutText(KeyText))
                    return (0);

                state = TEEX_DRAW_KEY_UPDATE;

            case TEEX_DRAW_KEY_UPDATE:

                // update loop variables
                CountOfKeys++;
                pKeyTemp = pKeyTemp->pNextKey;

                state = TEEX_DRAW_KEY_SET_PANEL;
                break;

                /* ********************************************************************* */
                /*                  Update the displayed string                          */
                /* ********************************************************************* */
            case TEEX_UPDATE_STRING_INIT:

                // check if there are characters to remove
                if (pTeEx->pActiveKey != NULL) {
                    if (pTeEx->pActiveKey->command == TEEX_BKSP_COM) {
                        if (pTeEx->CurrentLength == 0) {
                            state = TEEX_START;
                            return (1);
                        }
                    }
                } else {

                    // check if text indeed needs to be updated
                    if ((pTeEx->CurrentLength == pTeEx->outputLenMax) && (GetState(pTeEx, TEEX_UPDATE_TEXT))) {
                        state = TEEX_START;
                        return (1);
                    }
                }

                //set the clipping region
                SetClipRgn
                        (
                        pTeEx->hdr.left + GOL_EMBOSS_SIZE,
                        pTeEx->hdr.top + GOL_EMBOSS_SIZE,
                        pTeEx->hdr.right - GOL_EMBOSS_SIZE,
                        pTeEx->hdr.top + GOL_EMBOSS_SIZE + GetTextHeight(pTeEx->pDisplayFont)
                        );

                SetClip(1); //set the clipping
                if (GetState(pTeEx, TEEX_DRAW)) {

                    // update only the displayed text
                    // position the string rendering on the right position
                    if (GetState(pTeEx, TEEX_ECHO_HIDE)) {

                        // fill the area with '*' character so we use the width of this character
                        MoveTo
                                (
                                pTeEx->hdr.right - 4 - GOL_EMBOSS_SIZE - (GetTextWidth(hideChar, pTeEx->pDisplayFont) * pTeEx->CurrentLength),
                                pTeEx->hdr.top + GOL_EMBOSS_SIZE
                                );
                    } else {
                        MoveTo
                                (
                                pTeEx->hdr.right - 4 - GOL_EMBOSS_SIZE - GetTextWidth(pTeEx->pTeOutput, pTeEx->pDisplayFont),
                                pTeEx->hdr.top + GOL_EMBOSS_SIZE
                                );
                    }
                } else if (GetState(pTeEx, TEEX_UPDATE_TEXT)) {

                    // erase the current text by drawing a bar over the edit box area
                    SetColor(pTeEx->hdr.pGolScheme->Color1);

                    // we have to make sure we finish the Bar() first before we continue.
                    state = TEEX_WAIT_ERASE_EBOX_AREA;
                    break;
                } else {
                    SetClip(0); //reset the clipping
                    state = TEEX_START;
                    return (1);
                }

                counter = 0;
                state = TEEX_UPDATE_STRING;
                break;

            case TEEX_WAIT_ERASE_EBOX_AREA:
                if (!Bar
                        (
                        pTeEx->hdr.left + GOL_EMBOSS_SIZE,
                        pTeEx->hdr.top + GOL_EMBOSS_SIZE,
                        pTeEx->hdr.right - GOL_EMBOSS_SIZE,
                        pTeEx->hdr.top + GOL_EMBOSS_SIZE + GetTextHeight(pTeEx->pDisplayFont)
                        ))
                    return 0;

                // check if the command given is delete a character
                if (pTeEx->pActiveKey->command == TEEX_BKSP_COM) {
                    *(pTeEx->pTeOutput + (--pTeEx->CurrentLength)) = 0;
                }

                // position the cursor to the start of string rendering
                // notice that we need to remove the characters first before we position the cursor when
                // deleting characters
                if (GetState(pTeEx, TEEX_ECHO_HIDE)) {

                    // fill the area with '*' character so we use the width of this character
                    MoveTo
                            (
                            pTeEx->hdr.right - 4 - GOL_EMBOSS_SIZE - (GetTextWidth(hideChar, pTeEx->pDisplayFont) * (pTeEx->CurrentLength)),
                            pTeEx->hdr.top + GOL_EMBOSS_SIZE
                            );
                } else {
                    MoveTo
                            (
                            pTeEx->hdr.right - 4 - GOL_EMBOSS_SIZE - GetTextWidth(pTeEx->pTeOutput, pTeEx->pDisplayFont),
                            pTeEx->hdr.top + GOL_EMBOSS_SIZE
                            );
                }

                counter = 0;
                state = TEEX_UPDATE_STRING;
                // add a break here to force a check of IsDeviceBusy() so when last Bar() function is still
                // ongoing it will wait for it to finish.
                break;

            case TEEX_UPDATE_STRING:

                //output the text
                SetColor(pTeEx->hdr.pGolScheme->TextColor1);
                SetFont(pTeEx->pDisplayFont);

                // this is manually doing the OutText() function but with the capability to replace the
                // characters to the * character when hide echo is enabled.
                XcharTmp = *((pTeEx->pTeOutput) + counter);
                if (XcharTmp < (XCHAR) 15) {

                    // update is done time to return to start and exit with success
                    SetClip(0); //reset the clipping
                    state = TEEX_START;
                    return (1);
                } else {
                    if (GetState(pTeEx, TEEX_ECHO_HIDE))
                        OutChar(0x2A);
                    else
                        OutChar(XcharTmp);
                    state = TEEX_UPDATE_CHARACTERS;
                }

            case TEEX_UPDATE_CHARACTERS:
                if (IsDeviceBusy()) return (0);
                counter++;
                state = TEEX_UPDATE_STRING;
                break;
        } //end switch
    } // end of while(1)
} //end TeDraw()

/*********************************************************************
 * Function: TeExTranslateMsg(void *pObj, GOL_MSG *pMsg)
 *
 * Notes: Function to check which key was pressed/released
 *
 ********************************************************************/
WORD TeExTranslateMsg(void *pObj, GOL_MSG *pMsg) {
    static DWORD tickShift,tickBksp;
    SHORT param1, param2;
    TEEX_KEYMEMBER *pKeyTemp = NULL;
    TEXTENTRYEX *pTeEx;

    pTeEx = (TEXTENTRYEX *) pObj;

    // Check if disabled first
    if (GetState(pTeEx, TEEX_DISABLED))
        return (OBJ_MSG_INVALID);

    #ifdef USE_TOUCHSCREEN

    //find the total number of keys
    param1 = pMsg->param1;
    param2 = pMsg->param2;

    if ((pMsg->type == TYPE_TOUCHSCREEN)) {

        // Check if it falls in the panel of the TextEntry
        if
            (
                (pTeEx->hdr.left < pMsg->param1) &&
                (pTeEx->hdr.right > pMsg->param1) &&
                (pTeEx->hdr.top + (GetTextHeight(pTeEx->pDisplayFont) + (GOL_EMBOSS_SIZE << 1)) < pMsg->param2) &&
                (pTeEx->hdr.bottom > pMsg->param2)
                ) {

            /* If it fell inside the TextEntry panel, go through the link list and check which one was pressed
               At this point the touch screen event is either EVENT_MOVE or EVENT_PRESS.
             */

            //point to the head of the link list
            pKeyTemp = pTeEx->pHeadOfList;

            while (pKeyTemp != NULL) {
                if
                    (
                        (pKeyTemp->left < param1) &&
                        (pKeyTemp->right > param1) &&
                        (pKeyTemp->top < param2) &&
                        (pKeyTemp->bottom > param2)
                        ) {
                    if (pMsg->uiEvent == EVENT_PRESS) {
                        if (pKeyTemp->command == TEEX_SHIFT_COM) {
                            tickShift=tick;
                            if(GetState(pTeEx,TEEX_LOCK_ACTIVE))
                                ClrState(pTeEx,TEEX_LOCK_ACTIVE);
                        } else if (pKeyTemp->command == TEEX_BKSP_COM) {
                            tickBksp=tick;
                        }
                    } else if (pMsg->uiEvent == EVENT_STILLPRESS) {
                        if (pKeyTemp->command == TEEX_SHIFT_COM) {
                            if (GetState(pTeEx, TEEX_LOCK_TRANS) != TEEX_LOCK_TRANS
                                    && GetState(pTeEx, TEEX_LOCK_ACTIVE) != TEEX_LOCK_ACTIVE
                                    && tick - tickShift > 80
                                    && tick - tickShift < 200) {
                                SetState(pTeEx, TEEX_LOCK_TRANS);
                                SetState(pTeEx, TEEX_LOCK_ACTIVE);
                                return (TEEX_MSG_CAPSLOCK);
                            }
                        } else if (pKeyTemp->command == TEEX_BKSP_COM && (tick - tickBksp > 80)) {
                            tickBksp=tick;
                            TeExClearBuffer(pTeEx);
                            SetState(pTeEx, TEEX_DRAW);
                        }

                    } else if (pMsg->uiEvent == EVENT_RELEASE) {
                        pTeEx->pActiveKey = pKeyTemp;
                        pKeyTemp->update = TRUE;

                        if (pTeEx->pActiveKey->state == TEEX_KEY_PRESSED) {
                            if (pKeyTemp->command == 0)
                                return (TEEX_MSG_ADD_CHAR);

                            //command for a TEEX_SHIFT_COM key
                            if (pKeyTemp->command == TEEX_SHIFT_COM) {
                                if(GetState(pTeEx,TEEX_LOCK_TRANS)) {
                                    ClrState(pTeEx,TEEX_LOCK_TRANS);
                                } else {
                                    ClrState(pTeEx,TEEX_LOCK_ACTIVE);
                                }
                                return (TEEX_MSG_SHIFT);
                            }
                            
                            //command for a TEEX_DELETE_COM key
                            if (pKeyTemp->command == TEEX_BKSP_COM) {
                                return (TEEX_MSG_BKSP);
                            }

                            //command for a TEEX_SPACE_COM key 0x20
                            if (pKeyTemp->command == TEEX_SPACE_COM)
                                return (TEEX_MSG_SPACE);

                            //command for a TEEX_ENTER_COM key
                            if (pKeyTemp->command == TEEX_ENTER_COM)
                                return (TEEX_MSG_ENTER);

                            //command for a TEEX_ALT_COM key
                            if (pKeyTemp->command == TEEX_ALT_COM)
                                return (TEEX_MSG_ALTERNATE);
                        }

                        // this is a catch all backup
                        return (TEEX_MSG_RELEASED);
                    }
                    // to shift the press to another key make sure that there are no other
                    // keys currently pressed. If there is one it must be released first.
                    // check if there are previously pressed keys
                    if (GetState(pTeEx, TEEX_KEY_PRESSED)) {

                        // there is a key being pressed.
                        if (pKeyTemp->index != pTeEx->pActiveKey->index) {

                            // release the currently pressed key first
                            pTeEx->pActiveKey->update = TRUE;
                            return (TEEX_MSG_RELEASED);
                        }
                    } else {

                        // check if the active key is not pressed
                        // if not, set to press since the current touch event
                        // is either move or press
                        // check if there is an active key already set
                        // if none, set the current key as active and return a pressed mesage
                        if (pTeEx->pActiveKey == NULL) {
                            pTeEx->pActiveKey = pKeyTemp;
                            pKeyTemp->update = TRUE;
                            return (TEEX_MSG_PRESSED);
                        }

                        if (pTeEx->pActiveKey->state != TEEX_KEY_PRESSED) {
                            pTeEx->pActiveKey = pKeyTemp;
                            pKeyTemp->update = TRUE;
                            return (TEEX_MSG_PRESSED);
                        } else {
                            return (OBJ_MSG_INVALID);
                        }
                    }
                } else {

                    // if the key is in the pressed state and current touch is not here
                    // then it has to be redrawn
                    if (pKeyTemp->state == TEEX_KEY_PRESSED) {
                        pTeEx->pActiveKey = pKeyTemp;
                        pKeyTemp->update = TRUE;
                        return (TEEX_MSG_RELEASED);
                    }
                }

                //access the next link list
                pKeyTemp = pKeyTemp->pNextKey;
            } //end while
        } else {
            if ((pMsg->uiEvent == EVENT_MOVE) && (GetState(pTeEx, TEEX_KEY_PRESSED))) {
                pTeEx->pActiveKey->update = TRUE;
                return (TEEX_MSG_RELEASED);
            }
        }
    }

    return (OBJ_MSG_INVALID);
    #endif // USE_TOUCHSCREEN
} //end TeTranslateMsg()

/*********************************************************************
 * Function: TeExMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG* pMsg)
 *
 *
 * Notes: This the default operation to change the state of the key.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void TeExMsgDefault(WORD translatedMsg, void *pObj, GOL_MSG *pMsg) {
    TEXTENTRYEX *pTeEx;

    pTeEx = (TEXTENTRYEX *) pObj;

    switch (translatedMsg) {
        case TEEX_MSG_BKSP:
            SetState(pTeEx, TEEX_UPDATE_KEY | TEEX_UPDATE_TEXT);
            break;
        case TEEX_MSG_SPACE:
            TeExSpaceChar(pTeEx);
            SetState(pTeEx, TEEX_UPDATE_KEY | TEEX_UPDATE_TEXT);
            break;

        case TEEX_MSG_ENTER:
            SetState(pTeEx, TEEX_UPDATE_KEY);
            break;

        case TEEX_MSG_ALTERNATE:
            if(GetState(pTeEx,TEEX_LOCK_ACTIVE))
                SetState(pTeEx, TEEX_SHIFT_ACTIVE);
            else
                ClrState(pTeEx, TEEX_SHIFT_ACTIVE);
            if(GetState(pTeEx,TEEX_ALT_ACTIVE)) {
                ClrState(pTeEx, TEEX_ALT_ACTIVE);
            } else
                SetState(pTeEx, TEEX_ALT_ACTIVE);
            SetState(pTeEx, TEEX_UPDATE_KEY | TEEX_DRAW_UPDATE);
            break;

        case TEEX_MSG_SHIFT:
            if(GetState(pTeEx,TEEX_SHIFT_ACTIVE))
                ClrState(pTeEx, TEEX_SHIFT_ACTIVE);
            else
                SetState(pTeEx, TEEX_SHIFT_ACTIVE);
            SetState(pTeEx, TEEX_UPDATE_KEY | TEEX_DRAW_UPDATE);
            break;

        case TEEX_MSG_CAPSLOCK:
            TeExDrawCapsLock(pTeEx);
            if(GetState(pTeEx,TEEX_LOCK_ACTIVE))
                ClrState(pTeEx, TEEX_SHIFT_ACTIVE);
            else
                SetState(pTeEx, TEEX_SHIFT_ACTIVE);
            SetState(pTeEx, TEEX_UPDATE_KEY | TEEX_DRAW_UPDATE);
            break;

        case TEEX_MSG_ADD_CHAR:
            TeExAddChar(pTeEx);
            SetState(pTeEx, TEEX_UPDATE_KEY | TEEX_UPDATE_TEXT);
            break;

        case TEEX_MSG_PRESSED:
            (pTeEx->pActiveKey)->state = TEEX_KEY_PRESSED;
            SetState(pTeEx, TEEX_KEY_PRESSED | TEEX_UPDATE_KEY);
            return;

        case TEEX_MSG_RELEASED:
            (pTeEx->pActiveKey)->state = 0;
            ClrState(pTeEx, TEEX_KEY_PRESSED); // reset pressed
            SetState(pTeEx, TEEX_UPDATE_KEY); // redraw
            return;
    }

    if (pTeEx->pActiveKey != NULL)
        (pTeEx->pActiveKey)->state = 0;
    ClrState(pTeEx, TEEX_KEY_PRESSED);
}

void TeExDrawCapsLock(TEXTENTRYEX *pTeEx) {
    TEEX_KEYMEMBER *pKeyTemp;
    pKeyTemp = pTeEx->pHeadOfList;
    while (pKeyTemp != NULL) {
        if (pKeyTemp->command == TEEX_SHIFT_COM) {
            if (GetState(pTeEx, TEEX_LOCK_ACTIVE)) {
                SetColor(RGBConvert(255, 0, 0));
//                SetState(pTeEx, TEEX_DRAW_UPDATE || TEEX_UPDATE_KEY);
            } else {
                SetColor(pTeEx->hdr.pGolScheme->Color0);
            }
            SHORT bw=((pKeyTemp->right-pKeyTemp->left)>>3)+2;
            while (!FillCircle(pKeyTemp->left + bw+2, pKeyTemp->top + bw+2, (bw>>1)));
            break;
        }
        pKeyTemp = pKeyTemp->pNextKey;
    }
}

/*********************************************************************
 * Function: void TeExClearBuffer(TEXTENTRYEX *pTe)
 *
 * Notes: This function will clear the edibox and the buffer.
 *		 You must set the drawing state bit TEEX_UPDATE_TEXT
 *		 to update the TEXTENTRYEX on the screen.
 *
 ********************************************************************/
void TeExClearBuffer(TEXTENTRYEX *pTeEx) {
    WORD i;

    //clear the buffer
    for (i = 0; i < (pTeEx->outputLenMax); i++) {
        *(pTeEx->pTeOutput + i) = 0;
    }

    pTeEx->CurrentLength = 0;
}

/*********************************************************************
 * Function: void TeExSetBuffer(TEXTENTRYEX *pTe, XCHAR *pText, WORD size)
 *
 * Notes: This function will replace the currently used buffer.
 *		 MaxSize defines the length of the buffer. Buffer must be
 *		 a NULL terminated string.
 *
 ********************************************************************/
void TeExSetBuffer(TEXTENTRYEX *pTeEx, XCHAR *pText, WORD MaxSize) {
    WORD count = 0;
    XCHAR *pTemp;

    pTemp = pText;

    while (*pTemp != 0) {
        if (count >= MaxSize)
            break;
        pTemp++;
        count++;
    }

    // terminate the string
    *pTemp = 0;

    pTeEx->CurrentLength = count;
    pTeEx->outputLenMax = MaxSize - 1;
    pTeEx->pTeOutput = pText;
}

/*********************************************************************
 * Function: BOOL TeExIsKeyPressed(TEXTENTRYEX *pTe,WORD index)
 *
 * Notes: This function will check if the key was pressed. If no
 *		 key was pressed it will return FALSE.
 *
 ********************************************************************/
BOOL TeExIsKeyPressed(TEXTENTRYEX *pTeEx, WORD index) {
    TEEX_KEYMEMBER *pTemp;

    pTemp = pTeEx->pHeadOfList;

    //search the key using the given index
    while (index != pTemp->index) {

        // catch all check
        if (pTemp == NULL)
            return (FALSE);
        pTemp = pTemp->pNextKey;
    }

    if (pTemp->state == TEEX_KEY_PRESSED) {
        return (TRUE);
    } else {
        return (FALSE);
    }
}

/*********************************************************************
 * Function: BOOL TeExSetKeyCommand(TEXTENTRYEX *pTe,WORD index,WORD command)
 *
 * Notes: This function will assign a command to a particular key.
 *		 Returns TRUE if sucessful and FALSE if not.
 *
 ********************************************************************/
BOOL TeExSetKeyCommand(TEXTENTRYEX *pTeEx, WORD index, WORD command) {
    TEEX_KEYMEMBER *pTemp;

    pTemp = pTeEx->pHeadOfList;

    //search the key using the given index
    while (index != pTemp->index) {

        // catch all check
        pTemp = pTemp->pNextKey;
        if (pTemp == NULL)
            return (FALSE);
    }

    pTemp->command = command;
    return (TRUE);
}

/*********************************************************************
 * Function: TeExGetKeyCommand(pTe, index)
 *
 * Notes: This function will return the currently used command by a key
 *		 with the given index.
 *
 ********************************************************************/
WORD TeExGetKeyCommand(TEXTENTRYEX *pTeEx, WORD index) {
    TEEX_KEYMEMBER *pTemp;

    pTemp = pTeEx->pHeadOfList;

    //search the key using the given index
    while (index != pTemp->index) {

        // catch all check
        if (pTemp == NULL)
            return (0);
        pTemp = pTemp->pNextKey;
    }

    return (pTemp->command);
}

/*********************************************************************
 * Function: BOOL TeExSetKeyText(TEXTENTRYEX *pTe,WORD index, XCHAR *pText)
 *
 * Notes: This function will set the string associated with the key
 *		 with the new string pText. The key to be modified is determined
 *        by the index. Returns TRUE if sucessful and FALSE if not.
 *
 ********************************************************************/
BOOL TeExSetKeyText(TEXTENTRYEX *pTeEx, WORD index, XCHAR *pText) {
    TEEX_KEYMEMBER *pTemp;

    pTemp = pTeEx->pHeadOfList;

    //search the key using the given index
    while (index != pTemp->index) {
        // catch all check
        if (pTemp == NULL)
            return (FALSE);
        pTemp = pTemp->pNextKey;
    }

    // Set the the text
    pTemp->pKeyName = pText;

    return (TRUE);
}

/*********************************************************************
 * Function: KEYMEMBER *TeExCreateKeyMembers(TEXTENTRYEX *pTe,XCHAR *pText[])
 *
 * Notes: This function will create the members of the list
 *
 ********************************************************************/
TEEX_KEYMEMBER *TeExCreateKeyMembers(TEXTENTRYEX *pTeEx, XCHAR *pText[], XCHAR *pTextAlternate[], XCHAR *pTextShift[], XCHAR *pTextShiftAlternate[], SHORT aCommandKeys[]) {
    SHORT ButtonWidth, ButtonHeight,bw;
    SHORT keyTop, keyLeft;
    WORD rowcount, colcount;
    WORD buttonIndex = 0;
    WORD buttonCommand = 0;
    XCHAR *buttonText;
    BYTE i;

    TEEX_KEYMEMBER *pKl = NULL; //link list
    TEEX_KEYMEMBER *pTail = NULL;

    // determine starting positions of the keys
    keyTop = pTeEx->hdr.top + GetTextHeight(pTeEx->pDisplayFont) + (GOL_EMBOSS_SIZE << 1);
    keyLeft = pTeEx->hdr.left;

    //calculate the total number of keys, and width and height of each key
    ButtonWidth = (pTeEx->hdr.right - pTeEx->hdr.left+1 - (pTeEx->HorizontalKeySpacing*pTeEx->horizontalKeys)) / pTeEx->horizontalKeys;
    ButtonHeight = (pTeEx->hdr.bottom - keyTop + 1 - (pTeEx->VerticalKeySpacing*pTeEx->verticalKeys)) / pTeEx->verticalKeys;

    /*create the list and calculate the coordinates of each bottom, and the textwidth/textheight of each font*/

    //Add a list for each key
    INT16 buttonLeft,HorKeys;
    for (rowcount = 0; rowcount < pTeEx->verticalKeys; rowcount++) {
        buttonLeft = keyLeft;
        HorKeys = pTeEx->horizontalKeys;
        if ((rowcount & 0x01) && GetState(pTeEx, TEEX_LIKEKEYBOARD)) { // Odd rows are offseted in keyboard-like layout
            buttonLeft += (ButtonWidth >> 1);
            HorKeys--;
        }
        for (colcount = 0; colcount < HorKeys; colcount++) {
            buttonLeft += pTeEx->HorizontalKeySpacing;
            if (buttonIndex >= pTeEx->totalKeys)
                break;

            buttonText = pText[buttonIndex];
            bw=ButtonWidth;

            buttonCommand=0;
            for (i = 0; i < 5; i++) {
                if (aCommandKeys[i] == buttonIndex+1) {
                    buttonCommand = i+1; // Range TEEX_DELETE_COM=1 ... TEEX_ALT_COM=5
                    break;
                    }
                }

            if(GetState(pTeEx, TEEX_LIKEKEYBOARD)) {
                switch (buttonCommand) {
                    case TEEX_BKSP_COM:
                        bw = ButtonWidth * 1.5;
                        buttonText = NULLSTRING;
                        break;

                    case TEEX_SPACE_COM:
                        bw = 4 * ButtonWidth + 5 * pTeEx->HorizontalKeySpacing;
                        buttonText = NULLSTRING;
                        break;

                    case TEEX_ENTER_COM:
                        bw = ButtonWidth * 1.5;
                        buttonText = OKSTRING;
                        break;

                    case TEEX_SHIFT_COM:
                        bw = ButtonWidth * 1.5;
                        buttonText = NULLSTRING;
                        break;

                    case TEEX_ALT_COM:
                        bw = ButtonWidth * 1.8;
                        buttonText = ALTERNATESTRING;
                        break;
                }
            }
            if(buttonLeft+bw-1>pTeEx->hdr.right)
                break;

            //get storage for new entry
            pKl = (TEEX_KEYMEMBER *) GFX_malloc(sizeof (TEEX_KEYMEMBER));
            if (pKl == NULL) {
                TeExDelKeyMembers(pTeEx);
                return (NULL);
            }
            if (pTeEx->pHeadOfList == NULL)
                pTeEx->pHeadOfList = pKl;
            if (pTail == NULL) {
                pTail = pKl;
            } else {
                pTail->pNextKey = pKl;
                pTail = pTail->pNextKey;
            }

            pKl->command = buttonCommand;

            //set the index for the new list
            pKl->index = buttonIndex;

            // set update flag to off
            pKl->update = FALSE;

            //Add the text to the list and increase the index
            pKl->pKeyName = buttonText;
            pKl->pKeyNameAlternate = buttonText;
            pKl->pKeyNameShift = buttonText;
            pKl->pKeyNameShiftAlternate = buttonText;

            //calculate the x-y coordinate for each key
            pKl->left = buttonLeft;
            pKl->top = keyTop + (ButtonHeight+pTeEx->VerticalKeySpacing)*rowcount;
            pKl->right = pKl->left+ bw-1;
            pKl->bottom = pKl->top+ ButtonHeight;

            buttonLeft = pKl->right;

            //calculate the textwidth, textheight
            pKl->textWidth = 0;
            pKl->textWidthAlternate = 0;
            pKl->textWidthShift = 0;
            pKl->textWidthShiftAlternate = 0;
            pKl->textHeight = 0;

            if (*(pKl->pKeyName) != 0) {
                // Calculate the text width & height
                pKl->textWidth = GetTextWidth(pKl->pKeyName, pTeEx->hdr.pGolScheme->pFont);
                pKl->textWidthAlternate = pKl->textWidth;
                pKl->textWidthShift = pKl->textWidth;
                pKl->textWidthShiftAlternate = pKl->textWidth;
                pKl->textHeight = GetTextHeight(pTeEx->hdr.pGolScheme->pFont);
            }

            if(buttonIndex<pTeEx->totalKeysAlternate) {
                pKl->pKeyNameAlternate = pTextAlternate[buttonIndex];
                if(pTextAlternate[buttonIndex][0]!=0)
                    pKl->textWidthAlternate = GetTextWidth(pKl->pKeyNameAlternate, pTeEx->hdr.pGolScheme->pFont);
            }
            if(buttonIndex<pTeEx->totalKeysShift) {
                pKl->pKeyNameShift = pTextShift[buttonIndex];;
                if(pTextShift[buttonIndex][0]!=0)
                    pKl->textWidthShift = GetTextWidth(pKl->pKeyNameShift, pTeEx->hdr.pGolScheme->pFont);
            }
            if(buttonIndex<pTeEx->totalKeysShiftAlternate) {
                pKl->pKeyNameShiftAlternate = pTextShiftAlternate[buttonIndex];;
                if(pTextShiftAlternate[buttonIndex][0]!=0)
                    pKl->textWidthShiftAlternate = GetTextWidth(pKl->pKeyNameShiftAlternate, pTeEx->hdr.pGolScheme->pFont);
            }

            buttonIndex++;

        } //end for
    } //end for

    pTail->pNextKey = NULL;

    return (pKl);
}

/*********************************************************************
 * Function: void TeExDelKeyMembers(void *pObj)
 *
 * Notes: This function will delete the members of the list
 ********************************************************************/
void TeExDelKeyMembers(void *pObj) {
    TEEX_KEYMEMBER *pCurItem;
    TEEX_KEYMEMBER *pItem;
    TEXTENTRYEX *pTeEx;

    pTeEx = (TEXTENTRYEX *) pObj;

    pCurItem = pTeEx->pHeadOfList;

    while (pCurItem != NULL) {
        pItem = pCurItem;
        pCurItem = pCurItem->pNextKey;
        GFX_free(pItem);
    }

    pTeEx->pHeadOfList = NULL;
}

/*********************************************************************
 * Function: void TeExSpaceChar(TEXTENTRYEX *pTe)
 *
 * Notes: This function will add a space to the buffer/editbox
 ********************************************************************/
void TeExSpaceChar(TEXTENTRYEX *pTeEx) {

    //first determine if the array has not overflown
    if ((pTeEx->CurrentLength) < pTeEx->outputLenMax) {
        *(pTeEx->pTeOutput + (pTeEx->CurrentLength)) = 0x20;
        *(pTeEx->pTeOutput + (pTeEx->CurrentLength) + 1) = 0;
    } //end if
    (pTeEx->CurrentLength)++;
}

/*********************************************************************
 * Function: void TeExAddChar(TEXTENTRYEX *pTe)
 *
 * Notes: This function will add a character to the buffer/editbox
 ********************************************************************/
void TeExAddChar(TEXTENTRYEX *pTeEx) {
    XCHAR *pPoint;

    //first determine if the array has not overflown
    if ((pTeEx->CurrentLength) < pTeEx->outputLenMax) {
        pPoint = (pTeEx->pActiveKey)->pKeyName;
        if(GetState(pTeEx,TEEX_ALT_ACTIVE)) {
            pPoint=NULLSTRING;
            if(GetState(pTeEx,TEEX_SHIFT_ACTIVE))
                pPoint = (pTeEx->pActiveKey)->pKeyNameShiftAlternate;
            if(pPoint[0]==0)
                pPoint = (pTeEx->pActiveKey)->pKeyNameAlternate;
        } else if(GetState(pTeEx,TEEX_SHIFT_ACTIVE)) {
            pPoint = (pTeEx->pActiveKey)->pKeyNameShift;
        } else if(GetState(pTeEx,TEEX_LOCK_ACTIVE)) {
            pPoint = (pTeEx->pActiveKey)->pKeyNameShift;
        }
        while (*(pPoint) != 0) {
            *(pTeEx->pTeOutput + (pTeEx->CurrentLength)) = *(pPoint)++;
        }
        if(GetState(pTeEx,TEEX_SHIFT_ACTIVE)) {
            if (GetState(pTeEx, TEEX_LOCK_ACTIVE)) {
                ClrState(pTeEx, TEEX_DRAW_UPDATE);
            } else {
                ClrState(pTeEx, TEEX_SHIFT_ACTIVE);
                SetState(pTeEx, TEEX_DRAW_UPDATE);
            }
        }
    }//end if
    else {

        // it is full ignore the added key
        return;
    }

    (pTeEx->CurrentLength)++;
    // add the string terminator
    *(pTeEx->pTeOutput + pTeEx->CurrentLength) = 0;
}

#endif // USE_TEXTENTRYEX
