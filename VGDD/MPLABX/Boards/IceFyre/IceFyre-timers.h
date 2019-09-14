/********************************************************************
 FileName:    	timers.h
 Dependencies: 	none
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
    timers header file


 Change History:
  Rev   Description                                 Modified by:
  ----  -----------------------------------------  --------------
  1.0   Initial release                             JCOG  

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/
#ifndef TIMERS_H
#define	TIMERS_H


//the following values are PERIOD -> __|--|__|--|__|--|__|--|__
#define TIMER_TICK_100MS   	0x61A8
#define TIMER_TICK_50MS		0x30D4
#define TIMER_TICK_10MS    	0x09C4
#define TIMER_TICK_5MS          0x04E2
#define TIMER_TICK_1MS     	0x00FA
#define TIMER_TICK_500uS   	0x007D
/* These values, are specific to the 1:64 prescaler selected during TimerX_init();*/

void        Timer1_Init     (void);
void        Timer1_SetTick  (unsigned int tick_ms);
inline void Timer1_Enable   (void);
inline void Timer1_Disable  (void);


#endif	/* TIMERS_H */

