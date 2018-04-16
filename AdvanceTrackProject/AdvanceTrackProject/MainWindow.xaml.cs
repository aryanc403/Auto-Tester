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
using System.Diagnostics;

namespace AdvanceTrackProject
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class ProcessExtensions
    {
        public static bool IsRunning (this Process process)
        {
            if(process == null)
            {
                throw new ArgumentNullException("process");
            }

            try
            {
                Process.GetProcessById(process.Id);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
    }


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        DataTable dt;// table;

        string getContent(string file)
        {
            string text;

            try
            {
                text = File.ReadAllText(file, Encoding.UTF8);
            }
            catch
            {
                text = null;
            }
            return text;
        }

        void initializeDataTable()
        {
            //DataTable dt = new DataTable();
            dt = new DataTable();
            DataColumn no = new DataColumn("Test Case", typeof(int));
            DataColumn input = new DataColumn("Input", typeof(string));
            DataColumn expOut = new DataColumn("Expected Output", typeof(string));
            DataColumn output = new DataColumn("Output", typeof(string));
            DataColumn result = new DataColumn("Result", typeof(string));

            string location = fileTb.Text;//@"E:\PracticeWPF\Tritonic Iridescence\a.exe"; //
            string dir = findDir(location);
            //string logFile = dir + "\\log" + i + ".txt";
            if (dir == null)
            {
                return;
            }

            dt.Columns.Add(no);
            dt.Columns.Add(input);
            dt.Columns.Add(expOut);
            dt.Columns.Add(output);
            dt.Columns.Add(result);
            
            int totalCases = Convert.ToInt32(totCases.Text);

            for (int i = 1; i <= totalCases ; ++i)
            {
                DataRow row = dt.NewRow();
                row[0] = i;
                row[1] = getContent(dir + @"\in" + i + ".txt");
                row[2] = getContent(dir + @"\exp" + i + ".txt");
                row[3] = getContent(dir + @"\out" + i + ".txt");
                row[4] = getContent(dir + @"\res" + i + ".txt");
                dt.Rows.Add(row);
            }
            dataTbl.ItemsSource = dt.DefaultView;
        }

        void kill_Process(Int64 id,string dir, string file)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = false;// true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.StandardInput.WriteLine(@"Taskkill /PID " + id + @"/F");
            process.StandardInput.Flush();
            int drive = dir.IndexOf("\\");
            process.StandardInput.WriteLine(dir.Substring(0, drive));
            process.StandardInput.Flush();
            process.StandardInput.WriteLine(@"cd /../../../../../../../../../../../../../");
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("cd " + dir.Substring(drive, dir.Length - drive));
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("del " + file);
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("exit");
            process.StandardInput.Flush();
            //List<string> TLEVerdict = new List<string>();
            //TLEVerdict.Add(@"Time Limit Exceed");
            //System.IO.File.WriteAllLines(@"E:\PracticeWPF\Tritonic Iridescence\out4.txt"/*outputFile*/, TLEVerdict );
            //System.IO.File.WriteAllLines(dir + @"\" +file, TLEVerdict);
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
            //initializeDataTable();
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

            if(i==-1)
            {
                return null;
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

        void createFile(List<HtmlNode> data, string fileType, int totalCases, string dir)
        {
            //loop to transverse all input and expected output files
            for (int i = 1; i <= totalCases; ++i)
            {
                string fileName = dir + "\\" + fileType + i + ".txt";//current file name with address
                string toWrite = convertHtmlToText(data[i - 1].InnerHtml);//text to be written in testcase file

                //separating lines in test case file
                List<string> lines = new List<string>();
                int end;
                end = toWrite.IndexOf("\n");
                while (end != -1)
                {
                    lines.Add(toWrite.Substring(0, end));
                    toWrite = toWrite.Substring(end + 1, toWrite.Length - end - 1);
                    end = toWrite.IndexOf("\n");
                }

                System.IO.File.WriteAllLines(fileName, lines);//creating test case file and adding test into it
            }
        }

        void createFileEmpty(string fileType, int totalCases, string dir)
        {
            string toWrite = "";//text to be written in file
            List<string> lines = new List<string>();
            //loop to transverse all files
            for (int i = 1; i <= totalCases; ++i)
            {
                string fileName = dir + "\\" + fileType + i + ".txt";//current file name with address
                System.IO.File.WriteAllLines(fileName, lines);//creating test case file and adding test into it
            }
        }

        bool IsValidUrl(string url)
        {
            for(int i=0;i<=url.Length - 14;++i)
            {
                if(url.Substring(i,14)=="codeforces.com")
                {
                    return true;
                }
            }
            return false;
        }

        //private static async Task StartCrawlerAsync()
        private async Task StartCrawlerAsync()
        {
            var url = pathUrlTb.Text;
            //var url = "http://codeforces.com/problemset/problem/957/A";
            //var url = @"http://codeforces.com/problemset/problem/955/A";
            //fileTb.Text = @"E:\PracticeWPF\Tritonic Iridescence\a.exe";
            //var url = "http://codeforces.com/contest/957/submission/36585822";//sample url must change before final submit
            //var url = "http://codeforces.com/problemset/problem/949/B";
            var httpClient = new HttpClient(); //create HttpClient class

            if (IsValidUrl(url)==false)
            {
                pathUrlTb.Text = "Error - Invalid Url.";
                return;
            }

            var html = await httpClient.GetStringAsync(url);//set Source code
            var exeFile = fileTb.Text;
            //var exeFile = @"E:\PracticeWPF\Tritonic Iridescence\a.exe"; 

            string dir = findDir(exeFile);

            if(dir==null)
            {
                fileTb.Text = "Error - Directory Not Found.";
                return;
            }

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

            if(totalCases<=0)
            {
                pathUrlTb.Text = "Error - No Test Cases found.";
                return;
            }
            
            //function calls to create in<no>.txe ans exp<no>.txt files in exeFile directory
            createFile(inputs, "in", totalCases, dir);
            createFile(outputs, "exp", totalCases, dir);
            createFileEmpty("out", totalCases, dir);
            createFileEmpty("log", totalCases, dir);
            createFileEmpty("res", totalCases, dir);
            initializeDataTable();
        }

        private void runBtn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string location = fileTb.Text;//@"E:\PracticeWPF\Tritonic Iridescence\a.exe"; //
            string dir = findDir(location);
            if(dir==null)
            {
                return;
            }

            string appName = location.Substring(dir.Length + 1, location.Length - dir.Length - 1);
            int totalCases = Convert.ToInt32(totCases.Text);
            for(int i =1; i<= totalCases;++i)
            {
                runExe(appName,dir,i);
                //run result generator.
                runGen(dir, i);
            }
            initializeDataTable();
        }

        private void runGen(string dir, int i)
        {//fill this fn
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;// true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string DIR = Directory.GetCurrentDirectory();
            int drive = dir.IndexOf("\\");
            process.StandardInput.WriteLine(DIR.Substring(0, drive));
            process.StandardInput.Flush();
            process.StandardInput.WriteLine(@"cd /../../../../../../../../../../../../../");
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("cd " + DIR.Substring(drive, DIR.Length - drive));
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("cd " + @"../../../");
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("cd " + @"Result Generator");
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("result \"" + dir + @"\exp" + i + ".txt\" \"" + dir + @"\out" + i + ".txt\" \"" + dir + @"\res" + i + ".txt\" \"" +dir + @"\log" + i + ".txt\"");
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("exit");
            process.StandardInput.Flush();
        }

        private void runExe(string app,string dir ,int i)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;// true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();

            int drive = dir.IndexOf("\\");
            process.StandardInput.WriteLine(dir.Substring(0, drive));
            process.StandardInput.Flush();
            process.StandardInput.WriteLine(@"cd /../../../../../../../../../../../../../");
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("cd " + dir.Substring(drive, dir.Length - drive));
            process.StandardInput.Flush();
            int id = process.Id;
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            process.StandardInput.WriteLine(app + " <in" + i + ".txt> out" + i + ".txt");
             process.StandardInput.Flush();
            //int x = process.ExitCode;
            process.StandardInput.WriteLine("exit");
            process.StandardInput.Flush();
            //int y = process.ExitCode;
            //process.WaitForExit();
            int TL = Convert.ToInt32(tlVal.Text);
            bool isTLE = false,a;
            while (process.IsRunning() && (sw.Elapsed.TotalMilliseconds <= (TL + 100)) &&(isTLE==false))
            {
                if( sw.Elapsed.TotalMilliseconds > TL )
                {
                    isTLE = true;
                    sw.Stop();
                    break;
                }
                a = process.IsRunning();
            }
            string logFile = dir + "\\log" + i + ".txt";
            sw.Stop();
            if (sw.Elapsed.TotalMilliseconds > TL)
            {
                kill_Process(process.Id,dir, "out" + i + ".txt");//process.Kill();
                //process.WaitForExit();
                List<string> TLEVerdict = new List<string>();
                TLEVerdict.Add(@"TimeLimitExceed.");
                //System.IO.File.WriteAllLines(@"E:\PracticeWPF\Tritonic Iridescence\out4.txt"/*outputFile*/, TLEVerdict );
                System.IO.File.WriteAllLines(logFile, TLEVerdict);
            }
            else
            {
                List<string> _success = new List<string>();
                _success.Add(@"Successful.");
                //System.IO.File.WriteAllLines(@"E:\PracticeWPF\Tritonic Iridescence\out4.txt"/*outputFile*/, TLEVerdict );
                System.IO.File.WriteAllLines(logFile, _success);
                process.Close();
            }
        }
    }
}