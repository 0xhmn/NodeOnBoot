using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace NodeOnBoot
{
    class WinStartupRegistry
    {
        public static RegistryKey RegKey { get; set; }
        public static string test { get; set; }

        static WinStartupRegistry()
        {
            RegKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        }

        // test the current startup programs
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

    }
}
