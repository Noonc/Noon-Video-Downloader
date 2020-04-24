using System;
using System.Collections;
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
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.IO.Compression;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Net.Sockets;
using System.Configuration;

namespace Video_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        TextWriter _writer = null;

        public object Proccess { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeStart();
            InitializeCMD();
            InitializeFinal();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void InitializeCFG()
        {
            var autoLg = ConfigurationManager.AppSettings["autolog"];
            var ipChk = ConfigurationManager.AppSettings["checkip"];
            Console.WriteLine(String.Format("[CONFIG] Autolog set to: '{0}'", autoLg));
            Console.WriteLine(String.Format("[CONFIG] IP-Check set to: '{0}'", ipChk));
        }

        private void InitializeIP()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            string GetIp = new WebClient().DownloadString("http://icanhazip.com");
            var Host = Dns.GetHostEntry(Dns.GetHostName());
            Console.WriteLine("[YDDL] Current Public IP: " + GetIp + "[YDDL] Current Local IP: " + localIP);
            if (localIP.StartsWith("10"))
            {
                Console.WriteLine("[YDDL] Possible VPN Detected...");
                vpn_lbl.Content = "VPN Enabled?";
            }
            else
            {
                Console.WriteLine("[YDDL] No VPN Detected...");
                vpn_lbl.Content = "VPN Disabled?";
            }
            
        }

        private void InitializeFinal()
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl");
            var fileLOG = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\logs\");
            Console.WriteLine("[YDDL] Checking for all required files...");
            vrs_lbl.Content = "5.8.9";
            if (Directory.Exists(filePath) & Directory.Exists(fileLOG) & File.Exists(filePath + @"\youtube-dl.exe") & File.Exists(filePath + @"\ffmpeg.exe") & File.Exists(filePath + @"\common-bugs.txt") & File.Exists(filePath + @"\readme.txt"))
            {
                Console.WriteLine("[YDDL] All Necessary Files Found...");
                InitializeDebug();
            }
            else
            {
                Console.WriteLine("[YDDL] Some or all files not found... \n[YDDL] Initializing Repair...");
                InitializeDownload();
            }

        }
        private void  tb_URL_doub(object sender, EventArgs eventArgs)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
            textBox.Focus();
        }
        private void InitializeCMD()
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var cmdPath = Environment.ExpandEnvironmentVariables(pathWithEnv + "/vddl/youtube-dl.exe");
            cmdlab.Text = cmdPath;
            textCookie.Text = "";
            textOutput.Text = "";
        }

        private void InitializeStart()
        {
           
            _writer = new TextBoxStreamWriter(txtConsole);
            Console.SetOut(_writer);
            Console.WriteLine("[YDDL] Initalizing Start...");
            InitializeCFG();
        }

        private void AdminRelauncher()
        {
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;

                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
                }
            }
        }

        private bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


        public void InitializeDownload()
        {
            using (WebClient webClient = new WebClient())
            {
                var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
                var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl");
                string readMe = filePath + @"\readme.txt";
                string fileame = filePath + @"\common-bugs.txt";
                if (!Directory.Exists(filePath) || !File.Exists(filePath + @"\youtube-dl.exe") || !File.Exists(filePath + @"\ffmpeg.exe") || !File.Exists(filePath + @"\phantomjs.exe"))
                {
                    Console.WriteLine("[YDDL] Executing First Time Setup...");
                    Directory.CreateDirectory(filePath);
                    webClient.DownloadFile("https://yt-dl.org/latest/youtube-dl.exe", filePath + "/youtube-dl.exe");
                    webClient.DownloadFile("https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.zip", filePath + "/ffmpeg.zip");
                    webClient.DownloadFile("https://bitbucket.org/ariya/phantomjs/downloads/phantomjs-2.1.1-windows.zip", filePath + "/phantomjs.zip");
                    Console.Write("[YDDL] Download Complete \n[YDDL] Moving files to Correct Path...");
                    ZipFile.ExtractToDirectory(filePath + "/ffmpeg.zip", filePath);
                    ZipFile.ExtractToDirectory(filePath + "/phantomjs.zip", filePath);
                    File.Delete(filePath + "/ffmpeg.zip");
                    File.Delete(filePath + "/phantomjs.zip");
                    string ffmpegexemove = filePath + "/ffmpeg-latest-win64-static/bin";
                    string phantomjsexemove = filePath + "/phantomjs-2.1.1-windows/bin";
                    string target = filePath;
                    string[] files = System.IO.Directory.GetFiles(ffmpegexemove);
                    string[] filess = System.IO.Directory.GetFiles(phantomjsexemove);
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        string fileName = System.IO.Path.GetFileName(s);
                        string targetF = System.IO.Path.Combine(target, fileName);
                        System.IO.File.Copy(s, targetF, true);
                    }
                    foreach (string s in filess)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        string fileName = System.IO.Path.GetFileName(s);
                        string targetJ = System.IO.Path.Combine(target, fileName);
                        System.IO.File.Copy(s, targetJ, true);
                    }
                    string ffzipfolder = filePath + "/ffmpeg-latest-win64-static";
                    string PJzipfolder = filePath + "/phantomjs-2.1.1-windows";
                    Directory.Delete(ffzipfolder, true);
                    Directory.Delete(PJzipfolder, true);
                    Console.WriteLine("\n[YDDL] Install Complete... ");
                    InitializeFinal();
                }
                if (!File.Exists(fileame))
                {
                    File.AppendAllText(fileame, "These are the common-bugs of the downloader. \n" +
                        "If a download fails please reference this in your troubleshooting, " +
                        "\nor search yt-dl (youtube-dl) and your problem/error online. \n" +
                        "Please note any console message without the prefix '[YDDL]' is all youtube-dl. \n" +
                        "Therefore any error's or bugs should be researched based off of youtube-dl libraries and resources. \n" +
                        "If your file does not show up in your selected output, check the Console Window for any error messages. \n" +
                        "If the downloader is unable to extract the title your cookie file is invalid. Simply create a new one and reload it." +
                        "\nIf the error is related to authentication, \nmake sure you have permission to view the link you are trying to download. \n" +
                        "I suggest using cookies for links like privated youtube videos, etc. \n" +
                        "In order to create a proper cookie file, download any cookie plugin/addon for your browser. \n" +
                        "Then depending on what site you are downloading from, make sure you are logged in to the correct account. \n" +
                        "E.g if it is youtube, make sure you are logged in. Then create a cookie.txt. \nThe name does not matter, just has to be a txt. \n" +
                        "If you get the error 'Log Directory does not exist', you must create a log first.\n" +
                        "If you get an error saying 'Cannot save audio and video on same file', etc." +
                        "\nThis means you have a space in your folder.\n" +
                        "Sadly currently the ouput folder cannot have spaces in them, instead changed them later.   ");
                    if (File.Exists(fileame))
                    {
                        Console.WriteLine("[YDDL] All Debug TXT's Created...");
                        InitializeFinal();
                    }
                    else
                    {
                        Console.WriteLine("[YDDL] Debug TXT's Failed to Create...");
                    }
                }
                var fileLOG = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\logs\");
                if (!Directory.Exists(fileLOG))
                {
                    Directory.CreateDirectory(fileLOG);
                    Console.WriteLine("[YDDL] Log Directory Created...");
                    InitializeFinal();
                }
                if (!File.Exists(readMe))
                {
                    File.AppendAllText(readMe, "Thank you for using my program. \n" +
                        "This is a video/media downloader that utilizes the youtube-dl libraries. \n" +
                        "All you have to do is input the url in the top textboxt then select an ouput. \n" +
                        "There are other options like cookies for privated links, \n and format options like MP3 (Only current format). \n" +
                        "\n" +
                        "If you have multiple links to download (like a series) or an album, you can use the batch list. \n" +
                        "If you have a youtube playlist, you can just use that url in the normal textbox. \n" +
                        "However, if you have multiple single links, you can use the right textbox and the add button. \n" +
                        "This will your link to the list and you will see the light red box fill up. \n" +
                        "If you are using a vpn, upon launch you will be 'notified' of your current IP. \nThis has only been tested with NordVPN, however. \n" +
                        "You will also be notified if a vpn is detected. \nIf your vpn uses a local IP starting with 10, it will 'detect' a vpn. \n" +
                        "Any bugs or errors can first be refered to the common-bugs.txt, \n a shortcut to that exists at the bottom of the window. \n" +
                        "Any 'Error:' messages without the prefix '[YDDL]' should be researched according to youtube-dl. \n" +
                        "To access the config file, navigate to your install directory. \n" +
                        "Find the file 'Video Downloader.dll.config' and edit it with notepad or any other text editor. \n" +
                        "Be careful not to mess up the layout of the document, not removing any characters. \n" +
                        "To turn on either Autolog or Check-IP, just make sure they are equal to 'true' no caps. \n" +
                        "Otherwise anything else with act as 'false'. \n" +
                        "Currently that is the only way to change the config");
                    InitializeFinal();

                }
            }
        }

        public void InitializeDebug()
        {
            Console.WriteLine("[YDDL] Starting Routine Debug...");
            //var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            //var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\");
            //string fileName = filePath + "common-bugs.txt";
            //if (!File.Exists(fileName))
            //{
            //    File.AppendAllText(fileName, "These are the common-bugs of the downloader. \n If a download fails please reference this in your troubleshooting, or search yt-dl (youtube-dl) and your problem/error online. \n If the downloader fails to get the title, either one, you need to update by performing the -u command under youtube-dl.exe, or two, your cookie file is invalid. Simply create a new one and reload it. \n If the error is related to authentication, make sure you have permission to view the link you are trying to download.");
            //    if (File.Exists(fileName))
            //    {
            //        Console.WriteLine("All Debug TXT's Created...");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Debug TXT's Failed to Create...");
            //    }
            //}
            Console.WriteLine("[YDDL] Checking for Youtube-DL Update...");
            Process update = new Process();
            update.StartInfo.FileName = cmdlab.Text;
            update.StartInfo.Arguments = "--update";
            update.StartInfo.UseShellExecute = false;
            update.StartInfo.RedirectStandardOutput = true;
            update.OutputDataReceived += proc_OutputDataRecieved;
            update.StartInfo.CreateNoWindow = true;
            update.EnableRaisingEvents = true;
            update.Exited += new EventHandler(end_Debug);
            update.Start();
            update.BeginOutputReadLine();
        }

        private void end_Debug(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate ()
            {
                InitializeIP();
            }));
        }

        private void but_SD_Click(object sender, RoutedEventArgs e)
        {
            string checkIP = ConfigurationManager.AppSettings["checkip"];
            if(checkIP == "true")
            {
                string localIP;
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
                if (localIP.StartsWith("10"))
                {
                    StartDownload();
                }
                else
                {
                    Console.WriteLine("[YDDL] No VPN Detected, Canceling Download!...");
                }
            }
            else
            {
                StartDownload();
            }
        }
        private void StartDownload()
        {
            string cmdFull = cmdlab.Text;
            string cookie = textCookie.Text;
            string output = textOutput.Text;
            string URL = tb_URL.Text;
            if (output == "")
            {
                Console.WriteLine("[YDDL] No Output Directory Found...");
                MessageBox.Show("[YDDL] Please Select an Output Location");
                return;
            }
            if (mp3_check.IsChecked ?? true)
            {
                if (cookie != "")
                {
                    string ALL = System.IO.Path.Combine(cmdFull + " --format mp3" + " --cookies " + cookie + " --output " + output + " " + URL);
                    Console.WriteLine("[YDDL] Executing Command: " + ALL);
                    Process p = new Process();
                    p.StartInfo.FileName = cmdFull;
                    p.StartInfo.Arguments = "-x --audio-format mp3" + " --cookies " + cookie + " --output " + output + " " + URL;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.OutputDataReceived += proc_OutputDataRecieved;
                    p.ErrorDataReceived += proc_ErrorDataRecieved;
                    p.EnableRaisingEvents = true;
                    p.Exited += new EventHandler(myProcess_Exited);
                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                }
                else
                {
                    //-x --audio-format
                    string Mp3a = System.IO.Path.Combine(cmdFull + " --format mp3" + " --output " + output + " " + URL);
                    Console.WriteLine("[YDDL] Executing Command: " + Mp3a);
                    Process pa = new Process();
                    pa.StartInfo.FileName = cmdFull;
                    pa.StartInfo.Arguments = "-x --audio-format mp3" + " --output " + output + " " + URL;
                    pa.StartInfo.UseShellExecute = false;
                    pa.StartInfo.RedirectStandardOutput = true;
                    pa.StartInfo.RedirectStandardError = true;
                    pa.StartInfo.CreateNoWindow = true;
                    pa.OutputDataReceived += proc_OutputDataRecieved;
                    pa.ErrorDataReceived += proc_ErrorDataRecieved;
                    pa.EnableRaisingEvents = true;
                    pa.Exited += new EventHandler(myProcess_Exited);
                    pa.Start();
                    pa.BeginOutputReadLine();
                    pa.BeginErrorReadLine();
                }
            }
            else if (cookie != "")
            {
                string CookOut = System.IO.Path.Combine(cmdFull + " --cookies " + cookie + " --output " + output + " " + URL);
                Console.WriteLine("[YDDL] Executing Command: " + CookOut);
                Process pas = new Process();
                pas.StartInfo.FileName = cmdFull;
                pas.StartInfo.Arguments = "--cookies " + cookie + " --output " + output + " " + URL;
                pas.StartInfo.UseShellExecute = false;
                pas.StartInfo.RedirectStandardOutput = true;
                pas.StartInfo.RedirectStandardError = true;
                pas.StartInfo.CreateNoWindow = true;
                pas.OutputDataReceived += proc_OutputDataRecieved;
                pas.ErrorDataReceived += proc_ErrorDataRecieved;
                pas.EnableRaisingEvents = true;
                pas.Exited += new EventHandler(myProcess_Exited);
                pas.Start();
                pas.BeginOutputReadLine();
                pas.BeginErrorReadLine();
            }
            else
            {
                string Out = System.IO.Path.Combine(cmdFull + " --output " + output + " " + URL);
                Console.WriteLine("[YDDL] Executing Command: " + Out);
                Process pass = new Process();
                pass.StartInfo.FileName = cmdFull;
                pass.StartInfo.Arguments = "--output " + output + " " + URL;
                pass.StartInfo.UseShellExecute = false;
                pass.StartInfo.RedirectStandardOutput = true;
                pass.StartInfo.RedirectStandardError = true;
                pass.StartInfo.CreateNoWindow = true;
                pass.OutputDataReceived += proc_OutputDataRecieved;
                pass.ErrorDataReceived += proc_ErrorDataRecieved;
                pass.EnableRaisingEvents = true;
                pass.Exited += new EventHandler(myProcess_Exited);
                pass.Start();
                pass.BeginOutputReadLine();
                pass.BeginErrorReadLine();
                //MessageBox.Show("Wait for the CMD Window to Close, then check output for file");

            }
        }

        private void proc_ErrorDataRecieved(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate ()
            {
                Console.WriteLine(e.Data);
                //MessageBox.Show(e.Data);
                txtConsole.ScrollToEnd();
            }));
        }

        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate ()
            {
                Console.WriteLine("[YDDL] DOWNLOAD COMPLETE...");
                Console.WriteLine("\n[YDDL] CHECK OUTPUT DIRECTORY...");
                txtConsole.ScrollToEnd();
            }));
        }

        private void proc_OutputDataRecieved(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate ()
            {
                Console.WriteLine(e.Data);
                txtConsole.ScrollToEnd();
            }));
               
        }

        private void but_outputset_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folddl = new VistaFolderBrowserDialog();
            folddl.ShowNewFolderButton = true;

            Nullable<bool> result = folddl.ShowDialog();
            if (result == true)
            {
                textOutput.Text = folddl.SelectedPath;
                textOutputF.Text = folddl.SelectedPath;
                out_lb.Content = folddl.SelectedPath;
                string textO = textOutput.Text;
                Console.WriteLine(@"[YDDL] Output Location Selected: """ + textO + @"""...");
                textOutput.Text = textO + @"\%(title)s.%(ext)s ";
            }

        }

        private void but_cookieset_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                textCookie.Text = openFileDialog.FileName;
                string textC = textCookie.Text;
                Console.WriteLine(@"[YDDL] Cookie File Selected: """ + textC + @"""...");
                CookieDir.Content = textC;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string outa = textOutputF.Text;

            if (Directory.Exists(outa))
            {
                Console.WriteLine("[YDDL] Opening Output Location: " + @"""" + outa + @"""...");
                Process.Start("explorer.exe", outa);
            }
            else if (String.IsNullOrEmpty(textOutputF.Text))
            {
                Console.WriteLine("[YDDL] No Directory Selected!");
            }
            else
            {
                Console.WriteLine(string.Format("[YDDL] Directory does not exist!", outa));
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var fileTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\batchdown.txt");
            File.Delete(fileTXT);

            string autoLG = ConfigurationManager.AppSettings["autolog"];
            if (autoLG == "true")
            {
                string dateTime = DateTime.Now.ToString("MM-dd-yyyy h-mm tt");
                var pathWithEnvv = @"%USERPROFILE%\Appdata\roaming";
                var fileTXTt = Environment.ExpandEnvironmentVariables(pathWithEnvv + @"\vddl\logs\");
                string fileNamee = fileTXTt + dateTime + ".txt";
                File.AppendAllText(fileNamee, txtConsole.Text);               
            }
        }

        private void logC_bt_Click(object sender, RoutedEventArgs e)
        {
            string dateTime = DateTime.Now.ToString("MM-dd-yyyy h-mm tt");
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var fileTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\logs\");
            string fileName = fileTXT + dateTime + ".txt";
            File.AppendAllText(fileName, txtConsole.Text);
            Console.WriteLine("[YDDL] Creating Log File at: " + fileName);
            //if (!Directory.Exists(fileTXT))
            //{
            //    Directory.CreateDirectory(fileTXT);
            //}
            if (!File.Exists(fileName))
            {
                Console.WriteLine("[YDDL] Log Creation Failed...");
            }
        }

        private void open_LogDir_Click(object sender, RoutedEventArgs e)
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var fileTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\logs\");
            if (Directory.Exists(fileTXT))
            {
                Console.WriteLine("[YDDL] Opening Logs Location: " + fileTXT + "...");
                Process.Start("explorer.exe", fileTXT);
            }
            else
            {
                Console.WriteLine("[YDDL] Log Directory does not exist! Perhaps it moved?");
            }
        }

        private void open_LogBug_Click(object sender, RoutedEventArgs e)
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var fileTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\common-bugs.txt");
            if (File.Exists(fileTXT))
            {
                Console.WriteLine("[YDDL] Opening Bug/Error TXT Location: " + fileTXT + "...");
                Process.Start("notepad.exe", fileTXT);
            }
            else
            {
                Console.WriteLine("[YDDL] Bug/Error file does not exist! Perhaps it moved?");
            }
        }

        private void open_YDDLData_Click(object sender, RoutedEventArgs e)
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var fileTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\");
            if (Directory.Exists(fileTXT))
            {
                Console.WriteLine("[YDDL] Opening Program Location at: " + fileTXT + "...");
                Process.Start("explorer.exe", fileTXT);
            }
            else
            {
                Console.WriteLine("[YDDL] Error While Attempting to Open Program Location...");
            }
        }
        public int ount;
        private void Add_Butt_Click(object sender, RoutedEventArgs e)
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var fileTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\batchdown.txt");
            ArrayList BatchD = new ArrayList();
            if (File.Exists(fileTXT))
            {
                BatchD.Add(Add_TXT.Text);
                string appTEXT = Add_TXT.Text;
                Add_TXT.Clear();
                ount++;
                string CaC = ount.ToString();
                OutBat_TXT.AppendText("\n" + CaC + ": " + appTEXT);
                File.AppendAllLines(fileTXT, BatchD.Cast<string>());
                OutBat_TXT.ScrollToEnd();
                   
            }
            else
            {
                File.AppendAllText(fileTXT, "");
                BatchD.Add(Add_TXT.Text);
                string appTEXT = Add_TXT.Text;
                Add_TXT.Clear();
                ount++;
                string CaC = ount.ToString();
                OutBat_TXT.AppendText(ount + ": " + appTEXT);
                File.AppendAllLines(fileTXT, BatchD.Cast<string>());
                OutBat_TXT.ScrollToEnd();
            }

            //Use this on download button, separte.
            //File.WriteAllLines(fileTXT, BatchD.Cast<string>());
        }
        
        private void Down_Butt_Batch_Click(object sender, RoutedEventArgs e)
        {
            string checkIP = ConfigurationManager.AppSettings["checkip"];
            if (checkIP == "true")
            {
                string localIP;
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
                if (localIP.StartsWith("10"))
                {
                    StartBat();
                }
                else
                {
                    Console.WriteLine("[YDDL] No VPN Detected, Canceling Download!...");
                }
            }
            else
            {
                StartBat();
            }
            
        }
        private void StartBat()
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var BatTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\batchdown.txt");
            string cmdFull = cmdlab.Text;
            string cookie = textCookie.Text;
            string output = textOutput.Text;
            if (output == "")
            {
                Console.WriteLine("[YDDL] No Output Directory Found...");
                MessageBox.Show("[YDDL] Please Select an Output Location");
                return;
            }
            if (mp3_check.IsChecked ?? true)
            {
                if (cookie != "")
                {
                    string ALL = System.IO.Path.Combine(cmdFull + " --format mp3" + " --cookies " + cookie + " --output " + output + " " + "-a " + BatTXT);
                    Console.WriteLine("[YDDL] Executing Batch Command: " + ALL);
                    Process p = new Process();
                    p.StartInfo.FileName = cmdFull;
                    p.StartInfo.Arguments = "-x --audio-format mp3" + " --cookies " + cookie + " --output " + output + " " + "-a " + BatTXT;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.OutputDataReceived += proc_OutputDataRecieved;
                    p.ErrorDataReceived += proc_ErrorDataRecieved;
                    p.EnableRaisingEvents = true;
                    p.Exited += new EventHandler(myProcess_Exited);
                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                }
                else
                {
                    //-x --audio-format
                    string Mp3a = System.IO.Path.Combine(cmdFull + " --format mp3" + " --output " + output + " " + "-a " + BatTXT);
                    Console.WriteLine("[YDDL] Executing Batch Command: " + Mp3a);
                    Process pa = new Process();
                    pa.StartInfo.FileName = cmdFull;
                    pa.StartInfo.Arguments = "-x --audio-format mp3" + " --output " + output + " " + "-a " + BatTXT;
                    pa.StartInfo.UseShellExecute = false;
                    pa.StartInfo.RedirectStandardOutput = true;
                    pa.StartInfo.RedirectStandardError = true;
                    pa.StartInfo.CreateNoWindow = true;
                    pa.OutputDataReceived += proc_OutputDataRecieved;
                    pa.ErrorDataReceived += proc_ErrorDataRecieved;
                    pa.EnableRaisingEvents = true;
                    pa.Exited += new EventHandler(myProcess_Exited);
                    pa.Start();
                    pa.BeginOutputReadLine();
                    pa.BeginErrorReadLine();
                }
            }
            else if (cookie != "")
            {
                string CookOut = System.IO.Path.Combine(cmdFull + " --cookies " + cookie + " --output " + output + " " + "-a " + BatTXT);
                Console.WriteLine("[YDDL] Executing Batch Command: " + CookOut);
                Process pas = new Process();
                pas.StartInfo.FileName = cmdFull;
                pas.StartInfo.Arguments = "--cookies " + cookie + " --output " + output + " " + "-a " + BatTXT;
                pas.StartInfo.UseShellExecute = false;
                pas.StartInfo.RedirectStandardOutput = true;
                pas.StartInfo.RedirectStandardError = true;
                pas.StartInfo.CreateNoWindow = true;
                pas.OutputDataReceived += proc_OutputDataRecieved;
                pas.ErrorDataReceived += proc_ErrorDataRecieved;
                pas.EnableRaisingEvents = true;
                pas.Exited += new EventHandler(myProcess_Exited);
                pas.Start();
                pas.BeginOutputReadLine();
                pas.BeginErrorReadLine();
            }
            else
            {
                string Out = System.IO.Path.Combine(cmdFull + " --output " + output + " " + "-a " + BatTXT);
                Console.WriteLine("[YDDL] Executing Batch Command: " + Out);
                Process pass = new Process();
                pass.StartInfo.FileName = cmdFull;
                pass.StartInfo.Arguments = "--output " + output + " " + "-a " + BatTXT;
                pass.StartInfo.UseShellExecute = false;
                pass.StartInfo.RedirectStandardOutput = true;
                pass.StartInfo.RedirectStandardError = true;
                pass.StartInfo.CreateNoWindow = true;
                pass.OutputDataReceived += proc_OutputDataRecieved;
                pass.ErrorDataReceived += proc_ErrorDataRecieved;
                pass.EnableRaisingEvents = true;
                pass.Exited += new EventHandler(myProcess_Exited);
                pass.Start();
                pass.BeginOutputReadLine();
                pass.BeginErrorReadLine();
                //MessageBox.Show("Wait for the CMD Window to Close, then check output for file");

            }
        }

        private void Clear_Bat_Butt_Click(object sender, RoutedEventArgs e)
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var BatTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\batchdown.txt");
            File.Delete(BatTXT);
            OutBat_TXT.Clear();
            ount = 0;
        }

        private void saveBat_Butt_Click(object sender, RoutedEventArgs e)
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var prgCMD = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\Batch Saves\");
            var BatTXT = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl\batchdown.txt");
            string dateTime = DateTime.Now.ToString("MM-dd-yyyy h-mm tt");
            string fileDes = prgCMD + dateTime + ".txt";
            if (File.Exists(BatTXT))
            {
                if (new FileInfo(BatTXT).Length <= 2)
                {
                    Console.WriteLine("[YDDL] Batch List is Empty...");
                }
                else
                {
                    if (!Directory.Exists(prgCMD))
                    {
                        Directory.CreateDirectory(prgCMD);
                        File.Copy(BatTXT, fileDes);
                        Console.WriteLine("[YDDL] Saved Batch List at: " + fileDes);
                    }
                    else
                    {
                        File.Copy(BatTXT, fileDes);
                        Console.WriteLine("[YDDL] Saved Batch List at: " + fileDes);
                    }
                }
            }
            else
            {
                Console.WriteLine("[YDDL] Batchdown.txt does not exist...");
            }
            
        }

        private void up_prg_btt_Click(object sender, RoutedEventArgs e)
        {
            var pathWithEnv = @"%USERPROFILE%\Appdata\roaming";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv + @"\vddl");
            var commonbugsTXT = filePath + @"\common-bugs.txt";
            var readmeTXT = filePath + @"\readme.txt";
            File.Delete(commonbugsTXT);
            File.Delete(readmeTXT);
            Console.WriteLine("[YDDL] Calling Downloader...");
            InitializeDownload();

        }

        private void ip_butt_Click(object sender, RoutedEventArgs e)
        {
            InitializeIP();
        }
    }

}
