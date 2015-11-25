using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeOnBoot
{
    class ServerSetup
    {
        public static string PfxPath { get; set; }
        public static string PfxPassword { get; set; }
        public static bool DevMode { get; set; } = true;
        public static bool SslMode { get; set; } = false;
        public static string NodeServerPath { get; set; }
        public static bool UsingPm2 { get; set; } = true;
        public static bool UsingNode { get; set; } = false;

        public static void Pm2OrNode()
        {
            Console.WriteLine("Please selecet an option (1 or 2) - (default: Pm2)");
            Console.WriteLine("Note that if you want to use a SSL Certificate you need to choose Pm2");
            do
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n\tNode\t[1]\n\tPm2\t[2]\n");
                Console.ResetColor();
                var ans = Console.ReadLine();
                if (ans == "1")
                {
                    UsingNode = true;
                    UsingPm2 = false;
                    return;
                }
                else if (ans == "2")
                {
                    UsingNode = false;
                    UsingPm2 = true;
                    return;
                }
                Console.WriteLine("1 or 2");
            } while (true);
        }

        public static void SetNodeServerPath()
        {
            Console.WriteLine("\nEnter your Node Server Path (www file). e.g. server\\bin\\www:");
            var path = Console.ReadLine();
            if (string.IsNullOrEmpty(path)) return;
            NodeServerPath = path;
            Console.WriteLine("New path has been set");
        }

        public static void GetSSLInfo()
        {
            do
            {
                Console.WriteLine("\nUsing SSL config? (y/n)");
                var ans = Console.ReadLine();
                if (ans == "n") break;
                else if (ans == "y")
                {
                    SetPfxInfo();
                    SslMode = true;
                    break;
                }
                else { Console.WriteLine("y or n"); }
            } while (true);

            do
            {
                Console.WriteLine("Development Mode? (y/n)");
                var ans = Console.ReadLine();
                if (ans == "n")
                {
                    DevMode = false;
                    break;
                }
                else if (ans == "y") break;
                Console.WriteLine("y or n");
            } while (true);
        }

        public static void SetPfxInfo()
        {
            var flag = true;
            while (flag)
            {
                Console.WriteLine("Enter you PFX file path:");
                PfxPath = Console.ReadLine();
                Console.WriteLine("Enter you PFX password:");
                PfxPassword = Console.ReadLine();

                flag = (string.IsNullOrEmpty(PfxPath) || PfxPath.Length == 0 || string.IsNullOrEmpty(PfxPassword) ||
                        PfxPassword.Length == 0);
            }
        }


        public static void CheckRootDir()
        {
            if (File.Exists(Path.Combine(ProjectConfig.GetCurrentDir(), "package.json"))) return;
            Console.WriteLine("Make sure you have copied the file inside the Project's Root" +
                              ", then try again");

            Console.ReadKey();
            Environment.Exit(0);
        }

        public static void RunPm2(string path, string pass)
        {
            string command = null;

            if (DevMode)
            {
                command = SslMode ? $"/C pm2 start {ProjectConfig.GetFullPath(NodeServerPath)} --  -p {pass} -a \"{path}\" && pm2 save" : $"/C pm2 start {ProjectConfig.GetFullPath(NodeServerPath)} && pm2 save";
            }
            // production mode
            else
            {
                command = SslMode ? $"/C pm2 start {ProjectConfig.GetFullPath(NodeServerPath)} -i 0 --  -p {pass} -a \"{path}\" && pm2 save" : $"/C pm2 start {ProjectConfig.GetFullPath(NodeServerPath)} -i 0 && pm2 save";
            }

            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.Arguments = command;
            process.StartInfo = info;
            process.Start();

            // generate batch file and register
            var batchFilePath = WinStartupRegistry.GenerateBatchFile(null, true);
            WinStartupRegistry.RegisterWithNode(batchFilePath);
        }

        public static void StopPm2()
        {
            string command = "/C pm2 stop all && pm2 delete all";
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.Arguments = command;
            process.StartInfo = info;
            process.Start();
        }

        public static void RunNode()
        {
            var serverFullPath = ProjectConfig.GetFullPath(NodeServerPath);

            string command = $"/C node {serverFullPath}";
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.Arguments = command;
            process.StartInfo = info;
            process.Start();

            // make batch file and register
            var batchFilePath = WinStartupRegistry.GenerateBatchFile(serverFullPath, false);
            WinStartupRegistry.RegisterWithNode(batchFilePath);
        }


    }
}
