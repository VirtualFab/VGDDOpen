/*****************************************************************************
 * ADS7843 SPI touch screen driver
 *****************************************************************************
 * FileName:        TouchScreenADS7843.h
 * Dependencies:    Graphics.h
 * Processor:       PIC24F, PIC24H, dsPIC, PIC32
 * Compiler:       	MPLAB C30, MPLAB C32
 * Linker:          MPLAB LINK30, MPLAB LINK32
 * Company:         EasyMeter Srl
 *
 * Software License Agreement
 *
 * Copyright (c) 2010-2011 EasyMeter Srl.  All rights reserved.
 * Author                          Date         Comment
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Fabio Violino		09/07/2010	Prima stesura dopo 12gg di sofferenza...
 * Fabio Violino		31/12/2010	Modifiche per includerlo nel BootLoader
 *****************************************************************************/
#ifndef _TOUCHSCREEN_H
#define _TOUCHSCREEN_H

#include "Graphics/Graphics.h"
#ifdef USE_HWTP_IC
#if defined(__PIC24F__) || defined(__PIC24H__) || defined(__dsPIC33F__)
#include "spi.h"
#elif defined(__PIC32MX__)
#include "peripheral/spi.h"
#endif
#endif

//#include "OutTextXYchars.h"

//#define SWAP_X_AND_Y
//#define FLIP_X
//#define FLIP_Y

// Default calibration values 120 922 131 701
#define XMAXCAL 1710
#define YMINCAL 360
#define YMAXCAL 1610
#define XMINCAL 290

// Max/Min ADC values for each direction
extern volatile WORD _calXMin;
extern volatile WORD _calXMax;
extern volatile WORD _calYMin;
extern volatile WORD _calYMax;

// Addresses for calibration and version values in EEPROM or EEPROM Emulation 16bit
#define ADDRESS_VERSION 0x0000 
#define ADDRESS_XMIN    0x0002 
#define ADDRESS_XMAX    0x0004 
#define ADDRESS_YMIN    0x0006 
#define ADDRESS_YMAX    0x0008 

// Current ADC values for X and Y channels and potentiometer R6
extern volatile INT16 adcX;
extern volatile INT16 adcY;
extern volatile INT16 adcPot;
extern volatile UINT8 TpTouched;

/*********************************************************************
 * Function: void TouchDetectPosition(void)
 * PreCondition: none
 * Input: none
 * Output: 1 if it has completed the reading cycle, otherwise 0
 * Side Effects: none
 * Overview: Process the detection of touch on the ADC
 * Note: none
 ********************************************************************/
UINT8 TouchDetectPosition(void);

/*********************************************************************
 * Function: void TouchInit(void *initValues)
 * PreCondition: none
 * Input: none
 * Output: none
 * Side Effects: none
 * Overview: Open SPIx channel to communicate with ADS7843
 * Note: none
 ********************************************************************/
void TouchHardwareInit(void *initValues);

/*********************************************************************
 * Function: SHORT TouchGetX()
 * PreCondition: none
 * Input: none
 * Output: x coordinate
 * Side Effects: none
 * Overview: returns x coordinate if touch screen is pressed
 *           and -1 if not
 * Note: none
 ********************************************************************/
INT16 TouchGetX(void);

/*********************************************************************
 * Function: SHORT TouchGetY()
 * PreCondition: none
 * Input: none
 * Output: y coordinate
 * Side Effects: none
 * Overview: returns y coordinate if touch screen is pressed
 *           and -1 if not
 * Note: none
 ********************************************************************/
INT16 TouchGetY(void);

/*********************************************************************
 * Function: void TouchGetMsg(GOL_MSG* pMsg)
 * PreCondition: none
 * Input: pointer to the message structure to be populated
 * Output: none
 * Side Effects: none
 * Overview: populates GOL message structure
 * Note: none
 ********************************************************************/
void TouchGetMsg(GOL_MSG *pMsg);

/*********************************************************************
 * Function: void TouchCalibration()
 * PreCondition: none
 * Input: none
 * Output: none
 * Side Effects: none
 * Overview: Runs the calibration routine.
 * Note: none
 ********************************************************************/
void TouchCalibration(void);

/*********************************************************************
 * Function: void TouchStoreCalibration(void)
 * PreCondition: EEPROMInit() must be called before
 * Input: none
 * Output: none
 * Side Effects: none
 * Overview: stores calibration parameters into EEPROM
 * Note: none
 ********************************************************************/
void TouchStoreCalibration(void);

/*********************************************************************
 * Function: void TouchLoadCalibration(void)
 * PreCondition: EEPROMInit() must be called before
 * Input: none
 * Output: none
 * Side Effects: none
 * Overview: loads calibration parameters from EEPROM
 * Note: none
 ********************************************************************/
void TouchLoadCalibration(void);

/*********************************************************************
 * Macros: ADCGetX()
 * PreCondition: none
 * Input: none
 * Output: ADC result
 * Side Effects: none
 * Overview: returns ADC value for x direction if touch screen is pressed
 *           and -1 if not
 * Note: none
 ********************************************************************/
//#define ADCGetX()   adcX

/*********************************************************************
 * Macros: ADCGetY()
 * PreCondition: none
 * Input: none
 * Output: ADC result
 * Side Effects: none
 * Overview: returns ADC value for y direction if touch screen is pressed
 *           and -1 if not
 * Note: none
 ********************************************************************/
//#define ADCGetY()   adcY

/*********************************************************************
 * Macros: ADCGetPot()
 * PreCondition: none
 * Input: none
 * Output: ADC result
 * Side Effects: none
 * Overview: returns ADC value for potentiometer
 * Note: none
 ********************************************************************/
//#define ADCGetPot() adcPot

void TouchWaitRelease(void);
UINT8 TouchReadSPI(void);

// macro to draw repeating text
#define TouchShowMessage(pStr, color, x, y)             \
                {                                       \
                    SetColor(color);                    \
                    while(!OutTextXY(x,y,pStr));        \
                }

// Default calibration points
#define TOUCHCAL_ULX 0x2A
#define TOUCHCAL_ULY 0xE67E
#define TOUCHCAL_URX 0x2A
#define TOUCHCAL_URY 0xE67E
#define TOUCHCAL_LLX 0x2A
#define TOUCHCAL_LLY 0xE67E
#define TOUCHCAL_LRX 0x2A
#define TOUCHCAL_LRY 0xE67E

#endif
