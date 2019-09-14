#ifndef RTCC_H
#define	RTCC_H

#include <stdint.h>
#include <stdbool.h>

typedef struct
{
    bool bcdFormat;
    uint8_t year;
    uint8_t month;
    uint8_t day;
    uint8_t weekday;
    uint8_t hour;
    uint8_t minute;
    uint8_t second;
} BSP_RTCC_DATETIME;

void BSP_RTCC_Initialize (BSP_RTCC_DATETIME * value);
void BSP_RTCC_TimeGet (BSP_RTCC_DATETIME * value);
// GetTimestamp will be called by the FILEIO library when it needs a timestamp for a file
//void GetTimestamp (FILEIO_TIMESTAMP * timeStamp);
// Helper function to initialize the RTCC from the compiler timestamp
void RTCCInit (void);

#endif
