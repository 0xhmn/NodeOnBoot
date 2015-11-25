using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace NodeOnBoot
{
    class WinStartupRegistry
    {
        public static RegistryKey RegKey { get; set; }

        static WinStartupRegistry()
        {
            RegKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        }

        public static bool GetCurrentStartups()
        {
            if (RegKey == null) return false;
            var subKeyNames = RegKey.GetSubKeyNames();  // doesn't have any subkey
            var subValNames = RegKey.GetValueNames();

            foreach (var val in subValNames)
            {
                Console.WriteLine(val);
            }
            return true;
        }

        /// <summary>
        /// setting up key and value on win registry
        /// name as key and path as value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool SetKeyVal(string name, string path)
        {
            if (RegKey == null) return false;
            if (!ExistsInReg(name))
            {
                RegKey.SetValue(name, path);
                return true;
            }
            // override if the name exists
            RegKey.DeleteValue(name);
            RegKey.SetValue(name, path);
            return false;
        }

        public static bool ExistsInReg(string name)
        {
            var subValueNames = RegKey.GetValueNames();
            return subValueNames.Any(v => v.Equals(name));
        }

        public static bool RegisterWithNode(string batchFilePath)
        {
            var fileName = System.IO.Path.GetFileName(batchFilePath);
            SetKeyVal(fileName, batchFilePath);

            return true;
        }

        // generate a batch file with unique name
        public static string GenerateBatchFile(string fullJsPath, bool usePm2)
        {
            var command = usePm2 ? "pm2 resurrect" : $"node {fullJsPath}";

            var dirName = ProjectConfig.GetBatchDirecotry();
            var fileName = "NodeSetup_" + System.IO.Path.GetRandomFileName() + ".bat";
            var batchFilePath = Path.Combine(dirName, fileName);
            if (!File.Exists(batchFilePath))
            {
                using (var writer = new StreamWriter(batchFilePath))
                {
                    writer.WriteLine(command);
                }
            }
            else
            {
                Console.WriteLine("Another confige file exists, do you want to overwrite the file?(y/n)");
                if (Console.ReadLine() == "y")
                {
                    using (var writer = new StreamWriter(batchFilePath, false)) // overwrite the existing batch
                    {
                        writer.WriteLine(command);
                    }
                }      
            }
            return ProjectConfig.GetFullPath(batchFilePath);
        }
    }
}
