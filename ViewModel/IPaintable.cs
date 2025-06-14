using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ViewModel
{
    public interface IPaintable
    {
        public int XWalleePosition { get; set; }
        public int YWalleePosition { get; set; }
        void SetPixelColor(int x, int y);
        void PaintWallee(int x, int y);
        Brush PixelColor { get; set; }
        int BrushSize { get; set; }
        public (int, int)[] Directions { get; }
        public bool IsValidPosition(int x, int y);
        public Rectangle[,]? RectangMatrix { get; }
        public int gridSize { get; }
        public bool OnlyOneSpawn {get; set;}
       
    }
}
