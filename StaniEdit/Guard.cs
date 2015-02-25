using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StaniEdit
{
    class Guard : DraggableGridSnapper
    {

        public Guard() {
            tileWidth = 1;
            tileHeight = 1;
            color = new SolidColorBrush(Colors.Teal);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Guard(int width, int height)
        {
            tileWidth = width;
            tileHeight = height;
            color = new SolidColorBrush(Colors.Teal);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

    }
}
