/**
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

#ifndef EPD_H
#define	EPD_H

#ifdef	__cplusplus
extern "C" {
#endif
    #include "HardwareProfile.h"
    //#include "GenericTypeDefs.h"
    #include "GraphicsConfig.h"
    #include "Pervasive_Displays_small_EPD.h"

/** 
 * Using 2.7" as maximum supported size. 2.7" resolution is 264*176.
 * 264/8=33Bytes per line(total 176 lines), but use 64Bytes as a base for future
 * supporting larger size image.
 * 64*176=11264 Bytes --> 12K Bytes per default image size  */
#define _epd_image_size     (long)4*1024*3  //12k
#define _epd_page_size()    (_epd_image_size/32)    //memory access per page=32 bytes
#define getAddress(page)    (long)(_epd_image_size*page)

void SetEPDImageindex(uint8_t image_size);
void EPD_Global_Update(void);
void EPD_PWD_Init(void);
void EPD_Partial_Update(void);
void read_SRAM_handle(EInt memory_address,uint8_t *target_buffer,
                              uint8_t byte_length);
#ifdef	__cplusplus
}
#endif

#endif	/* EPD_H */

