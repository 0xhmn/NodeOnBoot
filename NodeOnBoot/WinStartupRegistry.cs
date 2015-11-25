using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace NodeOnBoot
{
    class WinStartupRegistry
    {

        public static RegistryKey RegKey { get; set; }


        // check if there is any node server startup on registry
        public static bool RegStartupCheck()
        {
            var files = GetCurrentStartups();
            var rgx = new Regex(@"(?i:node)");
            if (!files.Any(f => rgx.IsMatch(f))) return false;

            do
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING:");
                Console.WriteLine("There is already some Node server on your registry startup entry");
                Console.WriteLine("Do you want to delete them? (y/n)");
                Console.ResetColor();
                var ans = Console.ReadLine();
                if (ans == "n")
                {
                    return false;
                } else if (ans == "y")
                {
                    foreach (var file in files.Where(file => rgx.IsMatch(file)))
                    {
                        Console.WriteLine(" > " + file + " is deleted");
                        DeleteRegkey(file);
                    }
                    return true;
                }
            } while (true);
        }

        // check if there is any previous registry entry for current project in NodeOnBoot folder
        public static bool NodeOnBootDirStartupCheck()
        {
            var dirName = ProjectConfig.GetBatchDirecotry();
            if (!Directory.Exists(dirName)) return false;
            
            FileInfo[] files = new DirectoryInfo(dirName).GetFiles("*.bat");
            if (files.Length == 0) return false;

            do
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING:");
                Console.WriteLine("There is already some batch files in NodeToBood Folder:");
                Console.WriteLine("Do you want to proceed? Those file will be deleted (y/n)");
                Console.ResetColor();
                var ans = Console.ReadLine();
                if (ans == "y")
                {
                    foreach (var file in files)
                    {
                        Console.WriteLine(" > " + file.Name + "is deleted.");
                        DeleteRegkey(file.Name);
                        File.Delete(file.FullName);

                    }
                    return true;
                }
                else if (ans == "n")
                {
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            } while (true);
        }

        public static bool DeleteRegkey(string name)
        {
            if (!ExistsInReg(name))
            {
                return false;
            }
            RegKey.DeleteValue(name);
            return true;
        }

        static WinStartupRegistry()
        {
            RegKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        }

        public static List<string>  GetCurrentStartups()
        {
            if (RegKey == null) return null;
            var subKeyNames = RegKey.GetSubKeyNames();  // doesn't have any subkey
            var subValNames = RegKey.GetValueNames();

            return subValNames.ToList();
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
