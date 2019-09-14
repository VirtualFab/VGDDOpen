// *****************************************************************************
// Module for Microchip Graphics Library
// GOL Layer
// FontLed7Seg.c
// *****************************************************************************
// FileName:        FontLed7Seg.c
// Processor:       PIC24F, PIC24H, dsPIC, PIC32
// Compiler:        MPLAB C30, MPLAB C32
// Company:         VirtualFab, parts from Microchip Technology Incorporated
//
// VirtualFab's Software License Agreement:
// Copyright 2013-2016 Virtualfab - All rights reserved.
// VirtualFab licenses to you the right to use, modify, copy and distribute
// this software only in the event that you purchased at least one license of the VirtualFab's
// Visual Graphics Display Designer (VGDD) software.
//
// Usage of this software without owning a License for VGDD is explicitly forbidden.
//
// The Demo version of VGDD, from which this source may come, doesn't allow you to use it
// in any projects other than those created for test purposes, even if the code is manually created.
//
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
// OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// Microchip's Software License Agreement:
//
// Copyright 2012 Microchip Technology Inc.  All rights reserved.
// Microchip licenses to you the right to use, modify, copy and distribute
// Software only when embedded on a Microchip microcontroller or digital
// signal controller, which is integrated into your product or third party
// product (pursuant to the sublicense terms in the accompanying license
// agreement).
//
// You should refer to the license agreement accompanying this Software
// for additional information regarding your rights and obligations.
//
// SOFTWARE AND DOCUMENTATION ARE PROVIDED ?AS IS? WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY
// OF MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR
// PURPOSE. IN NO EVENT SHALL MICROCHIP OR ITS LICENSORS BE LIABLE OR
// OBLIGATED UNDER CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION,
// BREACH OF WARRANTY, OR OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT
// DAMAGES OR EXPENSES INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL,
// INDIRECT, PUNITIVE OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
// COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY
// CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF),
// OR OTHER SIMILAR COSTS.
//
// Date         Comment
// *****************************************************************************
//  2012/03/04	Start of Developing
//  2012/03/10  Merged with FontSeg - Now it is a Vector Font that renders with Bar or DrawPoly
//  2013/12/29  Removed DeviceIsBusy() in FontLed7SegPrintDigit 
// *****************************************************************************

#include "FontLed7Seg.h"
#include "Graphics/GOL.h"
#include "Graphics/DisplayDriver.h"

UINT8 FontLed7SegCurrentSizeX, FontLed7SegCurrentSizeY;

const unsigned char aLed7SegSegments[11] = {
    //GFEDCBA
    0b0000000, // Blank
    0b0111111, // 0
    0b0000110, // 1
    0b1011011, // 2
    0b1001111, // 3
    0b1100110, // 4
    0b1101101, // 5
    0b1111101, // 6
    0b0000111, // 7
    0b1111111, // 8
    0b1101111 // 9
};

BYTE aLed7SegCoords[7][14];
FontLed7SegStyle Led7SegCurrentStyle;

void FontLed7SegSetSize(UINT8 sizeY, UINT8 sizeX, UINT8 thickness, FontLed7SegStyle Style) {
    //UINT8 sizeX = sizeY * 5 / 7;
    FontLed7SegCurrentSizeX = sizeX;
    FontLed7SegCurrentSizeY = sizeY;
    Led7SegCurrentStyle = Style;

    switch (Style) {
        case FontLed7SegBar:
            // Segment A
            aLed7SegCoords[0][0] = 0;
            aLed7SegCoords[0][1] = 0;
            aLed7SegCoords[0][2] = FontLed7SegCurrentSizeX;
            aLed7SegCoords[0][3] = thickness;

            // Segment B
            aLed7SegCoords[1][0] = FontLed7SegCurrentSizeX - thickness;
            aLed7SegCoords[1][1] = 0;
            aLed7SegCoords[1][2] = FontLed7SegCurrentSizeX;
            aLed7SegCoords[1][3] = (FontLed7SegCurrentSizeY >>1) - (thickness>>1);

            // Segment C
            aLed7SegCoords[2][0] = FontLed7SegCurrentSizeX - thickness;
            aLed7SegCoords[2][1] = (FontLed7SegCurrentSizeY >>1) -(thickness>>1);
            aLed7SegCoords[2][2] = FontLed7SegCurrentSizeX;
            aLed7SegCoords[2][3] = FontLed7SegCurrentSizeY;

            // Segment D
            aLed7SegCoords[3][0] = 0;
            aLed7SegCoords[3][1] = FontLed7SegCurrentSizeY - thickness;
            aLed7SegCoords[3][2] = FontLed7SegCurrentSizeX;
            aLed7SegCoords[3][3] = FontLed7SegCurrentSizeY;

            // Segment E
            aLed7SegCoords[4][0] = 0;
            aLed7SegCoords[4][1] = (FontLed7SegCurrentSizeY >>1)-(thickness>>1);
            aLed7SegCoords[4][2] = thickness;
            aLed7SegCoords[4][3] = FontLed7SegCurrentSizeY;

            // Segment F
            aLed7SegCoords[5][0] = 0;
            aLed7SegCoords[5][1] = 0;
            aLed7SegCoords[5][2] = thickness;
            aLed7SegCoords[5][3] = (FontLed7SegCurrentSizeY >>1)-(thickness>>1);

            // Segment G
            aLed7SegCoords[6][0] = 0;
            aLed7SegCoords[6][1] = (FontLed7SegCurrentSizeY >>1)-(thickness>>1);
            aLed7SegCoords[6][2] = FontLed7SegCurrentSizeX ;
            aLed7SegCoords[6][3] = (FontLed7SegCurrentSizeY >>1) + (thickness>>1);

            break;

        case FontLed7SegPoly:
            FontLed7SegCurrentSizeY=FontLed7SegCurrentSizeY*0.7;
            
            // Segment A
            aLed7SegCoords[0][0] = FontLed7SegCurrentSizeX * 2.8 / 10;
            aLed7SegCoords[0][1] = FontLed7SegCurrentSizeY / 10;
            aLed7SegCoords[0][2] = FontLed7SegCurrentSizeX * 10 / 10;
            aLed7SegCoords[0][3] = FontLed7SegCurrentSizeY / 10;
            aLed7SegCoords[0][4] = FontLed7SegCurrentSizeX * 8.8 / 10;
            aLed7SegCoords[0][5] = FontLed7SegCurrentSizeY * 2.0 / 10;
            aLed7SegCoords[0][6] = FontLed7SegCurrentSizeX * 3.8 / 10;
            aLed7SegCoords[0][7] = FontLed7SegCurrentSizeY * 2.0 / 10;
            aLed7SegCoords[0][8] = FontLed7SegCurrentSizeX * 2.8 / 10;
            aLed7SegCoords[0][9] = FontLed7SegCurrentSizeY / 10;

            // Segment B
            aLed7SegCoords[1][0] = FontLed7SegCurrentSizeX * 10 / 10;
            aLed7SegCoords[1][1] = FontLed7SegCurrentSizeY * 1.4 / 10;
            aLed7SegCoords[1][2] = FontLed7SegCurrentSizeX * 9.3 / 10;
            aLed7SegCoords[1][3] = FontLed7SegCurrentSizeY * 6.8 / 10;
            aLed7SegCoords[1][4] = FontLed7SegCurrentSizeX * 8.4 / 10;
            aLed7SegCoords[1][5] = FontLed7SegCurrentSizeY * 6.4 / 10;
            aLed7SegCoords[1][6] = FontLed7SegCurrentSizeX * 9.0 / 10;
            aLed7SegCoords[1][7] = FontLed7SegCurrentSizeY * 2.2 / 10;
            aLed7SegCoords[1][8] = FontLed7SegCurrentSizeX * 10 / 10;
            aLed7SegCoords[1][9] = FontLed7SegCurrentSizeY * 1.4 / 10;

            // Segment C
            aLed7SegCoords[2][0] = FontLed7SegCurrentSizeX * 9.2 / 10;
            aLed7SegCoords[2][1] = FontLed7SegCurrentSizeY * 7.2 / 10;
            aLed7SegCoords[2][2] = FontLed7SegCurrentSizeX * 8.5 / 10;
            aLed7SegCoords[2][3] = FontLed7SegCurrentSizeY * 12.7 / 10;
            aLed7SegCoords[2][4] = FontLed7SegCurrentSizeX * 7.7 / 10;
            aLed7SegCoords[2][5] = FontLed7SegCurrentSizeY * 11.9 / 10;
            aLed7SegCoords[2][6] = FontLed7SegCurrentSizeX * 8.2 / 10;
            aLed7SegCoords[2][7] = FontLed7SegCurrentSizeY * 7.7 / 10;
            aLed7SegCoords[2][8] = FontLed7SegCurrentSizeX * 9.2 / 10;
            aLed7SegCoords[2][9] = FontLed7SegCurrentSizeY * 7.2 / 10;

            // Segment D
            aLed7SegCoords[3][0] = FontLed7SegCurrentSizeX * 7.4 / 10;
            aLed7SegCoords[3][1] = FontLed7SegCurrentSizeY * 12.1 / 10;
            aLed7SegCoords[3][2] = FontLed7SegCurrentSizeX * 8.4 / 10;
            aLed7SegCoords[3][3] = FontLed7SegCurrentSizeY * 13.0 / 10;
            aLed7SegCoords[3][4] = FontLed7SegCurrentSizeX * 1.1 / 10;
            aLed7SegCoords[3][5] = FontLed7SegCurrentSizeY * 13.0 / 10;
            aLed7SegCoords[3][6] = FontLed7SegCurrentSizeX * 2.2 / 10;
            aLed7SegCoords[3][7] = FontLed7SegCurrentSizeY * 12.1 / 10;
            aLed7SegCoords[3][8] = FontLed7SegCurrentSizeX * 7.4 / 10;
            aLed7SegCoords[3][9] = FontLed7SegCurrentSizeY * 12.1 / 10;

            // Segment E
            aLed7SegCoords[4][0] = FontLed7SegCurrentSizeX * 2.2 / 10;
            aLed7SegCoords[4][1] = FontLed7SegCurrentSizeY * 11.8 / 10;
            aLed7SegCoords[4][2] = FontLed7SegCurrentSizeX * 1.0 / 10;
            aLed7SegCoords[4][3] = FontLed7SegCurrentSizeY * 12.7 / 10;
            aLed7SegCoords[4][4] = FontLed7SegCurrentSizeX * 1.8 / 10;
            aLed7SegCoords[4][5] = FontLed7SegCurrentSizeY * 7.2 / 10;
            aLed7SegCoords[4][6] = FontLed7SegCurrentSizeX * 2.8 / 10;
            aLed7SegCoords[4][7] = FontLed7SegCurrentSizeY * 7.7 / 10;
            aLed7SegCoords[4][8] = FontLed7SegCurrentSizeX * 2.2 / 10;
            aLed7SegCoords[4][9] = FontLed7SegCurrentSizeY * 11.8 / 10;

            // Segment
            aLed7SegCoords[5][0] = FontLed7SegCurrentSizeX * 3.0 / 10;
            aLed7SegCoords[5][1] = FontLed7SegCurrentSizeY * 6.2 / 10;
            aLed7SegCoords[5][2] = FontLed7SegCurrentSizeX * 1.9 / 10;
            aLed7SegCoords[5][3] = FontLed7SegCurrentSizeY * 6.8 / 10;
            aLed7SegCoords[5][4] = FontLed7SegCurrentSizeX * 2.7 / 10;
            aLed7SegCoords[5][5] = FontLed7SegCurrentSizeY * 1.3 / 10;
            aLed7SegCoords[5][6] = FontLed7SegCurrentSizeX * 3.6 / 10;
            aLed7SegCoords[5][7] = FontLed7SegCurrentSizeY * 2.2 / 10;
            aLed7SegCoords[5][8] = FontLed7SegCurrentSizeX * 3.0 / 10;
            aLed7SegCoords[5][9] = FontLed7SegCurrentSizeY * 6.2 / 10;

            // Segment G
            aLed7SegCoords[6][0] = FontLed7SegCurrentSizeX * 2.0 / 10;
            aLed7SegCoords[6][1] = FontLed7SegCurrentSizeY * 7.0 / 10;
            aLed7SegCoords[6][2] = FontLed7SegCurrentSizeX * 3.1 / 10;
            aLed7SegCoords[6][3] = FontLed7SegCurrentSizeY * 6.5 / 10;
            aLed7SegCoords[6][4] = FontLed7SegCurrentSizeX * 8.3 / 10;
            aLed7SegCoords[6][5] = FontLed7SegCurrentSizeY * 6.5 / 10;
            aLed7SegCoords[6][6] = FontLed7SegCurrentSizeX * 9.0 / 10;
            aLed7SegCoords[6][7] = FontLed7SegCurrentSizeY * 7.0 / 10;
            aLed7SegCoords[6][8] = FontLed7SegCurrentSizeX * 8.2 / 10;
            aLed7SegCoords[6][9] = FontLed7SegCurrentSizeY * 7.5 / 10;
            aLed7SegCoords[6][10] = FontLed7SegCurrentSizeX * 2.9 / 10;
            aLed7SegCoords[6][11] = FontLed7SegCurrentSizeY * 7.5 / 10;
            aLed7SegCoords[6][12] = FontLed7SegCurrentSizeX * 2.0 / 10;
            aLed7SegCoords[6][13] = FontLed7SegCurrentSizeY * 7.0 / 10;

            break;
    }
}


WORD FontLed7SegPrintValue(INT16 Value, INT16 LastValue, UINT16 x, UINT16 y, BYTE MaxDigits, BYTE gap, WORD BackColor, WORD ForeColor, BOOL OnlyUpdate) {
    BYTE Digit;//, OldDigit;
    BYTE CurrentDigit = 0;
    UINT16 TotalWidth = (FontLed7SegCurrentSizeX + gap) * MaxDigits;//+gap<<1);

    while (1) {
        CurrentDigit++;
        //OldDigit = (LastValue % 10);
        Digit = (Value % 10);
        //if (OldDigit != Digit || !OnlyUpdate) {
        SetColor(BackColor);
        if (!FontLed7SegPrintDigit('8', x + TotalWidth - (FontLed7SegCurrentSizeX + gap) * CurrentDigit, y))
            return (0);
        SetColor(ForeColor);
        if (CurrentDigit == 1 || Digit > 0||Value>9) {
            if (!FontLed7SegPrintDigit(Digit + '0', x + TotalWidth - (FontLed7SegCurrentSizeX + gap) * CurrentDigit, y))
                return (0);
        }
        //}
        Value /= 10;
        if (/*(Value == 0) ||*/ (CurrentDigit >= MaxDigits))
            return (1);
    }
}

WORD FontLed7SegPrintDigit(char d, UINT16 x, UINT16 y) {
    UINT8 segs, i, j, numPoints;
    SHORT aSegPoly[14];
    segs = d - '0' + 1;
    if (segs > 10) segs = 0;
    for (i = 0; i < 7; i++) {
        if (aLed7SegSegments[segs] & (1 << i)) {
            switch (Led7SegCurrentStyle) {
                case FontLed7SegBar:
                    while (!Bar(x + aLed7SegCoords[i][0], y + aLed7SegCoords[i][1], x + aLed7SegCoords[i][2], y + aLed7SegCoords[i][3]));
                    break;
                case FontLed7SegPoly:
                    numPoints = (i == 6 ? 7 : 5);
                    for (j = 0; j < (numPoints << 1); j += 2) {
                        aSegPoly[j] = aLed7SegCoords[i][j] + x;
                        aSegPoly[j + 1] = aLed7SegCoords[i][j + 1] + y;
                    }
                    while (!DrawPoly(numPoints, aSegPoly));
                    break;
            }
        }
    }
    return (1);
}
