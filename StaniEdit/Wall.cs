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
            realWidth = 400.0;
            realHeight = 20.0;
            color = new SolidColorBrush(Colors.Orange);
            rect.Fill = color;
            zIndex = 3;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Wall(double width, double height)
        {
            realWidth = width;
            realHeight = height;
            color = new SolidColorBrush(Colors.Orange);
            rect.Fill = color;
            zIndex = 3;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

    }
}
