using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StaniEdit
{
    class Floor : DraggableGridSnapper
    {

        public Floor() {
            tileWidth = 4;
            tileHeight = 4;
            color = new SolidColorBrush(Colors.SkyBlue);
            rect.Fill = color;
            zIndex = 0;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Floor(int width, int height) {
            tileWidth = width;
            tileHeight = height;
            color = new SolidColorBrush(Colors.SkyBlue);
            rect.Fill = color;
            zIndex = 0;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }
    }
}
