/*****************************************************************************
 *  Module for VirtualFab GUIBoard32
 *  Board Support Package
 *****************************************************************************
 * FileName:        BSP.c
 * Dependencies:    HardwareProfile.h
 * Processor:       PIC32
 * Company:         VirtualFab
 *
 * Software License Agreement
 *
 * Copyright (c) 2013 VirtualFab  All rights reserved.
 * VirtualFab licenses to you the right to use, modify, copy and distribute
 * this Software as you wish
 *
 * SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY
 * OF MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR
 * PURPOSE. IN NO EVENT SHALL VIRTUALFAB OR ITS LICENSORS BE LIABLE OR
 * OBLIGATED UNDER CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION,
 * BREACH OF WARRANTY, OR OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT
 * DAMAGES OR EXPENSES INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL,
 * INDIRECT, PUNITIVE OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 * COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY
 * CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF),
 * OR OTHER SIMILAR COSTS.
 *
 * Author               Date        Comment
 *****************************************************************************
 * VirtualFab           2013/02/10	Version 1.0 release
 *****************************************************************************/
#include "HardwareProfile.h"
#include "BSP.h"

/* Private define ------------------------------------------------------------*/
#define BUTTONS_NUMBER 1
#define BUTTON_DEBOUNCE_COUNT 10

/* Private typedef -----------------------------------------------------------*/
typedef struct _BUTTONS_STRUCT {
    int counter[BUTTONS_NUMBER];
    BOOL state[BUTTONS_NUMBER];
} BUTTONS_STRUCT;


/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
static BUTTONS_STRUCT buttonsData;

/* Private function prototypes -----------------------------------------------*/
/* Private functions ---------------------------------------------------------*/

/*************************************************************************
 * Function Name: UserPeriphInit(..)
 * Parameters: none
 *
 * Return: none
 *
 * Description: initializes pins used by the LEDS, buttons, digital ins, 
 *
 *************************************************************************/
void UserPeriphInit(void) {
    int i;

    // init buttons
    // init buttons struct
    for (i = 0; i < BUTTONS_NUMBER; i++) {
        buttonsData.counter[i] = BUTTON_DEBOUNCE_COUNT;
        buttonsData.state[i] = FALSE;
    }

    // init USB OTG pins
    //PORTClearBits(IOPORT_B, BIT_5);
    //PORTSetPinsDigitalOut(IOPORT_B, BIT_5); // -> VBUSON

    //PORTSetPinsDigitalIn(IOPORT_G, BIT_3); // -> USB_FAULT

    //PORTSetPinsDigitalIn(IOPORT_F, BIT_3); // -> USBID
}

/*************************************************************************
 * Function Name: ButtonsGet(..)
 * Parameters: none
 *
 * Return: Bitmask of the states of the buttons
 *
 * Description: Gets the state of buttons (button presses are debounced)
 *
 * NOTE: A button press may be read only once!
 *
 *************************************************************************/
UINT32 ButtonsGet(void) {
    UINT32 tmp;

    tmp = 0x00;

    if (buttonsData.state[0])
        tmp |= BUTTON_1;
    //if (buttonsData.state[1])
    //    tmp |= BUTTON_2;
    //if (buttonsData.state[2])
    //    tmp |= BUTTON_3;

    return tmp;
}

/*************************************************************************
 * Function Name: ButtonsUpdate(..)
 * Parameters:	val - bitmask of states of buttons as they appear on the port
 *          enabled - flag to indicate if val is actually an updated state of buttons
 *
 * Return: none
 *
 * Description: Call this function frequently to implement debounce of button presses
 *
 *************************************************************************/
void ButtonsUpdate(UINT32 val, BOOL updated) {
    static const UINT32 butArr[BUTTONS_NUMBER] = {BUTTON_1}; //, BUTTON_2, BUTTON_3
    int i;

    if (updated) {
        // cycle through all the buttons
        for (i = 0; i < BUTTONS_NUMBER; i++) {
            if (!(val & butArr[i])) {
                if (buttonsData.counter[i]) {
                    if (--buttonsData.counter[i])
                        buttonsData.state[i] = FALSE;
                    else
                        buttonsData.state[i] = TRUE;
                } else {
                    // uncomment this line if you walnt short press of the button
                    //					buttonsData.state[i] = FALSE;
                }
            } else {
                buttonsData.state[i] = FALSE;
                buttonsData.counter[i] = BUTTON_DEBOUNCE_COUNT;
            }
        }
    }
}

/*************************************************************************
 * Function Name: USBOTG_Overloaded(..)
 * Parameters:	none
 *
 * Return: TRUE if overload detected, FALSE if not
 *
 * Description: Detects an overload on the usb line
 *
 *************************************************************************/
BOOL USBOTG_Overloaded(void) {
    return !PORTReadBits(IOPORT_B, BIT_3);
}

/*************************************************************************
 * Function Name: USBOTG_ReadID(..)
 * Parameters:	none
 *
 * Return: value on the USBID pin
 *
 * Description: Detects the state if the USBID pin
 *
 *************************************************************************/
BOOL USBOTG_ReadID(void) {
    return PORTReadBits(IOPORT_F, BIT_3);
}

/*************************************************************************
 * Function Name: USBOTG_PowerLine(..)
 * Parameters:	TRUE to power the USB line, FALSE to power OFF
 *
 * Return: none
 *
 * Description: Powers ON/OFF the usb line
 *
 *************************************************************************/
void USBOTG_PowerLine(BOOL enable) {
//    if (enable)
//        PORTSetBits(IOPORT_B, BIT_5);
//    else
//        PORTClearBits(IOPORT_B, BIT_5);
}
