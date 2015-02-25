using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StaniEdit
{
    class Wall : DraggableGridLineSnapper
    {

        public Wall() {
            tileWidth = 4;
            tileHeight = 1;
            color = new SolidColorBrush(Colors.Orange);
            rect.Fill = color;
            zIndex = 3;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Wall(int width, int height)
        {
            tileWidth = width;
            tileHeight = height;
            color = new SolidColorBrush(Colors.Orange);
            rect.Fill = color;
            zIndex = 3;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

    }
}
