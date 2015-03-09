using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace StaniEdit
{
    public class PatrolPoint : DraggableGridSnapper
    {
        ObservableCollection<PatrolPoint> patrolRoute = null;
        ComboBox cmbRoutes;

        public string MyText
        {
            get { return ToString(); }
        }

        public int PatrolRouteIndex
        {
            get { return mainWindow.patrolRoutes.IndexOf(patrolRoute); }
        }

        public int Index {
            get { return patrolRoute.IndexOf(this); }
        }

        public PatrolPoint() {
            realWidth = 33.0;
            realHeight = 33.0;
            snapMode = SnapMode.TileSnap;
            color = new SolidColorBrush(Colors.Purple);
            rect.Fill = color;
            zIndex = 2;
            SetValue(Canvas.ZIndexProperty, zIndex);
            Name = "Charles";
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
            
            cmbRoutes.SelectionChanged += routeChanged;

            if (main.patrolRoutes.Count == 0)
            {
                addRoute(this, new RoutedEventArgs());
            }
            else {
                cmbRoutes.SelectedIndex = 0;
            }
            Button b = new Button();
            b.Content = "New";
            b.Click += addRoute;
            stack.Children.Add(l);
            stack.Children.Add(cmbRoutes);
            stack.Children.Add(b);
            contextMenu.Items.Add(stack);

        }

        public override string ToString()
        {
            if(patrolRoute != null)
                return "Patrol point " + patrolRoute.IndexOf(this).ToString();
            return
                "oops";
        }

        private void addRoute(object sender, RoutedEventArgs e)
        {
            mainWindow.patrolRoutes.Add(new ObservableCollection<PatrolPoint>());
            mainWindow.patrolRouteIndices.Add(mainWindow.patrolRoutes.Count);
            cmbRoutes.SelectedIndex = mainWindow.patrolRoutes.Count - 1;
            //mbRoutes.ItemsSource = mainWindow.patrolRoutes;
        }

        private void routeChanged(object sender, SelectionChangedEventArgs e)
        {
            if(patrolRoute != null)
                patrolRoute.Remove(this);
            if (cmbRoutes.SelectedIndex != -1)
            {
                patrolRoute = mainWindow.patrolRoutes[cmbRoutes.SelectedIndex];
                mainWindow.patrolRoutes[cmbRoutes.SelectedIndex].Add(this);

                foreach (ObservableCollection<PatrolPoint> o in mainWindow.patrolRoutes)
                {
                    foreach (PatrolPoint p in o)
                    {
                        p.UpdateLabel();
                    }
                }
            }
            
        }

        public void UpdateLabel() {
            lblLabel.Content = patrolRoute.IndexOf(this);
            color = new SolidColorBrush(mainWindow.patrolRouteColors[mainWindow.patrolRoutes.IndexOf(patrolRoute)]);
            rect.Fill = color;
        }

        public void Delete() {
            patrolRoute.Remove(this);
            if (patrolRoute.Count == 0) {
                mainWindow.patrolRoutes.Remove(patrolRoute);
            }
            foreach (ObservableCollection<PatrolPoint> o in mainWindow.patrolRoutes)
            {
                foreach (PatrolPoint p in o)
                {
                    p.UpdateLabel();
                }
            }
        }

    }
}
