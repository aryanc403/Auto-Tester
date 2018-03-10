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
using System.Data;

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
            var html = await httpClient.GetStringAsync(url);//set Source code

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var inputs = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue
                ("class", "")
                    .Equals("input")).ToList();//store All of inputs in a list

            var outputs = htmlDocument.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue
                            ("class", "")
                                .Equals("output")).ToList();//store All of outputs in a list

            var n = inputs.Count();
            //dataTbl.initializeDataTable(n+1);
            Console.WriteLine("Aryan");
        }

        void initializeDataTable(int n)
        {
            DataTable dt = new DataTable();
            DataColumn no = new DataColumn("Test Case", typeof(int));
            DataColumn input = new DataColumn("Input", typeof(string));
            DataColumn expOut = new DataColumn("Expected Output", typeof(string));
            DataColumn output = new DataColumn("Output", typeof(string));
            DataColumn result = new DataColumn("Result", typeof(string));

            dt.Columns.Add(no);
            dt.Columns.Add(input);
            dt.Columns.Add(expOut);
            dt.Columns.Add(output);
            dt.Columns.Add(result);

            for (int i = 0; i < n; ++i)
            {
                DataRow row = dt.NewRow();
                row[0] = i + 1;
                row[1] = " - ";
                row[2] = " - ";
                row[3] = " - ";
                row[4] = " - ";
                dt.Rows.Add(row);
            }
            dataTbl.ItemsSource = dt.DefaultView;
        }

        private void dataTbl_Loaded(object sender, RoutedEventArgs e)
        {
            this.initializeDataTable(3);
        }
    }
}