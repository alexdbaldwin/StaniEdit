﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public Label lblInfo;

        public int gridX = 36;
        public int gridY = 36;
        public double tileWidth, tileHeight;

        public double realWidth = 2000.0;
        public double realHeight = 2000.0;

        public double widthRatio = 1.0;
        public double heightRatio = 1.0;

        private List<DraggableGridSnapper> floorLayer = new List<DraggableGridSnapper>();
        private List<DraggableGridSnapper> wallLayer = new List<DraggableGridSnapper>();
        private List<DraggableGridSnapper> stuffLayer = new List<DraggableGridSnapper>();
        private List<DraggableGridSnapper> lightsLayer = new List<DraggableGridSnapper>();

        public ObservableCollection<ObservableCollection<PatrolPoint>> patrolRoutes = new ObservableCollection<ObservableCollection<PatrolPoint>>();
        public ObservableCollection<int> patrolRouteIndices = new ObservableCollection<int>();

        public List<Color> patrolRouteColors = new List<Color>() { Colors.Red, Colors.Yellow, Colors.Blue, Colors.Brown, Colors.Chartreuse, Colors.Fuchsia, Colors.Gainsboro, Colors.Firebrick };

        private DraggableGridSnapper dragging = null;
        private DraggableGridSnapper selected = null;

        private string currentFile = "";

        private Mesh northDoor = null, eastDoor = null, southDoor = null, westDoor = null;

        public MainWindow()
        {
            InitializeComponent();

            RedrawGrid(5, 5);

            widthRatio = canvasRoom.Width / realWidth;
            heightRatio = canvasRoom.Height / realHeight;

            LoadCustomObjects();

            lblInfo = new Label();
            lblInfo.Content = "";
            
            canvasRoom.Children.Add(lblInfo);

            lblInfo.SetValue(Canvas.ZIndexProperty, 50);

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
            Mesh f = MeshFactory.MakeFloor(this);
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
            f.SnapToGrid();
        }

        private void btnWallH_Click(object sender, RoutedEventArgs e)
        {
            Mesh w;
            switch (cmbHWalls.SelectedIndex) { 
                case 0:
                    w = MeshFactory.MakeHorizontalWall100(this);
                    break;
                case 1:
                    w = MeshFactory.MakeHorizontalWall200(this);
                    break;
                case 2:
                    w = MeshFactory.MakeHorizontalWall400(this);
                    break;
                default:
                    w = MeshFactory.MakeHorizontalWall100(this);
                    break;
            }
             
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
            w.SnapToGrid();
        }

        private void btnWallV_Click(object sender, RoutedEventArgs e)
        {
            Mesh w;
            switch (cmbVWalls.SelectedIndex)
            {
                case 0:
                    w = MeshFactory.MakeVerticalWall100(this);
                    break;
                case 1:
                    w = MeshFactory.MakeVerticalWall200(this);
                    break;
                case 2:
                    w = MeshFactory.MakeVerticalWall400(this);
                    break;
                default:
                    w = MeshFactory.MakeVerticalWall100(this);
                    break;
            }
          
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
            w.SnapToGrid();

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
            g.SnapToGrid();
        }

        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera g = new Camera();
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
            g.SnapToGrid();
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
            i.SnapToGrid();
        }

        private void btnPatrolPoint_Click(object sender, RoutedEventArgs e)
        {
            PatrolPoint pp = new PatrolPoint();
            pp.Init(this);
            canvasRoom.Children.Add(pp);
            stuffLayer.Add(pp);
            if (!(bool)radStuff.IsChecked)
            {
                EnableStuffLayer();
                radStuff.IsChecked = true;
            }
            if (selected != null)
                selected.Deselect();
            selected = pp;
            pp.Select();
            pp.SnapToGrid();
        }

        private void btnLight_Click(object sender, RoutedEventArgs e)
        {
            Light l = new Light();
            l.Init(this);
            canvasRoom.Children.Add(l);
            lightsLayer.Add(l);
            if (!(bool)radLights.IsChecked)
            {
                EnableLightsLayer();
                radLights.IsChecked = true;
            }
            if (selected != null)
                selected.Deselect();
            selected = l;
            l.Select();
            l.SnapToGrid();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            selected = null;
            dragging = null;
            canvasRoom.Children.Clear();
            floorLayer.Clear();
            wallLayer.Clear();
            stuffLayer.Clear();
            lightsLayer.Clear();
            EnableFloorLayer();
            chkNorth.IsChecked = false;
            chkEast.IsChecked = false;
            chkSouth.IsChecked = false;
            chkWest.IsChecked = false;
            radFloor.IsChecked = true;
            cmbRoomType.SelectedIndex = 0;
            cmbRarity.SelectedIndex = 0;
            patrolRoutes.Clear();
            patrolRouteIndices.Clear();
            canvasRoom.Children.Add(lblInfo);
        }

        private void radFloor_Checked(object sender, RoutedEventArgs e)
        {
            EnableFloorLayer();
        }

        private void EnableFloorLayer()
        {
            RedrawGrid(5, 5);
            if (selected != null)
            {
                selected.Deselect();
                selected = null;
            }
            foreach (DraggableGridSnapper control in floorLayer) control.Enable();
            foreach (DraggableGridSnapper control in wallLayer) control.Disable();
            foreach (DraggableGridSnapper control in stuffLayer) control.Disable();
            foreach (DraggableGridSnapper control in lightsLayer) control.Disable();
        }

        private void radStuff_Checked(object sender, RoutedEventArgs e)
        {
            EnableStuffLayer();
        }

        private void EnableStuffLayer()
        {
            RedrawGrid(60, 60);
            if (selected != null)
            {
                selected.Deselect();
                selected = null;
            }
            foreach (DraggableGridSnapper control in floorLayer) control.Disable();
            foreach (DraggableGridSnapper control in wallLayer) control.Disable();
            foreach (DraggableGridSnapper control in stuffLayer) control.Enable();
            foreach (DraggableGridSnapper control in lightsLayer) control.Disable();
        }

        private void radWalls_Checked(object sender, RoutedEventArgs e)
        {
            EnableWallsLayer();
        }

        private void EnableWallsLayer()
        {
            RedrawGrid(20, 20);
            if (selected != null)
            {
                selected.Deselect();
                selected = null;
            }
            foreach (DraggableGridSnapper control in floorLayer) control.Disable();
            foreach (DraggableGridSnapper control in wallLayer) control.Enable();
            foreach (DraggableGridSnapper control in stuffLayer) control.Disable();
            foreach (DraggableGridSnapper control in lightsLayer) control.Disable();
        }

        private void radLights_Checked(object sender, RoutedEventArgs e)
        {
            EnableLightsLayer();
        }

        private void EnableLightsLayer()
        {
            RedrawGrid(1, 1);
            if (selected != null)
            {
                selected.Deselect();
                selected = null;
            }
            foreach (DraggableGridSnapper control in floorLayer) control.Disable();
            foreach (DraggableGridSnapper control in wallLayer) control.Disable();
            foreach (DraggableGridSnapper control in stuffLayer) control.Disable();
            foreach (DraggableGridSnapper control in lightsLayer) control.Enable();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (selected != null) {
                    if (selected is PatrolPoint) {
                        (selected as PatrolPoint).Delete();
                    }

                    floorLayer.Remove(selected);
                    wallLayer.Remove(selected);
                    stuffLayer.Remove(selected);
                    lightsLayer.Remove(selected);
                    canvasRoom.Children.Remove(selected);
                    selected.SafeDelete();
                    selected = null;
                }
            }
            if (e.Key == Key.R)
            {
                if (selected != null) {
                    (selected.RenderTransform as RotateTransform).Angle += 90.0;
                    if (selected.snapMode == DraggableGridSnapper.SnapMode.HorizontalLineSnap)
                        selected.snapMode = DraggableGridSnapper.SnapMode.VerticalLineSnap;
                    else if (selected.snapMode == DraggableGridSnapper.SnapMode.VerticalLineSnap)
                        selected.snapMode = DraggableGridSnapper.SnapMode.HorizontalLineSnap;
                    selected.SnapToGrid();

                }
            }
        }

        private void chkNorth_Checked(object sender, RoutedEventArgs e)
        {
            northDoor = MeshFactory.MakeHorizontalDoorWall(this);
            canvasRoom.Children.Add(northDoor);
            northDoor.SetValue(Canvas.LeftProperty, 800.0 * widthRatio);
            northDoor.SetValue(Canvas.TopProperty, -20.0 * heightRatio);
        }


        private void chkEast_Checked(object sender, RoutedEventArgs e)
        {
            eastDoor = MeshFactory.MakeVerticalDoorWall(this);
            canvasRoom.Children.Add(eastDoor);
            eastDoor.SetValue(Canvas.LeftProperty, canvasRoom.Width);
            eastDoor.SetValue(Canvas.TopProperty, 800.0 * heightRatio);
        }

        private void chkSouth_Checked(object sender, RoutedEventArgs e)
        {
            southDoor = MeshFactory.MakeHorizontalDoorWall(this);
            canvasRoom.Children.Add(southDoor);
            southDoor.SetValue(Canvas.LeftProperty, 800.0 * widthRatio);
            southDoor.SetValue(Canvas.TopProperty, canvasRoom.Height);
        }

        private void chkWest_Checked(object sender, RoutedEventArgs e)
        {
            westDoor = MeshFactory.MakeVerticalDoorWall(this);
            canvasRoom.Children.Add(westDoor);
            westDoor.SetValue(Canvas.LeftProperty, -20.0 * widthRatio);
            westDoor.SetValue(Canvas.TopProperty, 800.0 * heightRatio);
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
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Room file (*.room)|*.room";
            if (ofd.ShowDialog() == true)
            {
                LoadRoom(ofd.FileName);
                currentFile = ofd.FileName;
            }

        }

        private void LoadRoom(string file)
        {
            Clear();

            window.Title = file;

            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RoomDefinition));
            RoomDefinition room = (RoomDefinition)serializer.ReadObject(fs);

            cmbRoomType.SelectedIndex = room.roomType;
            cmbRarity.SelectedIndex = room.roomRarity;
            cmbFloorType.SelectedIndex = room.roomFloor;


            if(room.northDoor) {
                //chkNorth_Checked(this, new RoutedEventArgs());
                chkNorth.IsChecked = true;
            }
            if (room.eastDoor)
            {
                //chkEast_Checked(this, new RoutedEventArgs());
                chkEast.IsChecked = true;
            }
            if (room.southDoor)
            {
                //chkSouth_Checked(this, new RoutedEventArgs());
                chkSouth.IsChecked = true;
            }
            if (room.westDoor)
            {
                //chkWest_Checked(this, new RoutedEventArgs());
                chkWest.IsChecked = true;
            }

            foreach (PatrolRouteDefinition prd in room.patrolRoutes) {
                ObservableCollection<PatrolPoint> pr = new ObservableCollection<PatrolPoint>();
                patrolRoutes.Add(pr);
                patrolRouteIndices.Add(patrolRoutes.Count);
                foreach (PatrolPointDefinition ppd in prd.patrolPoints) {
                    PatrolPoint pp = new PatrolPoint();
                    pp.Init(this);
                    canvasRoom.Children.Add(pp);
                    stuffLayer.Add(pp);
                    pp.WorldOriginX = ppd.x;
                    pp.WorldOriginY = ppd.y;
                    pp.cmbRoutes.SelectedIndex = patrolRoutes.Count - 1;
                }
            }


            foreach (SpawnGroupDefinition sgd in room.spawnGroups) {
                foreach (MeshDefinition md in sgd.meshes) {
                    if (md.staticMesh == "floor") {
                        Mesh f = MeshFactory.MakeFloor(this);
                        f.Init(this);
                        canvasRoom.Children.Add(f);
                        floorLayer.Add(f);
                        f.WorldOriginX = md.x;
                        f.WorldOriginY = md.y;
                        f.Angle = md.rotation;
                    }
                    else if (md.staticMesh == "wall400")
                    {
                        Mesh w = MeshFactory.MakeHorizontalWall400(this);
                        w.Init(this);
                        canvasRoom.Children.Add(w);
                        wallLayer.Add(w);
                        w.WorldOriginX = md.x;
                        w.WorldOriginY = md.y;
                        w.Angle = md.rotation;
                        if (w.Angle != 0.0)
                        {
                            w.snapMode = DraggableGridSnapper.SnapMode.VerticalLineSnap;

                        }
                    }
                    else if (md.staticMesh == "wall200")
                    {
                        Mesh w = MeshFactory.MakeHorizontalWall200(this);
                        w.Init(this);
                        canvasRoom.Children.Add(w);
                        wallLayer.Add(w);
                        w.WorldOriginX = md.x;
                        w.WorldOriginY = md.y;
                        w.Angle = md.rotation;
                        if (w.Angle != 0.0)
                        {
                            w.snapMode = DraggableGridSnapper.SnapMode.VerticalLineSnap;

                        }
                    }
                    else if (md.staticMesh == "wall100")
                    {
                        Mesh w = MeshFactory.MakeHorizontalWall100(this);
                        w.Init(this);
                        canvasRoom.Children.Add(w);
                        wallLayer.Add(w);
                        w.WorldOriginX = md.x;
                        w.WorldOriginY = md.y;
                        w.Angle = md.rotation;
                        if (w.Angle != 0.0)
                        {
                            w.snapMode = DraggableGridSnapper.SnapMode.VerticalLineSnap;

                        }
                    }
                    else {
                        foreach(CustomMesh cm in cmbObjects.Items){
                            if(cm.assetName == md.staticMesh){
                                Mesh m = MeshFactory.MakeCustomMesh(cm, this);
                                canvasRoom.Children.Add(m);
                                stuffLayer.Add(m);
                                m.WorldOriginX = md.x;
                                m.WorldOriginY = md.y;
                                m.Angle = md.rotation;
                                break;
                            }
                        }
                        
                    }
                }
                foreach (ItemDefinition id in sgd.items) {
                    Item i = new Item();
                    i.Init(this);
                    canvasRoom.Children.Add(i);
                    stuffLayer.Add(i);
                    i.WorldOriginX = id.x;
                    i.WorldOriginY = id.y;
                }
                
                foreach (GuardDefinition gd in sgd.guards)
                {
                    Guard g = new Guard();
                    g.Init(this);
                    canvasRoom.Children.Add(g);
                    stuffLayer.Add(g);
                    g.WorldOriginX = gd.x;
                    g.WorldOriginY = gd.y;
                    g.cmbRoutes.SelectedIndex = gd.patrolRouteIndex;
                    g.cmbStart.SelectedIndex = gd.startIndex;
                }
                foreach (CameraDefinition cd in sgd.cameras) {
                    Camera c = new Camera();
                    c.Init(this);
                    canvasRoom.Children.Add(c);
                    stuffLayer.Add(c);
                    c.WorldOriginX = cd.x;
                    c.WorldOriginY = cd.y;
                    c.Angle = cd.rotation;
                }

                if (sgd.lights != null)
                {
                    foreach (LightDefinition ld in sgd.lights)
                    {
                        Light l = new Light();
                        l.Init(this);
                        canvasRoom.Children.Add(l);
                        lightsLayer.Add(l);
                        l.WorldOriginX = ld.x;
                        l.WorldOriginY = ld.y;
                        l.Radius = ld.radius;
                    }
                }
            }

            fs.Close();

            EnableStuffLayer();
        }

        private void SaveRoom(string file) {

            window.Title = file;

            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
         

            RoomDefinition room = new RoomDefinition();
            if (northDoor != null) room.northDoor = true;
            if (eastDoor != null) room.eastDoor = true;
            if (southDoor != null) room.southDoor = true;
            if (westDoor != null) room.westDoor = true;
            room.roomRarity = cmbRarity.SelectedIndex;
            room.roomType = cmbRoomType.SelectedIndex;
            room.roomFloor = cmbFloorType.SelectedIndex;

            SpawnGroupDefinition g = new SpawnGroupDefinition();

            //Update the following when spawn groups are implemented:
            foreach (Mesh d in floorLayer) {
                g.meshes.Add(new MeshDefinition() { x = d.WorldOriginX, y = d.WorldOriginY, staticMesh = d.MeshType, rotation = d.Angle });
            }

            foreach (Mesh d in wallLayer)
            {
                g.meshes.Add(new MeshDefinition() { x = d.WorldOriginX, y = d.WorldOriginY, staticMesh = d.MeshType, rotation = d.Angle });
            }

            //foreach (Mesh d in stuffLayer)
            //{
            //    g.meshes.Add(new MeshDefinition() { x = d.WorldOriginX, y = d.WorldOriginY, staticMesh = d.MeshType, rotation = d.Angle });
            //}



            foreach (Light d in lightsLayer)
            {
                g.lights.Add(new LightDefinition() { x = d.WorldOriginX, y = d.WorldOriginY, radius = d.Radius });
            }
            
            foreach (DraggableGridSnapper d in stuffLayer)
            {
                if (d is Item) {
                    g.items.Add(new ItemDefinition() { x = d.WorldOriginX, y = d.WorldOriginY });
                }
                else if (d is Guard){
                    g.guards.Add(new GuardDefinition() { x = d.WorldOriginX, y = d.WorldOriginY, patrolRouteIndex = ((Guard)d).PatrolRouteIndex, startIndex = ((Guard)d).StartIndex });
                }
                else if (d is Camera)
                {
                    g.cameras.Add(new CameraDefinition() { x = d.WorldOriginX, y = d.WorldOriginY, rotation = d.Angle });
                }
                else if (d is Mesh)
                {
                    g.meshes.Add(new MeshDefinition() { x = d.WorldOriginX, y = d.WorldOriginY, staticMesh = ((Mesh)d).MeshType, rotation = d.Angle });
                }
            }

            room.spawnGroups.Add(g);

            List<PatrolRouteDefinition> patrolRouteDefs = new List<PatrolRouteDefinition>();
            foreach (var pr in patrolRoutes)
            {
                patrolRouteDefs.Add(new PatrolRouteDefinition());
                foreach (PatrolPoint p in pr)
                {
                    patrolRouteDefs[patrolRouteDefs.Count - 1].patrolPoints.Add(new PatrolPointDefinition() { x = p.WorldOriginX, y = p.WorldOriginY });
                }
            }
            room.patrolRoutes = patrolRouteDefs;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RoomDefinition));
            serializer.WriteObject(fs, room);

            fs.Close();
            
        }

        private void cmbObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCreateStaticObject_Click(object sender, RoutedEventArgs e)
        {
            if (cmbObjects.SelectedItem == null) return;

            Mesh obj = MeshFactory.MakeCustomMesh((CustomMesh)cmbObjects.SelectedItem,this);
            canvasRoom.Children.Add(obj);
            stuffLayer.Add(obj);
            if (!(bool)radStuff.IsChecked)
            {
                EnableStuffLayer();
                radStuff.IsChecked = true;
            }
            if (selected != null)
                selected.Deselect();
            selected = obj;
            obj.Select();
            obj.SnapToGrid();
        }

        private void btnAddNewStaticObject_Click(object sender, RoutedEventArgs e)
        {
            Window1 dialog = new Window1();
            dialog.ShowDialog();
            if(!dialog.cancelled){
                CustomMesh cm = new CustomMesh(dialog.width, dialog.height, dialog.originX, dialog.originY, dialog.startRotation, dialog.assetName);
                cmbObjects.Items.Add(cm);
                

                SaveCustomObjects();
            }
        }

        private void SaveCustomObjects() {

            FileStream fs = new FileStream("customobjects.json", FileMode.Create, FileAccess.Write);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<CustomMesh>));

            List<CustomMesh> customMeshes = new List<CustomMesh>();
            foreach (CustomMesh cm in cmbObjects.Items) {
                customMeshes.Add(cm);
            }
            serializer.WriteObject(fs, customMeshes);

            fs.Close();
        }

        private void LoadCustomObjects() {
            FileStream fs;
            try
            {
                fs = new FileStream("customobjects.json", FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException)
            {
                return;
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<CustomMesh>));

            List<CustomMesh> cm = (List<CustomMesh>)serializer.ReadObject(fs);
            foreach(CustomMesh c in cm)
                cmbObjects.Items.Add(c);

            fs.Close();
        }

        private void cmbHWalls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbVWalls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        




    }
}
