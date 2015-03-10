using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StaniEdit
{
    static class MeshFactory
    {

        public static Mesh MakeFloor(MainWindow main) {
            Mesh m = new Mesh("floor", 400.0, 400.0, 0.0, 0.0, 0.0, DraggableGridSnapper.SnapMode.TileSnap, Colors.SkyBlue, 0);
            m.Init(main);
            return m;
        }

        public static Mesh MakeVerticalWall(MainWindow main)
        {
            Mesh m = new Mesh("wall", 400.0, 20.0, 0.0, 10.0, 90.0, DraggableGridSnapper.SnapMode.VerticalLineSnap, Colors.Orange, 3);
            m.Init(main);
            return m;
        }

        public static Mesh MakeHorizontalWall(MainWindow main)
        {
            Mesh m = new Mesh("wall", 400.0, 20.0, 0.0, 10.0, 0.0, DraggableGridSnapper.SnapMode.HorizontalLineSnap, Colors.Orange, 3);
            m.Init(main);
            return m;
        }

        public static Mesh MakeVerticalDoorWall(MainWindow main)
        {
            Mesh m = new Mesh("wall", 400.0, 20.0, 0.0, 10.0, 90.0, DraggableGridSnapper.SnapMode.VerticalLineSnap, Colors.Black, 4);
            m.Init(main);
            m.IsEnabled = false;
            return m;
        }

        public static Mesh MakeHorizontalDoorWall(MainWindow main)
        {
            Mesh m = new Mesh("wall", 400.0, 20.0, 0.0, 10.0, 0.0, DraggableGridSnapper.SnapMode.HorizontalLineSnap, Colors.Black, 4);
            m.Init(main);
            m.IsEnabled = false;
            return m;
        }

    }
}
