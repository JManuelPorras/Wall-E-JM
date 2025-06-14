using Core.Language;
using Core.Model;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Action = Core.Language.Action;

namespace ViewModel
{
    public partial class MainWindow : Window, IPaintable
    {
        private const string CANVAS_KEY = "GridSettings";
        public const int MaxCanvasSize = 256;
        private GridSettings mapSettings;
        public Rectangle[,]? RectangMatrix { get; set; }
        private FuncDefinitions funcDefinitions;
        public int gridSize { get; }



        public MainWindow()
        {
            InitializeComponent();
            mapSettings = (Resources[CANVAS_KEY] as GridSettings)!;
            mapSettings.PropertyChanged += ResizeCanvasGrid!;
            funcDefinitions = new FuncDefinitions(this);

            funcDictionary = new()
                {
                    {"GetActualX", (Portals.Portal(funcDefinitions.GetActualX), [], typeof(int)) },
                    {"GetActualY", (Portals.Portal(funcDefinitions.GetActualY), [], typeof(int)) },
                    {"GetCanvasSize", (Portals.Portal(funcDefinitions.GetCanvasSize), [], typeof(int)) },
                    {"GetColorCount", (Portals.Portal<string,int,int,int,int,int>(funcDefinitions.GetColorCount), [typeof(string),typeof(int),typeof(int),typeof(int),typeof(int)], typeof(int)) },
                    {"IsBrushColor", (Portals.Portal<string,int>(funcDefinitions.IsBrushColor), [typeof(string)], typeof(int)) },
                    {"IsBrushSize", (Portals.Portal<int,int>(funcDefinitions.IsBrushSize), [typeof(int)], typeof(int)) },
                    {"IsCanvasColor", (Portals.Portal<string,int,int,int>(funcDefinitions.IsCanvasColor), [typeof(string),typeof(int),typeof(int)], typeof(int)) },

                };

            actionDictionary = new()
                {
                    {"Spawn", (Portals.Portal<int, int>(funcDefinitions.Spawn), [typeof(int), typeof (int)]) },
                    {"MoveWallee", (Portals.Portal<int, int>(funcDefinitions.MoveWallee), [typeof(int), typeof (int)]) },
                    {"Color", (Portals.Portal<string>(funcDefinitions.Color), [typeof(string)]) },
                    {"Size", (Portals.Portal<int>(funcDefinitions.Size), [typeof(int)]) },
                    {"DrawLine", (Portals.Portal<int,int,int>(funcDefinitions.DrawLine), [typeof(int), typeof(int), typeof(int)]) },
                    {"DrawRectangle", (Portals.Portal<int,int,int,int,int>(funcDefinitions.DrawRectangle), [typeof(int), typeof(int), typeof(int),typeof(int),typeof(int)]) },
                    {"Fill", (Portals.Portal(funcDefinitions.Fill), []) },
                    {"DrawCircle", (Portals.Portal<int,int,int>(funcDefinitions.DrawCircle), [typeof(int), typeof(int), typeof(int)]) },

                };

            gridSize = mapSettings.GridSize;
            PixelColor = Brushes.Transparent;

#if DEBUG
            // En modo DEBUG, subimos desde el directorio de ejecución hasta la raíz del proyecto
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var path = directory
                .Parent? // bin
                .Parent? // Debug
                .Parent? // netX.Y
                .FullName!;
#else
            path = AppDomain.CurrentDomain.BaseDirectory;
#endif
            path = System.IO.Path.Combine(path, @"Assent\WALL-E-PNG-Transparent.png");
            walleeImage = new BitmapImage(new Uri(path));
        }

        public Dictionary<string, (Func, Type[], Type)> funcDictionary { get; private set; }
        public Dictionary<string, (Action, Type[])> actionDictionary { get; private set; }

        private void ResetCanvas()
        {
            var cols = (int)(CanvasGrid.ActualHeight / mapSettings.CellSize);
            var rows = (int)(CanvasGrid.ActualWidth / mapSettings.CellSize);
            mapSettings.GridSize = Math.Min(cols, rows);
            RectangMatrix = new Rectangle[mapSettings.GridSize, mapSettings.GridSize];
            CanvasGrid.ColumnDefinitions.Clear();
            CanvasGrid.RowDefinitions.Clear();
            CanvasGrid.Children.Remove(wallee);
            OnlyOneSpawn = true;
        }

        private void InitializeCanvas()
        {
            var gridLength = new GridLength(1, GridUnitType.Star);
            for (int i = 0; i < mapSettings.GridSize; i++)
            {
                CanvasGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = gridLength });
                CanvasGrid.RowDefinitions.Add(new RowDefinition() { Height = gridLength });
            }

            //filas
            for (int i = 0; i < RectangMatrix!.GetLength(0); i++)
            {
                //columnas
                for (int k = 0; k < RectangMatrix.GetLength(1); k++)
                {
                    var cell = new Rectangle()
                    {
                        Fill = Brushes.White,
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.5,
                    };
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, k);

                    RectangMatrix[i, k] = cell;
                    CanvasGrid.Children.Add(cell);
                }
            }
        }

        private void ResizeCanvasGrid(object sender, PropertyChangedEventArgs e)
        {
            ResetCanvas();
            InitializeCanvas();

        }

        private void CanvasGrid_Loaded(object sender, RoutedEventArgs e)
        {
            ResetCanvas();
            InitializeCanvas();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearCanvas();
                funcDefinitions = new FuncDefinitions(this);
                string[] lines = GetLines();

                var codemanager = new CoreManager(lines, funcDictionary, actionDictionary);
                if (codemanager.errors != null)
                    ErrorLabel.Content = string.Join("\n", codemanager.errors);
                codemanager.blockInstruction.Execute(codemanager.context); 

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing code: {ex.Message}",
                                "Execution Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }



        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            { 
                Filter = "PW Files (*.pw)|*.pw|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    CodeEditor.Text = File.ReadAllText(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}",
                                  "Load Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "PW Files (*.pw)|*.pw|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, CodeEditor.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}",
                                    "Save Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        private void CodeEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            funcDefinitions = new FuncDefinitions(this);
            string[] lines = GetLines();

            var codemanager = new CoreManager(lines, funcDictionary, actionDictionary);

            if (codemanager.errors != null)
            {
                var strErrors = codemanager.errors.Select(x => x.ToString());
                ErrorLabel.Content = string.Join("\n", strErrors);
            }
        }
    }
}