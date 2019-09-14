/*****************************************************************************
 *  Module for Microchip Graphics Library
 *  Solomon Systech. SSD1963 LCD controller driver
 *****************************************************************************
 * FileName:        SSD1963.c
 * Dependencies:    Graphics.h
 * Processor:       PIC24, PIC32
 * Compiler:        MPLAB C30, MPLAB C32
 * Linker:          MPLAB LINK30, MPLAB LINK32
 * Company:	    TechToys Company
 * Remarks:	    The origin of this file was the ssd1926.c driver released
 *		    by Microchip Technology Incorporated. 
 *
 * Software License Agreement as below:
 *
 * Company:         Microchip Technology Incorporatedpage

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
 * SOFTWARE AND DOCUMENTATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
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
 *****************************************************************************
 * Anton Alkhimenok     08/27/08	(Original)
 * 	
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
 * Revision:
 * Port ot Microchip Graphics Library v2.00
 * (1) Only BLOCKING CONFIGURATION is supported
 * (2) GetPixel() not working yet.
 * John Leung @ TechToys Co.			15th Jan 2010
 * www.TechToys.com.hk
 ******************************************************************************
 */


/*
 ******************************************************************************
 * Revision:
 * (1) Optimize for WriteData() by removing CS_LAT_BIT for each write cycle
 *	  and append CS strobe between multiple WriteData()
 *
 * John Leung @ TechToys Co.			3rd Feb 2010
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
 * Revision:
 * (1) Port to Microchip Graphics Library version 3.01
 * (2) SetActivePage(0), SetVisualPage(0) under ResetDevice()
 * (3) Removed WORD PutImage(SHORT left, SHORT top, void* bitmap, BYTE stretch)
 *		It is now declared under primitive.c and primitive.h
 *
 * Programmer: John Leung @ www.TechToys.com.hk
 * Date: 10th Aug 2011
 ******************************************************************************
 */

/*
 ******************************************************************************
 * Revision:
 * Add support for double buffering
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

/*
 ******************************************************************************
 * Revision:
 * Removed #if defined (USE_DISPLAY_CONTROLLER_SSD1963_R3B) ..#endif
 * Programmer: John Leung @ www.TechToys.com.hk
 * Date: 9th Feb 2012
 ******************************************************************************
 */

/*
 ******************************************************************************
 * Revision:
 * Add support for USE_8BIT_PMP with PIC32MX795F512L on Explorer 16
 * Add support for TY700TFT800480_R3
 * Programmer: John Leung @ www.TechToys.com.hk
 * Date: 28th Aug 2012
 ******************************************************************************
 */

/*
 ******************************************************************************
 * Revision:
 * changes on WriteCommand() and WriteData() for compatibility with Explorer 16
 * for 8-bit and 16-bit PMP
 * Programmer: John Leung @ www.TechToys.com.hk
 * Date: 6th Sept 2012
 ******************************************************************************
 */

/*
 ******************************************************************************
 * Revision:
 * Add WriteCommandSlow() and WriteDataSlow() for Fosc before PLL locked
 *	Before PLL is locked, Fosc = 10MHz only.
 * Add support for USE_MCHP_PMPWR
 *
 * Programmer: John Leung @ www.TechToys.com.hk
 * Date: 21st Sept 2012
 ******************************************************************************
 */

/*
 ******************************************************************************
 * Revision:
 * Changed CS_LAT_BIT, RS_LAT_BIT, RST_LAT_BIT... references to 
 *	   DisplayEnable(), DisplaySetCommand(), DisplayResetEnable()...
 *     to support MAL 3.0x
 * Replaced USE_MCHP_PMPWR with USE_GFX_PMP to support MCHP standard functions and initialization
 *
 * Programmer: VirtualFab @ www.Virtualfab.it
 * Date: 15th Dec 2012
 ******************************************************************************
 */

#include "HardwareProfile.h"
#include "TimeDelay.h"
#include "Graphics/DisplayDriver.h"
#include "SSD1963.h"
#include "Graphics/Primitive.h"
#include "Graphics/gfxpmp.h"

#ifdef USE_DOUBLE_BUFFERING
BYTE blInvalidateAll;
BYTE blEnableDoubleBuffering;
BYTE NoOfInvalidatedRectangleAreas;
RectangleArea InvalidatedArea[GFX_MAX_INVALIDATE_AREAS];

volatile DWORD _drawbuffer;
volatile BYTE blDisplayUpdatePending;

static void ExchangeDrawAndFrameBuffers(void);
#endif //USE_DOUBLE_BUFFERING

GFX_COLOR _color; // Color
#ifdef USE_TRANSPARENT_COLOR
GFX_COLOR _colorTransparent;
SHORT _colorTransparentEnable;
#endif

#ifdef GFX_DRV_PAGE_COUNT
volatile DWORD _PageTable[GFX_DRV_PAGE_COUNT];
#endif

SHORT _clipRgn; // Clipping region control
SHORT _clipLeft; // Clipping region borders
SHORT _clipTop;
SHORT _clipRight;
SHORT _clipBottom;

BYTE _activePage; // Active Page
BYTE _visualPage; // Visual Page

BYTE _gpioStatus = 0; // ssd1963 specific

void PutImage1BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch);
void PutImage4BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch);
void PutImage8BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch);
void PutImage16BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch);

void PutImage1BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch);
void PutImage4BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch);
void PutImage8BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch);
void PutImage16BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch);

/**************** LOCAL FUNCTION PROTOTYPE (SSD1963 SPECIFIC) ****************/
static void SetArea(SHORT start_x, SHORT start_y, SHORT end_x, SHORT end_y);
static void GPIO_WR(BYTE pin, BOOL state);
static void SPI_Write(BYTE byte);
static void SPI_SetReg(BYTE reg, WORD cmd);

/*********************************************************************
 * Function: IsDeviceBusy()
 *
 * Overview: Returns non-zero if LCD controller is busy
 *           (previous drawing operation is not completed).
 *
 * PreCondition: none
 *
 * Input: none
 *
 * Output: Busy status.
 *
 * Remarks: Not implemented yet
 *
 ********************************************************************/
WORD IsDeviceBusy() {
    return 0;
}

#if defined (USE_DOUBLE_BUFFERING)

/*********************************************************************
 * Function:  static void ExchangeDrawAndFrameBuffers(void);
 *
 * Overview: Interchanges Draw and Frame buffers and copies the contents
 *           of current frame buffer to the draw buffer
 *
 * PreCondition: The graphical frame must be completely drawn.
 *
 * Input: None
 *
 * Output: None
 *
 * Side Effects: Always draw on draw buffer & not on frame buffer
 *
 ********************************************************************/
static void ExchangeDrawAndFrameBuffers(void) {
    DWORD SourceBuffer, DestBuffer;

    if (blEnableDoubleBuffering == 0) {
        return;
    }

    if (_drawbuffer == GFX_BUFFER1) {
        SourceBuffer = GFX_BUFFER1;
        DestBuffer = GFX_BUFFER2;
    } else {
        SourceBuffer = GFX_BUFFER2;
        DestBuffer = GFX_BUFFER1;
    }

    _drawbuffer = DestBuffer;
    //...
}

/*********************************************************************
 * Function:  SwitchOnDoubleBuffering()
 *
 * Overview: Switches on the double buffering.
 *			Double buffering utilizes two buffers. The frame buffer and the
 *           draw buffer. The frame buffer is the buffer that is being displayed
 *			while the draw buffer is used for all rendering.
 *           When this function is called, it copies the contents of the frame buffer
 *           to the draw buffer once and all succeeding rendering will be performed on
 *           the draw buffer. To update the frame buffer with newly drawn
 *           items on the draw buffer call UpdateDisplayNow() or RequestDisplayUpdate().
 *
 * PreCondition: none
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void SwitchOnDoubleBuffering(void) {
    if (blEnableDoubleBuffering == 0) {
        blEnableDoubleBuffering = 1;
        InvalidateAll();
    }
}

/*********************************************************************
 * Function:  SwitchOffDoubleBuffering()
 *
 * Overview: Switches off the double buffering.
 *           All rendering will be performed on the frame buffer. Calls
 *           to UpdateDisplayNow() or RequestDisplayUpdate() will
 *           have no effect.
 *
 * PreCondition: none
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void SwitchOffDoubleBuffering(void) {
    if (blEnableDoubleBuffering == 1) {
        UpdateDisplayNow();
        _drawbuffer = (_drawbuffer == GFX_BUFFER1) ? GFX_BUFFER2 : GFX_BUFFER1;
        blEnableDoubleBuffering = 0;
    }
}

/*********************************************************************
 * Function:  void UpdateDisplayNow(void)
 *
 * Overview: Synchronizes the draw and frame buffers immediately
 *
 * PreCondition: none
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Remarks:	For SSD1963, this is equivalent to updating the pointer
 *			to the DrawBuffer and swap the role of DrawBuffer with
 *			FrameBuffer with ExchangeDrawAndFrameBuffers()
 ********************************************************************/
void UpdateDisplayNow(void) {
    SetScrollArea(0, GetMaxY() + 1, 0);
    if (_drawbuffer == GFX_BUFFER1) {
        SetScrollStart(0);
    } else {
        SetScrollStart(GetMaxY() + 1);
    }
    ExchangeDrawAndFrameBuffers();
}

/*********************************************************************
 * Function:  void RequestDisplayUpdate(void)
 *
 * Overview: Synchronizes the draw and frame buffers at next VBlank
 *
 * PreCondition: none
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 * Remarks: Not implemented yet, may use tearing effect to achieve this
 ********************************************************************/
void RequestDisplayUpdate(void) {
    //use tearing effect!
}

#endif	//USE_DOUBLE_BUFFERING

#if defined(__PIC32MX__)
    #define PMDIN1 PMDIN
#endif

#ifndef USE_GFX_PMP
    #define DeviceWrite(data) { PMPWaitBusy(); WR_LAT_BIT = 0; PMDIN1 = data; WR_LAT_BIT = 1; }
#endif

/*********************************************************************
 * Macros:  PMPWaitBusy()
 *
 * Overview: waits for PMP cycle end.
 *
 * PreCondition: none
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Note:
 ********************************************************************/
#define PMPWaitBusy()  while(PMMODEbits.BUSY);

/*************************************************************************************
 * Macros:  WriteCommand(cmd)
 *
 * PreCondition: PMCONbits.PTWREN = 1; 	// enable WR line with defined (USE_MCHP_PMPWR)
 *
 * Input: cmd - controller command
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: writes command
 *
 * Note: none
 *************************************************************************************/
#define WriteCommand(cmd) { DisplayEnable(); DisplaySetCommand(); DeviceWrite(cmd); DisplayDisable(); DisplaySetData(); }

/*********************************************************************
 * Macros:  WriteCommandSlow(cmd)
 *
 * PreCondition: Disable PMPWR with PMCONbits.PTWREN = 0 prior to using WriteCommandSlow(cmd)
 *				This is useful when PLL not started
 * Input: cmd - controller command
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: writes command
 *
 * Note: none
 ********************************************************************/
#define WriteCommandSlow(cmd) { PMPWaitBusy(); WriteCommand(cmd); }

/*********************************************************************
 * Macros:  WriteDataSlow(data)
 *
 * PreCondition: Disable PMPWR with PMCONbits.PTWREN = 0 prior to using WriteDataSlow(data)
 *				This is useful when PLL not started
 * Input: data - controller data
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: writes data
 *
 * Note: none
 ********************************************************************/
#define WriteDataSlow(data) { PMPWaitBusy(); DisplaySetData(); DisplayEnable(); DeviceWrite(data); DisplayDisable(); }

/*********************************************************************
 * Function:  void  WriteData(WORD data)
 *
 * PreCondition: 1. PMCONbits.PTWREN = 1; 	// enable WR line with defined (USE_MCHP_PMPWR)
 *				2. RS_LAT_BIT = 1
 * Input:  data - value to be written in WORD format
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview:
 ********************************************************************/
#if defined (USE_16BIT_PMP)
    #define WriteData(data) { DeviceWrite(data); }
#elif defined (USE_8BIT_PMP)	//USE_8BIT_PMP with PIC32MX460 or PIC32MX795 series on Explorer 16
    #if defined (USE_GFX_PMP)
        #define WriteData(data) { Nop(); PMDIN1 = data;}
        #define WriteColor(color) { Nop(); PMDIN1 = (color>>8); \
                                    Nop(); PMDIN1 = (color>>3); \
                                    Nop(); PMDIN1 = (color<<3); \
                                  }
    #else
        #define WriteData(data) { WR_LAT_BIT = 0; PMDIN1 = data;  WR_LAT_BIT = 1;}
        #define WriteColor(color) { WR_LAT_BIT = 0; PMDIN1 = (color>>8); WR_LAT_BIT = 1; \
                                    WR_LAT_BIT = 0; PMDIN1 = (color>>3); WR_LAT_BIT = 1; \
                                    WR_LAT_BIT = 0; PMDIN1 = (color<<3); WR_LAT_BIT = 1; \
                                  }
    #endif	//defined (USE_GFX_PMP)


#endif		//defined (USE_16BIT_PMP) / USE_8BIT_PMP

/*********************************************************************
 * Function: Set a GPIO pin to state high(1) or low(0)
 *
 * PreCondition: Set the GPIO pin an output prior using this function
 *
 * Arguments: BYTE pin	- 	LCD_RESET
 *							LCD_SPENA
 *							LCD_SPCLK
 *							LCD_SPDAT	defined under hardwareProfile_xxx.h
 *
 *			 BOOL state - 	0 for low
 *							1 for high
 * Return: none
 *
 * Note:
 *********************************************************************/
static void GPIO_WR(BYTE pin, BOOL state) {
    if (state == 1)
        _gpioStatus = _gpioStatus | pin;
    else
        _gpioStatus = _gpioStatus & (~pin);

    WriteCommand(0xBA); // Set GPIO value
    DisplayEnable();
    WriteData(_gpioStatus);
    DisplayDisable();
}

/*********************************************************************
 * Function:
 *
 * PreCondition: GPIO pins for the SPI port set all output prior to
 *               using this function
 * Arguments:
 *
 * Return: none
 *
 * Note:
 *********************************************************************/
static void SPI_Write(BYTE byte) {
    BYTE bit_ctr;

    for (bit_ctr = 0; bit_ctr < 8; bit_ctr++) {
        if (byte & 0x80)
            GPIO_WR(LCD_SPDAT, 1);
        else
            GPIO_WR(LCD_SPDAT, 0);

        GPIO_WR(LCD_SPCLK, 0);
        GPIO_WR(LCD_SPCLK, 1);
        byte = (byte << 1);
    }
}

/*********************************************************************
 * Functions:  SetActivePage(page)
 *
 * Overview: Sets active graphic page.
 *
 * PreCondition: none
 *
 * Input: page - Graphic page number.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void SetActivePage(WORD page) {
    _activePage = (BYTE) page;
}

/*********************************************************************
 * Functions: SetVisualPage(page)
 *
 * Overview: Sets graphic page to display.
 *
 * PreCondition: none
 *
 * Input: page - Graphic page number
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void SetVisualPage(WORD page) {
    _visualPage = (BYTE) page;
    SetScrollArea(0, GetMaxY() + 1, 0);
    SetScrollStart((SHORT) _visualPage * (GetMaxY() + 1));
}

/*********************************************************************
 * Function: SetClipRgn(left, top, right, bottom)
 *
 * Overview: Sets clipping region.
 *
 * PreCondition: none
 *
 * Input: left - Defines the left clipping region border.
 *		 top - Defines the top clipping region border.
 *		 right - Defines the right clipping region border.
 *	     bottom - Defines the bottom clipping region border.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void SetClipRgn(SHORT left, SHORT top, SHORT right, SHORT bottom) {
    _clipLeft = left;
    _clipTop = top;
    _clipRight = right;
    _clipBottom = bottom;
}

/*********************************************************************
 * Macros: SetClip(control)
 *
 * Overview: Enables/disables clipping.
 *
 * PreCondition: none
 *
 * Input: control - Enables or disables the clipping.
 *			- 0: Disable clipping
 *			- 1: Enable clipping
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void SetClip(BYTE control) {
    _clipRgn = control;
}

/*********************************************************************
 * Function:
 *
 * PreCondition: GPIO pins for the SPI port set all output prior to
 *               using this function
 * Arguments:
 *
 * Return: none
 *
 * Note:
 *********************************************************************/
static void SPI_SetReg(BYTE reg, WORD cmd) {
    GPIO_WR(LCD_SPENA, 0); // RS=0 for index read/write, RS=1 for instruction read/write
    SPI_Write(0x70); // This is a bitwise for (Device ID|RS|RW), therefore 0b011100|RS|RW, with RS=0, RW=0
    SPI_Write(0x00); // register in 0x00[reg]
    SPI_Write(reg);
    GPIO_WR(LCD_SPENA, 1);

    GPIO_WR(LCD_SPENA, 0);
    SPI_Write(0x72); // Now RS=1 & RW=0 for command set
    SPI_Write(((WORD_VAL) cmd).v[1]);
    SPI_Write(((WORD_VAL) cmd).v[0]);
    GPIO_WR(LCD_SPENA, 1);
}

/*********************************************************************
 * Function:  SetArea(start_x,start_y,end_x,end_y)
 *
 * PreCondition: SetActivePage(page)
 *
 * Input: start_x, end_x	- start column and end column
 *		 start_y,end_y 	- start row and end row position (i.e. page address)
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: defines start/end columns and start/end rows for memory access
 *			from host to SSD1963
 * Note: none
 ********************************************************************/
static void SetArea(SHORT start_x, SHORT start_y, SHORT end_x, SHORT end_y) {
    DWORD offset;

#if defined (USE_DOUBLE_BUFFERING)
    if (_drawbuffer == GFX_BUFFER1)
        offset = 0;
    else
        offset = (DWORD) (GetMaxY() + 1);
#else
    offset = (DWORD) _activePage * (GetMaxY() + 1);
#endif

    start_y = offset + start_y;
    end_y = offset + end_y;

    WriteCommand(CMD_SET_COLUMN);
    DisplayEnable();
    WriteData(start_x >> 8);
    WriteData(start_x);
    WriteData(end_x >> 8);
    WriteData(end_x);
    DisplayDisable();
    WriteCommand(CMD_SET_PAGE);
    DisplayEnable();
    WriteData(start_y >> 8);
    WriteData(start_y);
    WriteData(end_y >> 8);
    WriteData(end_y);
    DisplayDisable();
}

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
 ********************************************************************/
void SetScrollArea(SHORT top, SHORT scroll, SHORT bottom) {
    WriteCommand(CMD_SET_SCROLL_AREA);
    DisplayEnable();
    WriteData(top >> 8);
    WriteData(top);
    WriteData(scroll >> 8);
    WriteData(scroll);
    WriteData(bottom >> 8);
    WriteData(bottom);
    DisplayDisable();
}

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
 *		SetScrollArea(0,272,0);
 *		for(line=0;line<272;line++) {SetScrollStart(line);DelayMs(100);}
 *
 *		Code above scrolls the whole page upwards in 100ms interval
 *		with page 2 replacing the first page in scrolling
 ********************************************************************/
void SetScrollStart(SHORT line) {
    WriteCommand(CMD_SET_SCROLL_START);
    DisplayEnable();
    WriteData(line >> 8);
    WriteData(line);
    DisplayDisable();
}

/*********************************************************************
 * Function:  void EnterSleepMode (void)
 * PreCondition: none
 * Input:  none
 * Output: none
 * Side Effects: none
 * Overview: SSD1963 enters sleep mode
 * Note: Host must wait 5mS after sending before sending next command
 ********************************************************************/
void EnterSleepMode(void) {
    WriteCommand(CMD_ENT_SLEEP);
}

/*********************************************************************
 * Function:  void ExitSleepMode (void)
 * PreCondition: none
 * Input:  none
 * Output: none
 * Side Effects: none
 * Overview: SSD1963 enters sleep mode
 * Note:   none
 ********************************************************************/
void ExitSleepMode(void) {
    WriteCommand(CMD_EXIT_SLEEP);
}

/*********************************************************************
 * Function		: void DisplayOff(void)
 * PreCondition	: none
 * Input			: none
 * Output		: none
 * Side Effects	: none
 * Overview		: SSD1963 changes the display state to OFF state
 * Note			: none
 ********************************************************************/
void DisplayOff(void) {
    WriteCommand(CMD_BLANK_DISPLAY);
}

/*********************************************************************
 * Function		: void DisplayOn(void)
 * PreCondition	: none
 * Input			: none
 * Output		: none
 * Side Effects	: none
 * Overview		: SSD1963 changes the display state to ON state
 * Note			: none
 ********************************************************************/
void DisplayOn(void) {
    WriteCommand(CMD_ON_DISPLAY);
}

/*********************************************************************
 * Function		: void EnterDeepSleep(void)
 * PreCondition	: none
 * Input			: none
 * Output		: none
 * Side Effects	: none
 * Overview		: SSD1963 enters deep sleep state with PLL stopped
 * Note			: none
 ********************************************************************/
void EnterDeepSleep(void) {
    WriteCommand(CMD_ENT_DEEP_SLEEP);
}

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
void SetBacklight(BYTE intensity) {
    WriteCommand(0xBE); // Set PWM configuration for backlight control
    DisplayEnable();
    WriteData(0x0E); // PWMF[7:0] = 2, PWM base freq = PLL/(256*(1+5))/256 =
    // 300Hz for a PLL freq = 120MHz
    WriteData(intensity); // Set duty cycle, from 0x00 (total pull-down) to 0xFF
    // (99% pull-up , 255/256)
    WriteData(0x01); // PWM enabled and controlled by host (mcu)
    WriteData(0x00);
    WriteData(0x00);
    WriteData(0x00);

    DisplayDisable();
}

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
void SetTearingCfg(BOOL state, BOOL mode) {
    if (state == 1) {
        WriteCommand(0x35);
        DisplayEnable();
        WriteData(mode & 0x01);
        DisplayDisable();
    } else {
        WriteCommand(0x34);
    }
}

/*********************************************************************
 * Function:  void ResetDevice()
 *
 * PreCondition: none
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: Resets LCD, initializes PMP
 *			Initialize low level IO port for mcu,
 *			initialize SSD1963 for PCLK,
 *			HSYNC, VSYNC etc
 *
 * Note: Need to set the backlight intensity by SetBacklight(BYTE intensity)
 *		in main()
 *
 ********************************************************************/
void ResetDevice(void) {
    TE_TRIS_BIT = 1; // TE pin as input
    //set CS_LAT_BIT digital
    #if defined(__PIC32MX__) 
        AD1PCFGbits.PCFG15 	= 1;
    #elif defined(__dsPIC33E__) || defined(__PIC24E__)
//        AD1PCFGbits.PCFG15 	= 1;
    #endif
#ifdef USE_GFX_PMP
    DriverInterfaceInit(); // in gfxpmp.h
#else
    DisplayResetConfig(); //TFT_RST_LAT_BIT = 0;
    DisplayResetEnable(); //TFT_RST_TRIS_BIT = 0; // enable RESET line
    DisplayCmdDataConfig(); //TFT_RS_TRIS_BIT = 0; // enable RS line
    DisplayDisable(); // SSD1963 is not selected by default
    DisplayConfig(); //TFT_CS_TRIS_BIT = 0; // enable SSD1963 CS line

    RD_LAT_BIT = 1;
    RD_TRIS_BIT = 0;

    WR_LAT_BIT = 1;
    WR_TRIS_BIT = 0;

    // PMP setup
    PMMODE = 0;
    PMAEN = 0;
    PMCON = 0;
    PMMODEbits.MODE = 2; // Intel 80 master interface

    // Before PLL is set and locked, the reference clock = crystal freq.
    // Set the largest data setup time for 10MHz clock
    PMMODEbits.WAITB = 3;
    PMMODEbits.WAITM = 15;
    PMMODEbits.WAITE = 3;
    PMCONbits.PTRDEN = 0; // Disable RD line
    PMCONbits.PTWREN = 0; // Disable WR line

    #ifdef USE_16BIT_PMP
    PMMODEbits.MODE16 = 1; // 16 bit mode
    #else
    PMMODEbits.MODE16 = 0; // 8 bit mode
    #endif

    PMCONbits.PMPEN = 1; // enable PMP

    DisplayResetEnable(); //TFT_RST_LAT_BIT = 0;
    Delay10us(10);
    DisplayResetDisable(); //TFT_RST_LAT_BIT = 1; // release from reset state to sleep state
    Delay10us(10); // This delay time is very important, else, CMD_SET_PLL_MN command won't work!

#endif

    //Set MN(multipliers) of PLL, VCO = crystal freq * (M+1)
    //PLL freq = VCO/(N+1) with 250MHz < VCO < 800MHz, PLL frequency <110MHz
    WriteCommandSlow(CMD_SET_PLL_MN); // Set PLL with OSC = 10MHz (hardware)
    // Multiplier M = 35, VCO (>250MHz)= OSC*(M+1), VCO = 360MHz
    WriteDataSlow(35);
    WriteDataSlow(2); // Divider N = 2, PLL = 360/(N+1) = 120MHz
    WriteDataSlow(0x54); // Validate M and N values

    WriteCommandSlow(CMD_PLL_START); // Start PLL command
    WriteDataSlow(0x01); // enable PLL

    Delay10us(10); // wait stablize for 100us

    WriteCommandSlow(CMD_PLL_START); // Start PLL command again
    WriteDataSlow(0x03); // now, use PLL output as system clock

    WriteCommandSlow(CMD_SOFT_RESET); // Soft reset. All configuration register reset except 0xE0 to 0xE5
    Delay10us(10); // wait stablize for 100us

#ifdef USE_16BIT_PMP
    PMMODEbits.MODE16 = 1; // 16 bit mode
#else
    PMMODEbits.MODE16 = 0; // 8 bit mode
#endif
    //once PLL locked (at 110MHz), the data hold time set shortest PMP setup
    PMMODEbits.WAITB = 0;
    PMMODEbits.WAITM = 0;
    PMMODEbits.WAITE = 0;

    PMCONbits.WRSP = 0; // Write Strobe Active Low
    PMCONbits.RDSP = 0; // Read Strobe Active Low
    PMCONbits.PTRDEN = 1; // enable RD line
    PMCONbits.PTWREN = 1; // enable WR line

    /*
     ****************************************************************
     * 4.3" TFT panel model # TY430TFT480272
     ****************************************************************
     */
#if defined (USE_TY430TFT480272)	
    //Set LSHIFT freq, i.e. the DCLK with PLL freq 120MHz set previously
    //Typical DCLK for TY430TFT480272 is 9MHz
    //9MHz = 120MHz*(LCDC_FPR+1)/2^20
    //LCDC_FPR = 78642 (0x13332)
    WriteCommand(CMD_SET_PCLK);
    DisplayEnable();
    WriteData(0x01);
    WriteData(0x33);
    WriteData(0x32);
    DisplayDisable();
    /*
     ****************************************************************
     * 5.0" TFT panel model # TY500TFT800480
     ****************************************************************
     */
#elif defined (USE_TY500TFT800480)|| defined (USE_TY700TFT800480_R3)
    //Set LSHIFT freq, i.e. the DCLK with PLL freq 120MHz set previously
    //Typical DCLK for TY500TFT800480 is 33MHz
    //30MHz = 120MHz*(LCDC_FPR+1)/2^20
    //LCDC_FPR = 262143 (0x3FFFF)
    WriteCommand(CMD_SET_PCLK);
    DisplayEnable();
    WriteData(0x03);
    WriteData(0xff);
    WriteData(0xff);
    DisplayDisable();

    /*
     ****************************************************************
     * 7" TFT model # TY700TFT800480
     ****************************************************************
     */
#elif defined (USE_TY700TFT800480)
    //Set LSHIFT freq, i.e. the DCLK with PLL freq 120MHz set previously
    //Typical DCLK for TY700TFT800480 is 33.3MHz(datasheet), experiment shows 30MHz gives a stable result
    //30MHz = 120MHz*(LCDC_FPR+1)/2^20
    //LCDC_FPR = 262143 (0x3FFFF)
    //Time per line = (DISP_HOR_RESOLUTION+DISP_HOR_PULSE_WIDTH+DISP_HOR_BACK_PORCH+DISP_HOR_FRONT_PORCH)/30 us = 1056/30 = 35.2us
    WriteCommand(CMD_SET_PCLK);
    DisplayEnable();
    WriteData(0x03);
    WriteData(0xff);
    WriteData(0xff);
    DisplayDisable();
#endif

    //Set panel mode, varies from individual manufacturer
    WriteCommand(CMD_SET_PANEL_MODE);
#if defined (USE_TY430TFT480272) || defined (USE_TY700TFT800480_R3)
    DisplayEnable();
    WriteData(0x20); // set 24-bit for TY430TF480272 4.3" / TY700TFT800480_R3 panel, data latch in rising edge for LSHIFT
    WriteData(0x00); // set Hsync+Vsync mode
    WriteData((DISP_HOR_RESOLUTION - 1) >> 8); //Set panel size
    WriteData(DISP_HOR_RESOLUTION - 1);
    WriteData((DISP_VER_RESOLUTION - 1) >> 8);
    WriteData(DISP_VER_RESOLUTION - 1);
    WriteData(0x00); //RGB sequence
    DisplayDisable();
#elif defined (USE_TY500TFT800480) 
    DisplayEnable();
    WriteData(0x24); // set 24-bit for TY500TFT800480 5" panel, data latch in falling edge for LSHIFT
    WriteData(0x00); // set Hsync+Vsync mode
    WriteData((DISP_HOR_RESOLUTION - 1) >> 8); //Set panel size
    WriteData(DISP_HOR_RESOLUTION - 1);
    WriteData((DISP_VER_RESOLUTION - 1) >> 8);
    WriteData(DISP_VER_RESOLUTION - 1);
    WriteData(0x00); //RGB sequence
    DisplayDisable();
#elif defined (USE_TY700TFT800480)
    DisplayEnable();
    WriteData(0x10); // set 18-bit for 7" panel TY700TFT800480
    WriteData(0x80); // set TTL mode
    WriteData((DISP_HOR_RESOLUTION - 1) >> 8); //Set panel size
    WriteData(DISP_HOR_RESOLUTION - 1);
    WriteData((DISP_VER_RESOLUTION - 1) >> 8);
    WriteData(DISP_VER_RESOLUTION - 1);
    WriteData(0x00); //RGB sequence
    DisplayDisable();
#endif


    //Set horizontal period
    WriteCommand(CMD_SET_HOR_PERIOD);
#define HT (DISP_HOR_RESOLUTION+DISP_HOR_PULSE_WIDTH+DISP_HOR_BACK_PORCH+DISP_HOR_FRONT_PORCH)
    DisplayEnable();
    WriteData((HT - 1) >> 8);
    WriteData(HT - 1);
#define HPS (DISP_HOR_PULSE_WIDTH+DISP_HOR_BACK_PORCH)
    WriteData((HPS - 1) >> 8);
    WriteData(HPS - 1);
    WriteData(DISP_HOR_PULSE_WIDTH - 1);
    WriteData(0x00);
    WriteData(0x00);
    WriteData(0x00);
    DisplayDisable();
    //Set vertical period
    WriteCommand(CMD_SET_VER_PERIOD);
#define VT (DISP_VER_PULSE_WIDTH+DISP_VER_BACK_PORCH+DISP_VER_FRONT_PORCH+DISP_VER_RESOLUTION)
    DisplayEnable();
    WriteData((VT - 1) >> 8);
    WriteData(VT - 1);
#define VSP (DISP_VER_PULSE_WIDTH+DISP_VER_BACK_PORCH)
    WriteData((VSP - 1) >> 8);
    WriteData(VSP - 1);
    WriteData(DISP_VER_PULSE_WIDTH - 1);
    WriteData(0x00);
    WriteData(0x00);
    DisplayDisable();

    //Set pixel format, i.e. the bpp
    WriteCommand(CMD_SET_PIXEL_FORMAT);
    DisplayEnable();
    WriteData(0x55); // set 16bpp
    DisplayDisable();

    //Set pixel data interface
    WriteCommand(CMD_SET_DATA_INTERFACE);

    DisplayEnable();

#if defined (USE_16BIT_PMP)
    WriteData(0x03); //16-bit(565 format) data for 16bpp PIC32MX only
#elif defined (USE_8BIT_PMP)
    WriteData(0x00); //8-bit data for PIC32MX460 or PIC32MX795 USB/ETH MCU series on Explorer 16
#endif

    DisplayDisable();


#if defined (USE_TY500TFT800480) || defined (USE_TY430TFT480272) || defined (USE_TY700TFT800480_R3)
    WriteCommand(CMD_SET_GPIO_CONF); // Set all GPIOs to output, controlled by host
    DisplayEnable();
    WriteData(0x0f); // Set GPIO0 as output
    WriteData(0x01); // GPIO[3:0] used as normal GPIOs
    DisplayDisable();
    // LL Reset to LCD!!!
    GPIO_WR(LCD_SPENA, 1);
    GPIO_WR(LCD_SPCLK, 1);
    GPIO_WR(LCD_SPDAT, 1);
    GPIO_WR(LCD_RESET, 1);
    GPIO_WR(LCD_RESET, 0);
    DelayMs(1);
    GPIO_WR(LCD_RESET, 1);
#endif
    /*
    #if (DISPLAY_PANEL==LVC75Z779V1S)
            SPI_SetReg(0x01, 0x7300);					// 0x73 gives the best result by trial-an-error
            SPI_SetReg(0x02, 0x0200);
            SPI_SetReg(0x03, 0x6364);
            SPI_SetReg(0x04, 0x0407);					// Set himax for 24-bit parallel TFT mode
            SPI_SetReg(0x05, 0xBCC4);
            SPI_SetReg(0x0A, 0x4008);
            SPI_SetReg(0x0B, 0xD400);
            SPI_SetReg(0x0D, 0x3229);
            SPI_SetReg(0x0E, 0x3200);
            SPI_SetReg(0x0F, 0x0000);
            SPI_SetReg(0x16, 0x9F80);
            SPI_SetReg(0x17, 0x2212);
            SPI_SetReg(0x1E, 0x0052);
            SPI_SetReg(0x30, 0x0000);
            SPI_SetReg(0x31, 0x0407);
            SPI_SetReg(0x32, 0x0202);
            SPI_SetReg(0x33, 0x0000);
            SPI_SetReg(0x34, 0x0505);
            SPI_SetReg(0x35, 0x0003);
            SPI_SetReg(0x36, 0x0707);
            SPI_SetReg(0x37, 0x0000);
            SPI_SetReg(0x3A, 0x0904);
            SPI_SetReg(0x3B, 0x0904);
    #endif
     */

#ifdef USE_DOUBLE_BUFFERING
    // initialize double buffering feature
    blInvalidateAll = 1;
    blDisplayUpdatePending = 0;
    NoOfInvalidatedRectangleAreas = 0;
    _drawbuffer = GFX_BUFFER1;
    SwitchOnDoubleBuffering();
#endif //USE_DOUBLE_BUFFERING


#ifdef GFX_DRV_PAGE_COUNT
    for (i = 0; i < GFX_DRV_PAGE_COUNT; i++) {
        _PageTable[i] = GFX_DISPLAY_BUFFER_START_ADDRESS + (GFX_DISPLAY_BUFFER_LENGTH * i);
    }


    for (i = 0; i < GFX_DRV_PAGE_COUNT; i++) {
        SetActivePage(i);
        SetColor(0);
        ClearDevice();
    }
#endif		//GFX_DRV_PAGE_COUNT

    SetActivePage(0); //Set page1 as  _drawbuffer
    SetVisualPage(0);

    //SetDisplayOrientation(0);
    SetBacklight(255);
    WriteCommand(CMD_ON_DISPLAY); // Turn on display; show the image on display
}

#ifdef USE_TRANSPARENT_COLOR

/*********************************************************************
 * Function:  void TransparentColorEnable(GFX_COLOR color)
 *
 * Overview: Sets current transparent color.
 *
 * PreCondition: none
 *
 * Input: color - Color value chosen.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
void TransparentColorEnable(GFX_COLOR color) {
    _colorTransparent = color;
    _colorTransparentEnable = TRANSPARENT_COLOR_ENABLE;
}
#endif

/*********************************************************************
 * Function: void PutPixel(SHORT x, SHORT y)
 *
 * PreCondition: none
 *
 * Input: x,y - pixel coordinates
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: puts pixel
 *
 * Note:
 ********************************************************************/
void PutPixel(SHORT x, SHORT y) {
    if (_clipRgn) {
        if (x < _clipLeft)
            return;
        if (x > _clipRight)
            return;
        if (y < _clipTop)
            return;
        if (y > _clipBottom)
            return;
    }
#if (DISP_ORIENTATION == 0)
    SetArea(x, y, GetMaxX(), GetMaxY());
#elif (DISP_ORIENTATION == 90)
    SHORT _x = y;
    SHORT _y = GetMaxX() - x;

    SetArea(_x, _y, _x+1, _y+1);
#endif
    WriteCommand(CMD_WR_MEMSTART);
    DisplayEnable();
#if defined (USE_16BIT_PMP)
    WriteData(_color);
#elif defined (USE_8BIT_PMP)
    WriteColor(_color);
#endif
    DisplayDisable();
}

/*********************************************************************
 * Function: WORD GetPixel(SHORT x, SHORT y)
 *
 * PreCondition: none
 *
 * Input: x,y - pixel coordinates
 *
 * Output: pixel color
 *
 * Side Effects: none
 *
 * Overview: returns pixel color at x,y position
 *
 * Note: none
 *
 ********************************************************************/
WORD GetPixel(SHORT x, SHORT y) {
    return 0; // TODO: Implement GetPixel
}

/*********************************************************************
 * Function: WORD Bar(SHORT left, SHORT top, SHORT right, SHORT bottom)
 *
 * PreCondition: none
 *
 * Input: left,top - top left corner coordinates,
 *        right,bottom - bottom right corner coordinates
 *
 * Output: For Blocking configuration:
 *         - Always return 1.
 *
 * Side Effects: none
 *
 * Overview: draws rectangle filled with current color
 *
 * Note: none
 *
 ********************************************************************/
//#ifdef USE_DRV_BAR

WORD Bar(SHORT left, SHORT top, SHORT right, SHORT bottom) {
    register SHORT x, y;

    if (_clipRgn) {
        if (left < _clipLeft)
            left = _clipLeft;
        if (right > _clipRight)
            right = _clipRight;
        if (top < _clipTop)
            top = _clipTop;
        if (bottom > _clipBottom)
            bottom = _clipBottom;
    }

#if (DISP_ORIENTATION==0)
    SetArea(left, top, right, bottom);
#elif (DISP_ORIENTATION==90)
    SHORT _left = top;
    SHORT _right = bottom;
    SHORT _top = GetMaxX()-right;
    SHORT _bottom = GetMaxX()-left;

    SetArea(_left, _top, _right, _bottom);
#endif
    WriteCommand(CMD_WR_MEMSTART);

    DisplayEnable();
    for (y = top; y < bottom + 1; y++) {
        for (x = left; x < right + 1; x++) {
#if defined (USE_16BIT_PMP)
            WriteData(_color);
#elif defined (USE_8BIT_PMP)
            WriteColor(_color);
#endif
        }
    }
    DisplayDisable();
    return (1);
}
//#endif

/*********************************************************************
 * Function: void ClearDevice(void)
 *
 * PreCondition: none
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: clears screen with current color
 *
 * Note: none
 *
 ********************************************************************/
void ClearDevice(void) {
    //DWORD xcounter, ycounter, counter; //depreciated
    DWORD counter;
#if (DISP_ORIENTATION == 0) || (DISP_ORIENTATION == 180)	
    SetArea(0, 0, GetMaxX(), GetMaxY());
#elif (DISP_ORIENTATION == 90)|| (DISP_ORIENTATION == 270)
    SetArea(0, 0, GetMaxY(), GetMaxX());
#endif	

    WriteCommand(CMD_WR_MEMSTART);

    DisplayEnable();
    for (counter = 0; counter < (DWORD)(GetMaxY() + 1)*(DWORD)(GetMaxX() + 1); counter++) {
#if defined (USE_16BIT_PMP)
        WriteData(_color);
#elif defined (USE_8BIT_PMP)
        WriteColor(_color);
#endif
    }
    DisplayDisable();

    /*
            //depreciated
            for(ycounter=0;ycounter<GetMaxY()+1;ycounter++)
            {
                    for(xcounter=0;xcounter<GetMaxX()+1;xcounter++)
                    {

                            #if defined (USE_16BIT_PMP)
                                    WriteData(_color+xcounter+ycounter);
                            #elif defined (USE_8BIT_PMP)
                                    WriteColor(_color);
                            #endif

                    }
            }
     */


}


//#ifdef USE_BITMAP_FLASH
/*********************************************************************
 * Function: void PutImage1BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch)
 *
 * PreCondition: none
 *
 * Input: left,top - left top image corner,
 *        bitmap - image pointer,
 *        stretch - image stretch factor
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: outputs monochrome image starting from left,top coordinates
 *
 * Note: image must be located in flash
 *
 ********************************************************************/
/*
void PutImage1BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch)
{
        register FLASH_BYTE* flashAddress;
        register FLASH_BYTE* tempFlashAddress;
        BYTE temp;
        WORD sizeX, sizeY;
        WORD x,y,y_inc;
        BYTE stretchX,stretchY;
        WORD pallete[2];
        BYTE mask;

        // Move pointer to size information
        flashAddress = bitmap + 2;

        // Read image size
        sizeY = *((FLASH_WORD*)flashAddress);
        flashAddress += 2;
        sizeX = *((FLASH_WORD*)flashAddress);
        flashAddress += 2;
        pallete[0] = *((FLASH_WORD*)flashAddress);
        flashAddress += 2;
        pallete[1] = *((FLASH_WORD*)flashAddress);
        flashAddress += 2;

        y_inc = 0;		// Y-counter, in case stretch > 1
        for(y=0; y<sizeY; y++){
                tempFlashAddress = flashAddress;
                for(stretchY = 0; stretchY<stretch; stretchY++){
                        flashAddress = tempFlashAddress;
                        SetArea(left, top+y_inc, GetMaxX(), GetMaxY());
                        y_inc++;
                        WriteCommand(CMD_WR_MEMSTART);
                        mask = 0;
                        CS_LAT_BIT= 0 ;
                        for(x=0; x<sizeX; x++){
                                // Read 8 pixels from flash
                                if(mask == 0){
                                        temp = *flashAddress;
                                        flashAddress++;
                                        mask = 0x80;
                                }
                
                                // Set color
                                if(mask&temp){
                                        SetColor(pallete[1]);
                                }else{
                                        SetColor(pallete[0]);
                                }
                                // Write pixel to screen
                                for(stretchX=0; stretchX<stretch; stretchX++){
                                        WriteData(_color);
                                }

                                // Shift to the next pixel
                                mask >>= 1;
                        }
                        DisplayDisable();
                }
        }
}
 */
/*********************************************************************
 * Function: void PutImage4BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch)
 *
 * PreCondition: none
 *
 * Input: left,top - left top image corner, bitmap - image pointer,
 *        stretch - image stretch factor
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: outputs 16 color image starting from left,top coordinates
 *
 * Note: image must be located in flash
 *
 ********************************************************************/
/*
void PutImage4BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch)
{
        register FLASH_BYTE* flashAddress;
        register FLASH_BYTE* tempFlashAddress;
        WORD sizeX, sizeY;
        register WORD x,y;
        WORD y_inc;
        BYTE temp;
        register BYTE stretchX,stretchY;
        WORD pallete[16];
        WORD counter;

        // Move pointer to size information
        flashAddress = bitmap + 2;

        // Read image size
        sizeY = *((FLASH_WORD*)flashAddress);
        flashAddress += 2;
        sizeX = *((FLASH_WORD*)flashAddress);
        flashAddress += 2;

        // Read pallete
        for(counter=0;counter<16;counter++){
                pallete[counter] = *((FLASH_WORD*)flashAddress);
                flashAddress += 2;
        }

        y_inc = 0;		// Y-counter, in case stretch > 1
        for(y=0; y<sizeY; y++){
                tempFlashAddress = flashAddress;
                for(stretchY = 0; stretchY<stretch; stretchY++){
                        flashAddress = tempFlashAddress;
                        SetArea(left, top+y_inc, GetMaxX(), GetMaxY());
                        y_inc++;
                        WriteCommand(CMD_WR_MEMSTART);
                        DisplayEnable();
                        for(x=0; x<sizeX; x++){
                                // Read 2 pixels from flash
                                if(x&0x0001){
                                        // second pixel in byte
                                        SetColor(pallete[temp>>4]);
                                }else{
                                        temp = *flashAddress;
                                        flashAddress++;
                                        // first pixel in byte
                                        SetColor(pallete[temp&0x0f]);
                                }

                                // Write pixel to screen
                                for(stretchX=0; stretchX<stretch; stretchX++){
                                        WriteData(_color);
                                }
                        }
                        DisplayDisable();
                }
        }
}
 */
/*********************************************************************
 * Function: void PutImage8BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch)
 *
 * PreCondition: none
 *
 * Input: left,top - left top image corner, bitmap - image pointer,
 *        stretch - image stretch factor
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: outputs 256 color image starting from left,top coordinates
 *
 * Note: image must be located in flash
 *
 ********************************************************************/
/*
void PutImage8BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch)
{
        register FLASH_BYTE* flashAddress;
        register FLASH_BYTE* tempFlashAddress;
        WORD sizeX, sizeY;
        WORD x,y, y_inc;
        BYTE temp;
        BYTE stretchX, stretchY;
        WORD pallete[256];
        WORD counter;

        // Move pointer to size information
        flashAddress = bitmap + 2;

        // Read image size
        sizeY = *((FLASH_WORD*)flashAddress);
        flashAddress += 2;
        sizeX = *((FLASH_WORD*)flashAddress);
        flashAddress += 2;

        // Read pallete
        for(counter=0;counter<256;counter++){
                pallete[counter] = *((FLASH_WORD*)flashAddress);
                flashAddress += 2;
        }
        y_inc = 0;		// Y-counter, in case stretch > 1

        for(y=0; y<sizeY; y++){
                tempFlashAddress = flashAddress;
                for(stretchY = 0; stretchY<stretch; stretchY++){
                        flashAddress = tempFlashAddress;
                        SetArea(left, top+y_inc, GetMaxX(), GetMaxY());
                        y_inc++;
                        WriteCommand(CMD_WR_MEMSTART);
                        DisplayEnable();
                        for(x=0; x<sizeX; x++){
                                // Read pixels from flash
                                temp = *flashAddress;
                                flashAddress++;

                                // Set color
                                SetColor(pallete[temp]);
                                // Write pixel to screen
                                for(stretchX=0; stretchX<stretch; stretchX++){
                                        WriteData(_color);
                                }
                        }
                        DisplayDisable();
                }
        }
}
 */
/*********************************************************************
 * Function: void PutImage16BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch)
 *
 * PreCondition: none
 *
 * Input: left,top - left top image corner, bitmap - image pointer,
 *        stretch - image stretch factor
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: outputs hicolor image starting from left,top coordinates
 *
 * Note: image must be located in flash
 *
 ********************************************************************/
/*
void PutImage16BPP(SHORT left, SHORT top, FLASH_BYTE* bitmap, BYTE stretch)
{
        register FLASH_WORD* flashAddress;
        register FLASH_WORD* tempFlashAddress;
        WORD sizeX, sizeY;
        register WORD x,y;
        WORD y_inc;
        WORD temp;
        register BYTE stretchX,stretchY;

        // Move pointer to size information
        flashAddress = (FLASH_WORD*)bitmap + 1;

        // Read image size
        sizeY = *flashAddress;
        flashAddress++;
        sizeX = *flashAddress;
        flashAddress++;
     
        y_inc = 0;		// Y-counter, in case stretch > 1
        for(y=0; y<sizeY; y++){
                tempFlashAddress = flashAddress;
                for(stretchY = 0; stretchY<stretch; stretchY++){
                        flashAddress = tempFlashAddress;
                        SetArea(left, top+y_inc, GetMaxX(), GetMaxY());
                        y_inc++;
                        WriteCommand(CMD_WR_MEMSTART);
                        DisplayEnable();
                        for(x=0; x<sizeX; x++){
                                // Read pixels from flash
                                temp = *flashAddress;
                                flashAddress++;

                                // Set color
                                SetColor(temp);

                                // Write pixel to screen
                                for(stretchX=0; stretchX<stretch; stretchX++){
                                        WriteData(_color);
                                }
                        }
                        DisplayDisable();
                }
        }
}
 */
//#endif

//#ifdef USE_BITMAP_EXTERNAL
/*********************************************************************
 * Function: void PutImage1BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch)
 *
 * PreCondition: none
 *
 * Input: left,top - left top image corner, bitmap - image pointer,
 *        stretch - image stretch factor
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: outputs monochrome image starting from left,top coordinates
 *
 * Note: image must be located in external memory
 *
 ********************************************************************/
/*
void PutImage1BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch)
{
        register DWORD      memOffset;
        BITMAP_HEADER       bmp;
        WORD                pallete[2];
        BYTE                lineBuffer[((GetMaxX()+1)/8)+1];
        BYTE*               pData;
        SHORT               byteWidth;
        BYTE                temp;
        BYTE                mask;
        WORD                sizeX, sizeY;
        WORD                x,y,y_inc;
        BYTE                stretchX, stretchY;

        // Get bitmap header
        ExternalMemoryCallback(bitmap, 0, sizeof(BITMAP_HEADER), &bmp);

        // Get pallete (2 entries)
        ExternalMemoryCallback(bitmap, sizeof(BITMAP_HEADER), 2*sizeof(WORD), pallete);

        // Set offset to the image data
        memOffset = sizeof(BITMAP_HEADER) + 2*sizeof(WORD);

        // Line width in bytes
        byteWidth = bmp.width>>3;
        if(bmp.width&0x0007)
                byteWidth++;

        // Get size
        sizeX = bmp.width;
        sizeY = bmp.height;

        y_inc = 0;		// Y-counter, in case stretch > 1
        for(y=0; y<sizeY; y++){
                // Get line
                ExternalMemoryCallback(bitmap, memOffset, byteWidth, lineBuffer);
                memOffset += byteWidth;

                for(stretchY = 0; stretchY<stretch; stretchY++){
                        pData = lineBuffer;
                        SetArea(left, top+y_inc, GetMaxX(), GetMaxY());
                        y_inc++;
                        WriteCommand(CMD_WR_MEMSTART);
                        mask = 0;
                        DisplayEnable();
                        for(x=0; x<sizeX; x++){
                                // Read 8 pixels from flash
                                if(mask == 0){
                                        temp = *pData++;
                                        mask = 0x80;
                                }
                
                                // Set color
                                if(mask&temp){
                                        SetColor(pallete[1]);
                                }else{
                                        SetColor(pallete[0]);
                                }

                                // Write pixel to screen
                                for(stretchX=0; stretchX<stretch; stretchX++){
                                        WriteData(_color);
                                }

                                // Shift to the next pixel
                                mask >>= 1;
                        }
                        DisplayDisable();
                }
        }
}
 */
/*********************************************************************
 * Function: void PutImage4BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch)
 *
 * PreCondition: none
 *
 * Input: left,top - left top image corner, bitmap - image pointer,
 *        stretch - image stretch factor
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: outputs monochrome image starting from left,top coordinates
 *
 * Note: image must be located in external memory
 *
 ********************************************************************/
/*
void PutImage4BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch)
{
        register DWORD      memOffset;
        BITMAP_HEADER       bmp;
        WORD                pallete[16];
        BYTE                lineBuffer[((GetMaxX()+1)/2)+1];
        BYTE*               pData;
        SHORT               byteWidth;
        BYTE                temp;
        WORD                sizeX, sizeY;
        WORD                x,y,y_inc;
        BYTE                stretchX, stretchY;

        // Get bitmap header
        ExternalMemoryCallback(bitmap, 0, sizeof(BITMAP_HEADER), &bmp);

        // Get pallete (16 entries)
        ExternalMemoryCallback(bitmap, sizeof(BITMAP_HEADER), 16*sizeof(WORD), pallete);

        // Set offset to the image data
        memOffset = sizeof(BITMAP_HEADER) + 16*sizeof(WORD);

        // Line width in bytes
        byteWidth = bmp.width>>1;
        if(bmp.width&0x0001)
                byteWidth++;

        // Get size
        sizeX = bmp.width;
        sizeY = bmp.height;

        y_inc = 0;		// Y-counter, in case stretch > 1
        for(y=0; y<sizeY; y++){
                // Get line
                ExternalMemoryCallback(bitmap, memOffset, byteWidth, lineBuffer);
                memOffset += byteWidth;

                for(stretchY = 0; stretchY<stretch; stretchY++){
                        pData = lineBuffer;
                        SetArea(left, top+y_inc, GetMaxX(), GetMaxY());
                        y_inc++;
                        WriteCommand(CMD_WR_MEMSTART);
                        DisplayEnable();
                        for(x=0; x<sizeX; x++){
                                // Read 2 pixels from flash
                                if(x&0x0001){
                                        // second pixel in byte
                                        SetColor(pallete[temp>>4]);
                                }else{
                                        temp = *pData++;
                                        // first pixel in byte
                                        SetColor(pallete[temp&0x0f]);
                                }

                                // Write pixel to screen
                                for(stretchX=0; stretchX<stretch; stretchX++){
                                        WriteData(_color);
                                }
                        }
                        DisplayDisable();
                }
        }
}
 */
/*********************************************************************
 * Function: void PutImage8BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch)
 *
 * PreCondition: none
 *
 * Input: left,top - left top image corner, bitmap - image pointer,
 *        stretch - image stretch factor
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: outputs monochrome image starting from left,top coordinates
 *
 * Note: image must be located in external memory
 *
 ********************************************************************/
/*
void PutImage8BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch)
{
        register DWORD      memOffset;
        BITMAP_HEADER       bmp;
        WORD                pallete[256];
        BYTE                lineBuffer[(GetMaxX()+1)];
        BYTE*               pData;
        BYTE                temp;
        WORD                sizeX, sizeY;
        WORD                x,y,y_inc;
        BYTE                stretchX, stretchY;

        // Get bitmap header
        ExternalMemoryCallback(bitmap, 0, sizeof(BITMAP_HEADER), &bmp);

        // Get pallete (256 entries)
        ExternalMemoryCallback(bitmap, sizeof(BITMAP_HEADER), 256*sizeof(WORD), pallete);

        // Set offset to the image data
        memOffset = sizeof(BITMAP_HEADER) + 256*sizeof(WORD);

        // Get size
        sizeX = bmp.width;
        sizeY = bmp.height;

        y_inc = 0;		// Y-counter, in case stretch > 1
        for(y=0; y<sizeY; y++){
                // Get line
                ExternalMemoryCallback(bitmap, memOffset, sizeX, lineBuffer);
                memOffset += sizeX;

                for(stretchY = 0; stretchY<stretch; stretchY++){
                        pData = lineBuffer;
                        SetArea(left, top+y_inc, GetMaxX(), GetMaxY());
                        y_inc++;
                        WriteCommand(CMD_WR_MEMSTART);
                        CS_LAT_BIT  = 0;
                        for(x=0; x<sizeX; x++){
                                temp = *pData++;
                                SetColor(pallete[temp]);

                                // Write pixel to screen
                                for(stretchX=0; stretchX<stretch; stretchX++){
                                        WriteData(_color);
                                }
                        }
                        DisplayDisable();
                }
        }
}

/*********************************************************************
 * Function: void PutImage16BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch)
 *
 * PreCondition: none
 *
 * Input: left,top - left top image corner, bitmap - image pointer,
 *        stretch - image stretch factor
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: outputs monochrome image starting from left,top coordinates
 *
 * Note: image must be located in external memory
 *
 ********************************************************************/

void PutImage16BPPExt(SHORT left, SHORT top, void* bitmap, BYTE stretch)
{
    register DWORD  memOffset;
    BITMAP_HEADER   bmp;
    WORD            lineBuffer[(GetMaxX() + 1)];
    WORD            *pData;
    WORD            byteWidth;

    WORD            temp;
    WORD            sizeX, sizeY;
    WORD            x, y;
    WORD            xc, yc;


    // Get image header
    ExternalMemoryCallback(bitmap, 0, sizeof(BITMAP_HEADER), &bmp);
     
    // Set offset to the image data
    memOffset = sizeof(BITMAP_HEADER);
    
    // Get size

    sizeX = bmp.width/stretch;          //   *stretch/100;
    sizeY = bmp.height/stretch;        //   *stretch/100;
    
    byteWidth = bmp.width << 1;
    
#if (DISP_ORIENTATION==0)
    SetArea(left, top, right, bottom);
#elif (DISP_ORIENTATION==90)
    SetArea(top, GetMaxX() - sizeX - left + 1, top + sizeY - 1, GetMaxX() - left);
#endif    

    WriteCommand(CMD_WR_MEMSTART);
    DisplayEnable(); //already enabled .. can be ignored
///////////      
   
    if (stretch == IMAGE_NORMAL)
    {
        for(y = 0; y < sizeY; y++)
        {
            // Get line
            ExternalMemoryCallback(bitmap, memOffset, byteWidth, lineBuffer);
            memOffset += byteWidth;
            
            pData = lineBuffer;

            for(x = 0; x < sizeX; x++)
                {
                temp = *pData++;
                SetColor(temp);

                #ifdef USE_TRANSPARENT_COLOR
                    if (!((GetTransparentColor() == GetColor()) && (GetTransparentColorStatus() == TRANSPARENT_COLOR_ENABLE)))
                #endif
                        WriteData(_color);
            }          
        }
        DisplayDisable();
    }
    else
    {
         ///stretch
        
        for(y = 0; y < sizeY; y++)
        {
            // Get line
            ExternalMemoryCallback(bitmap, memOffset, byteWidth, lineBuffer);
            memOffset += byteWidth*stretch; 
            pData = lineBuffer;
            
            xc = left;
            for(x = 0; x < sizeX; x++)
                {

                pData+=stretch;
                temp = *pData;
                SetColor(temp);
    
                #ifdef USE_TRANSPARENT_COLOR
                    if (!((GetTransparentColor() == GetColor()) && (GetTransparentColorStatus() == TRANSPARENT_COLOR_ENABLE)))
                #endif
                    {
                        WriteData(_color);

                    }                    
            }
          
        }
        DisplayDisable();
    }    
}

//#endif

//#endif	//#if defined (USE_DISPLAY_CONTROLLER_SSD1963_R3B)
