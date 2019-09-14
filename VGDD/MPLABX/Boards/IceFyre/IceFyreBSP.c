/********************************************************************
 FileName:    	IceFyreBSP.c
 Dependencies: 	none
 Hardware:	IceFyre BETA
 Complier:  	Microchip XC32
 Company:	Vinagron Digital
 Author:	Juan Carlos Orozco Gutierrez

 Software License Agreement:

 This software has been licensed under the GNU General Public
 License is intended to guarantee your freedom to share and change free
 software--to make sure the software is free for all its users.

********************************************************************
 File Description:
    Board Support Package


 Change History:
  Rev   Description                                 Modified by:
  ----  -----------------------------------------  --------------
  1.0   Initial release                             JCOG

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/
#include "HardwareProfile.h"


#if !defined(VGDD_WILL_CONFIG)  //only config here if VGDD is not used
// Configuration bits
#pragma config FPLLMUL  = MUL_20        // PLL Multiplier
#pragma config FPLLIDIV = DIV_2         // PLL Input Divider
#pragma config FPLLODIV = DIV_1         // PLL Output Divider
#pragma config FPBDIV   = DIV_1         // Peripheral Clock divisor
#pragma config FWDTEN   = OFF           // Watchdog Timer
#pragma config WDTPS    = PS1           // Watchdog Timer Postscale
#pragma config FCKSM    = CSDCMD        // Clock Switching & Fail Safe Clock Monitor
#pragma config OSCIOFNC = OFF           // CLKO Enable
#pragma config POSCMOD  = HS            // Primary Oscillator
#pragma config IESO     = OFF           // Internal/External Switch-over
#pragma config FSOSCEN  = OFF           // Secondary Oscillator Enable
#pragma config FNOSC    = PRIPLL        // Oscillator Selection
#pragma config CP       = OFF           // Code Protect
#pragma config BWP      = OFF           // Boot Flash Write Protect
#pragma config PWP      = OFF           // Program Flash Write Protect
#pragma config FCANIO   = OFF           // CAN I/O Pin Select (Alternate CAN I/O)
#pragma config ICESEL   = ICS_PGx1      // ICE/ICD Comm Channel Select
#endif
// End Configuration bits

//*More* Local Defines
#define ADC_MAX_VALUE   1023.0  
#define ADC_REF_VOLTAGE 3.3     

#define BUZZER_SAMPLE_RATE  2000                   //should be ~1 ms
#define FPB                 (GetPeripheralClock()/2)   //all peripherals run at 80MHz
//End local defines


//void BacklightPWM_Init(void);
////////////////////////////////////////////////////////////////////////////////
static void IO_Init(void)
{    
    AD1PCFG = 0xFFFF;   //all pins as digital

    STATUS_LED_1_TRIS   = 0;
    STATUS_LED_2_TRIS   = 0;

    ADDR_DIPSW_1_TRIS   = 1;
    ADDR_DIPSW_2_TRIS   = 1;
    ADDR_DIPSW_3_TRIS   = 1;
    ADDR_DIPSW_4_TRIS   = 1;

    EEPROM_CS_TRIS      = 0;

    STATUS_LED_1_LAT    = 0;
    STATUS_LED_2_LAT    = 0;

    UART_RXD_TRIS       = 1;
    UART_TXD_TRIS       = 0;

    BUZZER_TRIS  = 0;

    MXP_GP1_TRIS = 0;
    MXP_GP2_TRIS = 0;
}

void IceFyre_Init(void)
{
    INTEnableSystemMultiVectoredInt();
    SYSTEMConfigPerformance(GetSystemClock());
    SYSTEMConfig(GetSystemClock(), SYS_CFG_WAIT_STATES | SYS_CFG_PCACHE );

    IO_Init();      //initialize all GPIO
}

BYTE DIPSW_Read(void)
{
    BYTE output = 0;
    
    output  = (PORTB & ADDR_DIPSW_MASK) >> 12;
    output &= 0x0F; //mask it! since DIPSW is only 4 bits wide
    
    return output;
}

static void ADC_Init(void)
{
    TEMPSENS_PIN_TRIS = 1;

    AD1PCFG = 0xFFFB;   //PORTB = Digital; RB2 = analog
    AD1CON1 = 0x0000;   //SAMP bit = 0 ends sampling and starts converting
    AD1CSSL = 0;        //No Scanning
    AD1CON3 = 0x0002;   //Manual Sample, Tad = internal 6 TPB
    AD1CON2 = 0;
    AD1CON1SET = 0x8000;// turn ADC ON
}

static WORD_VAL ReadADC(int channel)
{
    WORD_VAL w;
    static BOOL initADC = TRUE;

    if(initADC){
        ADC_Init();
        initADC = FALSE;
    }

    w.Val = 0;
    AD1CHS = (channel<<16) & 0x00FF0000;    //Connect appropiate pin to ADC input ..
    AD1CON1SET = 0x0002;                    //start sampling ...
    for(w.Val=0;w.Val<1000;w.Val++);        //Sample delay, conversion start automatically
    AD1CON1CLR = 0x0002;                    //Start Converting
    while (!(AD1CON1 & 0x0001));            //Conversion done?

    w.Val = ADC1BUF0;

    return w;
}

float Temperature_Read(BYTE output_scale)
{
    //0-> Celsius, 1->Kelvin, 2->Fahrenheit
    WORD_VAL rawADC;
    float rawTemp   = 0.0;
    float finalTemp = 0.0;

    rawADC.Val = 0;
    rawADC = ReadADC(2);    //get raw ADC value from AN2 (temp sensor)

    /*ADC_REF_VOLTAGE / ADC_MAX_VALUE equals the ADC steps per value*/
    rawTemp = (rawADC.Val) * (ADC_REF_VOLTAGE/ADC_MAX_VALUE);

    rawTemp -= 0.5;     //Calibrate to 0 Celsius
    rawTemp /= 0.01;    //Difference divided by the 10mV/Step resolution

    switch(output_scale){
        case 0: finalTemp = rawTemp;            break;
        case 1: finalTemp = (rawTemp + 273.15); break;
        case 2: //fahrenheit
            rawTemp *= 1.8;
            finalTemp = (rawTemp + 32);
            break;
    }
//    return finalTemp-3.3;   //*TODO:* come up with a fix for the offset
    return finalTemp;   
}

void PWM_Init(BYTE pwm_channel, int period, int duty_cycle)
{
    //Low level PWM driver function.
    //PWM channel -> 1=Backlight, 2=Buzzer, 4=MXP port
    UINT32 localFPB    = 0;
    UINT32 localPeriod = 0;
    UINT32 localDuty   = 0;

    if(period){
        //Only make this operations if a valid period and duty is passed.
        localFPB    = FPB;
        localPeriod = (localFPB/period);
        localDuty   = (localPeriod * duty_cycle) / 100;
        
        PR2 = (localFPB / (period - 1));
        OpenTimer2(T2_ON | T2_PS_1_2 | T2_SOURCE_INT, localPeriod);
        mT2SetIntPriority(5);  // you don't have to use ipl7, but make sure INT definition is the same as your choice here
        mT2ClearIntFlag();     // make sure no int will be pending until x counts from this point.
        mT2IntEnable(1);       // allow T2 int
    }else{
        switch(pwm_channel){
            case 1:
                CloseOC1();
                SetDCOC1PWM(0);      //Not valid parameter so stop PWM
                break;
            case 2:
                CloseOC2();
                SetDCOC2PWM(0);     //Not valid parameter so stop PWM
                break;
            case 4:
                CloseOC4();
                SetDCOC2PWM(0);     //Not valid parameter so stop PWM
                break;
            default: break;
        }
        mT2IntEnable(0);    //disable T2 interrupt
        return;             //nothing to do here
    }

    switch(pwm_channel){
        case 1:
            OpenOC1(OC_ON | OC_TIMER2_SRC | OC_PWM_FAULT_PIN_DISABLE,0,0);
            SetDCOC1PWM(localDuty);      //set Duty cycle according to the period
            break;
        case 2:
            OpenOC2(OC_ON | OC_TIMER2_SRC | OC_PWM_FAULT_PIN_DISABLE,0,0);
            SetDCOC2PWM(localDuty);      //set Duty cycle according to the period
            break;
        case 4:
            OpenOC4(OC_ON | OC_TIMER2_SRC | OC_PWM_FAULT_PIN_DISABLE,0,0);
            SetDCOC4PWM(localDuty);      //set Duty cycle according to the period
            break;
    }
}

void __ISR( _TIMER_2_VECTOR, IPL5AUTO) T2Interrupt(void)
{
    mT2ClearIntFlag();      //clear interrupt flag
}

inline void Buzzer_On(int period, int duty_cycle)
{
    PWM_Init(2,period, duty_cycle); //channel 2 is BUZZER
}

void Backlight_SetPWM(BYTE brightness)
{
    if(brightness){
        if(brightness == 100){
            //at 100% lets turn the PWM off
            PWM_Init(1,0,0);
            DisplayBacklightOn();   
        }else if(brightness < 50){
            //brightness can't be less than 50%
            brightness = 50;
        }
        //Init PWM via driver, 1kHz is fine and works well
        PWM_Init(1,1000,brightness);
    }else{
        //can't also be zero, so full off.
        DisplayBacklightOff();
        PWM_Init(1,0,0);
    }
}


#if defined(ICEFYRE_BETA)   //Legacy support
//Local private defines
#define SWITCH_DEBOUNCE_TIME_MS     1
BOOL SW_Read(void)//TRUE->SW is pressed, FALSE->SW isn't pressed
{
    /*Will be deprecated for IceFyre RC1!!!*/
    static BOOL old_state = FALSE;
    static BOOL new_state = FALSE;

    new_state = SWITCH1;    //read SW

    if(new_state){  //debounce time
        DelayMs(SWITCH_DEBOUNCE_TIME_MS);

        if(SWITCH1){        //not pressed after delay? (logic 0 is pressed)
            new_state = 0;  //fail
            return FALSE;
        }

        if(new_state == old_state){ //same state as previous reading?
            return 0;               //not good
        }else{
            old_state = new_state;
        }

    }else if(old_state){
        old_state = 0;
    }

    return new_state;
}
#endif
