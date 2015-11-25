using System;
using System.Diagnostics;
using System.IO;

namespace NodeOnBoot
{
    /// <summary>
    /// Generating a batch file based on SSL path and password
    /// </summary>
    class Program
    {
        public static string PfxPath { get; set; }
        public static string PfxPassword { get; set; }
        public static bool DevMode { get; set; } = true;
        public static bool SslMode { get; set; } = false;
        public static string NodeServerPath { get; set; }
        public static bool UsingPm2 { get; set; } = true;
        public static bool UsingNode { get; set; } = false;



        static void Main(string[] args)
        {


            /**
            WinStartupRegistry.GetCurrentStartups();
            WinStartupRegistry.SetKeyVal("NodeTest", @"d:\Test\node_batch>\test.cmd");
            WinStartupRegistry.GetCurrentStartups();
            **/

            #region CHECKING DEPENDENCIES

            if (!Dependencies.HasNode())
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Couldn't find Node.js");
                Console.ResetColor();

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Node is installed");
                Console.ResetColor();
            }

            Pm2OrNode();

            if (UsingPm2)
            {
                Dependencies.InstallNpm();  // only if pm2 is selected
            }

            #endregion

            #region SETTING UP

            //CheckRootDir();
            SetNodeServerPath();
            if (UsingPm2)
            {
                GetSSLInfo();
                RunPm2(PfxPath, PfxPassword);
            }
            else
            {
                RunNode();
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done!");
            Console.ReadKey();
            #endregion
            
        }

        public static void Pm2OrNode()
        {
            Console.WriteLine("Which One Do You Want to Use? (defaule: Pm2)");
            Console.WriteLine("Note that if you want to use a SSL Certificates you need to choose Pm2");
            do
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  Node [1]\tPm2 [2]\n");
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
            Console.WriteLine("Enter your Node Server Path (www file). e.g. server\\bin\\www:");
            var path = Console.ReadLine();
            if (string.IsNullOrEmpty(path)) return;
            NodeServerPath = path;
            Console.WriteLine("New path has been set");
        }



        public static void GenerateBatchFile(string path, string pass)
        {

            // if pm2 -> 1/devmode 2/producMode





            const string homeDrive = "set HOMEDRIVE=C:";
            string pm2Home = $"set PM2_HOME={ProjectConfig.GetUserDir()}\\.pm2";
            var dir = "cd " + ProjectConfig.GetCurrentDir(); // or just "cd %~dp0"
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
            const string fileName = "NodeSetup.bat";

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

        public static void GetSSLInfo()
        {
            do
            {
                Console.WriteLine("Using SSL config? (y/n)");
                var ans = Console.ReadLine();
                if (ans == "n") break;
                else if (ans == "y")
                {
                    SetPfxInfo();
                    SslMode = true;
                    break;
                }
                else { Console.WriteLine("y or n");}
            } while (true);

            do
            {
                Console.WriteLine("Development Mode? (y/n)");
                var ans = Console.ReadLine();
                if (ans == "n")
                {
                    DevMode = false;
                    break;
                } else if (ans == "y") break;
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
