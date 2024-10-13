using System;
using System.Windows;
using System.IO;
using File = System.IO.File;
using IWshRuntimeLibrary;

using System.Reflection;
using System.Text.RegularExpressions;

namespace FolderCreater
{
    public partial class MainWindow : Window
    {
        private string parentFolderPath = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
        private string folderOpenerPath; // = Path.Combine(parentFolderPath, "OpenFolder.exe")
        public MainWindow()
        {
            folderOpenerPath = @"K:\FastFolder\FolderOpener\bin\Release\FolderOpener.exe"; //Path.Combine(parentFolderPath, "OpenFolder.exe");
            InitializeComponent();
            CwdTextBox.Text = Directory.GetCurrentDirectory();
        }
        private string GetFreePath(string wantedPath) => GetFreePath(wantedPath, "");
        private string GetFreePath(string wantedPath, string extension)
        {
            string path = wantedPath + extension;
            int i = 1;
            while (Directory.Exists(path) || File.Exists(path))
            {
                path = wantedPath + "_" + i.ToString() + extension;
                i++;
            }
            return path;
        }
        void Cancel(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        void OnCreateFolderAndShortcut(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = CreateFolderAndShortcut();
                MsgBox.Text = $"Folder: '{Path.GetFileName(folderPath)}' erfolgreich erstellt";
            }
            catch (Exception exception)
            {
                MsgBox.Text = exception.Message;
            }
        }
        string CreateFolderAndShortcut()
        {
            // Error Checking
            Regex re = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]"); //Stackoverflow Code
            if (re.IsMatch(NameTextBox.Text)) throw new Exception($"Folder name '{NameTextBox.Text}' includes illegal characters");
            if (!Directory.Exists(CwdTextBox.Text)) throw new Exception($"The path '{CwdTextBox.Text}' does not exist");

            string lnkFile = GetFreePath(Path.Combine(CwdTextBox.Text, NameTextBox.Text), ".lnk");
            string folderPath = GetFreePath(Path.Combine(parentFolderPath, @"data\" + Path.GetFileNameWithoutExtension(lnkFile)));


            // creates Folder
            Directory.CreateDirectory(folderPath);

            // creates Shortcut
            WshShell shell = new WshShell();
            IWshShortcut shortcut = shell.CreateShortcut(lnkFile);
            shortcut.WorkingDirectory = folderPath;
            shortcut.TargetPath = folderOpenerPath;
            shortcut.Save();

            return Path.GetFileNameWithoutExtension(lnkFile);
        }
    }
}
