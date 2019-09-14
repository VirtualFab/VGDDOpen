/*********************************************************************
 *
 *     File system access interface layer Source File
 *
 *********************************************************************
 * FileName:        FileSystem.c
 * Description:     File system access interface layer
 * Processor:       PIC18, PIC24F, PIC24H, dsPIC30F, dsPIC33F, PIC32
 * Compiler:        Microchip C32 v1.00 or higher
 *					Microchip C30 v3.01 or higher
 *					Microchip C18 v3.20 or higher
 *					HI-TECH PICC-18 STD 9.50PL3 or higher
 * Company:         Microchip Technology, Inc.
 *
 * Software License Agreement
 *
 * Copyright (C) 2008 Microchip Technology Inc.  All rights 
 * reserved.
 *
 * Microchip licenses to you the right to use, modify, copy, and 
 * distribute: 
 * (i)  the Software when embedded on a Microchip microcontroller or 
 *      digital signal controller product ("Device") which is 
 *      integrated into Licensee's product; or
 * (ii) ONLY the Software driver source files ENC28J60.c and 
 *      ENC28J60.h ported to a non-Microchip device used in 
 *      conjunction with a Microchip ethernet controller for the 
 *      sole purpose of interfacing with the ethernet controller. 
 *
 * You should refer to the license agreement accompanying this 
 * Software for additional information regarding your rights and 
 * obligations.
 *
 * THE SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT 
 * WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT SHALL 
 * MICROCHIP BE LIABLE FOR ANY INCIDENTAL, SPECIAL, INDIRECT OR 
 * CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, COST OF 
 * PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR SERVICES, ANY CLAIMS 
 * BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE 
 * THEREOF), ANY CLAIMS FOR INDEMNITY OR CONTRIBUTION, OR OTHER 
 * SIMILAR COSTS, WHETHER ASSERTED ON THE BASIS OF CONTRACT, TORT 
 * (INCLUDING NEGLIGENCE), BREACH OF WARRANTY, OR OTHERWISE.
 *
 * Author               Date    	Comment
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Aseem Swalah         7/31/08         Original
 * Amit Shirbhate       7/18/09         Modified
 * VirtualFab           5/19/2013       Modified for FatFs support + added various missing functions
 ********************************************************************/
#include "FileSystem.h"
BOOL FileSysInitLock=FALSE;
BOOL MemInterfaceAttached=FALSE;

#if defined(FILESYSTEM_USE_FATFS)
#include "ff.h"
#include "diskio.h"
FATFS Fatfs; //File system object
FIL aFil[FS_MAX_FILES_OPEN]; // Array that contains file handle information (static allocation)
FILINFO aFilInfo[FS_MAX_FILES_OPEN]; // Array that contains file information (static allocation)
BYTE aFileSlot[FS_MAX_FILES_OPEN]; // Array that indicates which elements of aFil are available for use
DIR CurrentDir;
#endif

void FileCheckMedia(void) {
    MemInterfaceAttached = FileMediaDetect(); // Check for SD card/USB Thumb drive if attached
    if (MemInterfaceAttached == TRUE && FileSysInitLock == FALSE) {
        FileSysInitLock = FileSystemInit();
    } else if (MemInterfaceAttached == FALSE) {
        FileSysInitLock = FALSE;
    }
}

int FileSystemInit(void) {
#if defined(FILESYSTEM_USE_FATFS)
    DSTATUS result;
    result = disk_initialize(0); //Initialize Disk
    if (result == RES_OK) {
        result = f_mount(0, &Fatfs); //Mount Filesystem
        if (result == RES_OK) {
            return TRUE;
        } else {
            return FALSE;
        }
    } else {
        return FALSE;
    }
#elif defined(FILESYSTEM_USE_MPFS2)
    MPFSInit();
#elif defined(FILESYSTEM_USE_MDD)
#if defined(STACK_USE_MDD) // Needed only by TCPIP stack
#include "TCPIP Stack/_HTTP2.h"
    extern HTTP_CONN curHTTP; // Needed by HTTP2_MDD in case of re-inserting the SD
    curHTTP.CurWorkDirChangedToMddRootPath = FALSE;
#endif
    return FSInit();
#endif

    return FALSE;
}

FILE_HANDLE FileOpen(const char * fileName, const char *mode) {
    FileCheckMedia();
    if (FileSysInitLock) {
#if defined(FILESYSTEM_USE_FATFS)
        FRESULT result;
        FILE_HANDLE filePtr;
        BYTE fIndex, NewMode = 0;

        filePtr = NULL;
        //Pick available file structure
        for (fIndex = 0; fIndex < FS_MAX_FILES_OPEN; fIndex++) {
            if (aFileSlot[fIndex] == FALSE) //this slot is available
            {
                aFileSlot[fIndex] = TRUE;
                filePtr = &aFil[fIndex];
                break;
            }
        }
        if (filePtr == NULL) {
            // TODO:FSerrno = CE_TOO_MANY_FILES_OPEN;
            return NULL; //no file structure slot available
        }
        switch (mode[0]) {
            case 'w': // FS_WRITE - Create a new file or replace an existing file
                NewMode = FA_WRITE | FA_CREATE_ALWAYS;
                break;
            case 'r': // FS_READ - Read data from an existing file
                NewMode = FA_READ | FA_OPEN_EXISTING;
                break;
            case 'a': // FS_APPEND - Append data to an existing file
                //        case 'a+': // FS_APPENDPLUS - Append data to an existing file (reads also enabled)
                NewMode = FA_WRITE | FA_OPEN_EXISTING;
                break;
                //        case 'w+': // FS_WRITEPLUS - Create a new file or replace an existing file (reads also enabled)
                //            NewMode = FA_READ | FA_WRITE | FA_CREATE_NEW | FA_CREATE_ALWAYS;
                //            break;
                //        case 'r+': // FS_READPLUS - Read data from an existing file (writes also enabled)
                //            NewMode = FA_READ | FA_WRITE | FA_OPEN_EXISTING;
                //            break;
        }
        if (NewMode == 0) {
            while (1); // TODO: Error handling
        }
        result = f_stat(fileName, &aFilInfo[fIndex]); // Try to get FILINFO for file
        result = f_open(filePtr, fileName, NewMode);
        if (result == FR_OK) {
            switch (mode[0]) {
                case 'a': // FS_APPEND - Append data to an existing file
                    //            case 'a+': // FS_APPENDPLUS - Append data to an existing file (reads also enabled)
                    f_lseek(filePtr, f_size(filePtr));
            }
            return filePtr;
        } else {
            aFileSlot[fIndex] = FALSE; // File not opened: free its slot
            return NULL; // TODO: Error handling
        }
#elif defined(FILESYSTEM_USE_MPFS2)
        return MPFSOpen((BYTE*) fileName);
#elif defined(FILESYSTEM_USE_MDD) 
        return FSfopen(fileName, mode);
#endif
    }
return NULL;
}

#if defined(FILESYSTEM_USE_MDD) || defined(FILESYSTEM_USE_FATFS)

DWORD FileGetFileSize(FILE_HANDLE fh) {
#if defined(FILESYSTEM_USE_FATFS)
    return fh->fsize;
#elif defined(FILESYSTEM_USE_MDD)
    return fh->size;
#endif
}

#if defined(FILESYSTEM_USE_FATFS)

char * FileGetNameFromHandle(FILE_HANDLE fh) {
    BYTE fIndex;
    for (fIndex = 0; fIndex < FS_MAX_FILES_OPEN; fIndex++) {
        if (aFileSlot[fIndex] == TRUE) { //this slot is used
            if (&aFil[fIndex] == fh) {
                return aFilInfo[fIndex].fname;
                break;
            }
        }
    }
    return NULL;
}
#endif

BYTE FileMediaDetect(void) {
#if defined(FILESYSTEM_USE_MDD) 
    MDD_InitIO();
    return MDD_MediaDetect();
#elif defined(FILESYSTEM_USE_FATFS)
    #if defined(USE_USB_INTERFACE)
        BYTE USBHostMSDSCSIMediaDetect(void);
        return(USBHostMSDSCSIMediaDetect());
    #else
    return (!SD_CD);
    #endif
#else
    return (1);
#endif
}

FILE_HANDLE FileOpenROM(const char * fileName, const char *mode) {
#if defined(FILESYSTEM_USE_FATFS)
    return FileOpen(fileName, mode);
#elif defined(FILESYSTEM_USE_MPFS2)
    return MPFSOpenROM((BYTE*) fileName);
#elif defined(FILESYSTEM_USE_MDD)
    return FSfopen(fileName, mode);
#endif
}

int FileClose(FILE_HANDLE fh) {
#if defined(FILESYSTEM_USE_FATFS)
    FRESULT result;
    BYTE fIndex;
    if (fh != NULL) {
        result = f_close(fh);
        if (result == FR_OK) {
            for (fIndex = 0; fIndex < FS_MAX_FILES_OPEN; fIndex++) {
                if (aFileSlot[fIndex] == TRUE && (fh == &aFil[fIndex])) {
                    aFileSlot[fIndex] = FALSE;
                    return (1);
                    break;
                }
            }
        }
    }
#elif defined(FILESYSTEM_USE_MPFS2)
    MPFSClose(fh);
#elif defined(FILESYSTEM_USE_MDD) 
    return FSfclose(fh);
#endif
    return 0;
}

size_t FileRead(void *ptr, size_t size, size_t n, FILE_HANDLE stream) {
#if defined(FILESYSTEM_USE_FATFS)
    size_t BytesRead;
    FRESULT result;
    if (stream == NULL) {
        return 0;
    } else {
        result = f_read(stream, ptr, (UINT) (n * size), (UINT *) & BytesRead);
        if (result == FR_OK) {
            return BytesRead;
        } else {
            return 0;
        }
    }
#elif defined(FILESYSTEM_USE_MPFS2)
    WORD length;
    length = size * n;
    return MPFSGetArray(stream, (BYTE*) ptr, length);
#elif defined(FILESYSTEM_USE_MDD) 
    if (ptr == NULL) {
        return 0;
    } else {
        return FSfread(ptr, size, n, stream);
    }
#endif
}

int FileSeek(FILE_HANDLE stream, long offset, int whence) {
#if defined(FILESYSTEM_USE_FATFS)
    FRESULT result;
    //    FIL *pFil;
    //    pFil = &((*stream).FatFsFIL);
    switch (whence) {
        case SEEK_CUR:
            result = f_lseek(stream, offset + f_tell(stream));
            break;
        case SEEK_END:
            result = f_lseek(stream, f_size(stream) - offset);
            break;
        case SEEK_SET:
            result = f_lseek(stream, offset);
        default:
            break;
    }
    if (result == FR_OK) {
        return (0);
    }
    return EOF;
#elif defined(FILESYSTEM_USE_MPFS2)
    BOOL status;
    status = MPFSSeek(stream, offset, whence);
    if (status == TRUE)
        return 0;
    else
        return -1;

#elif defined(FILESYSTEM_USE_MDD) 
    return FSfseek(stream, offset, whence);
#endif
}

long FileTell(FILE_HANDLE fh) {
#if defined(FILESYSTEM_USE_FATFS)
    //    FIL *pFil;
    //    pFil = &((*fh).FatFsFIL);
    return f_tell(fh);
#elif defined(FILESYSTEM_USE_MPFS2)
    return MPFSGetPosition(fh);
#elif defined(FILESYSTEM_USE_MDD) 
    return FSftell(fh);
#endif
}

int FileEOF(FILE_HANDLE stream) {
#if defined(FILESYSTEM_USE_FATFS)
    //    FIL *pFil;
    //    pFil = &((*stream).FatFsFIL);
    return (f_tell(stream) == f_size(stream));
#elif defined(FILESYSTEM_USE_MPFS2)
    return MPFSGetBytesRem(stream);
#elif defined(FILESYSTEM_USE_MDD) 
    return FSfeof(stream);
#endif
}

int FileFormat(char mode, long int serialNumber, char *volumeID) {
#if defined(FILESYSTEM_USE_FATFS)
    FRESULT result;
    result = f_mkfs(0, 1, 0); // First logical drive, SFD (no partition),Auto cluster size
    if (result == FR_OK) {
        //f_setlabel(volumeID); Only in newer FsFat versions
        return (0);
    }
    return EOF;
    // TODO: SerialNumber
#elif defined(FILESYSTEM_USE_MPFS2)
#if defined(MPFS_USE_EEPROM) || defined(MPFS_USE_SPI_FLASH)
    int status;
    status = MPFSFormat();
    if (status == MPFS_INVALID_HANDLE)
        return -1;
    else
#endif
        return 0;
#elif defined(FILESYSTEM_USE_MDD) && defined(ALLOW_FORMATS) && defined(ALLOW_WRITES)
    return FSformat(mode, serialNumber, volumeID);
#endif
}

size_t FileWrite(const void *ptr, size_t size, size_t n, FILE_HANDLE stream) {
#if defined(FILESYSTEM_USE_FATFS)
    FRESULT result;
    UINT bw;
    //    FIL *pFil;
    //    pFil = &((*stream).FatFsFIL);

    result = f_write(stream, ptr, (UINT) n*size, &bw);
    //    if (result == FR_OK) {
    return (bw);
    //    }
    //    while (1); // TODO: Error handling
#elif defined(FILESYSTEM_USE_MPFS2)
    WORD length;
    length = size * n;
    return MPFSPutArray(stream, (BYTE*) ptr, length);
#elif defined(FILESYSTEM_USE_MDD) 
    return FSfwrite(ptr, size, n, stream);
#endif
}

size_t FileReadUInt32(DWORD *ptr, FILE_HANDLE stream) {
    BYTE databuff[4];
    *ptr = 0x00000000;

#if defined(FILESYSTEM_USE_FATFS)
    size_t BytesRead;
    FRESULT result;
    if (stream == NULL) {
        return 0;
    } else {
        result = f_read(stream, databuff, (UINT) 4, (UINT *) & BytesRead);
        if (result == FR_OK) {
            ((BYTE*) ptr)[3] = databuff[3];
            ((BYTE*) ptr)[2] = databuff[2];
            ((BYTE*) ptr)[1] = databuff[1];
            ((BYTE*) ptr)[0] = databuff[0];
            return 4; //Number of bytes read
        } else {
            return 0;
        }
    }
#elif defined(FILESYSTEM_USE_MPFS2)
    WORD retVal;

    retVal = MPFSGetArray(stream, (BYTE*) ptr, 4);

    if (retVal == 4)//Number of Uints of 4 bytes each Read
    {

        ((BYTE*) ptr)[3] = databuff[3];
        ((BYTE*) ptr)[2] = databuff[2];
        ((BYTE*) ptr)[1] = databuff[1];
        ((BYTE*) ptr)[0] = databuff[0];

        return 4; //Number of bytes read
    } else
        return 0;

#elif defined(FILESYSTEM_USE_MDD) 
    size_t retVal;

    retVal = FSfread(databuff, 4, 1, stream);

    if (retVal == 1)//Number of Uints of 4 bytes each Read
    {

        ((BYTE*) ptr)[3] = databuff[3];
        ((BYTE*) ptr)[2] = databuff[2];
        ((BYTE*) ptr)[1] = databuff[1];
        ((BYTE*) ptr)[0] = databuff[0];

        return 4; //Number of bytes read
    } else
        return 0;

#endif
}

size_t FileReadUInt16(WORD *ptr, FILE_HANDLE stream) {
    BYTE databuff[2];
    *ptr = 0x0000;

#if defined(FILESYSTEM_USE_FATFS)
    size_t BytesRead;
    FRESULT result;
    if (stream == NULL) {
        return 0;
    } else {
        result = f_read(stream, databuff, (UINT) 2, (UINT *) & BytesRead);
        if (result == FR_OK) {
            ((BYTE*) ptr)[1] = databuff[1];
            ((BYTE*) ptr)[0] = databuff[0];
            return 2; //Number of bytes read
        } else {
            return 0;
        }
    }
#elif defined(FILESYSTEM_USE_MPFS2)
    WORD retVal;

    retVal = MPFSGetArray(stream, (BYTE*) ptr, 2);

    if (retVal == 2)//Number of bytes read
    {
        ((BYTE*) ptr)[1] = databuff[1];
        ((BYTE*) ptr)[0] = databuff[0];
        return 2; //Number of bytes read
    } else
        return 0;

#elif defined(FILESYSTEM_USE_MDD) 
    size_t retVal;

    retVal = FSfread(databuff, 2, 1, stream);

    if (retVal == 1)//Number of Uints of 4 bytes each Read
    {
        ((BYTE*) ptr)[1] = databuff[1];
        ((BYTE*) ptr)[0] = databuff[0];
        return 2; //Number of bytes read
    } else
        return 0;

#endif
}

int FileMkDir(const char * path) {
    FileCheckMedia();
    if (FileSysInitLock) {
#if defined(FILESYSTEM_USE_FATFS)
        FRESULT result;
        if (path == NULL) {
            return 1;
        } else {
            result = f_mkdir(path);
            if (result == FR_OK) {
                return 0;
            } else {
                return 1;
            }
        }
#elif defined(FILESYSTEM_USE_MPFS2)
#elif defined(FILESYSTEM_USE_MDD)
        return FSchdir((char *) path);
#endif
    } else
        return 1;
}

int FileChDir(const char * path) {
    FileCheckMedia();
    if (FileSysInitLock) {
#if defined(FILESYSTEM_USE_FATFS)
        FRESULT result;
        if (path == NULL) {
            return 1;
        } else {
            result = f_chdir(path);
            if (result == FR_OK) {
                return 0;
            } else {
                return 1;
            }
        }
#elif defined(FILESYSTEM_USE_MPFS2)
#elif defined(FILESYSTEM_USE_MDD)
        return FSchdir((char *) path);
#endif
    } else
        return 1;
}

int FileDirExists(char *path) {
#if defined(FILESYSTEM_USE_FATFS)
    FRESULT result;
    FILINFO finfo;
    if (path == NULL) {
        return 1;
    } else {
        result = f_stat(path, &finfo);
        if (result == FR_OK) {
            if (finfo.fattrib && AM_DIR) {
                return 0;
            } else {
                return 1;
            }
        } else {
            return 1;
        }
    }
#elif defined(FILESYSTEM_USE_MPFS2)
#elif defined(FILESYSTEM_USE_MDD)
    SearchRec rec;
    unsigned char attributes;
    attributes = ATTR_DIRECTORY;
    return (!FindFirst((const char*) path, (unsigned int) attributes, &rec));
#endif
}

#endif //#if defined(FILESYSTEM_USE_MDD) || defined(FILESYSTEM_USE_FATFS)
