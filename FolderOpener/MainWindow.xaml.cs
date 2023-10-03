using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Path = System.IO.Path;
using System.Collections.Specialized;
using System.Diagnostics;

namespace FolderOpener
{
    public partial class MainWindow : Window
    {
        private Vector MouseClickPos;
        private AppTile[] appTiles;
        private int lastSelectedTile = -1;
        public MainWindow()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {


                string[] SysArgv = Environment.GetCommandLineArgs();
                for (int i=1; i < SysArgv.Length; i++)
                {
                    Console.WriteLine(SysArgv[i]);
                    try
                    {
                        string newFileLocation = Path.Combine(Constants.Cwd, Path.GetFileName(SysArgv[i]));
                        Console.WriteLine($"tries to move {SysArgv[i]} -> {newFileLocation}");
                        if (File.Exists(SysArgv[i]))
                        {
                            File.Move(SysArgv[i], newFileLocation);
                        }
                        else if (Directory.Exists(SysArgv[i]))
                        {
                            Directory.Move(SysArgv[i], newFileLocation);
                        }
                        else
                        {
                            throw new Exception($"Die Datei:'{SysArgv[i]}' existiert nicht");
                        }
                        Folder.CreateCache();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Fehler {e.Message}");
                    }
                }
                Application.Current.Shutdown();
            }

            InitializeComponent();
            SetStyle();
            LoadAppTiles();
        }
        private void LoadAppTiles()
        {
            appTiles = new AppTile[Folder.Items.Count];
            CreateGridCells(appTiles.Length);

            int column = 0;
            int fileIndex;
            while (true)
            {
                for (int row = 0; row < Constants.NumRows; row++)
                {
                    fileIndex = row + column * Constants.NumRows;
                    if (fileIndex == appTiles.Length) return;
                    appTiles[fileIndex] = AddTile(fileIndex, row, column);
                }
                column++;
            }
        }
        void CreateGridCells(int NumApps)
        {
            for (int i = 0; i < Constants.NumRows; i++)
            {
                appGrid.RowDefinitions.Add(new RowDefinition { });
            }
            for (int i=0; i < NumApps; i += Constants.NumRows)
            {
                appGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Constants.AppButtonWidth)});
            }
        }
        AppTile AddTile(int fileIndex, int row, int column)
        {
            AppTile tile = new AppTile(fileIndex, () => OnButtonSelected(fileIndex));
            Grid.SetRow(tile, row);
            Grid.SetColumn(tile, column);
            appGrid.Children.Add(tile);
            return tile;
        }
        void SetStyle()
        {
            WindowBorder.BorderBrush = WindowBorder.Background = Constants.BGColor;
            WindowBorder.BorderThickness = new Thickness(Constants.BorderThickness);
            Width = Constants.WindowWidth;
            Height = Constants.WindowHeight;

        }
        void OnButtonSelected(int tileIndex)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                Debug.WriteLine("Select shit");
                appTiles[tileIndex].Selected = !appTiles[tileIndex].Selected;
                lastSelectedTile = tileIndex;
                return;
            }

            foreach (AppTile tile in appTiles)
            {
                tile.Selected = false;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift && lastSelectedTile != -1)
            {
                int min = Math.Min(lastSelectedTile, tileIndex);
                int max = Math.Max(lastSelectedTile, tileIndex);
                for (int i = min; i <= max; i++)
                {
                    appTiles[i].Selected = true;
                }
                return;
            }

            appTiles[tileIndex].Selected = true;
            lastSelectedTile = tileIndex;
        }

        StringCollection GetSelectedFiles()
        {
            StringCollection selectedFiles = new StringCollection();
            foreach (AppTile tile in appTiles)
            {
                if (tile.Selected)
                {
                    selectedFiles.Add(tile.FilePath);
                }
            }
            return selectedFiles;
        }
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseClickPos = (Vector)e.GetPosition(this);
        }
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            // handels file drag
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (MouseClickPos == null || ((Vector)e.GetPosition(this) - MouseClickPos).LengthSquared < Constants.MinMoveDistSqrt) return;
            StringCollection selectedFiles = GetSelectedFiles();
            if (selectedFiles.Count == 0) return;
            DataObject data = new DataObject();
            data.SetFileDropList(selectedFiles);
            DragDropEffects output = DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            if (output == DragDropEffects.Move)
            {
                Application.Current.Shutdown();
                Folder.CreateCache();
            }
        }
        void OnLostFocus(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        void OpenInExplorer(object sender, RoutedEventArgs e)
        {
            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = Constants.Cwd;
            fileopener.Start();
        }
        void ReloadAndRestart(object sender, RoutedEventArgs e)
        {
            Folder.CreateCache();
            Folder.LoadFromCache();

            ((App)Application.Current).Restart();
        }
        void OnScroll(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            int speed = 5;
            if (e.Delta > 0)
            {
                for (int i = 0; i < speed; i++)
                {
                    scrollviewer.LineLeft();
                }
            }
            else
            {
                for (int i = 0; i < speed; i++)
                {
                    scrollviewer.LineRight();
                }
            }
            e.Handled = true;
        }
    }
}
