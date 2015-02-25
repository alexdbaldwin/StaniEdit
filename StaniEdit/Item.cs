using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StaniEdit
{
    class Item : DraggableGridSnapper
    {
        public Item() {
            tileWidth = 1;
            tileHeight = 1;
            color = new SolidColorBrush(Colors.Red);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Item(int width, int height)
        {
            tileWidth = width;
            tileHeight = height;
            color = new SolidColorBrush(Colors.Red);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

    }
}
