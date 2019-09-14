/**
 *
 * \brief The initialization and configuration of COG hardware driver
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
#include <math.h>
#include "EPD_hardware_driver.h"
#include "timers_Pic24.h" // timer.h copied from xc16/v1.21/support/peripheral_24F to workaround conflict with MLA Legacy timer.h - VirtualFab

static volatile uint32_t EPD_Counter;
static uint8_t spi_flag = FALSE;


/**
 * \brief Interrupt Service Routine for Timer A0
 */
 void __attribute__ ((interrupt,no_auto_psv)) _T2Interrupt (void)
{
   EPD_Counter++;
   T2_Clear_Intr_Status_Bit;
}

/**
 * \brief Set up EPD Timer for 1 mSec interrupts
 *
 * \note
 * desired value: 1mSec
 * actual value:  1.000mSec
 */
static void initialize_EPD_timer(void) {
    #define TSYS   32000000       //8MHZ PLL 32MHZ
    #define TIMER_INTERVAL  1       //   1ms
    #define TIME_PRESCALER  64
    #define TIMER2_VAL  (UINT)(((TIMER_INTERVAL*(TSYS/1000))/(TIME_PRESCALER*2))-1) //CY=(PR2+1)*Tcy*(timer prescaler)
    OpenTimer2((T2_ON | T2_PS_1_64 | T2_SOURCE_INT | T2_IDLE_CON ),TIMER2_VAL);//624);//20ms  0x9C40); //Timer is configured for 10 msec
    ConfigIntTimer2(T2_INT_ON|T2_INT_PRIOR_1);/*Enable Interrupt*/
    EPD_Counter = 0;
}

/**
 * \brief Start Timer
 */
void start_EPD_timer(void) {
	initialize_EPD_timer();	
	EPD_Counter = 0;
}

/**
 * \brief Stop Timer
 */
void stop_EPD_timer(void) {
	CloseTimer2();
}

/**
 * \brief Get current Timer after starting a new one
 */
uint32_t get_current_time_tick(void) {
	return EPD_Counter;
}

/**
 * \brief Interrupt Service Routine for system tick counter
 */
void SysTick_Handler(void) {
	EPD_Counter++;
}
/**
* \brief Delay mini-seconds
* \param ms The number of mini-seconds
*/
void sys_delay_ms(unsigned int ms) {
	delay_ms(ms);
}


static void Wait_10us(void) {
	delay_us(5);
}

//******************************************************************
//* PWM  Configuration/Control
//******************************************************************

/**
 * \brief The PWM signal starts toggling
 */
void PWM_start_toggle(void) {

}

/**
 * \brief The PWM signal stops toggling.
 */
void PWM_stop_toggle(void) {

}

/**
 * \brief PWM toggling.
 *
 * \param ms The interval of PWM toggling (mini seconds)
 */
void PWM_run(uint16_t ms) {
	start_EPD_timer();
	do {
		EPD_pwm_high();
		delay_us(5);
		EPD_pwm_low();
		delay_us(5);
	} while (get_current_time_tick() < ms); //wait Delay Time
	stop_EPD_timer();
         
}

//******************************************************************
//* SPI  Configuration
//******************************************************************

/**
 * \brief Configure SPI
 */
void epd_spi_init(void) {
    uint32_t SPICON1Value;
    uint32_t SPICON2Value;
    uint32_t SPISTATValue;
    // config_gpio_dir_o(SPICLK_PORT,SPICLK_PIN);
    // config_gpio_dir_o(SPIMOSI_PORT,SPIMOSI_PIN);
    // config_gpio_dir_i(SPIMISO_PORT,SPIMISO_PIN);
	if (spi_flag)
		return;
	spi_flag = TRUE;
        CloseSPI1();    //Disbale SPI1 mdolue if enabled previously
	SPICON1Value =ENABLE_SCK_PIN | ENABLE_SDO_PIN | SPI_MODE8_ON
                | MASTER_ENABLE_ON | SEC_PRESCAL_1_1 | PRI_PRESCAL_1_1 
                | CLK_POL_ACTIVE_HIGH | SPI_CKE_ON | SLAVE_ENABLE_OFF | SPI_SMP_ON ; //16MHZ               
    SPICON2Value = FRAME_ENABLE_OFF | FRAME_SYNC_OUTPUT | FIFO_BUFFER_DISABLE;
    SPISTATValue = SPI_ENABLE | SPI_ENABLE | SPI_IDLE_CON | SPI_RX_OVFLOW_CLR;
    OpenSPI1(SPICON1Value,SPICON2Value,SPISTATValue );
    
}

/**
 * \brief Initialize SPI
 */
void epd_spi_attach(void) {
       
	epd_spi_init();
}

/**
 * \brief Disable SPI and change to GPIO
 */
void epd_spi_detach(void) {
     spi_flag = FALSE;
    CloseSPI1();    //Disbale SPI1 mdolue if enabled previously
    OpenSPI1(0,0,0 );
    config_gpio_dir_o(SPICLK_PORT,SPICLK_PIN);
    config_gpio_dir_o(SPIMOSI_PORT,SPIMOSI_PIN);
    config_gpio_dir_o(SPIMISO_PORT,SPIMISO_PIN);
    set_gpio_low(SPICLK_PORT,SPICLK_PIN);
    set_gpio_low(SPIMOSI_PORT,SPIMOSI_PIN);
    set_gpio_low(SPIMISO_PORT,SPIMISO_PIN);        
}

/**
 * \brief SPI synchronous write
 */
void epd_spi_write(unsigned char Data) {
    WriteSPI1((uint32_t)Data);
    while(SPI1_Tx_Buf_Full);  //wait till completion of transmission
    ReadSPI1();
}
/**
 * \brief SPI synchronous read
 */
uint8_t epd_spi_read(uint8_t data) {
    WriteSPI1(data);
    while(!DataRdySPI1());
    return (uint8_t)ReadSPI1();
}

/**
 * \brief Send data to SPI with time out feature
 *
 * \param data The data to be sent out
 */
uint8_t epd_spi_write_ex(unsigned char Data) {
	//uint8_t cnt = 200;
	uint8_t flag = 1;
	WriteSPI1((uint32_t)Data);
        while(SPI1_Tx_Buf_Full);  //wait till completion of transmission
	return flag;
}

#if (defined COG_V230_G2)
/**
* \brief SPI command
*
* \param register_index The Register Index as SPI Data to COG
* \param register_data The Register Data for sending command data to COG
* \return the SPI read value
*/
uint8_t SPI_R(uint8_t Register, uint8_t Data) {
	uint8_t result;
	EPD_cs_low ();
	epd_spi_write (0x70); // header of Register Index
	epd_spi_write (Register);

	EPD_cs_high ();
	Wait_10us ();
	EPD_cs_low ();

	epd_spi_write (0x73); // header of Register Data of read command
	result=epd_spi_read (Data);

	EPD_cs_high ();

	return result;
}
#endif

/**
* \brief SPI command if register data is larger than two bytes
*
* \param register_index The Register Index as SPI command to COG
* \param register_data The Register Data for sending command data to COG
* \param length The number of bytes of Register Data which depends on which
* Register Index is selected.
*/
void epd_spi_send (unsigned char register_index, unsigned char *register_data,
               unsigned length) {
	EPD_cs_low ();
	epd_spi_write (0x70); // header of Register Index
	epd_spi_write (register_index);

	EPD_cs_high ();
	Wait_10us ();
	EPD_cs_low ();

	epd_spi_write (0x72); // header of Register Data of write command
	while(length--) {
		epd_spi_write (*register_data++);
	}
	EPD_cs_high ();
    Wait_10us ();
}

/**
* \brief SPI command
*
* \param register_index The Register Index as SPI command to COG
* \param register_data The Register Data for sending command data to COG
*/
void epd_spi_send_byte (uint8_t register_index, uint8_t register_data) {
	EPD_cs_low ();
	epd_spi_write (0x70); // header of Register Index
	epd_spi_write (register_index);

	EPD_cs_high ();
	Wait_10us ();
	EPD_cs_low ();
	epd_spi_write (0x72); // header of Register Data
	epd_spi_write (register_data);
	EPD_cs_high ();
    Wait_10us ();
}

//******************************************************************
//* Temperature sensor  Configuration
//******************************************************************
static float IntDegC;
#define Voltag_base     (float)((3.3*100)/1024)
#define Deg_0_voltag    (float)(500/10)    //500mv/10
/**
 * \brief Get temperature value from ADC
 *
 * \return the Celsius temperature
 */
int16_t get_temperature(void) {
    int i;
    unsigned int adcval;
     EnableADC1;
    AD1CON1bits.SAMP = 1;
    for(i = 0;i < 10000;++i);
    ConvertADC10();
     while(BusyADC10());
    adcval= ReadADC10(0);
    IntDegC=(adcval*Voltag_base)-Deg_0_voltag;
    DisableADC1;
    return (int16_t) IntDegC;
}

/**
 * \brief Initialize the temperature sensor
 */
void initialize_temperature(void) {
 unsigned int uiConfigADC_1,uiConfigADC_2,uiConfigADC_3;
 uiConfigADC_1 = ADC_MODULE_OFF | ADC_FORMAT_INTG | ADC_CLK_MANUAL | \
                     ADC_AUTO_SAMPLING_OFF | ADC_SAMP_OFF;
 uiConfigADC_2 = ADC_VREF_AVDD_AVSS | ADC_SCAN_OFF | ADC_ALT_INPUT_OFF;
 uiConfigADC_3 = ADC_CONV_CLK_INTERNAL_RC   | ADC_CONV_CLK_16Tcy;//ADC_CONV_CLK_INTERNAL_RC

// OpenADC10(uiConfigADC_1,uiConfigADC_2,uiConfigADC_3,ENABLE_AN4_ANA, DISABLE_ALL_INPUT_SCAN);
 SetChanADC10(ADC_CH0_POS_SAMPLEA_AN4);


}

/**
 * \brief Initialize the EPD hardware setting
 */
void EPD_display_hardware_init(void) {
	EPD_initialize_gpio();
	EPD_Vcc_turn_off();
	epd_spi_init();
	initialize_temperature();
	EPD_cs_low();
	EPD_pwm_low();
	EPD_rst_low();
	EPD_discharge_low();
	EPD_border_low();
}

