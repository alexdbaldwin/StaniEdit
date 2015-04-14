using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StaniEdit
{
    class Light : DraggableGridSnapper
    {

        private Slider slider;
        private Label lblRadius;
        private double radius;

        private double widthRatio = 1.0;
        private double heightRatio = 1.0;

        private Ellipse ellipse;

        public double Radius {
            get { return radius; }
            set {
                radius = value;
                slider.Value = radius;

                lblRadius.Content = radius;

                ellipse.Width = radius * 2 * widthRatio;
                ellipse.Height = radius * 2 * heightRatio;
                Canvas.SetLeft(ellipse, -radius * widthRatio);
                Canvas.SetTop(ellipse, -radius * heightRatio);
            }
        }

        public Light() {

            realWidth = 50.0;
            realHeight = 50.0;
            snapMode = SnapMode.Free;
            color = new SolidColorBrush(Colors.Yellow);
            rect.Fill = color;
            zIndex = 6;
            SetValue(Canvas.ZIndexProperty, zIndex);

            
            radius = 100;
        }

        public void Init(MainWindow main) {
            base.Init(main);

            widthRatio = main.widthRatio;
            heightRatio = main.heightRatio;

            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Label l = new Label();
            l.Content = "Radius:";

            slider = new Slider();
            slider.Value = radius;
            slider.Minimum = 50;
            slider.Maximum = 1000;
            slider.Width = 100;

            lblRadius = new Label();
            lblRadius.Content = radius;
            

            stack.Children.Add(l);
            stack.Children.Add(slider);
            stack.Children.Add(lblRadius);
            contextMenu.Items.Add(stack);
            slider.ValueChanged += sliderChanged;

            ellipse = new Ellipse();
            ellipse.Width = radius * 2 * widthRatio;
            ellipse.Height = radius * 2 * heightRatio;
            ellipse.Fill = Brushes.SaddleBrown;
            Canvas.SetLeft(ellipse, -radius * widthRatio);
            Canvas.SetTop(ellipse, -radius * heightRatio);
            Canvas.SetZIndex(ellipse, 1);
            ellipse.Opacity = 0.3;

            canvasHolder.Children.Add(ellipse);
        }


        private void sliderChanged(object sender, RoutedEventArgs e)
        {
            radius = slider.Value;

            lblRadius.Content = radius;

            ellipse.Width = radius * 2 * widthRatio;
            ellipse.Height = radius * 2 * heightRatio;
            Canvas.SetLeft(ellipse, -radius * widthRatio);
            Canvas.SetTop(ellipse, -radius * heightRatio);
        }



    }
}
