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
            realWidth = 400.0;
            realHeight = 400.0;
            snapMode = SnapMode.TileSnap;
            meshType = "floor";
            color = new SolidColorBrush(Colors.SkyBlue);
            rect.Fill = color;
            zIndex = 0;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Floor(double width, double height) : this(){
            realWidth = width;
            realHeight = width;

        }
    }
}
