
#include <xc.h>
#include "rtcc.h"
#include <stdint.h>
#include <stdbool.h>
#include <string.h>

uint8_t BSP_RTCC_DecToBCD (uint8_t value);
uint8_t BSP_RTCC_BCDToDec (uint8_t value);

void BSP_RTCC_Initialize (BSP_RTCC_DATETIME * value)
{
   // Turn on the secondary oscillator
   __builtin_write_OSCCONL(0x02);

   // Set the RTCWREN bit
   __builtin_write_RTCWEN();

   RCFGCALbits.RTCPTR0 = 1;
   RCFGCALbits.RTCPTR1 = 1;

   // Set it to the correct time
   if (value->bcdFormat)
   {
       RTCVAL = 0x0000 | value->year;
       RTCVAL = ((uint16_t)(value->month) << 8) | value->day;
       RTCVAL = ((uint16_t)(value->weekday) << 8) | value->hour;
       RTCVAL = ((uint16_t)(value->minute) << 8) | value->second;
   }
   else
   {
       // Set (Reserved : year)
       RTCVAL = BSP_RTCC_DecToBCD (value->year);
       // Set (month : day)
       RTCVAL = (BSP_RTCC_DecToBCD (value->month) << 8) | BSP_RTCC_DecToBCD(value->day);
       // Set (weekday : hour)
       RTCVAL = (BSP_RTCC_DecToBCD (value->weekday) << 8) | BSP_RTCC_DecToBCD(value->hour);
       // Set (minute : second)
       RTCVAL = (BSP_RTCC_DecToBCD (value->minute) << 8) | BSP_RTCC_DecToBCD(value->second);
   }

   // Enable RTCC, clear RTCWREN
   RCFGCAL = 0x8000;
}

void BSP_RTCC_TimeGet (BSP_RTCC_DATETIME * value)
{
    uint16_t registerValue;
    bool checkValue;

    RCFGCALbits.RTCPTR0 = 1;
    RCFGCALbits.RTCPTR1 = 1;

    checkValue = RCFGCALbits.RTCSYNC;

    registerValue = RTCVAL;
    value->year = registerValue & 0xFF;
    registerValue = RTCVAL;
    value->month = registerValue >> 8;
    value->day = registerValue & 0xFF;
    registerValue = RTCVAL;
    value->weekday = registerValue >> 8;
    value->hour = registerValue & 0xFF;
    registerValue = RTCVAL;
    value->minute = registerValue >> 8;
    value->second = registerValue & 0xFF;

    if (checkValue)
    {
        BSP_RTCC_DATETIME tempValue;

        do
        {
            memcpy (&tempValue, value, sizeof (BSP_RTCC_DATETIME));

            RCFGCALbits.RTCPTR0 = 1;
            RCFGCALbits.RTCPTR1 = 1;

            value->year = RTCVAL;
            registerValue = RTCVAL;
            value->month = registerValue >> 8;
            value->day = registerValue & 0xFF;
            registerValue = RTCVAL;
            value->weekday = registerValue >> 8;
            value->hour = registerValue & 0xFF;
            registerValue = RTCVAL;
            value->minute = registerValue >> 8;
            value->second = registerValue & 0xFF;

        } while (memcmp (value, &tempValue, sizeof (BSP_RTCC_DATETIME)));
    }

    if (!value->bcdFormat)
    {
        value->year = BSP_RTCC_BCDToDec (value->year);
        value->month = BSP_RTCC_BCDToDec (value->month);
        value->day = BSP_RTCC_BCDToDec (value->day);
        value->weekday = BSP_RTCC_BCDToDec (value->weekday);
        value->hour = BSP_RTCC_BCDToDec (value->hour);
        value->minute = BSP_RTCC_BCDToDec (value->minute);
        value->second = BSP_RTCC_BCDToDec (value->second);
    }
}

// Note : value must be < 100
uint8_t BSP_RTCC_DecToBCD (uint8_t value)
{
    return (((value / 10)) << 4) | (value % 10);
}

uint8_t BSP_RTCC_BCDToDec (uint8_t value)
{
    return ((value >> 4) * 10) + (value & 0x0F);
}

// Helper function to initialize the RTCC module.
// This function will use the compiler-generated timestamp to initialize the RTCC.
void RTCCInit (void)
{
    BSP_RTCC_DATETIME dateTime;
    uint8_t weekday;
    uint8_t month;
    uint8_t y;
    uint8_t dateTable[] = {0, 3, 3, 6, 1, 4, 6, 2, 5, 0, 3, 5};

    dateTime.bcdFormat = true;

    dateTime.second =  (((__TIME__[6]) & 0x0f) << 4) | ((__TIME__[7]) & 0x0f);
    dateTime.minute =  (((__TIME__[3]) & 0x0f) << 4) | ((__TIME__[4]) & 0x0f);
    dateTime.hour = (((__TIME__[0]) & 0x0f) << 4) | ((__TIME__[1]) & 0x0f);
    dateTime.day =  (((__DATE__[4]) & 0x0f) << 4) | ((__DATE__[5]) & 0x0f);
    dateTime.year = (((__DATE__[9]) & 0x0f) << 4) | ((__DATE__[10]) & 0x0f);

    //Set the month
    switch(__DATE__[0])
    {
        case 'J':
            //January, June, or July
            switch(__DATE__[1])
            {
                case 'a':
                    //January
                    month = 0x01;
                    break;
                case 'u':
                    switch(__DATE__[2])
                    {
                        case 'n':
                            //June
                            month = 0x06;
                            break;
                        case 'l':
                            //July
                            month = 0x07;
                            break;
                    }
                    break;
            }
            break;
        case 'F':
            month = 0x02;
            break;
        case 'M':
            //March,May
            switch(__DATE__[2])
            {
                case 'r':
                    //March
                    month = 0x03;
                    break;
                case 'y':
                    //May
                    month = 0x05;
                    break;
            }
            break;
        case 'A':
            //April, August
            switch(__DATE__[1])
            {
                case 'p':
                    //April
                    month = 0x04;
                    break;
                case 'u':
                    //August
                    month = 0x08;
                    break;
            }
            break;
        case 'S':
            month = 0x09;
            break;
        case 'O':
            month = 0x10;
            break;
        case 'N':
            month = 0x11;
            break;
        case 'D':
            month = 0x12;
            break;
    }

    dateTime.month = month;

    // Start with weekday = 6.  This value is valid for this algorithm for this century.
    weekday = 6;
    // y = year
    y = ((dateTime.year >> 4) * 10) + (dateTime.year & 0x0f);
    // Weekday = base day + year + x number of leap days
    weekday += y + (y / 4);
    // If the current year is a leap year but it's not March, subtract 1 from the date
    if (((y % 4) == 0) && (month < 3))
    {
        weekday -= 1;
    }
    // Add an offset based on the current month
    weekday += dateTable[month - 1];
    // Add the current day in the month
    weekday += ((dateTime.day >> 4) * 10) + (dateTime.day & 0x0f);
    weekday = weekday % 7;

    dateTime.weekday = weekday;

    // Initialize the RTCC with the calculated date/time.
    BSP_RTCC_Initialize (&dateTime);
}

//void GetTimestamp (FILEIO_TIMESTAMP * timeStamp)
//{
//    BSP_RTCC_DATETIME dateTime;
//
//    dateTime.bcdFormat = false;
//
//    BSP_RTCC_TimeGet(&dateTime);
//
//    timeStamp->timeMs = 0;
//    timeStamp->time.bitfield.hours = dateTime.hour;
//    timeStamp->time.bitfield.minutes = dateTime.minute;
//    timeStamp->time.bitfield.secondsDiv2 = dateTime.second / 2;
//
//    timeStamp->date.bitfield.day = dateTime.day;
//    timeStamp->date.bitfield.month = dateTime.month;
//    // Years in the RTCC module go from 2000 to 2099.  Years in the FAT file system go from 1980-2108.
//    timeStamp->date.bitfield.year = dateTime.year + 20;;
//}