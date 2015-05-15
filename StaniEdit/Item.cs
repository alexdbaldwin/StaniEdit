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
            realWidth = 100.0;
            realHeight = 100.0;
            snapMode = SnapMode.TileSnap;
            color = new SolidColorBrush(Colors.Red);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public Item(double width, double height) : this()
        {
            realWidth = width;
            realHeight = height;
        }

        //public override DraggableGridSnapper Clone(MainWindow main){
        //    Item i = new Item(this.Width, this.Height);
        //    i.Init(main);
        //    main.canvasRoom.Children.Add(i);
        //    main.stuffLayer.Add(i);
        //    if (!(bool)main.radStuff.IsChecked)
        //    {
        //        main.EnableStuffLayer();
        //        main.radStuff.IsChecked = true;
        //    }
        //    return i;
        //}

    }
}
