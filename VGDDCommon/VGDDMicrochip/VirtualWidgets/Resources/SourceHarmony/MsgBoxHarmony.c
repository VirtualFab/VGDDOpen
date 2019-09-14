// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer 
// MsgBox - Harmony version
// *****************************************************************************
// FileName:        msgbox.c
// Dependencies:    msgbox.h
// Processor:       PIC32
// Compiler:        MPLAB XC32
// Linker:          MPLAB LINK32
// Company:         VirtualFab
// Remarks:         Original material from Microchip Technology Incorporated.
//
// VirtualFab Software License Agreement:
//
// Copyright 2013-2016 Virtualfab - All rights reserved.
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
// Copyright © 2008 Microchip Technology Inc.  All rights reserved.
// Microchip licenses to you the right to use, modify, copy and distribute
// Software only when embedded on a Microchip microcontroller or digital
// signal controller, which is integrated into your product or third party
// product (pursuant to the sublicense terms in the accompanying license
// agreement).
//
// You should refer to the license agreement accompanying this Software
// for additional information regarding your rights and obligations.
//
// SOFTWARE AND DOCUMENTATION ARE PROVIDED “AS IS” WITHOUT WARRANTY OF ANY
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
// Date        	Comment
// *****************************************************************************
// 2013/09/29   Fabio Violino - Initial release
// 2014/09/07   Harmony version
// *****************************************************************************
#include "msgbox.h"

/*********************************************************************
 * Function: MsgBox  *MsgBoxCreate(uint16_t ID, uint16_t left, uint16_t top, uint16_t right, uint16_t bottom,
 *                                     uint16_t radius, MSGBOX_BUTTONS buttonsToDisplay, XCHAR *pTextMessage,
 *                                     GFX_XCHAR *pTextCaption, uint16_t state, void *pBitmapMessage,
 *                                     void *pBitmapCaption, GFX_GOL_OBJ_SCHEME *pScheme, GFX_GOL_OBJ_SCHEME *pButtonScheme,
 *                                     void *pBitmapReleasedKey, void *pBitmapPressedKey)
 *
 * Notes: Creates a MSGBOX object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
 *
 ********************************************************************/
MSGBOX *MsgBoxCreate(
        uint16_t ID, // Unique ID for the Widget
        uint16_t left, // Left
        uint16_t top, // Top
        uint16_t right, // Right
        uint16_t bottom, // Bottom
        uint16_t radius, // Radius for rounded MessageBoxes.
        MSGBOX_BUTTONS buttonsToDisplay, // Type of buttons to display (OK,Yes/No,Yes/No/Cancel) as defined by MSGBOX_BUTTONS
        GFX_XCHAR *pTextMessage, // Pointer to the text used for the main message.
        GFX_XCHAR *pTextCaption, // (optional) Pointer to the text used for the optional caption message.
        uint16_t state, // Initial state for the Widget - MSGBOX_DRAW to draw it
        void *pBitmapMessage, // (optional) Pointer to bitmap to be put aside the main message.
        void *pBitmapCaption, // (optional) Pointer to bitmap used as optional caption icon.
        GFX_GOL_OBJ_SCHEME *pScheme, // Pointer to the scheme used for the MsgBox body.
        GFX_GOL_OBJ_SCHEME *pButtonScheme, // (optional) Pointer to the scheme used for buttons.
        void *pBitmapReleasedKey, // (optional) Bitmap to draw for the released key
        void *pBitmapPressedKey // (optional) Bitmap to draw for the pressed key
        ) {
    MSGBOX *pM = NULL;
    pM = (MSGBOX *) GFX_malloc(sizeof (MSGBOX));
    if (pM == NULL)
        return (NULL);

    pM->hdr.ID = ID; // unique id assigned for referencing
    pM->hdr.pNxtObj = NULL; // initialize pointer to NULL
    pM->hdr.type = GFX_GOL_BUTTON_TYPE; // set object type - set to BUTTON for GOL to recognize it as a focusable object (see GOLCanBeFocused() in GOL.c)
    pM->hdr.left = left; // left position
    pM->hdr.top = top; // top position
    pM->hdr.right = right; // right position
    pM->hdr.bottom = bottom; // bottom position
    pM->radius = radius; // radius
    pM->pBitmapMessage = pBitmapMessage; // location of background bitmap
    pM->pBitmapCaption = pBitmapCaption; // location of caption icon bitmap
    pM->pTextMessage = pTextMessage; // location of the message text
    pM->pTextCaption = pTextCaption; // location of the caption text
    pM->hdr.state = state; // state
    pM->hdr.DrawObj = MsgBoxDraw; // draw function
    pM->hdr.actionGet = MsgBoxTranslateMsg; // message function
    pM->hdr.actionSet = NULL; //MsgBoxMsgDefault; // default message function
    pM->hdr.FreeObj = NULL; // free function

    // Set the color scheme to be used for MsgBox
    pM->hdr.pGolScheme = (GFX_GOL_OBJ_SCHEME *) pScheme;

    // Set the color scheme to be used for buttons
    pM->pButtonScheme = (GFX_GOL_OBJ_SCHEME *) pButtonScheme;

    pM->textMessageWidth = 0;
    pM->textMessageHeight = 0;
    if (pM->pTextMessage != NULL) {
        MsgBoxSetText(pM, pTextMessage);
    }

    #ifdef USE_ALPHABLEND_LITE
    pM->previousAlphaColor = BLACK;
    #endif

    #ifdef USE_ALPHABLEND                //Store the background image
    if (pM->hdr.pGolScheme->AlphaValue != 100) {
        CopyPageWindow(_GFXActivePage,
                _GFXBackgroundPage,
                pM->hdr.left, pM->hdr.top, pM->hdr.left, pM->hdr.top,
                pM->hdr.right - pM->hdr.left,
                pM->hdr.bottom - pM->hdr.top);
    }
    #endif

    GFX_GOL_ObjectAdd(GFX_INDEX_0, (GFX_GOL_OBJ_HEADER *) pM);

    if(pM->pTextCaption==NULL) { // No Caption        
        pM->CaptionHeight = 0;
        pM->pBitmapCaption=NULL; // No caption, no party 
    } else if (pM->pBitmapCaption != NULL) { // check if there is an icon  to be drawn
        pM->CaptionHeight = GFX_ImageHeightGet((void *) pM->pBitmapCaption) + 12;
    } else {
        pM->CaptionHeight = GFX_TextStringHeightGet(pM->hdr.pGolScheme->pFont)*1.5;
    }

    // Set Button's bitmaps, if passed
    pM->pBitmapReleasedKey = pBitmapReleasedKey;
    pM-> pBitmapPressedKey = pBitmapPressedKey;

    // ---------------------------------------------------------------------
    // Buttons "sub-Widgets" creation
    // ---------------------------------------------------------------------
    uint16_t intBtnWidth, intBtnTop, intBtnRight;
    uint16_t buttonState = GFX_GOL_BUTTON_DRAW_STATE;

    // Gap in pixels between buttons and between button and MsgBox edges
    #define BTNGAP 20
    // IDs for used buttons. These are internals so let's compute them starting from MsgBox ID
    #define ID_BTN_OK     ID*0x200  // Use a multiplier to keep computed ID unique
    #define ID_BTN_YES    ID_BTN_OK // Use same ID because OK and YES are never used toghether
    #define ID_BTN_NO     ID*0x200+1
    #define ID_BTN_CANCEL ID*0x200+2

    // Compute buttons size, based on font or bitmap size
    if (pBitmapReleasedKey == NULL) {
        pM->BtnHeight = GFX_TextStringHeightGet(pM->pButtonScheme->pFont) + BTNGAP; // Buttons font height + BTNGAP
    } else {
        pM->BtnHeight = GFX_ImageHeightGet((void *) pBitmapReleasedKey); // Buttons image height
        // Button's bitmaps are specified: switch buttons mode to NOPANEL, bitmap only+text
        buttonState |= GFX_GOL_BUTTON_NOPANEL_STATE;
    }
    intBtnWidth = (right - left) / 5; // Each button's width will be a fifth of parent MsgBox width
    intBtnRight = right - BTNGAP; // Buttons will be right aligned, from the right edge of MsgBox
    intBtnTop = bottom - BTNGAP - pM->BtnHeight; // and bottom aligned at BTNGAP pixels from MsgBox bottom edge

    pM->pButtons[0] = NULL;
    pM->pButtons[1] = NULL;
    pM->pButtons[2] = NULL;
    switch (buttonsToDisplay) {
        case BTN_OK:
            pM->pButtons[0] = GFX_GOL_ButtonCreate(GFX_INDEX_0, ID_BTN_OK, intBtnRight - intBtnWidth, intBtnTop, intBtnRight,
                    intBtnTop + pM->BtnHeight, radius, buttonState, pBitmapPressedKey, pBitmapReleasedKey, "OK", GFX_ALIGN_CENTER, pM->pButtonScheme);
            break;

        case BTN_YES_NO:
        case BTN_YES_NO_CANCEL:
            if (buttonsToDisplay == BTN_YES_NO_CANCEL) {
                // "Cancel" Button
                pM->pButtons[2] = GFX_GOL_ButtonCreate(GFX_INDEX_0, ID_BTN_CANCEL, intBtnRight - intBtnWidth, intBtnTop, intBtnRight,
                        intBtnTop + pM->BtnHeight, radius, buttonState, pBitmapPressedKey, pBitmapReleasedKey, "Cancel", GFX_ALIGN_CENTER, pM->pButtonScheme);
                intBtnRight -= (intBtnWidth + BTNGAP); // Compute next button right coordinate
            }

            // "No" Button
            pM->pButtons[1] = GFX_GOL_ButtonCreate(GFX_INDEX_0, ID_BTN_NO, intBtnRight - intBtnWidth, intBtnTop, intBtnRight,
                    intBtnTop + pM->BtnHeight, radius, buttonState, pBitmapPressedKey, pBitmapReleasedKey, "No", GFX_ALIGN_CENTER, pM->pButtonScheme);
            intBtnRight -= (intBtnWidth + BTNGAP); // Compute next button right coordinate

            // "Yes" Button
            pM->pButtons[0] = GFX_GOL_ButtonCreate(GFX_INDEX_0, ID_BTN_YES, intBtnRight - intBtnWidth, intBtnTop, intBtnRight,
                    intBtnTop + pM->BtnHeight, radius, buttonState, pBitmapPressedKey, pBitmapReleasedKey, "Yes", GFX_ALIGN_CENTER, pM->pButtonScheme);
            break;
    }

    return (pM);
}

/*********************************************************************
 * Function: MsgBoxSetText(MSGBOX *pM, GFX_XCHAR *pTextMessage)
 *
 *
 * Notes: Sets the text used in the MsgBox.
 *
 ********************************************************************/
void MsgBoxSetText(MSGBOX *pM, GFX_XCHAR *pTextMessage) {
    int width = 0, chCtr = 0, lineCtr = 1;
    GFX_XCHAR ch, *pParser;

    pM->pTextMessage = pTextMessage;

    // calculate width and height taking into account the multiple lines of text
    pParser = pM->pTextMessage;
    ch = *pTextMessage;

    // calculate the width (taken from the longest line)
    while (1) {
        if ((ch == 0x000A) || (ch == 0x0000)) {
            if (width < GFX_TextStringWidthGet(pParser, pM->hdr.pGolScheme->pFont)) {
                width = GFX_TextStringWidthGet(pParser, pM->hdr.pGolScheme->pFont);
            }

            if (ch == 0x000A) {
                pParser = pTextMessage + chCtr + 1;
                lineCtr++;
            } else {
                break;
            }
        }

        chCtr++;
        ch = *(pTextMessage + chCtr);
    }

    pM->textMessageWidth = width;
    pM->textMessageHeight = GFX_TextStringHeightGet(pM->hdr.pGolScheme->pFont) * lineCtr;
}

/*********************************************************************
 * Function: uint16_t MsgBoxTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 *
 * Notes: Evaluates the message if the object will be affected by the
 *        message or not.
 *
 ********************************************************************/
GFX_GOL_TRANSLATED_ACTION MsgBoxTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg) {
    MSGBOX *pM;
    uint8_t i;
    uint16_t TranslatedMsg;

    pM = (MSGBOX *) pObj;
    for (i = 0; i < 3; i++) {
        if (pM->pButtons[i] != NULL) {
            // If the button is active, call its BtnTranslateMsg to get the translated message, if any
            TranslatedMsg = pM->pButtons[i]->hdr.actionGet(pM->pButtons[i], pMsg);
            switch (TranslatedMsg) {
                case GFX_GOL_BUTTON_ACTION_RELEASED:
                    switch (i) {
                        case 0:
                            return MSGBOX_MSG_OK_YES;
                        case 1:
                            return MSGBOX_MSG_NO;
                        case 2:
                            return MSGBOX_MSG_CANCEL;
                    }
                    break;

            }
        }
    }
    return (GFX_GOL_OBJECT_ACTION_INVALID);
}

/*********************************************************************
 * MsgBox draw states
 ********************************************************************/
typedef enum {
    REMOVE,
    DRAW_PANELBORDER1,
    DRAW_PANELBORDER2,
    DRAW_CAPTIONPANEL,
    DRAW_PANELBOTTOM,
    DRAW_PANELCENTER,
    DRAW_CAPTIONTEXTANDICON,
    DRAW_BITMAP,
    CHECK_TEXT_DRAW,
    TEXT_DRAW_RUN,
} MSGBOX_DRAW_STATES;

/*********************************************************************
 * Function: inline void __attribute__((always_inline)) SetMsgBoxTextPosition(MSGBOX *MsgBox, GFX_XCHAR *pCurLine, uint16_t lineCtr)
 ********************************************************************/
inline void __attribute__((always_inline)) SetMsgBoxTextPosition(MSGBOX *MsgBox, GFX_XCHAR *pCurLine, uint16_t lineCtr) {
    uint16_t xText, yText;
    uint16_t textWidth;

    textWidth = GFX_TextStringWidthGet(pCurLine, MsgBox->hdr.pGolScheme->pFont);

    // check text alignment
    if (GFX_GOL_ObjectStateGet(MsgBox, MSGBOX_TEXTRIGHT)) {
        xText = MsgBox->hdr.right - (textWidth  + 2)- BTNGAP;
    } else if (GFX_GOL_ObjectStateGet(MsgBox, MSGBOX_TEXTLEFT)) {
        xText = MsgBox->hdr.left  + 2;
    } else {

        // centered	text in x direction
        xText = (MsgBox->hdr.left + MsgBox->hdr.right - textWidth) >> 1;
    }

    if (GFX_GOL_ObjectStateGet(MsgBox, MSGBOX_TEXTTOP)) {
        yText = MsgBox->hdr.top  + MsgBox->CaptionHeight +
                (lineCtr * GFX_TextStringHeightGet(MsgBox->hdr.pGolScheme->pFont));
    } else if (GFX_GOL_ObjectStateGet(MsgBox, MSGBOX_TEXTBOTTOM)) {
        yText = MsgBox->hdr.bottom - MsgBox->BtnHeight 
                - MsgBox->textMessageHeight
                + (lineCtr * GFX_TextStringHeightGet(MsgBox->hdr.pGolScheme->pFont));
    } else {

        // centered	text in y direction
        yText = ((MsgBox->hdr.bottom + MsgBox->hdr.top - MsgBox->BtnHeight - MsgBox->textMessageHeight)
                >> 1) + (lineCtr * GFX_TextStringHeightGet(MsgBox->hdr.pGolScheme->pFont));
    }

    GFX_TextCursorPositionSet(GFX_INDEX_0, xText, yText);

}

/*********************************************************************
 * Function: uint16_t MsgBoxDraw(void *pObj)
 *
 *
 * Notes: This is the state machine to draw the MsgBox.
 *
 ********************************************************************/
GFX_STATUS MsgBoxDraw(void *pObj) {

    static MSGBOX_DRAW_STATES state = REMOVE;
    static uint16_t width, height;

    static uint16_t charCtr = 0, lineCtr = 0;
    static GFX_XCHAR *pCurLine = NULL;
    GFX_XCHAR ch = 0;
    static GFX_COLOR embossLtClr, embossDkClr;
    static GFX_COLOR faceClr;
    MSGBOX *pM;
    static uint16_t intCaptionLeft;

    pM = (MSGBOX *) pObj;

    if (GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT)
        return (0);

    switch (state) {
        case REMOVE:
            if (GFX_RenderStatusGet(GFX_INDEX_0) == GFX_STATUS_BUSY_BIT)
                return (0);

    #ifdef USE_BISTABLE_DISPLAY_GOL_AUTO_REFRESH
            GFX_DRIVER_SetupDrawUpdate(pM->hdr.left, pM->hdr.top, pM->hdr.right, pM->hdr.bottom);
    #endif
            if (GFX_GOL_ObjectStateGet(pM, MSGBOX_HIDE)) { // Hide the MsgBox (remove from screen)
    #ifdef USE_ALPHABLEND
                if (pM->hdr.pGolScheme->AlphaValue != 100) {
                    CopyPageWindow(_GFXBackgroundPage, _GFXActivePage,
                            pM->hdr.left, pM->hdr.top, pM->hdr.left, pM->hdr.top,
                            pM->hdr.right - pM->hdr.left, pM->hdr.bottom - pM->hdr.top);
                } else
    #endif
                    GFX_ColorSet(GFX_INDEX_0, pM->hdr.pGolScheme->CommonBkColor);
                if (!GFX_BarDraw(GFX_INDEX_0, pM->hdr.left, pM->hdr.top, pM->hdr.right, pM->hdr.bottom))
                    return (0);
    #ifdef USE_BISTABLE_DISPLAY_GOL_AUTO_REFRESH
                GFX_DRIVER_CompleteDrawUpdate(GFX_INDEX_0, pM->hdr.left, pM->hdr.top, pM->hdr.right, pM->hdr.bottom);
    #endif
                return (1);
            } // End Hide

            width = (pM->hdr.right - pM->hdr.left);
            height = (pM->hdr.bottom - pM->hdr.top);

            embossLtClr = pM->hdr.pGolScheme->EmbossLtColor;
            embossDkClr = pM->hdr.pGolScheme->EmbossDkColor;
            faceClr = pM->hdr.pGolScheme->CommonBkColor;
    #ifdef USE_ALPHABLEND
            if (pM->hdr.pGolScheme->AlphaValue != 100)
                CopyPageWindow(GFX_INDEX_0, _GFXBackgroundPage, _GFXActivePage,
                    pM->hdr.left, pM->hdr.top, pM->hdr.left, pM->hdr.top, width, height);
    #endif
            GFX_LineStyleSet(GFX_INDEX_0,GFX_LINE_STYLE_THIN_SOLID);
            GFX_FontSet(GFX_INDEX_0, pM->hdr.pGolScheme->pFont);
    #ifdef USE_GRADIENT
            SetGOLPanelGradient(pM->hdr.pGolScheme);
    #endif
    #ifdef USE_ALPHABLEND_LITE
            SetPrevAlphaColor(pM->previousAlphaColor);
            SetGOLPanelAlpha(pM->hdr.pGolScheme->AlphaValue);
    #endif
            state = DRAW_PANELBORDER1;

        case DRAW_PANELBORDER1:
            // draw upper left portion of the embossed area
            GFX_ColorSet(GFX_INDEX_0, embossLtClr);
            if (!GFX_BevelDraw(GFX_INDEX_0, pM->hdr.left + pM->radius, pM->hdr.top + pM->radius,
                    pM->hdr.right - pM->radius, pM->hdr.bottom - pM->radius,
                    pM->radius )) //, pM->radius, 0xE1
                return (0);
            state = DRAW_PANELBORDER2;

        case DRAW_PANELBORDER2:
            // draw lower right portion of the embossed area
            GFX_ColorSet(GFX_INDEX_0, embossDkClr);
            if (!GFX_BevelDraw(GFX_INDEX_0, pM->hdr.left + pM->radius, pM->hdr.top + pM->radius,
                    pM->hdr.right - pM->radius, pM->hdr.bottom - pM->radius,
                    pM->radius )) //, pM->radius, 0x1E
                return (0);
            state = DRAW_CAPTIONPANEL;

        case DRAW_CAPTIONPANEL:
            if(pM->pTextCaption==NULL) {
                // No Caption
                GFX_ColorSet(GFX_INDEX_0, faceClr);
            } else {
                GFX_ColorSet(GFX_INDEX_0, pM->hdr.pGolScheme->Color1);
            }
    #ifdef USE_ALPHABLEND_LITE
            SetAlpha(_rpnlAlpha); // set alpha value
    #endif
            if (pM->radius) {
    #ifdef USE_GRADIENT
                if (_gradientScheme.gradientType != GRAD_NONE) {
                    BevelGradient(GFX_INDEX_0, pM->hdr.left + pM->radius, pM->hdr.top + pM->radius,
                            pM->hdr.right - pM->radius, pM->hdr.top + pM->CaptionHeight - pM->radius,
                            pM->radius ,
                            pM->hdr.pGolScheme->gradientScheme.gradientStartColor,
                            pM->hdr.pGolScheme->gradientScheme.gradientEndColor,
                            pM->hdr.pGolScheme->gradientScheme.gradientLength,
                            pM->hdr.pGolScheme->gradientScheme.gradientType);
                } else
    #endif
                    if (!GFX_BevelFillDraw(GFX_INDEX_0, pM->hdr.left + pM->radius, pM->hdr.top + pM->radius,
                        pM->hdr.right - pM->radius, pM->hdr.top + (pM->CaptionHeight == 0 ? pM->radius : pM->CaptionHeight),
                        pM->radius ))
                    return (0);
            } else {
                if (!GFX_BarDraw(GFX_INDEX_0, pM->hdr.left , pM->hdr.top ,
                        pM->hdr.right , pM->hdr.top  + (pM->CaptionHeight == 0 ? pM->radius : pM->CaptionHeight) - pM->radius))
                    return (0);
            }
            state = DRAW_PANELBOTTOM;

        case DRAW_PANELBOTTOM:
            GFX_ColorSet(GFX_INDEX_0, faceClr);
            if (pM->radius) {
    #ifdef USE_GRADIENT
                if (_gradientScheme.gradientType != GRAD_NONE) {
                    BevelGradient(GFX_INDEX_0, pM->hdr.left, pM->hdr.top, pM->hdr.right, pM->hdr.top + pM->CaptionHeight - pM->radius,
                            pM->radius ,
                            pM->hdr.pGolScheme->gradientScheme.gradientStartColor,
                            pM->hdr.pGolScheme->gradientScheme.gradientEndColor,
                            pM->hdr.pGolScheme->gradientScheme.gradientLength,
                            pM->hdr.pGolScheme->gradientScheme.gradientType);
                } else
    #endif
                    if (!GFX_BevelFillDraw(GFX_INDEX_0, pM->hdr.left + pM->radius, pM->hdr.bottom - (pM->radius << 1),
                        pM->hdr.right - pM->radius, pM->hdr.bottom - pM->radius,
                        pM->radius))
                    return (0);
            } else {
                if (!GFX_BarDraw(GFX_INDEX_0, pM->hdr.left , pM->hdr.bottom - pM->CaptionHeight,
                        pM->hdr.right , pM->hdr.bottom ))
                    return (0);
            }

            state = DRAW_PANELCENTER;

        case DRAW_PANELCENTER:
            if (!GFX_BarDraw(GFX_INDEX_0, pM->hdr.left , pM->hdr.top + (pM->CaptionHeight == 0 ? pM->radius : pM->CaptionHeight),
                    pM->hdr.right , pM->hdr.bottom - (pM->radius << 1) ))
                return (0);
            if(pM->pTextCaption==NULL) {
                // No Caption
                state = DRAW_BITMAP;
                goto draw_bitmap_here;
            } else {
                state = DRAW_CAPTIONTEXTANDICON;
            }

        case DRAW_CAPTIONTEXTANDICON:
            // Draw the caption icon
            if (pM->pBitmapCaption == NULL) {
                intCaptionLeft = pM->radius>>1;
            } else {
                intCaptionLeft = pM->radius+GFX_ImageWidthGet((void *) pM->pBitmapCaption) + 10;
                if (!GFX_ImageDraw(GFX_INDEX_0, pM->hdr.left  + 4+(pM->radius>>1)
                        , pM->hdr.top  + 4
                        , pM->pBitmapCaption)) {
                    return (0);
                }
            }

            // Set ClipRegion for caption
            GFX_TextAreaLeftSet(GFX_INDEX_0, pM->hdr.left  + intCaptionLeft);
            GFX_TextAreaTopSet(GFX_INDEX_0, pM->hdr.top);
            GFX_TextAreaRightSet(GFX_INDEX_0, pM->hdr.right);
            GFX_TextAreaBottomSet(GFX_INDEX_0, pM->hdr.top+GFX_TextStringHeightGet(pM->hdr.pGolScheme->pFont));
            GFX_ColorSet(GFX_INDEX_0, pM->hdr.pGolScheme->TextColor0);
            while(!GFX_TextStringDraw(GFX_INDEX_0, pM->hdr.left  + intCaptionLeft + 2
                    , pM->hdr.top  + ((pM->CaptionHeight - GFX_TextStringHeightGet(pM->hdr.pGolScheme->pFont)) >> 1)
                , pM->pTextCaption,0));

    #ifdef USE_ALPHABLEND_LITE
            pM->previousAlphaColor = faceClr;
    #endif
            state = DRAW_BITMAP;

        case DRAW_BITMAP:
            draw_bitmap_here:
            // check if there is a background image to be drawn
            if (pM->pBitmapMessage != NULL) {
                if (GFX_GOL_ObjectStateGet(pM, MSGBOX_TEXTRIGHT)) {
                    // If Text is right-aligned, put background picture to the left
                    if (!GFX_ImageDraw(GFX_INDEX_0,
                            pM->hdr.left + BTNGAP,
                            pM->hdr.top + pM->CaptionHeight + BTNGAP,
                            pM->pBitmapMessage
                            )) {
                        return (0);
                    }
                } else {
                    // Otherwise put background picture to the right
                    if (!GFX_ImageDraw(GFX_INDEX_0,
                            pM->hdr.right - GFX_ImageWidthGet((void *) pM->pBitmapMessage) - BTNGAP,
                            pM->hdr.top + pM->CaptionHeight + BTNGAP,
                            pM->pBitmapMessage
                            )) {
                        return (0);
                    }
                }
            }
            state = CHECK_TEXT_DRAW;

        case CHECK_TEXT_DRAW:
            if (pM->pTextMessage != NULL) {
                // Set ClipRegion for message
                GFX_TextAreaLeftSet(GFX_INDEX_0, pM->hdr.left);
                GFX_TextAreaTopSet(GFX_INDEX_0, pM->hdr.top);
                GFX_TextAreaRightSet(GFX_INDEX_0, pM->hdr.right);
                GFX_TextAreaBottomSet(GFX_INDEX_0, pM->hdr.bottom);
                GFX_ColorSet(GFX_INDEX_0, pM->hdr.pGolScheme->TextColor0);
                pCurLine = pM->pTextMessage;
                lineCtr = 0;
                charCtr = 0;
                SetMsgBoxTextPosition(pM, pCurLine, lineCtr);
                state = TEXT_DRAW_RUN;
            }

        case TEXT_DRAW_RUN:
            ch = *(pCurLine + charCtr);

            // output one character at time until a newline character or a NULL character is sampled
            while (0x0000 != ch) {
                if (!GFX_TextCharDraw(GFX_INDEX_0, ch))
                    return (0);
                // render the character
                charCtr++; // update to next character
                ch = *(pCurLine + charCtr);

                if (ch == 0x000A) { // new line character
                    pCurLine = pCurLine + charCtr + 1; // go to first char of next line
                    lineCtr++; // update line counter
                    charCtr = 0; // reset char counter
                    SetMsgBoxTextPosition(pM, pCurLine, lineCtr);
                    ch = *(pCurLine + charCtr);
                }
            }

    }

    #ifdef USE_BISTABLE_DISPLAY_GOL_AUTO_REFRESH
    GFX_DRIVER_CompleteDrawUpdate(pM->hdr.left,
            pM->hdr.top,
            pM->hdr.right,S
            pM->hdr.bottom);
    #endif
    state = REMOVE;
    return (1);
}

