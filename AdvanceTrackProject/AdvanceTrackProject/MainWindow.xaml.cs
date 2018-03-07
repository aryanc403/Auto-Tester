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

namespace AdvanceTrackProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            //create dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //set type of file to open
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Executable (.exe)|*.exe";

            //open dialog box
            Nullable<bool> isOpened = dlg.ShowDialog();

            //if file selected
            if(isOpened == true)
            {
                string fileName = dlg.FileName;
                fileTb.Text = fileName;
            }
        }
    }
}
