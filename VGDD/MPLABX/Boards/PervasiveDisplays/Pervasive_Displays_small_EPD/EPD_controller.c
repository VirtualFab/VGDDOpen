/**
* \brief The interface for external application wants to update EPD
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

#include  "EPD_controller.h"

/**
 * \brief Initialize the EPD hardware setting 
 */
void EPD_display_init(void) {
	EPD_init();
}


/**
 * \brief Show image from memory by global update
 *
 * \param EPD_type_index The defined EPD size
 * \param previous_image_address The address of memory that stores previous image
 * \param new_image_address The address of memory that stores new image
 * \param On_EPD_read_memory External function to read memory
 */
void EPD_display_global(uint8_t EPD_type_index,EInt previous_image_address,
EInt new_image_address,EPD_read_memory_handler On_EPD_read_memory) {

	/* Initialize EPD hardware */
	EPD_init();
	
	/* Power on COG Driver */
	EPD_power_on();

	/* Initialize COG Driver */
	EPD_initialize_driver(EPD_type_index);
    
	/* Display image data on EPD from SRAM memory */
	EPD_image_data_globa_handle(previous_image_address,
	    new_image_address,On_EPD_read_memory);

	/* Power off COG Driver */
	EPD_power_off ();
  
}

/**
 * \brief Show image from memory by partial update
 *
 * \note
 * - The EPD_init, power on and initialize COG just run at the beginning of partial
 *   update cycle, not per pattern.
 * - The difference of Partial Update from Global Update is if there is next
 *   pattern to be updated on EPD, user is able to write next image data to
 *   memory and COG without power off the COG for better visual experience and
 *   faster update time.
 * - The timing to use EPD_power_off for partial update is by use case and ghosting
 *   improvement.
 *
 * \param EPD_type_index The defined EPD size
 * \param previous_image_address The address of memory that stores previous image
 * \param new_image_address The address of memory that stores new image
 * \param On_EPD_read_memory External function to read memory
 */
void EPD_display_partial(uint8_t EPD_type_index,EInt previous_image_address,
EInt new_image_address,EPD_read_memory_handler On_EPD_read_memory) {

    EPD_image_data_partial_handle(previous_image_address,
    	new_image_address,On_EPD_read_memory);
}

/**
 * \brief Initialize the EPD hardware setting and COG driver
 *
 * \param EPD_type_index The defined EPD size 
 */
void EPD_power_init(uint8_t EPD_type_index) {

	/* Initialize EPD hardware */
	EPD_init();

	/* Power on COG Driver */
	EPD_power_on();

	/* Initialize COG Driver */
	EPD_initialize_driver(EPD_type_index);
}

/**
 * Power off COG Driver
 */
void EPD_power_end(void){
	EPD_power_off ();
}

/**
 * \brief Show image from memory when SPI is common used with COG and memory
 * for global update
 *
 * \note
 * - This function must work with EPD_power_init when SPI is common used with
 *   COG and memory, or the charge pump doesn't work correctly.
 * - EPD_power_init -> write data to memory (switch SPI) -> EPD_display_glabal_Ex
 *
 * \param EPD_type_index The defined EPD size
 * \param previous_image_address The address of memory that stores previous image
 * \param new_image_address The address of memory that stores new image
 * \param On_EPD_read_memory External function to read memory
 */
void EPD_display_global_Ex(uint8_t EPD_type_index,EInt previous_image_address,
	EInt new_image_address,EPD_read_memory_handler On_EPD_read_memory) {

	/* Display image data on EPD from memory */
	EPD_image_data_globa_handle(previous_image_address,
	    new_image_address,On_EPD_read_memory);
	
	/* Power off COG Driver */
	EPD_power_off ();
}


/**
 * \brief Show image from memory when SPI is common used with COG and memory
 * for partial update
 *
 * \param EPD_type_index The defined EPD size
 * \param previous_image_address The address of memory that stores previous image
 * \param new_image_address The address of memory that stores new image
 * \param On_EPD_read_memory External function to read memory
 */
void EPD_display_partial_Ex(uint8_t EPD_type_index,EInt previous_image_address,
	EInt new_image_address,EPD_read_memory_handler On_EPD_read_memory){
	/* Display image data on EPD from SRAM memory */
	EPD_image_data_partial_handle(previous_image_address,
	   new_image_address,On_EPD_read_memory);
}

