/*****************************************************************************
 *  Module for Microchip Graphics Library
 *  Solomon Systech. SSD1963 LCD controller driver
 *****************************************************************************
 * FileName:        SSD1963.h
 * Dependencies:    p24Fxxxx.h or plib.h
 * Processor:       PIC24, PIC32
 * Compiler:        MPLAB C30, MPLAB C32
 * Linker:          MPLAB LINK30, MPLAB LINK32
 * Company:         TechToys Company
 * Remarks:         The origin of this file was the ssd1926.c driver released
 *                  by Microchip Technology Incorporated. 
 *
 * Company:         Microchip Technology Incorporated
 *
 * Software License Agreement
 *
 * Copyright (c)2008 Microchip Technology Inc.  All rights reserved.
 * Microchip licenses to you the right to use, modify, copy and distribute
 * Software only when embedded on a Microchip microcontroller or digital
 * signal controller, which is integrated into your product or third party
 * product (pursuant to the sublicense terms in the accompanying license
 * agreement).  
 *
 * You should refer to the license agreement accompanying this Software
 * for additional information regarding your rights and obligations.
 *
 * SOFTWARE AND DOCUMENTATION ARE PROVIDED �AS IS?WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY
 * OF MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR
 * PURPOSE. IN NO EVENT SHALL MICROCHIP OR ITS LICENSORS BE LIABLE OR
 * OBLIGATED UNDER CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION,
 * BREACH OF WARRANTY, OR OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT
 * DAMAGES OR EXPENSES INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL,
 * INDIRECT, PUNITIVE OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 * COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY
 * CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF),
 * OR OTHER SIMILAR COSTS.
 *
 * Author               Date        Comment
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Anton Alkhimenok     08/27/08
 *****************************************************************************/

/*
******************************************************************************
* Versions 120709
*
* John Leung @ TechToys Co.			12/07/09
* www.TechToys.com.hk
******************************************************************************
*/

/*
******************************************************************************
* Remarks: 
* 1. Removed hardware definition for LED_LAT_BIT and LED_TRIS_BIT
*	  because PWM pin of SSD1963 applied, therefore backlight intensity
*	  set by software
* 2. Add new function void SetBacklight(BYTE intensity)
* 3. Funny finding, PLL to 120MHz work only when 10MHz crystal applied with 
* 	  multiplier N = 35. A crystal with 4MHz attempted but the PLL frequency 
*	  failed to boost to 120MHz somehow!
*
* John Leung @ TechToys Co.			09/09/2009
* www.TechToys.com.hk
******************************************************************************
*/

/*
******************************************************************************
* Remarks:
* Port ot Microchip Graphics Library v2.00
* (1) Only BLOCKING CONFIGURATION is supported
* (2) GetPixel() not working yet.
* John Leung @ TechToys Co.			15th Jan 2010
* www.TechToys.com.hk
******************************************************************************
*/
/*
******************************************************************************
* Revision
* An attempt to make use of 74HC573 latch on Rev3A board and it is working.
* Date: 8th April 2011
******************************************************************************
*/

/*
******************************************************************************
* Revision:
*
* Three new functions for power management. They are 
* void DisplayOff(void), void DisplayOn(void), void EnterDeepSleep(void).
* The hardware consists of a SSD1963 Rev2A EVK + PIC24/32 EVK R2C with 
* PIC32 GP Starter kit stacked on it. Display was a 4.3" TFT LCD (TY430TFT480272 Rev03).
*
* With Display state set to Display ON, the current drawn was 192mA,
* being the full power state. This is a baseline.
*
* With EnterDeepSleep() function executed, current reduced to 128mA
* with instant blackout of the display.
* 
* Dummy read was not implemented to bring SSD1963 out of the deep sleep state.
* Instead, DisplayOn() function executed which was also able to bring the TFT
* to display ON state without any loss on the screen content.
* However, it would be advised to follow the datasheet for a proper 
* deep sleep state exit.
* 
* User may refer to the state chart on page 19 of SSD1963 datasheet
* regarding individual power states
* 
* Date: 20th April 2011
******************************************************************************
*/

/*
******************************************************************************
* Revision
* Port to Microchip Graphics Library version 3.01
* (1) Removed basic color definition, now defined under gfxcolors.h
* (2) Removed SetColor(color), GetColor(), GetMaxX(), GetMaxY(), ResetDevice(),
*	  CLIP_DISABLE, CLIP_ENABLE, _color, PutPixel(x,y), GetPixel(x,y), 
*	  DelayMs(time), IsDeviceBusy(), SetClip(control), SetClipRgn(left,top,right,bottom)
*		They are now defined under DisplayDriver.h.
* (3) Removed _clipRgn, _clipLeft, _clipTop, _clipRight, and _clipBottom.
*		They are now defined under DisplayDriver.h
* (4) Removed _visualPage and _activePage, SetVisualPage(page) and SetActivePage(page)
*		They are now defined under DisplayDriver.h
* (5) SetActivePage(0) and SetVisualPage(0) now udner ResetDevice().
* (6) Removed PAGE_MEM_SIZE. It is no longer required
* (7) Removed GetClipTop(), GetClipRight(), GetClipLeft(), and GetClipBottom().
*		They are now defined under DisplayDriver.h
* 
* Programmer: John Leung @ www.TechToys.com.hk
* Date: 10th Aug 2011
******************************************************************************
*/

/*
******************************************************************************
* Revision
* Add support for Double Buffering (USE_DOUBLE_BUFFERING)
* Programmer: John Leung @ www.TechToys.com.hk
* Date: 15th Aug 2011
******************************************************************************
*/

/*
******************************************************************************
* Revision:
* Removed all PutImage() functions as they are now declared under Primitive.c
* with Graphcis Library Version 3.0.1
* Programmer: John Leung @ www.TechToys.com.hk
* Date: 5th Sept 2011
******************************************************************************
*/

#ifndef _SSD1963_H
#define _SSD1963_H

#ifdef __PIC32MX
	#include <plib.h>
	#define PMDIN1   PMDIN
#elif defined __dsPIC33F__
	#include <p33Fxxxx.h>
#elif defined __PIC24H__
	#include <p24Hxxxx.h>
#elif defined __PIC24F__
	#include <p24Fxxxx.h>
#else
	#error CONTROLLER IS NOT SUPPORTED
#endif

//include the command table for SSD1963
#include "SSD1963_CMD.h"
#include "Graphics/DisplayDriver.h"

/*********************************************************************
* Overview: Additional hardware-accelerated functions can be implemented
*           in the driver. These definitions exclude the PutPixel()-based
*           functions in the primitives layer (Primitive.c file) from compilation.
*********************************************************************/

// Define this to implement Bar function in the driver.
//#define USE_DRV_BAR

// Define this to implement ClearDevice function in the driver.
//#define USE_DRV_CLEARDEVICE

// Define this to implement PutImage function in the driver.
//#define USE_DRV_PUTIMAGE


/*********************************************************************
* PARAMETERS VALIDATION
*********************************************************************/
#if COLOR_DEPTH != 16
#error This driver supports 16 BPP only.
#endif

#if (DISP_HOR_RESOLUTION % 8) != 0
#error Horizontal resolution must be divisible by 8.
#endif

#if (DISP_ORIENTATION != 0) && (DISP_ORIENTATION != 180) && (DISP_ORIENTATION != 90) && (DISP_ORIENTATION != 270)
#error The display orientation selected is not supported. It can be only 0,90,180 or 270.
#endif

/*********************************************************************
* ADD SUPPORT FOR DOUBLE BUFFERING
*********************************************************************/
// Calculate Display Buffer Size required in bytes
#define GFX_DISPLAY_PIXEL_COUNT ((DWORD)DISP_HOR_RESOLUTION*DISP_VER_RESOLUTION)

#if (COLOR_DEPTH == 16)
    #define GFX_REQUIRED_DISPLAY_BUFFER_SIZE_IN_BYTES       (GFX_DISPLAY_PIXEL_COUNT*2)
#elif (COLOR_DEPTH == 8)
    #define GFX_REQUIRED_DISPLAY_BUFFER_SIZE_IN_BYTES       (GFX_DISPLAY_PIXEL_COUNT)
#elif (COLOR_DEPTH == 4)
    #define GFX_REQUIRED_DISPLAY_BUFFER_SIZE_IN_BYTES       (GFX_DISPLAY_PIXEL_COUNT/2)
#elif (COLOR_DEPTH == 1)
    #define GFX_REQUIRED_DISPLAY_BUFFER_SIZE_IN_BYTES       (GFX_DISPLAY_PIXEL_COUNT/8)
#endif


typedef struct
{
    WORD X;
    WORD Y;
    WORD W;
    WORD H;
} RectangleArea;

#if defined (USE_DOUBLE_BUFFERING)
	#define GFX_MAX_INVALIDATE_AREAS 5
    #define GFX_BUFFER1 (GFX_DISPLAY_BUFFER_START_ADDRESS)
    #define GFX_BUFFER2 (GFX_DISPLAY_BUFFER_START_ADDRESS + GFX_REQUIRED_DISPLAY_BUFFER_SIZE_IN_BYTES)
#endif

/*********************************************************************
* Macros: SetPalette(colorNum, color)
*
* Overview:  Sets palette register.
*
* PreCondition: none
*
* Input: colorNum - Register number.
*        color - Color.
*
* Output: none
*
* Side Effects: none
*
********************************************************************/
#define SetPalette(colorNum, color)


/*********************************************************************
* Function:  SetScrollArea(SHORT top, SHORT scroll, SHORT bottom)
*
* PreCondition: none
*
* Input: top - Top Fixed Area in number of lines from the top
*				of the frame buffer
*		 scroll - Vertical scrolling area in number of lines
*		 bottom - Bottom Fixed Area in number of lines
*
* Output: none
*
* Side Effects: none
*
* Overview:
*
* Note: Reference: section 9.22 Set Scroll Area, SSD1963 datasheet Rev0.20
*
********************************************************************/
void SetScrollArea(SHORT top, SHORT scroll, SHORT bottom);

/*********************************************************************
* Function:  void  SetScrollStart(SHORT line)
*
* Overview: First, we need to define the scrolling area by SetScrollArea()
*			before using this function. 
*
* PreCondition: SetScrollArea(SHORT top, SHORT scroll, SHORT bottom)
*
* Input: line - Vertical scrolling pointer (in number of lines) as 
*		 the first display line from the Top Fixed Area defined in SetScrollArea()
*
* Output: none
*
* Note: Example -
*
*		SHORT line=0;
*		SetScrollArea(0,272,272);
*		for(line=0;line<272;line++) {SetScrollStart(line);DelayMs(100);}
*		
*		Code above scrolls the whole page upwards in 100ms interval 
*		with page 2 replacing the first page in scrolling
********************************************************************/
void SetScrollStart(SHORT line);

/*********************************************************************
* Function:  void EnterSleepMode (void)
* PreCondition: none
* Input:  none
* Output: none
* Side Effects: none
* Overview: SSD1963 enters sleep mode
* Note: Host must wait 5mS after sending before sending next command
********************************************************************/
void EnterSleepMode (void);

/*********************************************************************
* Function:  void ExitSleepMode (void)
* PreCondition: none
* Input:  none
* Output: none
* Side Effects: none
* Overview: SSD1963 exits sleep mode
* Note:   cannot be called sooner than 15ms
********************************************************************/
void ExitSleepMode (void);

/*********************************************************************
* Function		: void DisplayOff(void)
* PreCondition	: none
* Input			: none
* Output		: none
* Side Effects	: none
* Overview		: SSD1963 changes the display state to OFF state
* Note			: none
********************************************************************/
void DisplayOff(void);

/*********************************************************************
* Function		: void DisplayOn(void)
* PreCondition	: none
* Input			: none
* Output		: none
* Side Effects	: none
* Overview		: SSD1963 changes the display state to ON state
* Note			: none
********************************************************************/
void DisplayOn(void);

/*********************************************************************
* Function		: void EnterDeepSleep(void)
* PreCondition	: none
* Input			: none
* Output		: none
* Side Effects	: none
* Overview		: SSD1963 enters deep sleep state with PLL stopped
* Note			: none
********************************************************************/
void EnterDeepSleep(void);

/*********************************************************************
* Function:  void  SetBacklight(BYTE intensity)
*
* Overview: This function makes use of PWM feature of ssd1963 to adjust
*			the backlight intensity. 
*
* PreCondition: Backlight circuit with shutdown pin connected to PWM output of ssd1963.
*
* Input: 	(BYTE) intensity from 
*			0x00 (total backlight shutdown, PWM pin pull-down to VSS)
*			0xff (99% pull-up, 255/256 pull-up to VDD)
*
* Output: none
*
* Note: The base frequency of PWM set to around 300Hz with PLL set to 120MHz.
*		This parameter is hardware dependent
********************************************************************/
void SetBacklight(BYTE intensity);

/*********************************************************************
* Function:  void  SetTearingCfg(BOOL state, BOOL mode)
*
* Overview: This function enable/disable tearing effect
*
* PreCondition: none
*
* Input: 	BOOL state -	1 to enable
*							0 to disable
*			BOOL mode -		0:  the tearing effect output line consists
*								of V-blanking information only
*							1:	the tearing effect output line consists
*								of both V-blanking and H-blanking info.
* Output: none
*
* Note:
********************************************************************/
void SetTearingCfg(BOOL state, BOOL mode);


/************************************************************************
* Macro: Lo                                                             *
* Preconditions: None                                                   *
* Overview: This macro extracts a low byte from a 2 byte word.          *
* Input: None.                                                          *
* Output: None.                                                         *
************************************************************************/
#define Lo(X)   (BYTE)(X&0x00ff)

/************************************************************************
* Macro: Hi                                                             *
* Preconditions: None                                                   *
* Overview: This macro extracts a high byte from a 2 byte word.         *
* Input: None.                                                          *
* Output: None.                                                         *
************************************************************************/
#define Hi(X)   (BYTE)((X>>8)&0x00ff)

void SetBacklight(BYTE intensity);

#endif // _SSD1963_H
