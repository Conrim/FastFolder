using System;
using System.Text.RegularExpressions;
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
        Vector MouseClickPos;
        AppTile[] appTiles;
        int lastSelectedTile = -1;

        public MainWindow()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                // handels files moved in folder (initilized by Drag and Drop)
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
                        Folder.CreateCache(); //TODO: why here?
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
            KeyDown += OnKeyEvent;
        }
        
        // initialization
        void SetStyle()
        {
            WindowBorder.BorderBrush = WindowBorder.Background = Constants.BGColor;
            WindowBorder.BorderThickness = new Thickness(Constants.BorderThickness);
            Width = Constants.WindowWidth;
            Height = Constants.WindowHeight;
        }
        void LoadAppTiles()
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
                    appTiles[fileIndex] = CreateTile(fileIndex, row, column);
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
        AppTile CreateTile(int fileIndex, int row, int column)
        {
            AppTile tile = new AppTile(fileIndex, () => OnButtonSelected(fileIndex));
            Grid.SetRow(tile, row);
            Grid.SetColumn(tile, column);
            appGrid.Children.Add(tile);
            return tile;
        }

        // input events
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
        void OnKeyEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) {
                Application.Current.Shutdown();
                return;
            }

            if (e.Key == Key.Enter)
            {
                foreach (AppTile appTile in appTiles)
                {
                    if (appTile.Selected)
                    {
                        appTile.OpenFile();
                    }
                }
                return;
            }

            // default case: search for files which starts with the letter
            string letter = e.Key.ToString().ToLower();
            Console.WriteLine(letter);
            var match = Regex.Match(letter, @"^(d|numpad)(\d+)$");
            if (match.Success)
            {
                letter = match.Groups[2].Value; // gets number
            }
            else if (letter.Length != 1)
            {
                // sort out keys like "tab"
                return;
            }

            int firstHit = -1;
            bool selectNext = false;
            int selectIndex = -1;

            for (int i = 0; i < appTiles.Length; i++)
            {
                if (!appTiles[i].FileName.ToLower().StartsWith(letter))
                {
                    continue;
                }
                if (firstHit == -1)
                {
                    firstHit = i;
                }
                if (selectNext)
                {
                    selectIndex = i;
                    break;
                }
                else if (appTiles[i].Selected)
                {
                    selectNext = true;
                }
            }
            if (firstHit == -1)
            {
                return;
            }
            if (selectIndex == -1)
            {
                selectIndex = firstHit;
            }
            for (int i = 0; i < appTiles.Length; i++)
            {
                if (i == selectIndex)
                {
                    appTiles[selectIndex].Selected = true;
                    appTiles[selectIndex].BringIntoView();
                    continue;
                }
                appTiles[i].Selected = false;
            }
        }
        void OnButtonSelected(int tileIndex)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
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
        void OnLostFocus(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        // context menu methods
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
    }
}