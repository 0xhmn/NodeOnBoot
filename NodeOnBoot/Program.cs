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




        static void Main(string[] args)
        {
            Figlet();

            // CHECKING THE REGISTRY
            WinStartupRegistry.NodeOnBootDirStartupCheck();
            WinStartupRegistry.RegStartupCheck();
            
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
                Console.WriteLine("\n... Node is installed\n");
                Console.ResetColor();
            }

            ServerSetup.Pm2OrNode();

            if (ServerSetup.UsingPm2)
            {
                Dependencies.InstallNpm();  // only if pm2 is selected
            }

            #endregion

            #region SETTING UP

            //CheckRootDir();
            ServerSetup.SetNodeServerPath();
            if (ServerSetup.UsingPm2)
            {
                ServerSetup.GetSSLInfo();
                ServerSetup.RunPm2(ServerSetup.PfxPath, ServerSetup.PfxPassword);
            }
            else
            {
                ServerSetup.RunNode();
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done!");
            Console.ReadKey();
            #endregion
            
        }

        public static void Figlet()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"

   ,mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
  ]MMPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPMMb
  ]MM   _   _           _       ___        ____              _     dMb
  ]MM  | \ | | ___   __| | ___ / _ \ _ __ | __ )  ___   ___ | |_   dMb
  ]MM  |  \| |/ _ \ / _` |/ _ \ | | | '_ \|  _ \ / _ \ / _ \| __|  dMb
  ]MM  | |\  | (_) | (_| |  __/ |_| | | | | |_) | (_) | (_) | |_   dMb
  ]MM  |_| \_|\___/ \__,_|\___|\___/|_| |_|____/ \___/ \___/ \__|  dMb
  ]MM                                                              dMb
  ]MMmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmMMb
   ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ");
            Console.ResetColor();
        }
    }
}
