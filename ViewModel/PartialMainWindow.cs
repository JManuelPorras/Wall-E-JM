using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ViewModel
{
    public partial class MainWindow : Window, IPaintable
    {
        private readonly BitmapImage walleeImage;
        private const string walleeName = "actualWallee";
        private Image? wallee;
        public int XWalleePosition { get; set; } = -1;
        public int YWalleePosition { get; set; } = -1;
        public Brush PixelColor { get; set; }
        public int BrushSize { get; set; }
        public bool OnlyOneSpawn { get; set; } = true;

        // Direcciones: [fila, columna]
        public (int, int)[] Directions { get; } =
        {
          (-1, 0),  // Arriba
          (1, 0),   // Abajo
          (0, -1),  // Izquierda
          (0, 1),   // Derecha
          (-1, -1), // Arriba-Izquierda
          (-1, 1),  // Arriba-Derecha
          (1, -1),  // Abajo-Izquierda
          (1, 1)    // Abajo-Derecha
        };

        public string[] GetLines()
        {
            string[] lines = new string[CodeEditor.LineCount];

            for (int i = 0; i < CodeEditor.LineCount; i++)
            {
                lines[i] = CodeEditor.GetLineText(i);

                string input = lines[i];
                string toRemove = " ";

                int lastIndex = input.LastIndexOf(toRemove);
                if (lastIndex != -1)
                {
                    string result = input.Remove(lastIndex, toRemove.Length);
                    lines[i] = result;
                }


            }
            return lines;
        }

        public void PaintWallee(int x, int y)
        {
            if (!IsValidPosition(x, y)) return;
            if (XWalleePosition > -1)
                CanvasGrid.Children.Remove(wallee);
            wallee = new Image() { Name = walleeName, Source = walleeImage };
            Grid.SetRow(wallee, x);
            Grid.SetColumn(wallee, y);
            CanvasGrid.Children.Add(wallee);
            XWalleePosition = x;
            YWalleePosition = y;
        }

        public bool IsValidPosition(int x, int y)
        {
            if (x < 0 || y < 0 || x >= RectangMatrix!.GetLength(0) || y >= RectangMatrix!.GetLength(1))
                return false;
            else return true;
        }

        private void ClearCanvas()
        {
            for (int y = 0; y < RectangMatrix!.GetLength(1); y++)
            {
                for (int x = 0; x < RectangMatrix!.GetLength(0); x++)
                {
                    SetPixelColor(x, y, Brushes.White);
                }
            }
            XWalleePosition = -1;
            YWalleePosition = -1;
            CanvasGrid.Children.Remove(wallee);
            OnlyOneSpawn = true;
            PixelColor = default!; 
            BrushSize = 1;
        }

        public void SetPixelColor(int x, int y)
        {
            var dif = BrushSize / 2;
            for (int i = x - dif; i <= x + dif; i++)
            {
                for (int k = y - dif; k <= y + dif; k++)
                {
                    if (IsValidPosition(i, k))
                    {
                        RectangMatrix![i, k].Fill = PixelColor;
                    }
                }
            }
        }

        //private bool IsValidZone(int x, int y)
        //{

        //    var dif = BrushSize / 2;
        //    for (int i = 0; i < 4; i++)
        //    {
        //        (int, int) position = (x + Directions[i].Item1 * dif, y + Directions[i].Item2 * dif);
        //        if (position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= RectangMatrix!.GetLength(0) || position.Item2 >= RectangMatrix!.GetLength(1))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public void SetPixelColor(int x, int y, Brush color)
        {

            if (x >= 0 && x < RectangMatrix!.GetLength(0) && y >= 0 && y < RectangMatrix!.GetLength(1))
            {
                RectangMatrix[x, y].Fill = color;
            }
        }



    }
}
