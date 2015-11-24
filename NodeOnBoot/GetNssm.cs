using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace NodeOnBoot
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
            const string installCommand = "/C choco install nssm";
            System.Diagnostics.Process.Start("CMD.exe", installCommand);
            return true;
        }

        public static bool StartSsm(string pathToNodeSetup)
        {
            if (string.IsNullOrEmpty(pathToNodeSetup)) return false;
            string startCommand = $"/C nssm install NodeService {pathToNodeSetup}";
            System.Diagnostics.Process.Start("CMD.exe", startCommand);
            return true;
        }

        public static bool StartService()
        {
            // start the service
            var controller = new ServiceController("NodeService");
            if (controller.Status == ServiceControllerStatus.Running)
            {
                Console.WriteLine("The Service Already was running");
            }
            controller.Start();
            return true;
        }
    }
}
