/********************************************************************
 FileName:    	IceFyreBSP.h
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
  2.0   Added support for IceFyre RC1               JCOG

 For extra documentation and support:
 *  http://www.vinagrondigital.com
 ********************************************************************/
#ifndef ICEFYRE_BSP_H
#define	ICEFYRE_BSP_H

//Function prototypes
void IceFyre_Init(void);
BOOL SW_Read(void);
BYTE DIPSW_Read(void);
float Temperature_Read(BYTE output_scale);
inline void Buzzer_On(int period, int duty_cycle);
void Backlight_SetPWM(BYTE brightness);


//Function defines
#define CELSIUS     0
#define KELVIN      1
#define FAHRENHEIT  2

#define HAMMERHEAD_DEVICE "IceFyre RC1"

#define STATUS_LED_1_LAT        LATAbits.LATA7
#define STATUS_LED_1_TRIS       TRISAbits.TRISA7
#define STATUS_LED_1_PORT       PORTAbits.RA7
#define STATUS_LED_1_MASK       _PORTA_RA7_MASK //green

#define STATUS_LED_2_LAT        LATAbits.LATA6
#define STATUS_LED_2_TRIS       TRISAbits.TRISA6
#define STATUS_LED_2_PORT       PORTAbits.RA6
#define STATUS_LED_2_MASK       _PORTA_RA6_MASK //orange

/*No physical SW in RC1!!!*/

//MXP
#define MXP_RESET_LAT           LATAbits.LATA0
#define MXP_RESET_TRIS          TRISAbits.TRISA0

/*Review name convention*/
#define MXP_SDCD_PORT           PORTCbits.RC3
#define MXP_SDCD_TRIS           TRISCbits.TRISC3

#define MXP_GP0_LAT             PORTCbits.RC3
#define MXP_GP0_TRIS            TRISCbits.TRISC3
/*End Review name convention*/

#define MXP_INT_TRIS            TRISEbits.TRISE8
#define MXP_INT_PORT            PORTEbits.RE8
#define MXP_INT_ENABLE          IEC0bits.INT1IE
#define MXP_INT_TRIGGER         INTCONbits.INT1EP
#define MXP_INT_PRIORITY        IPC1bits.INT1IP
#define MXP_INT_FLAG            IFS0bits.INT1IF
//            #define MXP_INT_FLAG_MASK       _INT2R_INT2R_MASK

#define MXP_OC2_LAT             LATDbits.LATD3
#define MXP_OC2_TRIS            TRISDbits.TRISD3

#define MXP_GP1_LAT             LATDbits.LATD3      
#define MXP_GP1_MASK            _LATD_LATD3_MASK
#define MXP_GP1_TRIS            TRISDbits.TRISD3

#define MXP_GP2_LAT             LATDbits.LATD11
#define MXP_GP2_TRIS            TRISDbits.TRISD11

#define MXP_SDA_LAT             LATAbits.LATA15
#define MXP_SDA_PORT            PORTAbits.RA15
#define MXP_SDA_TRIS            TRISAbits.TRISA15

#define MXP_SCL_LAT             LATAbits.LATA14
#define MXP_SCL_TRIS            TRISAbits.TRISA14

#define MXP_SCK_TRIS            TRISDbits.TRISD15
#define MXP_SCK_LAT             LATDbits.LATD15

#define MXP_MOSI_TRIS           TRISFbits.TRISF8
#define MXP_MOSI_LAT            LATFbits.LATF8

#define MXP_MISO_TRIS           TRISFbits.TRISF2
#define MXP_MISO_PORT           PORTFbits.RF2

//EEPROM uses and shares the SPI bus with MXP! (SPI3)
#define EEPROM_CS_LAT           LATGbits.LATG13
#define EEPROM_CS_TRIS          TRISGbits.TRISG13
#define EEPROM_SCK_TRIS         MXP_SCK_TRIS
#define EEPROM_SDO_TRIS         MXP_MOSI_TRIS
#define EEPROM_SDI_TRIS         MXP_MISO_TRIS

//SXP
#define ADDR_DIPSW_1            PORTBbits.RB12
#define ADDR_DIPSW_1_TRIS       TRISBbits.TRISB12

#define ADDR_DIPSW_2            PORTBbits.RB13
#define ADDR_DIPSW_2_TRIS       TRISBbits.TRISB13

#define ADDR_DIPSW_3            PORTBbits.RB14
#define ADDR_DIPSW_3_TRIS       TRISBbits.TRISB14

#define ADDR_DIPSW_4            PORTBbits.RB15
#define ADDR_DIPSW_4_TRIS       TRISBbits.TRISB15

#define ADDR_DIPSW_MASK         (0xF000)

#define SXP_INT_LAT             LATAbits.LATA10 //output to master eg.Aguijon
#define SXP_INT_TRIS            TRISAbits.TRISA10

#define SXP_SSL_TRIS            TRISGbits.TRISG9    //must only be set as input!

#define SXP_RESERVED_LAT        LATAbits.LATA9  //check
#define SXP_RESERVED_TRIS       TRISAbits.TRISA9//check

#define SXP_SCK_TRIS            TRISGbits.TRISG6
#define SXP_SCK_LAT             LATGbits.LATG6

#define SXP_MOSI_TRIS           TRISGbits.TRISG8
#define SXP_MOSI_LAT            LATGbits.LATG8

#define SXP_MISO_TRIS           TRISGbits.TRISG7
#define SXP_MISO_PORT           PORTGbits.RG7

//PCT module
#define PCT_SCL_TRIS            TRISAbits.TRISA2
#define PCT_SCL_LAT             LATAbits.LATA2

#define PCT_SDA_TRIS            TRISAbits.TRISA3
#define PCT_SDA_LAT             LATAbits.LATA3

#define PCT_INT_TRIS            TRISEbits.TRISE9
#define PCT_INT_ENABLE          IEC0bits.INT2IE
#define PCT_INT_TRIGGER         INTCONbits.INT2EP
#define PCT_INT_PRIORITY        IPC2bits.INT2IP
#define PCT_INT_FLAG            IFS0bits.INT2IF
#define PCT_INT_FLAG_MASK       _IFS0_INT2IF_MASK

#define PCT_RST_TRIS            TRISAbits.TRISA1
#define PCT_RST_LAT             LATAbits.LATA1

//RS232 & RS485
#define UART_TXD_TRIS           TRISFbits.TRISF5
#define UART_RXD_TRIS           TRISFbits.TRISF4
#define UART_MODULE_ID          UART2

//CAN
#define CANTX_TRIS              TRISFbits.TRISF13
#define CANRX_TRIS              TRISFbits.TRISF12

//Buzzer
#define BUZZER_LAT              LATDbits.LATD1
#define BUZZER_TRIS             TRISDbits.TRISD1
#define BUZZER_MASK             _TRISD_TRISD1_MASK

//Temp sensor
#define TEMPSENS_ADC_CH         2
#define TEMPSENS_ADC_MASK       0xFFFB
#define TEMPSENS_PIN_PORT       PORTBbits.RB2
#define TEMPSENS_PIN_TRIS       TRISBbits.TRISB2

//Backlight PWM
#define BACKLIGHT_OCCON
#define BACKLIGHT_PWM_DUTY
#define BACKLIGHT_PWM_PERIOD

//Watchdog & reset
#define WDOG_PIN                LATGbits.LATG15
#define WDOG_PIN_TRIS           TRISGbits.TRISG15
#define WDOG_PIN_MASK           _TRISG_TRISG15_MASK

//TODO: cambiar por atomic bit manipulation
#define WDOG_Pet()              do{ LATGINV  = WDOG_PIN_MASK; }while(0)
#define WDOG_SoftDisable()      do{ TRISGSET = WDOG_PIN_MASK; }while(0)
#define WDOG_SoftEnable()       do{ TRISGCLR = WDOG_PIN_MASK; }while(0)

//Misc
#define IceFyre_Heartbeat()    do{ LATAINV = STATUS_LED_1_MASK; }while(0)

#endif /* ICEFYRE_BSP_H */
