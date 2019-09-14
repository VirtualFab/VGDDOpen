/********************************************************************
 FileName:    	TouchScreenCapacitive.h
 Hardware:	IceFyre BETA
 Complier:  	Microchip XC32
 Company:	Vinagrón Digital
 Author:	Juan Carlos Orozco Gutierrez

 Software License Agreement:

 This software has been licensed under the GNU General Public
 License is intended to guarantee your freedom to share and change free
 software--to make sure the software is free for all its users.

********************************************************************
 File Description:

   This is a capacitive touch screen driver that is using the
   Microchip Graphics Library. 

 Change History:
  Rev   Description                                 Modified by:
  ----  -----------------------------------------  --------------
  1.0   Initial release                             JCOG
  1.1   Removed TouchScreen.h dependency            JCOG

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/

#ifndef TOUCHSCREEN_CAPACITIVE_H
#define TOUCHSCREEN_CAPACITIVE_H


//FT5306 available gestures
#define GESTURE_MOVE_UP         0x10
#define GESTURE_MOVE_LEFT       0x14
#define GESTURE_MOVE_DOWN       0x18
#define GESTURE_MOVE_RIGHT      0x1C
#define GESTURE_ZOOM_IN         0x48
#define GESTURE_ZOOM_OUT        0x49
#define GESTURE_INVALID         0x00    //not really invalid, just no gesture
typedef unsigned char           FT5306_GESTURE_t;

//Function Prototypes
SHORT   TouchGetX(BYTE touchNumber);
SHORT   TouchGetY(BYTE touchNumber);
inline unsigned char TouchGetGesture(void);

#endif


