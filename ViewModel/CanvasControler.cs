using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace ViewModel
{
    public class GridSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int cellSize;
        public required int CellSize
        {
            get => cellSize;
            set
            {

                if (cellSize != value)
                {
                    cellSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellSize)));
                }
            }
        }
        public int GridSize { get; set; }
        

    }
}