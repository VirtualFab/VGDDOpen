/*****************************************************************************
 *  Module for Microchip Graphics Library
 *  LCD controller driver
 *  Renesas R61509V
 *****************************************************************************
 * FileName:        drvTFTR61509V.h
 * Dependencies:    p24Fxxxx.h or plib.h
 * Processor:       PIC24, PIC32
 * Compiler:        MPLAB C30, MPLAB C32
 * Linker:          MPLAB LINK30, MPLAB LINK32
 * Company:         VirtualFab
 *
 * Author               Date        Comment
 * VirtualFab           2010/12/31  Version 1.0 release
 * VirtualFab           2011/06/09  Isolated from old drv...001 and
 *                                  Window address implementation
 * VirtualFab           2011/07/15  Implementation of TRANSPARENT_COLOR
 * VirtualFab           2013/02/10  Integration for VGDD MplabX Wizard
 *****************************************************************************/
#ifndef _R61509V_H
    #define _R61509V_H

#include "Compiler.h"
//
//    #if defined(__dsPIC33F__)
//        #include <p33Fxxxx.h>
//    #elif defined(__PIC24H__)
//        #include <p24Hxxxx.h>
//    #elif defined(__PIC32MX__)
//        #include <plib.h>
//        #define PMDIN1  PMDIN
//    #elif defined(__PIC24F__)
//        #include <p24Fxxxx.h>
//    //else
//        //error CONTROLLER IS NOT SUPPORTED
//    #endif
    #include "GraphicsConfig.h"
    #include "GenericTypeDefs.h"
    #include "gfxcolors.h"

/*********************************************************************
* Overview: Additional hardware-accelerated functions can be implemented
*           in the driver. These definitions exclude the PutPixel()-based
*           functions in the primitives layer (Primitive.c file) from compilation.
*********************************************************************/

// Define this to implement Font related functions in the driver.
//#define USE_DRV_FONT
// Define this to implement Line function in the driver.
//#define USE_DRV_LINE
// Define this to implement Circle function in the driver.
//#define USE_DRV_CIRCLE
// Define this to implement FillCircle function in the driver.
//#define USE_DRV_FILLCIRCLE
// Define this to implement Bar function in the driver.
    #define USE_DRV_BAR

// Define this to implement ClearDevice function in the driver.
    #define USE_DRV_CLEARDEVICE

// Define this to implement PutImage function in the driver.
    #define USE_DRV_PUTIMAGE

    #ifndef DISP_HOR_RESOLUTION
        //error DISP_HOR_RESOLUTION must be defined in HardwareProfile.h
    #endif
    #ifndef DISP_VER_RESOLUTION
        //error DISP_VER_RESOLUTION must be defined in HardwareProfile.h
    #endif
    #ifndef COLOR_DEPTH
        //error COLOR_DEPTH must be defined in HardwareProfile.h
    #endif
    #ifndef DISP_ORIENTATION
        //error DISP_ORIENTATION must be defined in HardwareProfile.h
    #endif

/*********************************************************************
* Overview: Horizontal and vertical screen size.
*********************************************************************/
    //if (DISP_HOR_RESOLUTION != 240)
        //error This driver doesn't supports this resolution. Horisontal resolution must be 240 pixels.
    //endif
    //if (DISP_VER_RESOLUTION != 320)
        //error This driver doesn't supports this resolution. Vertical resolution must be 320 pixels.
    //endif

/*********************************************************************
* Overview: Display orientation.
*********************************************************************/
    #if (DISP_ORIENTATION != 0) && (DISP_ORIENTATION != 90) && (DISP_ORIENTATION != 180) && (DISP_ORIENTATION != 270)
        #error "This driver doesn't support this orientation."
    #endif

/*********************************************************************
* Overview: Color depth.
*********************************************************************/
    //if (COLOR_DEPTH != 16)
        //error This driver doesn't support this color depth. It should be 16.
    //endif

/*********************************************************************
* Overview: Clipping region control codes to be used with SetClip(...)
*           function.
*********************************************************************/
#define CLIP_DISABLE                0   // Disables clipping.
#define CLIP_ENABLE                 1   // Enables clipping.

#ifdef USE_TRANSPARENT_COLOR
#define TRANSPARENT_COLOR_ENABLE    1   // Check pixel if color is equal to transparent color, if equal do not render pixel
#define TRANSPARENT_COLOR_DISABLE   0   // Check of transparent color is not performed
#endif

extern GFX_COLOR            _color;
extern GFX_COLOR            _chrcolor;

#ifdef USE_TRANSPARENT_COLOR
extern GFX_COLOR            _colorTransparent;
extern SHORT                _colorTransparentEnable;
#endif

/*********************************************************************
* Overview: Clipping region control and border settings.
*
*********************************************************************/
// Clipping region enable control
extern SHORT _clipRgn;
// Left clipping region border
extern SHORT _clipLeft;
// Top clipping region border
extern SHORT _clipTop;
// Right clipping region border
extern SHORT _clipRight;
// Bottom clipping region border
extern SHORT _clipBottom;


#ifdef GFX_DRV_PAGE_COUNT
/*********************************************************************
* Overview: Page table that summarizes the addresses of each available
*           page. The number of pages is dictated by the GFX_DRV_PAGE_COUNT
*           value defined in the hardware profile.
*
*********************************************************************/
extern volatile DWORD _PageTable[GFX_DRV_PAGE_COUNT];
#endif

// Memory pitch for line
    #define LINE_MEM_PITCH  0x100

/*********************************************************************
* Function:  void ResetDevice()
*
* Overview: Initializes LCD module.
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
void    ResetDevice(void);

void DispEnableWindow(SHORT Left, SHORT Top, SHORT Right, SHORT Bottom);
void DispDisableWindow(void);

/*********************************************************************
* Macros:  GetMaxX()
*
* Overview: Returns maximum horizontal coordinate.
*
* PreCondition: none
*
* Input: none
*
* Output: Maximum horizontal coordinate.
*
* Example:
*   <CODE> 
*
*	// Create a window that will occupy the whole screen.
*	WndCreate(0xFF,				    	// ID
*             0,0,
*			  GetMaxX(),GetMaxY(),		// dimension
*             WND_DRAW,					// will be dislayed after creation
*             (void*)&mchpIcon,         // use icon used
*             pText,	   				// set to text pointed to by pText
*             NULL);					// use default scheme 
*
*  </CODE>
*
* Side Effects: none
*
********************************************************************/
    #if (DISP_ORIENTATION == 90) || (DISP_ORIENTATION == 270)
        #define GetMaxX()   (DISP_VER_RESOLUTION - 1)
    #elif (DISP_ORIENTATION == 0) || (DISP_ORIENTATION == 180)
        #define GetMaxX()   (DISP_HOR_RESOLUTION - 1)
    #endif

/*********************************************************************
* Macros:  GetMaxY()
*
* Overview: Returns maximum vertical coordinate.
*
* PreCondition: none
*
* Input: none
*
* Output: Maximum vertical coordinate.
*
* Example: (see GetMaxX()) example.
*
* Side Effects: none
*
********************************************************************/
    #if (DISP_ORIENTATION == 90) || (DISP_ORIENTATION == 270)
        #define GetMaxY()   (DISP_HOR_RESOLUTION - 1)
    #elif (DISP_ORIENTATION == 0) || (DISP_ORIENTATION == 180)
        #define GetMaxY()   (DISP_VER_RESOLUTION - 1)
    #endif

/*********************************************************************
* Macros:  SetColor(color)
*
* Overview: Sets current drawing color.
*
* PreCondition: none
*
* Input: color - Color coded in 5:6:5 RGB format.
*
* Output: none
*
* Side Effects: none
*
********************************************************************/
//    #define SetColor(color) _color = color;

/*********************************************************************
* Macros:  GetColor()
*
* Overview: Returns current drawing color.
*
* PreCondition: none
*
* Input: none
*
* Output: Color coded in 5:6:5 RGB format.
*
* Side Effects: none
*
********************************************************************/
    #define GetColor()  _color

#ifdef USE_TRANSPARENT_COLOR
/*********************************************************************
* Function:  void TransparentColorEnable(GRFX_COLOR color)
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
void TransparentColorEnable(GFX_COLOR color);

/*********************************************************************
* Macros:  TransparentColorDisable()
*
* Overview: Disables the transparent color function.
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
#define TransparentColorDisable() _colorTransparentEnable = TRANSPARENT_COLOR_DISABLE;

/*********************************************************************
* Macros:  GetTransparentColorStatus()
*
* Overview: Returns the current transparent color function status.
*
* PreCondition: none
*
* Input: none
*
* Output: Returns the current transparent color function status
*	<CODE>
*          0 � Transparent color function is disabled.
*          1 � Transparent color function is enabled.
*	</CODE>
*
* Side Effects: PutImage() will not render pixels with the transparent
*               color.
*
********************************************************************/
#define GetTransparentColorStatus() _colorTransparentEnable

/*********************************************************************
* Macros:  GetTransparentColor()
*
* Overview: Returns the current transparent color value.
*
* PreCondition: none
*
* Input: none
*
* Output: Returns the current transparent color used.
*
* Side Effects: none
*
********************************************************************/
#define GetTransparentColor() _colorTransparent

#endif //USE_TRANSPARENT_COLOR

/*********************************************************************
* Macros:  SetActivePage(page)
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
    #define SetActivePage(page)

/*********************************************************************
* Macros: SetVisualPage(page)
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
    #define SetVisualPage(page)

/*********************************************************************
* Function: void PutPixel(SHORT x, SHORT y)
*
* Overview: Puts pixel with the given x,y coordinate position.
*
* PreCondition: none
*
* Input: x - x position of the pixel.
*		 y - y position of the pixel.
*
* Output: none
*
* Side Effects: none
*
********************************************************************/
void    PutPixel(SHORT x, SHORT y);

/*********************************************************************
* Function: WORD GetPixel(SHORT x, SHORT y)
*
* Overview: Returns pixel color at the given x,y coordinate position.
*
* PreCondition: none
*
* Input: x - x position of the pixel.
*		 y - y position of the pixel.
*
* Output: pixel color
*
* Side Effects: none
*
********************************************************************/
WORD    GetPixel(SHORT x, SHORT y);

/*********************************************************************
* Macros: SetClipRgn(left, top, right, bottom)
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
void SetClipRgn(SHORT left, SHORT top, SHORT right, SHORT bottom);

/*********************************************************************
* Macros: GetClipLeft()
*
* Overview: Returns left clipping border.
*
* PreCondition: none
*
* Input: none
*
* Output: Left clipping border.
*
* Side Effects: none
*
********************************************************************/
    #define GetClipLeft()   _clipLeft

/*********************************************************************
* Macros: GetClipRight()
*
* Overview: Returns right clipping border.
*
* PreCondition: none
*
* Input: none
*
* Output: Right clipping border.
*
* Side Effects: none
*
********************************************************************/
    #define GetClipRight()  _clipRight

/*********************************************************************
* Macros: GetClipTop()
*
* Overview: Returns top clipping border.
*
* PreCondition: none
*
* Input: none
*
* Output: Top clipping border.
*
* Side Effects: none
*
********************************************************************/
    #define GetClipTop()    _clipTop

/*********************************************************************
* Macros: GetClipBottom()
*
* Overview: Returns bottom clipping border.
*
* PreCondition: none
*
* Input: none
*
* Output: Bottom clipping border.
*
* Side Effects: none
*
********************************************************************/
    #define GetClipBottom() _clipBottom

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
void SetClip(BYTE control);

/*********************************************************************
* Macros: IsDeviceBusy()
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
WORD IsDeviceBusy();
//    #define IsDeviceBusy()  0

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

#endif // _DRVTFT001_H
