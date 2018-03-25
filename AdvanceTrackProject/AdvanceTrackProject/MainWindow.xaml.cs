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
            StartCrawlerAsync();//call task of web Crawler
        }

        public string findDir(string addr)
            // function to find working directory of user.
        {
            string path;
            int i = 0;
            //find last position of '/'
            for (i = addr.Length - 1; i >= 0; i--)
            {
                if (addr[i] == '\\')
                {
                    break;
                }
            }
            path = addr.Substring(0, i);//truncate string to /
            return path;
        }

        string convertHtmlToText(string text)
            //convert given html languge input and output class in string
        {
            int i = 0;
            i = text.IndexOf("<pre>");
            //remove all text till <pre> and last line </pre>
            text = text.Substring(i + 5, text.Length - i - 11);


            //remove all <br> and replace them with '\n'
            i = text.IndexOf("<br>");
            while (i != -1)
            {
                text = text.Substring(0, i) + "\n" + text.Substring(i + 4, text.Length - i - 4);
                i = text.IndexOf("<br>");
            }

            return text;
        }

        void createFile( List <HtmlNode> data ,string fileType,int totalCases, string dir)
        {
            //loop to transverse all input and expected output files
            for(int i = 1; i <= totalCases; ++i)
            {
                string fileName = dir + "\\" + fileType + i + ".txt";//current file name with address
                string toWrite = convertHtmlToText(data[i-1].InnerHtml);//text to be written in testcase file

                //separating lines in test case file
                List<string> lines = new List<string>();
                int end;
                end = toWrite.IndexOf("\n");
                while(end!=-1)
                {
                    lines.Add(toWrite.Substring(0, end));
                    toWrite = toWrite.Substring(end + 1, toWrite.Length - end -1);
                    end = toWrite.IndexOf("\n");
                }

                System.IO.File.WriteAllLines(fileName, lines);//creating test case file and adding test into it
            }
        }

        //private static async Task StartCrawlerAsync()
        private async Task StartCrawlerAsync()
        {
            var url = "http://codeforces.com/problemset/problem/957/A";
            //var url = @"http://codeforces.com/problemset/problem/955/A";
            //fileTb.Text = @"E:\PracticeWPF\Tritonic Iridescence\a.exe";
            //var url = "http://codeforces.com/contest/957/submission/36585822";//sample url must change before final submit
            //var url = "http://codeforces.com/problemset/problem/949/B";
            var httpClient = new HttpClient(); //create HttpClient class
            var html = await httpClient.GetStringAsync(url);//set Source code
            var exeFile = @"E:\PracticeWPF\Tritonic Iridescence\a.exe"; // fileTb.Text;

            var htmlDocument = new HtmlDocument(); //create HtmlDocument class
            htmlDocument.LoadHtml(html); // loading html document in HtmlDocument class
            stateMent.Navigate(url);//Display web Page in web Browser

            //store All of inputs in a list
            var inputs = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue
                ("class", "")
                    .Equals("input")).ToList();

            //store All of outputs in a list
            var outputs = htmlDocument.DocumentNode.Descendants("div")
                            .Where(node => node.GetAttributeValue
                            ("class", "")
                                .Equals("output")).ToList();

            var totalCases = inputs.Count();//count no of inputs

            totCases.Text = totalCases.ToString();//display total no of inputs in textbox

            //function calls to create in<no>.txe ans exp<no>.txt files in exeFile directory
            createFile(inputs, "in", totalCases, findDir(exeFile));
            createFile(outputs, "exp", totalCases, findDir(exeFile));
            //dataTbl.initializeDataTable(totalCases+1);
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

        private void runBtn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //await runExe(exeFile, n);
        }

        private async Task runExe(string exeFile, int n)
        {
            string dir = findDir(exeFile);
            string fileName = exeFile.Substring(dir.Length + 1, exeFile.Length - dir.Length - 1);

            for (int i = 1; i <= n; ++i)
            {
                //runForFile(dir,)
            }
        }
    }
}