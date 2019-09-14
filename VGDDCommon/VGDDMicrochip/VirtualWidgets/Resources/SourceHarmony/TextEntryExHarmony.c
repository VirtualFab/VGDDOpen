// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// TextEntryEx - Harmony version
// *****************************************************************************
// FileName:        textentryex.c
// Processor:       PIC32
// Compiler:        MPLAB XC32
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
//  2014/08/31  Harmony Version
//  2014/10/19  Fixed TeExTranslateMsg bug with capacitive touchscreen
// *****************************************************************************

#include "textentryex.h"

/*********************************************************************
 * Function: TEXTENTRYEX *TeExCreate(uint16_t ID, int16_t left, int16_t top, int16_t right, int16_t bottom, uint16_t state
 *					int16_t horizontalKeys, int16_t verticalKeys, GFX_XCHAR *pText[],
 *					void *pBuffer, uint16_t bufferLength, void *pDisplayFont,
 *					GFX_GOL_OBJ_SCHEME *pScheme)
 *
 *
 * Notes:
 *
 ********************************************************************/
TEXTENTRYEX *TeExCreate (
        uint16_t    ID,                      // Unique ID for the Widget
        int16_t     left,                    // Left
        int16_t     top,                     // Top
        int16_t     right,                   // Right
        int16_t     bottom,                  // Bottom
        uint16_t    state,                   // Initial state for the Widget - TEEX_DRAW to simply draw it
        GFX_XCHAR   *pText[],                // Array for keys texts
        GFX_XCHAR   *pTextAlternate[],       // Array for alternate keys texts
        GFX_XCHAR   *pTextShift[],           // Array for shift keys texts
        GFX_XCHAR   *pTextShiftAlternate[],  // Array for shift keys texts
        int16_t     aCommandKeys[],          // Array of command key indexes
        void        *pBuffer,                // Buffer where to store typed text - output of the Widget
        void        *pBitmapReleasedKey,     // Bitmap to draw for the released key
        void        *pBitmapPressedKey,      // Bitmap to draw for the pressed key
        void        *pDisplayFont,           // Font for displaying typed text
        void        *Params,                 // Rest of the widget's parameters
        GFX_GOL_OBJ_SCHEME  *pScheme         // GOL scheme for the rest of the Widget
        ) {
    TEXTENTRYEX *pTeEx = NULL; //TextEntryEx

    uint8_t *p = Params, i, cs = 0;
    for (i = 0; i<*(uint8_t *) Params; i++) cs ^= *p++;
    if (cs != *p) return NULL;
    p = Params;

    pTeEx = (TEXTENTRYEX *) GFX_malloc(sizeof (TEXTENTRYEX));
    if (pTeEx == NULL)
        return (NULL);

    pTeEx->hdr.ID = ID;
    pTeEx->hdr.pNxtObj = NULL;
    pTeEx->hdr.type = GFX_GOL_TEXTENTRY_TYPE; // set object type - set to OBJ_TEXTENTRY for GOL to recognize it as a focusable object (see GOLCanBeFocused() in GOL.c)
    pTeEx->hdr.left = left; // left parameter of the text-entry
    pTeEx->hdr.top = top; // top parameter of the text-entry
    pTeEx->hdr.right = right; // right parameter of the text-entry
    pTeEx->hdr.bottom = bottom; // bottom parameter of the text-entry
    pTeEx->hdr.state = state; // State of the Text-Entry

    pTeEx->radius = (int16_t) ((p[1]) << 8) + p[2]; // Radius for the keys buttons
    pTeEx->totalKeys = (int16_t) (p[3] << 8) + p[4]; // Total number of keys
    pTeEx->totalKeysAlternate = (int16_t) (p[5] << 8) + p[6]; // Total number of alternate keys
    pTeEx->totalKeysShift = (int16_t) (p[7] << 8) + p[8]; // Total number of shift keys
    pTeEx->totalKeysShiftAlternate = (int16_t) (p[9] << 8) + p[10]; // Total number of shift keys
    pTeEx->horizontalKeys = (int16_t) (p[11] << 8) + p[12]; // number of horizontal keys
    pTeEx->verticalKeys = (int16_t) (p[13] << 8) + p[14]; // number of vertical keys
    pTeEx->VerticalKeySpacing = (int16_t) (p[17] << 8) + p[18]; // Vertical spacing (in pixels) between keys and from widget's edges
    pTeEx->HorizontalKeySpacing = (int16_t) (p[19] << 8) + p[20]; // Horizontal spacing (in pixels) between keys and from widget's edges

    pTeEx->CurrentLength = 0; // current length of text
    pTeEx->pHeadOfList = NULL;
    TeExSetBuffer(pTeEx, pBuffer, (int16_t) (p[15] << 8) + p[16]); // set the text to be displayed buffer length is also initialized in this call
            pTeEx->pActiveKey = NULL;
    pTeEx->hdr.DrawObj = TeExDraw; // draw function
    pTeEx->hdr.actionGet = GFX_TeExActionGet; // message function
    pTeEx->hdr.actionSet = GFX_TeExActionSet; // default message function
    pTeEx->hdr.FreeObj = TeExDelKeyMembers; // free function

    // Set the color scheme to be used
    if (pScheme == NULL)
        return (NULL);
    pTeEx->hdr.pGolScheme = (GFX_GOL_OBJ_SCHEME *) pScheme;

    // Set the font to be used
    if (pDisplayFont == NULL)
        Nop();
        // TODO: pTeEx->pDisplayFont = (void *) &DRV_TOUCHSCREEN_FONT;
    else
        pTeEx->pDisplayFont = pDisplayFont;

    // Set the ReleasedKey and PressedKey bitmaps
    pTeEx->pBitmapReleasedKey = pBitmapReleasedKey;
    pTeEx->pBitmapPressedKey = pBitmapPressedKey;
    if(pTeEx->pBitmapReleasedKey != NULL) {
        pTeEx->bitmapWidth = GFX_ImageWidthGet((void *) pTeEx->pBitmapReleasedKey);
        pTeEx->bitmapHeight = GFX_ImageHeightGet((void *) pTeEx->pBitmapReleasedKey);
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
    GFX_GOL_ObjectAdd(GFX_INDEX_0,(GFX_GOL_OBJ_HEADER *) pTeEx);
    return (pTeEx);
} //end TeCreate()

int16_t xPolyPathLeft, yPolyPathTop;
int16_t xPolyPathSize, yPolyPathSize;
int16_t xPolyPathFirst, yPolyPathFirst;
int16_t xPolyPathLast, yPolyPathLast;

void DrawPolyPathInit(int16_t left, int16_t top, int16_t right, int16_t bottom, int16_t xStart, int16_t yStart) {
    xPolyPathLeft=left;
    yPolyPathTop=top;
    xPolyPathSize = right - left;
    yPolyPathSize = bottom - top;
    xPolyPathFirst = left + ((int32_t)(xPolyPathSize * xStart)>>7);
    yPolyPathFirst = top + ((int32_t)(yPolyPathSize * yStart)>>7);
    xPolyPathLast=xStart;
    yPolyPathLast=yStart;
}

void DrawPolyPath(int16_t xEnd, int16_t yEnd) {
    int16_t x1,y1,x2,y2;
    x1 = xPolyPathLeft+((int32_t)(xPolyPathSize * (xPolyPathLast))>>7);
    y1 = yPolyPathTop+((int32_t)(yPolyPathSize * (yPolyPathLast))>>7);
    x2 = xPolyPathLeft+((int32_t)(xPolyPathSize * (xPolyPathLast+xEnd))>>7);
    y2 = yPolyPathTop+((int32_t)(yPolyPathSize * (yPolyPathLast+yEnd))>>7);
    while (!GFX_LineDraw(GFX_INDEX_0,x1, y1, x2, y2));
    xPolyPathLast += xEnd;
    yPolyPathLast += yEnd;
}

void DrawPolyPathClose(void) {
    while (!GFX_LineDraw(GFX_INDEX_0,xPolyPathLeft+((int32_t)(xPolyPathSize * (xPolyPathLast))>>7), yPolyPathTop+((int32_t)(yPolyPathSize * (yPolyPathLast))>>7), xPolyPathFirst, yPolyPathFirst));
}

/*********************************************************************
 * Function: uint16_t TeExDraw(void *pObj)
 *
 * Notes: This function draws the keys with their appropriate text
 *
 ********************************************************************/
GFX_STATUS TeExDraw(void *pObj) {
    static GFX_COLOR faceClr, embossLtClr, embossDkClr;
    static uint16_t xText, yText;
    static GFX_XCHAR xCharTmp;
    static TEEX_KEYMEMBER *pKeyTemp = NULL;

    static uint16_t CountOfKeys = 0;
    static uint16_t counter = 0;
    static GFX_XCHAR hideChar[2] = {0x2A, 0x00};
    static uint16_t bitmapLeft,bitmapTop;
    GFX_XCHAR *KeyText;

    GFX_COLOR color1, color2;
    GFX_GOL_OBJ_SCHEME  *pScheme;

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

    pScheme=pTeEx->hdr.pGolScheme;

    while (1) {
        if (GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT)
            return (0);

        switch (state) {
            case TEEX_START:

                if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_HIDE)) {
                    GFX_ColorSet(GFX_INDEX_0,pScheme->CommonBkColor);
                    state = TEEX_HIDE_WIDGET;
                    // no break here so it falls through to the TEEX_HIDE_WIDGET state.
                } else {
                    if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW)) {
                        if (pTeEx->radius == 0) {
                            color1 = pScheme->EmbossDkColor;
                            color2 = pScheme->EmbossLtColor;
                        } else {
                            color1 = pScheme->Color0;
                            color2 = pScheme->Color0;
                        }

                        /************DRAW THE WIDGET PANEL*****************************/
                        GFX_GOL_PanelParameterSet
                                (
                                GFX_INDEX_0,
                                pTeEx->hdr.left,
                                pTeEx->hdr.top,
                                pTeEx->hdr.right,
                                pTeEx->hdr.bottom,
                                0,
                                pScheme->Color0, //face color of panel
                                color1, //emboss dark color
                                color2, //emboss light color
                                NULL,
                                GFX_FILL_STYLE_COLOR,
                                pScheme->EmbossSize
                                );
                        state = TEEX_DRAW_PANEL;
                        break;
                    }
                        // update the keys (if TEEX_UPDATEEX_TEXT is also set it will also be redrawn)
                        // at the states after the keys are updated
                    else if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW_UPDATE)) {
                        GFX_GOL_ObjectStateClear(pTeEx, TEEX_KEY_PRESSED);
                        state = TEEX_DRAW_KEY_INIT;
                        break;
                    }
                    else if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_UPDATE_KEY)) {
                        state = TEEX_DRAW_KEY_INIT;
                        break;
                    }

                        // check if updating only the text displayed
                    else if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_UPDATE_TEXT)) {
                        state = TEEX_UPDATE_STRING_INIT;
                        break;
                    }
                }

                /*hide the widget*/
            case TEEX_HIDE_WIDGET:
                // this state only gets entered if GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT immediately after while(1) returns a 0.
                if (!GFX_BarDraw(GFX_INDEX_0,pTeEx->hdr.left, pTeEx->hdr.top, pTeEx->hdr.right, pTeEx->hdr.bottom))
                    return (0);
                else {
                    state = TEEX_START;
                    return (1);
                }

                /*Draw the widget of the Text-Entry*/
            case TEEX_DRAW_PANEL:
                if (!GFX_GOL_PanelDraw(GFX_INDEX_0))
                    return (0);
                state = TEEX_INIT_DRAW_EDITBOX;

            case TEEX_INIT_DRAW_EDITBOX:

                //Draw the editbox
                GFX_GOL_PanelParameterSet
                        (
                        GFX_INDEX_0,
                        pTeEx->hdr.left,
                        pTeEx->hdr.top,
                        pTeEx->hdr.right,
                        pTeEx->hdr.top + GFX_TextStringHeightGet(pTeEx->pDisplayFont) + pScheme->EmbossSize,
                        0,
                        pScheme->Color1,
                        pScheme->EmbossDkColor,
                        pScheme->EmbossLtColor,
                        NULL,
                        GFX_FILL_STYLE_COLOR,
                        pScheme->EmbossSize
                        );

                state = TEEX_DRAW_EDITBOX;

            case TEEX_DRAW_EDITBOX:
                if (!GFX_GOL_PanelDraw(GFX_INDEX_0))
                    return (0);
                state = TEEX_DRAW_KEY_INIT;

                /* ********************************************************************* */
                /*                  Update the keys                                      */
                /* ********************************************************************* */
            case TEEX_DRAW_KEY_INIT:
                embossLtClr = pScheme->EmbossLtColor;
                embossDkClr = pScheme->EmbossDkColor;
                faceClr = pScheme->Color0;

                if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW_UPDATE)) {
                    CountOfKeys = 0;
                    pKeyTemp = pTeEx->pHeadOfList;
                } else if ((GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW) != TEEX_DRAW) && (pTeEx->pActiveKey->update == true)) {
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
                    if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW) != TEEX_DRAW && GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW_UPDATE) != TEEX_DRAW_UPDATE) {
                        if (pKeyTemp->update == true || GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW_UPDATE)) {
                            // set the colors needed
                            if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_KEY_PRESSED)) {
                                // If a bitmap has been specified, then draw it and skip Panel settings
                                if(pTeEx->pBitmapPressedKey!=NULL){
                                    if(!GFX_ImageDraw(GFX_INDEX_0,pKeyTemp->left+bitmapLeft, pKeyTemp->top+bitmapTop,pTeEx->pBitmapPressedKey))
                                        return (0);
                                    state = TEEX_DRAW_KEY_DRAW_PANEL;
                                    break;
                                }
                                embossLtClr = pScheme->EmbossDkColor;
                                embossDkClr = pScheme->EmbossLtColor;
                                faceClr = pScheme->Color1;
                            } else {
                                // If a bitmap has been specified, then draw it and skip Panel settings
                                if(pTeEx->pBitmapReleasedKey!=NULL){
                                    if(!GFX_ImageDraw(GFX_INDEX_0,pKeyTemp->left+bitmapLeft, pKeyTemp->top+bitmapTop,pTeEx->pBitmapReleasedKey))
                                        return (0);
                                    state = TEEX_DRAW_KEY_DRAW_PANEL;
                                    break;
                                }
                                embossLtClr = pScheme->EmbossLtColor;
                                embossDkClr = pScheme->EmbossDkColor;
                                faceClr = pScheme->Color0;
                            }
                        } else {
                            state = TEEX_DRAW_KEY_UPDATE;
                            break;
                        }
                    }

                    // If a bitmap has been specified as ReleasedKey, then draw it and skip Panel settings
                    if(pTeEx->pBitmapReleasedKey!=NULL){
                        if(!GFX_ImageDraw(GFX_INDEX_0,pKeyTemp->left+bitmapLeft, pKeyTemp->top+bitmapTop,pTeEx->pBitmapReleasedKey))
                            return (0);
                        state = TEEX_DRAW_KEY_DRAW_PANEL;
                        break;
                    }

                    if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_DISABLED) == TEEX_DISABLED) {
                        faceClr = pScheme->ColorDisabled;
                        GFX_ColorSet(GFX_INDEX_0,faceClr);
                    }

#ifdef USE_GRADIENT
                    // set the gradient parameters
                    SetGOLPanelGradient(pScheme);
    #endif

                    // set up the panel
                    GFX_GOL_PanelParameterSet
                            (
                            GFX_INDEX_0,
                            pKeyTemp->left + pTeEx->radius,
                            pKeyTemp->top + pTeEx->radius,
                            pKeyTemp->right - pTeEx->radius,
                            pKeyTemp->bottom - pTeEx->radius,
                            pTeEx->radius,
                            faceClr,
                            embossLtClr,
                            embossDkClr,
                            NULL,
                            GFX_FILL_STYLE_COLOR,
                            pScheme->EmbossSize
                            );

                    state = TEEX_DRAW_KEY_DRAW_PANEL;
                } else { // End of key drawing
                    TeExDrawCapsLock(pTeEx);
                    GFX_GOL_ObjectStateClear(pTeEx, TEEX_DRAW_UPDATE);
                    state = TEEX_UPDATE_STRING_INIT;
                    break;
                }

            case TEEX_DRAW_KEY_DRAW_PANEL:
                // Don't call PanelDrawTask if bitmaps have been specified
                if(pTeEx->pBitmapReleasedKey==NULL){
                    if (!GFX_GOL_PanelDraw(GFX_INDEX_0))
                        return (0);
                }

                // reset the update flag since the key panel is already redrawn
                pKeyTemp->update = false;

                //set the text coordinates of the drawn key
                int16_t textWidth;
                textWidth=pKeyTemp->textWidth;
                if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_ALT_ACTIVE)) {
                    if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_SHIFT_ACTIVE) && *(pKeyTemp->pKeyNameShiftAlternate) != 0)
                        textWidth = pKeyTemp->textWidthShiftAlternate;
                    else if (*(pKeyTemp->pKeyNameAlternate) != 0)
                        textWidth = pKeyTemp->textWidthAlternate;
                } else if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_SHIFT_ACTIVE) && *(pKeyTemp->pKeyNameShift) != 0) {
                    textWidth = pKeyTemp->textWidthShift;
                }

                xText = ((pKeyTemp->left) + (pKeyTemp->right) - (textWidth)) >> 1;
                yText = ((pKeyTemp->bottom) + (pKeyTemp->top) - (pKeyTemp->textHeight)) >> 1;

                //set color of text
                // if the object is disabled, draw the disabled colors
                if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_DISABLED) == TEEX_DISABLED) {
                    GFX_ColorSet(GFX_INDEX_0,pScheme->TextColorDisabled);
                } else {
                    if ((GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW) != TEEX_DRAW) && (GFX_GOL_ObjectStateGet(pTeEx, TEEX_KEY_PRESSED)) == TEEX_KEY_PRESSED) {
                        GFX_ColorSet(GFX_INDEX_0,pScheme->TextColor1);
                    } else {
                        GFX_ColorSet(GFX_INDEX_0,pScheme->TextColor0);
                    }
                }

                // set the font to be used
                GFX_FontSet(GFX_INDEX_0,pScheme->pFont);

                state = TEEX_DRAW_KEY_TEXT;

            case TEEX_DRAW_KEY_TEXT:
                switch (pKeyTemp->command) {
                    case TEEX_BKSP_COM:
                        KeyText = NULLSTRING;
                        // d="m 90,20 20,-20 80,0 0,40 -80,0 z"
                        GFX_LineStyleSet(GFX_INDEX_0,GFX_LINE_STYLE_THICK_SOLID);
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
                        GFX_LineStyleSet(GFX_INDEX_0,GFX_LINE_STYLE_THIN_SOLID);

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
                        GFX_LineStyleSet(GFX_INDEX_0,GFX_LINE_STYLE_THICK_SOLID);
                        DrawPolyPathInit(pKeyTemp->left,pKeyTemp->top,pKeyTemp->right,pKeyTemp->bottom,64,30);
                        DrawPolyPath(-30,40);
                        DrawPolyPath(20,0);
                        DrawPolyPath(0,30);
                        DrawPolyPath(20,0);
                        DrawPolyPath(0,-30);
                        DrawPolyPath(20,0);
                        DrawPolyPathClose();
                        GFX_LineStyleSet(GFX_INDEX_0,GFX_LINE_STYLE_THIN_SOLID);

                        break;
                    case TEEX_ALT_COM:
                        if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_ALT_ACTIVE))
                            KeyText = NORMALSTRING;
                        else
                            KeyText = ALTERNATESTRING;
                        break;
                    default: {
                        KeyText = pKeyTemp->pKeyName;
                        if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_ALT_ACTIVE)) {
                            if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_SHIFT_ACTIVE) && *(pKeyTemp->pKeyNameShiftAlternate) != 0)
                                KeyText = pKeyTemp->pKeyNameShiftAlternate;
                            else if (*(pKeyTemp->pKeyNameAlternate) != 0)
                                KeyText = pKeyTemp->pKeyNameAlternate;
                        } else if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_SHIFT_ACTIVE) && *(pKeyTemp->pKeyNameShift) != 0) {
                            KeyText = pKeyTemp->pKeyNameShift;
                        }
                    }
                }

                //set the clipping region for the single key
                GFX_TextAreaLeftSet(GFX_INDEX_0,pKeyTemp->left);
                GFX_TextAreaTopSet(GFX_INDEX_0,pKeyTemp->top);
                GFX_TextAreaRightSet(GFX_INDEX_0,pKeyTemp->right);
                GFX_TextAreaBottomSet(GFX_INDEX_0,pKeyTemp->bottom);
                
                //output the text
                if (!GFX_TextStringDraw(GFX_INDEX_0,xText, yText, KeyText, 0))
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
                    if ((pTeEx->CurrentLength == pTeEx->outputLenMax) && (GFX_GOL_ObjectStateGet(pTeEx, TEEX_UPDATE_TEXT))) {
                        state = TEEX_START;
                        return (1);
                    }
                }

                //set the clipping region
                GFX_TextAreaLeftSet(GFX_INDEX_0,  pTeEx->hdr.left  + pScheme->EmbossSize);
                GFX_TextAreaTopSet(GFX_INDEX_0,   pTeEx->hdr.top   + pScheme->EmbossSize);
                GFX_TextAreaRightSet(GFX_INDEX_0, pTeEx->hdr.right - pScheme->EmbossSize);
                GFX_TextAreaBottomSet(GFX_INDEX_0,pTeEx->hdr.top   + pScheme->EmbossSize +
                        GFX_TextStringHeightGet(pTeEx->pDisplayFont));

                if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_DRAW)) {

                    // update only the displayed text
                    // position the string rendering on the right position
                    if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_ECHO_HIDE)) {

                        // fill the area with '*' character so we use the width of this character
                        GFX_TextCursorPositionSet
                                (
                                GFX_INDEX_0,
                                pTeEx->hdr.right - 4 - pScheme->EmbossSize - (GFX_TextStringWidthGet(hideChar, pTeEx->pDisplayFont) * pTeEx->CurrentLength),
                                pTeEx->hdr.top + pScheme->EmbossSize
                                );
                    } else {
                        GFX_TextCursorPositionSet
                                (
                                GFX_INDEX_0,
                                pTeEx->hdr.right - 4 - pScheme->EmbossSize - GFX_TextStringWidthGet(pTeEx->pTeOutput, pTeEx->pDisplayFont),
                                pTeEx->hdr.top + pScheme->EmbossSize
                                );
                    }
                } else if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_UPDATE_TEXT)) {

                    // erase the current text by drawing a bar over the edit box area
                    GFX_ColorSet(GFX_INDEX_0,pScheme->Color1);

                    // we have to make sure we finish the GFX_BarDraw() first before we continue.
                    state = TEEX_WAIT_ERASE_EBOX_AREA;
                    break;
                } else {
                    //SetClip(0); //reset the clipping
                    state = TEEX_START;
                    return (1);
                }

                counter = 0;
                state = TEEX_UPDATE_STRING;
                break;

            case TEEX_WAIT_ERASE_EBOX_AREA:
                if (!GFX_BarDraw
                        (
                        GFX_INDEX_0,
                        pTeEx->hdr.left + pScheme->EmbossSize,
                        pTeEx->hdr.top + pScheme->EmbossSize,
                        pTeEx->hdr.right - pScheme->EmbossSize,
                        pTeEx->hdr.top + pScheme->EmbossSize + GFX_TextStringHeightGet(pTeEx->pDisplayFont)
                        ))
                    return 0;

                // check if the command given is delete a character
                if (pTeEx->pActiveKey->command == TEEX_BKSP_COM) {
                    *(pTeEx->pTeOutput + (--pTeEx->CurrentLength)) = 0;
                }

                // position the cursor to the start of string rendering
                // notice that we need to remove the characters first before we position the cursor when
                // deleting characters
                if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_ECHO_HIDE)) {

                    // fill the area with '*' character so we use the width of this character
                    GFX_TextCursorPositionSet
                            (
                            GFX_INDEX_0,
                            pTeEx->hdr.right - 4 - pScheme->EmbossSize - (GFX_TextStringWidthGet(hideChar, pTeEx->pDisplayFont) * (pTeEx->CurrentLength)),
                            pTeEx->hdr.top + pScheme->EmbossSize
                            );
                } else {
                    GFX_TextCursorPositionSet
                            (
                            GFX_INDEX_0,
                            pTeEx->hdr.right - 4 - pScheme->EmbossSize - GFX_TextStringWidthGet(pTeEx->pTeOutput, pTeEx->pDisplayFont),
                            pTeEx->hdr.top + pScheme->EmbossSize
                            );
                }

                counter = 0;
                state = TEEX_UPDATE_STRING;
                // add a break here to force a check of GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT so when last GFX_BarDraw() function is still
                // ongoing it will wait for it to finish.
                break;

            case TEEX_UPDATE_STRING:

                //output the text
                GFX_ColorSet(GFX_INDEX_0,pScheme->TextColor1);
                GFX_FontSet(GFX_INDEX_0,pTeEx->pDisplayFont);

                // this is manually doing the OutText() function but with the capability to replace the
                // characters to the * character when hide echo is enabled.
                xCharTmp = *((pTeEx->pTeOutput) + counter);
                if (xCharTmp < (GFX_XCHAR) 15) {

                    // update is done time to return to start and exit with success
                    //SetClip(0); //reset the clipping
                    state = TEEX_START;
                    return (1);
                } else {
                    if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_ECHO_HIDE))
                        GFX_TextCharDraw(GFX_INDEX_0,0x2A);
                    else
                        GFX_TextCharDraw(GFX_INDEX_0,xCharTmp);
                    state = TEEX_UPDATE_CHARACTERS;
                }

            case TEEX_UPDATE_CHARACTERS:
                if (GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT) return (0);
                counter++;
                state = TEEX_UPDATE_STRING;
                break;
        } //end switch
    } // end of while(1)
} //end TeDraw()

/*********************************************************************
 * Function: TeExTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Notes: Function to check which key was pressed/released
 *
 ********************************************************************/
GFX_GOL_TRANSLATED_ACTION GFX_TeExActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg) {
    static uint32_t tickShift,tickBksp;
    int16_t param1, param2;
    TEEX_KEYMEMBER *pKeyTemp = NULL;
    TEXTENTRYEX *pTeEx;

    pTeEx = (TEXTENTRYEX *) pObj;

    // Check if disabled first
    if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_DISABLED))
        return (GFX_GOL_OBJECT_ACTION_INVALID);

    #ifndef GFX_CONFIG_USE_TOUCHSCREEN_DISABLE

    //find the total number of keys
    param1 = pMsg->param1;
    param2 = pMsg->param2;

    if ((pMsg->type == TYPE_TOUCHSCREEN)) {

        // Check if it falls in the panel of the TextEntry
        if
            (
                (pTeEx->hdr.left < pMsg->param1) &&
                (pTeEx->hdr.right > pMsg->param1) &&
                (pTeEx->hdr.top + (GFX_TextStringHeightGet(pTeEx->pDisplayFont) + (pTeEx->hdr.pGolScheme->EmbossSize << 1)) < pMsg->param2) &&
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
                            if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_LOCK_ACTIVE))
                                GFX_GOL_ObjectStateClear(pTeEx,TEEX_LOCK_ACTIVE);
                        } else if (pKeyTemp->command == TEEX_BKSP_COM) {
                            tickBksp=tick;
                        }
                    } else if (pMsg->uiEvent == EVENT_STILLPRESS) {
                        if (pKeyTemp->command == TEEX_SHIFT_COM) {
                            if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_LOCK_TRANS) != TEEX_LOCK_TRANS
                                    && GFX_GOL_ObjectStateGet(pTeEx, TEEX_LOCK_ACTIVE) != TEEX_LOCK_ACTIVE
                                    && tick - tickShift > 5000
                                    && tick - tickShift < 20000) {
                                GFX_GOL_ObjectStateSet(pTeEx, TEEX_LOCK_TRANS);
                                GFX_GOL_ObjectStateSet(pTeEx, TEEX_LOCK_ACTIVE);
                                return (TEEX_MSG_CAPSLOCK);
                            }
                        } else if (pKeyTemp->command == TEEX_BKSP_COM && (tick - tickBksp > 5000)) {
                            tickBksp=tick;
                            TeExClearBuffer(pTeEx);
                            GFX_GOL_ObjectStateSet(pTeEx, TEEX_DRAW);
                        }

                    } else if (pMsg->uiEvent == EVENT_RELEASE) {
                        pTeEx->pActiveKey = pKeyTemp;
                        pKeyTemp->update = true;

                        if (pTeEx->pActiveKey->state == TEEX_KEY_PRESSED) {
                            if (pKeyTemp->command == 0)
                                return (TEEX_MSG_ADD_CHAR);

                            //command for a TEEX_SHIFT_COM key
                            if (pKeyTemp->command == TEEX_SHIFT_COM) {
                                if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_LOCK_TRANS)) {
                                    GFX_GOL_ObjectStateClear(pTeEx,TEEX_LOCK_TRANS);
                                } else {
                                    GFX_GOL_ObjectStateClear(pTeEx,TEEX_LOCK_ACTIVE);
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
                    if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_KEY_PRESSED)) {

                        // there is a key being pressed.
                        if (pKeyTemp->index != pTeEx->pActiveKey->index) {

                            // release the currently pressed key first
                            pTeEx->pActiveKey->update = true;
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
                            pKeyTemp->update = true;
                            return (TEEX_MSG_PRESSED);
                        }

                        if (pTeEx->pActiveKey->state != TEEX_KEY_PRESSED) {
                            pTeEx->pActiveKey = pKeyTemp;
                            pKeyTemp->update = true;
                            return (TEEX_MSG_PRESSED);
                        } else {
                            return (GFX_GOL_OBJECT_ACTION_INVALID);
                        }
                    }
                } else {

                    // if the key is in the pressed state and current touch is not here
                    // then it has to be redrawn
                    if (pKeyTemp->state == TEEX_KEY_PRESSED) {
                        pTeEx->pActiveKey = pKeyTemp;
                        pKeyTemp->update = true;
                        return (TEEX_MSG_RELEASED);
                    }
                }

                //access the next link list
                pKeyTemp = pKeyTemp->pNextKey;
            } //end while
        } else {
            if ((pMsg->uiEvent == EVENT_MOVE) && (GFX_GOL_ObjectStateGet(pTeEx, TEEX_KEY_PRESSED))) {
                pTeEx->pActiveKey->update = true;
                return (TEEX_MSG_RELEASED);
            }
        }
    }

    #endif // USE_TOUCHSCREEN
    return (GFX_GOL_OBJECT_ACTION_INVALID);
} //end TeTranslateMsg()

/*********************************************************************
 * Function: TeExMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
 *
 *
 * Notes: This the default operation to change the state of the key.
 *		 Called inside GOLMsg() when GOLMsgCallback() returns a 1.
 *
 ********************************************************************/
void GFX_TeExActionSet(GFX_GOL_TRANSLATED_ACTION translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg) {
    TEXTENTRYEX *pTeEx;

    pTeEx = (TEXTENTRYEX *) pObj;

    switch (translatedMsg) {
        case TEEX_MSG_BKSP:
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_UPDATE_KEY | TEEX_UPDATE_TEXT);
            break;
        case TEEX_MSG_SPACE:
            TeExSpaceChar(pTeEx);
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_UPDATE_KEY | TEEX_UPDATE_TEXT);
            break;

        case TEEX_MSG_ENTER:
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_UPDATE_KEY);
            break;

        case TEEX_MSG_ALTERNATE:
            if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_LOCK_ACTIVE))
                GFX_GOL_ObjectStateSet(pTeEx, TEEX_SHIFT_ACTIVE);
            else
                GFX_GOL_ObjectStateClear(pTeEx, TEEX_SHIFT_ACTIVE);
            if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_ALT_ACTIVE)) {
                GFX_GOL_ObjectStateClear(pTeEx, TEEX_ALT_ACTIVE);
            } else
                GFX_GOL_ObjectStateSet(pTeEx, TEEX_ALT_ACTIVE);
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_UPDATE_KEY | TEEX_DRAW_UPDATE);
            break;

        case TEEX_MSG_SHIFT:
            if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_SHIFT_ACTIVE))
                GFX_GOL_ObjectStateClear(pTeEx, TEEX_SHIFT_ACTIVE);
            else
                GFX_GOL_ObjectStateSet(pTeEx, TEEX_SHIFT_ACTIVE);
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_UPDATE_KEY | TEEX_DRAW_UPDATE);
            break;

        case TEEX_MSG_CAPSLOCK:
            TeExDrawCapsLock(pTeEx);
            if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_LOCK_ACTIVE))
                GFX_GOL_ObjectStateClear(pTeEx, TEEX_SHIFT_ACTIVE);
            else
                GFX_GOL_ObjectStateSet(pTeEx, TEEX_SHIFT_ACTIVE);
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_UPDATE_KEY | TEEX_DRAW_UPDATE);
            break;

        case TEEX_MSG_ADD_CHAR:
            TeExAddChar(pTeEx);
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_UPDATE_KEY | TEEX_UPDATE_TEXT);
            break;

        case TEEX_MSG_PRESSED:
            (pTeEx->pActiveKey)->state = TEEX_KEY_PRESSED;
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_KEY_PRESSED | TEEX_UPDATE_KEY);
            return;

        case TEEX_MSG_RELEASED:
            (pTeEx->pActiveKey)->state = 0;
            GFX_GOL_ObjectStateClear(pTeEx, TEEX_KEY_PRESSED); // reset pressed
            GFX_GOL_ObjectStateSet(pTeEx, TEEX_UPDATE_KEY); // redraw
            return;
    }

    if (pTeEx->pActiveKey != NULL)
        (pTeEx->pActiveKey)->state = 0;
    GFX_GOL_ObjectStateClear(pTeEx, TEEX_KEY_PRESSED);
}

void TeExDrawCapsLock(TEXTENTRYEX *pTeEx) {
    TEEX_KEYMEMBER *pKeyTemp;
    pKeyTemp = pTeEx->pHeadOfList;
    while (pKeyTemp != NULL) {
        if (pKeyTemp->command == TEEX_SHIFT_COM) {
            if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_LOCK_ACTIVE)) {
                GFX_ColorSet(GFX_INDEX_0,GFX_RGBConvert(255, 0, 0));
//                GFX_GOL_ObjectStateSet(pTeEx, TEEX_DRAW_UPDATE || TEEX_UPDATE_KEY);
            } else {
                GFX_ColorSet(GFX_INDEX_0,pTeEx->hdr.pGolScheme->Color0);
            }
            int16_t bw=((pKeyTemp->right-pKeyTemp->left)>>3)+2;
            while (!GFX_CircleFillDraw(GFX_INDEX_0,pKeyTemp->left + bw+2, pKeyTemp->top + bw+2, (bw>>1)));
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
    uint16_t i;

    //clear the buffer
    for (i = 0; i < (pTeEx->outputLenMax); i++) {
        *(pTeEx->pTeOutput + i) = 0;
    }

    pTeEx->CurrentLength = 0;
}

/*********************************************************************
 * Function: void TeExSetBuffer(TEXTENTRYEX *pTe, GFX_XCHAR *pText, uint16_t size)
 *
 * Notes: This function will replace the currently used buffer.
 *		 MaxSize defines the length of the buffer. Buffer must be
 *		 a NULL terminated string.
 *
 ********************************************************************/
void TeExSetBuffer(TEXTENTRYEX *pTeEx, GFX_XCHAR *pText, uint16_t MaxSize) {
    uint16_t count = 0;
    GFX_XCHAR *pTemp;

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
 * Function: BOOL TeExIsKeyPressed(TEXTENTRYEX *pTe,uint16_t index)
 *
 * Notes: This function will check if the key was pressed. If no
 *		 key was pressed it will return false.
 *
 ********************************************************************/
bool TeExIsKeyPressed(TEXTENTRYEX *pTeEx, uint16_t index) {
    TEEX_KEYMEMBER *pTemp;

    pTemp = pTeEx->pHeadOfList;

    //search the key using the given index
    while (index != pTemp->index) {

        // catch all check
        if (pTemp == NULL)
            return (false);
        pTemp = pTemp->pNextKey;
    }

    if (pTemp->state == TEEX_KEY_PRESSED) {
        return (true);
    } else {
        return (false);
    }
}

/*********************************************************************
 * Function: BOOL TeExSetKeyCommand(TEXTENTRYEX *pTe,uint16_t index,uint16_t command)
 *
 * Notes: This function will assign a command to a particular key.
 *		 Returns true if sucessful and false if not.
 *
 ********************************************************************/
bool TeExSetKeyCommand(TEXTENTRYEX *pTeEx, uint16_t index, uint16_t command) {
    TEEX_KEYMEMBER *pTemp;

    pTemp = pTeEx->pHeadOfList;

    //search the key using the given index
    while (index != pTemp->index) {

        // catch all check
        pTemp = pTemp->pNextKey;
        if (pTemp == NULL)
            return (false);
    }

    pTemp->command = command;
    return (true);
}

/*********************************************************************
 * Function: TeExGetKeyCommand(pTe, index)
 *
 * Notes: This function will return the currently used command by a key
 *		 with the given index.
 *
 ********************************************************************/
uint16_t TeExGetKeyCommand(TEXTENTRYEX *pTeEx, uint16_t index) {
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
 * Function: BOOL TeExSetKeyText(TEXTENTRYEX *pTe,uint16_t index, GFX_XCHAR *pText)
 *
 * Notes: This function will set the string associated with the key
 *		 with the new string pText. The key to be modified is determined
 *        by the index. Returns true if sucessful and false if not.
 *
 ********************************************************************/
bool TeExSetKeyText(TEXTENTRYEX *pTeEx, uint16_t index, GFX_XCHAR *pText) {
    TEEX_KEYMEMBER *pTemp;

    pTemp = pTeEx->pHeadOfList;

    //search the key using the given index
    while (index != pTemp->index) {
        // catch all check
        if (pTemp == NULL)
            return (false);
        pTemp = pTemp->pNextKey;
    }

    // Set the the text
    pTemp->pKeyName = pText;

    return (true);
}

/*********************************************************************
 * Function: KEYMEMBER *TeExCreateKeyMembers(TEXTENTRYEX *pTe,GFX_XCHAR *pText[])
 *
 * Notes: This function will create the members of the list
 *
 ********************************************************************/
TEEX_KEYMEMBER *TeExCreateKeyMembers(TEXTENTRYEX *pTeEx, GFX_XCHAR *pText[], GFX_XCHAR *pTextAlternate[], GFX_XCHAR *pTextShift[], GFX_XCHAR *pTextShiftAlternate[], int16_t aCommandKeys[]) {
    int16_t ButtonWidth, ButtonHeight,bw;
    int16_t keyTop, keyLeft;
    uint16_t rowcount, colcount;
    uint16_t buttonIndex = 0;
    uint16_t buttonCommand = 0;
    GFX_XCHAR *buttonText;
    uint8_t i;

    TEEX_KEYMEMBER *pKl = NULL; //link list
    TEEX_KEYMEMBER *pTail = NULL;

    // determine starting positions of the keys
    keyTop = pTeEx->hdr.top + GFX_TextStringHeightGet(pTeEx->pDisplayFont) + (pTeEx->hdr.pGolScheme->EmbossSize << 1);
    keyLeft = pTeEx->hdr.left;

    //calculate the total number of keys, and width and height of each key
    ButtonWidth = (pTeEx->hdr.right - pTeEx->hdr.left+1 - (pTeEx->HorizontalKeySpacing*pTeEx->horizontalKeys)) / pTeEx->horizontalKeys;
    ButtonHeight = (pTeEx->hdr.bottom - keyTop + 1 - (pTeEx->VerticalKeySpacing*pTeEx->verticalKeys)) / pTeEx->verticalKeys;

    /*create the list and calculate the coordinates of each bottom, and the textwidth/textheight of each font*/

    //Add a list for each key
    int16_t buttonLeft,HorKeys;
    for (rowcount = 0; rowcount < pTeEx->verticalKeys; rowcount++) {
        buttonLeft = keyLeft;
        HorKeys = pTeEx->horizontalKeys;
        if ((rowcount & 0x01) && GFX_GOL_ObjectStateGet(pTeEx, TEEX_LIKEKEYBOARD)) { // Odd rows are offseted in keyboard-like layout
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

            if(GFX_GOL_ObjectStateGet(pTeEx, TEEX_LIKEKEYBOARD)) {
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
            pKl->update = false;

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
                pKl->textWidth = GFX_TextStringWidthGet(pKl->pKeyName, pTeEx->hdr.pGolScheme->pFont);
                pKl->textWidthAlternate = pKl->textWidth;
                pKl->textWidthShift = pKl->textWidth;
                pKl->textWidthShiftAlternate = pKl->textWidth;
                pKl->textHeight = GFX_TextStringHeightGet(pTeEx->hdr.pGolScheme->pFont);
            }

            if(buttonIndex<pTeEx->totalKeysAlternate) {
                pKl->pKeyNameAlternate = pTextAlternate[buttonIndex];
                if(pTextAlternate[buttonIndex][0]!=0)
                    pKl->textWidthAlternate = GFX_TextStringWidthGet(pKl->pKeyNameAlternate, pTeEx->hdr.pGolScheme->pFont);
            }
            if(buttonIndex<pTeEx->totalKeysShift) {
                pKl->pKeyNameShift = pTextShift[buttonIndex];;
                if(pTextShift[buttonIndex][0]!=0)
                    pKl->textWidthShift = GFX_TextStringWidthGet(pKl->pKeyNameShift, pTeEx->hdr.pGolScheme->pFont);
            }
            if(buttonIndex<pTeEx->totalKeysShiftAlternate) {
                pKl->pKeyNameShiftAlternate = pTextShiftAlternate[buttonIndex];;
                if(pTextShiftAlternate[buttonIndex][0]!=0)
                    pKl->textWidthShiftAlternate = GFX_TextStringWidthGet(pKl->pKeyNameShiftAlternate, pTeEx->hdr.pGolScheme->pFont);
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
    GFX_XCHAR *pPoint;

    //first determine if the array has not overflown
    if ((pTeEx->CurrentLength) < pTeEx->outputLenMax) {
        pPoint = (pTeEx->pActiveKey)->pKeyName;
        if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_ALT_ACTIVE)) {
            pPoint=NULLSTRING;
            if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_SHIFT_ACTIVE))
                pPoint = (pTeEx->pActiveKey)->pKeyNameShiftAlternate;
            if(pPoint[0]==0)
                pPoint = (pTeEx->pActiveKey)->pKeyNameAlternate;
        } else if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_SHIFT_ACTIVE)) {
            pPoint = (pTeEx->pActiveKey)->pKeyNameShift;
        } else if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_LOCK_ACTIVE)) {
            pPoint = (pTeEx->pActiveKey)->pKeyNameShift;
        }
        while (*(pPoint) != 0) {
            *(pTeEx->pTeOutput + (pTeEx->CurrentLength)) = *(pPoint)++;
        }
        if(GFX_GOL_ObjectStateGet(pTeEx,TEEX_SHIFT_ACTIVE)) {
            if (GFX_GOL_ObjectStateGet(pTeEx, TEEX_LOCK_ACTIVE)) {
                GFX_GOL_ObjectStateClear(pTeEx, TEEX_DRAW_UPDATE);
            } else {
                GFX_GOL_ObjectStateClear(pTeEx, TEEX_SHIFT_ACTIVE);
                GFX_GOL_ObjectStateSet(pTeEx, TEEX_DRAW_UPDATE);
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
