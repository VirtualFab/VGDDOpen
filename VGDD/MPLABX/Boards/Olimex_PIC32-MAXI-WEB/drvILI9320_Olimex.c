/*****************************************************************************
 *  Module for Microchip Graphics Library
 *  LCD controller driver
 *  Ilitek ILI9320 - Olimex version
 *****************************************************************************
 * FileName:        drvILI9320_Olimex.c
 * Processor:       PIC24, PIC32
 * Compiler:        MPLAB C30, MPLAB C32
 * Company:         Microchip Technology Incorporated
 *                  VirtualFab adaptations
 *
 * Software License Agreement
 *
 * Copyright ? 2008 Microchip Technology Inc.  All rights reserved.
 * Microchip licenses to you the right to use, modify, copy and distribute
 * Software only when embedded on a Microchip microcontroller or digital
 * signal controller, which is integrated into your product or third party
 * product (pursuant to the sublicense terms in the accompanying license
 * agreement).  
 *
 * You should refer to the license agreement accompanying this Software
 * for additional information regarding your rights and obligations.
 *
 * SOFTWARE AND DOCUMENTATION ARE PROVIDED oAS ISo WITHOUT WARRANTY OF ANY
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
 * Date         Comment
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * 11/12/07	    Version 1.0 release
 * 01/30/08	    combined version for landscape and portrait
 * 01/30/08     PIC32 support
 * 06/25/09     dsPIC & PIC24H support 
 * 06/26/09     16-bit PMP support
 * 03/11/11     - Modified dependencies
 *              - changes for Graphics Library Version 3.00
 * 07/02/12     Modified PutImageXBPPYYY() functions to use new API.
 * 10/13/12     drvTFT001.c Adaptations for Olimex ILI9320 - PIC32-MAXI-WEB by VirtualFab
 *****************************************************************************/
//#include "Graphics/Graphics.h"

#include "HardwareProfile.h"
#include "Compiler.h"
#include "TimeDelay.h"
#include "Graphics/DisplayDriver.h"
#include "drvILI9320_Olimex.h"
#include "Graphics/Primitive.h"

#ifdef USE_GFX_PMP
#include "Graphics/gfxpmp.h"
#elif USE_GFX_EPMP
#include "Graphics/gfxepmp.h"
#endif    
#define USE_PRIMITIVE_PUTIMAGE

// Clipping region control
SHORT _clipRgn;

// Clipping region borders
SHORT _clipLeft;
SHORT _clipTop;
SHORT _clipRight;
SHORT _clipBottom;

// Color
GFX_COLOR _color;
#ifdef USE_TRANSPARENT_COLOR
GFX_COLOR _colorTransparent;
SHORT _colorTransparentEnable;
#endif

/*********************************************************************
 * Macro:  WritePixel(color)
 *
 * PreCondition: none
 *
 * Input: color
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: writes pixel at the current address
 *
 * Note: chip select should be enabled
 *
 ********************************************************************/
#ifdef USE_16BIT_PMP
#define WritePixel(color)	DeviceWrite(color)
#else
#define WritePixel(color)	{ DeviceWrite(((WORD_VAL)color).v[1]); DeviceWrite(((WORD_VAL)color).v[0]);}
#endif

/*********************************************************************
 * Macros:  SetAddress(addr)
 *
 * Overview: Writes address pointer.
 *
 * PreCondition: none
 *
 * Input: add0 -  32-bit address.
 *
 * Output: none
 *
 * Side Effects: none
 *
 ********************************************************************/
#ifdef USE_16BIT_PMP
#define SetAddress(addr)                    \
	{                                       \
	DisplaySetCommand();                    \
    DeviceWrite(0x0020);                    \
	DisplaySetData();                       \
    DeviceWrite((WORD) addr & 0x00ff);      \
	DisplaySetCommand();                    \
    DeviceWrite(0x0021);                    \
	DisplaySetData();                       \
    DeviceWrite((WORD) ((DWORD) addr >> 8));\
	DisplaySetCommand();                    \
    DeviceWrite(0x0022);                    \
	DisplaySetData();                       \
	}
#else
#define SetAddress(addr)                    \
	{\
	DisplaySetCommand();                    \
    DeviceWrite(0);                         \
    DeviceWrite(0x20);                      \
	DisplaySetData();                       \
	DeviceWrite(0);                         \
    DeviceWrite(((DWORD_VAL) (DWORD) addr).v[0]);\
	DisplaySetCommand();                    \
    DeviceWrite(0);                         \
    DeviceWrite(0x21);                      \
	DisplaySetData();                       \
	DeviceWrite(((DWORD_VAL) (DWORD) addr).v[2]);\
	DeviceWrite(((DWORD_VAL) (DWORD) addr).v[1]);\
	DisplaySetCommand();                    \
    DeviceWrite(0);                         \
    DeviceWrite(0x22);                      \
	DisplaySetData();                       \
	}
#endif

/*********************************************************************
 * Function:  void  SetReg(WORD index, WORD value)
 *
 * PreCondition: none
 *
 * Input: index - register number
 *        value - value to be set
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: sets graphics controller register
 *
 * Note: none
 *
 ********************************************************************/
void SetReg(WORD index, WORD value) {
#ifdef USE_16BIT_PMP
    DisplayEnable();
    DisplaySetCommand();
    DeviceWrite(index);
    DisplaySetData();
    DeviceWrite(value);
    DisplayDisable();
#else
    DisplayEnable();
    DisplaySetCommand();
    DeviceWrite(((WORD_VAL) index).v[1]);
    DeviceWrite(((WORD_VAL) index).v[0]);
    DisplaySetData();
    DeviceWrite(((WORD_VAL) value).v[1]);
    DeviceWrite(((WORD_VAL) value).v[0]);
    DisplayDisable();
#endif
}

// SPP +

/*********************************************************************
 * Function:  WORD GetReg(WORD index)
 *
 * PreCondition: none
 *
 * Input: index - register number
 *
 * Output: register value
 *
 * Side Effects: none
 *
 * Overview: sets graphics controller register
 *
 * Note: none
 *
 ********************************************************************/
unsigned int GetReg(WORD index) {
    unsigned int value;
    while (PMMODEbits.BUSY);
    DisplayEnable();
    DisplaySetCommand();
    DeviceWrite(index);
    DisplaySetData();

    while (mIsPMPBusy());
    value = PMDIN;
    while (mIsPMPBusy());
    value = PMDIN;
    while (mIsPMPBusy());
    value = PMDIN;

    DisplayDisable();
    return value;
}
// SPP -

/*********************************************************************
 * Function:  void DisplayBrightness(WORD level)
 *
 * PreCondition: none
 *
 * Input: level - Brightness level. Valid values are 0 to 100.
 *			- 0: brightness level is zero or display is turned off
 *			- 1: brightness level is maximum
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: Sets the brightness of the display.
 *
 * Note: none
 *
 ********************************************************************/
void DisplayBrightness(WORD level) {
    // If the brightness can be controlled (for example through PWM)
    // add code that will control the PWM here.

    if (level > 0) {
        DisplayBacklightOn();
    } else if (level == 0) {
        DisplayBacklightOff();
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
 * Overview: resets LCD, initializes PMP
 *
 * Note: none
 *
 ********************************************************************/
void ResetDevice(void) {
    // Set FLASH CS pin as output
    DisplayFlashConfig();

    // Initialize the device
    DriverInterfaceInit();

    DelayMs(5);

    // Power on LCD
    DisplayPowerOn();
    DisplayPowerConfig();

    DelayMs(2);

#if defined (GFX_USE_DISPLAY_CONTROLLER_ILI9320)

    // SPP +

    int DISPLAY_CODE;
    DISPLAY_CODE = GetReg(0);

    // SPP -

    SetReg(0x0000, 0x0001); //start Int. osc
    SetReg(0x0001, 0x0100); //Set SS bit (shift direction of outputs is from S720 to S1)
    DelayMs(15);

    SetReg(0x0002, 0x0700); //select  the line inversion
#if (DISP_ORIENTATION == 0)
    SetReg(0x0003, 0x1030); //Entry mode(Horizontal : increment,Vertical : increment, AM=0)
#elif  (DISP_ORIENTATION == 90)
    SetReg(0x0003, 0x1038); //Entry mode(Horizontal : increment,Vertical : increment, AM=1)
#elif  (DISP_ORIENTATION == 180)
    SetReg(0x0003, 0x1020); //Entry mode(Horizontal : increment,Vertical : increment, AM=1)
#elif  (DISP_ORIENTATION == 270)
    SetReg(0x0003, 0x1018); //Entry mode(Horizontal : increment,Vertical : increment, AM=1)
#endif
    SetReg(0x0004, 0x0000); //Resize control(No resizing)
    SetReg(0x0008, 0x0202); //front and back porch 2 lines
    SetReg(0x0009, 0x0000); //select normal scan
    SetReg(0x000A, 0x0000); //display control 4
    SetReg(0x000C, 0x0000); //system interface(2 transfer /pixel), internal sys clock,
    SetReg(0x000D, 0x0000); //Frame marker position
    SetReg(0x000F, 0x0000); //selects clk, enable and sync signal polarity,
    SetReg(0x0010, 0x0000); //
    SetReg(0x0011, 0x0000); //power control 2 reference voltages = 1:1,
    SetReg(0x0012, 0x0000); //power control 3 VRH
    SetReg(0x0013, 0x0000); //power control 4 VCOM amplitude
    DelayMs(20);
    SetReg(0x0010, 0x17B0); //power control 1 BT,AP
    SetReg(0x0011, 0x0137); //power control 2 DC,VC
    DelayMs(50);
    SetReg(0x0012, 0x0139); //power control 3 VRH
    DelayMs(50);
    SetReg(0x0013, 0x1d00); //power control 4 vcom amplitude
    SetReg(0x0029, 0x0011); //power control 7 VCOMH
    DelayMs(50);
    SetReg(0x0030, 0x0007);
    SetReg(0x0031, 0x0403);
    SetReg(0x0032, 0x0404);
    SetReg(0x0035, 0x0002);
    SetReg(0x0036, 0x0707);
    SetReg(0x0037, 0x0606);
    SetReg(0x0038, 0x0106);
    SetReg(0x0039, 0x0007);
    SetReg(0x003c, 0x0700);
    SetReg(0x003d, 0x0707);
    SetReg(0x0020, 0x0000); //starting Horizontal GRAM Address
    SetReg(0x0021, 0x0000); //starting Vertical GRAM Address
    SetReg(0x0050, 0x0000); //Horizontal GRAM Start Position
    SetReg(0x0051, 0x00EF); //Horizontal GRAM end Position
    SetReg(0x0052, 0x0000); //Vertical GRAM Start Position
    SetReg(0x0053, 0x013F); //Vertical GRAM end Position

    // SPP +

    switch (DISPLAY_CODE) {
        case 0x9320:
            SetReg(0x0060, 0x2700); // starts scanning from G1, and 320 drive lines
            break;
        case 0x9325:
            SetReg(0x0060, 0xA700); // starts scanning from G1, and 320 drive lines
            SetReg(0x002B, 0x000E); // increase the refresh rate of the LCD
            break;
    }

    // SPP -

    SetReg(0x0061, 0x0001); //fixed base display
    SetReg(0x006a, 0x0000); //no scroll
    SetReg(0x0090, 0x0010); //set Clocks/Line =16, Internal Operation Clock Frequency=fosc/1,
    SetReg(0x0092, 0x0000); //set gate output non-overlap period=0
    SetReg(0x0093, 0x0003); //set Source Output Position=3
    SetReg(0x0095, 0x0110); //RGB interface(Clocks per line period=16 clocks)
    SetReg(0x0097, 0x0110); //set Gate Non-overlap Period 0 locksc
    SetReg(0x0098, 0x0110); //
    SetReg(0x0007, 0x0173); //display On
#else
#error Graphics controller is not supported.
#endif
    DelayMs(20);
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
 * Macro: mCalcAddressXY(x, y)
 *
 * PreCondition: none
 *
 * Input: x,y - pixel coordinates
 *
 * Output: Address in display memory
 *
 * Side Effects: none
 *
 * Overview: Calculates Address based on DISP_ORIENTATION
 *
 * Note: none
 *
 ********************************************************************/
#if (DISP_ORIENTATION == 0)
#define mCalcAddressXY(x, y) ((DWORD) LINE_MEM_PITCH * y + x)
#elif (DISP_ORIENTATION == 90)
#define mCalcAddressXY(x, y) ((DWORD) LINE_MEM_PITCH * x + (GetMaxY() - y))
#elif (DISP_ORIENTATION == 180)
#define mCalcAddressXY(x, y) ((DWORD) LINE_MEM_PITCH * (GetMaxY() - y) + (GetMaxX()-x))
#elif(DISP_ORIENTATION == 270)
#define mCalcAddressXY(x, y) ((DWORD) LINE_MEM_PITCH * (GetMaxX() - x) + y)
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
 * Note: none
 *
 ********************************************************************/
void PutPixel(SHORT x, SHORT y) {
    DWORD address;
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

    address = mCalcAddressXY(x, y);

    DisplayEnable();
    SetAddress(address);
    WritePixel(_color);
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
#ifndef __PIC24FJ256GB210__
#ifdef USE_16BIT_PMP

/* */
WORD GetPixel(SHORT x, SHORT y) {
    DWORD address;
    WORD result;
    address = mCalcAddressXY(x, y);

    DisplayEnable();

    SetAddress(address);

    // Temporary change wait cycles for reading (250ns = 4 cycles)
#if defined(__C30__)
    PMMODEbits.WAITM = 4;
#elif defined(__PIC32MX__)
    PMMODEbits.WAITM = 8;
#else
#error Need wait states for the device
#endif
    DisplaySetData();

    // First RD cycle to move data from GRAM to Read Data Latch
    result = PMDIN1;

    while (PMMODEbits.BUSY);

    // Second RD cycle to get data from Read Data Latch
    result = PMDIN1;

    while (PMMODEbits.BUSY);

    // Disable LCD
    DisplayDisable();

    // Disable PMP
    PMCONbits.PMPEN = 1;

    // Read result
    result = PMDIN1;

    // Restore wait cycles for writing (60ns)
#if defined(__dsPIC33F__) || defined(__PIC24H__)
    PMMODEbits.WAITM = 2;
#else
    PMMODEbits.WAITM = 1;
#endif

    // Enable PMP
    PMCONbits.PMPEN = 1;

    return (result);
}

#else

/* */
WORD GetPixel(SHORT x, SHORT y) {
    DWORD address;
    WORD_VAL result;

    address = mCalcAddressXY(x, y);

    DisplayEnable();

    SetAddress(address);

    // Temporary change wait cycles for reading (250ns = 4 cycles)
#if defined(__C30__)
    PMMODEbits.WAITM = 4;
#elif defined(__PIC32MX__)
    PMMODEbits.WAITM = 8;
#else
#error Need wait states for the device
#endif
    DisplaySetData();

    // First RD cycle to move data from GRAM to Read Data Latch
    result.v[1] = PMDIN1;

    while (PMMODEbits.BUSY);

#if defined(GFX_USE_DISPLAY_CONTROLLER_ILI9320)
    DelayForSync();
#endif

    // Second RD cycle to move data from GRAM to Read Data Latch
    result.v[1] = PMDIN1;

    while (PMMODEbits.BUSY);
#if defined (GFX_USE_DISPLAY_CONTROLLER_ILI9320)
    DelayForSync();
#endif

    // First RD cycle to get data from Read Data Latch
    // Read previous dummy value
    result.v[1] = PMDIN1;

    while (PMMODEbits.BUSY);
#if defined (GFX_USE_DISPLAY_CONTROLLER_ILI9320)
    DelayForSync();
#endif

    // Second RD cycle to get data from Read Data Latch
    // Read MSB
    result.v[1] = PMDIN1;

    while (PMMODEbits.BUSY);
#if defined (GFX_USE_DISPLAY_CONTROLLER_ILI9320)
    DelayForSync();
#endif

    // Disable LCD
    DisplayDisable();

    // Disable PMP
    PMCONbits.PMPEN = 1;

    // Read LSB
    result.v[0] = PMDIN1;
#if defined (GFX_USE_DISPLAY_CONTROLLER_ILI9320)
    DelayForSync();
#endif

    // Restore wait cycles for writing (60ns)
#if defined(__dsPIC33F__) || defined(__PIC24H__)
    PMMODEbits.WAITM = 2;
#else
    PMMODEbits.WAITM = 1;
#endif

    // Enable PMP
    PMCONbits.PMPEN = 1;

    return (result.Val);
}

#endif
#endif

/*********************************************************************
 * Function: WORD Bar(SHORT left, SHORT top, SHORT right, SHORT bottom)
 *
 * PreCondition: none
 *
 * Input: left,top - top left corner coordinates,
 *        right,bottom - bottom right corner coordinates
 *
 * Output: For NON-Blocking configuration:
 *         - Returns 0 when device is busy and the shape is not yet completely drawn.
 *         - Returns 1 when the shape is completely drawn.
 *         For Blocking configuration:
 *         - Always return 1.
 *
 * Side Effects: none
 *
 * Overview: draws rectangle filled with current color
 *
 * Note: none
 *
 ********************************************************************/
WORD Bar(SHORT left, SHORT top, SHORT right, SHORT bottom) {
    DWORD address;
    register SHORT x, y;

#ifndef USE_NONBLOCKING_CONFIG
    while (IsDeviceBusy() != 0);

    /* Ready */
#else
    if (IsDeviceBusy() != 0)
        return (0);
#endif
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
    address = mCalcAddressXY(left, top);
    DisplayEnable();
    for (y = top; y < bottom + 1; y++) {
        SetAddress(address);
        for (x = left; x < right + 1; x++) {
            WritePixel(_color);
        }
#if (DISP_ORIENTATION == 0)
        address += LINE_MEM_PITCH;
#elif  (DISP_ORIENTATION == 90)
        address -= 1;
#elif  (DISP_ORIENTATION == 180)
        address -= LINE_MEM_PITCH;
#elif  (DISP_ORIENTATION == 270)
        address += 1;
#endif
    }
    DisplayDisable();

    return (1);
}

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
    DWORD counter;

    DisplayEnable();
    SetAddress(0);
    for (counter = 0; counter < (DWORD) (GetMaxX() + 1) * (GetMaxY() + 1); counter++) {
        WritePixel(_color);
    }

    DisplayDisable();
}

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
 * Side Effects: none
 *
 ********************************************************************/
WORD IsDeviceBusy(void) {
    return (0);
}
