using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace ZAntiCheatClient
{

    public partial class Form1 : Form
    {
        bool exited = false;
        public bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        [DllImport("ZAntiCheat.dll")]
        public static extern bool InstallDriver();
        [DllImport("ZAntiCheat.dll")]
        public static extern bool UninstallDriver();
        [DllImport("ZAntiCheat.dll")]
        public static extern bool ProcessProtect(int pid);
        [DllImport("ZAntiCheat.dll")]
        public static extern bool StopProcessProtect();
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();
        public Form1()
        {
            InitializeComponent();
            CommandLineProcessor.getVerifyMD5();
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://bobh.zendee.cn/");
        }
        private static bool gok = true;
        private static bool firstPost = true;
        private static bool firstInit = true;
        private void myINIT() {
            Thread.Sleep(1000);
            if (!IsAdministrator())
            {
                MessageBox.Show("请以管理员身份运行ZAntiCheat!");
                Application.Exit();
            }
            if (!InstallDriver())
            {
                MessageBox.Show("组件加载失败，请尝试关闭杀毒软件并以管理员运行ZAntiCheat!");
                Application.Exit();
            }
        }
        private int lastid = -1;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (firstInit) {
                firstInit = false;
                myINIT();
            }
            string serverStatus = Network.getServerStatus();
            if (serverStatus.Contains("normal"))
            {
                label6.Text = "验证服务器状态:正常";
                label6.ForeColor = Color.Green;
            }
            else if (serverStatus.Contains("stop"))
            {
                label6.Text = "验证服务器状态:停止工作";
                label6.ForeColor = Color.Red;
            }
            else
            {
                label6.Text = "验证服务器状态:未知";
                label6.ForeColor = Color.Gray;
            }
            bool ok = true;
            bool hasclient = false;
            bool hasCheatClient = false;
            Process[] ps = Process.GetProcesses();
            int clientcnt = 0;
            foreach (Process p in ps)
            {
                if (p.Id == lastid) hasclient = true;
                
                if (p.ProcessName == "java")
                {
                    //Shell.WriteLine("{0} : {1}", p.ProcessName, p.Id);
                    //Shell.WriteLine("{0}",ProcessExtensions.GetCommandLineArgs(p));
                    string cline = ProcessExtensions.GetCommandLineArgs(p);
                    //Shell.WriteLine("test:" + p.MainWindowTitle);
                    if (p.MainWindowTitle.Contains("LiquidBounce")) {
                        //Shell.WriteLine("LiquidBounce!!!");
                        hasCheatClient = true;
                        ok = false;
                    }
                    if (p.Id == lastid && p.MainWindowTitle != "" && !p.MainWindowTitle.Contains("Minecraft 1.8.9")) {
                        //Shell.WriteLine("Minecraft 1.8.9!!Title:{0}", p.MainWindowTitle);
                        hasCheatClient = true;
                        ok = false;
                    }
                    if (CommandLineProcessor.isMCProcess(cline))
                    {
                        //Shell.WriteLine(p.MainWindowTitle);
                        //Shell.WriteLine("{0} : {1}", p.ProcessName, p.Id);

                        if (!CommandLineProcessor.isVailidClient(cline))
                        {
                            ok = false;
                            hasCheatClient = true;
                            break;
                        }
                        else
                        {
                            Shell.WriteLine("{0} : {1}", p.ProcessName, p.Id);
                            clientcnt++;
                            hasclient = true;
                            string uname = CommandLineProcessor.getClientName();
                            label4.Text = "当前状态:欢迎您 " + uname;
                            label4.ForeColor = Color.Green;
                            if (p.Id != lastid) {
                                StopProcessProtect();
                                if (!ProcessProtect(p.Id)) Shell.WriteLine("Protect Process Failed");
                                lastid = p.Id;
                            }
                            
                        }
                    }
                }
            }
            
            if (!hasclient)
            {
                ok = false;
                label4.Text = "当前状态:等待游戏启动";
                label4.ForeColor = Color.Black;
            }
            
            if (ok && gok && !firstPost && Network.getCanexit(CommandLineProcessor.getClientUUID()))
            {
                exited = true;
                //Application.Exit();
            }
            if (ok && gok)
            {
                //network upload uuid
                //Shell.WriteLine("OK");
                string res = Network.UUIDBeat(CommandLineProcessor.getClientUUID());
                firstPost = false;
                //Shell.WriteLine(res);
            }
            else if (hasCheatClient || clientcnt > 1)
            {
                Shell.WriteLine("hasCC:{0} clientcnt:{1}",hasCheatClient.ToString(),clientcnt.ToString());
                Network.setCancelUUID(CommandLineProcessor.getClientUUID());
                Shell.WriteLine("错误:有其他客户端启动，即将退出");
                timer1.Enabled = false;
                label4.Text = "当前状态:作弊检测!";
                label4.ForeColor = Color.Red;
                MessageBox.Show("错误:有其他客户端启动，即将退出,请关闭其他客户端后再试");
                gok = false;
                Application.Exit();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UninstallDriver();
        }
    }
    static class Shell
    {
        /// <summary>  
        /// 输出信息  
        /// </summary>  
        /// <param name="format"></param>  
        /// <param name="args"></param>  
        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        /// <summary>  
        /// 输出信息  
        /// </summary>  
        /// <param name="output"></param>  
        public static void WriteLine(string output)
        {
            Console.ForegroundColor = GetConsoleColor(output);
            Console.WriteLine(@"[{0}]{1}", DateTimeOffset.Now, output);
        }

        /// <summary>  
        /// 根据输出文本选择控制台文字颜色  
        /// </summary>  
        /// <param name="output"></param>  
        /// <returns></returns>  
        private static ConsoleColor GetConsoleColor(string output)
        {
            if (output.StartsWith("警告")) return ConsoleColor.Yellow;
            if (output.StartsWith("错误")) return ConsoleColor.Red;
            if (output.StartsWith("注意")) return ConsoleColor.Green;
            return ConsoleColor.Gray;
        }
    }
    public static class ProcessExtensions
    {
        /// <summary>
        /// 获取一个正在运行的进程的命令行参数。
        /// 与 <see cref="Environment.GetCommandLineArgs"/> 一样，使用此方法获取的参数是包含应用程序路径的。
        /// 关于 <see cref="Environment.GetCommandLineArgs"/> 可参见：
        /// [.NET 命令行参数包含应用程序路径吗？](https://walterlv.com/post/when-will-the-command-line-args-contain-the-executable-path.html)
        /// </summary>
        /// <param name="process">一个正在运行的进程。</param>
        /// <returns>表示应用程序运行命令行参数的字符串。</returns>
        public static string GetCommandLineArgs(this Process process)
        {
            if (process is null) throw new ArgumentNullException(nameof(process));

            try
            {
                return GetCommandLineArgsCore();
            }
            catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
            {
                // 没有对该进程的安全访问权限。
                return string.Empty;
            }
            catch (InvalidOperationException)
            {
                // 进程已退出。
                return string.Empty;
            }

            string GetCommandLineArgsCore()
            {
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                using (var objects = searcher.Get())
                {
                    var @object = objects.Cast<ManagementBaseObject>().SingleOrDefault();
                    return @object?["CommandLine"]?.ToString() ?? "";
                }
            }
        }
    }
}
