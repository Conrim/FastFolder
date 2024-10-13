using System.Windows;
using System.Diagnostics;

namespace FolderOpener
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void Restart()
        {
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(appPath);
            Shutdown();
        }
    }
}