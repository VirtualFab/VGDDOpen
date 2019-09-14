/*************************************************************************
 *
 *    (c) Olimex Ltd. 2010
 *
 *    File name   : BSP.h
 *    Description : Board support package
 *
 *    History :
 *    1. Date        : 31 May 2010
 *       Author      : Aleksandar Mitev
 *       Description : Create
 *
 **************************************************************************/

#ifndef _BSP_H
#define _BSP_H

// DIGITAL INS
#define DIGITAL_IN_1 (1 << 7)
#define DIGITAL_IN_2 (1 << 6)

// buttons
#define BUTTON_1 (1 << 14)
#define BUTTON_2 (1 << 15)
#define BUTTON_3 (1 << 13)

/*************************************************************************
 * Function Name: UserPeriphInit(..)
 * Parameters: none
 *
 * Return: none
 *
 * Description: initializes pins used by the LEDS, buttons, digital ins, 
 *
 *************************************************************************/
void UserPeriphInit(void);

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
void StatLEDSet(int ledno, BOOL enabled);

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
BOOL StatLEDGet(int ledno);

/*************************************************************************
 * Function Name: StatLEDToggle(..)
 * Parameters:	ledno - index of the led to alter (refer to the schematics)
 *
 * Return: none
 *
 * Description: toggles a LED
 *
 *************************************************************************/
void StatLEDToggle(int ledno);

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
void RelaySet(int relayno, BOOL enabled);

/*************************************************************************
 * Function Name: RelaySet(..)
 * Parameters:	relayno - index of the relay to check (refer to the schematics)
 *
 * Return: TRUE if on, FALSE if off
 *
 * Description: check state of relay
 *
 *************************************************************************/
BOOL RelayGet(int relayno);

/*************************************************************************
 * Function Name: PotentiometerGet(..)
 * Parameters: none
 *
 * Return: 10 bit unsigned average of the potentiometer value measured
 *
 * Description: Get the average of the potentiometer value measured
 *
 *************************************************************************/
int PotentiometerGet(void);

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
UINT32 ButtonsGet(void);

/*************************************************************************
 * Function Name: DigitalINsGet(..)
 * Parameters: none
 *
 * Return: Bitmask of the states of the digital opto inputs
 *
 * Description: Gets the state of the digital opto inputs as they appear on the port
 *
 *************************************************************************/
UINT32 DigitalINsGet(void);

/*************************************************************************
 * Function Name: ButtonsUpdate(..)
 * Parameters:	val - bitmask of states of buttons as they appear on the port
 *				enabled - flag to indicate if val is actually an updated state of buttons
 *
 * Return: none
 *
 * Description: Call this function frequently to implement debounce of button presses
 *
 *************************************************************************/
void ButtonsUpdate(UINT32 val, BOOL updated);

/*************************************************************************
 * Function Name: PotUpdate(..)
 * Parameters:	val - voltage in steps as it appears on the ADC
 *
 * Return: none
 *
 * Description: Call this function frequently to averaging of value on pot
 *
 *************************************************************************/
void PotUpdate(int val);

/*************************************************************************
 * Function Name: USBOTG_Overloaded(..)
 * Parameters:	none
 *
 * Return: TRUE if overload detected, FALSE if not
 *
 * Description: Detects an overload on the usb line
 *
 *************************************************************************/
BOOL USBOTG_Overloaded(void);

/*************************************************************************
 * Function Name: USBOTG_ReadID(..)
 * Parameters:	none
 *
 * Return: value on the USBID pin
 *
 * Description: Detects the state if the USBID pin
 *
 *************************************************************************/
BOOL USBOTG_ReadID(void);

/*************************************************************************
 * Function Name: USBOTG_PowerLine(..)
 * Parameters:	TRUE to power the USB line, FALSE to power OFF
 *
 * Return: none
 *
 * Description: Powers ON/OFF the usb line
 *
 *************************************************************************/
void USBOTG_PowerLine(BOOL enable);


/* ADC sampling ------------------------------------------------------------*/
void ADCStartSampling(void);
extern int ADCMeanValue;

#endif // _BSP_H
