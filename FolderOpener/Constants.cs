using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Reflection;
using System.IO;
using Path = System.IO.Path;
using SDIcon = System.Drawing.Icon;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using SDColor = System.Drawing.Color;

namespace FolderOpener
{
    static class Constants
    {
        public static string Cwd = Directory.GetCurrentDirectory(); // current working directory
        public static string ParentDir = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
        public static string CacheFileName = "FastFolderCache";
        public static string CachePath = Cwd + "\\" + CacheFileName;

        private static SDIcon _folderIcon = null;
        public static SDIcon FolderIcon
        {
            get
            {
                if (_folderIcon == null)
                {
                    _folderIcon = new SDIcon(Path.Combine(ParentDir, "folder.ico"));
                }
                return _folderIcon;
            }
        }

        public const int MinMoveDistSqrt = 200; // min move distance squared for drag and drop

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
