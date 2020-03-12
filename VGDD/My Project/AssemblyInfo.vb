Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' General Information about an assembly is controlled through the following 
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

<Assembly: Obfuscation(ApplyToMembers:=True, Feature:="encrypt symbol names with password pa$$w0rdcivicivi23*", Exclude:=False)> 
<Assembly: Obfuscation(Feature:="Apply to type VGDDMicrochip.*: all", Exclude:=True, ApplyToMembers:=True)> 
<Assembly: Obfuscation(Feature:="encrypt resources [exclude] *.dll", Exclude:=False)> 
<Assembly: Obfuscation(Feature:="string encryption", Exclude:=True)> 
<Assembly: ObfuscateAssembly(True)> 
'<Assembly: Obfuscation(ApplyToMembers:=True, Feature:="Apply to type VFab.*: all", Exclude:=True)> 
'<Assembly: Obfuscation(Feature:="embed VFabLM.dll", Exclude:=False)> 
'<Assembly: Obfuscation(Feature:="merge with VFabLM.dll", Exclude:=False)> 
'<Assembly: Obfuscation(Feature:="merge with VGDDCommonEmbedded.dll", Exclude:=False)> 
'<Assembly: Obfuscation(Feature:="assembly probing path C:\!DotNet\VGDD3.0\VGDDCommon\bin")> 

<Assembly: AssemblyTitle("VGDD")> 
<Assembly: AssemblyProduct("VGDD")> 
<Assembly: AssemblyDescription("A Visual Designer and Code Generator for Microchip Graphics Library.")> 
<Assembly: AssemblyCompany("VirtualFab")> 
<Assembly: AssemblyCopyright("Copyright © VirtualFab 2011-2016")> 
<Assembly: AssemblyTrademark("Visual Graphics Display Designer")> 

<Assembly: ComVisible(False)> 

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("369e97c6-6112-457c-bd61-1448b4b060ac")> 

' Version information for an assembly consists of the following four values:
'
'      Major Version
'      Minor Version 
'      Build Number
'      Revision
'
' You can specify all the values or you can default the Build and Revision Numbers 
' by using the '*' as shown below:
' <Assembly: AssemblyVersion("1.0.*")> 

<Assembly: AssemblyVersion(VGDDCommon.Common.ASSEMBLIES_VERSION)> 
'<Assembly: AssemblyVersion("3.1.*")> 
'<Assembly: AssemblyFileVersion("3.0.0.3")> 
'<Assembly: AssemblyInformationalVersion("3.0.0")> 
<Assembly: AssemblyExceptionLoggingURLAttribute(VIRTUALFABSITE & "/lm/exception.php")>

