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
using System.Windows.Shapes;

namespace StaniEdit
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public bool cancelled = false;
        public double width = 0.0;
        public double height = 0.0;
        public double originX = 0.0;
        public double originY = 0.0;
        public double startRotation = 0.0;
        public string assetName = "";
        public bool lineSnap = false;

        public Window1()
        {
            InitializeComponent();
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            width = Double.Parse(txtWidth.Text);
            height = Double.Parse(txtHeight.Text);
            originX = Double.Parse(txtOriginX.Text);
            originY = Double.Parse(txtOriginY.Text);
            startRotation = Double.Parse((String)((ComboBoxItem)cmbStartRotation.SelectedItem).Content);
            assetName = txtName.Text;
            lineSnap = (bool)chkLineSnap.IsChecked;

            cancelled = false;
            Hide();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            cancelled = true;
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}
