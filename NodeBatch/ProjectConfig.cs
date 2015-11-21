using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeBatch
{
    static class ProjectConfig
    {
        public static string GetCurrentDir()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        public static string GetUserDir()
        {
            return System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public static string GetRootFolder()
        {
            return System.IO.Directory.GetDirectoryRoot(System.IO.Directory.GetCurrentDirectory());
        }

        public static string GetNssmPath()
        {
            return Path.Combine(GetRootFolder(), "nssm-2.24");
        }
    }
}
