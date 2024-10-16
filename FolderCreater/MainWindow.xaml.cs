using System;
using System.Windows;
using System.IO;
using File = System.IO.File;
using IWshRuntimeLibrary;

using System.Reflection;
using System.Text.RegularExpressions;
using ModernWpf;

namespace FolderCreater
{
    public partial class MainWindow : Window
    {
        private string parentFolderPath = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
        private string folderOpenerPath;
        public MainWindow()
        {
            folderOpenerPath = Path.Combine(parentFolderPath, "FolderOpener.exe");
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
        void OnCreateFolderAndShortcut(object sender, RoutedEventArgs e)
        {
            try
            {
                bool successful = CreateFolderAndShortcut();
                if (successful)
                {
                    Application.Current.Shutdown();
                }
            }
            catch (Exception exception)
            {
                ModernWpf.MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        bool CreateFolderAndShortcut() // false -> no Folder was created. true -> Folder was created
        {
            // Error Checking
            Regex re = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]"); //Stackoverflow Code
            if (re.IsMatch(NameTextBox.Text)) throw new Exception($"Folder name '{NameTextBox.Text}' includes illegal characters");
            if (!Directory.Exists(CwdTextBox.Text)) throw new Exception($"The path '{CwdTextBox.Text}' does not exist");

            string lnkFile = GetFreePath(Path.Combine(CwdTextBox.Text, NameTextBox.Text), ".lnk");
            if (lnkFile != Path.Combine(CwdTextBox.Text, NameTextBox.Text) + ".lnk")
            {
                // folder with name allready exists
                if (ModernWpf.MessageBox.Show($"'{NameTextBox.Text}' allready exists.\n\nChange name to '{Path.GetFileNameWithoutExtension(lnkFile)}'?", "Rename or Cancel", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            string folderPath = GetFreePath(Path.Combine(parentFolderPath, @"data\" + Path.GetFileNameWithoutExtension(lnkFile)));

            // creates Folder
            Directory.CreateDirectory(folderPath);

            // creates Shortcut
            WshShell shell = new WshShell();
            IWshShortcut shortcut = shell.CreateShortcut(lnkFile);
            shortcut.WorkingDirectory = folderPath;
            shortcut.TargetPath = folderOpenerPath;
            shortcut.Save();

            return true;
        }
    }
}
