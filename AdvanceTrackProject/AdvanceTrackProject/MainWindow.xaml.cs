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
using System.IO;

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

        public string findDir(string addr)
        {
            string path;
            int i = 0;
            for (i = addr.Length - 1; i >= 0; i--)
            {
                if (addr[i] == '\\')
                {
                    break;
                }
            }
            path = addr.Substring(0, i);
            return path;
        }

        string convertHtmlToText(string text)
        {
            int i = 0;
            i = text.IndexOf("<pre>");
            text = text.Substring(i + 5, text.Length - i - 11);
            i = text.IndexOf("<br>");

            while (i != -1)
            {
                text = text.Substring(0, i) + "\n" + text.Substring(i + 4, text.Length - i - 4);
                i = text.IndexOf("<br>");
            }
            return text;
        }

        void createFile( List <HtmlNode> data ,string pre,int n,string path)
        {
            int i;
            for(i=1;i<=n;++i)
            {
                string fileName = path + "\\" + pre + i + ".txt";
                //FileStream f = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                string toWrite = convertHtmlToText(data[i-1].InnerHtml);

                int end;
                end = toWrite.IndexOf("\n");

                List<string> lines = new List<string>();

                while(end!=-1)
                {
                    lines.Add(toWrite.Substring(0, end));
                    toWrite = toWrite.Substring(end + 1, toWrite.Length - end -1);
                    end = toWrite.IndexOf("\n");
                }

                //byte b[] = new byte[toWrite];
                /*for(int j=0;j<toWrite.Length;++j)
                {
                    f.WriteByte((byte)(toWrite[j]));
                }*/
                //f.Write(b, 0, b.length);
                //f.Close();
                System.IO.File.WriteAllLines(fileName, lines);
            }
        }

        //private static async Task StartCrawlerAsync()
        private async Task StartCrawlerAsync()
        {
            //var url = "http://codeforces.com/problemset/problem/957/A";
            var url = @"http://codeforces.com/problemset/problem/955/A";
            //fileTb.Text = @"E:\PracticeWPF\Tritonic Iridescence\a.exe";
            //var url = "http://codeforces.com/contest/957/submission/36585822";//sample url must change before final submit
            //var url = "http://codeforces.com/problemset/problem/949/B";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);//set Source code
            var exeFile = @"E:\PracticeWPF\Tritonic Iridescence\a.exe"; // fileTb.Text;

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
            createFile(inputs, "in", n, findDir(exeFile));
            createFile(outputs, "exp", n, findDir(exeFile));
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