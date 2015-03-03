using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StaniEdit
{
    class DoorWall : Wall
    {

        public DoorWall(double width, double height) : base(width,height){
            IsEnabled = false;
            color = new SolidColorBrush(Colors.Black);
            rect.Fill = color;
            zIndex = 4;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

    }
}
