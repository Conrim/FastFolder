' this script removes all FastFolder on the desktop

' to prevent accidental removal of shortcuts
if WScript.Arguments.Count <> 2 Then
    WScript.Echo("WRONG PARAMETER")
    Wscript.Quit()
ElseIf WScript.Arguments(0) <> "rm desktop shortcuts" Then
    WScript.Echo("WRONG PARAMETER")
    Wscript.Quit()
End If

Dim folderopener_exe_path
folderopener_exe_path = WScript.Arguments(1) ' i. e. "C:\Program Files (x86)\FastFolder\FolderOpener.exe"

Dim objShell,  objFSO
Set objShell = CreateObject("WScript.Shell")
Set objFSO = CreateObject("Scripting.FileSystemObject")

Function ListDesktopShortcuts()
    Dim objFolder, objFile
    Dim desktopPath, lnkFiles()
    Dim fileCount

    ' Get the user's desktop folder path
    desktopPath = objShell.SpecialFolders("Desktop")

    ' Get the desktop folder as an object
    Set objFolder = objFSO.GetFolder(desktopPath)

    ' Initialize a dynamic array to store .lnk files
    fileCount = 0
    ReDim lnkFiles(fileCount)

    ' Loop through each file in the desktop folder
    For Each objFile in objFolder.Files
        ' Check if the file ends with ".lnk"
        If LCase(objFSO.GetExtensionName(objFile.Name)) = "lnk" Then
            ' Add the .lnk file path to the array
            ReDim Preserve lnkFiles(fileCount)  ' Resize the array dynamically
            lnkFiles(fileCount) = objFile.Path  ' Add the file path to the array
            fileCount = fileCount + 1           ' Increment the file count
        End If
    Next

    ' Return the array of .lnk files
    ListDesktopShortcuts = lnkFiles
End Function

Function IsFastFolder(path_to_shortcut)
    Dim objShortcut, targetPath

    ' Try to load the shortcut
    On Error Resume Next
    Set objShortcut = objShell.CreateShortcut(path_to_shortcut)
    If Err.Number <> 0 Then
        IsFastFolder = False ' Return False if loading the shortcut fails
        Exit Function
    End If

    ' Get the target path of the shortcut
    targetPath = objShortcut.TargetPath

    If LCase(targetPath) <> "" AND LCase(targetPath) = LCase(folderopener_exe_path) Then
        IsFastFolder = True  ' Return True if the paths match
    Else
        IsFastFolder = False ' Return False if the paths don't match
    End If
    On Error GoTo 0
End Function

For Each lnkFile In ListDesktopShortcuts()
    If IsFastFolder(lnkFile) Then
        On Error Resume Next
        ' WScript.Echo "delete " + lnkFile
        objFSO.DeleteFile lnkFile
        On Error Goto 0
    End If
Next