using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PixelArtApp
{
    public partial class MainWindow : Window
    {
        private int canvasSize = 20;
        private const int MaxCanvasSize = 256;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCanvas();
        }

        private void InitializeCanvas()
        {
            PixelCanvas.ItemsSource = new int[canvasSize * canvasSize];

        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CanvasSizeTextBox.Text, out int newSize) && newSize > 0 && newSize <= MaxCanvasSize)
            {
                canvasSize = newSize;
                InitializeCanvas();
            }
            else
            {
                MessageBox.Show($"Please enter a valid number between 1 and {MaxCanvasSize}",
                              "Invalid Input",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = CodeEditor.Text;
                // Aquí procesarías el código para modificar el canvas
                // Esto es un ejemplo simple que pinta algunos píxeles
                ExecuteSampleCode();
                throw new NotImplementedException("Esto no esta hecho");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing code: {ex.Message}",
                                "Execution Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ExecuteSampleCode()
        {
            // Limpiar canvas primero (opcional)
            ClearCanvas();

            // Ejemplo: dibuja patrones básicos
            for (int i = 0; i < canvasSize; i++)
            {
                SetPixelColor(i, i, Brushes.Red); // Diagonal principal
                SetPixelColor(i, canvasSize - 1 - i, Brushes.Blue); // Diagonal secundaria

                // Bordes
                SetPixelColor(0, i, Brushes.Green);
                SetPixelColor(canvasSize - 1, i, Brushes.Green);
                SetPixelColor(i, 0, Brushes.Green);
                SetPixelColor(i, canvasSize - 1, Brushes.Green);
            }
        }

        private void ClearCanvas()
        {
            for (int y = 0; y < canvasSize; y++)
            {
                for (int x = 0; x < canvasSize; x++)
                {
                    SetPixelColor(x, y, Brushes.White);
                }
            }
        }

        private void SetPixelColor(int x, int y, Brush color)
        {
            if (x >= 0 && x < canvasSize && y >= 0 && y < canvasSize)
            {
                var container = (ContentPresenter)PixelCanvas.ItemContainerGenerator.ContainerFromIndex(y * canvasSize + x);
                if (container != null)
                {
                    var border = (Border)VisualTreeHelper.GetChild(container, 0);
                    var rectangle = (Rectangle)border.Child;
                    rectangle.Fill = color;
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "GW Files (*.gw)|*.gw|All files (*.*)|*.*",
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
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "GW Files (*.gw)|*.gw|All files (*.*)|*.*",
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
    }
}