/**
*
* \brief The definition of Pervasive Displays Inc.'s EPDs
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

#ifndef EPAPER_H_INCLUDED
#define EPAPER_H_INCLUDED

#define USE_AND_OR /* To enable AND_OR mask setting */
#include <stdint.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <p24Fxxxx.h>
#include <libpic30.h>
#include "spi.h"
#include <adc.h>
#include "ports.h"
#include "outcompare.h"
//#include <timer.h>
#include "HardwareProfile.h" 

typedef  unsigned int EInt;
/** 
 * \brief Developer needs to create an external function if wants to read memory */
typedef void (*EPD_read_memory_handler)(EInt memory_address,uint8_t *target_buffer,
		uint8_t byte_length);

#define FOSC       32000000LL  // clock-frequecy in Hz with suffix LL (64-bit-long), eg. 32000000LL for 32MHz
#define FCY        (FOSC/2)  // MCU is running at FCY MIPS
#define delay_us(x) __delay32(((x*FCY)/1000000L)) // delays x us
#define delay_ms(x) __delay32(((x*FCY)/1000L))  // delays x ms
    
#if !defined(FALSE)
#define FALSE 0 /**< define FALSE=0 */
#endif

#if !defined(TRUE)
#define TRUE (1) /**< define TRUE=1 */
#endif

#if !defined(NULL)
#define NULL (void *)0  /**< define NULL */
#endif

#if !defined(_NOP)
#define _NOP() asm("nop")
#endif

#if !defined(bool)
#define bool uint8_t
#endif


//extern void delay_ms(unsigned int ms);

#define LINE_SIZE	64  /**< maximum data line size */

/**
 * \brief Support 1.44", 2" and 2.7" three type EPD */
#define EPD_144     0
#define EPD_200     1
#define EPD_270     2

#include "config_EPD.h"
#include "EPD_hardware_gpio.h"
#include "EPD_hardware_driver.h"
#include "EPD_COG_process.h"
#include "EPD_controller.h"

#endif	//EPAPER_H_INCLUDED



