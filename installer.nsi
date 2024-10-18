# NSIS Script for FastFolder Installer

# Define the installer name and output file
OutFile "FastFolder_Setup.exe"

# Define the name of the application and the developer details
Name "FastFolder"
Caption "FastFolder Installer"
BrandingText "FastFolder by Conrim (www.github.com/conrim)"

# Set the default installation directory, but let the user choose
InstallDir "$PROGRAMFILES\FastFolder_Conrim"

# Set the installation directory based on the registry (if available)
InstallDirRegKey HKCU "Software\FastFolder_Conrim" "Install_Dir"

# Request that the user selects the installation directory
Page directory
# Display component selection page for optional desktop shortcut
Page components
# Display installation progress and final button (this fixes the issue of only seeing "Close")
Page instfiles

# Show detailed installation and uninstallation progress
ShowInstDetails show
ShowUninstDetails show

# Section for installing FastFolder
Section
    SetOutPath "$INSTDIR"
    
    # Create folder for data
    CreateDirectory "$APPDATA\FastFolder_Conrim"
    
    # Add the files
    File "FolderOpener\folder.ico"
    
    File "scripts\uninstall_helper.vbs"

    File "FolderOpener\bin\Release\FolderOpener.exe"

    File "FolderCreater\bin\Release\FolderCreater.exe"
    File "FolderCreater\bin\Release\ModernWpf.dll"
    File "FolderCreater\bin\Release\ModernWpf.MessageBox.dll"
    
    # Add Uninstaller
    WriteUninstaller "$INSTDIR\uninstall.exe"

    # Save installation directory in the registry
    WriteRegStr HKCU "Software\FastFolder_Conrim" "Install_Dir" "$INSTDIR"
    
    # Set uninstall information
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastFolder_Conrim" "DisplayName" "FastFolder"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastFolder_Conrim" "UninstallString" "$INSTDIR\uninstall.exe"
    
    # Set current working directory for shourtcuts
    SetOutPath "$DESKTOP"

    # Automatically create a Start Menu shortcut
    CreateShortCut "$SMPROGRAMS\FastFolder - FolderCreater.lnk" "$INSTDIR\FolderCreater.exe"
SectionEnd

Section "Desktop Shortcut (Recommended)"
    # Create a desktop shortcut
    CreateShortCut "$DESKTOP\FastFolder - FolderCreater.lnk" "$INSTDIR\FolderCreater.exe"
SectionEnd

Section "Start after installation"
    # Run Application
    Exec "$INSTDIR\FolderCreater.exe"
SectionEnd

# Uninstaller Section
Section "Uninstall"
    # Display warning message box
    MessageBox MB_ICONEXCLAMATION|MB_YESNO  "Warning: Continuing will delete all files in all FastFolders. Do you wish to proceed?" IDNO cancel IDYES proceed
    cancel:
        Quit
    proceed:
        # Remove the registry entries
        DeleteRegKey HKCU "Software\FastFolder_Conrim"
        DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastFolder_Conrim"

        # Remove desktop shortcut if they exist
        Delete "$DESKTOP\FastFolder - FolderCreater.lnk"
        
        # Remove Start Menu shortcut
        Delete "$SMPROGRAMS\FastFolder - FolderCreater.lnk"
        
        # Remove desktop shortcuts (Created Folders)
        ExecWait '"$SYSDIR\wscript.exe" "$INSTDIR\uninstall_helper.vbs" "rm desktop shortcuts" "$INSTDIR\FolderOpener.exe"'
        
        # Remove the data directory
        RMDir /r "$APPDATA\FastFolder_Conrim"
        
        # Delete the installation directory
        RMDir /r "$INSTDIR"
SectionEnd