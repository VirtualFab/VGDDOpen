/********************************************************************
 FileName:    	timers.c
 Dependencies: 	-
 Hardware:	IceFyre RC1
 Complier:  	Microchip XC32
 Company:	Vinagrón Digital
 Author:	Juan Carlos Orozco Gutierrez

 Software License Agreement:

 This software has been licensed under the GNU General Public
 License is intended to guarantee your freedom to share and change free
 software--to make sure the software is free for all its users.

********************************************************************
 File Description:
    Timers Support File for PIC32


 Change History:
  Rev   Description                                 Modified by:
  ----  -----------------------------------------  --------------
  1.0   Initial release                             JCOG

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/
#include "HardwareProfile.h"

#define TIMER_FBP       (GetPeripheralClock()/2)   //all peripherals run at 80MHz
#define TIMER_PRESCALER 256
#define TMR_PS_REG(x) T1_PS_1_##x
#define TMR_PS_REG2(x) T1_PS_1_##x

#define TIMER_PRESCALER 256
#define TIMER_PRESC_REG XCAT(XCAT(T1_PS_1_, ),TIMER_PRESCALER)

#define TIMER_PERIOD    TIMER_FBP / TIMER_PRESCALER

static unsigned int _timer_period = 0;

void Timer1_Init(void)
{
    // Set up the timer interrupt with a priority of 5
    INTEnable(INT_T1, INT_ENABLED);
    INTSetVectorPriority(INT_TIMER_1_VECTOR, INT_PRIORITY_LEVEL_5);
    INTSetVectorSubPriority(INT_TIMER_1_VECTOR, INT_SUB_PRIORITY_LEVEL_0);
}

void Timer1_SetTick(unsigned int tick_ms)
{
    _timer_period = (TIMER_FBP / TIMER_PRESCALER);
}

void Timer1_Enable(void)
{
    OpenTimer1(T1_ON | T1_SOURCE_INT | T1_PS_1_256, _timer_period);
//    OpenTimer1(T1_ON | T1_SOURCE_INT | T1_PS_1_64, _timer_period);

//    TMR_PS_REG(TIMER_PRESCALER);
//    TMR_PS_REG(64);
}

inline void Timer1_Disable(void)
{
    //unimplemented
    mT1IntEnable(0);    //disable T1 interrupt
}

void __ISR( _TIMER_1_VECTOR, IPL5AUTO) T1Interrupt(void)
{
    mT1ClearIntFlag();      //clear interrupt flag
}
