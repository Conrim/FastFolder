using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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


namespace FolderOpener
{
    public class Folder
    {
        // private static ... IconDict
        private static List<string> _items = null;
        public static List<string> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = Directory.GetFiles(Constants.Cwd).Concat(Directory.GetDirectories(Constants.Cwd)).ToList();
                }
                return _items;
            }
        }
        
        public static ImageSource GetIcon(int IndexItem)
        {
            string PathItem = Items[IndexItem];
            IntPtr icon;
            if (File.Exists(PathItem))
            {
                icon = SDIcon.ExtractAssociatedIcon(PathItem).Handle;
            }
            else
            {
                icon = Constants.FolderIcon;
            }

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon,
                new Int32Rect(0, 0, 0, 0),
                BitmapSizeOptions.FromWidthAndHeight(32, 32)
            );
            return imageSource;
        }

        public static void CreateCache()
        {
            throw new NotImplementedException();
        }
        public static void AddItem(string OldPath)
        {
            throw new NotImplementedException();
        }
    }
}
