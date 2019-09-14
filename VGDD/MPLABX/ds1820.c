/******************************************************************************
 *
 *    FILENAME:    ds1820.h
 *    DATE:        25.02.2005
 *    AUTHOR:      Christian Stadler
 *
 *    DESCRIPTION: Driver for DS1820 1-Wire Temperature sensor (Dallas)
 *
 ******************************************************************************/

#include "HardwareProfile.h"
#include "ds1820.h"
#include <stdio.h>

// check configuration of driver
#ifndef DS1820_DATAPIN_OUT
#error DS1820 data pin not defined!
#endif

/* -------------------------------------------------------------------------- */
/*                            static variables                                */
/* -------------------------------------------------------------------------- */
static BOOL bDoneFlag;
static UINT8 nLastDiscrepancy_u8;
static UINT8 nRomAddr_au8[NUM_DS1820][DS1820_ADDR_LEN];
INT16 DS1820LastTemp[NUM_DS1820];
UINT8 DS1820Found=0;
UINT8 DS1820Selected=0;

/* -------------------------------------------------------------------------- */
/*                           Low-Level Functions                              */
/* -------------------------------------------------------------------------- */

#define DELAYOVERHEAD (2)

void DS1820_DelayUs(UINT8 us) {
    UINT8 us10;
    if (us <= DELAYOVERHEAD) return; // prevent underflow
    us -= DELAYOVERHEAD; // overhead of function call in us.
    us10=us/10;
    if(us10){
        Delay10us(us10);
        return;
    }
    Nop(); // 1  extra overhead to make function overhead an even us.
    Nop(); // 1  add or remove Nop's as necessary.
    Nop(); // 1
    Nop();       // 1
    Nop();       // 1
    Nop();       // 1
    Nop();       // 1
    Nop();       // 1
    Nop();       // 1
    Nop();       // 1
    Nop();       // 1
    Nop();       // 1

    do // loop needs to be 12 cycles so each cycle is 1us.
    {
        Nop(); // 1
        Nop(); // 1
        Nop(); // 1
        Nop(); // 1
        Nop(); // 1
        Nop(); // 1
        Nop(); // 1
        Nop(); // 1
        ClrWdt(); // 1
    } while (--us); // 3
}

/*******************************************************************************
 * FUNCTION:   DS1820_Reset
 * PURPOSE:    Initializes the DS1820 device.
 *
 * INPUT:      -
 * OUTPUT:     -
 * RETURN:     FALSE if at least one device is on the 1-wire bus, TRUE otherwise
 ******************************************************************************/
BOOL DS1820_Reset(void) {
    BOOL bPresPulse;

    DS1820_DisableInterrupts();

    /* reset pulse */
    DS1820_output_low();
    DS1820_DelayUs(DS1820_RST_PULSE/2);
    DS1820_DelayUs(DS1820_RST_PULSE/2);
    //DS1820_output_high();
    DS1820_DATATRIS = 1;

    /* wait until pullup pull 1-wire bus to high */
    DS1820_DelayUs(DS1820_PRESENCE_WAIT);

    /* get presence pulse */
//    DS1820_DATATRIS = 1;
    bPresPulse = DS1820_DATAPIN_IN;

    DS1820_DelayUs(DS1820_RST_PULSE/2);
    DS1820_DelayUs(DS1820_RST_PULSE/2);

    DS1820_EnableInterrupts();

    return bPresPulse;
}

/*******************************************************************************
 * FUNCTION:   DS1820_ReadBit
 * PURPOSE:    Reads a single bit from the DS1820 device.
 *
 * INPUT:      -
 * OUTPUT:     -
 * RETURN:     BOOL        value of the bit which as been read form the DS1820
 ******************************************************************************/
BOOL DS1820_ReadBit(void) {
    BOOL bBit;

    DS1820_DisableInterrupts();

    DS1820_output_low();
    DS1820_DelayUs(DS1820_MSTR_BITSTART);

    DS1820_DATATRIS = 1;
    DS1820_DelayUs(DS1820_BITREAD_DLY);
    bBit = DS1820_DATAPIN_IN;

    DS1820_EnableInterrupts();

    return (bBit);
}

/*******************************************************************************
 * FUNCTION:   DS1820_WriteBit
 * PURPOSE:    Writes a single bit to the DS1820 device.
 *
 * INPUT:      bBit        value of bit to be written
 * OUTPUT:     -
 * RETURN:     -
 ******************************************************************************/
void DS1820_WriteBit(BOOL bBit) {
    DS1820_DisableInterrupts();

    DS1820_output_low();
    DS1820_DelayUs(DS1820_MSTR_BITSTART);
    if (bBit != FALSE) {
        DS1820_output_high();
    }

    DS1820_DelayUs(DS1820_BITWRITE_DLY);
    DS1820_output_high();

    DS1820_EnableInterrupts();
}

/*******************************************************************************
 * FUNCTION:   DS1820_ReadByte
 * PURPOSE:    Reads a single byte from the DS1820 device.
 *
 * INPUT:      -
 * OUTPUT:     -
 * RETURN:     UINT8          byte which has been read from the DS1820
 ******************************************************************************/
UINT8 DS1820_ReadByte(void) {
    UINT8 i;
    UINT8 value = 0;

    for (i = 0; i < 8; i++) {
        if (DS1820_ReadBit()) {
            value |= (1 << i);
        }
        DS1820_DelayUs(120);
    }
    return (value);
}

/*******************************************************************************
 * FUNCTION:   DS1820_WriteByte
 * PURPOSE:    Writes a single byte to the DS1820 device.
 *
 * INPUT:      val_u8         byte to be written
 * OUTPUT:     -
 * RETURN:     -
 ******************************************************************************/
void DS1820_WriteByte(UINT8 val_u8) {
    UINT8 i;
    UINT8 temp;

    for (i = 0; i < 8; i++) { /* writes byte, one bit at a time */
        temp = val_u8 >> i; /* shifts val right 'i' spaces */
        temp &= 0x01; /* copy that bit to temp */
        DS1820_WriteBit(temp); /* write bit in temp into */
    }
    DS1820_DelayUs(110);
}

/* -------------------------------------------------------------------------- */
/*                             API Interface                                  */
/* -------------------------------------------------------------------------- */

/*******************************************************************************
 * FUNCTION:   DS1820_AddrDevice
 * PURPOSE:    Addresses a single or all devices on the 1-wire bus.
 *
 * INPUT:      nAddrMethod       use DS1820_CMD_MATCHROM to select a single
 *                               device or DS1820_CMD_SKIPROM to select all
 * OUTPUT:     -
 * RETURN:     -
 ******************************************************************************/
void DS1820_AddrDevice(UINT8 nAddrMethod) {
    UINT8 i;

    if (nAddrMethod == DS1820_CMD_MATCHROM) {
        DS1820_WriteByte(DS1820_CMD_MATCHROM); /* address single devices on bus */
        for (i = 0; i < DS1820_ADDR_LEN; i++) {
            DS1820_WriteByte(nRomAddr_au8[DS1820Selected][i]);
        }
    } else {
        DS1820_WriteByte(DS1820_CMD_SKIPROM); /* address all devices on bus */
    }
}

/*******************************************************************************
 * FUNCTION:   DS1820_FindNextDevice
 * PURPOSE:    Finds next device connected to the 1-wire bus.
 *
 * INPUT:      -
 * OUTPUT:     nRomAddr_au8[]       ROM code of the next device
 * RETURN:     BOOL                 TRUE if there are more devices on the 1-wire
 *                                  bus, FALSE otherwise
 ******************************************************************************/
BOOL DS1820_FindNextDevice(void) {
    UINT8 state_u8;
    UINT8 byteidx_u8;
    UINT8 mask_u8 = 1;
    UINT8 bitpos_u8 = 1;
    UINT8 nDiscrepancyMarker_u8 = 0;
    BOOL bit_b;
    BOOL bStatus;
    BOOL next_b = FALSE;

    bStatus = DS1820_Reset(); /* reset the 1-wire */

    if (bStatus || bDoneFlag) { /* no device found */
        nLastDiscrepancy_u8 = 0; /* reset the search */
        return FALSE;
    }

    /* send search rom command */
    DS1820_WriteByte(DS1820_CMD_SEARCHROM);

    byteidx_u8 = 0;
    do {
        state_u8 = 0;

        /* read bit */
        if (DS1820_ReadBit() != 0) {
            state_u8 = 2;
        }
        DS1820_DelayUs(120);

        /* read bit complement */
        if (DS1820_ReadBit() != 0) {
            state_u8 |= 1;
        }
        DS1820_DelayUs(120);

        /* description for values of state_u8: */
        /* 00    There are devices connected to the bus which have conflicting */
        /*       bits in the current ROM code bit position. */
        /* 01    All devices connected to the bus have a 0 in this bit position. */
        /* 10    All devices connected to the bus have a 1 in this bit position. */
        /* 11    There are no devices connected to the 1-wire bus. */

        /* if there are no devices on the bus */
        if (state_u8 == 3) {
            break;
        } else {
            /* devices have the same logical value at this position */
            if (state_u8 > 0) {
                /* get bit value */
                bit_b = (BOOL) (state_u8 >> 1);
            }/* devices have confilcting bits in the current ROM code */
            else {
                /* if there was a conflict on the last iteration */
                if (bitpos_u8 < nLastDiscrepancy_u8) {
                    /* take same bit as in last iteration */
                    bit_b = ((nRomAddr_au8[DS1820Selected][byteidx_u8] & mask_u8) > 0);
                } else {
                    bit_b = (bitpos_u8 == nLastDiscrepancy_u8);
                }

                if (bit_b == 0) {
                    nDiscrepancyMarker_u8 = bitpos_u8;
                }
            }

            /* store bit in ROM address */
            if (bit_b != 0) {
                nRomAddr_au8[DS1820Selected][byteidx_u8] |= mask_u8;
            } else {
                nRomAddr_au8[DS1820Selected][byteidx_u8] &= ~mask_u8;
            }

            DS1820_WriteBit(bit_b);

            /* increment bit position */
            bitpos_u8++;

            /* calculate next mask value */
            mask_u8 = mask_u8 << 1;

            /* check if this byte has finished */
            if (mask_u8 == 0) {
                byteidx_u8++; /* advance to next byte of ROM mask */
                mask_u8 = 1; /* update mask */
            }
        }
    } while (byteidx_u8 < DS1820_ADDR_LEN);


    /* if search was unsuccessful then */
    if (bitpos_u8 < 65) {
        /* reset the last discrepancy to 0 */
        nLastDiscrepancy_u8 = 0;
    } else {
        /* search was successful */
        DS1820Found++;
        nLastDiscrepancy_u8 = nDiscrepancyMarker_u8;
        bDoneFlag = (nLastDiscrepancy_u8 == 0);

        /* indicates search is not complete yet, more parts remain */
        next_b = TRUE;
    }

    return next_b;
}

/*******************************************************************************
 * FUNCTION:   DS1820_FindFirstDevice
 * PURPOSE:    Starts the device search on the 1-wire bus.
 *
 * INPUT:      -
 * OUTPUT:     nRomAddr_au8[]       ROM code of the first device
 * RETURN:     BOOL                 TRUE if there are more devices on the 1-wire
 *                                  bus, FALSE otherwise
 ******************************************************************************/
BOOL DS1820_FindFirstDevice(void) {
    UINT8 i, j;
    nLastDiscrepancy_u8 = 0;
    bDoneFlag = FALSE;

    /* init ROM addresses */
    for (i = 0; i < NUM_DS1820; i++) {
        for (j = 0; j < 8; j++) {
            nRomAddr_au8[i][j] = 0x00;
        }
    }
    return ( DS1820_FindNextDevice());
}

/*******************************************************************************
 * FUNCTION:   DS1820_WriteEEPROM
 * PURPOSE:    Writes to the DS1820 EEPROM memory (2 bytes available).
 *
 * INPUT:      nTHigh         high byte of EEPROM
 *             nTLow          low byte of EEPROM
 * OUTPUT:     -
 * RETURN:     -
 ******************************************************************************/
void DS1820_WriteEEPROM(UINT8 nTHigh, UINT8 nTLow) {
    /* --- write to scratchpad ----------------------------------------------- */
    DS1820_Reset();
    DS1820_AddrDevice(DS1820_CMD_MATCHROM);
    DS1820_WriteByte(DS1820_CMD_WRITESCRPAD); /* start conversion */
    DS1820_WriteByte(nTHigh);
    DS1820_WriteByte(nTLow);

    DS1820_DelayUs(10);

    DS1820_Reset();
    DS1820_AddrDevice(DS1820_CMD_MATCHROM);
    DS1820_WriteByte(DS1820_CMD_COPYSCRPAD); /* start conversion */

    DS1820_DelayUs(100);
}

void DS1820_StartConversion(void) {

    /* --- start temperature conversion -------------------------------------- */
    DS1820_Reset();
    DS1820_AddrDevice(DS1820_CMD_MATCHROM); /* address the device */
    DS1820_output_high();
    DS1820_WriteByte(DS1820_CMD_CONVERTTEMP); /* start conversion */
}

void DS1820_GetAllTemps(void) {
    for (DS1820Selected = 0; DS1820Selected < DS1820Found; DS1820Selected++) {
        DS1820LastTemp[DS1820Selected] = DS1820_GetTempRaw();
    }
}

/*******************************************************************************
 * FUNCTION:   DS1820_GetTempRaw
 * PURPOSE:    Get temperature raw value from single DS1820 device.
 *
 *             Scratchpad Memory Layout
 *             Byte  Register
 *             0     Temperature_LSB
 *             1     Temperature_MSB
 *             2     Temp Alarm High / User Byte 1
 *             3     Temp Alarm Low / User Byte 2
 *             4     Reserved
 *             5     Reserved
 *             6     Count_Remain
 *             7     Count_per_C
 *             8     CRC
 *
 *             Temperature calculation for DS18S20 (Family Code 0x10):
 *             =======================================================
 *                                             (Count_per_C - Count_Remain)
 *             Temperature = temp_raw - 0.25 + ----------------------------
 *                                                     Count_per_C
 *
 *             Where temp_raw is the value from the temp_MSB and temp_LSB with
 *             the least significant bit removed (the 0.5C bit).
 *
 *
 *             Temperature calculation for DS18B20 (Family Code 0x28):
 *             =======================================================
 *                      bit7   bit6   bit5   bit4   bit3   bit2   bit1   bit0
 *             LSB      2^3    2^2    2^1    2^0    2^-1   2^-2   2^-3   2^-4
 *                      bit15  bit14  bit13  bit12  bit3   bit2   bit1   bit0
 *             MSB      S      S      S      S      S      2^6    2^5    2^4
 *
 *             The temperature data is stored as a 16-bit sign-extended two(c)s
 *             complement number in the temperature register. The sign bits (S)
 *             indicate if the temperature is positive or negative: for
 *             positive numbers S = 0 and for negative numbers S = 1.
 *
 * RETURN:     INT16         raw temperature value with a resolution
 *                            of 1/256?C
 ******************************************************************************/
INT16 DS1820_GetTempRaw(void) {
    UINT8 i;
    UINT16 temp_u16;
    UINT16 highres_u16;
    UINT8 scrpad[DS1820_SCRPADMEM_LEN];

    /* --- read sratchpad ---------------------------------------------------- */
    DS1820_Reset();
    DS1820_AddrDevice(DS1820_CMD_MATCHROM); /* address the device */
    DS1820_WriteByte(DS1820_CMD_READSCRPAD); /* read scratch pad */

    /* read scratch pad data */
    for (i = 0; i < DS1820_SCRPADMEM_LEN; i++) {
        scrpad[i] = DS1820_ReadByte();
    }


    /* --- calculate temperature --------------------------------------------- */
    /* Formular for temperature calculation: */
    /* Temp = Temp_read - 0.25 + ((Count_per_C - Count_Remain)/Count_per_C) */

    /* get raw value of temperature (0.5?C resolution) */
    temp_u16 = 0;
    temp_u16 = (UINT16) ((UINT16) scrpad[DS1820_REG_TEMPMSB] << 8);
    temp_u16 |= (UINT16) (scrpad[DS1820_REG_TEMPLSB]);

    if (nRomAddr_au8[DS1820Selected][0] == DS1820_FAMILY_CODE_DS18S20) {
        /* get temperature value in 1?C resolution */
        temp_u16 >>= 1;

        /* temperature resolution is TEMP_RES (0x100), so 1?C equals 0x100 */
        /* => convert to temperature to 1/256?C resolution */
        temp_u16 = ((UINT16) temp_u16 << 8);

        /* now substract 0.25?C */
        temp_u16 -= ((UINT16) TEMP_RES >> 2);

        /* now calculate high resolution */
        highres_u16 = scrpad[DS1820_REG_CNTPERSEC] - scrpad[DS1820_REG_CNTREMAIN];
        highres_u16 = ((UINT16) highres_u16 << 8);
        if (scrpad[DS1820_REG_CNTPERSEC]) {
            highres_u16 = highres_u16 / (UINT16) scrpad[DS1820_REG_CNTPERSEC];
        }

        /* now calculate result */
        highres_u16 = highres_u16 + temp_u16;
    } else {
        /* 12 bit temperature value has 0.0625?C resolution */
        /* shift left by 4 to get 1/256?C resolution */
        highres_u16 = temp_u16;
        highres_u16 <<= 4;
    }

    return (highres_u16);
}

/*******************************************************************************
 * FUNCTION:   DS1820_GetTempFloat
 * PURPOSE:    Converts internal temperature value to string (physical value).
 *
 * INPUT:      none
 * OUTPUT:     none
 * RETURN:     float          temperature value with as float value
 ******************************************************************************/
float DS1820_GetTempFloat(void) {
    return ((float) DS1820_GetTempRaw() / (float) TEMP_RES);
}

/*******************************************************************************
 * FUNCTION:   DS1820_GetTempString
 * PURPOSE:    Converts internal temperature value to string (physical value).
 *
 * INPUT:      tRaw_s16       internal temperature value
 * OUTPUT:     strTemp_pc     user string buffer to write temperature value
 * RETURN:     INT16         temperature value with an internal resolution
 *                            of TEMP_RES
 ******************************************************************************/
void DS1820_GetTempString(INT16 tRaw_s16, char *strTemp_pc) {
    INT16 tPhyLow_s16;
    INT8 tPhy_s8;

    /* convert from raw value (1/256?C resolution) to physical value */
    tPhy_s8 = (INT8) (tRaw_s16 / TEMP_RES);

    /* convert digits from raw value (1/256?C resolution) to physical value */
    /*tPhyLow_u16 = tInt_s16 % TEMP_RES;*/
    tPhyLow_s16 = tRaw_s16 & 0xFF; /* this operation is the same as */
    /* but saves flash memory tInt_s16 % TEMP_RES */
    tPhyLow_s16 = tPhyLow_s16 * 100;
    tPhyLow_s16 = (UINT16) tPhyLow_s16 / TEMP_RES;

    /* write physical temperature value to string */
#if defined(__18CXX)
    sprintf(strTemp_pc, (const far rom char*) "%d.%02d", tPhy_s8, tPhyLow_s16);
#elif defined (__PIC32MX__)
    //sprintf(strTemp_pc, (char *) "%d.%02d%cC", tPhy_s8, tPhyLow_s16,0xB0); // With ?C
    sprintf(strTemp_pc, (char *) "%d.%02d", tPhy_s8, tPhyLow_s16);
    //sprintf(strTemp_pc, "%u.%02u", 10, 20);
#endif
}

