/*****************************************************************************
 *
 * ADS7843 SPI touch screen driver
 *
 *****************************************************************************
 * FileName:        TouchScreenADS7843.c
 * Dependencies:    TouchScreenADS7843.h
 * Processor:       PIC24, PIC32, dsPIC, PIC24H
 * Compiler:       	MPLAB C30, MPLAB C32
 * Linker:          MPLAB LINK30, MPLAB LINK32
 * Company:         EasyMeter Srl
 *
 * Software License Agreement
 *
 * Copyright (c) 2010 EasyMeter Srl.  All rights reserved.
 * Author               Date        Comment
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Fabio Violino		09/07/2010	Prima stesura dopo 12gg di sofferenza...
 *****************************************************************************/
#include "TouchScreenADS7843.h"
#include "Compiler.h"
#include "GenericTypeDefs.h"
//#include "OutTextXYchars.h"
//#include "BeepPwm.h"
#include "TimeDelay.h"

#ifdef USE_TOUCHSCREEN
//#include <stdarg.h>
#ifdef USE_EEPROM_EMU
#include "DEE Emulation 16-bit/DEE Emulation 16-bit.h"
#endif

#define CALIBRATION_DELAY   300				                // delay between calibration touch points
#define SAMPLE_POINTS   4

volatile SHORT xRawTouch[SAMPLE_POINTS] = {TOUCHCAL_ULX, TOUCHCAL_URX, TOUCHCAL_LRX, TOUCHCAL_LLX};
volatile SHORT yRawTouch[SAMPLE_POINTS] = {TOUCHCAL_ULY, TOUCHCAL_URY, TOUCHCAL_LRY, TOUCHCAL_LLY};

// coefficient values
volatile long _trA;
volatile long _trB;
volatile long _trC;
volatile long _trD;

// WARNING: Watch out when selecting the value of SCALE_FACTOR
// since a big value will overflow the signed int type
// and the multiplication will yield incorrect values.
#ifndef TOUCHSCREEN_RESISTIVE_CALIBRATION_SCALE_FACTOR
// default scale factor is 256
#define TOUCHSCREEN_RESISTIVE_CALIBRATION_SCALE_FACTOR 8
#endif

// use this scale factor to avoid working in floating point numbers
#define SCALE_FACTOR (1<<TOUCHSCREEN_RESISTIVE_CALIBRATION_SCALE_FACTOR)

//////////////////////// GUI Color Assignments ///////////////////////
// Set the colors used in the calibration screen, defined by
// GraphicsConfig.h or gfxcolors.h
#if (COLOR_DEPTH == 1)
#define RESISTIVETOUCH_FOREGROUNDCOLOR BLACK
#define RESISTIVETOUCH_BACKGROUNDCOLOR WHITE
#elif (COLOR_DEPTH == 4)
#define RESISTIVETOUCH_FOREGROUNDCOLOR BLACK
#define RESISTIVETOUCH_BACKGROUNDCOLOR WHITE
#elif (COLOR_DEPTH == 8) || (COLOR_DEPTH == 16) || (COLOR_DEPTH == 24)
#define RESISTIVETOUCH_FOREGROUNDCOLOR BRIGHTRED
#define RESISTIVETOUCH_BACKGROUNDCOLOR WHITE
#endif

// Default Calibration Inset Value (percentage of vertical or horizontal resolution)
// Calibration Inset = ( CALIBRATIONINSET / 2 ) % , Range of 0�20% with 0.5% resolution
// Example with CALIBRATIONINSET == 20, the calibration points are measured
// 10% from the corners.
#ifndef CALIBRATIONINSET
#define CALIBRATIONINSET   20       // range 0 <= CALIBRATIONINSET <= 40
#endif

#define CAL_X_INSET    (((GetMaxX()+1)*(CALIBRATIONINSET>>1))/100)
#define CAL_Y_INSET    (((GetMaxY()+1)*(CALIBRATIONINSET>>1))/100)
#define SAMPLE_POINTS   4

//////////////////////// Resistive Touch Driver Version ////////////////////////////
// The first four bits is the calibration inset, next 8 bits is assigned the version
// number and 0xF is assigned to this 4-wire resistive driver.
const WORD mchpTouchScreenVersion = 0xF110 | CALIBRATIONINSET;
//////////////////////// LOCAL PROTOTYPES ////////////////////////////
void TouchGetCalPoints(void);

#define WAIT_UNTIL_FINISH(x)    while(!x)

typedef struct {
    UINT16 XSample; //@field X Coordinate.
    UINT16 YSample; //@field Y Coordinate.

} TpPointSample;

//#define TOUCHPANEL_NUMBER_SAMPLES_PER_POINT 4
static TpPointSample TpPointSamples[TOUCHPANEL_NUMBER_SAMPLES_PER_POINT];
static UINT8 TpNumSample;
static UINT8 TpProcessing = 0;

//////////////////////// GLOBAL VARIABLES ////////////////////////////
//#define PRESS_THRESHOULD    0x02// between 0-0x03ff the lesser this value the lighter the screen must be pressed
volatile UINT16 TP_PressThreshold = 0x300; //50;

// Max/Min ADC values for each derection
volatile UINT16 _calXMin = XMINCAL;
volatile UINT16 _calXMax = XMAXCAL;
volatile UINT16 _calYMin = YMINCAL;
volatile UINT16 _calYMax = YMAXCAL;

// Current "ADC" values for X and Y channels
volatile INT16 adcX = -1;
volatile INT16 adcY = -1;
volatile UINT8 TpTouched = 0; // TP touched flag

#ifdef USE_HWTP_IC

typedef enum {
    IDLE,
    START_READ,
    GET_X1,
    GET_X2,
    GET_Y1,
    GET_Y2,
} TOUCH_STATES;
volatile TOUCH_STATES state = IDLE;

#elif defined(USE_INTERNAL_ADC_FOR_TP)

typedef enum {
    SET_THRESHOLD,
    GET_THRESHOLD,
    SET_X,
    RUN_X,
    GET_X,
    RUN_CHECK_X,
    CHECK_X,
    SET_Y,
    RUN_Y,
    GET_Y,
    CHECK_Y,
    SET_VALUES
} TOUCH_STATES;

volatile TOUCH_STATES state = SET_THRESHOLD; //SET_X;
#endif

/*********************************************************************
 * Function: void TouchInit(void *initValues)
 * PreCondition: none
 * Input: none
 * Output: none
 * Side Effects: none
 * Overview: Open SPIx channel to communicate with ADS7843
 * Note: none
 ********************************************************************/
void TouchHardwareInit(void *initValues) {

#ifndef PIN_OUTPUT
#define PIN_OUTPUT 0
#define PIN_INPUT 1
#endif

#ifdef USE_HWTP_IC
    HWTP_CS_LAT = 1; // ADS7843 !CS disable
    HWTP_CS_TRIS = PIN_OUTPUT; // CS pin as output

    HWTP_INT_TRIS = PIN_INPUT; // TP_INT pin as Input
#ifdef HW_TP_SPI_INIT
#if !defined(HW_TP_USE_HARDWARE_SPI)
    HWTP_SPI_SDO = 0;
    HWTP_SPI_SDO_TRIS = PIN_OUTPUT;
    HWTP_SPI_SDI = 0;
    HWTP_SPI_SDI_TRIS = PIN_INPUT;
    HWTP_SPI_SCK = 0;
    HWTP_SPI_SCK_TRIS = PIN_OUTPUT;
#else
#if defined(__dsPIC33F__) || defined(__PIC24H__)
    HWTP_SD_SPI_OPEN(ENABLE_SCK_PIN & // Use SCK pin
            ENABLE_SDO_PIN & // SDO pin is used by SPI
            SPI_MODE16_OFF & // xmt/rcv 8 bits at a time
            SPI_SMP_OFF & // sample input data at middle of data output time
            SPI_CKE_OFF & // output data changes @ idle to active clock cycle
            SLAVE_ENABLE_OFF & // SS pin not used by SPI
            CLK_POL_ACTIVE_LOW & // idle clock state is high, active low
            MASTER_ENABLE_ON & // SPI is master
            SEC_PRESCAL_2_1 & // secondary prescale 2:1
            PRI_PRESCAL_4_1, // primary prescale @ 64:1
            FRAME_ENABLE_OFF &
            FIFO_BUFFER_DISABLE, // no framing
            SPI_ENABLE & // enable SPI
            SPI_IDLE_STOP & // stop on idle
            SPI_RX_OVFLOW_CLR & // reset spi rx overflow
            FIFO_BUF_LEN_1);
#elif defined(__PIC32MX__)
    OpenSPI2(
            PRI_PRESCAL_4_1 & // primary prescale @ 64:1
            SEC_PRESCAL_2_1 & // secondary prescale 2:1
            MASTER_ENABLE_ON & // SPI is master
            SLAVE_ENABLE_OFF & // SS pin not used by SPI
            SPI_CKE_OFF & // output data changes @ idle to active clock cycle
            SPI_SMP_OFF & // sample input data at middle of data output time
            SPI_MODE16_OFF & // xmt/rcv 8 bits at a time
            ENABLE_SDO_PIN & // SDO pin is used by SPI
            FRAME_ENABLE_OFF &
            CLK_POL_ACTIVE_LOW & // idle clock state is high, active low
            ENABLE_SCK_PIN, // Use SCK pin

            SPI_ENABLE & // enable SPI
            SPI_IDLE_STOP & // stop on idle
            SPI_RX_OVFLOW_CLR & // reset spi rx overflow
            FIFO_BUFFER_DISABLE // no framing
            );
#endif
    //	HWTP_SPI_SPIBUF = 0x00;		// void read byte

#endif  // SOFT_SPI
#endif  // HW_TP_SPI_INIT
#elif defined(USE_INTERNAL_ADC_FOR_TP)
    // Initialize ADC
    AD1CON1 = 0x080E0; // Turn on, auto-convert
    AD1CON2 = 0; // AVdd, AVss, int every conversion, MUXA only
    //AD1CON3 = 0x1FFF; // 31 Tad auto-sample, Tad = 256*Tcy
    AD1CON3 = 0x1F3F; // 31 Tad auto-sample, Tad = 64*Tcy
#if defined(__dsPIC33F__) || defined(__PIC24H__)
    AD1PCFGL = 0; // All inputs are analog
    AD1PCFGLbits.PCFG11 = AD1PCFGLbits.PCFG12 = 1;
#else
#if !(defined(__PIC24FJ256DA210__) || defined(__PIC24FJ256GB210__))
    AD1PCFG = 0xffff; // All inputs are digital
    mAdcXconfig()
    mAdcYconfig()
#endif
#endif
            AD1CSSL = 0; // No scanned inputs

#endif

#ifdef USE_EEPROM_EMU
    DataEEInit();
    dataEEFlags.val = 0;
#endif
}

//--------------------------------------------------------------
// Function: evaluateSample
//
// Purpose:  Each time
//       on a pen down, 4 pairs XY from the ADC are collected and
//       3 of them will be used here. This function implements
//       the algorithm to the best sample from 3 pairs of samples
//       by discarding one that is too way out and take a mean of
//       the rest two.
//
// Returns:  TouchSampleValidFlag and average of the 2 good samples
//--------------------------------------------------------------

//void evaluateSample(UINT16 val0, UINT16 val1, UINT16 val2, INT16 *sample) {
//    UINT32 diff0, diff1, diff2;
//    // Calculate the absolute value of the differences of the sample
//    diff0 = val0 - val1;
//    diff1 = val1 - val2;
//    diff2 = val2 - val0;
//    diff0 = diff0 > 0 ? diff0 : -diff0;
//    diff1 = diff1 > 0 ? diff1 : -diff1;
//    diff2 = diff2 > 0 ? diff2 : -diff2;
//
//    // Eliminate the one away from other two and add the two
//    if (diff0 < diff1)
//        *sample = (UINT16) (val0 + ((diff2 < diff0) ? val2 : val1));
//    else
//        *sample = (UINT16) (val2 + ((diff2 < diff1) ? val0 : val1));
//
//    // Get the average of the two good samples
//    *sample >>= 1;
//}

#if !defined(HW_TP_USE_HARDWARE_SPI)

/*********************************************************************
 * Function:         void SPIPut(BYTE v)
 * PreCondition:     none
 * Input:	    v - is the byte that needs to be transfered
 * Output:	    none
 * Side Effects:	    SPI transmits the byte
 * Overview:	    This function will send a byte over the SPI
 * Note:		    None
 ********************************************************************/
void BitBangSPIPut(BYTE v) {
    BYTE i;
    HWTP_SPI_SDO = 0;
    HWTP_SPI_SCK = 0;

    for (i = 0; i < 8; i++) {
        HWTP_SPI_SDO = (v >> (7 - i));
        HWTP_SPI_SCK = 1;
        Nop();
        Nop();
        HWTP_SPI_SCK = 0;
    }
    HWTP_SPI_SDO = 0;
}

/*********************************************************************
 * Function:         BYTE SPIGet(void)
 * PreCondition:     SPI has been configured
 * Input:            none
 * Output:           BYTE - the byte that was last received by the SPI
 * Side Effects:	    none
 * Overview:         This function will read a byte over the SPI
 * Note:	            None
 ********************************************************************/
BYTE BitBangSPIGet(void) {
    BYTE i;
    BYTE spidata = 0;

    HWTP_SPI_SDO = 0;
    HWTP_SPI_SCK = 0;

    for (i = 0; i < 8; i++) {
        spidata = (spidata << 1) | HWTP_SPI_SDI;
        HWTP_SPI_SCK = 1;
        Nop();
        Nop();
        HWTP_SPI_SCK = 0;
    }
    return spidata;
}
#endif // !defined(HW_TP_USE_HARDWARE_SPI)

/*********************************************************************
 * Function: void TouchProcessTouch(void)
 * PreCondition: none
 * Input: none
 * Output: 1 if it has completed the reading cycle, otherwise 0
 * Side Effects: none
 * Overview: Polls ADS7843 to read Y and X coordinates
 * Note: none
 ********************************************************************/
UINT8 TouchDetectPosition(void) {
#ifdef USE_HWTP_IC
    BYTE control;
    UINT16 byteRead;
    //	char charString[80];
    //	XCHAR xcharString[80];
    //	int i;
    if (TpProcessing != 0)
        return (0);
    TpProcessing = 1;
    if (HWTP_INT) { // the screen is not touched
        adcY = -1;
        adcX = -1;
        state = IDLE;
    } else {
        switch (state) {
            case IDLE:
#if defined DEBUG
                LED_IO_TRIS = 0;
                LED_IO ^= 1;
#endif
                state = START_READ;
                TpNumSample = 0;

            case START_READ:
                HWTP_CS_LAT = 0; // ADS7843 !CS enable

                //Send the control byte to read Y coord in 12bit notation
#if (HWTP_IC==TSC2046)
                control = 0b10010000; // 1 101=Y 0=12bit 0=DFR 00=PowerDown+PenIrqDisabled
#else
                control = 0b11010000; // 1 101=Y 0=12bit 0=DFR 00=PowerDown+PenIrqEnabled
#endif

#if !defined(HW_TP_USE_HARDWARE_SPI)
                mHWTP_SpiPut(control);
#else
                HWTP_SPI_WRITE(control);
#endif
                state = GET_Y1;
                break;

            case GET_Y1:
                // read 1st byte for Y coord  (first 8 bits of 12)
                byteRead = TouchReadSPI();
                TpPointSamples[TpNumSample].YSample = ((int) byteRead) << 4;
                state = GET_Y2;
                break;

            case GET_Y2:
                // read 2nd byte for Y coord  (last 4 bits of 12)
                byteRead = TouchReadSPI();
                //TouchReadSPI();

                TpPointSamples[TpNumSample].YSample += (byteRead >> 4) & 0x0F;

                if (HWTP_INT) { // the screen is not touched
                    adcY = -1;
                    adcX = -1;
                    state = IDLE;
                    HWTP_CS_LAT = 1; // ADS7843 !CS disable
                    break;
                }
                //Send the control byte to read X coord in 12bit notation
#if (HWTP_IC==TSC2046)
                control = 0b11010000; // 1 001=X 0=12bit 0=DFR 00=PowerDown+PenIrqDisabled
#else
                control = 0b10010000; // 1 001=X 0=12bit 0=DFR 00=PowerDown+PenIrqEnabled
#endif

#if !defined(HW_TP_USE_HARDWARE_SPI)
                mHWTP_SpiPut(control);
#else
                HWTP_SPI_WRITE(control);
#endif
                state = GET_X1;
                break;

            case GET_X1:
                // read 1st byte for X coord  (first 8 bits of 12)
                byteRead = TouchReadSPI();
                TpPointSamples[TpNumSample].XSample = ((int) byteRead) << 4;
                state = GET_X2;
                break;

            case GET_X2:
                // read 2nd byte for X coord  (last 4 bits of 12)
                byteRead = TouchReadSPI();
                TpPointSamples[TpNumSample].XSample += (byteRead >> 4) & 0x0F;
                //TouchReadSPI();
                if (++TpNumSample >= TOUCHPANEL_NUMBER_SAMPLES_PER_POINT) {
                    BYTE i;
                    for (i = 0; i < TOUCHPANEL_NUMBER_SAMPLES_PER_POINT; i++) {
                        adcX = (adcX + TpPointSamples[i].XSample) >> 1;
                        adcY = (adcY + TpPointSamples[i].YSample) >> 1;
                    }
                    //                    evaluateSample(TpPointSamples[0].XSample, TpPointSamples[1].XSample, TpPointSamples[2].XSample, (INT16 *) & adcX);
                    //                    evaluateSample(TpPointSamples[0].YSample, TpPointSamples[1].YSample, TpPointSamples[2].YSample, (INT16 *) & adcY);
                    state = IDLE;
                    HWTP_CS_LAT = 1; // ADS7843 !CS disable
                    TpProcessing = 0;
                    return (1); // Reading complete!
                } else {
                    if (HWTP_INT) { // the screen is not touched
                        adcY = -1;
                        adcX = -1;
                        state = IDLE;
                        HWTP_CS_LAT = 1; // ADS7843 !CS disable
                        break;
                    }
                    state = START_READ;
                }
        }
        //HWTP_CS_LAT = 1; // ADS7843 !CS disable
    }
#elif defined USE_INTERNAL_ADC_FOR_TP
    /*
    1) SET_X
    z XPOS
    z YPOS
    z XNEG
    0 YNEG
    campiona XPOS

    2) CHECK_X
    1 YPOS

    3) RUN_X
    campiona XPOS

    4) GET_X
    legge XPOS in tempX
    z YPOS
    campiona XPOS

    5) SET_Y
    z XPOS
    z YPOS
    0 XNEG
    z YNEG
    campiona YPOS

    6) CHECK_Y
    1 XPOS

    7) RUN_Y
    campiona YPOS

    8) GET_Y
    legge YPOS in tempY
    z XPOS
    campiona YPOS

    9) SET_VALUES
     */
    static INT16 tempX, tempY;
    volatile INT16 temp;

    switch (state) {

        case SET_VALUES:
            if (!TOUCH_ADC_DONE)
                break;

            state = SET_X;
            if ((((WORD) ADC1BUF0 > (WORD) TP_PressThreshold)) ||
                    (tempX == 0 || tempY == 0 || tempX == 1023 || tempY == 1023)) {
                adcX = -1;
                adcY = -1;
                //if (TpTouched) {
                TpTouched = 0;
                return (1); // change
                //} else {
                //    return (0); // no change
                //}
            } else {
                adcX = tempX;
                adcY = tempY;
                TpTouched = 1;
                return (1);
            }

        case SET_THRESHOLD:
        case SET_X:
#if defined(__dsPIC33F__) || defined(__PIC24H__)
            AD1CHS0 = ADC_XPOS; // switch ADC channel
#else
            AD1CHS = ADC_XPOS; // switch ADC channel
#endif
            TRIS_XPOS = 1;
            TRIS_YPOS = 1;
            TRIS_XNEG = 1;
            LAT_YNEG = 0;
            TRIS_YNEG = 0;

            AD1CON1bits.SAMP = 1; // run conversion
            state = (state == SET_X) ? CHECK_X : GET_THRESHOLD;
            break;

        case GET_THRESHOLD:
        case CHECK_X:
        case CHECK_Y:
            if (!TOUCH_ADC_DONE)
                break;

            if (state == GET_THRESHOLD) {
                state = SET_X;
                TP_PressThreshold = ADC1BUF0 >> 1; //2
                break;
            }

            if ((WORD) ADC1BUF0 > (WORD) TP_PressThreshold) {
                adcX = -1;
                adcY = -1;
                state = SET_X;
                TpTouched = 0;
                break;
            } else {
                if (state == CHECK_X) {
                    LAT_YPOS = 1;
                    TRIS_YPOS = 0;
                    tempX = -1;
                    state = RUN_X;
                } else {
                    LAT_XPOS = 1;
                    TRIS_XPOS = 0;
                    tempY = -1;
                    state = RUN_Y;
                }
                break;
            }

        case RUN_X:
        case RUN_Y:
            AD1CON1bits.SAMP = 1;
            state = (state == RUN_X) ? GET_X : GET_Y;
            break;

        case GET_X:
        case GET_Y:
            if (!TOUCH_ADC_DONE)
                break;

            temp = ADC1BUF0;
            if (state == GET_X) {
                if (temp != tempX) {
                    tempX = temp;
                    state = RUN_X;
                    break;
                }
            } else {
                if (temp != tempY) {
                    tempY = temp;
                    state = RUN_Y;
                    break;
                }
            }

            if (state == GET_X)
                TRIS_YPOS = 1;
            else
                TRIS_XPOS = 1;
            AD1CON1bits.SAMP = 1;
            state = (state == GET_X) ? SET_Y : SET_VALUES;
            break;

        case SET_Y:
            if (!TOUCH_ADC_DONE)
                break;

            if ((WORD) ADC1BUF0 > (WORD) TP_PressThreshold) {
                adcX = -1;
                adcY = -1;
                state = SET_X;
                TpTouched = 0;
                break;
            }

#if defined(__dsPIC33F__) || defined(__PIC24H__)
            AD1CHS0 = ADC_YPOS; // switch ADC channel
#else
            AD1CHS = ADC_YPOS; // switch ADC channel
#endif
            TRIS_XPOS = 1;
            TRIS_YPOS = 1;
            LAT_XNEG = 0;
            TRIS_XNEG = 0;
            TRIS_YNEG = 1;

            AD1CON1bits.SAMP = 1; // run conversion
            state = CHECK_Y;
            break;

        default:
            state = SET_X;
    }
#endif
    TpProcessing = 0;
    return (0);
}

/*********************************************************************
 * Function: INT16 TouchGetRawX()
 * PreCondition: none
 * Input: none
 * Output: x coordinate
 * Side Effects: none
 * Overview: returns x ABSOLUTE coordinate if touch screen is pressed
 *           and -1 if not
 * Note: none
 ********************************************************************/
INT16 TouchGetRawX(void) {
    INT16 result;

#ifdef TOUCHSCREEN_RESISTIVE_SWAP_XY
#ifdef USE_HWTP_IC
    if (HWTP_INT) adcY = -1; // Don't invalidate if still touching
#endif
    result = adcY;
#else
#ifdef USE_HWTP_IC
    if (HWTP_INT) adcX = -1; // Don't invalidate if still touching
#endif
    result = adcX;
#endif
    return (result);
}

/*********************************************************************
 * Function: INT16 TouchGetRawX()
 * PreCondition: none
 * Input: none
 * Output: y coordinate
 * Side Effects: none
 * Overview: returns ABSOLUTE y coordinate if touch screen is pressed
 *           and -1 if not
 * Note: none
 ********************************************************************/
INT16 TouchGetRawY(void) {
    INT16 result;

#ifdef TOUCHSCREEN_RESISTIVE_SWAP_XY
#ifdef USE_HWTP_IC
    if (HWTP_INT) adcX = -1; // Don't invalidate if still touching
#endif
    result = adcX;
#else
#ifdef USE_HWTP_IC
    if (HWTP_INT) adcY = -1; // Don't invalidate if still touching
#endif
    result = adcY;
#endif
    return (result);
}

/*********************************************************************
 * Function: INT16 TouchGetX()
 * PreCondition: none
 * Input: none
 * Output: x coordinate
 * Side Effects: none
 * Overview: returns x coordinate if touch screen is pressed
 *           and -1 if not
 * Note: none
 ********************************************************************/
INT16 TouchGetX(void) {
    INT16 result = TouchGetRawX();
    if (result >= 0) {
#ifdef TOUCHSCREEN_RESISTIVE_SWAP_XY
        result = (GetMaxX() * (result - _calYMin)) / (_calYMax - _calYMin);
#else
        result = (GetMaxX() * (result - _calXMin)) / (_calXMax - _calXMin);
#endif
#ifdef TOUCHSCREEN_RESISTIVE_FLIP_X
        result = GetMaxX() - result;
#endif
    }
    return (result);
}

/*********************************************************************
 * Function: INT16 TouchGetY()
 * PreCondition: none
 * Input: none
 * Output: y coordinate
 * Side Effects: none
 * Overview: returns y coordinate if touch screen is pressed
 *           and -1 if not
 * Note: none
 ********************************************************************/
INT16 TouchGetY(void) {
    INT16 result = TouchGetRawY();

    if (result >= 0) {
#ifdef TOUCHSCREEN_RESISTIVE_SWAP_XY
        result = (GetMaxY() * (result - _calXMin)) / (_calXMax - _calXMin);
#else
        result = (GetMaxY() * (result - _calYMin)) / (_calYMax - _calYMin);
#endif
#ifdef TOUCHSCREEN_RESISTIVE_FLIP_Y
        result = GetMaxY() - result;
#endif
    }
    return (result);
}

#ifdef USE_HWTP_IC

/*********************************************************************
 * Function: UINT8 TouchReadSPI(void)
 * PreCondition: none
 * Input: none
 * Output: none
 * Side Effects: none
 * Overview: Reads 1 byte from ADS7843
 * Note: none
 ********************************************************************/
UINT8 TouchReadSPI(void) {
    UINT8 buffer;
#if !defined(HW_TP_USE_HARDWARE_SPI)
    buffer = mHWTP_SpiGet();
#else
    //if(!HWTP_SPI_SPIRBF) HWTP_SPI_SPIBUF = 0x00;	// If RX buffer not already full, write to the RX buffer to generate 8 clock pulses
    if (HWTP_SPI_SPIRBF) {
        HWTP_SPI_SPIROV = 0;
        buffer = (HWTP_SPI_SPIBUF & 0xff); //			buffer=ReadSPI1();
    } else {
        buffer = -1;
    }
#endif
    return (buffer);
}
#endif

/*********************************************************************
 * Function: void TouchStoreCalibration(void)
 * PreCondition: EEPROMInit() must be called before
 * Input: none
 * Output: none
 * Side Effects: none
 * Overview: stores calibration parameters into EEPROM
 * Note: none
 ********************************************************************/
void TouchStoreCalibration(void) {
#if defined(USE_EEPROM_EMU)
    DataEEWrite((unsigned int) _calXMin, ADDRESS_XMIN);
    DataEEWrite((unsigned int) _calXMax, ADDRESS_XMAX);
    DataEEWrite((unsigned int) _calYMin, ADDRESS_YMIN);
    DataEEWrite((unsigned int) _calYMax, ADDRESS_YMAX);
    DataEEWrite(GRAPHICS_LIBRARY_VERSION, ADDRESS_VERSION);
#elif defined(GFX_PICTAIL_V2)
    EEPROMWriteWord(_calXMin, ADDRESS_XMIN);
    EEPROMWriteWord(_calXMax, ADDRESS_XMAX);
    EEPROMWriteWord(_calYMin, ADDRESS_YMIN);
    EEPROMWriteWord(_calYMax, ADDRESS_YMAX);
    EEPROMWriteWord(GRAPHICS_LIBRARY_VERSION, ADDRESS_VERSION);
    /*    #else
        SST25SectorErase(ADDRESS_XMIN); // erase 4K sector
        SST25WriteWord(_calXMin, ADDRESS_XMIN);
        SST25WriteWord(_calXMax, ADDRESS_XMAX);
        SST25WriteWord(_calYMin, ADDRESS_YMIN);
        SST25WriteWord(_calYMax, ADDRESS_YMAX);
        SST25WriteWord(GRAPHICS_LIBRARY_VERSION, ADDRESS_VERSION);*/
#endif
}

/*********************************************************************
 * Function: void TouchLoadCalibration(void)
 * PreCondition: EEPROMInit() must be called before
 * Input: none
 * Output: none
 * Side Effects: none
 * Overview: loads calibration parameters from EEPROM
 * Note: none
 ********************************************************************/
void TouchLoadCalibration(void) {
#if defined(USE_EEPROM_EMU)
    _calXMin = DataEERead(ADDRESS_XMIN);
    _calXMax = DataEERead(ADDRESS_XMAX);
    _calYMin = DataEERead(ADDRESS_YMIN);
    _calYMax = DataEERead(ADDRESS_YMAX);
#elif defined (GFX_PICTAIL_V2)
    _calXMin = EEPROMReadWord(ADDRESS_XMIN);
    _calXMax = EEPROMReadWord(ADDRESS_XMAX);
    _calYMin = EEPROMReadWord(ADDRESS_YMIN);
    _calYMax = EEPROMReadWord(ADDRESS_YMAX);
    /*#else
    _calXMin = SST25ReadWord(ADDRESS_XMIN);
    _calXMax = SST25ReadWord(ADDRESS_XMAX);
    _calYMin = SST25ReadWord(ADDRESS_YMIN);
    _calYMax = SST25ReadWord(ADDRESS_YMAX);*/
#endif
}

#ifdef DEBUG

void ShowCoords(INT16 x, INT16 y, INT16 x1, INT16 y1) {
    static INT16 oldX, oldY, oldX1, oldY1;
    char charString[80];
    if (x != oldX || y != oldY || x1 != oldX1 || y1 != oldY1) {
        SetColor(WHITE);
        Bar(81, 1, GetMaxX(), 20);
        sprintf(charString, " T=%d X=%4d Y=%4d X1=%4d Y1=%4d TS=%d", TpTouched, x, y, x1, y1, TP_PressThreshold);
        TouchShowMessage(charString, BLACK, 81, 1);
        oldX = x;
        oldY = y;
        oldX1 = x1;
        oldY1 = y1;
    }
}
#endif

/*********************************************************************
 * Function: void TouchGetCalPoints(void)
 *
 * PreCondition: InitGraph() must be called before
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: gets values for 3 touches
 *
 * Note: none
 *
 ********************************************************************/
//void TouchCalculateCalPoints(WORD *xRawTouch, WORD *yRawTouch, WORD *xPoint, WORD *yPoint)

void TouchCalculateCalPoints(void) {
    long trA, trB, trC, trD; // variables for the coefficients
    long trAhold, trBhold, trChold, trDhold;
    long test1, test2; // temp variables (must be signed type)

    SHORT xPoint[SAMPLE_POINTS], yPoint[SAMPLE_POINTS];

    yPoint[0] = yPoint[1] = CAL_Y_INSET;
    yPoint[2] = yPoint[3] = (GetMaxY() - CAL_Y_INSET);
    xPoint[0] = xPoint[3] = CAL_X_INSET;
    xPoint[1] = xPoint[2] = (GetMaxX() - CAL_X_INSET);

    // calculate points transfer functiona
    // based on two simultaneous equations solve for the
    // constants

    // use sample points 1 and 4
    // Dy1 = aTy1 + b; Dy4 = aTy4 + b
    // Dx1 = cTx1 + d; Dy4 = aTy4 + b

    test1 = (long) yPoint[0] - (long) yPoint[3];
    test2 = (long) yRawTouch[0] - (long) yRawTouch[3];

    trA = ((long) ((long) test1 * SCALE_FACTOR) / test2);
    trB = ((long) ((long) yPoint[0] * SCALE_FACTOR) - (trA * (long) yRawTouch[0]));

    test1 = (long) xPoint[0] - (long) xPoint[2];
    test2 = (long) xRawTouch[0] - (long) xRawTouch[2];

    trC = ((long) ((long) test1 * SCALE_FACTOR) / test2);
    trD = ((long) ((long) xPoint[0] * SCALE_FACTOR) - (trC * (long) xRawTouch[0]));

    trAhold = trA;
    trBhold = trB;
    trChold = trC;
    trDhold = trD;

    // use sample points 2 and 3
    // Dy2 = aTy2 + b; Dy3 = aTy3 + b
    // Dx2 = cTx2 + d; Dy3 = aTy3 + b

    test1 = (long) yPoint[1] - (long) yPoint[2];
    test2 = (long) yRawTouch[1] - (long) yRawTouch[2];

    trA = ((long) (test1 * SCALE_FACTOR) / test2);
    trB = ((long) ((long) yPoint[1] * SCALE_FACTOR) - (trA * (long) yRawTouch[1]));

    test1 = (long) xPoint[1] - (long) xPoint[3];
    test2 = (long) xRawTouch[1] - (long) xRawTouch[3];

    trC = ((long) ((long) test1 * SCALE_FACTOR) / test2);
    trD = ((long) ((long) xPoint[1] * SCALE_FACTOR) - (trC * (long) xRawTouch[1]));

    // get the average and use the average
    _trA = (trA + trAhold) >> 1;
    _trB = (trB + trBhold) >> 1;
    _trC = (trC + trChold) >> 1;
    _trD = (trD + trDhold) >> 1;

}

/*********************************************************************
 * Function: void TouchGetCalPoints(void)
 *
 * PreCondition: InitGraph() must be called before
 *
 * Input: none
 *
 * Output: none
 *
 * Side Effects: none
 *
 * Overview: gets values for 3 touches
 *
 * Note: none
 *
 ********************************************************************/
void TouchCalHWGetPoints(void) {
#define TOUCH_DIAMETER	10
#define SAMPLE_POINTS   4

    XCHAR calStr1[] = {'o', 'n', ' ', 't', 'h', 'e', ' ', 'f', 'i', 'l', 'l', 'e', 'd', 0};
    XCHAR calStr2[] = {'c', 'i', 'r', 'c', 'l', 'e', 0};
    XCHAR calTouchPress[] = {'P', 'r', 'e', 's', 's', ' ', '&', ' ', 'R', 'e', 'l', 'e', 'a', 's', 'e', 0};

    XCHAR calRetryPress[] = {'R', 'e', 't', 'r', 'y', 0};
    XCHAR *pMsgPointer;
    SHORT counter;

    WORD dx[SAMPLE_POINTS], dy[SAMPLE_POINTS];
    WORD textHeight, msgX, msgY;
    SHORT tempX, tempY;

    SetFont((void *) &FONTDEFAULT);
    SetColor(RESISTIVETOUCH_FOREGROUNDCOLOR);

    textHeight = GetTextHeight((void *) &FONTDEFAULT);

    while
        (
            !OutTextXY
            (
            (GetMaxX() - GetTextWidth((XCHAR *) calStr1, (void *) &FONTDEFAULT)) >> 1,
            (GetMaxY() >> 1),
            (XCHAR *) calStr1
            )
            );

    while
        (
            !OutTextXY
            (
            (GetMaxX() - GetTextWidth((XCHAR *) calStr2, (void *) &FONTDEFAULT)) >> 1,
            ((GetMaxY() >> 1) + textHeight),
            (XCHAR *) calStr2
            )
            );

    // calculate center points (locate them at 15% away from the corners)
    // draw the four touch points

    dy[0] = dy[1] = CAL_Y_INSET;
    dy[2] = dy[3] = (GetMaxY() - CAL_Y_INSET);
    dx[0] = dx[3] = CAL_X_INSET;
    dx[1] = dx[2] = (GetMaxX() - CAL_X_INSET);


    msgY = ((GetMaxY() >> 1) - textHeight);
    pMsgPointer = calTouchPress;

    // get the four samples or calibration points
    for (counter = 0; counter < SAMPLE_POINTS;) {

        // redraw the filled circle to unfilled (previous calibration point)
        if (counter > 0) {
            SetColor(RESISTIVETOUCH_BACKGROUNDCOLOR);
            while (!(FillCircle(dx[counter - 1], dy[counter - 1], TOUCH_DIAMETER - 3)));
        }

        // draw the new filled circle (new calibration point)
        SetColor(RESISTIVETOUCH_FOREGROUNDCOLOR);
        while (!(Circle(dx[counter], dy[counter], TOUCH_DIAMETER)));
        while (!(FillCircle(dx[counter], dy[counter], TOUCH_DIAMETER - 3)));

        // show points left message
        msgX = (GetMaxX() - GetTextWidth((XCHAR *) pMsgPointer, (void *) &FONTDEFAULT)) >> 1;
        TouchShowMessage(pMsgPointer, RESISTIVETOUCH_FOREGROUNDCOLOR, msgX, msgY);

        // Wait for press
        do {
        } while ((TouchGetRawX() == -1) && (TouchGetRawY() == -1));

        tempX = TouchGetRawX();
        tempY = TouchGetRawY();

        // wait for release
        do {
        } while ((TouchGetRawX() != -1) && (TouchGetRawY() != -1));

        // check if the touch was detected properly
        if ((tempX == -1) || (tempY == -1)) {
            // cannot proceed retry the touch, display RETRY PRESS message

            // remove the previous string
            TouchShowMessage(pMsgPointer, RESISTIVETOUCH_BACKGROUNDCOLOR, msgX, msgY);
            pMsgPointer = calRetryPress;
            // show the retry message
            msgX = (GetMaxX() - GetTextWidth((XCHAR *) pMsgPointer, (void *) &FONTDEFAULT)) >> 1;
            TouchShowMessage(pMsgPointer, RESISTIVETOUCH_FOREGROUNDCOLOR, msgX, msgY);
        } else {

            // remove the previous string
            TouchShowMessage(pMsgPointer, RESISTIVETOUCH_BACKGROUNDCOLOR, msgX, msgY);
            pMsgPointer = calTouchPress;



#ifdef TOUCHSCREEN_RESISTIVE_FLIP_Y
            yRawTouch[3 - counter] = tempY; //TouchGetRawY();
#else
            yRawTouch[counter] = tempY; //ouchGetRawY();
#endif

#ifdef TOUCHSCREEN_RESISTIVE_FLIP_X
            xRawTouch[3 - counter] = tempX; //TouchGetRawX();
#else
            xRawTouch[counter] = tempX; //TouchGetRawX();
#endif

            counter++;

        }

        // Wait for release
        do {
        } while ((TouchGetRawX() != -1) && (TouchGetRawY() != -1));

        DelayMs(CALIBRATION_DELAY);

    }

    TouchCalculateCalPoints();

#ifdef ENABLE_DEBUG_TOUCHSCREEN
    TouchScreenResistiveTestXY();
#endif
}

void TouchWaitRelease(void) {
    INT16 x, y;
    // Wait for release
    do {
        x = TouchGetRawX();
        y = TouchGetRawY();
#ifdef DEBUG
        ShowCoords(x, y, TouchGetX(), TouchGetY());
#endif
    } while ((y != -1) && (x != -1));
}

#endif // USE_TOUCHSCREEN
