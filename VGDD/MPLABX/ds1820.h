/*** FILEHEADER ****************************************************************
 *
 *    FILENAME:    ds1820.h
 *    DATE:        25.02.2005
 *    AUTHOR:      Christian Stadler
 *
 *    DESCRIPTION: Driver for DS1820 1-Wire Temperature sensor (Dallas)
 *
 ******************************************************************************/

#ifndef _DS1820_H
#define _DS1820_H

// check configuration of driver
#ifndef DS1820_DATAPIN_OUT
#error DS1820 data pin not defined!
#endif

/* --- configure DS1820 temparture sensor pin --- */
#define DS1820_output_low()     DS1820_DATATRIS = 0;(DS1820_DATAPIN_OUT = 0)
#define DS1820_output_high()    DS1820_DATATRIS = 0;(DS1820_DATAPIN_OUT = 1)

#define TEMP_RES              0x100 /* temperature resolution => 1/256°C = 0.0039°C */

/* -------------------------------------------------------------------------- */
/*                         DS1820 Timing Parameters                           */
/* -------------------------------------------------------------------------- */

#define DS1820_RST_PULSE       500 //480   /* master reset pulse time in 10*[us] */
#define DS1820_MSTR_BITSTART   2       /* delay time for bit start by master */
#define DS1820_PRESENCE_WAIT   40      /* delay after master reset pulse in 10*[us] */
//#define DS1820_PRESENCE_FIN    480   /* dealy after reading of presence pulse 10*[us] */
#define DS1820_BITREAD_DLY     5       /* bit read delay */
#define DS1820_BITWRITE_DLY    100     /* bit write delay */


/* -------------------------------------------------------------------------- */
/*                            DS1820 Registers                                */
/* -------------------------------------------------------------------------- */

#define DS1820_REG_TEMPLSB    0
#define DS1820_REG_TEMPMSB    1
#define DS1820_REG_CNTREMAIN  6
#define DS1820_REG_CNTPERSEC  7
#define DS1820_SCRPADMEM_LEN  9     /* length of scratchpad memory */

#define DS1820_ADDR_LEN       8


/* -------------------------------------------------------------------------- */
/*                            DS1820 Commands                                 */
/* -------------------------------------------------------------------------- */

#define DS1820_CMD_SEARCHROM     0xF0
#define DS1820_CMD_READROM       0x33
#define DS1820_CMD_MATCHROM      0x55
#define DS1820_CMD_SKIPROM       0xCC
#define DS1820_CMD_ALARMSEARCH   0xEC
#define DS1820_CMD_CONVERTTEMP   0x44
#define DS1820_CMD_WRITESCRPAD   0x4E
#define DS1820_CMD_READSCRPAD    0xBE
#define DS1820_CMD_COPYSCRPAD    0x48
#define DS1820_CMD_RECALLEE      0xB8


#define DS1820_FAMILY_CODE_DS18B20      0x28
#define DS1820_FAMILY_CODE_DS18S20      0x10


/* -------------------------------------------------------------------------- */
/*                           Low-Level Functions                              */
/* -------------------------------------------------------------------------- */
BOOL DS1820_Reset(void);
BOOL DS1820_ReadBit(void);
void DS1820_WriteBit(BOOL bBit);
UINT8 DS1820_ReadByte(void);
void DS1820_WriteByte(UINT8 val_u8);
void DS1820_AddrDevice(UINT8 nAddrMethod);
BOOL DS1820_FindNextDevice(void);
BOOL DS1820_FindFirstDevice(void);
void DS1820_WriteEEPROM(UINT8 nTHigh, UINT8 nTLow);
void DS1820_StartConversion(void);
void DS1820_GetAllTemps(void);
INT16 DS1820_GetTempRaw(void);
float DS1820_GetTempFloat(void);
void DS1820_GetTempString(INT16 tRaw_s16, char *strTemp_pc);
extern INT16 DS1820LastTemp[];
extern UINT8 DS1820Found;
extern UINT8 DS1820Selected;



//#define DS1820_DelayUs(x) DelayUs(x)
//#define DS1820_DelayUs(dly_us)       Delay10us(dly_us/10)


///*******************************************************************************
// * FUNCTION:   DS1820_DelayUs
// * PURPOSE:    Delay for the given number of micro seconds.
// *
// * INPUT:      dly_us      number of micro seconds to delay
// * OUTPUT:     -
// * RETURN:     -
// ******************************************************************************/
//#define DS1820_Delay10Us(dly_us)       Delay10us(dly_us)


/*******************************************************************************
 * FUNCTION:   DS1820_DelayMs
 * PURPOSE:    Delay for the given number of milliseconds.
 *
 * INPUT:      dly_ms      number of milliseconds to delay
 * OUTPUT:     -
 * RETURN:     -
 ******************************************************************************/
//#define DS1820_DelayMs(dly_ms)   DelayMs(dly_ms)

/*******************************************************************************
 * FUNCTION:   DS1820_DisableInterrupts
 * PURPOSE:    Disable interrupts
 *
 * INPUT:      -
 * OUTPUT:     -
 * RETURN:     -
 ******************************************************************************/
#ifdef DS1820_INTERRUPT_LOCK
#define DS1820_DisableInterrupts()  disable_interrupts(GLOBAL)
#else
#define DS1820_DisableInterrupts()
#endif

/*******************************************************************************
 * FUNCTION:   DS1820_EnableInterrupts
 * PURPOSE:    Enable interrupts
 *
 * INPUT:      -
 * OUTPUT:     -
 * RETURN:     -
 ******************************************************************************/
#ifdef DS1820_INTERRUPT_LOCK
#define DS1820_EnableInterrupts()   enable_interrupts(GLOBAL)
#else
#define DS1820_EnableInterrupts()
#endif

#endif /* _DS1820_H */

