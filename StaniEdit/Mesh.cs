using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StaniEdit
{
    class Mesh : DraggableGridSnapper
    {

        protected string meshType = "default";
        public string MeshType {
            get { return meshType; }
        }

        public Mesh(string meshType, double width, double height, double originX, double originY, double angle, SnapMode snapMode, Color color, int zIndex) {
            realWidth = width;
            realHeight = height;
            this.snapMode = snapMode;
            this.meshType = meshType;
            this.color = new SolidColorBrush(color);
            rect.Fill = this.color;
            this.zIndex = zIndex;
            SetValue(Canvas.ZIndexProperty, this.zIndex);
            Angle = angle;
            this.originX = originX;
            this.originY = originY;
        }

        public override DraggableGridSnapper Clone(MainWindow main)
        {
            Mesh res = new Mesh(meshType, realWidth, realHeight, originX, originY, Angle, snapMode, Colors.Magenta, zIndex);
            res.Init(main);
            main.canvasRoom.Children.Add(res);
            main.stuffLayer.Add(res);
            if (!(bool)main.radStuff.IsChecked)
            {
                main.EnableStuffLayer();
                main.radStuff.IsChecked = true;
            }
            return res;
        }
    }
}
