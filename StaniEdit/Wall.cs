using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StaniEdit
{
    class Wall : DraggableGridSnapper
    {

        public Wall() {
            realWidth = 400.0;
            realHeight = 20.0;
            meshType = "hWall";
            snapMode = SnapMode.HorizontalLineSnap;
            color = new SolidColorBrush(Colors.Orange);
            rect.Fill = color;
            zIndex = 3;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Wall(double width, double height) : this()
        {
            realWidth = width;
            realHeight = height;
            if (width < height)
            {
                meshType = "vWall";
                snapMode = SnapMode.VerticalLineSnap;
            }

        }

    }
}
