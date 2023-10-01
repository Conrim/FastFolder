using System;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;
using System.Windows.Interop;
using SDIcon = System.Drawing.Icon;

using Bitmap = System.Drawing.Bitmap;
using System.Reflection;
using System.Collections.Specialized;
using System.Diagnostics;

using System.Drawing.Imaging;

namespace FolderOpener
{
    static class Constants
    {
        public static string Cwd = Directory.GetCurrentDirectory(); // current working directory
        public static string ParentDirectory = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;

        private static IntPtr _folderIcon = IntPtr.Zero;
        public static IntPtr FolderIcon
        {
            get
            {
                if (_folderIcon == IntPtr.Zero)
                {
                    _folderIcon = new Bitmap(Path.Combine(ParentDirectory, "folder.ico")).GetHicon();
                }
                return _folderIcon;
            }
        }

        public const int MinMoveDistSqrt = 200;

        // style related
        public static Brush BGColor = new SolidColorBrush(new Color
        {
            R = Convert.ToByte(0),
            G = Convert.ToByte(0),
            B = Convert.ToByte(20),
            A = Convert.ToByte(200)
        });
        public static Brush FontColor = new SolidColorBrush(new Color
        {
            R = Convert.ToByte(200),
            G = Convert.ToByte(200),
            B = Convert.ToByte(200),
            A = Convert.ToByte(255)
        });
        public static Brush BGColorSelected = new SolidColorBrush(new Color
        {
            R = Convert.ToByte(0),
            G = Convert.ToByte(25),
            B = Convert.ToByte(255),
            A = Convert.ToByte(100)
        });
        public const int BorderThickness = 20;
        public const int WindowWidth = 900 + 2 * BorderThickness;
        public static int WindowHeight = NumRows * AppButtonHeight + 2 * BorderThickness;
        public const int NumRows = 10;
        public const int AppButtonWidth = 200;
        public const int AppButtonHeight = 70;
    }
}
