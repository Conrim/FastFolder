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
    class AppTile : Label
    {
        private int FileIndex;
        public string FilePath;
        private bool _selected = false;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                Background = _selected ? Constants.BGColorSelected : Brushes.Transparent;
            }
        }
        public AppTile(int _fileIndex, Action HandleOnSelect)
        {
            FileIndex = _fileIndex;
            FilePath = Folder.Items[FileIndex];

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            Image icon = new Image
            {
                Source = Folder.GetIcon(FileIndex),
                Height = 32,
                Width = 32,
            };
            Label label = new Label
            {
                Content = Path.GetFileNameWithoutExtension(FilePath),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Constants.FontColor
            };

            panel.Children.Add(icon);
            panel.Children.Add(label);

            Content = panel;
            
            SetStyle();

            MouseDoubleClick += (sender, e) => OpenFile();
            MouseLeftButtonUp += (sender, e) => HandleOnSelect();
        }
        private void SetStyle()
        {
            Width = Constants.AppButtonWidth;
            Height = Constants.AppButtonHeight;

            Margin = new Thickness(3);

            VerticalContentAlignment = VerticalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            Background = new VisualBrush
            {
                Opacity = 0
            };

            ToolTip = Path.GetFileName(FilePath);
        }
        private void OpenFile()
        {
            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = FilePath;
            fileopener.Start();
        }
    }
}
