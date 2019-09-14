<?php
openlog("LicenseManager", LOG_NDELAY, LOG_LOCAL0);
syslog(LOG_INFO, "Eula: {$_SERVER['REMOTE_ADDR']}");
//echo "P=".$_GET['P'];
$ProductName=$_GET['P'];
$CompanyName="VirtualFab";
?>
<div style="font-family: verdana">
<div align="center">
END-USER LICENSE AGREEMENT FOR <b><?=$ProductName?></b><br/> 
<br/>
IMPORTANT PLEASE READ THE TERMS AND CONDITIONS OF THIS LICENSE AGREEMENT CAREFULLY BEFORE CONTINUING WITH THIS PROGRAM PURCHASE AND/OR INSTALL:<br/>
</div>
<br/>
<?=$CompanyName?>'s End-User License Agreement ("EULA") is a legal agreement between you (either an individual or a single entity)
and <?=$CompanyName?> for the <?=$CompanyName?>'s software product(s) identified above, which may include associated software components, 
media, printed materials, and "online" or electronic documentation ("SOFTWARE PRODUCT"). <br/>
<br/>
By installing, copying, or otherwise using the SOFTWARE PRODUCT, you agree to be bound by the terms of this EULA. <br/>
<br/>
This license agreement represents the entire agreement concerning the program between you and <?=$CompanyName?>, (referred to as "licenser"), 
and it supersedes any prior proposal, representation, or understanding between the parties. <br/>
<br/>
<b>If you do not agree to the terms of this EULA, do not install or use the SOFTWARE PRODUCT</b>.<br/>
<br/>
The SOFTWARE PRODUCT is protected by copyright laws and international copyright treaties, as well as other intellectual property laws and treaties. <br/>
The SOFTWARE PRODUCT is licensed, not sold.<br/>
<br/>
<b>1. GRANT OF LICENSE.</b><br/>
The SOFTWARE PRODUCT is licensed as follows:<br/>
(a) Installation and Use.<br/>
<?=$CompanyName?> grants you the right to install and use copies of the SOFTWARE PRODUCT on your computer running a validly licensed copy of the operating system 
for which the SOFTWARE PRODUCT was designed.<br/>
<br/>
(b) Backup Copies.<br/>
you may also make copies of the SOFTWARE PRODUCT as may be necessary for backup and archival purposes.<br/>
<br/>
<br/>
<b>2. DESCRIPTION OF OTHER RIGHTS AND LIMITATIONS.</b><br/>
(a) Maintenance of Copyright Notices.<br/>
you must not remove or alter any copyright notices on any and all copies of the SOFTWARE PRODUCT.<br/>
<br/>
(b) Distribution.<br/>
you may not distribute registered copies of the SOFTWARE PRODUCT to third parties. Evaluation versions available for download from <?=$CompanyName?>'s websites
may be freely distributed.<br/>
<br/>
(c) Prohibition on Reverse Engineering, Decompilation, and Disassembly.<br/>
you may not reverse engineer, decompile, or disassemble the SOFTWARE PRODUCT, except and only to the extent that such activity is expressly permitted
by applicable law notwithstanding this limitation.<br/>
<br/>
(d) Rental.<br/>
you may not rent, lease, or lend the SOFTWARE PRODUCT.<br/>
<br/>
(e) Support Services.<br/>
<?=$CompanyName?> may provide you with support services related to the SOFTWARE PRODUCT ("Support Services"). <br/>
Any supplemental software code provided to you as part of the Support Services shall be considered part of the SOFTWARE PRODUCT and subject
to the terms and conditions of this EULA.<br/>
<br/>
(f) Compliance with Applicable Laws.<br/>
you must comply with all applicable laws regarding use of the SOFTWARE PRODUCT.<br/>
<br/>
<br/>
<b>3. TERMINATION</b><br/>
Without prejudice to any other rights, <?=$CompanyName?> may terminate this EULA if you fail to comply with the terms and conditions of this EULA. <br/>
In such event, you must destroy all copies of the SOFTWARE PRODUCT in your possession.<br/>
<br/>
<br/>
<b>4. COPYRIGHT</b><br/>
All title, including but not limited to copyrights, in and to the SOFTWARE PRODUCT and any copies thereof are owned by <?=$CompanyName?> or its suppliers. <br/>
All title and intellectual property rights in and to the content which may be accessed through use of the SOFTWARE PRODUCT 
is the property of the respective content owner 
and may be protected by applicable copyright or other intellectual property laws and treaties. <br/>
This EULA grants you no rights to use such content. All rights not expressly granted are reserved by <?=$CompanyName?>.<br/>
<br/>
<br/>
<b>5.LIMITED WARRANTIES</b><br/>
(a) <?=$CompanyName?> warrants that, for ninety (90) days from the date of shipment of the SOFTWARE PRODUCT to you (the "WARRANTY PERIOD"), the SOFTWARE PRODUCT shall operate 
in accordance with the published functional specifications in effect at the time of shipment. If, during the WARRANTY PERIOD the SOFTWARE PRODUCT is found to be defective, 
<?=$CompanyName?> will use its reasonable efforts to correct the deviation within a reasonable time after notification from you. The entire liability of <?=$CompanyName?> 
and your exclusive remedy shall be, at the option of <?=$CompanyName?>, either to return the price paid to <?=$CompanyName?> or to replace the SOFTWARE PRODUCT. <br/>
<br/>
THE WARRANTY SET FORTH IN THIS SECTION 5.a IS THE ONLY WARRANTY MADE BY <?=strtoupper($CompanyName)?> AND <?=strtoupper($CompanyName)?> EXPRESSLY DISCLAIMS ALL OTHER WARRANTIES, 
WHETHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO ANY WARRANTIES OF MERCHANTABILITY, NONINFRINGEMENT, OR FITNESS OF A PARTICULAR PURPOSE.<br/>
<br/>
(b) In developing the Software <?=$CompanyName?> attempted to offer the most current, correct and clearly expressed information possible. Nonetheless, errors may occur 
and <?=$CompanyName?> does not warrant that the Software is free from bugs, errors, or other program limitations.<br/>
(c) If, during the Warranty Period, a defect appears in the Software, you shall notify <?=$CompanyName?>. To correct the deviation you must give detailed information about the error and the target
application.<br/>
(d) <?=$CompanyName?> makes no warranties respecting any harm that may be caused by the transmission of a computer virus, worm, time bomb, logic bomb, or other such computer program. <br/>
(e) <?=$CompanyName?> further expressly disclaims any warranty or representation to Authorized Users or to any third party.<br/>
<br/>
<b>6. LIMITATION OF LIABILITY</b><br/>
In no event shall <?=$CompanyName?> be liable for any damages (including, without limitation, lost profits, business interruption, or lost information) 
rising out of 'Authorized Users' use of or inability to use the SOFTWARE PRODUCT, even if <?=$CompanyName?> has been advised of the possibility of such damages. <br/>
In no event will <?=$CompanyName?> be liable for loss of data or for indirect, special, incidental, consequential (including lost profit), or other damages 
based in contract, tort or otherwise. <br/>
<?=$CompanyName?> shall have no liability with respect to the content of the SOFTWARE PRODUCT or any part thereof, including but not limited to errors 
or omissions contained therein, libel, infringements of rights of publicity, privacy, trademark rights, business interruption, personal injury, loss of privacy, 
moral rights or the disclosure of confidential information.<br/>
The total monetary liability of <?=$CompanyName?> shall be limited to the amount actually paid by you for the SOFTWARE PRODUCT License.
<br/>
<b>7. THIRD PARTY SOFTWARE LIBRARIES USAGE</b><br/>
<?=$ProductName?> uses a modified WeifenLuo's DockPanelSuite library whose code is licensed under MIT License: http://opensource.org/licenses/MIT
<br/>
<br/>
</div>



