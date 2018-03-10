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
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;

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
            if (isOpened == true)
            {
                string fileName = dlg.FileName;
                fileTb.Text = fileName;
            }
        }

        private void fetchBtn_Click(object sender, RoutedEventArgs e)
        {
            StartCrawlerAsync();
        }

        private static async Task StartCrawlerAsync()
        {
            var url = "http://codeforces.com/problemset/problem/950/B";//sample url must change before final submit
            //var url = "http://codeforces.com/problemset/problem/949/B";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var inputs = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue
                ("class", "")
                    .Equals("input")).ToList();

            var outputs = htmlDocument.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue
                            ("class", "")
                                .Equals("output")).ToList();



            //Console.WriteLine("Aryan");
        }
    }
}