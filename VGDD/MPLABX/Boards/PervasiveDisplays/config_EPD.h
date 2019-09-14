/**
* \file
*
* \brief The EPD configurations
*
* Copyright (c) 2013-2014 Pervasive Displays Inc. All rights reserved.
*
*  Authors: Pervasive Displays Inc.
*
*  Redistribution and use in source and binary forms, with or without
*  modification, are permitted provided that the following conditions
*  are met:
*
*  1. Redistributions of source code must retain the above copyright
*     notice, this list of conditions and the following disclaimer.
*  2. Redistributions in binary form must reproduce the above copyright
*     notice, this list of conditions and the following disclaimer in
*     the documentation and/or other materials provided with the
*     distribution.
*
*  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
*  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
*  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
*  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
*  OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
*  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
*  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
*  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
*  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
*  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
*  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#ifndef CONFIG_EPD_H_INCLUDED
#define CONFIG_EPD_H_INCLUDED

/** Using which EPD size with COG version for demonstration.
 * Please select the correct EPD size and COG version
 * that connects with EPD PICtail Plus Daughter Board from the pull-down menu
 * of MPLAB X IDE.*/

/** The SPI frequency of this kit (12MHz) */
#define COG_SPI_baudrate 12000000

/**
 * Define the resolution (Horizontal * Vertical) of EPD.  */
#if (USE_EPD_Type==EPD_144)
//#define DISP_HOR_RESOLUTION     128
//#define DISP_VER_RESOLUTION     96
#elif  (USE_EPD_Type==EPD_270)
//#define DISP_HOR_RESOLUTION     264
//#define DISP_VER_RESOLUTION     176
#elif  (USE_EPD_Type==EPD_200)
//#define DISP_HOR_RESOLUTION     200
//#define DISP_VER_RESOLUTION     96
#else
    #error "The EPD size is not supported."
#endif

#define GFX_DRIVER_FRAME_X_SIZE         ((DISP_HOR_RESOLUTION + 7) / 8)
#define GFX_DRIVER_FRAME_Y_SIZE         (DISP_VER_RESOLUTION)

//*****************************************************************************
//*****************************************************************************
// Constants and Enumerations
//*****************************************************************************
//*****************************************************************************

/** ID's of controls on the generic menu screen */
#define ID_Logo                     200
#define ID_Microchip_Logo           201
#define ID_HOME_TITLE1              202
#define ID_HOME_TITLE2              203
#define ID_BUTTON1                  204
#define ID_BUTTON2                  205
#define ID_BUTTON3                  206
#define ID_BUTTON4                  207
#define ID_BUTTON5                  208
#define ID_Barchart_BG              209
#define ID_AC_BG                    210
#define ID_AC_Slide                 211
#define ID_AC_Set_Temp              212
#define ID_AC_Room_Temp             213
#define ID_AC_Down                  214
#define ID_AC_Up                    215
#define ID_AC_RPM                   216
#define ID_AC_Energy                217
#define ID_AC_Last                  218
#define ID_AC_Avg                   219
#define ID_AC_Est                   220
#define ID_GLA_ECG_BG               221
#define ID_ECG_PR                   222
#define ID_ECG_QT                   223
#define ID_ECG_QRS                  224
#define ID_ECG_RR                   225
#define ID_ECG_Image                226
#define ID_ECG_TEMP                 227
#define ID_ECG_Peak                 228
#define ID_ECG_Yst                  229
#define ID_ECG_SYS                  230
#define ID_ECG_DIA                  231
#define ID_ECG_PUL                  232
#define ID_ECG_Heart                233
#define ID_PDI_Logo                 234


/** States of the demo application's state machine. */
typedef enum
{
    EPD_Init,
    EPD_DEMO1,
    EPD_DEMO2,
    EPD_DEMO3,
    EPD_DEMO4,
    EPD_DEMO5,
    EPD_Power_End,
    CHANGE_DEMO_MODE,
    EPD_DEMO_NONE
} DEMO_STATES;

#endif /* CONF_EPD_H_INCLUDED */
