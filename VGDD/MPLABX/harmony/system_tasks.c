/*******************************************************************************
 System Tasks File

  File Name:
    sys_tasks.c

  Summary:
    System tasks File.

  Description:
    This file will contain any source code necessary to maintain various tasks
    in the system.
 *******************************************************************************/

// DOM-IGNORE-BEGIN
/*******************************************************************************
Copyright (c) 2011-2012 released Microchip Technology Inc.  All rights reserved.

Microchip licenses to you the right to use, modify, copy and distribute
Software only when embedded on a Microchip microcontroller or digital signal
controller that is integrated into your product or third party product
(pursuant to the sublicense terms in the accompanying license agreement).

You should refer to the license agreement accompanying this Software for
additional information regarding your rights and obligations.

SOFTWARE AND DOCUMENTATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
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


// *****************************************************************************
// *****************************************************************************
// Section: Included Files
// *****************************************************************************
// *****************************************************************************
#include "app.h"

// *****************************************************************************
// *****************************************************************************
// Section: System "Tasks" Routine
// *****************************************************************************
// *****************************************************************************
/*******************************************************************************
  Function:
    void SYS_Tasks ( void )

  Summary:
    Calls all module-specific "tasks" routines to maintain module state

  Description:
    This routine calls all module-specific "tasks" routines to maintain module
    state.

  Precondition:
    SYS_Initialize has been called

  Parameters:
    None.

  Returns:
    None.

  Example:
    <code>
    int main ( void )
    {
        SYS_Initialize(&initData);

        while (true)
        {
            SYS_Tasks();
        }
    }
    </code>

  Remarks:
    When not using the dynamic system "Tasks" service, this routine must be
    implemented by the application's system configuration (in the application's
    configuration-specific "sys_tasks.c" file).
 */

void SYS_Tasks ( void )
{
    /* Maintain the state machines of all library modules executing polled in
    the system. */

    /* Maintain system services */
    SYS_DEVCON_Tasks(sysObj.sysDevcon);

// <editor-fold defaultstate="collapsed" desc="Generated Code">
// VGDD_MPLABX_WIZARD_START_SECTION: Tasks *** DO NOT DELETE THIS LINE! ***
// These lines will be replaced by VGDD MPLAB X Wizard with lines for the Tasks Section
// Don't delete the starting and ending markers!
// VGDD_MPLABX_WIZARD_END_SECTION *** DO NOT DELETE THIS LINE! ***
// </editor-fold>

    /* Maintain Graphics Library
    /* Maintain the gfx state machine 0. */
    GFX_Tasks(sysObj.gfxObject0);

    // <editor-fold defaultstate="collapsed" desc="Generated Code">
// VGDD_MPLABX_WIZARD_START_SECTION: MainFinishedDraw *** DO NOT DELETE THIS LINE! ***
    // VGDD_MPLABX_WIZARD_END_SECTION *** DO NOT DELETE THIS LINE! ***
// </editor-fold>

    /* Maintain the application's state machine. */
    APP_Tasks();
}


/*******************************************************************************
 End of File
*/
