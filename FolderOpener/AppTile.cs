using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Path = System.IO.Path;
using System.Diagnostics;

namespace FolderOpener
{
    class AppTile : Label
    {
        private int FileIndex;
        public string FilePath;
        public string FileName;
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
            FileName = Path.GetFileNameWithoutExtension(FilePath);

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Image icon = new Image
            {
                //Source = Constants.EmptyImgScr,
                Source = Folder.GetIcon(FileIndex),
                Height = 32,
                Width = 32,
            };
            
            Label label = new Label
            {
                Content = FileName,
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
        public void OpenFile()
        {
            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = FilePath;
            fileopener.Start();
        }
    }
}
