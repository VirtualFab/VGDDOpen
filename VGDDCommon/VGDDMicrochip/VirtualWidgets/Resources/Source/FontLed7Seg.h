#include "GenericTypeDefs.h"

extern UINT8 FontLed7SegCurrentSizeX, FontLed7SegCurrentSizeY;

typedef enum {
    FontLed7SegBar,
    FontLed7SegPoly
} FontLed7SegStyle;

void FontLed7SegSetSize(UINT8 sizeY, UINT8 sizeX, UINT8 thickness, FontLed7SegStyle Style);
WORD FontLed7SegPrintValue(INT16 Value, INT16 LastValue, UINT16 x, UINT16 y, BYTE MaxDigits, BYTE gap, WORD BackColor, WORD ForeColor,BOOL OnlyUpdate);
WORD FontLed7SegPrintDigit(char d, UINT16 x, UINT16 y);
