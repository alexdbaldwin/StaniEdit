﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int gridX = 36;
        public int gridY = 36;
        public double tileWidth, tileHeight;

        public double realWidth = 1200.0;
        public double realHeight = 1200.0;

        public double widthRatio = 1.0;
        public double heightRatio = 1.0;

        private List<DraggableGridSnapper> floorLayer = new List<DraggableGridSnapper>();
        private List<DraggableGridSnapper> wallLayer = new List<DraggableGridSnapper>();
        private List<DraggableGridSnapper> stuffLayer = new List<DraggableGridSnapper>();

        private DraggableGridSnapper dragging = null;
        private DraggableGridSnapper selected = null;

        private string currentFile = "";

        private DoorWall northDoor = null, eastDoor = null, southDoor = null, westDoor = null;

        public MainWindow()
        {
            InitializeComponent();

            RedrawGrid(3, 3);

            widthRatio = canvasRoom.Width / realWidth;
            heightRatio = canvasRoom.Height / realHeight;

        }

        private void RedrawGrid(int x, int y) {
            gridX = x;
            gridY = y;

            tileWidth = canvasRoom.Width / gridX;
            tileHeight = canvasRoom.Height / gridY; 

            for (int i = 0; i < canvasRoom.Children.Count; ++i) {
                if(canvasRoom.Children[i] is Line)
                    canvasRoom.Children.RemoveAt(i--);
            }

            for (int i = 0; i <= gridX; ++i)
            {
                Line l = new Line();
                canvasRoom.Children.Add(l);
                l.X1 = canvasRoom.Width / gridX * i;
                l.Y1 = 0;
                l.X2 = canvasRoom.Width / gridX * i;
                l.Y2 = canvasRoom.Height;
                l.Stroke = new SolidColorBrush(Colors.Black);
                l.SetValue(Canvas.ZIndexProperty, 20); //Draw on top of everything else
                l.IsHitTestVisible = false; //Prevent the grid lines from eating up mouse events

            }
            for (int i = 0; i <= gridY; ++i)
            {
                Line l = new Line();
                canvasRoom.Children.Add(l);
                l.X1 = 0;
                l.Y1 = canvasRoom.Height / gridY * i;
                l.X2 = canvasRoom.Width;
                l.Y2 = canvasRoom.Height / gridY * i;
                l.Stroke = new SolidColorBrush(Colors.Black);
                l.SetValue(Canvas.ZIndexProperty, 20); //Draw on top of everything else
                l.IsHitTestVisible = false; //Prevent the grid lines from eating up mouse events
            }

        }


        public void Select(DraggableGridSnapper control) {
            if (selected != null) {
                selected.Deselect();
            }
            selected = control;
            selected.Select();
            dragging = control;
        }

        public void EndDrag() {
            dragging = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (dragging != null)
            {
                //Check if the user released the mouse button while outside the window...
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    dragging.Deselect();
                    dragging = null;
                }
                else
                    ((DraggableGridSnapper)dragging).OnMouseMove_(e);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e){
            if (dragging != null)
            {
                ((DraggableGridSnapper)dragging).OnMouseUp_(e);
            }

            if (selected != null) {
                selected.Deselect();
                selected = null;
            }
        }

        private void btnFloor_Click(object sender, RoutedEventArgs e)
        {
            Floor f = new Floor();
            f.Init(this);
            canvasRoom.Children.Add(f);
            floorLayer.Add(f);
            if (!(bool)radFloor.IsChecked)
            {
                EnableFloorLayer();
                radFloor.IsChecked = true;
            }
            if (selected != null)
                selected.Deselect();
            selected = f;
            f.Select();
        }

        private void btnWallH_Click(object sender, RoutedEventArgs e)
        {
            Wall w = new Wall(400.0, 20.0);
            w.Init(this);
            canvasRoom.Children.Add(w);
            wallLayer.Add(w);
            if (!(bool)radWalls.IsChecked)
            {
                EnableWallsLayer();
                radWalls.IsChecked = true;
            }
            if (selected != null)
                selected.Deselect();
            selected = w;
            w.Select();
        }

        private void btnWallV_Click(object sender, RoutedEventArgs e)
        {
            Wall w = new Wall(20.0, 400.0);
            w.Init(this);
            canvasRoom.Children.Add(w);
            wallLayer.Add(w);
            if (!(bool)radWalls.IsChecked)
            {
                EnableWallsLayer();
                radWalls.IsChecked = true;
            }
            if(selected != null)
                selected.Deselect();
            selected = w;
            w.Select();
            

        }

        private void btnGuard_Click(object sender, RoutedEventArgs e) {
            Guard g = new Guard();
            g.Init(this);
            canvasRoom.Children.Add(g);
            stuffLayer.Add(g);
            if (!(bool)radStuff.IsChecked)
            {
                EnableStuffLayer();
                radStuff.IsChecked = true;
            }
            if (selected != null)
                selected.Deselect();
            selected = g;
            g.Select();
        }

        private void btnItem_Click(object sender, RoutedEventArgs e)
        {
            Item i = new Item();
            i.Init(this);
            canvasRoom.Children.Add(i);
            stuffLayer.Add(i);
            if (!(bool)radStuff.IsChecked)
            {
                EnableStuffLayer();
                radStuff.IsChecked = true;
            }
            if (selected != null)
                selected.Deselect();
            selected = i;
            i.Select();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            selected = null;
            dragging = null;
            canvasRoom.Children.Clear();
            floorLayer.Clear();
            wallLayer.Clear();
            stuffLayer.Clear();
            EnableFloorLayer();
            chkNorth.IsChecked = false;
            chkEast.IsChecked = false;
            chkSouth.IsChecked = false;
            chkWest.IsChecked = false;
            radFloor.IsChecked = true;
            cmbRoomType.SelectedIndex = 0;
            cmbRarity.SelectedIndex = 0;
        }

        private void radFloor_Checked(object sender, RoutedEventArgs e)
        {
            EnableFloorLayer();
        }

        private void EnableFloorLayer()
        {
            RedrawGrid(3, 3);
            if (selected != null)
            {
                selected.Deselect();
                selected = null;
            }
            foreach (DraggableGridSnapper control in floorLayer) control.Enable();
            foreach (DraggableGridSnapper control in wallLayer) control.Disable();
            foreach (DraggableGridSnapper control in stuffLayer) control.Disable();
        }

        private void radStuff_Checked(object sender, RoutedEventArgs e)
        {
            EnableStuffLayer();
        }

        private void EnableStuffLayer()
        {
            RedrawGrid(36, 36);
            if (selected != null)
            {
                selected.Deselect();
                selected = null;
            }
            foreach (DraggableGridSnapper control in floorLayer) control.Disable();
            foreach (DraggableGridSnapper control in wallLayer) control.Disable();
            foreach (DraggableGridSnapper control in stuffLayer) control.Enable();
        }

        private void radWalls_Checked(object sender, RoutedEventArgs e)
        {
            EnableWallsLayer();
        }

        private void EnableWallsLayer()
        {
            RedrawGrid(12, 12);
            if (selected != null)
            {
                selected.Deselect();
                selected = null;
            }
            foreach (DraggableGridSnapper control in floorLayer) control.Disable();
            foreach (DraggableGridSnapper control in wallLayer) control.Enable();
            foreach (DraggableGridSnapper control in stuffLayer) control.Disable();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (selected != null) {
                    floorLayer.Remove(selected);
                    wallLayer.Remove(selected);
                    stuffLayer.Remove(selected);
                    canvasRoom.Children.Remove(selected);
                    selected = null;
                }
            }
        }

        private void chkNorth_Checked(object sender, RoutedEventArgs e)
        {
            northDoor = new DoorWall(400.0, 20.0);
            northDoor.Init(this);
            canvasRoom.Children.Add(northDoor);
            northDoor.SetValue(Canvas.LeftProperty, 400.0 * widthRatio);
            northDoor.SetValue(Canvas.TopProperty, 0.0);
        }


        private void chkEast_Checked(object sender, RoutedEventArgs e)
        {
            eastDoor = new DoorWall(20.0, 400.0);
            eastDoor.Init(this);
            canvasRoom.Children.Add(eastDoor);
            eastDoor.SetValue(Canvas.LeftProperty, canvasRoom.Width - eastDoor.Width);
            eastDoor.SetValue(Canvas.TopProperty, 400.0 * heightRatio);
        }

        private void chkSouth_Checked(object sender, RoutedEventArgs e)
        {
            southDoor = new DoorWall(400.0, 20.0);
            southDoor.Init(this);
            canvasRoom.Children.Add(southDoor);
            southDoor.SetValue(Canvas.LeftProperty, 400.0 * widthRatio);
            southDoor.SetValue(Canvas.TopProperty, canvasRoom.Height - southDoor.Height);
        }

        private void chkWest_Checked(object sender, RoutedEventArgs e)
        {
            westDoor = new DoorWall(20.0, 400.0);
            westDoor.Init(this);
            canvasRoom.Children.Add(westDoor);
            westDoor.SetValue(Canvas.LeftProperty, 0.0);
            westDoor.SetValue(Canvas.TopProperty, 400.0 * heightRatio);
        }

        private void chkNorth_Unchecked(object sender, RoutedEventArgs e)
        {
            canvasRoom.Children.Remove(northDoor);
            northDoor = null;
        }

        private void chkEast_Unchecked(object sender, RoutedEventArgs e)
        {
            canvasRoom.Children.Remove(eastDoor);
            eastDoor = null;
        }

        private void chkSouth_Unchecked(object sender, RoutedEventArgs e)
        {
            canvasRoom.Children.Remove(southDoor);
            southDoor = null;
        }

        private void chkWest_Unchecked(object sender, RoutedEventArgs e)
        {
            canvasRoom.Children.Remove(westDoor);
            westDoor = null;
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentFile != "")
            {
                SaveRoom(currentFile);
            }
            else {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Room file (*.room)|*.room";
                if (sfd.ShowDialog() == true)
                {
                    SaveRoom(sfd.FileName);
                    currentFile = sfd.FileName;
                }
            } 
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Room file (*.room)|*.room";
            if (sfd.ShowDialog() == true)
            {
                SaveRoom(sfd.FileName);
                currentFile = sfd.FileName;
            }
        }

        private void menuLoad_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveRoom(string file) {

            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);

                List<MeshDefinition> meshes = new List<MeshDefinition>();
                meshes.Add(new MeshDefinition() { x = 100.0, y = 100.0, staticMesh = "floor"});
                RoomDefinition room = new RoomDefinition() { northDoor = false, eastDoor = true, southDoor = false, westDoor = true, meshes = meshes };

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RoomDefinition));
                serializer.WriteObject(fs, room);

                ////External doors:
                //if(northDoor != null) writer.Write(1); else writer.Write(0);
                //if(eastDoor != null) writer.Write(1); else writer.Write(0);
                //if(southDoor != null) writer.Write(1); else writer.Write(0);
                //if(westDoor != null) writer.Write(1); else writer.Write(0);
                //writer.Write("\n");

                ////Floor:
                //foreach(DraggableGridSnapper f in floorLayer){
                //    writer.Write(f.getLeftTile() + "," + f.getTopTile() + " ");
                //}
                //writer.Write("\n");

                ////Horizontal walls:
                //foreach (DraggableGridSnapper w in wallLayer) {
                //    Wall w_ = w as Wall;
                //    if (w_.Horizontal()) 
                //        writer.Write(w_.getLeftTile() + "," + w_.getTopTile() + " ");
                //}
                //writer.Write("\n");

                ////Vertical walls:
                //foreach (DraggableGridSnapper w in wallLayer)
                //{
                //    Wall w_ = w as Wall;
                //    if (!w_.Horizontal())
                //        writer.Write(w_.getLeftTile() + "," + w_.getTopTile() + " ");
                //}
                //writer.Write("\n");

                ////Items:
                //foreach (DraggableGridSnapper s in stuffLayer)
                //{
                //    if (s is Item) { 
                //         writer.Write(s.getLeftTile() + "," + s.getTopTile() + " ");
                //    }                       
                //}
                //writer.Write("\n");

                ////Guards:
                //foreach (DraggableGridSnapper s in stuffLayer)
                //{
                //    if (s is Guard)
                //    {
                //        writer.Write(s.getLeftTile() + "," + s.getTopTile() + " ");
                //    }
                //}
                //writer.Write("\n");
            
        }




    }
}
