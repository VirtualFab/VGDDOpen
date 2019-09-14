/**
* \brief The definition of EPD GPIO pins
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

#include "Pervasive_Displays_small_EPD.h"

#ifndef DISPLAY_HARDWARE_GPIO_H_INCLUDED
#define DISPLAY_HARDWARE_GPIO_H_INCLUDED

#define	_BV(bit)   (1 << (bit)) /**< left shift 1 bit */
#define	_HIGH      1            /**< signal high */
#define	_LOW       !_HIGH       /**< signal low */


#define BITSET(x,y) 		    ((x) |= (y))
#define BITCLR(x,y) 		    ((x) &= ~(y))
#define BITINV(x,y) 		    ((x) ^= (y))

#define	mPORTOutputConfig(Port,Pin)  m ## Port ## OutputConfig(Pin)/**< set output direction for an IOPORT pin */
#define	config_gpio_dir_o(Port,Pin)  mPORTOutputConfig(Port,Pin)/**< set output direction for an IOPORT pin */

#define	mPORTInputConfig(Port,Pin)   m ## Port ## InputConfig(Pin)  /**< set input direction for an IOPORT pin */
#define	config_gpio_dir_i(Port,Pin)  mPORTInputConfig(Port,Pin)  /**< set input direction for an IOPORT pin */

#define	mPORTSetBits(Port,Pin)       m ## Port ## SetBits(Pin) /**< set HIGH for an IOPORT pin */
#define	set_gpio_high(Port,Pin)      mPORTSetBits(Port,Pin) /**< set HIGH for an IOPORT pin */

#define	mPORTClearBits(Port,Pin)     m ## Port ## ClearBits(Pin)  /**< set LOW for an IOPORT pin */
#define	set_gpio_low(Port,Pin)       mPORTClearBits(Port,Pin)  /**< set LOW for an IOPORT pin */

#define	mPORTToggleBits(Port,Pin)    m ## Port ## ToggleBits(Pin) /**< toggle the value of an IOPORT pin */
#define	set_gpio_invert(Port,Pin)    mPORTToggleBits(Port,Pin) /**< toggle the value of an IOPORT pin */

#define	mPORTReadBit(Port,Pin)       m ## Port ## ReadBit(Pin)   /**< get current value of an IOPORT pin */
#define	input_get(Port,Pin)          mPORTReadBit(Port,Pin)   /**< get current value of an IOPORT pin */

/******************************************************************************
* GPIO Defines
*****************************************************************************/
#define Temper_PIN              IOPORT_BIT_0
#define Temper_PORT             PORTB       /**< RB0/AN0 (11) */
#define SPICLK_PIN              IOPORT_BIT_6
#define SPICLK_PORT             PORTF       /**< RF6 (3) */
#define SPIMISO_PIN             IOPORT_BIT_7
#define SPIMISO_PORT            PORTF       /**< RF7 (5) */
#define SPIMOSI_PIN             IOPORT_BIT_8
#define SPIMOSI_PORT            PORTF       /**< RF8 (7)  */

#define EPD_BUSY_PIN            IOPORT_BIT_15
#define EPD_BUSY_PORT           PORTD       /**< RD15 (20) */
#define PWM_PIN                 IOPORT_BIT_0
#define PWM_PORT                PORTD       /**< RD0/OC1 (93) */
#define EPD_RST_PIN             IOPORT_BIT_8
#define EPD_RST_PORT            PORTE       /**< RE8 (18) */
#define EPD_PANELON_PIN         IOPORT_BIT_9
#define EPD_PANELON_PORT        PORTE       /**< RE9 (17)  */
#define EPD_DISCHARGE_PIN       IOPORT_BIT_14
#define EPD_DISCHARGE_PORT      PORTD       /**< RD14 (19)  */
#define EPD_BORDER_PIN          IOPORT_BIT_14
#define EPD_BORDER_PORT         PORTA       /**< RA14 (50)  */

#define Flash_CS_PIN            IOPORT_BIT_9
#define Flash_CS_PORT           PORTG       /**< RG9 (49)  */
#define EEROM_CS_PIN            IOPORT_BIT_9
#define EEROM_CS_PORT           PORTG      /**<  RG9 (49)  */
#define EPD_CS_PIN              IOPORT_BIT_2
#define EPD_CS_PORT             PORTB       /**< RB2 (1)  */

bool EPD_IsBusy(void);
void EPD_cs_high (void);
void EPD_cs_low (void);
void EPD_flash_cs_high(void);
void EPD_flash_cs_low (void);
void EPD_rst_high (void);
void EPD_rst_low (void);
void EPD_discharge_high (void);
void EPD_discharge_low (void);
void EPD_Vcc_turn_off (void);
void EPD_Vcc_turn_on (void);
void EPD_border_high(void);
void EPD_border_low (void);
void EPD_pwm_low (void);
void EPD_pwm_high(void);
void SPIMISO_low(void);
void SPIMOSI_low(void);
void SPICLK_low(void);
void EPD_initialize_gpio(void);

#endif	//DISPLAY_HARDWARE_GPIO_H_INCLUDED


