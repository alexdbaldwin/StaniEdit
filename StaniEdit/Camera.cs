using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StaniEdit
{
    class Camera : DraggableGridSnapper
    {

        public Camera() {
            realWidth = 33;
            realHeight = 33;
            originX = 0;
            originY = realHeight / 2;
            snapMode = SnapMode.TileSnap;
            color = new SolidColorBrush(Colors.Yellow);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }


    }
}
