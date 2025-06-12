using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ViewModel
{
    public class FuncDefinitions
    {
        private IPaintable _paintable;
        private bool OnlyOneSpawn = true;

        public FuncDefinitions(IPaintable paintable)
        {
            _paintable = paintable;
        }

        public void Spawn(int x, int y)
        {
            if (!OnlyOneSpawn)
            {
                throw new Exception("Wallee ya esta posicionado");
            }
            _paintable.PaintWallee(x, y);
            OnlyOneSpawn = false;
        }

        public void Color(string color)
        {
            switch (color)
            {
                case "Blue":
                    {
                        _paintable.PixelColor = Brushes.Blue;
                        break;
                    }
                case "Red":
                    {
                        _paintable.PixelColor = Brushes.Red;
                        break;
                    }
                case "Green":
                    {
                        _paintable.PixelColor = Brushes.Green;
                        break;
                    }
                case "Yelow":
                    {
                        _paintable.PixelColor = Brushes.Yellow;
                        break;
                    }
                case "Orange":
                    {
                        _paintable.PixelColor = Brushes.Orange;
                        break;
                    }
                case "Purple":
                    {
                        _paintable.PixelColor = Brushes.Purple;
                        break;
                    }
                case "Black":
                    {
                        _paintable.PixelColor = Brushes.Black;
                        break;
                    }
                case "White":
                    {
                        _paintable.PixelColor = Brushes.White;
                        break;
                    }
                case "Transparent":
                    {
                        _paintable.PixelColor = Brushes.Transparent;
                        break;
                    }
                default:
                    throw new Exception("El color del pincel no es valido");

            }
        }

        public void Size(int brushSize)
        {
            if (brushSize % 2 == 0) brushSize = brushSize - 1;
            if (brushSize > 0)
                _paintable.BrushSize = brushSize;
            else throw new Exception("La dimension de pincel no es valida");
        }

        public void DrawLine(int dirX, int dirY, int distance)
        {
            if (!ValidDir(dirX, dirY))
                throw new Exception("Los valores de la direccion no son validos");

            if (distance < 0) throw new Exception("La distancia no puede ser negativa");

            for (int i = 1; i <= distance; i++)
            {
                var currentX = dirX * i;
                var currentY = dirY * i;
                _paintable.SetPixelColor(currentX, currentY);
                _paintable.PaintWallee(dirX * (i + 1), dirY * (i + 1));
            }

        }

        private bool ValidDir(int dirX, int dirY)
        {
            if (dirX < -1 || dirX > 1 || dirY < -1 || dirY > 1) return false;
            return true;
        }

        public void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
        {
            if (!ValidDir(dirX, dirY))
                throw new Exception("Los valores de la direccion no son validos");

            if (distance < 0) throw new Exception("La distancia no puede ser negativa");

            var currentX = GetActualX() + dirX * distance;
            var currentY = GetActualY() + dirY * distance;
            if (!_paintable.IsValidPosition(currentX, currentY)) return;
            _paintable.PaintWallee(currentX, currentY);

            PaintRectangle(width, height, currentX, currentY);
        }

        private void PaintRectangle(int width, int height, int currentX, int currentY)
        {
            int limUp = currentX - height / 2 - 1;
            int limDown = currentX + height / 2 + 1;
            int limLeft = currentY - width / 2 - 1;
            int limRight = currentY + width / 2 + 1;

            for (int i = limLeft; i <= limRight; i++)
            {
                _paintable.SetPixelColor(limDown, i);
                _paintable.SetPixelColor(limUp, i);
            }

            for (int i = limUp; i <= limDown; i++)
            {
                _paintable.SetPixelColor(i, limLeft);
                _paintable.SetPixelColor(i, limRight);
            }
        }

        public void Fill()
        {
            var actualSize = _paintable.BrushSize;
            _paintable.BrushSize = 1;
            (int, int) InitialPos = (_paintable.XWalleePosition, _paintable.YWalleePosition);
            Queue<(int, int)> values = new Queue<(int, int)>();
            values.Enqueue(InitialPos);
            bool anyElement = true;
            var actualColor = _paintable.PixelColor;

            while (anyElement)
            {
                var firstItem = values.Dequeue();
                _paintable.SetPixelColor(firstItem.Item1, firstItem.Item2);

                for (int i = 0; i < 4; i++)
                {
                    int nx = _paintable.Directions[i].Item1 + firstItem.Item1;
                    int ny = _paintable.Directions[i].Item2 + firstItem.Item2;
                    if (IsValidItem(nx, ny, actualColor))
                        values.Enqueue((nx, ny));

                }
                anyElement = values.Any();
            }
            _paintable.BrushSize = actualSize;
            _paintable.PaintWallee(GetActualX(), GetActualY());
        }

        private bool IsValidItem(int nx, int ny, Brush actualColor)
        {
            if (_paintable.RectangMatrix![nx, ny].Fill != actualColor || !_paintable.IsValidPosition(nx, ny))
                return false;
            return true;
        }

        public void DrawCircle(int dirX, int dirY, int radius)
        {
            if (!ValidDir(dirX, dirY))
                throw new Exception("Las direcciones son invalidas");

            if (radius < 0) throw new Exception("El radio no puede ser negativo");

            var currentX = _paintable.XWalleePosition + dirX * radius;
            var currentY = _paintable.YWalleePosition + dirY * radius;
            if (!_paintable.IsValidPosition(currentX, currentY)) return;
            _paintable.PaintWallee(currentX, currentY);

            DrawCircle1(radius, currentX, currentY);
        }

        private void DrawCircle1(int radius, int centerX, int centerY)
        {
            int x = radius;
            int y = 0;
            int d = 1 - radius;

            while (x >= y)
            {
                _paintable.SetPixelColor(centerX + x, centerY + y);
                _paintable.SetPixelColor(centerX - x, centerY + y);
                _paintable.SetPixelColor(centerX + x, centerX - y);
                _paintable.SetPixelColor(centerX - x, centerX - y);
                _paintable.SetPixelColor(centerX + y, centerX + x);
                _paintable.SetPixelColor(centerX - y, centerX + x);
                _paintable.SetPixelColor(centerX + y, centerX - x);
                _paintable.SetPixelColor(centerX - y, centerX - x);

                y++;
                if (d < 0)
                {
                    d = d + 2 * y + 1;
                }
                else
                {
                    x--;
                    d = d + 2 * (y - x) + 1;
                }

            }
        }

        public int GetActualX()
        {
            return _paintable.XWalleePosition;
        }

        public int GetActualY()
        {
            return _paintable.YWalleePosition;
        }

        public int GetCanvasSize()
        {
            return _paintable.gridSize;
        }

        public int GetColorCount(string color, int x1, int y1, int x2, int y2)
        {
            if (!_paintable.IsValidPosition(x1, y1) || !_paintable.IsValidPosition(x2, y2))
                return 0;
            Brush actualColor = GetColor(color);
            int count = 0;
            int minX = Math.Min(x1, x2);
            int minY = Math.Min(y1, y2);
            int maxX = Math.Max(x1, x2);
            int maxY = Math.Max(y1, y2);

            for (int i = minX; i < maxX; i++)
            {
                for (int k = minY; k < maxY; k++)
                {
                    if (_paintable.RectangMatrix![i, k].Fill == actualColor)
                        count += 1;
                }
            }
            return count;
        }

        private Brush GetColor(string color)
        {
            switch (color)
            {
                case "Blue":
                    {
                        return Brushes.Blue;
                    }
                case "Red":
                    {
                        return Brushes.Red;
                    }
                case "Green":
                    {
                        return Brushes.Green;
                    }
                case "Yelow":
                    {
                        return Brushes.Yellow;
                    }
                case "Orange":
                    {
                        return Brushes.Orange;
                    }
                case "Purple":
                    {
                        return Brushes.Purple;
                    }
                case "Black":
                    {
                        return Brushes.Black;
                    }
                case "White":
                    {
                        return Brushes.White; ;
                    }
                case "Transparent":
                    {
                        return Brushes.Transparent;
                    }
                default:
                    throw new Exception("El color del pincel no es valido");

            }
        }

        public int IsBrushColor(string color)
        {
            var colorBrush = GetColor(color);
            if (_paintable.PixelColor == colorBrush)
                return 1;
            return 0;
        }

        public int IsBrushSize(int size)
        {
            if (_paintable.BrushSize == size) return 1;
            return 0;
        }

        public int IsCanvasColor(string color, int vertical, int horizontal)
        {
            var colorBrush = GetColor(color);
            var currentX = _paintable.XWalleePosition + horizontal;
            var currentY = _paintable.XWalleePosition + vertical;

            if (!_paintable.IsValidPosition(currentX, currentY))
                return 0;
            if (_paintable.RectangMatrix![currentX, currentY].Fill == colorBrush) return 1;
            return 0;

        }


    }
}
