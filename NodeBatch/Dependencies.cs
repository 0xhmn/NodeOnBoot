using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace NodeBatch
{
    class Dependencies
    {
        // check if Node.js is installed
        public static bool HasNode()
        {
            // check registry
            const string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            var regRgx = new Regex(@"(?i:node)");

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regKey))
            {
                foreach (string skName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(skName))
                    {
                        if (subkey?.GetValue("DisplayName") == null) continue;
                        var name = subkey.GetValue("DisplayName").ToString();
                        if (regRgx.IsMatch(name)) return true;
                    }
                }
            }

            // check directory
            var rooDirectories = Directory.GetDirectories(@"C:\Program Files");
            var dirRgx = new Regex(@"(?i:node)");
            foreach (var directory in rooDirectories)
            {
                if (dirRgx.IsMatch(directory)) return true;
            }


            return false;
        }

        public static void InstallNpm()
        {
            // install pm2
            string[] npmDependencies =
            {
                "/C npm install -g pm2",
                "/C npm install -g pm2-windows-startup && pm2-startup install"
            };

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            startInfo.FileName = "cmd.exe";
            foreach (var dependency in npmDependencies)
            {
                startInfo.Arguments = dependency;
                process.StartInfo = startInfo;
                process.Start();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("NPM Dependencies Are Installing");
            Console.WriteLine("Please wait until all of the dependencies are installed");
            Console.ResetColor();
        }



    }
}
