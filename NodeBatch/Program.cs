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



        static void Main(string[] args)
        {
            // get current directory
            CurrentDir = System.IO.Directory.GetCurrentDirectory();
            
            // processInfo
            //var command = "/C node app.js";
            //System.Diagnostics.Process.Start("CMD.exe", command);

            SetPfxInfo();
            GenerateBatchFile(PfxPath, PfxPassword);

            Console.Read();


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
                if (!flag) Console.WriteLine("... OK");
            }
        }

        public static void GenerateBatchFile(string path, string pass)
        {
            const string homedrive = "set HOMEDRIVE=C:";
            var dir = "cd " + CurrentDir;       // or just "cd %~dp0"
            const string command = "node app.js";

            StreamWriter writer = new StreamWriter("test.bat");
            writer.WriteLine(homedrive);
            writer.WriteLine(dir);
            writer.WriteLine(command);
            writer.Close();
        }
    }
}
