// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// StaticTextEx - MLA version
// *****************************************************************************
// FileName:        statictext_ex.h
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
//  2013/11/19	Initial release
//  2014/09/07  MLA version
// *****************************************************************************

#ifndef _STATICTEXTEX_H
    #define _STATICTEXTEX_H

#include "system.h"
#include <stdlib.h>
#include "gfx/gfx_gol.h"

/*********************************************************************
* Object States Definition: 
*********************************************************************/
#define STEX_DISABLED     0x0002  // Bit for disabled state.
#define STEX_RIGHT_ALIGN  0x0004  // Bit to indicate text is left aligned.
#define STEX_CENTER_ALIGN 0x0008  // Bit to indicate text is center aligned.
#define STEX_FRAME        0x0010  // Bit to indicate frame is displayed.
#define STEX_NOPANEL      0x0020  // Bit to indicate bacground panel is disabled.
#define STEX_UPDATE       0x2000  // Bit to indicate that text area only is redrawn.
#define STEX_DRAW         0x4000  // Bit to indicate static text must be redrawn.
#define STEX_HIDE         0x8000  // Bit to remove object from screen.

/* Indent constant for the text used in the frame. */
#define STEX_INDENT   0x02        // Text indent constant.

#define OBJ_STATICTEXTEX GFX_GOL_UNKNOWN_TYPE+1012
#define STEX_MSG_SELECTED GFX_GOL_OBJECT_ACTION_PASSIVE+1012

/*********************************************************************
* Overview: Defines the parameters required for a Static Text Object.
*
*********************************************************************/
typedef struct
{
    GFX_GOL_OBJ_HEADER  hdr;        // Generic header for all Objects (see GFX_GOL_OBJ_HEADER).
    int16_t             textHeight; // Pre-computed text height.
    GFX_XCHAR           *pText;     // The pointer to text used.
} STATICTEXTEX;

/*********************************************************************
* Macros:  StExGetText(pSt)
*
* Overview: This macro returns the address of the current 
*			text string used for the object.
*
* PreCondition: none
*
* Input: pSt - Pointer to the object.
*
* Output: Returns the pointer to the text string used.
*
* Side Effects: none
*
********************************************************************/
    #define StExGetText(pSt)  pSt->pText

/*********************************************************************
* Function: StExSetText(STATICTEXTEX *pSt, XCHAR *pText)
*
* Overview: This function sets the string that will be used for the object.
*
* PreCondition: none
*
* Input: pSt - The pointer to the object whose text string will be modified. 
*        pText - The pointer to the string that will be used.
*
* Output: none
*
* Side Effects: none
*
********************************************************************/
void        StExSetText(STATICTEXTEX *pSt, GFX_XCHAR *pText);

/*********************************************************************
* Function: STATICTEXTEX  *StExCreate(uint16_t ID, int16_t left, int16_t top, int16_t right, int16_t bottom,
*								  uint16_t state , XCHAR *pText, GFX_GOL_OBJ_SCHEME *pScheme)
*
* Overview: This function creates a STATICTEXTEX object with the 
*			parameters given. It automatically attaches the new 
*			object into a global linked list of objects and returns 
*			the address of the object.
*
* PreCondition: none
*
* Input: ID - Unique user defined ID for the object instance.
*        left - Left most position of the object.
* 		 top - Top most position of the object. 
*		 right - Right most position of the object.
*		 bottom - Bottom most position of the object.
*        state - Sets the initial state of the object.
*        pText - Pointer to the text used in the static text.
*        pScheme - Pointer to the style scheme. Set to NULL if 
*				   default style scheme is used.
*
* Output: Returns the pointer to the object created.
*
* Example:
*   <CODE> 
*	GFX_GOL_OBJ_SCHEME *pScheme;
*	STATICTEXTEX *pSt;
*		
*		pScheme = GOLCreateScheme();
*		state = STEX_DRAW | STEX_FRAME | STEX_CENTER_ALIGN;
*		StExCreate(ID_STATICTEXTEX1,          // ID
*		         30,80,235,160,           // dimension
*		         state,                   // has frame and center aligned
*		         "Static Text\n Example", // text
*		         pScheme);                // use given scheme
*		
*		while(!StDraw(pSt));			  // draw the object
*	</CODE> 
*
* Side Effects: none
*
********************************************************************/
STATICTEXTEX  *StExCreate
            (
                uint16_t        ID,
                int16_t       left,
                int16_t       top,
                int16_t       right,
                int16_t       bottom,
                uint16_t        state,
                GFX_XCHAR       *pText,
                GFX_GOL_OBJ_SCHEME  *pScheme
            );

/*********************************************************************
* Function: uint16_t StExTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg)
*
* Overview: This function evaluates the message from a user if the 
*			message will affect the object or not. The table below 
*			enumerates the translated messages for each event of the 
*			touch screen and keyboard inputs.
*
*	<TABLE>
*    	Translated Message   Input Source  Events         				Description
*     	##################   ############  ######         				###########
*		STEX_MSG_SELECTED      Touch Screen  EVENT_PRESS, EVENT_RELEASE   If events occurs and the x,y position falls in the area of the static text.
*		GFX_GOL_OBJECT_ACTION_INVALID		 Any		   Any							If the message did not affect the object.
*	</TABLE>
*
* PreCondition: none
*
* Input: pSt   - The pointer to the object where the message will be
*				 evaluated to check if the message will affect the object.
*        pMsg  - Pointer to the message struct containing the message from 
*        		 the user interface.
*
* Output: Returns the translated message depending on the received GOL message:
*		  - STEX_MSG_SELECTED ? Static Text is selected
*    	  - GFX_GOL_OBJECT_ACTION_INVALID ? Static Text is not affected
*
* Example:
*   Usage is similar to BtnTranslateMsg() example.
*
* Side Effects: none
*
********************************************************************/
uint16_t        StExTranslateMsg(void *pObj, GFX_GOL_MESSAGE *pMsg);

/*********************************************************************
* Function: uint16_t StExDraw(void *pObj)
*
* Overview: This function renders the object on the screen using 
*			the current parameter settings. Location of the object 
*			is determined by the left, top, right and bottom 
*			parameters. The colors used are dependent on the state 
*			of the object. The font used is determined by the style 
*			scheme set.
*			
*			When rendering objects of the same type, each object must 
*			be rendered completely before the rendering of the next 
*			object is started. This is to avoid incomplete object rendering.
*
* PreCondition: Object must be created before this function is called.
*
* Input: pSt - Pointer to the object to be rendered.
*        
* Output: Returns the status of the drawing
*		  - 1 - If the rendering was completed and 
*		  - 0 - If the rendering is not yet finished. 
*		  Next call to the function will resume the 
*		  rendering on the pending drawing state.
*
* Example:
*   See StExCreate() Example.
*
* Side Effects: none
*
********************************************************************/
uint16_t StExDraw(void *pObj);
#endif // _STATICTEXTEX_H
