/**
* \file
*
* \brief The waveform driving processes and updating stages of G1 COG with V110 EPD
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

#include "EPD_COG_process.h"
#ifdef COG_V110_G1
#define partial_offset_time     200 //The stage time for partial update
/**
 * \brief The COG parameters of different EPD size
 */
const struct COG_parameters_t COG_parameters[COUNT_OF_EPD_TYPE] = {
	{
		// FOR 1.44"
		{0x00,0x00,0x00,0x00,0x00,0x0F,0xFF,0x00},
		0x03,
		(128/8),
		96,
		((((128+96)*2)/8)+1),
		0,
		480
	},
	{
		// For 2.0"
		{0x00,0x00,0x00,0x00,0x01,0xFF,0xE0,0x00},
		0x03,
		(200/8),
		96,
		((((200+96)*2)/8)+1),
		0,
		480
	},
	{
		// For 2.7"
		{0x00,0x00,0x00,0x7F,0xFF,0xFE,0x00,0x00},
		0x00,
		(264/8),
		176,
		((((264+176)*2)/8)+1),
		0,
		630
	}
};

/* Temperature factor combines with stage time for each driving stage */
const uint16_t temperature_table[3][8] = {
	{(480*17),(480*12),(480*8),(480*4),(480*3),(480*2),(480*1),(480*0.7)},
	{(480*17),(480*12),(480*8),(480*4),(480*3),(480*2),(480*1),(480*0.7)},
	{(630*17),(630*12),(630*8),(630*4),(630*3),(630*2),(630*1),(630*0.7)},
};

const uint8_t   SCAN_TABLE[4] = {0xC0,0x30,0x0C,0x03};
static uint16_t stage_time;
static COG_line_data_packet_type COG_Line;
static EPD_read_memory_handler _On_EPD_read_handle=NULL;
static uint16_t current_frame_time;
static uint8_t  *data_line_even;
static uint8_t  *data_line_odd;
static uint8_t  *data_line_scan;
static uint8_t  use_EPD_type_index;

static inline void nothing_frame (void) ;
/**
* \brief According to EPD size and temperature to get stage_time
* \note Refer to COG document Section 5.3 for more details
*
* \param EPD_type_index The defined EPD size
*/
static void set_temperature_factor(uint8_t EPD_type_index) {
	int8_t temperature;
	//temperature = get_temperature();	
	temperature = 20; // Assume temperature as 20°
	if (temperature <= -10) {
		stage_time = temperature_table[EPD_type_index][0];
		} else if (-5 >= temperature && temperature > -10) {
		stage_time = temperature_table[EPD_type_index][1];
		} else if (5 >= temperature && temperature > -5) {
		stage_time = temperature_table[EPD_type_index][2];
		} else if (10 >= temperature && temperature > 5) {
		stage_time = temperature_table[EPD_type_index][3];
		} else if (15 >= temperature && temperature > 10) {
		stage_time = temperature_table[EPD_type_index][4];
		} else if (20 >= temperature && temperature > 15) {
		stage_time = temperature_table[EPD_type_index][5];
		} else if (40 >= temperature && temperature > 20) {
		stage_time = temperature_table[EPD_type_index][6];
	} else stage_time = temperature_table[EPD_type_index][7];
}

/**
* \brief Initialize the EPD hardware setting
*/
void EPD_init(void) {
	EPD_display_hardware_init();
	EPD_cs_low();
	EPD_pwm_low();
	EPD_rst_low();
	EPD_discharge_low();
	EPD_border_low();
}


/**
* \brief Select the EPD size to get line data array for driving COG
*
* \param EPD_type_index The defined EPD size
*/
void COG_driver_EPDtype_select(uint8_t EPD_type_index) {
	switch(EPD_type_index) {
		case EPD_144:
		data_line_even = &COG_Line.line_data_by_size.line_data_for_144.even[0];
		data_line_odd  = &COG_Line.line_data_by_size.line_data_for_144.odd[0];
		data_line_scan = &COG_Line.line_data_by_size.line_data_for_144.scan[0];
		break;
		case EPD_200:
		data_line_even = &COG_Line.line_data_by_size.line_data_for_200.even[0];
		data_line_odd  = &COG_Line.line_data_by_size.line_data_for_200.odd[0];
		data_line_scan = &COG_Line.line_data_by_size.line_data_for_200.scan[0];
		break;
		case EPD_270:
		data_line_even = &COG_Line.line_data_by_size.line_data_for_270.even[0];
		data_line_odd  = &COG_Line.line_data_by_size.line_data_for_270.odd[0];
		data_line_scan = &COG_Line.line_data_by_size.line_data_for_270.scan[0];
		break;
	}
}


/**
* \brief Power on COG Driver
* \note For detailed flow and description, please refer to the COG document Section 3.
*/
void EPD_power_on (void) {
	/* Initial state */
	EPD_discharge_low();
	EPD_rst_low();
	EPD_cs_low();
	epd_spi_init();
	epd_spi_attach();

	PWM_run(5); //The PWM signal starts toggling
	EPD_Vcc_turn_on(); //Vcc and Vdd >= 2.7V
	delay_ms(10);
	EPD_cs_high(); // /CS=1
	EPD_border_high(); //BORDER=1
	EPD_rst_high(); // /RESET=1
	delay_ms(5);
	EPD_rst_low(); // /RESET=0
	delay_ms(5);
	EPD_rst_high(); // /RESET=1
	PWM_run(5);
}


/**
* \brief Initialize COG Driver
* \note For detailed flow and description, please refer to the COG document Section 4.
*
* \param EPD_type_index The defined EPD size
*/
uint8_t EPD_initialize_driver (uint8_t EPD_type_index) {
	uint8_t SendBuffer[2];
	uint16_t k;
    use_EPD_type_index=EPD_type_index;
	// Empty the Line buffer
	for (k = 0; k <= LINE_BUFFER_DATA_SIZE; k ++) {
		COG_Line.uint8[k] = 0x00;
	}
	// Determine the EPD size for driving COG
	COG_driver_EPDtype_select(use_EPD_type_index);

	// Sense temperature to determine Temperature Factor
	set_temperature_factor(use_EPD_type_index);
	k = 0;
	while (EPD_IsBusy()) {
		if((k++) >= 0x0FFF) return ERROR_BUSY;
	}
	// Channel select
	epd_spi_send (0x01, (uint8_t *)&COG_parameters[use_EPD_type_index].channel_select, 8);

	// DC/DC frequency setting
	epd_spi_send_byte (0x06, 0xFF);

	// High power mode OSC setting
	epd_spi_send_byte (0x07, 0x9D);

	// Disable ADC
	epd_spi_send_byte (0x08, 0x00);

	// Set Vcom level
	SendBuffer[0] = 0xD0;
	SendBuffer[1] = 0x00;
	epd_spi_send (0x09, SendBuffer, 2);

	// Gate and source voltage level
	epd_spi_send_byte (0x04,COG_parameters[use_EPD_type_index].voltage_level);
	delay_ms(5);

	// Driver latch on (cancel register noise)
	epd_spi_send_byte(0x03, 0x01);

	// Driver latch off
	epd_spi_send_byte(0x03, 0x00);

	// Start charge pump positive V VGH & VDH on
	epd_spi_send_byte (0x05, 0x01);
	PWM_run(30);

	// Start charge pump neg voltage VGL & VDL on
	epd_spi_send_byte (0x05, 0x03);
	delay_ms (30);

	// Set charge pump Vcom_Driver to ON
	epd_spi_send_byte(0x05, 0x0F);
	delay_ms(30);

	// Output enable to disable
	epd_spi_send_byte(0x02, 0x24);

	return RES_OK;
}

/**
 * Write Nothing frame
 */
inline void epd_display_line_dummy_handle(void){
    uint8_t i;
    for (i = 0; i < COG_parameters[use_EPD_type_index].horizontal_size; i++){
        data_line_odd[i] =NOTHING;
        data_line_even[i]=NOTHING;
    }
}


inline uint8_t Rotatebyte(uint8_t b){
    uint8_t tmp=b & 0xaa;
     if(b & 0x40) tmp|=0x01;
     if(b & 0x10) tmp|=0x04;
     if(b & 0x04) tmp|=0x10;
     if(b & 0x01) tmp|=0x40;
    return tmp;
}

static uint8_t cnt;


/**
 * \brief The driving stages for getting Odd/Even data per line for partial update
 *
 * \note
 * - The partial update uses one stage to update EPD.
 * - If the new data byte is same as previous data byte, send “Nothing” data byte
 *   which means the data byte on EPD won’t be changed.
 * - If the new data byte is different from the previous data byte, send the new
 *   data byte.
 *
 * @param x0 the beginning position of a line
 * @param x1 the end position of a line
 * @param previous_line_array The pointer of line array that stores previous image
 * @param new_line_array The pointer of line array that stores new image
 */
inline void epd_line_data_partial_handle( uint16_t x0,uint16_t x1,uint8_t *previous_line_array,uint8_t *new_line_array){
    uint16_t i,j;
    uint8_t draw_byte;
    j = COG_parameters[use_EPD_type_index].horizontal_size-1;
    x0 >>= 3;
    x1 = (x1 + 7) >> 3;
    for (i = 0; i < COG_parameters[use_EPD_type_index].horizontal_size; i++){  
     // if(x0<=i && i<x1){
            new_line_array[i]=Rotatebyte(new_line_array[i]);
            previous_line_array[i]=Rotatebyte(previous_line_array[i]);      
            draw_byte=new_line_array[i] ^ previous_line_array[i];
            data_line_odd[i]     = (draw_byte & 0xaa) | (((previous_line_array[i] >>1) & 0x55) & ((draw_byte & 0xaa) >>1)) ;
            data_line_even[j--]  = ((draw_byte <<1) & 0xaa) | ((previous_line_array[i] & 0x55) & (((draw_byte <<1) & 0xaa) >>1));
      /*} else {
            data_line_odd[i]     =NOTHING;
            data_line_even[j--]  =NOTHING;
      }*/
    }
}

/**
 * \brief Get each line data of frame for partial update
 *
 * \note
 * - Mark from (x0,y0) to (x1,y1) as update area to change data
 * - Default use whole area of EPD as update area currently
 *
 * @param x0 (x0,y0) as the left/top coordinates
 * @param x1 (x1,y1) as the right/bottom coordinates
 * @param y0 (x0,y0) as the left/top coordinates
 * @param y1 (x1,y1) as the right/bottom coordinates
 * @param previous_image_data_address The memory address of previous image data
 * @param new_image_data_address The memory address of new image data
 */
inline void epd_frame_partial_handle(uint16_t x0,uint16_t x1,uint16_t y0,uint16_t y1,EInt previous_image_data_address,EInt new_image_data_address){
    uint16_t i;
    uint8_t previous_line_array[LINE_BUFFER_DATA_SIZE];
    uint8_t new_line_array[LINE_BUFFER_DATA_SIZE];
     for (i = 0; i < COG_parameters[use_EPD_type_index].vertical_size; i++){
        /* Set charge pump voltage level reduce voltage shift */
        epd_spi_send_byte (0x04, COG_parameters[use_EPD_type_index].voltage_level);

         //Read line data from external array
        if(_On_EPD_read_handle!=NULL) {
                _On_EPD_read_handle(previous_image_data_address,previous_line_array,
                COG_parameters[use_EPD_type_index].horizontal_size);
                _On_EPD_read_handle(new_image_data_address,new_line_array,
                COG_parameters[use_EPD_type_index].horizontal_size);
        }
         //epd_display_line_handle(x0,x1,line_array,stage_no);

       // if(y0<=i && i<y1){
            epd_line_data_partial_handle(x0,x1,previous_line_array,new_line_array);
      // }else{
       //     epd_display_line_dummy_handle();
       // }

        previous_image_data_address+=COG_parameters[use_EPD_type_index].horizontal_size;//LINE_SIZE;
        new_image_data_address+=COG_parameters[use_EPD_type_index].horizontal_size;//LINE_SIZE;

        /* Scan byte shift per data line */
        data_line_scan[(i>>2)]= SCAN_TABLE[(i%4)];

        /* For 1.44 inch EPD, the border uses the internal signal control byte. */
        if(use_EPD_type_index==EPD_144)
                COG_Line.line_data_by_size.line_data_for_144.border_byte=0x00;

        /* Sending data */
        epd_spi_send (0x0A, (uint8_t *)&COG_Line.uint8,
                COG_parameters[use_EPD_type_index].data_line_size);

        /* Turn on Output Enable */
        epd_spi_send_byte (0x02, 0x2F);

        data_line_scan[(i>>2)]=0;
    }
 
}

/**
 * \brief Get each frame data of stage for partial update
 *
 * \note
 * - Mark from (x0,y0) to (x1,y1) as update area to change data
 * - Default use whole area of EPD as update area currently
 *
 * @param x0 (x0,y0) as the left/top coordinates
 * @param x1 (x1,y1) as the right/bottom coordinates
 * @param y0 (x0,y0) as the left/top coordinates
 * @param y1 (x1,y1) as the right/bottom coordinates
 * @param previous_image_data_address The memory address of previous image
 * @param new_image_data_address The memory address of new image
 */
inline void epd_stage_partial_handle(uint16_t x0,uint16_t x1,uint16_t y0,uint16_t y1,
                                          EInt previous_image_data_address,
                                          EInt new_image_data_address){

    current_frame_time=COG_parameters[use_EPD_type_index].frame_time_offset;

    /* Start a system SysTick timer to ensure the same duration of each stage  */
    start_EPD_timer();
    stage_time=partial_offset_time;
    cnt=0;
    /* Do while total time of frames exceed stage time
    * Per frame */
    do {
        //image_data_address=original_image_address;
       epd_frame_partial_handle(x0,x1,y0,y1,previous_image_data_address,new_image_data_address);
        cnt++;
    /* Count the frame time with offset */

        current_frame_time=(uint16_t)get_current_time_tick()+
                COG_parameters[use_EPD_type_index].frame_time_offset ;
    }while (stage_time>current_frame_time);

    /* Stop system timer */
    stop_EPD_timer();
}

/**
 * \brief The driving stages for getting Odd/Even data per line for global update
 *
 * \note
 * - There are 4 stages to complete an image global update on EPD.
 * - Each of the 4 stages time should be the same uses the same number of frames.
 * - One dot/pixel is comprised of 2 bits which are White(10), Black(11) or Nothing(01).
 *   The image data bytes must be divided into Odd and Even bytes.
 * - The COG driver uses a buffer to write one line of data (FIFO) - interlaced
 *   Even byte {D(200,y),D(198,y), D(196,y), D(194,y)}, ... ,{D(8,y),D(6,y),D(4,y), D(2,y)}
 *   Scan byte {S(1), S(2)...}, Odd{D(1,y),D(3,y)...}
 *   Odd byte  {D(1,y),D(3,y), D(5,y), D(7,y)}, ... ,{D(193,y),D(195,y),D(197,y), D(199,y)}
 * - One data bit can be
 * - For more details on the driving stages, please refer to the COG document Section 5.
 *
 * @param x0 the beginning position of a line
 * @param x1 the end position of a line
 * @param line_array The pointer of line array that stores line data
 * @param stage_no The assigned stage number that will proceed
 */
inline void epd_line_data_global_handle( uint16_t x0,uint16_t x1,uint8_t *line_array,uint8_t stage_no){
    uint16_t i,j;
    uint8_t temp_byte;
    j = COG_parameters[use_EPD_type_index].horizontal_size-1;
    x0 >>= 3;
    x1 = (x1 + 7) >> 3;
    for (i = 0; i < COG_parameters[use_EPD_type_index].horizontal_size; i++){
        if(x0<=i && i<x1){
            temp_byte =line_array[i];
            switch(stage_no) {
                case Stage1: // Compensate, Inverse previous image                    
                    data_line_odd[i]     =  ((temp_byte & 0x80) ? BLACK3  : WHITE3);
                    data_line_odd[i]    |=  ((temp_byte & 0x20) ? BLACK2  : WHITE2);
                    data_line_odd[i]    |=  ((temp_byte & 0x08) ? BLACK1  : WHITE1);
                    data_line_odd[i]    |=  ((temp_byte & 0x02) ? BLACK0  : WHITE0);

                    data_line_even[j]    = ((temp_byte & 0x01) ? BLACK3  : WHITE3);
                    data_line_even[j]   |= ((temp_byte & 0x04) ? BLACK2  : WHITE2);
                    data_line_even[j]   |= ((temp_byte & 0x10) ? BLACK1  : WHITE1);
                    data_line_even[j]   |= ((temp_byte & 0x40) ? BLACK0  : WHITE0);
                        break;
                case Stage2: // White
                    data_line_odd[i]     =  ((temp_byte & 0x80) ?  WHITE3 : NOTHING3);
                    data_line_odd[i]    |=  ((temp_byte & 0x20) ?  WHITE2 : NOTHING2);
                    data_line_odd[i]    |=  ((temp_byte & 0x08) ?  WHITE1 : NOTHING1);
                    data_line_odd[i]    |=  ((temp_byte & 0x02) ?  WHITE0 : NOTHING0);

                    data_line_even[j]    =  ((temp_byte & 0x01) ?  WHITE3 : NOTHING3);
                    data_line_even[j]   |=  ((temp_byte & 0x04) ?  WHITE2 : NOTHING2);
                    data_line_even[j]   |=  ((temp_byte & 0x10) ?  WHITE1 : NOTHING1);
                    data_line_even[j]   |=  ((temp_byte & 0x40) ?  WHITE0 : NOTHING0);
                        break;
                case Stage3: // Inverse new image
                    data_line_odd[i]     = ((temp_byte & 0x80) ? BLACK3  : NOTHING3);
                    data_line_odd[i]    |= ((temp_byte & 0x20) ? BLACK2  : NOTHING2);
                    data_line_odd[i]    |= ((temp_byte & 0x08) ? BLACK1  : NOTHING1);
                    data_line_odd[i]    |= ((temp_byte & 0x02) ? BLACK0  : NOTHING0);

                    data_line_even[j]    = ((temp_byte & 0x01) ? BLACK3  : NOTHING3);
                    data_line_even[j]   |= ((temp_byte & 0x04) ? BLACK2  : NOTHING2);
                    data_line_even[j]   |= ((temp_byte & 0x10) ? BLACK1  : NOTHING1);
                    data_line_even[j]   |= ((temp_byte & 0x40) ? BLACK0  : NOTHING0);
                        break;
                case Stage4: // New image
                    data_line_odd[i]     = ((temp_byte & 0x80) ? WHITE3  : BLACK3 );
                    data_line_odd[i]    |= ((temp_byte & 0x20) ? WHITE2  : BLACK2 );
                    data_line_odd[i]    |= ((temp_byte & 0x08) ? WHITE1  : BLACK1 );
                    data_line_odd[i]    |= ((temp_byte & 0x02) ? WHITE0  : BLACK0 );

                    data_line_even[j]    = ((temp_byte & 0x01) ? WHITE3  : BLACK3 );
                    data_line_even[j]   |= ((temp_byte & 0x04) ? WHITE2  : BLACK2 );
                    data_line_even[j]   |= ((temp_byte & 0x10) ? WHITE1  : BLACK1 );
                    data_line_even[j]   |= ((temp_byte & 0x40) ? WHITE0  : BLACK0 );
                        break;
                }
        }
        else{
            data_line_odd[i]    =NOTHING;
            data_line_even[j]   =NOTHING;
        }
        j--;
    }
}

/**
 * \brief Get each line data of frame
 *
 * \note
 * - Mark from (x0,y0) to (x1,y1) as update area to change data
 * - Default use whole area of EPD as update area currently
 *
 * @param x0 (x0,y0) as the left/top coordinates
 * @param x1 (x1,y1) as the right/bottom coordinates
 * @param y0 (x0,y0) as the left/top coordinates
 * @param y1 (x1,y1) as the right/bottom coordinates
 * @param image_data_address The memory address of image data
 * @param stage_no The assigned stage number that will proceed
 */
inline void epd_frame_global_handle(uint16_t x0,uint16_t x1,uint16_t y0,uint16_t y1,EInt image_data_address,uint8_t stage_no ){
    uint16_t i;
    uint8_t line_array[LINE_BUFFER_DATA_SIZE];   

    for (i = 0; i < COG_parameters[use_EPD_type_index].vertical_size; i++){
        /* Set charge pump voltage level reduce voltage shift */
        epd_spi_send_byte (0x04, COG_parameters[use_EPD_type_index].voltage_level);

         /* Read line data from external array */
        if(_On_EPD_read_handle!=NULL) {
                _On_EPD_read_handle(image_data_address,line_array,
                COG_parameters[use_EPD_type_index].horizontal_size);
        }

        /* Get line data */
        if(y0<=i && i<y1){
            epd_line_data_global_handle(x0,x1,line_array,stage_no);
        }else{
            epd_display_line_dummy_handle();    //last line, set to Nothing frame
        }

        image_data_address+=COG_parameters[use_EPD_type_index].horizontal_size;//LINE_SIZE

        /* Scan byte shift per data line */
        data_line_scan[(i>>2)]= SCAN_TABLE[(i%4)];

        /* For 1.44 inch EPD, the border uses the internal signal control byte. */
        if(use_EPD_type_index==EPD_144)
                COG_Line.line_data_by_size.line_data_for_144.border_byte=0x00;

        /* Sending data */
        epd_spi_send (0x0A, (uint8_t *)&COG_Line.uint8,
                COG_parameters[use_EPD_type_index].data_line_size);

        /* Turn on Output Enable */
        epd_spi_send_byte (0x02, 0x2F);

        data_line_scan[(i>>2)]=0;
    }   
}

/**
 * \brief Get each frame data of stage for global update
 *
 * \note
 * - Mark from (x0,y0) to (x1,y1) as update area to change data
 * - Default use whole area of EPD as update area currently
 *
 * @param x0 (x0,y0) as the left/top coordinates
 * @param x1 (x1,y1) as the right/bottom coordinates
 * @param y0 (x0,y0) as the left/top coordinates
 * @param y1 (x1,y1) as the right/bottom coordinates
 * @param image_data_address The memory address of image data
 * @param stage_no The assigned stage number that will proceed
 */
inline void epd_stage_global_handle(uint16_t x0,uint16_t x1,uint16_t y0,uint16_t y1,EInt image_data_address,uint8_t stage_no){
	current_frame_time=COG_parameters[use_EPD_type_index].frame_time_offset;
	/* Start a system SysTick timer to ensure the same duration of each stage  */
	start_EPD_timer();

	/* Do while total time of frames exceed stage time
	* Per frame */
	do {
        epd_frame_global_handle(x0,x1,y0,y1,image_data_address,stage_no);
		/* Count the frame time with offset */
		current_frame_time=(uint16_t)get_current_time_tick()+
			COG_parameters[use_EPD_type_index].frame_time_offset ;
	} while (stage_time>current_frame_time);

	/* Stop system timer */
	stop_EPD_timer();
}

/**
* \brief Write Dummy Line to COG
* \note A line whose all Scan Bytes are 0x00
*
* \param EPD_type_index The defined EPD size
*/
static inline void dummy_line(uint8_t EPD_type_index) {
	uint8_t	i;
	for (i = 0; i < (COG_parameters[EPD_type_index].vertical_size/8); i++) {
		switch(EPD_type_index) {
			case EPD_144:
				COG_Line.line_data_by_size.line_data_for_144.scan[i]=0x00;
				break;
			case EPD_200:
				COG_Line.line_data_by_size.line_data_for_200.scan[i]=0x00;
				break;
			case EPD_270:
				COG_Line.line_data_by_size.line_data_for_270.scan[i]=0x00;
				break;
		}
	}
	/* Set charge pump voltage level reduce voltage shift */
	epd_spi_send_byte (0x04, COG_parameters[EPD_type_index].voltage_level);

	/* Sending data */
	epd_spi_send (0x0A, (uint8_t *)&COG_Line.uint8, COG_parameters[EPD_type_index].data_line_size);

	/* Turn on Output Enable */
	epd_spi_send_byte (0x02, 0x2F);
}

/**
* \brief Write Nothing Frame to COG
*/
static inline void nothing_frame (void) {
	uint16_t i;
	for (i = 0; i <  COG_parameters[use_EPD_type_index].horizontal_size; i++) {
		data_line_even[i]=NOTHING;
		data_line_odd[i]=NOTHING;
	}
	for (i = 0; i < (COG_parameters[use_EPD_type_index].vertical_size); i++) {
		/* Set charge pump voltage level reduce voltage shift */
		epd_spi_send_byte (0x04, COG_parameters[use_EPD_type_index].voltage_level);

		/* Scan byte shift per data line */
		data_line_scan[(i>>2)]=SCAN_TABLE[(i%4)];

		/* Sending data */
		epd_spi_send (0x0A, (uint8_t *)&COG_Line.uint8, COG_parameters[use_EPD_type_index].data_line_size);

		/* Turn on Output Enable */
		epd_spi_send_byte (0x02, 0x2F);

		data_line_scan[(i>>2)]=0;
	}
}

/**
 * \brief Write image data from memory to EPD by partial update
 *
 * \note
 * - Mark from (x0,y0) to (x1,y1) as update area to change data
 * - Default use whole area of EPD as update area currently
 *
 * \param previous_image_memory_address The previous image address of memory
 * \param new_image_memory_address The new image address of memory
 * \param On_EPD_read_memory Developer needs to create an external function to read memory
 */
void EPD_image_data_partial_handle (EInt previous_image_memory_address,
                                            EInt new_image_memory_address,
                                            EPD_read_memory_handler On_EPD_read_memory) {

	_On_EPD_read_handle=On_EPD_read_memory;

        /* Standard 4 stages driving, update from (0,0) to (horizontal_size*8,vertical_size)  */
	epd_stage_partial_handle(0,
                             COG_parameters[use_EPD_type_index].horizontal_size*8,
                             0,
                             COG_parameters[use_EPD_type_index].vertical_size,
                             previous_image_memory_address,new_image_memory_address);
    nothing_frame();
    dummy_line(use_EPD_type_index);
    
}

/**
 * \brief Write image data from memory to EPD by global update
 *
 * \note
 * - Mark from (x0,y0) to (x1,y1) as update area to change data
 * - Default use whole area of EPD as update area currently
 *
 * \param previous_image_memory_address The previous image address of memory
 * \param new_image_memory_address The new image address of memory
 * \param On_EPD_read_memory Developer needs to create an external function to read memory
 */
void EPD_image_data_globa_handle(EInt previous_image_memory_address,
                                    EInt new_image_memory_address,
                                    EPD_read_memory_handler On_EPD_read_memory) {

    _On_EPD_read_handle=On_EPD_read_memory;

        /* Standard 4 stages driving, update from (0,0) to (horizontal_size*8,vertical_size)  */
   epd_stage_global_handle( 0,
                       COG_parameters[use_EPD_type_index].horizontal_size*8,
                       0,
                       COG_parameters[use_EPD_type_index].vertical_size,
                       previous_image_memory_address,
                       Stage1
                       );
	epd_stage_global_handle( 0,
                       COG_parameters[use_EPD_type_index].horizontal_size*8,
                       0,
                       COG_parameters[use_EPD_type_index].vertical_size,
                       previous_image_memory_address,
                       Stage2
                       );
	epd_stage_global_handle( 0,
                       COG_parameters[use_EPD_type_index].horizontal_size*8,
                       0,
                       COG_parameters[use_EPD_type_index].vertical_size,
                       new_image_memory_address,
                       Stage3
                       );
	epd_stage_global_handle( 0,
                       COG_parameters[use_EPD_type_index].horizontal_size*8,
                       0,
                       COG_parameters[use_EPD_type_index].vertical_size,
                       new_image_memory_address,
                       Stage4
                       );
}

/**
 * \brief Send Border dummy line (for 1.44" EPD at Power off COG stage)
 *
 * @param EPD_type_index The defined EPD size
 */
static void border_line(uint8_t EPD_type_index)
{
    uint16_t i;
    for (i = 0; i <  COG_parameters[EPD_type_index].horizontal_size; i++) {
 		data_line_even[i]=0x00;//NOTHING;
 		data_line_odd[i]=0x00;//NOTHING;
 	}
    for (i = 0; i < (COG_parameters[EPD_type_index].vertical_size/4); i++) {
            data_line_scan[i]=0x00;
    }

    COG_Line.line_data_by_size.line_data_for_144.border_byte=0xaa;
    epd_spi_send_byte (0x04, 0x03);

    // SPI (0x0a, line data....)
    epd_spi_send (0x0a, (uint8_t *)&COG_Line.uint8, COG_parameters[EPD_type_index].data_line_size);

    // SPI (0x02, 0x25)
     epd_spi_send_byte (0x02, 0x2F);
}

/**
* \brief Power Off COG Driver
* \note For detailed flow and description, please refer to the COG document Section 6.
*
* \param EPD_type_index The defined EPD size
*/
uint8_t EPD_power_off (void) {

	nothing_frame ();

	if(use_EPD_type_index==EPD_144) {
                EPD_border_high();
		border_line(use_EPD_type_index);
		delay_ms (200);
	 }
	 else {
		 dummy_line(use_EPD_type_index);

                delay_ms (25);

                 EPD_border_low();
                 delay_ms (200);
                 EPD_border_high();
	}
	// Latch reset turn on
	epd_spi_send_byte (0x03, 0x01);

	// Output enable off
	epd_spi_send_byte (0x02, 0x05);

	// Power off charge pump Vcom
	epd_spi_send_byte (0x05, 0x0E);

	// Power off charge negative voltage
	epd_spi_send_byte (0x05, 0x02);

	// Discharge
	epd_spi_send_byte (0x04, 0x0C);
	delay_ms (120);

	// Turn off all charge pumps
	epd_spi_send_byte (0x05, 0x00);

	// Turn off osc
	epd_spi_send_byte (0x07, 0x0D);

	// Discharge internal
	epd_spi_send_byte (0x04, 0x50);
	delay_ms (40);

	// Discharge internal
	epd_spi_send_byte (0x04, 0xA0);
	delay_ms (40);

	// Discharge internal
	epd_spi_send_byte (0x04, 0x00);

	// Set power and signals = 0
	EPD_rst_low ();
	epd_spi_detach ();
	EPD_cs_low ();
	EPD_Vcc_turn_off ();
	//EPD_border_low();

	// External discharge = 1
	EPD_discharge_high ();
	delay_ms (150);

	// External discharge = 0
	EPD_discharge_low ();
	return RES_OK;
}
#endif

