/**
*
* \brief The driver of serial RAM 23K256
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

#define USE_AND_OR /* To enable AND_OR mask setting */
#include "SpiRAM.h"

void SpiRAM_Init(void){
    uint32_t SPICON1Value;
    uint32_t SPICON2Value;
    uint32_t SPISTATValue;	
    CloseSPIx();    //Disbale SPI1 mdolue if enabled previously
	SPICON1Value =ENABLE_SCK_PIN | ENABLE_SDO_PIN | SPI_MODE16_OFF 
                | MASTER_ENABLE_ON | SEC_PRESCAL_1_1 | PRI_PRESCAL_1_1 
                | CLK_POL_ACTIVE_HIGH | SPI_CKE_ON | SLAVE_ENABLE_OFF | SPI_SMP_ON ; //16MHZ               
    SPICON2Value = FRAME_ENABLE_OFF | FRAME_SYNC_OUTPUT | FIFO_BUFFER_DISABLE;
    SPISTATValue =  SPI_ENABLE | SPI_IDLE_CON | SPI_RX_OVFLOW_CLR;
    OpenSPIx(SPICON1Value,SPICON2Value,SPISTATValue );
    EPD_flash_cs_high();
}

void SRAMWriteStatusReg(uint8_t RegValue){
    EPD_flash_cs_low();   
    WriteSPIx(SRAMWRSR);   
    while(!SPIx_Rx_Buf_Full);
    ReadSPIx();
    WriteSPIx(RegValue);
    while(!SPIx_Rx_Buf_Full);
    ReadSPIx();
    EPD_flash_cs_high();
}
uint8_t SRAMReadStatusReg(void){
    uint8_t ReadData=0;
    EPD_flash_cs_low();
    WriteSPIx(SRAMRDSR);
    while(!SPIx_Rx_Buf_Full);
    ReadData = ReadSPIx();
    WriteSPIx(DummyByte);
    while(!SPIx_Rx_Buf_Full);
    ReadData = ReadSPIx();
    EPD_flash_cs_high();
    return ReadData;
}
void SRAMCommand(unsigned int address,unsigned char RWCmd)
{
	unsigned char ReadData;
	//Send Read or Write command to SRAM
	WriteSPIx(RWCmd);
	while(!SPIx_Rx_Buf_Full);
	ReadData = ReadSPIx();
	//Send High byte of address to SRAM
	WriteSPIx((uint8_t)(address >> 8));
	while(!SPIx_Rx_Buf_Full);
	ReadData = ReadSPIx();
	//Send Low byte of address to SRAM
	WriteSPIx((uint8_t)address);
	while(!SPIx_Rx_Buf_Full);
	ReadData = ReadSPIx();
}
uint8_t SRAMWriteByte(unsigned int address,unsigned char WriteData)
{
	SRAMWriteStatusReg(SRAMByteMode);
        EPD_flash_cs_low();
	//Send Write command to SRAM along with address
	SRAMCommand(address,SRAMWrite);
	//Send Data to be written to SRAM
	WriteSPIx(WriteData);
	while(!SPIx_Rx_Buf_Full);
	WriteData = ReadSPIx();
	 EPD_flash_cs_high();
	return(0);			//Return non -ve number indicating success
}
uint8_t SRAMReadByte(unsigned int address)
{
	unsigned char ReadData;
	SRAMWriteStatusReg(SRAMByteMode);
	EPD_flash_cs_low();
	//Send Read command to SRAM along with address
	SRAMCommand(address,SRAMRead);
	//Send dummy data so SRAM can put desired Data read from SRAM
	WriteSPIx(DummyByte);
	while(!SPIx_Rx_Buf_Full);
	ReadData = ReadSPIx();
	 EPD_flash_cs_high();
	return(ReadData);
}
uint8_t SRAMWritePage(unsigned int address, unsigned char *WriteData)
{
	unsigned char ReadData,WriteCnt;
	SRAMWriteStatusReg(SRAMPageMode);
	//Send Write command to SRAM along with address
	EPD_flash_cs_low();
	SRAMCommand(address,SRAMWrite);
	//Send Data to be written to SRAM
	for(WriteCnt = 0;WriteCnt < SRAMPageSize;WriteCnt++)
	{
		WriteSPIx(*WriteData++);
		while(!SPIx_Rx_Buf_Full);
		ReadData = ReadSPIx();
	}
	 EPD_flash_cs_high();
	return(WriteCnt);			//Return no# of bytes written to SRAM
}
uint8_t SRAMReadPage(unsigned int address,unsigned char *ReadData)
{
	unsigned char ReadCnt;
	SRAMWriteStatusReg(SRAMPageMode);
	//Send Read command to SRAM along with address
	EPD_flash_cs_low();
	SRAMCommand(address,SRAMRead);
	//Send dummy data so SRAM can put desired Data read from SRAM
	for(ReadCnt = 0; ReadCnt < SRAMPageSize; ReadCnt++)
	{
		WriteSPIx(DummyByte);
		while(!SPIx_Rx_Buf_Full);
		*ReadData++ = ReadSPIx();
	}
	EPD_flash_cs_high();
	return(ReadCnt);			//Return no# of bytes read from SRAM
}

uint8_t SRAMWriteSeq(unsigned int address, unsigned char *WriteData,unsigned int WriteCnt)
{
	unsigned char DummyRead;
	SRAMWriteStatusReg(SRAMSeqMode);
	//Send Write command to SRAM along with address
    EPD_flash_cs_low();
	SRAMCommand(address,SRAMWrite);
	//Send Data to be written to SRAM
	for(;WriteCnt > 0;WriteCnt--)
	{
		WriteSPIx(*WriteData++);
		while(!SPIx_Rx_Buf_Full);
		DummyRead = ReadSPIx();
	}
	EPD_flash_cs_high();
	return(0);			//Return non -ve nuber indicating success
}

uint8_t SRAMReadSeq(unsigned int address,unsigned char *ReadData,unsigned int ReadCnt)
{
	SRAMWriteStatusReg(SRAMSeqMode);
	//Send Read command to SRAM along with address
	EPD_flash_cs_low();
	SRAMCommand(address,SRAMRead);
	//Send dummy data so SRAM can put desired Data read from SRAM
	for(; ReadCnt > 0; ReadCnt--)
	{
		WriteSPIx(DummyByte);
		while(!SPIx_Rx_Buf_Full);
		*ReadData++ = ReadSPIx();
	}
	EPD_flash_cs_high();
	return(0);			//Return non -ve nuber indicating success
}


