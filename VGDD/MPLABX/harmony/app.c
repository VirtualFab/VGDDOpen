/*******************************************************************************
  MPLAB Harmony Application Source File
  
  Company:
    Microchip Technology Inc.
  
  File Name:
    app.c

  Summary:
    This file contains the source code for the MPLAB Harmony application.

  Description:
    This file contains the source code for the MPLAB Harmony application.  It 
    implements the logic of the application's state machine and it may call 
    API routines of other MPLAB Harmony modules in the system, such as drivers,
    system services, and middleware.  However, it does not call any of the
    system interfaces (such as the "Initialize" and "Tasks" functions) of any of
    the modules in the system or make any assumptions about when those functions
    are called.  That is the responsibility of the configuration-specific system
    files.
 *******************************************************************************/

// DOM-IGNORE-BEGIN
/*******************************************************************************
Copyright (c) 2013-2014 released Microchip Technology Inc.  All rights reserved.

Microchip licenses to you the right to use, modify, copy and distribute
Software only when embedded on a Microchip microcontroller or digital signal
controller that is integrated into your product or third party product
(pursuant to the sublicense terms in the accompanying license agreement).

You should refer to the license agreement accompanying this Software for
additional information regarding your rights and obligations.

SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY WARRANTY OF
MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
IN NO EVENT SHALL MICROCHIP OR ITS LICENSORS BE LIABLE OR OBLIGATED UNDER
CONTRACT, NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION, BREACH OF WARRANTY, OR
OTHER LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT DAMAGES OR EXPENSES
INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL, INDIRECT, PUNITIVE OR
CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, COST OF PROCUREMENT OF
SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY CLAIMS BY THIRD PARTIES
(INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF), OR OTHER SIMILAR COSTS.
 *******************************************************************************/
// DOM-IGNORE-END


/************************************************************
 * Includes
 ************************************************************/
#include "app.h"
APP_DATA appData;
SYS_MODULE_OBJ   oSpiObject;
//SYS_MSG_INSTANCE iSysMsg = SYS_MSG_0;
SYS_OBJ_HANDLE hSysMsg, hMsgType, hMailbox;

// <editor-fold defaultstate="collapsed" desc="Generated Code">
// VGDD_MPLABX_WIZARD_START_SECTION: AppHeader *** DO NOT DELETE THIS LINE! ***
// These lines will be replaced by VGDD MPLAB X Wizard with lines for the AppHeader Section
// Don't delete the starting and ending markers!
// VGDD_MPLABX_WIZARD_END_SECTION *** DO NOT DELETE THIS LINE! ***
// </editor-fold>

// *****************************************************************************
// *****************************************************************************
// Section: Application Initialization and State Machine
// *****************************************************************************
// *****************************************************************************

SYS_MSG_INIT sSysMsgInit =
{
    .nMaxMsgsDelivered  = SYS_MSG_MAX_MSGS_DELIVERED,
    .nMessagePriorities = SYS_MSG_MAX_PRIORITY + 1,
//    .nQSizes = SYS_MSG_BUFFER_SIZES
};


void TouchMessageCallback(SYS_MSG_OBJECT *pMsg)
{
    static GFX_GOL_MESSAGE gMsg;

    gMsg.type = pMsg->nMessageTypeID;
    gMsg.uiEvent = pMsg->param0;
    gMsg.param1 = pMsg->param1;
    gMsg.param2 = pMsg->param2;

    GFX_GOL_ObjectMessage(GFX_INDEX_0, &gMsg);
}
/******************************************************************************
  Function:
    void APP_Initialize ( void )

  Remarks:
    See prototype in app.h.
*/

void APP_Initialize ( void )
{
    /* Place the App state machine in its initial state. */
    appData.state = APP_STATE_INIT;

    /* TODO: Initialize your application's state machine and other
     * parameters.
     */
    hMsgType = SYS_MSG_TypeCreate(SYS_MSG_0, TYPE_TOUCHSCREEN , 0);
    hMailbox = SYS_MSG_MailboxOpen(SYS_MSG_0, (SYS_MSG_RECEIVE_CALLBACK)&TouchMessageCallback );
    SYS_MSG_MailboxMsgAdd(hMailbox,hMsgType);

    appData.samplePoints.touchCalUlx = 0x00C0;
    appData.samplePoints.touchCalUly = 0x00D4;

    appData.samplePoints.touchCalUrx = 0x037C;
    appData.samplePoints.touchCalUry = 0x00D7;

    appData.samplePoints.touchCalLrx = 0x035A;
    appData.samplePoints.touchCalLry = 0x0333;

    appData.samplePoints.touchCalLlx = 0x00B9;
    appData.samplePoints.touchCalLly = 0x0350;
}

/******************************************************************************
  Function:
    void APP_Tasks ( void )

  Remarks:
    See prototype in app.h.
*/

void APP_Tasks(void) {

    /* Check the application's current state. */
    switch (appData.state) {

        /* Application's initial state. */
        case APP_STATE_INIT:
            if (GFX_Status(sysObj.gfxObject0) == SYS_STATUS_READY)
                appData.state = APP_STATE_GFX_INIT;
            break;

        case APP_STATE_GFX_INIT:
            GFX_GOL_MessageCallbackSet(GFX_INDEX_0, &APP_ObjectMessageCallback);
            GFX_GOL_DrawCallbackSet(GFX_INDEX_0, &APP_ObjectDrawCallback);

            /* set drawing state */
            appData.state = APP_STATE_DONE;

            break;

    }

}
