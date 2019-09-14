' FolderBrowserDialogEx.cs
'
' A replacement for the builtin System.Windows.Forms.FolderBrowserDialog class.
' This one includes an edit box, and also displays the full path in the edit box. 
'
' based on code from http://support.microsoft.com/default.aspx?scid=kb;[LN];306285 
' 
' 20 Feb 2009
'
' ========================================================================================
' Example usage:
' 
' string _folderName = "c:\\dinoch";
' private void button1_Click(object sender, EventArgs e)
' {
'     _folderName = (System.IO.Directory.Exists(_folderName)) ? _folderName : "";
'     var dlg1 = new Ionic.Utils.FolderBrowserDialogEx
'     {
'         Description = "Select a folder for the extracted files:",
'         ShowNewFolderButton = true,
'         ShowEditBox = true,
'         //NewStyle = false,
'         SelectedPath = _folderName,
'         ShowFullPathInEditBox= false,
'     };
'     dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;
' 
'     var result = dlg1.ShowDialog();
' 
'     if (result == DialogResult.OK)
'     {
'         _folderName = dlg1.SelectedPath;
'         this.label1.Text = "The folder selected was: ";
'         this.label2.Text = _folderName;
'     }
' }
'


Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Security.Permissions
Imports System.Security
Imports System.Threading
Namespace Ionic.Utils

    '[Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultEvent("HelpRequest"), SRDescription("DescriptionFolderBrowserDialog"), DefaultProperty("SelectedPath")]
    Public Class FolderBrowserDialogEx
        Inherits System.Windows.Forms.CommonDialog
        Private Shared ReadOnly MAX_PATH As Integer = 260

        ' Fields
        Private _callback As PInvoke.BrowseFolderCallbackProc
        Private _descriptionText As String
        Private _rootFolder As Environment.SpecialFolder
        Private _selectedPath As String
        Private _selectedPathNeedsCheck As Boolean
        Private _showNewFolderButton As Boolean
        Private _showEditBox As Boolean
        Private _showBothFilesAndFolders As Boolean
        Private _newStyle As Boolean = True
        Private _showFullPathInEditBox As Boolean = True
        Private _dontIncludeNetworkFoldersBelowDomainLevel As Boolean
        Private _uiFlags As Integer
        Private _hwndEdit As IntPtr
        Private _rootFolderLocation As IntPtr

        ' Events
        '[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        Public Shadows Custom Event HelpRequest As EventHandler
            AddHandler(ByVal value As EventHandler)
                MyBase.HelpRequest += value
            End AddHandler
            RemoveHandler(ByVal value As EventHandler)
                MyBase.HelpRequest -= value
            End RemoveHandler
        End Event

        ' ctor
        Public Sub New()
            Me.Reset()
        End Sub

        ' Factory Methods
        Public Shared Function PrinterBrowser() As FolderBrowserDialogEx
            Dim x As New FolderBrowserDialogEx()
            ' avoid MBRO comppiler warning when passing _rootFolderLocation as a ref:
            x.BecomePrinterBrowser()
            Return x
        End Function

        Public Shared Function ComputerBrowser() As FolderBrowserDialogEx
            Dim x As New FolderBrowserDialogEx()
            ' avoid MBRO comppiler warning when passing _rootFolderLocation as a ref:
            x.BecomeComputerBrowser()
            Return x
        End Function


        ' Helpers
        Private Sub BecomePrinterBrowser()
            _uiFlags += BrowseFlags.BIF_BROWSEFORPRINTER
            Description = "Select a printer:"
            PInvoke.Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.PRINTERS, Me._rootFolderLocation)
            ShowNewFolderButton = False
            ShowEditBox = False
        End Sub

        Private Sub BecomeComputerBrowser()
            _uiFlags += BrowseFlags.BIF_BROWSEFORCOMPUTER
            Description = "Select a computer:"
            PInvoke.Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.NETWORK, Me._rootFolderLocation)
            ShowNewFolderButton = False
            ShowEditBox = False
        End Sub


        Private Class CSIDL
            Public Const PRINTERS As Integer = 4
            Public Const NETWORK As Integer = &H12
        End Class

        Private Class BrowseFlags
            Public Const BIF_DEFAULT As Integer = &H0
            Public Const BIF_BROWSEFORCOMPUTER As Integer = &H1000
            Public Const BIF_BROWSEFORPRINTER As Integer = &H2000
            Public Const BIF_BROWSEINCLUDEFILES As Integer = &H4000
            Public Const BIF_BROWSEINCLUDEURLS As Integer = &H80
            Public Const BIF_DONTGOBELOWDOMAIN As Integer = &H2
            Public Const BIF_EDITBOX As Integer = &H10
            Public Const BIF_NEWDIALOGSTYLE As Integer = &H40
            Public Const BIF_NONEWFOLDERBUTTON As Integer = &H200
            Public Const BIF_RETURNFSANCESTORS As Integer = &H8
            Public Const BIF_RETURNONLYFSDIRS As Integer = &H1
            Public Const BIF_SHAREABLE As Integer = &H8000
            Public Const BIF_STATUSTEXT As Integer = &H4
            Public Const BIF_UAHINT As Integer = &H100
            Public Const BIF_VALIDATE As Integer = &H20
            Public Const BIF_NOTRANSLATETARGETS As Integer = &H400
        End Class

        Private NotInheritable Class BrowseForFolderMessages
            Private Sub New()
            End Sub
            ' messages FROM the folder browser
            Public Const BFFM_INITIALIZED As Integer = 1
            Public Const BFFM_SELCHANGED As Integer = 2
            Public Const BFFM_VALIDATEFAILEDA As Integer = 3
            Public Const BFFM_VALIDATEFAILEDW As Integer = 4
            Public Const BFFM_IUNKNOWN As Integer = 5

            ' messages TO the folder browser
            Public Const BFFM_SETSTATUSTEXT As Integer = &H464
            Public Const BFFM_ENABLEOK As Integer = &H465
            Public Const BFFM_SETSELECTIONA As Integer = &H466
            Public Const BFFM_SETSELECTIONW As Integer = &H467
        End Class

        Private Function FolderBrowserCallback(hwnd As IntPtr, msg As Integer, lParam As IntPtr, lpData As IntPtr) As Integer
            Select Case msg
                Case BrowseForFolderMessages.BFFM_INITIALIZED
                    If Me._selectedPath.Length <> 0 Then
                        PInvoke.User32.SendMessage(New HandleRef(Nothing, hwnd), BrowseForFolderMessages.BFFM_SETSELECTIONW, 1, Me._selectedPath)
                        If Me._showEditBox AndAlso Me._showFullPathInEditBox Then
                            ' get handle to the Edit box inside the Folder Browser Dialog
                            _hwndEdit = PInvoke.User32.FindWindowEx(New HandleRef(Nothing, hwnd), IntPtr.Zero, "Edit", Nothing)
                            PInvoke.User32.SetWindowText(_hwndEdit, Me._selectedPath)
                        End If
                    End If
                    Exit Select

                Case BrowseForFolderMessages.BFFM_SELCHANGED
                    Dim pidl As IntPtr = lParam
                    If pidl <> IntPtr.Zero Then
                        If ((_uiFlags And BrowseFlags.BIF_BROWSEFORPRINTER) = BrowseFlags.BIF_BROWSEFORPRINTER) OrElse ((_uiFlags And BrowseFlags.BIF_BROWSEFORCOMPUTER) = BrowseFlags.BIF_BROWSEFORCOMPUTER) Then
                            ' we're browsing for a printer or computer, enable the OK button unconditionally.
                            PInvoke.User32.SendMessage(New HandleRef(Nothing, hwnd), BrowseForFolderMessages.BFFM_ENABLEOK, 0, 1)
                        Else
                            Dim pszPath As IntPtr = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize)
                            Dim haveValidPath As Boolean = PInvoke.Shell32.SHGetPathFromIDList(pidl, pszPath)
                            Dim displayedPath As [String] = Marshal.PtrToStringAuto(pszPath)
                            Marshal.FreeHGlobal(pszPath)
                            ' whether to enable the OK button or not. (if file is valid)
                            PInvoke.User32.SendMessage(New HandleRef(Nothing, hwnd), BrowseForFolderMessages.BFFM_ENABLEOK, 0, If(haveValidPath, 1, 0))

                            ' Maybe set the Edit Box text to the Full Folder path
                            If haveValidPath AndAlso Not [String].IsNullOrEmpty(displayedPath) Then
                                If _showEditBox AndAlso _showFullPathInEditBox Then
                                    If _hwndEdit <> IntPtr.Zero Then
                                        PInvoke.User32.SetWindowText(_hwndEdit, displayedPath)
                                    End If
                                End If

                                If (_uiFlags And BrowseFlags.BIF_STATUSTEXT) = BrowseFlags.BIF_STATUSTEXT Then
                                    PInvoke.User32.SendMessage(New HandleRef(Nothing, hwnd), BrowseForFolderMessages.BFFM_SETSTATUSTEXT, 0, displayedPath)
                                End If
                            End If
                        End If
                    End If
                    Exit Select
            End Select
            Return 0
        End Function

        Private Shared Function GetSHMalloc() As PInvoke.IMalloc
            Dim ppMalloc As PInvoke.IMalloc() = New PInvoke.IMalloc(0) {}
            PInvoke.Shell32.SHGetMalloc(ppMalloc)
            Return ppMalloc(0)
        End Function

        Public Overrides Sub Reset()
            Me._rootFolder = CType(0, Environment.SpecialFolder)
            Me._descriptionText = String.Empty
            Me._selectedPath = String.Empty
            Me._selectedPathNeedsCheck = False
            Me._showNewFolderButton = True
            Me._showEditBox = True
            Me._newStyle = True
            Me._dontIncludeNetworkFoldersBelowDomainLevel = False
            Me._hwndEdit = IntPtr.Zero
            Me._rootFolderLocation = IntPtr.Zero
        End Sub

        Protected Overrides Function RunDialog(hWndOwner As IntPtr) As Boolean
            Dim result As Boolean = False
            If _rootFolderLocation = IntPtr.Zero Then
                PInvoke.Shell32.SHGetSpecialFolderLocation(hWndOwner, CInt(Me._rootFolder), _rootFolderLocation)
                If _rootFolderLocation = IntPtr.Zero Then
                    PInvoke.Shell32.SHGetSpecialFolderLocation(hWndOwner, 0, _rootFolderLocation)
                    If _rootFolderLocation = IntPtr.Zero Then
                        Throw New InvalidOperationException("FolderBrowserDialogNoRootFolder")
                    End If
                End If
            End If
            _hwndEdit = IntPtr.Zero
            '_uiFlags = 0;
            If _dontIncludeNetworkFoldersBelowDomainLevel Then
                _uiFlags += BrowseFlags.BIF_DONTGOBELOWDOMAIN
            End If
            If Me._newStyle Then
                _uiFlags += BrowseFlags.BIF_NEWDIALOGSTYLE
            End If
            If Not Me._showNewFolderButton Then
                _uiFlags += BrowseFlags.BIF_NONEWFOLDERBUTTON
            End If
            If Me._showEditBox Then
                _uiFlags += BrowseFlags.BIF_EDITBOX
            End If
            If Me._showBothFilesAndFolders Then
                _uiFlags += BrowseFlags.BIF_BROWSEINCLUDEFILES
            End If


            If Control.CheckForIllegalCrossThreadCalls AndAlso (Application.OleRequired() <> ApartmentState.STA) Then
                Throw New ThreadStateException("DebuggingException: ThreadMustBeSTA")
            End If
            Dim pidl As IntPtr = IntPtr.Zero
            Dim hglobal As IntPtr = IntPtr.Zero
            Dim pszPath As IntPtr = IntPtr.Zero
            Try
                Dim browseInfo As New PInvoke.BROWSEINFO()
                hglobal = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize)
                pszPath = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize)
                Me._callback = New PInvoke.BrowseFolderCallbackProc(AddressOf Me.FolderBrowserCallback)
                browseInfo.pidlRoot = _rootFolderLocation
                browseInfo.Owner = hWndOwner
                browseInfo.pszDisplayName = hglobal
                browseInfo.Title = Me._descriptionText
                browseInfo.Flags = _uiFlags
                browseInfo.callback = Me._callback
                browseInfo.lParam = IntPtr.Zero
                browseInfo.iImage = 0
                pidl = PInvoke.Shell32.SHBrowseForFolder(browseInfo)
                If ((_uiFlags And BrowseFlags.BIF_BROWSEFORPRINTER) = BrowseFlags.BIF_BROWSEFORPRINTER) OrElse ((_uiFlags And BrowseFlags.BIF_BROWSEFORCOMPUTER) = BrowseFlags.BIF_BROWSEFORCOMPUTER) Then
                    Me._selectedPath = Marshal.PtrToStringAuto(browseInfo.pszDisplayName)
                    result = True
                Else
                    If pidl <> IntPtr.Zero Then
                        PInvoke.Shell32.SHGetPathFromIDList(pidl, pszPath)
                        Me._selectedPathNeedsCheck = True
                        Me._selectedPath = Marshal.PtrToStringAuto(pszPath)
                        result = True
                    End If
                End If
            Finally
                Dim sHMalloc As PInvoke.IMalloc = GetSHMalloc()
                sHMalloc.Free(_rootFolderLocation)
                _rootFolderLocation = IntPtr.Zero
                If pidl <> IntPtr.Zero Then
                    sHMalloc.Free(pidl)
                End If
                If pszPath <> IntPtr.Zero Then
                    Marshal.FreeHGlobal(pszPath)
                End If
                If hglobal <> IntPtr.Zero Then
                    Marshal.FreeHGlobal(hglobal)
                End If
                Me._callback = Nothing
            End Try
            Return result
        End Function

        ' Properties
        '[SRDescription("FolderBrowserDialogDescription"), SRCategory("CatFolderBrowsing"), Browsable(true), DefaultValue(""), Localizable(true)]

        ''' <summary>
        ''' This description appears near the top of the dialog box, providing direction to the user.
        ''' </summary>
        Public Property Description() As String
            Get
                Return Me._descriptionText
            End Get
            Set(value As String)
                Me._descriptionText = If((value Is Nothing), String.Empty, value)
            End Set
        End Property

        '[Localizable(false), SRCategory("CatFolderBrowsing"), SRDescription("FolderBrowserDialogRootFolder"), TypeConverter(typeof(SpecialFolderEnumConverter)), Browsable(true), DefaultValue(0)]
        Public Property RootFolder() As Environment.SpecialFolder
            Get
                Return Me._rootFolder
            End Get
            Set(value As Environment.SpecialFolder)
                If Not [Enum].IsDefined(GetType(Environment.SpecialFolder), value) Then
                    Throw New InvalidEnumArgumentException("value", CInt(value), GetType(Environment.SpecialFolder))
                End If
                Me._rootFolder = value
            End Set
        End Property

        '[Browsable(true), SRDescription("FolderBrowserDialogSelectedPath"), SRCategory("CatFolderBrowsing"), DefaultValue(""), Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Localizable(true)]

        ''' <summary>
        ''' Set or get the selected path.  
        ''' </summary>
        Public Property SelectedPath() As String
            Get
                If ((Me._selectedPath IsNot Nothing) AndAlso (Me._selectedPath.Length <> 0)) AndAlso Me._selectedPathNeedsCheck Then
                    Dim ofp As New FileIOPermission(FileIOPermissionAccess.PathDiscovery, Me._selectedPath)
                    ofp.Demand()
                    Me._selectedPathNeedsCheck = False
                End If
                Return Me._selectedPath
            End Get
            Set(value As String)
                Me._selectedPath = If((value Is Nothing), String.Empty, value)
                Me._selectedPathNeedsCheck = True
            End Set
        End Property

        '[SRDescription("FolderBrowserDialogShowNewFolderButton"), Localizable(false), Browsable(true), DefaultValue(true), SRCategory("CatFolderBrowsing")]

        ''' <summary>
        ''' Enable or disable the "New Folder" button in the browser dialog.
        ''' </summary>
        Public Property ShowNewFolderButton() As Boolean
            Get
                Return Me._showNewFolderButton
            End Get
            Set(value As Boolean)
                Me._showNewFolderButton = value
            End Set
        End Property

        ''' <summary>
        ''' Show an "edit box" in the folder browser.
        ''' </summary>
        ''' <remarks>
        ''' The "edit box" normally shows the name of the selected folder.  
        ''' The user may also type a pathname directly into the edit box.  
        ''' </remarks>
        ''' <seealso cref="ShowFullPathInEditBox"/>
        Public Property ShowEditBox() As Boolean
            Get
                Return Me._showEditBox
            End Get
            Set(value As Boolean)
                Me._showEditBox = value
            End Set
        End Property

        ''' <summary>
        ''' Set whether to use the New Folder Browser dialog style.
        ''' </summary>
        ''' <remarks>
        ''' The new style is resizable and includes a "New Folder" button.
        ''' </remarks>
        Public Property NewStyle() As Boolean
            Get
                Return Me._newStyle
            End Get
            Set(value As Boolean)
                Me._newStyle = value
            End Set
        End Property


        Public Property DontIncludeNetworkFoldersBelowDomainLevel() As Boolean
            Get
                Return _dontIncludeNetworkFoldersBelowDomainLevel
            End Get
            Set(value As Boolean)
                _dontIncludeNetworkFoldersBelowDomainLevel = value
            End Set
        End Property

        ''' <summary>
        ''' Show the full path in the edit box as the user selects it. 
        ''' </summary>
        ''' <remarks>
        ''' This works only if ShowEditBox is also set to true. 
        ''' </remarks>
        Public Property ShowFullPathInEditBox() As Boolean
            Get
                Return _showFullPathInEditBox
            End Get
            Set(value As Boolean)
                _showFullPathInEditBox = value
            End Set
        End Property

        Public Property ShowBothFilesAndFolders() As Boolean
            Get
                Return _showBothFilesAndFolders
            End Get
            Set(value As Boolean)
                _showBothFilesAndFolders = value
            End Set
        End Property
    End Class



    Friend NotInheritable Class PInvoke
        Private Sub New()
        End Sub
        Shared Sub New()
        End Sub

        Public Delegate Function BrowseFolderCallbackProc(hwnd As IntPtr, msg As Integer, lParam As IntPtr, lpData As IntPtr) As Integer

        Friend NotInheritable Class User32
            Private Sub New()
            End Sub
            <DllImport("user32.dll", CharSet:=CharSet.Auto)> _
            Public Shared Function SendMessage(hWnd As HandleRef, msg As Integer, wParam As Integer, lParam As String) As IntPtr
            End Function

            <DllImport("user32.dll", CharSet:=CharSet.Auto)> _
            Public Shared Function SendMessage(hWnd As HandleRef, msg As Integer, wParam As Integer, lParam As Integer) As IntPtr
            End Function

            'public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
            'public static extern IntPtr FindWindowEx(HandleRef hwndParent, HandleRef hwndChildAfter, string lpszClass, string lpszWindow);
            <DllImport("user32.dll", SetLastError:=True)> _
            Public Shared Function FindWindowEx(hwndParent As HandleRef, hwndChildAfter As IntPtr, lpszClass As String, lpszWindow As String) As IntPtr
            End Function

            <DllImport("user32.dll", SetLastError:=True)> _
            Public Shared Function SetWindowText(hWnd As IntPtr, text As [String]) As [Boolean]
            End Function
        End Class

        <ComImport(), Guid("00000002-0000-0000-c000-000000000046"), SuppressUnmanagedCodeSecurity(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
        Public Interface IMalloc
            <PreserveSig()> _
            Function Alloc(cb As Integer) As IntPtr
            <PreserveSig()> _
            Function Realloc(pv As IntPtr, cb As Integer) As IntPtr
            <PreserveSig()> _
            Sub Free(pv As IntPtr)
            <PreserveSig()> _
            Function GetSize(pv As IntPtr) As Integer
            <PreserveSig()> _
            Function DidAlloc(pv As IntPtr) As Integer
            <PreserveSig()> _
            Sub HeapMinimize()
        End Interface

        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
        Public Class BROWSEINFO
            Public Owner As IntPtr
            Public pidlRoot As IntPtr
            Public pszDisplayName As IntPtr
            Public Title As String
            Public Flags As Integer
            Public callback As BrowseFolderCallbackProc
            Public lParam As IntPtr
            Public iImage As Integer
        End Class



        <SuppressUnmanagedCodeSecurity()> _
        Friend NotInheritable Class Shell32
            Private Sub New()
            End Sub
            ' Methods
            <DllImport("shell32.dll", CharSet:=CharSet.Auto)> _
            Public Shared Function SHBrowseForFolder(<[In]()> lpbi As PInvoke.BROWSEINFO) As IntPtr
            End Function
            <DllImport("shell32.dll")> _
            Public Shared Function SHGetMalloc(<Out(), MarshalAs(UnmanagedType.LPArray)> ppMalloc As PInvoke.IMalloc()) As Integer
            End Function
            <DllImport("shell32.dll", CharSet:=CharSet.Auto)> _
            Public Shared Function SHGetPathFromIDList(pidl As IntPtr, pszPath As IntPtr) As Boolean
            End Function
            <DllImport("shell32.dll")> _
            Public Shared Function SHGetSpecialFolderLocation(hwnd As IntPtr, csidl As Integer, ByRef ppidl As IntPtr) As Integer
            End Function
        End Class

    End Class
End Namespace


