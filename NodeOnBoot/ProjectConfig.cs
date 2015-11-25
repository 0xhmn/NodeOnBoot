using System;
using System.IO;

namespace NodeOnBoot
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

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
            {
                var f = new FileInfo(fileName);
                return f.FullName;
            }
            return null;
        }
    }
}
