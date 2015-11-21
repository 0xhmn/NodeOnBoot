using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NodeBatch
{
    class GetNssm
    {
        public static bool HaveNssm()
        {
            // search through chocolatey libraries
            var rooDirectories =  Directory.GetDirectories(@"C:\ProgramData\chocolatey\lib");
            var rgx = new Regex(@"(?i:nssm)");
            return rooDirectories.Any(dir => rgx.IsMatch(dir));
        }

        public static bool DownloadNssm()
        {
            const string command = "/C choco install nssm";
            System.Diagnostics.Process.Start("CMD.exe", command);
            return true;
        }
    }
}
