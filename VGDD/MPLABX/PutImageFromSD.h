#ifndef _PUTIMAGEFROMSD_H
#define _PUTIMAGEFROMSD_H

#include "Graphics/Primitive.h"

#ifndef USE_BITMAP_EXTERNAL
#define USE_BITMAP_EXTERNAL	
#endif

#define BINBMP_ON_SDFAT 0x0010

typedef struct {
    GFX_RESOURCE type; // Graphics resource type, determines the type and location of data
    WORD ID;           // memory ID, user defined value to differentiate
                       // between graphics resources of the same type	
    union {
        DWORD extAddress;            // generic address	
        FLASH_BYTE *progByteAddress; // for addresses in program section
        FLASH_WORD *progWordAddress; // for addresses in program section
        const char *constAddress;    // for addresses in FLASH
        char *ramAddress;            // for addresses in RAM
#if defined(__PIC24F__) 	    
        __eds__ char *edsAddress;    // for addresses in EDS
#endif	    
    } LOCATION;

    WORD width;   // width of the image 
    WORD height;  // height of the image
    DWORD param1; // Parameters used for the GFX_RESOURCE. Depending on the GFX_RESOURCE type 
                  // definition of param1 can change. For IPU and RLE compressed images, param1 
                  // indicates the compressed size of the image.
    DWORD param2; // Parameters used for the GFX_RESOURCE. Depending on the GFX_RESOURCE type 
                  // definition of param2 can change. For IPU and RLE compressed images, param2 
                  // indicates the uncompressed size of the image.
    WORD colorDepth; // color depth of the image
    char *filename; // pointer to filename
} IMAGE_ON_SD;

WORD ExternalMemoryCallback(IMAGE_EXTERNAL *memory, LONG offset, WORD nCount, void *buffer);

#endif // ifndef _PUTIMAGEFROMSD_H
