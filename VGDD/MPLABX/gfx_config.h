/*******************************************************************************
 Module for Microchip Graphics Library

  Company:
    Microchip Technology Inc.

  File Name:
    gfx_config.h

  Summary:
    This header file defines the Graphics Library configurations
    that are enabled. 
*******************************************************************************/

// DOM-IGNORE-BEGIN
/*******************************************************************************
Copyright (c) 2013 released Microchip Technology Inc.  All rights reserved.

Microchip licenses to you the right to use, modify, copy and distribute
Software only when embedded on a Microchip microcontroller or digital signal
controller that is integrated into your product or third party product
(pursuant to the sublicense terms in the accompanying license agreement).

You should refer to the license agreement accompanying this Software for
additional information regarding your rights and obligations.

SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY OF
MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
IN NO EVENT SHALL MICROCHIP OR ITS LICENSORS BE LIABLE OR OBLIGATED UNDER
CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION, BREACH OF WARRANTY, OR
OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT DAMAGES OR EXPENSES
INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL, INDIRECT, PUNITIVE OR
CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, COST OF PROCUREMENT OF
SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY CLAIMS BY THIRD PARTIES
(INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF), OR OTHER SIMILAR COSTS.
*******************************************************************************/
// DOM-IGNORE-END

#ifndef _GRAPHICS_CONFIG_H
    #define _GRAPHICS_CONFIG_H

//////////////////// COMPILE OPTIONS ////////////////////

/*********************************************************************
  Overview: Blocking and Non-Blocking configuration selection.
            Non-Blocking is enabled by default. To disable the
            non-blocking feature, this macro must be enabled.

  GFX_CONFIG_NONBLOCKING_DISABLE
********************************************************************/
//#define GFX_CONFIG_NONBLOCKING_DISABLE

/*********************************************************************
  Overview: To disable support for Alpha Blending.
            Alpha blending is feature available to certain fill
            functions.

  GFX_CONFIG_ALPHABLEND_DISABLE
*********************************************************************/
#define GFX_CONFIG_ALPHABLEND_DISABLE

/*********************************************************************
  Overview: Filling of shapes can be optionally set to fill with
            gradient colors. This feature is by default enabled.
            To disable this feature to reduce code size, define
            this macro at build time.

  GFX_CONFIG_GRADIENT_DISABLE
*********************************************************************/
//#define GFX_CONFIG_GRADIENT_DISABLE

/*********************************************************************
  Overview: Using Palettes, different colors can be used with the same
            bit depth.

  GFX_CONFIG_PALETTE_DISABLE
********************************************************************/
#define GFX_CONFIG_PALETTE_DISABLE

/*********************************************************************
  Overview: Palettes can also be specified to reside in external memory
            similar to fonts and images. Use this when the palette is
            located in external memory.

  GFX_CONFIG_PALETTE_EXTERNAL_DISABLE
********************************************************************/
#define GFX_CONFIG_PALETTE_EXTERNAL_DISABLE

/*********************************************************************
  Overview: Keyboard control on some objects is enabled by default.
            To disable the feature this macro must be defined to
            disable GOL Focus support.

  GFX_CONFIG_FOCUS_DISABLE
*********************************************************************/
//#define GFX_CONFIG_FOCUS_DISABLE

/*********************************************************************
  Overview: To enable support for unicode fonts, GFX_CONFIG_FONT_CHAR_SIZE
            must be defined as 16 (size is 16 bits). For standard ascii
            fonts, this can be defined as 8. This changes the
            GFX_XCHAR definition. See GFX_XCHAR for details.

  GFX_CONFIG_FONT_CHAR_SIZE
*********************************************************************/
//#define GFX_CONFIG_FONT_CHAR_SIZE 16

/*********************************************************************
  Overview: Font data can be placed in multiple locations. They can be
            placed in FLASH memory, RAM and external memory.
            To reduce code size, any one of these locations, when not
            used, can be disabled by defining the macros at build time.
            - GFX_CONFIG_FONT_FLASH_DISABLE - Disable font in internal
                                              flash memory support.
            - GFX_CONFIG_FONT_EXTERNAL_DISABLE - Disable font in
                                              external memory support.
            - GFX_CONFIG_FONT_RAM_DISABLE - Disable font in RAM
                                            support.
*********************************************************************/
//#define GFX_CONFIG_FONT_FLASH_DISABLE  
//#define GFX_CONFIG_FONT_EXTERNAL_DISABLE
#define GFX_CONFIG_FONT_RAM_DISABLE      

/*********************************************************************
  Overview: Images can be placed in multiple locations. They can be
            placed in FLASH memory, RAM and external memory.
            To reduce code size, any one of these locations, when not
            used, can be disabled by defining the macros at build time.
            - GFX_CONFIG_IMAGE_FLASH_DISABLE - Disable images in internal
                                               flash memory support.
            - GFX_CONFIG_IMAGE_EXTERNAL_DISABLE - Disable images in
                                                  external memory support.
            - GFX_CONFIG_IMAGE_RAM_DISABLE - Disable images in RAM
                                             support.
*********************************************************************/
//#define GFX_CONFIG_IMAGE_FLASH_DISABLE 
//#define GFX_CONFIG_IMAGE_EXTERNAL_DISABLE
#define GFX_CONFIG_IMAGE_RAM_DISABLE 

/*********************************************************************
  Overview: Images with 8 and 4 BPP color depths can be optionally
            compressed with RLE. This feature is by default enabled.
            To disable this feature to reduce code size if none of the
            images are compressed with RLE, define this macro at
            build time.

  GFX_CONFIG_RLE_DECODE_DISABLE
*********************************************************************/
//#define GFX_CONFIG_RLE_DECODE_DISABLE

/*********************************************************************
  Overview: When using PIC24FJ256DA210, IPU compression can be
            enabled for images.This feature is by default enabled.
            To disable this feature to reduce code size if none of the
            images are compressed with IPU, define this macro at
            build time.

  GFX_CONFIG_IPU_DECODE_DISABLE
*********************************************************************/
#define GFX_CONFIG_IPU_DECODE_DISABLE

/*********************************************************************
  Overview: Specifies the color depth used in the demo.

  GFX_CONFIG_COLOR_DEPTH
*********************************************************************/
#define GFX_CONFIG_COLOR_DEPTH 16

/*********************************************************************
  Overview: Anti-aliased fonts is a feature that allows characters
            to be rendered with anti-aliasing. The fonts must be
            created by the "Graphics Resource Converter" tool with
            anti-aliasing enabled. To disable this feature this macro
            must be defined at build time.

  GFX_CONFIG_FONT_ANTIALIASED_DISABLE
*********************************************************************/
#define GFX_CONFIG_FONT_ANTIALIASED_DISABLE 

/*********************************************************************
  Overview: Transparent color is a feature in GFX_ImageDraw() where
            pixels that equals the color set in the transparent 
            color variable will not be rendered. This is useful in
            rendering rounded icons or images to the screen with
            complex background.

            To disable this feature the macro must be declared.

  GFX_CONFIG_TRANSPARENT_COLOR_DISABLE
*********************************************************************/
//#define GFX_CONFIG_TRANSPARENT_COLOR_DISABLE 

/*********************************************************************
  Overview: To disable support for double buffering. Use this feature only
            if the display driver used can support double buffering.

  GFX_CONFIG_DOUBLE_BUFFERING_DISABLE
*********************************************************************/
#define GFX_CONFIG_DOUBLE_BUFFERING_DISABLE

/*********************************************************************
* Overview: Specifies the size of the buffer used for externally
*           sourced fonts.
*
* GFX_EXTERNAL_FONT_RASTER_BUFFER_SIZE
*********************************************************************/
#define GFX_EXTERNAL_FONT_RASTER_BUFFER_SIZE 51

/*********************************************************************
  Overview: When using bi-state displays, allows for widget auto-update
  NOTE: The primitive drawing routines will need to be manually updated

  GFX_CONFIG_BISTABLE_DISPLAY_AUTO_REFRESH_ENABLE
*********************************************************************/

/*********************************************************************
  Overview: When this macro is enabled, bitmap images used has will
            disable padding. Define this in system_config.h.

            When converting images for use in the Graphics Library, the
            Graphics Resource Converter has an option to set the images
            to be padded or not padded. When bitmaps are padded, this means
            that each horizontal line will start on a byte boundary. Unpadded
            bitmaps allows the least resource space for a bitmap. Unpadded
            bitmaps also allows support for display controllers with windowing
            and auto-increment features.
 
  GFX_CONFIG_IMAGE_PADDING_DISABLE
*********************************************************************/

/*********************************************************************
  Overview: For XC16 or C30 builds only: When placing fonts in internal
            data memory, there is a 32K limit for data space. The total
            data should not exceed 32K. When this is unavoidable, the macro
            GFX_CONFIG_FONT_PROG_SPACE_ENABLE will relocate the font in
            program space. This will remove the 32K restriction but at
            the expense of slower access.
            Define this in system_config.h to enable the font to be
            placed in program space.

  GFX_CONFIG_FONT_PROG_SPACE_ENABLE
*********************************************************************/

/*********************************************************************
* Overview: Define the malloc() and free() for versatility on OS
*           based systems.
*
*********************************************************************/
#define GFX_malloc(size)    malloc(size)
#define GFX_free(pObj)      free(pObj)

/*********************************************************************
* Overview: Input devices used defines the messages that Objects will
*           process. The following definitions indicate the usage of
*           the different input device:
*           - GFX_CONFIG_USE_TOUCHSCREEN_DISABLE - disables the touch screen support.
*           - GFX_CONFIG_USE_KEYBOARD_DISABLE - disables the key board support.
*
*********************************************************************/
//#define GFX_CONFIG_USE_TOUCHSCREEN_DISABLE    // Enable touch screen support.
//#define GFX_CONFIG_USE_KEYBOARD_DISABLE       // Enable key board support.


#endif // _GRAPHICS_CONFIG_H
