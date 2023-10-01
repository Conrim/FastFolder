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
    public static class BitmapExtensions
    {
        public static byte[] ToByteArray(this Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }

    public class FolderCache
    {
        public FolderCache()
        {
            throw new NotImplementedException();
        }
        public FolderCache(string filePath)
        {
            throw new NotImplementedException();
        }
        public void SaveToFile(string filePath)
        {
            throw new NotImplementedException();
        }
    }

    public class Shared
    {

        private static void writeBinaryFile(byte[] data, string filePath)
        {
            using (FileStream binaryWriter = new FileStream(filePath, FileMode.Create))
            {
                binaryWriter.Write(data, 0, data.Length);

            }
        }
    }
    class AppButton : Label
    {

        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; updateSelected(); }
        }
        public string FilePath;
        public AppButton(string path, Action onSelected)
        {
            FilePath = path;
            Width = Constants.AppButtonWidth;
            Height = Constants.AppButtonHeight;
            VerticalContentAlignment = VerticalAlignment.Center;
            Margin = new Thickness(3);


            VerticalAlignment = VerticalAlignment.Center;
            StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal, };
            Image icon = new Image { Source = GetIconOfFile(path), Height = 32, Width = 32, };

            Label label = new Label { Content = Path.GetFileNameWithoutExtension(path), VerticalAlignment = VerticalAlignment.Center };

            label.Foreground = Constants.FontColor;

            panel.Children.Add(icon);
            panel.Children.Add(label);


            Background = new VisualBrush { Opacity = 0 };
            Content = panel;
            ToolTip = Path.GetFileName(FilePath);

            MouseDoubleClick += (sender, e) => OpenFile();
            MouseLeftButtonUp += (sender, e) => onSelected();
            updateSelected();
        }

        void updateSelected() => Background = selected ? Constants.BGColorSelected : Brushes.Transparent;

        ImageSource GetIconOfFile(string filename)
        {
            IntPtr icon;
            if (true && File.Exists(filename))
            {
                icon = SDIcon.ExtractAssociatedIcon(filename).Handle;
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
        void OpenFile()
        {
            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = FilePath;
            fileopener.Start();
        }
    }
}
