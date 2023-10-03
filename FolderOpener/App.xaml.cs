﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Diagnostics;
using System.Windows;
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
