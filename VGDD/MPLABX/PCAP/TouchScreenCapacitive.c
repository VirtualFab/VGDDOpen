/*****************************************************************************
 * FileName:        TouchScreenCapacitive.c
 * Processor:       PIC24, PIC32, dsPIC, PIC24H
 * Compiler:        MPLAB C30, MPLAB C32
 * Company:         Microchip Technology Incorporated
 *
 * Software License Agreement
 *
 * Copyright 2011 Microchip Technology Inc.  All rights reserved.
 * Microchip licenses to you the right to use, modify, copy and distribute
 * Software only when embedded on a Microchip microcontroller or digital
 * signal controller, which is integrated into your product or third party
 * product (pursuant to the sublicense terms in the accompanying license
 * agreement).  
 *
 * You should refer to the license agreement accompanying this Software
 * for additional information regarding your rights and obligations.
 *
 * SOFTWARE AND DOCUMENTATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY
 * OF MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR
 * PURPOSE. IN NO EVENT SHALL MICROCHIP OR ITS LICENSORS BE LIABLE OR
 * OBLIGATED UNDER CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION,
 * BREACH OF WARRANTY, OR OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT
 * DAMAGES OR EXPENSES INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL,
 * INDIRECT, PUNITIVE OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 * COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY
 * CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF),
 * OR OTHER SIMILAR COSTS.
 */
// Author               Date        Comment
//
// Microchip         2011/01/21     Original release
// VirtualFab        2014/02/22     Added FT5x06 support - Thanks David!
// VirtualFab        2015/07/02     Fixed MTCH6301 support
// ----------------------------------------------------------------------------

#include "HardwareProfile.h"
#include "Graphics/Graphics.h"
#include "TimeDelay.h"
#include "TouchScreenCapacitive.h"
#include "Compiler.h"

WORD_VAL PCapX[5]= {-1,-1,-1,-1,-1};
WORD_VAL PCapY[5]= {-1,-1,-1,-1,-1};

/*********************************************************************
* Function: void TouchGetMsg(GOL_MSG* pMsg)
*
* PreCondition: none
*
* Input: pointer to the message structure to be populated
*
* Output: none
*
* Side Effects: none
*
* Overview: populates GOL message structure
*
* Note: none
*
********************************************************************/
void TouchGetMsg(GOL_MSG *pMsg)
{
    static SHORT    prevX = -1;
    static SHORT    prevY = -1;

    SHORT           x, y;

    x = TouchGetX(0);
    y = TouchGetY(0);
    pMsg->type = TYPE_TOUCHSCREEN;
    pMsg->uiEvent = EVENT_INVALID;

    if((x == -1) || (y == -1))
    {
        y = -1;
        x = -1;
    }

    if((prevX == x) && (prevY == y) && (x != -1) && (y != -1))
    {
        pMsg->uiEvent = EVENT_STILLPRESS;
        pMsg->param1 = x;
        pMsg->param2 = y;
        return;
    }

    if((prevX != -1) || (prevY != -1))
    {
        if((x != -1) && (y != -1))
        {

            // Move
            pMsg->uiEvent = EVENT_MOVE;
        }
        else
        {

            // Released
            pMsg->uiEvent = EVENT_RELEASE;
            pMsg->param1 = prevX;
            pMsg->param2 = prevY;
            prevX = x;
            prevY = y;
            return;
        }
    }
    else
    {
        if((x != -1) && (y != -1))
        {

            // Pressed
            pMsg->uiEvent = EVENT_PRESS;
        }
        else
        {

            // No message
            pMsg->uiEvent = EVENT_INVALID;
        }
    }

    pMsg->param1 = x;
    pMsg->param2 = y;
    prevX = x;
    prevY = y;
}

/*********************************************************************
* Function: void TouchHardwareInit(void)
*
* PreCondition: none
*
* Input: none
*
* Output: none
*
* Side Effects: none
*
* Overview: Initializes touch screen module.
*
* Note: none
*
********************************************************************/
void TouchHardwareInit(void *initValues)
{

}

/*********************************************************************
* Function: SHORT TouchGetX()
*
* PreCondition: none
*
* Input: none
*
* Output: x coordinate
*
* Side Effects: none
*
* Overview: returns x coordinate if touch screen is pressed
*           and -1 if not
*
* Note: none
*
********************************************************************/
SHORT TouchGetX(BYTE touchNumber)
{
    long    result;

    result= PCapX[touchNumber].Val;
    return ((SHORT)result);
}

/*********************************************************************
* Function: SHORT TouchGetY()
*
* PreCondition: none
*
* Input: none
*
* Output: y coordinate
*
* Side Effects: none
*
* Overview: returns y coordinate if touch screen is pressed
*           and -1 if not
*
* Note: none
*
********************************************************************/
SHORT TouchGetY(BYTE touchNumber)
{

    long    result;
    result= PCapY[touchNumber].Val;
    return ((SHORT)result);
}

//****************************************************************************
//Touch Int Handler
//Triggered by the INT0 external interrupt pin
void __ISR(_EXTERNAL_0_VECTOR, ipl4) _PCAPHandler(void) {
#if defined(USE_CAPACITIVE_CONTROLLER_MTCH6301)  //When using MTCH6301
// Default Calibration Inset Value (percentage of vertical or horizontal resolution)
// Calibration Inset = ( CALIBRATIONINSET / 2 ) % , Range of 0?20% with 0.5% resolution
// Example with CALIBRATIONINSET == 20, the calibration points are measured
// 10% from the corners.
#define CALIBRATIONINSET   20                               // range 0 <= CALIBRATIONINSET <= 40 

#define CAL_X_INSET    (((GetMaxX()+1)*(CALIBRATIONINSET>>1))/100)
#define CAL_Y_INSET    (((GetMaxY()+1)*(CALIBRATIONINSET>>1))/100)
    
     static BYTE data[6]={0};
     BYTE touchpoint;
     BYTE penstatus;

     I2C_ReadBlock(0x4a,0x55, &data[0], 6);

     penstatus = data[1] & 0x01;	//Pen down
     touchpoint = (data[1]&0x78)>>3;

    if(penstatus == 1)
    {
      PCapX[touchpoint].Val = data[2];
      PCapX[touchpoint].Val |= (WORD)(data[3]<<7);

      PCapY[touchpoint].Val = data[4]; 
      PCapY[touchpoint].Val |= (WORD)(data[5]<<7);

      PCapX[touchpoint].Val = ((PCapX[touchpoint].Val * (GetMaxX()+1))>>10) - CAL_X_INSET;
      PCapY[touchpoint].Val = ((PCapY[touchpoint].Val * (GetMaxY()+1))>>10);

    }
    else
    {
      PCapX[touchpoint].Val = -1;
      PCapY[touchpoint].Val = -1;
    }

#elif defined(USE_CAPACITIVE_CONTROLLER_FT5x06) // When Using the NHD with FT5x06 Touch Controller

/********************************************************
 * Data[0] = Device Mode
 * Data[1] = Gesture ID
 * Data[2] = Touch Points Detected
 * Data[3] = [7:6] is Event Flag, [3:0] is X Pos 1 [11:8]
 * Data[4] = X Pos 1 [7:0]
 * Data[5] = [7:6] is Event Flag, [3:0] is Y Pos 1 [11:8]
 * Data[6] = Y Pos 1 [7:0]
 * .
 * .
 * Data[9] = [7:6] is Event Flag, [3:0] is X Pos 2 [11:8]
 * .
 * .
 * Data[15] = [7:6] is Event Flag, [3:0] is X Pos 3 [11:8]
 * .
 * .
 * Data[21] = [7:6] is Event Flag, [3:0] is X Pos 4 [11:8]
 * .
 * .
 * Data[27] = [7:6] is Event Flag, [3:0] is X Pos 5 [11:8]

 typedef enum  	ft5x06_gesture { 
  FT5X06_GESTURE_none = 0x00, FT5X06_GESTURE_up = 0x10, FT5X06_GESTURE_left = 0x14, FT5X06_GESTURE_down = 0x18, 
  FT5X06_GESTURE_right = 0x1c, FT5X06_GESTURE_zoomIn = 0x48, FT5X06_GESTURE_zoomOut = 0x49 
};

typedef enum  	ft5x06_touchEvent { FT5X06_EVENT_putDown = 0, FT5X06_EVENT_putUp = 1, FT5X06_EVENT_contact = 2, FT5X06_EVENT_invalid = 3 };
*/

    static BYTE data[32] = {0};
    BYTE touchpoint = 0;
    BYTE penstatus = 99;
    BYTE registeroffset = 3;
    BYTE bNumOfPointDetected;

    // FT5x06 Device Address = 0x70
    //Id = 70, offset = 0, *buffer, NoOfBytes
    I2C_ReadBlock(0x70, 0, &data[0], 0x1F);

    bNumOfPointDetected = data[2]&0x7;

    for (touchpoint = 0; touchpoint < 5; touchpoint++) {
        penstatus = (data[registeroffset] & 0xC0) >> 6;

        if ((penstatus == 0 || penstatus == 2) && bNumOfPointDetected > touchpoint) //Touch down
        {
            PCapX[touchpoint].Val = data[registeroffset + 1];
            PCapX[touchpoint].Val |= (WORD) ((data[registeroffset] & 0xf) << 8);

            registeroffset += 2;

            PCapY[touchpoint].Val = data[registeroffset + 1];
            PCapY[touchpoint].Val |= (WORD) ((data[registeroffset] & 0xf) << 8);

            registeroffset += 4;
        } else {
            PCapX[touchpoint].Val = -1;
            PCapY[touchpoint].Val = -1;
            registeroffset += 6;
        }
        penstatus = 99; //Resets so that the next one is evaluated correctly.
    }
#else
    #error No Capacitive Controller defined in HardwareProfile.h - Please use either #define USE_CAPACITIVE_CONTROLLER_MTCH6301 or #define USE_CAPACITIVE_CONTROLLER_FT5x06
#endif
        IFS0bits.INT0IF = 0; // clear the interrupt flag
}
