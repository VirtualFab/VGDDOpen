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
// 2015/01/31   Harmony 1.02 version
// 2016/04/01   MHC version
// *****************************************************************************
#ifndef _MSGBOX_H
    #define _MSGBOX_H
#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>
#include "gfx/gfx.h"
#include "system_config.h"
#include "system_definitions.h"
#include "gfx/gfx_gol_button.h"

/*********************************************************************
* Object States Definition:
*********************************************************************/
    #define MSGBOX_TEXTRIGHT   0x0010  // Bit to indicate text is right aligned.
    #define MSGBOX_TEXTLEFT    0x0020  // Bit to indicate text is left aligned.
    #define MSGBOX_TEXTBOTTOM  0x0040  // Bit to indicate text is top aligned.
    #define MSGBOX_TEXTTOP     0x0080  // Bit to indicate text is bottom aligned.

// Note that if bits[7:4] are all zero text is centered.
    #define MSGBOX_DRAW        0x4000  // Bit to indicate MsgBox must be redrawn.
    #define MSGBOX_HIDE        0x8000  // Bit to indicate MsgBox must be removed from screen.
    #define MSGBOX_REMOVE      0x8000

/*********************************************************************
* Button(s) to display:
*********************************************************************/
typedef enum
{
    BTN_OK                         // Only OK button is displayed
   ,BTN_YES_NO                     // A YES/NO choice
   ,BTN_YES_NO_CANCEL              // Same as above but with Cancel button
} MSGBOX_BUTTONS;

/*********************************************************************
* Overview: Defines the parameters required for a MsgBox Object.
*           The following relationships of the parameters determines
*           the general shape of the MsgBox:
*            1. Width is determined by right - left.
*            2. Height is determined by top - bottom.
*            3. Radius - specifies if the MsgBox will have a rounded
*                        edge. If zero then the MsgBox will have
*                        sharp (cornered) edge.
*********************************************************************/
typedef struct
{
    GFX_GOL_OBJ_HEADER  hdr;                 // Generic header for all Objects (see OBJ_HEADER).
    uint16_t            radius;              // Radius for rounded MsgBoxes.
    MSGBOX_BUTTONS      buttons;             // Type of buttons to display (OK,Yes/No,Yes/No/Cancel) as defined by MSGBOX_BUTTONS
    uint16_t            textMessageWidth;    // Computed message text width, done at creation.
    uint16_t            textMessageHeight;   // Computed message text height, done at creation.
    uint16_t            CaptionHeight;       // Computed caption bar height, done at creation.
    uint16_t            BtnHeight;           // Computed height of buttons, done at creation
    GFX_XCHAR           *pTextMessage;       // Pointer to the text used for the main message.
    GFX_XCHAR           *pTextCaption;       // Pointer to the text used for the optional caption message.
    void                *pBitmapMessage;     // (optional) Pointer to bitmap to be put aside the main message.
    void                *pBitmapCaption;     // (optional) Pointer to bitmap used as optional caption icon.
    void                *pBitmapReleasedKey; // (optional) Bitmap to draw for the released key
    void                *pBitmapPressedKey;  // (optional) Bitmap to draw for the pressed key
    GFX_GOL_OBJ_SCHEME  *pButtonScheme;      // Pointer to the scheme used for buttons.
    GFX_GOL_BUTTON      *pButtons[3];        // Pointer array to BUTTON widgets
    #ifdef USE_ALPHABLEND_LITE
    GFX_COLOR    previousAlphaColor;
    #endif
} MSGBOX;


/*********************************************************************
* Macros:  MsgBoxSetBitmapMessage(pB, pBitmap)
*
* Overview: This macro sets the bitmap used in the object.
*
* PreCondition: none
*
* Input: pB - Pointer to the object.
*        pBitmap - Pointer to the bitmap to be used.
*
* Output: none
*
* Example:
*   <CODE>
*    extern BITMAP_FLASH myIcon;
*    MSGBOX *pMsgBox;
*
*    MsgBoxSetBitmapBackground(pMsgBox , &myIcon);
*   </CODE>
*
* Side Effects: none
*
********************************************************************/
    #define MsgBoxSetBitmapMessage(pB, pBtmap)    ((MSGBOX *)pB)->pBitmapMessage = pBtmap

/*********************************************************************
* Function: MsgBoxSetText(MsgBox *pB, GFX_XCHAR *pText)
*
* Overview: This function sets the string used for the object.
*
* PreCondition: none
*
* Input: pB - The pointer to the object whose text will be modified.
*        pText - Pointer to the text that will be used.
*
* Output: none
*
* Example:
*   <CODE>
*    GFX_XCHAR Label0[] = "ON";
*    GFX_XCHAR Label1[] = "OFF";
*    MsgBox MsgBox[2];
*
*        MsgBoxSetText(MsgBox[0], Label0);
*        MsgBoxSetText(MsgBox[1], Label1);
*    </CODE>
*
* Side Effects: none
*
********************************************************************/
void MsgBoxSetText(MSGBOX * pB, GFX_XCHAR * pText);

/*********************************************************************
* Function: MSGBOX *MsgBoxCreate(uint16_t ID, uint16_t left, uint16_t top, uint16_t right, uint16_t bottom,
*        uint16_t radius, MSGBOX_BUTTONS buttonsToDisplay, GFX_XCHAR *pTextMessage, GFX_XCHAR *pTextCaption,
*        uint16_t state, void *pBitmapMessage, void *pBitmapCaption, GFX_GOL_OBJ_SCHEME *pScheme,
*        GFX_GOL_OBJ_SCHEME *pButtonScheme, void *pBitmapReleasedKey, void *pBitmapPressedKey )
*
* Overview: This function creates a MsgBox object with the parameters given.
*           It automatically attaches the new object into a global linked list of
*           objects and returns the address of the object.
*
* PreCondition: none
*
* Input:    ID: ID for the Widget. Must be set to a unique ID value.
*           left,  top,  right,  bottom: Pixel coordinates where to draw the Widget
*           radius: radius in pixels for rounded MsgBoxes. 0 for squared
*           buttonsToDisplay: Type of buttons to display. Possible values:
*           	BTN_OK - Only the OK button is displayed
*           	BTN_YES_NO  - A YES/NO choice is displayed
*           	BTN_YES_NO_CANCEL – Same as above, but with Cancel button also.
*           pTextMessage: Pointer to the text used for the main message.
*           pTextCaption: (optional) Pointer to the text used for the optional caption message.
*           state: Initial state for the Widget. Possible values:
*           	MSGBOX_TEXTRIGHT 	Text will be right aligned.
*           	MSGBOX_TEXTLEFT 		Text will be left aligned.
*           	MSGBOX_TEXTBOTTOM  	Text will be top aligned.
*           	MSGBOX_TEXTTOP		Text will be bottom aligned.
*           If none of the above is specified, text is centered
*           	MSGBOX_DRAW	 	MsgBox must be redrawn.
*           	MSGBOX_HIDE		MsgBox must be removed from screen.
*           	MSGBOX_REMOVE		Same as above
*           pBitmapMessage: (optional) Pointer to bitmap to be put aside the main message
*           pBitmapCaption: (optional) Pointer to bitmap used as optional caption icon.
*           pScheme: Pointer to the scheme used for the MsgBox body.
*           pButtonScheme: (optional) Pointer to the scheme used for buttons.
*           pBitmapReleasedKey: (optional) Bitmap to draw for the released key
*           pBitmapPressedKey: (optional) Bitmap to draw for the pressed key
*
* Output: Returns the pointer to the object created.
*
* Side Effects: none
*
********************************************************************/
MSGBOX *MsgBoxCreate(
        uint16_t ID,
        uint16_t left,
        uint16_t top,
        uint16_t right,
        uint16_t bottom,
        uint16_t radius,
        MSGBOX_BUTTONS buttonsToDisplay,
        GFX_XCHAR *pTextMessage,
        GFX_XCHAR *pTextCaption,
        uint16_t state,
        void *pBitmapMessage,
        void *pBitmapCaption,
        GFX_GOL_OBJ_SCHEME *pScheme,
        GFX_GOL_OBJ_SCHEME *pButtonScheme,
        void *pBitmapReleasedKey,
        void *pBitmapPressedKey
        );

GFX_GOL_TRANSLATED_ACTION MsgBoxTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg);

void    MsgBoxMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg);

GFX_STATUS MsgBoxDraw(void *pObj);

/*********************************************************************
* Overview: This structure defines the list of translated messages for
*           MsgBox Objects.
*
*********************************************************************/
typedef enum {
    MSGBOX_MSG_INVALID      = 0,    // Invalid message response.
    MSGBOX_MSG_OK_YES,              // OK/YES Button pressed action ID.
    MSGBOX_MSG_NO,                  // NO Button pressed action ID.
    MSGBOX_MSG_CANCEL               // CANCEL Button pressed action ID.
} TRANS_MSG_MSGBOX;
#endif // _MsgBox_H
