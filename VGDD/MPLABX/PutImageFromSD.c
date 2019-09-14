/*****************************************************************************
 *  Module for Microchip Graphics Library
 *  Large Bitmaps on SD handler
 *  This module takes care of handling Putimage calls in order to load bitmaps
 *  from SD/MMC memory card.
 *
 * Requisites:
 *  The following defines should be placed in HardwareProfile.h AFTER the other
 *  display defines (DISPLAY_CONTROLLER, DISP_ORIENTATION, DISP_HOR_RESOLUTION...)
 *
 * #define SD_IMAGEDIR "\\img" // replace img with your directory
 * #ifdef _GRAPHICS_H
 * #include "PutImageFromSD.h"
 * #endif
 *
 *****************************************************************************
 * FileName:        PutImageFromSD.c
 * Dependencies:    Graphics.h HardwareProfile.h FSIO.h
 * Processor:       PIC24, PIC32
 * Compiler:        MPLAB C30, MPLAB C32
 * Linker:          MPLAB LINK30, MPLAB LINK32
 * Company:         VirtualFab
 *
 * Software License Agreement
 *
 * Copyright (c) 2011 VirtualFab  All rights reserved.
 * VirtualFab licenses to you the right to use, modify, copy and distribute
 * this Software as you wish
 *
 * SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY
 * OF MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR
 * PURPOSE. IN NO EVENT SHALL VIRTUALFAB OR ITS LICENSORS BE LIABLE OR
 * OBLIGATED UNDER CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION,
 * BREACH OF WARRANTY, OR OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT
 * DAMAGES OR EXPENSES INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL,
 * INDIRECT, PUNITIVE OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 * COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY
 * CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF),
 * OR OTHER SIMILAR COSTS.
 *
 * Author               Date        Comment
 *****************************************************************************
 * VirtualFab           2011/06/17	Version 1.0 release
 *                      2012/04/23      Version 1.1 Release - not failing if media not mounted
 *                      2012/10/17      Version 1.2 Release - Working flawlessly with latest MAL
 *                      2012/05/17      Version 1.3 Release - Integrated with FileSystem.c to support FSIO/FatFs/USB
 *****************************************************************************/

#include "FileSystem.h"
#include "PutImageFromSD.h"

FILE_HANDLE SDImgFileHandler = NULL;
IMAGE_ON_SD *SDImgCurrent = NULL;

/*********************************************************************
 * Function: WORD ExternalMemoryCallback(IMAGE_EXTERNAL *memory, LONG offset, WORD nCount, void *buffer)
 *
 * PreCondition: FSInit() already done
 *
 * Input: memory - The pointer to image object for the Bitmap to read
 *        offset - start position in the file to be read
 *        nCount - number of bytes to read
 *        buffer - Pointer to the buffer
 *
 * Output: number of bytes read
 *         If error: returns 0xffff
 *
 * Side Effects: none
 *
 * Overview: Reads image from SD and outputs image starting from left,top coordinates
 *
 * Note: image must be located on SD card
 *
 ********************************************************************/
WORD ExternalMemoryCallback(IMAGE_EXTERNAL *memory, LONG offset, WORD nCount, void *buffer) {
    IMAGE_ON_SD *img;
    char *ImageName;

    if (FileMediaDetect()==0) {
        return 0;
    }

    img = (IMAGE_ON_SD *) memory;
    if (SDImgCurrent != img) {
        SDImgCurrent = img;
        if (SDImgFileHandler != NULL) {
            FileClose(SDImgFileHandler);
        }
        ImageName = img->filename;
        if (FileChDir(SD_IMAGEDIR) != 0) {
            return 0;
        }
        // Open image file on SD
        SDImgFileHandler = FileOpen(ImageName, "r");
        if (SDImgFileHandler == NULL)
            return 0;
    }
    if (FileSeek(SDImgFileHandler, offset, SEEK_SET) != 0) // Seek from start of file
        return 0;

    if (FileRead(buffer, 1, nCount, SDImgFileHandler) != nCount)
        return 0;

    return (nCount);
}
