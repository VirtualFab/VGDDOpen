/**
* \brief The SPI, PWM, Temperature definitions of COG hardware driver
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

#ifndef 	DISPLAY_HARDWARE_DRIVCE_H_INCLUDED_
#define 	DISPLAY_HARDWARE_DRIVCE_H_INCLUDED_

#include <Pervasive_Displays_small_EPD.h>

/**SPI Defines ****************************************************************/
#define SPI_baudrate        (SMCLK_FREQ/COG_SPI_baudrate)   /**< the baud rate of SPI */

void epd_spi_init (void);
void epd_spi_attach (void);
void epd_spi_detach (void);
void epd_spi_send (unsigned char Register, unsigned char *Data, unsigned Length);
void epd_spi_send_byte (uint8_t Register, uint8_t Data);
uint8_t epd_spi_read(uint8_t data);
void epd_spi_write (unsigned char Data);
uint8_t epd_spi_write_ex (unsigned char Data);
void sys_delay_ms(unsigned int ms);
void start_EPD_timer(void);
void stop_EPD_timer(void);
uint32_t get_current_time_tick(void);
void PWM_start_toggle(void);
void PWM_stop_toggle(void);
void PWM_run(uint16_t time);
void initialize_temperature(void);
int16_t get_temperature(void);
void EPD_display_hardware_init (void);
#if  (defined COG_V230_G2)
uint8_t SPI_R(uint8_t Register, uint8_t Data);
#endif
#endif 	//DISPLAY_HARDWARE_DRIVCE_H_INCLUDED_
