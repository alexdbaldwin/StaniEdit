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
            realWidth = 100.0;
            realHeight = 100.0;
            color = new SolidColorBrush(Colors.Teal);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Guard(double width, double height)
        {
            realWidth = width;
            realHeight = height;
            color = new SolidColorBrush(Colors.Teal);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

    }
}
