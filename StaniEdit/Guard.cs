using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace StaniEdit
{
    class Guard : DraggableGridSnapper
    {

        ComboBox cmbRoutes;
        ComboBox cmbStart;

        public int PatrolRouteIndex {
            get { return cmbRoutes.SelectedIndex; }
        }

        public int StartIndex {
            get { return cmbStart.SelectedIndex; }
        }

        public Guard() {
            realWidth = 100.0;
            realHeight = 100.0;
            snapMode = SnapMode.TileSnap;
            color = new SolidColorBrush(Colors.Teal);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);

            
        }


        public Guard(double width, double height) : this()
        {
            realWidth = width;
            realHeight = height;
        }

        public override void Init(MainWindow main)
        {
            base.Init(main);

            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Label l = new Label();
            l.Content = "Patrol route:";
            cmbRoutes = new ComboBox();
            cmbRoutes.ItemsSource = main.patrolRouteIndices;
            
            stack.Children.Add(l);
            stack.Children.Add(cmbRoutes);
            contextMenu.Items.Add(stack);
            cmbRoutes.SelectionChanged += routeChanged;
            

            stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            l = new Label();
            l.Content = "Start point: ";
            cmbStart = new ComboBox();
            cmbStart.ItemsSource =null;

            DataTemplate dt = new DataTemplate();
            FrameworkElementFactory labelFactory = new FrameworkElementFactory(typeof(Label));
            labelFactory.SetBinding(Label.ContentProperty, new Binding("MyText"));
            //labelFactory.SetValue(Label.ContentProperty, "Hello");

            dt.VisualTree = labelFactory;
            cmbStart.ItemTemplate = dt;

            cmbStart.SelectedIndex = 0;
            stack.Children.Add(l);
            stack.Children.Add(cmbStart);
            contextMenu.Items.Add(stack);

            cmbRoutes.SelectedIndex = -1;

        }

        private void routeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainWindow.patrolRoutes.Count != 0 && cmbRoutes.SelectedIndex != -1)
            {
                cmbStart.ItemsSource = mainWindow.patrolRoutes[cmbRoutes.SelectedIndex];
                cmbStart.SelectedIndex = 0;
            }
        }

    }
}
