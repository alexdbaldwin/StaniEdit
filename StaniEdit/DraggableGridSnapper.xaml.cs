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

        public double originX = 0.0;
        public double originY = 0.0;

        public double WorldOriginX {
            get {
                GeneralTransform t = RenderTransform;
                Point transformedOrigin = t.Transform(new Point(originX*mainWindow.widthRatio, originY*mainWindow.heightRatio));
                return RealX + transformedOrigin.X / mainWindow.widthRatio;
            }
            set {
                GeneralTransform t = RenderTransform;
                Point transformedOrigin = t.Transform(new Point(originX * mainWindow.widthRatio, originY * mainWindow.heightRatio));
                RealX = value - transformedOrigin.X / mainWindow.widthRatio;
            }
        }

        public double WorldOriginY
        {
            get
            {
                GeneralTransform t = RenderTransform;
                Point transformedOrigin = t.Transform(new Point(originX * mainWindow.widthRatio, originY * mainWindow.heightRatio));
                return RealY + transformedOrigin.Y / mainWindow.heightRatio;
            }
            set
            {
                GeneralTransform t = RenderTransform;
                Point transformedOrigin = t.Transform(new Point(originX * mainWindow.widthRatio, originY * mainWindow.heightRatio));
                RealY = value - transformedOrigin.Y / mainWindow.heightRatio;
            }
        }

        protected int zIndex = 1;
        protected Brush color = new SolidColorBrush(Colors.Magenta);

        protected double spawnChance = 1.0;
        public double Angle {
            get { return (RenderTransform as RotateTransform).Angle; }
            set { (RenderTransform as RotateTransform).Angle = value; }
        }

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

        public virtual void Init(MainWindow main) {
            mainWindow = main;
            Width = main.widthRatio * realWidth;
            Height = main.heightRatio * realHeight;
            (RenderTransform as RotateTransform).CenterX = originX * main.widthRatio;
            (RenderTransform as RotateTransform).CenterY = originY * main.heightRatio;
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

                GeneralTransform t = RenderTransform;
                Point first = t.Transform(e.GetPosition(this));

                FirstXPos = first.X;// e.GetPosition(this).X;
                FirstYPos = first.Y;// e.GetPosition(this).Y;
                FirstArrowXPos = e.GetPosition(Parent as Control).X - FirstXPos;
                FirstArrowYPos = e.GetPosition(Parent as Control).Y - FirstYPos;
                e.Handled = true;

            }

        }


        public void OnMouseMove_(MouseEventArgs e) {
            OnMouseMove(e);
        }

        public void SafeDelete() {
            dragging = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            mainWindow.lblInfo.Content = "";
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this is Mesh)
            {
                mainWindow.lblInfo.Content = ((Mesh)this).MeshType;
                mainWindow.lblInfo.SetValue(Canvas.LeftProperty,e.GetPosition(Parent as FrameworkElement).X + 20);
                mainWindow.lblInfo.SetValue(Canvas.TopProperty, e.GetPosition(Parent as FrameworkElement).Y + 20);
            }

            if (dragging)
            {

                GeneralTransform t = RenderTransform;
                Rect transformedBounds = t.TransformBounds(new Rect(0, 0, Width, Height));

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

        public virtual void SnapToGrid() {

            

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
            GeneralTransform t = RenderTransform;
            Rect transformedBounds = t.TransformBounds(new Rect(0, 0, Width, Height));

            SetValue(Canvas.LeftProperty, Math.Round(((double)GetValue(Canvas.LeftProperty) + transformedBounds.X) / mainWindow.tileWidth) * mainWindow.tileWidth - transformedBounds.X);
            SetValue(Canvas.TopProperty, Math.Round(((double)GetValue(Canvas.TopProperty) + transformedBounds.Y) / mainWindow.tileHeight) * mainWindow.tileHeight - transformedBounds.Y);

            if ((double)GetValue(Canvas.LeftProperty) + transformedBounds.X < 0)
                SetValue(Canvas.LeftProperty, -transformedBounds.X);
            if ((double)GetValue(Canvas.LeftProperty) + transformedBounds.X + transformedBounds.Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - transformedBounds.Width - transformedBounds.X);
            if ((double)GetValue(Canvas.TopProperty) + transformedBounds.Y < 0)
                SetValue(Canvas.TopProperty, -transformedBounds.Y);
            if ((double)GetValue(Canvas.TopProperty) + transformedBounds.Y + transformedBounds.Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - transformedBounds.Height - transformedBounds.Y);
        }

        private void HorizontalLineSnap() {
            GeneralTransform t = RenderTransform;
            Rect transformedBounds = t.TransformBounds(new Rect(0, 0, Width, Height));

            SetValue(Canvas.LeftProperty, Math.Round(((double)GetValue(Canvas.LeftProperty) + transformedBounds.X) / mainWindow.tileWidth) * mainWindow.tileWidth - transformedBounds.X);
            SetValue(Canvas.TopProperty, Math.Round(((double)GetValue(Canvas.TopProperty) + transformedBounds.Y) / mainWindow.tileHeight) * mainWindow.tileHeight - (transformedBounds.Bottom - transformedBounds.Top));


            if ((double)GetValue(Canvas.LeftProperty) + transformedBounds.X < 0)
                SetValue(Canvas.LeftProperty, -transformedBounds.X);
            if ((double)GetValue(Canvas.LeftProperty) + transformedBounds.X + transformedBounds.Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - transformedBounds.Width - transformedBounds.X);
            if ((double)GetValue(Canvas.TopProperty) + transformedBounds.Y - transformedBounds.Height < 0)
                SetValue(Canvas.TopProperty, -(transformedBounds.Bottom - transformedBounds.Top));
            if ((double)GetValue(Canvas.TopProperty) + transformedBounds.Y + transformedBounds.Height + transformedBounds.Height> (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - (transformedBounds.Bottom - transformedBounds.Top));
        }

        private void VerticalLineSnap() {
            GeneralTransform t = RenderTransform;
            Rect transformedBounds = t.TransformBounds(new Rect(0, 0, Width, Height));

            SetValue(Canvas.LeftProperty, Math.Round(((double)GetValue(Canvas.LeftProperty) + transformedBounds.X) / mainWindow.tileWidth) * mainWindow.tileWidth - (transformedBounds.Left > transformedBounds.Right ? transformedBounds.Width : 0));
            SetValue(Canvas.TopProperty, Math.Round(((double)GetValue(Canvas.TopProperty) + transformedBounds.Y) / mainWindow.tileHeight) * mainWindow.tileHeight - transformedBounds.Y);

            if ((double)GetValue(Canvas.LeftProperty) + transformedBounds.X - transformedBounds.Width < 0)
                SetValue(Canvas.LeftProperty, -(transformedBounds.Left > transformedBounds.Right ? transformedBounds.Width : 0));
            if ((double)GetValue(Canvas.LeftProperty) + transformedBounds.X + transformedBounds.Width + transformedBounds.Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - (transformedBounds.Left > transformedBounds.Right ? transformedBounds.Width : 0));
            if ((double)GetValue(Canvas.TopProperty) + transformedBounds.Y < 0)
                SetValue(Canvas.TopProperty, -transformedBounds.Y);
            if ((double)GetValue(Canvas.TopProperty) + transformedBounds.Y + transformedBounds.Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - transformedBounds.Height - transformedBounds.Y);
        }

        private void CornerSnap() {
            //To do

            if ((double)GetValue(Canvas.LeftProperty) < 0)
                SetValue(Canvas.LeftProperty, 0.0);
            if ((double)GetValue(Canvas.LeftProperty) + rect.Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - rect.Width);
            if ((double)GetValue(Canvas.TopProperty) < 0)
                SetValue(Canvas.TopProperty, 0.0);
            if ((double)GetValue(Canvas.TopProperty) + rect.Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - rect.Height);
        }

        private void FreeSnap() {
            //Do not snap, just limit to grid bounds
            GeneralTransform t = RenderTransform;
            Rect transformedBounds = t.TransformBounds(new Rect(0, 0, Width, Height));

            if ((double)GetValue(Canvas.LeftProperty) + transformedBounds.X < 0)
                SetValue(Canvas.LeftProperty, -transformedBounds.X);
            if ((double)GetValue(Canvas.LeftProperty) + transformedBounds.X + transformedBounds.Width > (Parent as Canvas).Width)
                SetValue(Canvas.LeftProperty, (Parent as Canvas).Width - transformedBounds.Width - transformedBounds.X);
            if ((double)GetValue(Canvas.TopProperty) + transformedBounds.Y < 0)
                SetValue(Canvas.TopProperty, -transformedBounds.Y);
            if ((double)GetValue(Canvas.TopProperty) + transformedBounds.Y + transformedBounds.Height > (Parent as Canvas).Height)
                SetValue(Canvas.TopProperty, (Parent as Canvas).Height - transformedBounds.Height - transformedBounds.Y);
        }

       


    }
}
