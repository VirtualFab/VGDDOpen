/**
*
* \brief The EPD driver for Microchip Graphic Library
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

#include "HardwareProfile.h"

#ifdef GFX_USE_DISPLAY_CONTROLLER_EPD
#include <string.h>
#include "Compiler.h"
#include "TimeDelay.h"
#include "Graphics/DisplayDriver.h"
#include "PDI e-paper display_driver.h"
#include "Graphics/gfxtcon.h"
#include "Graphics/Primitive.h"
#include "SpiRAM.h"

// Clipping region control
SHORT       _clipRgn;

// Clipping region borders
SHORT       _clipLeft;
SHORT       _clipTop;
SHORT       _clipRight;
SHORT       _clipBottom;

// Color
GFX_COLOR   _color;
//long image_address=0;
long cur_image_index=0;
long previous_image_address,new_image_address;


/**
 * Reset the data and image address of SRAM for EPD
 */
void ResetDevice(void)
{
    uint32_t i;
    uint8_t ImgData[32];
    //Sets 32 bytes of the block of memory pointed by ImgData to 0xFF
     for(i=0;i<32;i++)
    {
        ImgData[i]=0xFF;
    }
    //Write 0xFF to SRAM
    for(i=0;i<1024;i++){
        SRAMWritePage((i*32),ImgData);
    }
  
    cur_image_index=0;
    new_image_address=getAddress(cur_image_index);
    previous_image_address=getAddress((cur_image_index+1));
}



//static uint32_t bkaddress=0xffffff;
static uint8_t bkdata=0xff;

/**
 * Plots pixel at location *x,y)
 * @param x pixel coordinate
 * @param y pixel coordinate
 */
void PutPixel(SHORT x, SHORT y)
{
    uint32_t address;
    uint8_t PixelBit,sdata;

    PixelBit=(7-(x%8));
    address=(uint32_t)(y*(DISP_HOR_RESOLUTION/8))+(x/8);
    address+=new_image_address;//address page offset
        bkdata=SRAMReadByte(address);
      if(_color == BLACK)  {
         sdata=(bkdata & (~_BV(PixelBit)));
      }else{
         sdata=(bkdata | _BV(PixelBit));
      }

   if(sdata!=bkdata)
   {
       SRAMWriteByte(address,sdata);
       bkdata=sdata;
   }
}

/**
 * Store the last updated image to Previous image
 */
void StoreScreen(void){

    uint32_t i;
    uint8_t ImgData[32];
    // SpiRAM_Init();
    for(i=0;i<_epd_page_size();i++){
        SRAMReadPage((i*32),ImgData);
        SRAMWritePage((i*32)+previous_image_address,ImgData);
    }
}


/**
 * Return pixel color at x,y position
 * @param x pixel coordinate
 * @param y pixel coordinate
 * @return pixel color
 */
BYTE GetPixel(SHORT x, SHORT y)
{
   // BYTE    page, add, lAddr, hAddr;
    //BYTE    mask, temp, display;

    // check if point is in clipping region
    if(_clipRgn)
    {
        if(x < _clipLeft)
            return (0);
        if(x > _clipRight)
            return (0);
        if(y < _clipTop)
            return (0);
        if(y > _clipBottom)
            return (0);
    }

    return 1;        // mask all other bits and return the result
}

/**
 * Return Busy status
 * @return always not busy for this case
 */
WORD IsDeviceBusy(void)
{  
    return (0);
}

/**
 * Clear screen with current color
 */
void ClearDevice(void)
{
    uint32_t i;
    uint8_t tmp=0xFF;
    uint8_t ImgData[32];
   // SpiRAM_Init();
    if(GetColor()==BLACK) tmp=0x00;
    for(i=0;i<32;i++)
    {
        ImgData[i]=tmp;
    }
    for(i=0;i<_epd_page_size();i++){
        SRAMWritePage((i*32),ImgData);
    }   
}


/**
 * Read image data from SRAM
 * @param memory_address start address of memory to read
 * @param target_buffer the buffer of read data
 * @param byte_length the total length to read
 */
void read_SRAM_handle(EInt memory_address,uint8_t *target_buffer,
                              uint8_t byte_length) {
    SRAMReadSeq(memory_address,target_buffer,byte_length);
}

/**
 * EPD global update function
 */
void EPD_Global_Update(void){
    EPD_display_global(USE_EPD_Type,previous_image_address,new_image_address,read_SRAM_handle);
    StoreScreen();
}

/**
 * EPD power on and initialization
 */
void EPD_PWD_Init(void){
    EPD_power_init(USE_EPD_Type);
}

/**
 * EPD partial update function
 */
void EPD_Partial_Update(void){
    EPD_display_partial(USE_EPD_Type,previous_image_address,new_image_address,read_SRAM_handle);
    StoreScreen();
}
#endif // #if defined(USE_GFX_DISPLAY_CONTROLLER_SH1101A) || defined (USE_GFX_DISPLAY_CONTROLLER_SSD1303)

