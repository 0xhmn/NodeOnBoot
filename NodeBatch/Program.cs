using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NodeBatch
{
    /// <summary>
    /// Generating a batch file based on SSH path and password
    /// </summary>
    class Program
    {
        public static string PfxPath { get; set; }
        public static string PfxPassword { get; set; }
        public static string CurrentDir { get; set; }
        public static string UserDir { get; set; }
        public static bool DevMode { get; set; } = true;
        public static string NodeServerPath { get; set; }



        static void Main(string[] args)
        {
            // get current directory
            CurrentDir = System.IO.Directory.GetCurrentDirectory();
            UserDir = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            SetNodeServerPath();
            GetSSHInfo();
            GenerateBatchFile(PfxPath, PfxPassword);

            Console.WriteLine("Done!");
            Console.Read();


        }

        public static void SetNodeServerPath()
        {
            Console.WriteLine("Enter your Node Server Path (www file). e.g. server\\bin\\www:");
            var path = Console.ReadLine();
            if (string.IsNullOrEmpty(path)) return;
            NodeServerPath = path;
            Console.WriteLine("New path has been set");
        }

        // get path and password
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

        public static void GenerateBatchFile(string path, string pass)
        {
            const string homeDrive = "set HOMEDRIVE=C:";
            string pm2Home = $"set PM2_HOME={UserDir}\\.pm2";
            var dir = "cd " + CurrentDir; // or just "cd %~dp0"
            string command;

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(pass))
            {
                command = $"node {NodeServerPath}";
            }
            else
            {
                if (DevMode)
                {
                    command = $"pm2 start server\\bin\\www --  -p {pass} -a \"{path}\"";
                }
                else
                {
                    command = $"pm2 start server\\bin\\www -i 0 --  -p {pass} -a \"{path}\"";
                }
            }

            const string dirName = "NodeBatch";
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            const string fileName = "test.bat";

            var filePath = Path.Combine(dirName, fileName);
            if (!File.Exists(filePath))
            {
                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(homeDrive);
                    writer.WriteLine(pm2Home);
                    writer.WriteLine(dir);
                    writer.WriteLine(command);
                }
            }
            else
            {
                Console.WriteLine("Another confige file exists, do you want to overwrite the file?(y/n)");
                if (Console.ReadLine() != "y") return;
                using (var writer = new StreamWriter(filePath, false)) // overwrite the existing batch
                {
                    writer.WriteLine(homeDrive);
                    writer.WriteLine(pm2Home);
                    writer.WriteLine(dir);
                    writer.WriteLine(command);
                }
            }

            

        }

        public static void GetSSHInfo()
        {
            Console.WriteLine("Using SSH config? (y/n)");
            if (Console.ReadLine() != "y") return;
            Console.WriteLine("Development Mode? (y/n)");
            if (Console.ReadLine() == "n")
            {
                DevMode = false;
            }

            SetPfxInfo();
        }

    }
}
