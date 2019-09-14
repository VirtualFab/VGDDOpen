// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// TextEntryEx - Harmony version
// *****************************************************************************
// FileName:        textentryex.h
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
//  2014/08/31  Harmony Version
//  2016/04/01  MHC version
// *****************************************************************************

#ifndef _TEXTENTRYEX_H
    #define _TEXTENTRYEX_H

#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>
#include "gfx/gfx.h"
#include "system_config.h"
#include "system_definitions.h"

extern volatile uint32_t tick;

/*********************************************************************
* Object States Definition:
*********************************************************************/
    #define TEEX_KEY_PRESSED  0x0004  // Bit for press state of one of the keys.
    #define TEEX_DISABLED     0x0002  // Bit for disabled state.
    #define TEEX_ECHO_HIDE    0x0008  // Bit to hide the entered characters and instead echo "*" characters to the display.
    #define TEEX_LIKEKEYBOARD 0x0010  // Bit for keyboard-like layout. Default: grid layout
    #define TEEX_SHIFT_ACTIVE 0x0020  // Bit for Shift active
    #define TEEX_LOCK_ACTIVE  0x0040  // Bit for Caps Lock active
    #define TEEX_ALT_ACTIVE   0x0080  // Bit for Alternate active
    #define TEEX_DRAW_UPDATE  0x0100  // Bit to indicate that only keys must be redrawn.
    #define TEEX_LOCK_TRANS   0x0200  // Bit for Caps Lock transition handling
    #define TEEX_DRAW         0x4000  // Bit to indicate object must be redrawn.
    #define TEEX_HIDE         0x8000  // Bit to indicate object must be removed from screen.
    #define TEEX_UPDATE_KEY   0x2000  // Bit to indicate redraw of a key is needed.
    #define TEEX_UPDATE_TEXT  0x1000  // Bit to indicate redraw of the text displayed is needed.

/*********************************************************************
* Optional COMMANDS assigned to keys
*********************************************************************/
    #define TEEX_BKSP_COM     0x01    // This macro is used to assign a "backspace" command on a key.
    #define TEEX_SPACE_COM    0x02    // This macro is used to assign an insert "space" command on a key.
    #define TEEX_ENTER_COM    0x03    // This macro is used to assign an "enter" (carriage return) command on a key.
    #define TEEX_SHIFT_COM    0x04    // This macro is used to assign a "shift" command on a key.
    #define TEEX_ALT_COM      0x05    // This macro is used to assign an "alternate" command on a key.

// User can use this command to customize application code in the message
// callback function. Use the returned translated TEEX_MSG_ENTER to detect the key
// pressed was assigned the enter command. Refer to TeTranslateMsg() for details.

//#if defined(USE_MULTIBYTECHAR)
#define NULLSTRING (GFX_XCHAR []){0,0} // ""
#define SPACESTRING (GFX_XCHAR []){' ',0} // " "
#define OKSTRING (GFX_XCHAR []){'O','K',0} // "OK"
#define BKSTRING (GFX_XCHAR []){'B','K',0} // "BK"
#define SHSTRING (GFX_XCHAR []){'S','H',0} // "SH"
#define ALSTRING (GFX_XCHAR []){'A','L',0} // "AL"
#define SPSTRING (GFX_XCHAR []){'S','P',0} // "SP"
#define ALTERNATESTRING (GFX_XCHAR []){'?','1','2','3',0} // "?123"
#define NORMALSTRING (GFX_XCHAR []){' ','a','b','c',0} // " abc"
//#else
//#define NULLSTRING ""
//#define SPACESTRING " "
//#define OKSTRING "OK"
//#define BKSTRING "BK"
//#define SHSTRING "SH"
//#define ALSTRING "AL"
//#define SPSTRING "SP"
//#define ALTERNATESTRING "?123"
//#define NORMALSTRING "abc"
//#endif

/*********************************************************************
* Overview: Defines the parameters and the strings assigned for each key.
*********************************************************************/
typedef struct
{
    int16_t   left;                    // Left position of the key
    int16_t   top;                     // Top position of the key
    int16_t   right;                   // Right position of the key
    int16_t   bottom;                  // Bottom position of the key
    int16_t   index;                   // Index of the key in the list
    uint16_t  state;                   // State of the key. Either Pressed (TEEX_KEY_PRESSED) or Released (0)
    bool      update;                  // flag to indicate key is to be redrawn with the current state
    uint16_t  command;                 // Command of the key. Either TEEX_DELETE_COM, TEEX_SPACE_COM, TE_ENTER_COM, TEEX_BKSP_COM, TEEX_SHIFT_COM, TEEX_ALT_COM
    GFX_XCHAR *pKeyName;               // Pointer to the custom text assigned to the key. This is displayed over the face of the key.
    GFX_XCHAR *pKeyNameAlternate;      // Pointer to the custom alternate text assigned to the key. This is displayed over the face of the key when ALT mode is active.
    GFX_XCHAR *pKeyNameShift;          // Pointer to the custom shift text assigned to the key. This is displayed over the face of the key when ALT mode is active.
    GFX_XCHAR *pKeyNameShiftAlternate; // Pointer to the custom shift alternate text assigned to the key. This is displayed over the face of the key when ALT mode is active.
    int16_t   textWidth;               // Computed text width, done at creation. Used to predict size and position of text on the key face.
    int16_t   textWidthAlternate;      // Computed alternate text width, done at creation. Used to predict size and position of text on the key face.
    int16_t   textWidthShift;          // Computed shift text width, done at creation. Used to predict size and position of text on the key face.
    int16_t   textWidthShiftAlternate; // Computed shift text width, done at creation. Used to predict size and position of text on the key face.
    int16_t   textHeight;              // Computed text height, done at creation. Used to predict size and position of text on the key face.
    void      *pNextKey;               // Pointer to the next key parameters.
} TEEX_KEYMEMBER;

/*********************************************************************
* Overview: Defines the parameters required for a TextEntry Object.
*********************************************************************/
typedef struct
{
    GFX_GOL_OBJ_HEADER  hdr;                     // Generic header for all objects (see GFX_GOL_OBJ_HEADER).
    int16_t       radius;                  // Radius for the keys buttons
    int16_t       totalKeys;               // Total number of keys
    int16_t       totalKeysAlternate;      // Total number of alternate keys
    int16_t       totalKeysShift;          // Total number of Shift keys
    int16_t       totalKeysShiftAlternate; // Total number of Shift Alternate keys
    int16_t       horizontalKeys;          // Number of horizontal keys
    int16_t       verticalKeys;            // Number of vertical keys
    GFX_XCHAR       *pTeOutput;              // Pointer to the buffer assigned by the user which holds the text shown in the editbox.

    // User creates and manages the buffer. Buffer can also be managed using the APIs provided
    // to add a character, delete the last character or clear the buffer.
    uint16_t        CurrentLength;  // Current length of the string in the buffer. The maximum value of this is equal to outputLenMax.

    // TextEntry object will update this parameter when adding, removing characters or clearing the buffer
    // and switching buffers.
    uint16_t        outputLenMax;   // Maximum expected length of output buffer pTeOutput
    TEEX_KEYMEMBER   *pActiveKey;    // Pointer to the active key KEYMEMBER. This is only used by the Widget. User must not change

    // the value of this parameter directly.
    TEEX_KEYMEMBER   *pHeadOfList;          // Pointer to head of the list
    TEEX_KEYMEMBER   *pHeadOfAlternateList; // Pointer to head of the alternate list
    void        *pDisplayFont;  // Pointer to the font used in displaying the text.
    void        *pBitmapReleasedKey;  // (optional) Pointer to the bitmap to use when the key is released/first drawn
    void        *pBitmapPressedKey;   // (optional) Pointer to the bitmap to use when the key is pressed
    int16_t       bitmapWidth;          // Width of pBitmapReleasedKey, computed on TeExCreate. pBitmapPressedKey width is assumed to be the same
    int16_t       bitmapHeight;         // Height of pBitmapReleasedKey, computed on TeExCreate. pBitmapPressedKey height is assumed to be the same
    int16_t       VerticalKeySpacing;   // Vertical spacing (in pixels) between keys and from widget's edges
    int16_t       HorizontalKeySpacing; // Horizontal spacing (in pixels) between keys and from widget's edges
} TEXTENTRYEX;

/*********************************************************************
* Function: TEXTENTRY *TeCreate(uint16_t ID, int16_t left, int16_t top,
*                   int16_t right, int16_t bottom, uint16_t state,
*					int16_t horizontalKeys, int16_t verticalKeys, GFX_XCHAR *pText[],
*					void *pBuffer, uint16_t bufferLength,void *pDisplayFont,
*					GFX_GOL_OBJ_SCHEME *pScheme)
*
* Overview: This function creates a TEXTENTRY object with the parameters given.
*	       It automatically attaches the new object into a global linked list of
*	       objects and returns the address of the object.
*
*
* PreCondition: If the object will use customized keys, the structure CUSTOMEKEYS must be
*	            populated before calling this function.
*
* Input:  ID -		Unique user defined ID for the object instance
*		  left-     Left most position of the object.
*		  top -		Top most position of the object.
*		  right -	Right most position of the object.
*		  bottom - 	Bottom most position of the object.
*		  state			 - state of the widget.
*		  horizontalKeys - Number of horizontal keys
*		  verticalKeys	 - Number of vertical keys
*		  pText			 - array of pointer to the custom "text" assigned by the user.
*         pBuffer        - pointer to the buffer that holds the text to be displayed.
*		  bufferLength	 - length of the buffer assigned by the user.
*	      pDisplayFont   - pointer to the font image to be used on the editbox
*	      pScheme- Pointer to the style scheme used.
*
* Output Returns the pointer to the object created.
*
* Side Effects: none.
*
********************************************************************/
TEXTENTRYEX   *TeExCreate
    (
        uint16_t        ID,                      // Unique ID for the Widget
        int16_t       left,                    // Left
        int16_t       top,                     // Top
        int16_t       right,                   // Right
        int16_t       bottom,                  // Bottom
        uint16_t        state,                   // Initial state for the Widget - TEEX_DRAW to simply draw it
        GFX_XCHAR       *pText[],                // Array for keys texts
        GFX_XCHAR       *pTextAlternate[],       // Array for alternate keys texts
        GFX_XCHAR       *pTextShift[],           // Array for shift keys texts
        GFX_XCHAR       *pTextShiftAlternate[],  // Array for shift keys texts
        int16_t       aCommandKeys[],          // Array of command key indexes
        void        *pBuffer,                // Buffer where to store typed text - output of the Widget
        void        *pBitmapReleasedKey,     // Bitmap to draw for the released key
        void        *pBitmapPressedKey,      // Bitmap to draw for the pressed key
        void        *pDisplayFont,           // Font for displaying typed text
        void        *Params,                 // Rest of the widget's parameters
        GFX_GOL_OBJ_SCHEME  *pScheme                 // GOL scheme for the rest of the Widget
    );

void TeExDrawCapsLock(TEXTENTRYEX *pTeEx);

/*********************************************************************
* Function: uint16_t TeDraw(void *pObj)
*
* Overview: This function renders the object on the screen using
* 	        the current parameter settings. Location of the object is
*	        determined by the left, top, right and bottom parameters.
*	        The colors used are dependent on the state of the object.
*
*		    This widget will draw the keys using the function
*			GOLPanelDraw(). The number of keys will depend on the horizontal
*			and vertical parameters given (horizontalKeys*verticakKeys).
*
* PreCondition: Object must be created before this function is called.
*
* Input: pTe- Pointer to the object to be rendered.
*
* Output: Returns the status of the drawing
*		  - 1 - If the rendering was completed and
*		  - 0 - If the rendering is not yet finished.
*		  Next call to the function will resume the
*		  rendering on the pending drawing state.
*
* Side Effects: none.
*
********************************************************************/
GFX_STATUS TeExDraw(void *pObj);

/*********************************************************************
* Function:  uint16_t TeTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
*
* Overview: This function evaluates the message from a user if the
*			message will affect the object or not. If the message
*			is valid, the keys in the Text Entry object will be
*			scanned to detect which key was pressed. If True, the
*			corresponding text will be displayed, the (c)text(c) will
*			also be stored in the TeOutput parameter of the object.
*
*	<TABLE>
*    	Translated Message   Input Source  Events         				Description
*     	##################   ############  ######         				###########
*		TEEX_MSG_PRESS	 	 Touch Screen  EVENT_PRESS, EVENT_MOVE   	If the event occurs and the x,y position falls in the face of one of the keys of the object while the key is unpressed.
*		TEEX_MSG_RELEASED	 	 Touch Screen  EVENT_MOVE  	                If the event occurs and the x,y position falls outside the face of one of the keys of the object while the key is pressed.
*		TEEX_MSG_RELEASED	 	 Touch Screen  EVENT_RELEASE                If the event occurs and the x,y position falls does not falls inside any of the faces of the keys of the object.
*		TEEX_MSG_ADD_CHAR	 	 Touch Screen  EVENT_RELEASE, EVENT_MOVE  	If the event occurs and the x,y position falls in the face of one of the keys of the object while the key is unpressed and the key is associated with no commands.
*		TEEX_MSG_DELETE	 	 Touch Screen  EVENT_RELEASE, EVENT_MOVE  	If the event occurs and the x,y position falls in the face of one of the keys of the object while the key is unpressed and the key is associated with delete command.
*		TEEX_MSG_SPACE	 	 Touch Screen  EVENT_RELEASE, EVENT_MOVE  	If the event occurs and the x,y position falls in the face of one of the keys of the object while the key is unpressed and the key is associated with space command.
*		TEEX_MSG_ENTER	 	 Touch Screen  EVENT_RELEASE, EVENT_MOVE  	If the event occurs and the x,y position falls in the face of one of the keys of the object while the key is unpressed and the key is associated with enter command.
*		OBJ_MSG_INVALID		 Any		   Any			  				If the message did not affect the object.
*	</TABLE>
*
* PreCondition: none
*
* Input: 	pTe-	The pointer to the object where the message will be
*					evaluated to check if the message will affect the object.
*        	pMsg-   Pointer to the message struct containing the message from
*        			the user interface.
*
* Output: Returns the translated message depending on the received GOL message:
*		  - TEEX_MSG_PRESS (c) A key is pressed
*         - TEEX_MSG_RELEASED - A key was released (generic for keys with no commands or characters assigned)
*         - TEEX_MSG_ADD_CHAR (c) A key was released with character assigned
*         - TEEX_MSG_DELETE (c) A key was released with delete command assigned
*         - TEEX_MSG_SPACE - A key was released with space command assigned
*         - TEEX_MSG_ENTER - A key was released with enter command assigned
*         - OBJ_MSG_INVALID (c) Text Entry is not affected
*
* Side Effects: none.
*
********************************************************************/
GFX_GOL_TRANSLATED_ACTION        GFX_TeExActionGet(void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
* Function: TeMsgDefault(uint16_t translatedMsg, void *pObj, GFX_GOL_MESSAGE* pMsg)
*
* Overview: This function performs the actual state change
*			based on the translated message given. The following state changes
*			are supported:
*	<TABLE>
*    	Translated Message   Input Source	Set/Clear State Bit		Description
*     	##################   ############	######     				###########
*     	TEEX_MSG_ADD_CHAR      Touch Screen,	Set TEEX_UPDATE_TEXT,	    Add a character in the buffer and update the text displayed.
*												TEEX_UPDATE_KEY,
*                                           Clear TEEX_KEY_PRESSED
*     	TEEX_MSG_SPACE      	 Touch Screen,	Set TEEX_UPDATE_TEXT, 	Insert a space character in the buffer and update the text displayed.
*												TEEX_UPDATE_KEY,
*                                           Clear TEEX_KEY_PRESSED
*     	TEEX_MSG_DELETE      	 Touch Screen,	Set TEEX_UPDATE_TEXT, 	Delete the most recent character in the buffer and update the text displayed.
*												TEEX_UPDATE_KEY,
*                                           Clear TEEX_KEY_PRESSED
*     	TEEX_MSG_ENTER      	 Touch Screen,	Set TEEX_UPDATE_TEXT, 	User can define the use of this event in the message callback. Object will just update the key.
*												TEEX_UPDATE_KEY,
*                                           Clear TEEX_KEY_PRESSED
*		TEEX_MSG_RELEASED	 	 Touch Screen,	Clear TEEX_KEY_PRESSED	A Key in the object will be redrawn in the unpressed state.
*                                           Set Te_UPDATE_KEY
*		TEEX_MSG_PRESSED	 	 Touch Screen,	Set TEEX_KEY_PRESSED		A Key in the object will be redrawn in the pressed state.
*                                               TEEX_UPDATE_KEY
*
*	</TABLE>
*
* PreCondition: none
*
* Input: translatedMsg - The translated message.
*        pTe           - The pointer to the object whose state will be modified.
*        pMsg          - The pointer to the GOL message.
*
* Output: none
*
* Example:
*	See BtnTranslateMsg() example.
*
* Side Effects: none
*
********************************************************************/
void        GFX_TeExActionSet(GFX_GOL_TRANSLATED_ACTION translatedMsg, void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
* Function: void TeSetBuffer(TEXTENTRY *pTe, GFX_XCHAR *pText, uint16_t MaxSize)
*
* Overview: This function sets the buffer used to display text. If the
*			buffer is initialized with a string, the string must be
*			a null terminated string. If the string length is greater
*           than MaxSize, string will be truncated to MaxSize.
*           pText must point to a valid memory location with size equal
*           to MaxSize+1. The +1 is used for the string terminator.
*
*
* PreCondition: none
*
* Input: 	pTe- pointer to the object
*			pText- pointer to the new text buffer to be displayed
*			maxSize - maximu        int16_t       totalKeysShift,      // Total number of shift keys
m length of the new buffer to be used.
* Output:  none.
*
* Side Effects: none.
*
********************************************************************/
void        TeExSetBuffer(TEXTENTRYEX *pTe, GFX_XCHAR *pText, uint16_t MaxSize);

/*********************************************************************
* Macro: TeGetBuffer(pTe)
*
* Overview: This macro will return the currently used buffer in the
*			TextEntry object.
*
* PreCondition: none
*
* Input: 	pTe- pointer to the object
*
* Output:  It will return a pointer to the buffer used.
*
* Side Effects: none.
*
********************************************************************/
    #define TeExGetBuffer(pTe)    (((TEXTENTRYEX *)pTe)->pTeOutput)

/*********************************************************************
* Function: void TeClearBuffer (TEXTENTRY *pTe)
*
* Overview: This function will clear the data in the display. You must
*			set the drawing state bit TEEX_UPDATE_TEXT
*		 	to update the TEXTENTRY on the screen.
*
* PreCondition: none
*
* Input: 	pTe- pointer to the object
*
* Output:  none
*
* Side Effects: none.
*
********************************************************************/
void        TeExClearBuffer(TEXTENTRYEX *pTe);

/*********************************************************************
* Function: BOOL TeIsKeyPressed(TEXTENTRY *pTe, uint16_t index)
*
* Overview: This function will test if a key given by its index
*			in the TextEntry object has been pressed.
*
* PreCondition: none
*
* Input: 	pTe- pointer to the object
*			index- index to the key in the link list
* Output:  	Returns a TRUE if the key is pressed. FALSE if key
*			is not pressed or the given index does not exist in
*			the list.
*
* Side Effects: none.
*
********************************************************************/
bool        TeExIsKeyPressed(TEXTENTRYEX *pTe, uint16_t index);

/*********************************************************************
* Function: void TeSetKeyCommand(TEXTENTRY *pTe,uint16_t index,uint16_t command)
*
* Overview: This function will assign a command (TEEX_DELETE_COM, TEEX_SPACE_COM
*			or TEEX_ENTER_COM) to a key with the given index.
*
* PreCondition: none
*
* Input: 	pTe  - 		pointer to the object
*			index  - 	index to the key in the link list
*			command- 	command assigned for the key
*
* Output:  	Returns TRUE if successful and FALSE if not.
*
* Side Effects: none.
*
********************************************************************/
bool        TeExSetKeyCommand(TEXTENTRYEX *pTe, uint16_t index, uint16_t command);

/*********************************************************************
* Function: TeGetKeyCommand(pTe, index)
*
* Overview: This function will return the currently used command by a key
*			with the given index.
*
* PreCondition: none
*
* Input: 	pTe- pointer to the object
*			index- index to the key in the link list
*
* Output:  It will return the command ID currently set for the key. If the
*          given index is not in the list the function returns zero.
*          0x00 - no command is assigned or the index given does not exist.
*          0x01 - TEEX_DELETE_COM
*          0x02 - TEEX_SPACE_COM
*          0x03 - TEEX_ENTER_COM
*
* Side Effects: none.
*
********************************************************************/
uint16_t        TeExGetKeyCommand(TEXTENTRYEX *pTe, uint16_t index);

/*********************************************************************
* Function: TeSetKeyText(TEXTENTRY *pTe, uint16_t index, GFX_XCHAR *pText)
*
* Overview: This function will set the test assigned to a key with
*			the given index.
*
* PreCondition: none
*
* Input: 	pTe  - 		pointer to the object
*			index  - 	index to the key in the link list
*			pText - 	pointer to the new string to be used
*
* Output:  	Returns TRUE if successful and FALSE if not.
*
* Side Effects: none.
*
********************************************************************/
bool TeExSetKeyText(TEXTENTRYEX *pTe, uint16_t index, GFX_XCHAR *pText);

/*********************************************************************
* Function: KEYMEMBER *TeCreateKeyMembers(TEXTENTRY *pTe,GFX_XCHAR *pText[])
*
* Overview: This function will create the list of KEYMEMBERS that holds the
*			information on each key. The number of keys is determined by the
*			equation (verticalKeys*horizontalKeys). The object creates the information
*			holder for each key automatically and assigns each entry in the *pText[]
*			array with the first entry automatically assigned to the key with an
*			index of 1. The number of entries to *pText[] must be equal or greater
*			than (verticalKeys*horizontalKeys). The last key is assigned with an index
*			of (verticalKeys*horizontalKeys)-1. No checking is performed on the
*			length of *pText[] entries to match (verticalKeys*horizontalKeys).
*
* PreCondition: none
*
* Input: 	pTe  - 		pointer to the object
*			pText -		pointer to the text defined by the user
*
* Output: Returns the pointer to the newly created KEYMEMBER list. A NULL is returned
*		  if the list is not created succesfully.
*
* Side Effects: none.
*
********************************************************************/
TEEX_KEYMEMBER   *TeExCreateKeyMembers(TEXTENTRYEX *pTeEx, GFX_XCHAR *pText[], GFX_XCHAR *pTextAlternate[], GFX_XCHAR *pTextShift[], GFX_XCHAR *pTextShiftAlternate[], int16_t aCommandKeys[]);

/*********************************************************************
* Function: void TeDelKeyMembers(void *pObj)
*
* Overview: This function will delete the KEYMEMBER list assigned to
*			the object from memory. Pointer to the KEYMEMBER list is
*			then initialized to NULL.
*
* PreCondition: none
*
* Input: 	pTe  - 	pointer to the object
*
* Output:  none.
*
* Side Effects: none.
*
********************************************************************/
void        TeExDelKeyMembers(void *pObj);

/*********************************************************************
* Function: void TeSpaceChar(TEXTENTRY *pTe)
*
* Overview: This function will insert a space character to the end of
*			the buffer. Drawing states TEEX_UPDATE_TEXT or TEEX_DRAW must
*			be set to see the effect of this insertion.
*
* PreCondition: none
*
* Input: 	pTe  - 	pointer to the object
*
* Output:  none.
*
* Side Effects: none.
*
********************************************************************/
void        TeExSpaceChar(TEXTENTRYEX *pTe);

/*********************************************************************
* Function: void TeAddChar(TEXTENTRY *pTe)
*
* Overview: This function will insert a character to the end of
*			the buffer. The character inserted is dependent on the
*			currently pressed key. Drawing states TEEX_UPDATE_TEXT or
*			TEEX_DRAW must be set to see the effect of this insertion.
*
* PreCondition: none
*
* Input: 	pTe  - 	pointer to the object
*
* Output:
*
* Side Effects: none.
*
********************************************************************/
void        TeExAddChar(TEXTENTRYEX *pTe);

/*********************************************************************
* Overview: This structure defines the list of translated messages for
*           MessageBox Objects.
*
*********************************************************************/
typedef enum {
    TEEX_MSG_INVALID      = 0,      // Invalid message response.
    TEEX_MSG_ADD_CHAR,              // TextEntry add character action ID
    TEEX_MSG_BKSP,                  // TextEntry backspace character action ID
    TEEX_MSG_SPACE,                 // TextEntry add space character action ID
    TEEX_MSG_ENTER,                 // TextEntry enter action ID
    TEEX_MSG_SHIFT,                 // TextEntry shift action ID
    TEEX_MSG_CAPSLOCK,              // TextEntry Caps Lock action ID
    TEEX_MSG_ALTERNATE,             // TextEntry alternate action ID
    TEEX_MSG_PRESSED,               // TextEntry pressed action ID
    TEEX_MSG_RELEASED               // TextEntry released action ID
} TRANS_MSG_TEXTENTRYEX;

#endif // _TEXTENTRYEX_H
