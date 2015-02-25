using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StaniEdit
{
    /// <summary>
    /// Interaction logic for DraggableGridLineSnapper.xaml
    /// </summary>
    public class DraggableGridLineSnapper : DraggableGridSnapper
    {
        protected bool horizontal = false;

        public DraggableGridLineSnapper()
        {
            InitializeComponent();
        }

        public override void Init(MainWindow main)
        {
            if (tileWidth > 1)
            {
                horizontal = true;
            }
            mainWindow = main;

            if (horizontal)
            {
                Width = tileWidth * main.tileWidth;
                Height = main.tileHeight / 2;
            }
            else
            {
                Width = main.tileWidth / 2;
                Height = tileHeight * main.tileHeight;
            }
        }

        public bool Horizontal() {
            return horizontal;
        }

        protected override void SnapToGrid()
        {
            if (horizontal)
            {
                SetValue(Canvas.LeftProperty, (int)((double)GetValue(Canvas.LeftProperty) / mainWindow.tileWidth) * mainWindow.tileWidth);

                bool snapLeft = true;
                if (Math.Round((double)GetValue(Canvas.TopProperty) / mainWindow.tileHeight) < (double)GetValue(Canvas.TopProperty) / mainWindow.tileHeight)
                    snapLeft = false;

                SetValue(Canvas.TopProperty, Math.Round((double)GetValue(Canvas.TopProperty) / mainWindow.tileHeight) * mainWindow.tileHeight - (snapLeft?Height:0));
            }
            else
            {
                bool snapLeft = true;
                if (Math.Round((double)GetValue(Canvas.LeftProperty) / mainWindow.tileWidth) < (double)GetValue(Canvas.LeftProperty) / mainWindow.tileWidth)
                    snapLeft = false;

                SetValue(Canvas.LeftProperty, Math.Round((double)GetValue(Canvas.LeftProperty) / mainWindow.tileWidth) * mainWindow.tileWidth -(snapLeft?Width:0));
                SetValue(Canvas.TopProperty, (int)((double)GetValue(Canvas.TopProperty) / mainWindow.tileHeight) * mainWindow.tileHeight);
            }



            if ((double)GetValue(Canvas.LeftProperty) < 0)
                SetValue(Canvas.LeftProperty, 0.0);
            if ((double)GetValue(Canvas.LeftProperty) + Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - Width);
            if ((double)GetValue(Canvas.TopProperty) < 0)
                SetValue(Canvas.TopProperty, 0.0);
            if ((double)GetValue(Canvas.TopProperty) + Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - Height);

        }

        public override int getTopTile()
        {
            return (int)Math.Round((double)GetValue(Canvas.TopProperty) / mainWindow.tileHeight);
        }

        public override int getLeftTile()
        {
            return (int)Math.Round((double)GetValue(Canvas.LeftProperty) / mainWindow.tileWidth);
        }

         
    }
}
