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
using System.Runtime.InteropServices;

using Graphics = System.Drawing.Graphics;
using SDColor = System.Drawing.Color;
namespace FolderOpener
{
    static class Constants
    {
        public static string Cwd = Directory.GetCurrentDirectory(); // current working directory
        public static string ParentDir= Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
        public static string CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FastFolder\\cache";

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
        public static ImageSource _emptyImgScr;
        public static ImageSource EmptyImgScr
        {
            get
            {
                if (_emptyImgScr == null)
                {
                    Bitmap transparentBitmap = new Bitmap(32, 32);
                    using (Graphics g = Graphics.FromImage(transparentBitmap))
                    {
                        g.Clear(SDColor.Transparent);
                    }
                    _emptyImgScr = Imaging.CreateBitmapSourceFromHIcon(transparentBitmap.GetHicon(), new Int32Rect(0, 0, 0, 0),
                    BitmapSizeOptions.FromWidthAndHeight(32, 32));
                }
                return _emptyImgScr;
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
