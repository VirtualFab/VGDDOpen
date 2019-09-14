/**
*
* \brief The definition of serial RAM 23K256
*
* Copyright (c) 2013-2014 Pervasive Displays Inc. All rights reserved.
*
*  Authors: Pervasive Displays Inc.
*
*  Redistribution and use in source and binary forms, with or without
*  modification, are permitted provided that the following conditions
*  are met:
*
*  1. Redistributions of source code must retain the above copyright
*     notice, this list of conditions and the following disclaimer.
*  2. Redistributions in binary form must reproduce the above copyright
*     notice, this list of conditions and the following disclaimer in
*     the documentation and/or other materials provided with the
*     distribution.
*
*  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
*  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
*  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
*  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
*  OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
*  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
*  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
*  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
*  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
*  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
*  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#ifndef __SPIRAM_H
#define __SPIRAM_H
#include "Pervasive_Displays_small_EPD.h"



#define CloseSPIx           CloseSPI2
#define OpenSPIx            OpenSPI2
#define WriteSPIx           WriteSPI2
#define ReadSPIx            ReadSPI2
#define SPIx_Rx_Buf_Full    SPI2_Rx_Buf_Full

#define SRAMRead        0x03     //Read Command for SRAM
#define SRAMWrite       0x02     //Write Command for SRAM
#define SRAMRDSR        0x05     //Read the status register
#define SRAMWRSR        0x01     //Write the status register
#define SRAMByteMode    0x01
#define SRAMPageMode    0x81
#define	SRAMSeqMode     0x41
#define	SRAMPageSize    32
#define	DummyByte       0xFF
void SpiRAM_Init(void);
void SRAMWriteStatusReg(uint8_t RegValue);
uint8_t SRAMReadStatusReg(void);
void SRAMCommand(unsigned int address,unsigned char RWCmd);
uint8_t SRAMWriteByte(unsigned int address,unsigned char WriteData);
uint8_t SRAMReadByte(unsigned int address);
uint8_t SRAMWritePage(unsigned int address, unsigned char *WriteData);
uint8_t SRAMReadPage(unsigned int address,unsigned char *ReadData);
uint8_t SRAMWriteSeq(unsigned int address, unsigned char *WriteData,unsigned int WriteCnt);
uint8_t SRAMReadSeq(unsigned int address,unsigned char *ReadData,unsigned int ReadCnt);
#endif
