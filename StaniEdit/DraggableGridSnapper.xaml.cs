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
    /// Interaction logic for Floor.xaml
    /// </summary>
    public partial class DraggableGridSnapper : UserControl
    {
        public enum SnapMode { TileSnap, HorizontalLineSnap, VerticalLineSnap, CornerSnap, Free };
        public SnapMode snapMode = SnapMode.TileSnap;

        protected bool dragging = false;
        protected double FirstXPos, FirstYPos, FirstArrowXPos, FirstArrowYPos;
        protected MainWindow mainWindow;

        protected double realWidth = 400.0;
        protected double realHeight = 400.0;

        protected int zIndex = 1;
        protected Brush color = new SolidColorBrush(Colors.Magenta);

        protected double spawnChance = 1.0;
        protected string meshType = "default";

        public DraggableGridSnapper()
        {
            InitializeComponent();
        }

        public double RealX
        {
            get { return (double)GetValue(Canvas.LeftProperty) / mainWindow.widthRatio; }
            set { SetValue(Canvas.LeftProperty, value * mainWindow.widthRatio); }
        }
        public double RealY
        {
            get { return (double)GetValue(Canvas.TopProperty) / mainWindow.heightRatio; }
            set { SetValue(Canvas.TopProperty, value * mainWindow.heightRatio); }
        }
        public string MeshType {
            get { return meshType; }
        }

        public virtual void Init(MainWindow main) {
            mainWindow = main;
            Width = main.widthRatio * realWidth;
            Height = main.heightRatio * realHeight;
            SetValue(Canvas.LeftProperty, 0.0);
            SetValue(Canvas.TopProperty, 0.0);
        }

        public void Select() {
            rect.Stroke = new SolidColorBrush(Colors.Blue);
            SetValue(Canvas.ZIndexProperty, 10);
        }

        public void Deselect() {
            dragging = false;
            rect.Stroke = new SolidColorBrush(Colors.Transparent);
            SetValue(Canvas.ZIndexProperty, zIndex);
        }

        public void Enable() {
            IsEnabled = true;
            rect.Fill = color;
            this.IsHitTestVisible = true;
        }

        public void Disable() {
            IsEnabled = false;
            rect.Fill = new SolidColorBrush(Color.FromArgb(40,0,0,0));
            this.IsHitTestVisible = false;
        }
        

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mainWindow.Select(this);
                dragging = true;

                FirstXPos = e.GetPosition(this).X;
                FirstYPos = e.GetPosition(this).Y;
                FirstArrowXPos = e.GetPosition(Parent as Control).X - FirstXPos;
                FirstArrowYPos = e.GetPosition(Parent as Control).Y - FirstYPos;
                e.Handled = true;

            }

        }

        public void OnMouseMove_(MouseEventArgs e) {
            OnMouseMove(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            

            if (dragging)
            {

                SetValue(Canvas.LeftProperty,
                     e.GetPosition(Parent as FrameworkElement).X - FirstXPos);

                SetValue(Canvas.TopProperty,
                     e.GetPosition(Parent as FrameworkElement).Y - FirstYPos);

                SnapToGrid();
                e.Handled = true;
            }

            
        }

        public void OnMouseUp_(MouseButtonEventArgs e) {
            OnMouseUp(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (dragging)
            {
                mainWindow.EndDrag();
                dragging = false;
                e.Handled = true;
            }
            
        }

        protected virtual void SnapToGrid() {

            

            switch (snapMode) { 
                case SnapMode.TileSnap:
                    TileSnap();
                    break;
                case SnapMode.HorizontalLineSnap:
                    HorizontalLineSnap();
                    break;
                case SnapMode.VerticalLineSnap:
                    VerticalLineSnap();
                    break;
                case SnapMode.CornerSnap:
                    CornerSnap();
                    break;
                case SnapMode.Free:
                    FreeSnap();
                    break;
            }


            

        }

        private void TileSnap() {
            SetValue(Canvas.LeftProperty, (int)((double)GetValue(Canvas.LeftProperty) / mainWindow.tileWidth) * mainWindow.tileWidth);
            SetValue(Canvas.TopProperty, (int)((double)GetValue(Canvas.TopProperty) / mainWindow.tileHeight) * mainWindow.tileHeight);

            if ((double)GetValue(Canvas.LeftProperty) < 0)
                SetValue(Canvas.LeftProperty, 0.0);
            if ((double)GetValue(Canvas.LeftProperty) + Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - Width);
            if ((double)GetValue(Canvas.TopProperty) < 0)
                SetValue(Canvas.TopProperty, 0.0);
            if ((double)GetValue(Canvas.TopProperty) + Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - Height);
        }

        private void HorizontalLineSnap() {
            SetValue(Canvas.LeftProperty, (int)((double)GetValue(Canvas.LeftProperty) / mainWindow.tileWidth) * mainWindow.tileWidth);
            SetValue(Canvas.TopProperty, Math.Round((double)GetValue(Canvas.TopProperty) / mainWindow.tileHeight) * mainWindow.tileHeight - Height/2);

            if ((double)GetValue(Canvas.LeftProperty) < 0)
                SetValue(Canvas.LeftProperty, 0.0);
            if ((double)GetValue(Canvas.LeftProperty) + Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - Width);
            if ((double)GetValue(Canvas.TopProperty) < 0)
                SetValue(Canvas.TopProperty, 0.0);
            if ((double)GetValue(Canvas.TopProperty) + Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - Height);
        }

        private void VerticalLineSnap() {

            SetValue(Canvas.LeftProperty, Math.Round((double)GetValue(Canvas.LeftProperty) / mainWindow.tileWidth) * mainWindow.tileWidth - Width/2);
            SetValue(Canvas.TopProperty, (int)((double)GetValue(Canvas.TopProperty) / mainWindow.tileHeight) * mainWindow.tileHeight);

            if ((double)GetValue(Canvas.LeftProperty) < 0)
                SetValue(Canvas.LeftProperty, 0.0);
            if ((double)GetValue(Canvas.LeftProperty) + Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - Width);
            if ((double)GetValue(Canvas.TopProperty) < 0)
                SetValue(Canvas.TopProperty, 0.0);
            if ((double)GetValue(Canvas.TopProperty) + Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - Height);
        }

        private void CornerSnap() {
            //To do

            if ((double)GetValue(Canvas.LeftProperty) < 0)
                SetValue(Canvas.LeftProperty, 0.0);
            if ((double)GetValue(Canvas.LeftProperty) + Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - Width);
            if ((double)GetValue(Canvas.TopProperty) < 0)
                SetValue(Canvas.TopProperty, 0.0);
            if ((double)GetValue(Canvas.TopProperty) + Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - Height);
        }

        private void FreeSnap() {
            //Do not snap, just limit to grid bounds

            if ((double)GetValue(Canvas.LeftProperty) < 0)
                SetValue(Canvas.LeftProperty, 0.0);
            if ((double)GetValue(Canvas.LeftProperty) + Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - Width);
            if ((double)GetValue(Canvas.TopProperty) < 0)
                SetValue(Canvas.TopProperty, 0.0);
            if ((double)GetValue(Canvas.TopProperty) + Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - Height);
        }

       


    }
}
