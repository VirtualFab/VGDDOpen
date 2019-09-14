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
 * VirtualFab           2013/02/10	Version 1.0 release
 *****************************************************************************/

#ifndef _BSP_H
#define _BSP_H

//#include "GenericTypeDefs.h"
#include "Compiler.h"

// buttons
#define BUTTON_1 (1 << 9)

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

#endif // _BSP_H
