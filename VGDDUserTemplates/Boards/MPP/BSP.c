/*****************************************************************************
 *  Module for VirtualFab MPP board
 *  Board Support Package
 *****************************************************************************
 * FileName:        BSP.c
 * Dependencies:    HardwareProfile.h
 * Processor:       PIC24, PIC32
 * Company:         VirtualFab
 *
 * Software License Agreement
 *
 * Copyright (c) 2012 VirtualFab  All rights reserved.
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
 * VirtualFab           2012/11/04	Version 1.0 release
 *****************************************************************************/
#include "HardwareProfile.h"
#include "BSP.h"

/* Private define ------------------------------------------------------------*/
#define BUTTONS_NUMBER 1
#define POT_SAMPLES_NUMBER 10
#define BUTTON_DEBOUNCE_COUNT 10

/* Private typedef -----------------------------------------------------------*/
typedef struct _BUTTONS_STRUCT {
    int counter[BUTTONS_NUMBER];
    BOOL state[BUTTONS_NUMBER];
} BUTTONS_STRUCT;


/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
static BUTTONS_STRUCT buttonsData;
static int potSamples[POT_SAMPLES_NUMBER];
static int potSamplesIndex;

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

    // init LEDS
    PORTClearBits(IOPORT_E, BIT_9);
    PORTSetPinsDigitalOut(IOPORT_E, BIT_9);

    // init buttons
    // init buttons struct
    for (i = 0; i < BUTTONS_NUMBER; i++) {
        buttonsData.counter[i] = BUTTON_DEBOUNCE_COUNT;
        buttonsData.state[i] = FALSE;
    }

    // init digital ins
    //PORTSetPinsDigitalIn(IOPORT_G, BIT_6 | BIT_7);

    // init relays outs
    //PORTClearBits(IOPORT_G, BIT_12 | BIT_13);
    //PORTSetPinsDigitalOut(IOPORT_G, BIT_12 | BIT_13);

    // init USB OTG pins
    //PORTClearBits(IOPORT_B, BIT_5);
    //PORTSetPinsDigitalOut(IOPORT_B, BIT_5); // -> VBUSON

    //PORTSetPinsDigitalIn(IOPORT_G, BIT_3); // -> USB_FAULT

    //PORTSetPinsDigitalIn(IOPORT_F, BIT_3); // -> USBID

    // analogue in for pot
    //PORTSetPinsAnalogIn(IOPORT_B, BIT_8);

    // init pot data structures
    for (i = 0; i < POT_SAMPLES_NUMBER; i++) {
        potSamples[i] = 0;
    }
    potSamplesIndex = 0;

    // init accelerometer
    //SMB380_Init();
}

/*************************************************************************
 * Function Name: StatLEDSet(..)
 * Parameters:	ledno - index of the led to alter (refer to the schematics)
 *				enabled - TRUE to turn on, FALSE to turn off
 *
 * Return: none
 *
 * Description: turns on/off LED
 *
 *************************************************************************/
void StatLEDSet(int ledno, BOOL enabled) {
    switch (ledno) {
        case 1:
            if (enabled) PORTSetBits(IOPORT_E, BIT_9);
            else PORTClearBits(IOPORT_E, BIT_9);
            break;
        case 2:
            //if (enabled) PORTSetBits(IOPORT_D, BIT_1);
            //else PORTClearBits(IOPORT_D, BIT_1);
            break;
        case 3:
            //if (enabled) PORTSetBits(IOPORT_D, BIT_2);
            //else PORTClearBits(IOPORT_D, BIT_2);
            break;
    }
}

/*************************************************************************
 * Function Name: StatLEDGet(..)
 * Parameters:	ledno - index of the led to check (refer to the schematics)
 *				enabled - 
 *
 * Return: TRUE if on, FALSE if off
 *
 * Description: check state of led
 *
 *************************************************************************/
BOOL StatLEDGet(int ledno) {
    switch (ledno) {
        case 1:
            return (PORTReadBits(IOPORT_E, BIT_9) == BIT_9);
//        case 2:
//            return (PORTReadBits(IOPORT_D, BIT_1) == BIT_1);
//        case 3:
//            return (PORTReadBits(IOPORT_D, BIT_2) == BIT_2);
    }
}

/*************************************************************************
 * Function Name: StatLEDToggle(..)
 * Parameters:	ledno - index of the led to alter (refer to the schematics)
 *
 * Return: none
 *
 * Description: toggles a LED
 *
 *************************************************************************/
void StatLEDToggle(int ledno) {
    switch (ledno) {
        case 1:
            PORTToggleBits(IOPORT_E, BIT_9);
            break;
//        case 2:
//            PORTToggleBits(IOPORT_D, BIT_1);
//            break;
//        case 3:
//            PORTToggleBits(IOPORT_D, BIT_2);
    }
}

/*************************************************************************
 * Function Name: RelaySet(..)
 * Parameters:	relayno - index of the relay to alter (refer to the schematics)
 *				enabled - TRUE to turn on, FALSE to turn off
 *
 * Return: none
 *
 * Description: turns on/off a relay on the board
 *
 *************************************************************************/
void RelaySet(int relayno, BOOL enabled) {
    switch (relayno) {
        case 1:
            //if (enabled) PORTSetBits(IOPORT_G, BIT_12);
            //else PORTClearBits(IOPORT_G, BIT_12);
            break;
        case 2:
            //if (enabled) PORTSetBits(IOPORT_G, BIT_13);
            //else PORTClearBits(IOPORT_G, BIT_13);
            break;
    }
}

/*************************************************************************
 * Function Name: RelaySet(..)
 * Parameters:	relayno - index of the relay to check (refer to the schematics)
 *
 * Return: TRUE if on, FALSE if off
 *
 * Description: check state of relay
 *
 *************************************************************************/
BOOL RelayGet(int relayno) {
    switch (relayno) {
//        case 1:
//            return (PORTReadBits(IOPORT_G, BIT_12) == BIT_12);
//        case 2:
//            return (PORTReadBits(IOPORT_G, BIT_13) == BIT_13);
    }
}

/*************************************************************************
 * Function Name: PotentiometerGet(..)
 * Parameters: none
 *
 * Return: 10 bit unsigned average of the potentiometer value measured
 *
 * Description: Get the average of the potentiometer value measured
 *
 *************************************************************************/
int PotentiometerGet(void) {
    int tmp = 0;
    int i;

    for (i = 0; i < POT_SAMPLES_NUMBER; i++) {
        tmp += potSamples[i];
    }

    tmp /= POT_SAMPLES_NUMBER;

    return tmp;
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
 * Function Name: DigitalINsGet(..)
 * Parameters: none
 *
 * Return: Bitmask of the states of the digital opto inputs
 *
 * Description: Gets the state of the digital opto inputs as they appear on the port
 *
 *************************************************************************/
UINT32 DigitalINsGet(void) {
    UINT32 tmp = 0x00;

    //tmp = PORTRead(IOPORT_G);
    //tmp &= DIGITAL_IN_1 | DIGITAL_IN_2;

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
 * Function Name: PotUpdate(..)
 * Parameters:	val - voltage in steps as it appears on the ADC
 *
 * Return: none
 *
 * Description: Call this function frequently to averaging of value on pot
 *
 *************************************************************************/
void PotUpdate(int val) {
    if (potSamplesIndex >= POT_SAMPLES_NUMBER) {
        potSamplesIndex = 0;
    }

    potSamples[potSamplesIndex++] = val;
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

/* ADC sampling ------------------------------------------------------------*/
// Commented out - not needed for demo: using ResistiveTouschscreen POT conversion
//void ADCStartSampling(void) {
//    PORTSetPinsAnalogIn(IOPORT_B, BIT_8); // Configure AN8 pin as analog input
//    CloseADC10(); // ensure the ADC is off before setting the configuration
//    OpenADC10(ADC_FORMAT_INTG | ADC_CLK_AUTO | ADC_AUTO_SAMPLING_OFF,
//            ADC_VREF_AVDD_AVSS | ADC_OFFSET_CAL_DISABLE | ADC_SCAN_OFF | ADC_SAMPLES_PER_INT_1 | ADC_BUF_16 | ADC_ALT_INPUT_OFF,
//            ADC_CONV_CLK_INTERNAL_RC | ADC_SAMPLE_TIME_15,
//            0, //ENABLE_AN8_ANA,
//            //SKIP_SCAN_AN1 | SKIP_SCAN_AN2 | SKIP_SCAN_AN3 | SKIP_SCAN_AN4 | SKIP_SCAN_AN5 | SKIP_SCAN_AN6 | SKIP_SCAN_AN7 |
//            0); //SKIP_SCAN_AN9 | SKIP_SCAN_AN10 | SKIP_SCAN_AN11 | SKIP_SCAN_AN12 | SKIP_SCAN_AN13 | SKIP_SCAN_AN14 | SKIP_SCAN_AN15); // configure ADC
//    ConfigIntADC10(ADC_INT_ON | ADC_INT_PRI_4 | ADC_INT_SUB_PRI_0);
//    EnableADC10(); // Enable the ADC
//    //     choose channel to convert
//    //    AD1CHS = 8 << 16;
//    SetChanADC10(ADC_CH0_POS_SAMPLEA_AN8);
//    AcquireADC10(); // Start ADC conversion
//}
//
//void __ISR(_ADC_VECTOR, IPL4AUTO) _ADCInterruptHandler(void) {
//    // ADC sampling complete
//    if (INTGetFlag(INT_AD1)) {
//        INTClearFlag(INT_AD1);
//        PotUpdate(ReadADC10(0));
//        AcquireADC10(); // Start ADC conversion
//    }
//}