// *****************************************************************************
//  Module for Microchip Graphics Library
//  GOL Layer
//  Static Text - MLA version
// *****************************************************************************
// FileName:        StaticText.c
// Dependencies:    None
// Processor:       PIC24F, PIC24H, dsPIC, PIC32
// Compiler:        MPLAB C30 V3.00, MPLAB C32
// Linker:          MPLAB LINK30, MPLAB LINK32
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
// Copyright ? 2008 Microchip Technology Inc.  All rights reserved.
// Microchip licenses to you the right to use, modify, copy and distribute
// Software only when embedded on a Microchip microcontroller or digital
// signal controller, which is integrated into your product or third party
// product (pursuant to the sublicense terms in the accompanying license
// agreement).
// *
// You should refer to the license agreement accompanying this Software
// for additional information regarding your rights and obligations.
// *
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
// *
// Date         Comment
// *****************************************************************************
//  2013/11/19	Initial release
//  2014/09/07  MLA version
// *****************************************************************************/

#include "statictext_ex.h"

/*********************************************************************
 * Function: STATICTEXTEX  *StxCreate(uint16_t ID, int16_t left, int16_t top, int16_t right, int16_t bottom,
 *		uint16_t state , GFX_XCHAR *pText, GFX_GOL_OBJ_SCHEME *pScheme)
 *
 * Notes: Creates a STATICTEXTEX object and adds it to the current active list.
 *        If the creation is successful, the pointer to the created Object
 *        is returned. If not successful, NULL is returned.
 *
 ********************************************************************/
STATICTEXTEX *StExCreate
(
        uint16_t ID,
        int16_t left,
        int16_t top,
        int16_t right,
        int16_t bottom,
        uint16_t state,
        GFX_XCHAR *pText,
        GFX_GOL_OBJ_SCHEME *pScheme
        ) {
    STATICTEXTEX *pSt = NULL;

    pSt = (STATICTEXTEX *) GFX_malloc(sizeof (STATICTEXTEX));
    if (pSt == NULL)
        return (pSt);

    pSt->hdr.ID = ID; // unique id assigned for referencing
    pSt->hdr.pNxtObj = NULL; // initialize pointer to NULL
    pSt->hdr.type = OBJ_STATICTEXTEX; // set object type
    pSt->hdr.left = left; // left,top corner
    pSt->hdr.top = top;
    pSt->hdr.right = right; // right buttom corner
    pSt->hdr.bottom = bottom;
    pSt->pText = pText; // location of the text
    pSt->hdr.state = state;
    pSt->hdr.DrawObj = StExDraw; // draw function
    pSt->hdr.actionGet = StExTranslateMsg; // message function
    pSt->hdr.actionSet = NULL; // default message function
    pSt->hdr.FreeObj = NULL; // free function

    // Set the color scheme to be used
    pSt->hdr.pGolScheme = (GFX_GOL_OBJ_SCHEME *) pScheme;

    pSt->textHeight = 0;
    if (pSt->pText != NULL) {

        // Set the text height
        pSt->textHeight = GFX_TextStringHeightGet(pSt->hdr.pGolScheme->pFont);
    }

    GFX_GOL_ObjectAdd((GFX_GOL_OBJ_HEADER *) pSt);
    return (pSt);
}

/*********************************************************************
 * Function: StExSetText(STATICTEXTEX *pSt, GFX_XCHAR *pText)
 *
 * Notes: Sets the string that will be used.
 *
 ********************************************************************/
void StExSetText(STATICTEXTEX *pSt, GFX_XCHAR *pText) {
    pSt->pText = pText;
    pSt->textHeight = GFX_TextStringHeightGet(pSt->hdr.pGolScheme->pFont);
}

/*********************************************************************
 * Function: uint16_t StExTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
 *
 * Notes: Evaluates the message if the object will be affected by the
 *		 message or not.
 *
 ********************************************************************/
uint16_t StExTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg) {

    STATICTEXTEX *pSt;

    pSt = (STATICTEXTEX *) pObj;


    // Evaluate if the message is for the static text
    // Check if disabled first
    if (GFX_GOL_ObjectStateGet(pSt, STEX_DISABLED))
        return (GFX_GOL_OBJECT_ACTION_INVALID);

#ifdef USE_TOUCHSCREEN
    if (pMsg->type == TYPE_TOUCHSCREEN) {

        // Check if it falls in static text control borders
        if
            (
                (pSt->hdr.left < pMsg->param1) &&
                (pSt->hdr.right > pMsg->param1) &&
                (pSt->hdr.top < pMsg->param2) &&
                (pSt->hdr.bottom > pMsg->param2)
                ) {
            return (STEX_MSG_SELECTED);
        }
    }

#endif
    return (GFX_GOL_OBJECT_ACTION_INVALID);
}

/*********************************************************************
 * Function: uint16_t StDraw(void *pObj)
 *
 * Notes: This is the state machine to draw the static text.
 *
 ********************************************************************/
uint16_t StExDraw(void *pObj) {

    typedef enum {
        STEX_STATE_IDLE,
        STEX_STATE_CLEANAREA,
        STEX_STATE_INIT,
        STEX_STATE_SETALIGN,
        STEX_STATE_DRAWTEXT
    } STEX_DRAW_STATES;

    static STEX_DRAW_STATES state = STEX_STATE_IDLE;
    static int16_t charCtr = 0, lineCtr = 0;
    static GFX_XCHAR *pCurLine = NULL;
    int16_t textWidth;
    GFX_XCHAR ch = 0;
    STATICTEXTEX *pSt;

    pSt = (STATICTEXTEX *) pObj;

    while (1) {
        if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
            return (0);

        switch (state) {
            case STEX_STATE_IDLE:
#ifdef USE_BISTABLE_DISPLAY_GOL_AUTO_REFRESH
                GFX_DRIVER_SetupDrawUpdate(pSt->hdr.left,
                        pSt->hdr.top,
                        pSt->hdr.right,
                        pSt->hdr.bottom);
#endif

                if (GFX_GOL_ObjectStateGet(pSt, STEX_HIDE)) {
                    GFX_ColorSet(pSt->hdr.pGolScheme->CommonBkColor);
                    if (!GFX_BarDraw(pSt->hdr.left, pSt->hdr.top, pSt->hdr.right, pSt->hdr.bottom))
                        return (0);

                    // State is still IDLE STATE so no need to set state
#ifdef USE_BISTABLE_DISPLAY_GOL_AUTO_REFRESH
                    GFX_DRIVER_CompleteDrawUpdate(pSt->hdr.left,
                            pSt->hdr.top,
                            pSt->hdr.right,
                            pSt->hdr.bottom);
#endif
                    return (1);
                }

                if (GFX_GOL_ObjectStateGet(pSt, STEX_DRAW)) {
                    // show frame if specified to be shown
                    GFX_LineStyleSet(GFX_LINE_STYLE_THIN_SOLID);
                    GFX_LineStyleSet(GFX_LINE_STYLE_THIN_SOLID);

                    if (GFX_GOL_ObjectStateGet(pSt, STEX_FRAME)) {

                        if (!GFX_GOL_ObjectStateGet(pSt, STEX_DISABLED)) {

                            // show enabled color
                            GFX_ColorSet(pSt->hdr.pGolScheme->Color1);
                            if (!GFX_RectangleDraw(pSt->hdr.left, pSt->hdr.top, pSt->hdr.right, pSt->hdr.bottom))
                                return (0);
                        } else {

                            // show disabled color
                            GFX_ColorSet(pSt->hdr.pGolScheme->ColorDisabled);
                            if (!GFX_RectangleDraw(pSt->hdr.left, pSt->hdr.top, pSt->hdr.right, pSt->hdr.bottom))
                                return (0);
                        }
                    } else if (GFX_GOL_ObjectStateGet(pSt, STEX_NOPANEL) == 0) {
                        // show enabled color
                        GFX_ColorSet(pSt->hdr.pGolScheme->CommonBkColor);
                        if (!GFX_RectangleDraw(pSt->hdr.left, pSt->hdr.top, pSt->hdr.right, pSt->hdr.bottom))
                            return (0);

                    }
                }

                state = STEX_STATE_CLEANAREA;

            case STEX_STATE_CLEANAREA:

                if (GFX_GOL_ObjectStateGet(pSt, STEX_NOPANEL) == 0) {
                    // clean area where text will be placed.
                    GFX_ColorSet(pSt->hdr.pGolScheme->CommonBkColor);
                    if (!GFX_BarDraw(pSt->hdr.left + 1, pSt->hdr.top + 1, pSt->hdr.right - 1, pSt->hdr.bottom - 1))
                        return (0);
                }
                // set clipping area, text will only appear inside the static text area.
                GFX_TextAreaLeftSet(pSt->hdr.left + STEX_INDENT);
                GFX_TextAreaTopSet(pSt->hdr.top);
                GFX_TextAreaRightSet(pSt->hdr.right - STEX_INDENT);
                GFX_TextAreaBottomSet(pSt->hdr.bottom);
                state = STEX_STATE_INIT;

            case STEX_STATE_INIT:
                if (GFX_RenderStatusGet() == GFX_STATUS_BUSY_BIT)
                    return (0);

                // set the text color
                if (!GFX_GOL_ObjectStateGet(pSt, STEX_DISABLED)) {
                    GFX_ColorSet(pSt->hdr.pGolScheme->TextColor0);
                } else {
                    GFX_ColorSet(pSt->hdr.pGolScheme->TextColorDisabled);
                }

                // use the font specified in the object
                GFX_FontSet(pSt->hdr.pGolScheme->pFont);
                pCurLine = pSt->pText; // get first line of text
                state = STEX_STATE_SETALIGN; // go to drawing of text

            case STEX_STATE_SETALIGN:
                if (charCtr == 0) {

                    // set position of the next character (based on alignment and next character)
                    textWidth = GFX_TextStringWidthGet(pCurLine, pSt->hdr.pGolScheme->pFont);

                    // Display text with center alignment
                    if (GFX_GOL_ObjectStateGet(pSt, (STEX_CENTER_ALIGN))) {
                        GFX_TextCursorPositionSet((pSt->hdr.left + pSt->hdr.right - textWidth) >> 1, pSt->hdr.top + (lineCtr * pSt->textHeight));
                    }
                        // Display text with right alignment
                    else if (GFX_GOL_ObjectStateGet(pSt, (STEX_RIGHT_ALIGN))) {
                        GFX_TextCursorPositionSet((pSt->hdr.right - textWidth - STEX_INDENT), pSt->hdr.top + (lineCtr * pSt->textHeight));
                    }
                        // Display text with left alignment
                    else {
                        GFX_TextCursorPositionSet(pSt->hdr.left + STEX_INDENT, pSt->hdr.top + (lineCtr * pSt->textHeight));
                    }
                }

                state = STEX_STATE_DRAWTEXT;

            case STEX_STATE_DRAWTEXT:
                ch = *(pCurLine + charCtr);

                // output one character at time until a newline character or a NULL character is sampled
                while ((0x0000 != ch) && (0x000A != ch)) {
                    if (!GFX_TextCharDraw(ch))
                        return (0); // render the character
                    charCtr++; // update to next character
                    ch = *(pCurLine + charCtr);
                }

                // pCurText is updated for the next line
                if (ch == 0x000A) { // new line character
                    pCurLine = pCurLine + charCtr + 1; // go to first char of next line
                    lineCtr++; // update line counter
                    charCtr = 0; // reset char counter
                    state = STEX_STATE_SETALIGN; // continue to next line
                    break;
                }
                    // end of text string is reached no more lines to display
                else {
                    pCurLine = NULL; // reset static variables
                    lineCtr = 0;
                    charCtr = 0;
                    state = STEX_STATE_IDLE; // go back to IDLE state
                    // Reset clipping
                    GFX_TextAreaLeftSet(0);
                    GFX_TextAreaTopSet(0);
                    GFX_TextAreaRightSet(GFX_MaxXGet());
                    GFX_TextAreaBottomSet(GFX_MaxYGet());

#ifdef USE_BISTABLE_DISPLAY_GOL_AUTO_REFRESH
                    GFX_DRIVER_CompleteDrawUpdate(pSt->hdr.left,
                            pSt->hdr.top,
                            pSt->hdr.right,
                            pSt->hdr.bottom);
#endif
                    return (1);
                }
        } // end of switch()
    } // end of while(1)    
}

