/********************************************************************
 FileName:    	TouchScreenCapacitive.c
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
    Adapted to Microchip touch standard.
    "FT5306.h" must be included!


 Change History:
  Rev   Description                                 Modified by:
  ----  -----------------------------------------  --------------
  1.0   Initial release                             JCOG
  1.1   Tweaked ISR for faster access               JCOG

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/
#include "HardwareProfile.h"
#include "Graphics/Graphics.h"
#include "TouchScreenCapacitive.h"
#include "FT5306.h"

WORD_VAL PCapX[5]= {-1,-1,-1,-1,-1};
WORD_VAL PCapY[5]= {-1,-1,-1,-1,-1};

//used as lookup tables 
const  BYTE   Xtouch_point_addr[5] = {0x03, 0x09, 0x0F, 0x15, 0x1B};
const  BYTE   Ytouch_point_addr[5] = {0x05, 0x0B, 0x11, 0x17, 0x1D};
static FT5306_GESTURE_t _gesture = 0;


/*********************************************************************
* Function: void TouchGetMsg(GOL_MSG* pMsg)
* PreCondition: none
* Input: pointer to the message structure to be populated
* Output: none
* Side Effects: none
* Overview: populates GOL message structure
* Note: none
********************************************************************/
void TouchGetMsg(GOL_MSG *pMsg)
{
    static SHORT    prevX = -1;
    static SHORT    prevY = -1;

    SHORT           x, y;

    x = TouchGetX(0);   //Get X0 touch
    y = TouchGetY(0);   //Get Y0 touch
    pMsg->type      = TYPE_TOUCHSCREEN;
    pMsg->uiEvent   = EVENT_INVALID;

    if((x == -1) || (y == -1))  //No touch position
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
* PreCondition: none
* Input: none
* Output: none
* Side Effects: none
* Overview: Initializes touch screen module.
* Note: none
********************************************************************/
//void TouchHardwareInit(void *initValues)
//{
//      DEPRECATED
//}

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
SHORT TouchGetX(BYTE touchNumber)
{
    long result;

    result = PCapX[touchNumber].Val;
    return ((SHORT)result);
}

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
SHORT TouchGetY(BYTE touchNumber)
{
    long result;
    result = PCapY[touchNumber].Val;
    return ((SHORT)result);
}


/*********************************************************************
* Function: unsigned char TouchGetGesture()
* PreCondition: FT5306 must be initialized
* Input: none
* Output: detected gesture, if any
* Side Effects: none
* Overview: Gesture handler function
* Note: none
********************************************************************/
inline unsigned char TouchGetGesture(void)
{
    //returns the gesture detected (if any).
    return ((FT5306_GESTURE_t)_gesture);
}

//FT5306 interrupt service routine
void __ISR(_EXTERNAL_2_VECTOR, IPL4AUTO) _PCAPHandler(void)
{           
    BYTE pct_data[FT5306_PACKET_SIZE] = {0};   //31byte buffer
    BYTE event_flag[5]       = {0,0,0,0,0};    //0->Put Down, 1->Put Up, 2->Contact(hold), 3->Reserved
    BYTE touch_points        =  0;             //1-5 points
    BYTE i = 0;

    /* Read every register until FT_REG_TOUCH5_YL (0x1E)
     * Send Slave Addr = 0x70 and store data from pct in
     * data_from_pct array */
//    I2C_ReadBlock(FT5306_I2C_ADDRESS, 0, &pct_data[0], FT_REG_TOUCH5_YL);
    FT5306_Read(0,&pct_data[0],FT_REG_TOUCH5_YL);

    //First check for a gesture
    if(pct_data[1] != GESTURE_INVALID)
    {
        _gesture = pct_data[1];     //valid gesture, let's copy it
    }else if(_gesture)
    {
        _gesture = GESTURE_INVALID; //not a valid gesture
    }

    //Get number of touch points
    touch_points  = (pct_data[0x02] & FT_REG_TOUCHPOINT_MASK);

    event_flag[0] = (pct_data[0x03] & 0xC0) >> 6;
    event_flag[1] = (pct_data[0x09] & 0xC0) >> 6;
    event_flag[2] = (pct_data[0x0F] & 0xC0) >> 6;
    event_flag[3] = (pct_data[0x15] & 0xC0) >> 6;
    event_flag[4] = (pct_data[0x1B] & 0xC0) >> 6;

    if(touch_points){ //is there any touch point?
        for(i=0 ; i<touch_points ; i++){ //if so, let's collect touch data
            if(!event_flag[i] || event_flag[i] == 0x02){
                PCapX[i].Val  = ((pct_data[Xtouch_point_addr[i]] & 0x0F) << 8);   //Get TOUCHx_XH from 1st XH address
                PCapX[i].Val |=   pct_data[Xtouch_point_addr[i]+1];               //Get TOUCHx_XL from next address

                PCapY[i].Val  = ((pct_data[Ytouch_point_addr[i]] & 0x0F) << 8);   //Get TOUCHx_YH from 1st YH address
                PCapY[i].Val |=   pct_data[Ytouch_point_addr[i]+1];               //Get TOUCHx_YL from next address
            }else{  //should not get here, but well, we never know
                PCapX[i].Val = -1;
                PCapY[i].Val = -1;
            }
        }
    }else{  //no touch point(s) were detected, let's clear everything
        for(i=0 ; i<5 ; i++){
            if(event_flag[i] && event_flag[i] != 2){
                //Touch was released
                PCapX[i].Val = -1;
                PCapY[i].Val = -1;
            }
        }
    }


//    for(i=0 ; i<touch_points ; i++){ //let's collect touch data
//    for(i=0 ; i<FT5306_MAX_TOUCHES ; i++){ //let's collect touch data
//        event_flag[i] = (pct_data[Xtouch_point_addr[i]] & FT_REG_EVENT_FLAG_MASK) >> 6;
//
//        if(!event_flag[i] || event_flag[i] == 0x02){    //check for a valid event
//            PCapX[i].Val  = ((pct_data[Xtouch_point_addr[i]] & 0x0F) << 8);   //Get TOUCHx_XH from 1st XH address
//            PCapX[i].Val |=   pct_data[Xtouch_point_addr[i]+1];               //Get TOUCHx_XL from next address
//
//            PCapY[i].Val  = ((pct_data[Ytouch_point_addr[i]] & 0x0F) << 8);   //Get TOUCHx_YH from 1st YH address
//            PCapY[i].Val |=   pct_data[Ytouch_point_addr[i]+1];               //Get TOUCHx_YL from next address
//        }else{
//            PCapX[i].Val = -1;
//            PCapY[i].Val = -1;
////            for(i=touch_points ; i<FT5306_MAX_TOUCHES ; i++){
////                PCapX[i].Val = -1;
////                PCapY[i].Val = -1;
////            }
//            break;
//        }
//    }

    LATAINV = STATUS_LED_2_MASK;
    IFS0CLR = PCT_INT_FLAG_MASK;     //clear the interrupt flag the atomic way ;)
}
