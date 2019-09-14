/********************************************************************
 FileName:    	FT5306.h
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
    FT5306 Driver Helper File


 Change History:
  Rev   Description                                 Modified by:
  ----  -----------------------------------------  --------------
  1.0   Initial release                             JCOG  

 For extra documentation and support:
 *  http://www.vinagrondigital.com
********************************************************************/

#ifndef FT5306_H
#define	FT5306_H

#define FT5306_MAX_TOUCHES      0x05

#define FT5306_PACKET_SIZE      0x1F

#define FT5306_I2C_ADDRESS      0x70
#define FT5306_I2C_CLOCK_FREQ   400000  //400KHz
#define FT5306_I2C_BUS          I2C2

#define FT_REG_DEVICE_MODE      0x00    /*Bit Addr 6:4, 0->Normal OpMode 1,2->reserved*/
#define FT_REG_GESTURE_ID       0x01    /*Describes the gesture of a valid touch*/

#define FT_REG_EVENT_FLAG_MASK  0xC0
#define FT_REG_TOUCHPOINT_MASK  0x07
#define FT_REG_TD_STATUS        0x02    /*How many touch points are detected*/
#define FT_REG_TOUCH1_XH        0x03
#define FT_REG_TOUCH1_XL        0x04
#define FT_REG_TOUCH1_YH        0x05
#define FT_REG_TOUCH1_YL        0x06

#define FT_REG_TOUCH2_XH        0x09
#define FT_REG_TOUCH2_XL        0x0A
#define FT_REG_TOUCH2_YH        0x0B
#define FT_REG_TOUCH2_YL        0x0C

#define FT_REG_TOUCH3_XH        0x0F
#define FT_REG_TOUCH3_XL        0x10
#define FT_REG_TOUCH3_YH        0x11
#define FT_REG_TOUCH3_YL        0x12

#define FT_REG_TOUCH4_XH        0x15
#define FT_REG_TOUCH4_XL        0x16
#define FT_REG_TOUCH4_YH        0x17
#define FT_REG_TOUCH4_YL        0x18

#define FT_REG_TOUCH5_XH        0x1B
#define FT_REG_TOUCH5_XL        0x1C
#define FT_REG_TOUCH5_YH        0x1D
#define FT_REG_TOUCH5_YL        0x1E

#define FT_REG_ID_G_THGROUP     0x80
#define FT_REG_ID_G_THPEAK      0x81
#define FT_REG_ID_G_THCAL       0x82
#define FT_REG_ID_G_THWATER     0x83
#define FT_REG_ID_G_THTEMP      0x84
#define FT_REG_ID_G_CTRL        0x86

#define FT_REG_ID_G_TIME_ENTER_MONITOR  0x87
#define FT_REG_ID_G_PERIODACTIVE        0x88
#define FT_REG_ID_G_PERIOD_MONITOR      0x89
#define FT_REG_ID_G_AUTO_CLB_MODE       0xA0
#define FT_REG_ID_G_LIB_VERSION_H       0xA1
#define FT_REG_ID_G_LIB_VERSION_L       0xA2

#define FT_REG_ID_G_CIPHER      0xA3
#define FT_REG_ID_G_MODE        0xA4
#define FT_REG_ID_G_PMODE       0xA5
#define FT_REG_ID_G_FIRMID      0xA6
#define FT_REG_ID_G_STATE       0xA7
#define FT_REG_ID_G_FT5201ID    0xA8
#define FT_REG_ID_G_ERR         0xA9
#define FT_REG_ID_G_CLB         0xAA
#define FT_REG_ID_G__B_AREA_TH  0xAE

#define FT_REG_LOG_MSG_CNT      0xFE
#define FT_REG_LOG_CUR_CHA      0xFF

typedef struct _PCT_PACKET_{    
    //is it really needed? Don't think so
}PCT_DATA_PACKET_t;


void FT5306_Init        (void);

void I2C_Init           (void);
void I2C_WriteByte      (BYTE deviceID, BYTE offset, BYTE value);
BYTE I2C_ReadByte       (BYTE deviceID, BYTE offset);
BYTE I2C_WriteBock      (BYTE deviceID, BYTE offset, BYTE *buffer, WORD length);
BYTE I2C_ReadBlock      (BYTE deviceID, BYTE offset, BYTE *buffer, WORD length);
BYTE I2C_ReadBlock_PCAP (BYTE deviceID, BYTE offset, BYTE *buffer, WORD length);

#define FT5306_Write(reg,data)        I2C_WriteByte(FT5306_I2C_ADDRESS,reg,data)
#define FT5306_Read(offset,buf,len)   I2C_ReadBlock(FT5306_I2C_ADDRESS,offset,buf,len)


/* FT_REG_GESTURE_ID Description:
 * Bit Address: 7:0
 * 0x10 -> Move up
 * 0x14 -> Move left
 * 0x18 -> Move down
 * 0x1C -> Move right
 * 0x48 -> Move Zoom In
 * 0x49 -> Move Zoom Out
 * 0x00 -> No Gesture
 */

/* FT_REG_TD_STATUS Description:
 * Touch data status register
 * Bit Address: 3:0
 */

/* FT_REG_TOUCHn_XH Description:
 * Describes MSB of the X coordinate of the nth touch point
 * and the corresponding event flag.
 * Bit Address: 7:6
 * 0x00 -> Put Down
 * 0x01 -> Put Up
 * 0x10 -> Contact
 * 0x11 -> Reserved
 *
 * FT_REG_TOUCHn_XL Description:
 * Describes LSB of the X coordinate of the nth touch point
 * Bit Address: 7:0
 */

#endif	/* FT5306_H */
