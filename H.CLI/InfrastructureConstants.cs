using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI
{
    public static class InfrastructureConstants
    {
        public static string BaseOutputDirectoryPath { get; set; }
        public static string CommandLinePromptPrefix = "--> ";

        public static void CheckOutputDirectoryPath(string givenPath, string farmsFoldersPath)
        {
            string rootOfGivenPath = Path.GetPathRoot(givenPath);
            DriveInfo driveInfo = new DriveInfo(rootOfGivenPath);

            if (givenPath == "")
            {
                BaseOutputDirectoryPath = farmsFoldersPath;
                return;
            }
            if (driveInfo.DriveType == DriveType.Network)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Properties.Resources.CannotWriteToNetworkDrive, rootOfGivenPath);
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            if (Path.GetPathRoot(givenPath) == Path.GetPathRoot(farmsFoldersPath))
            {
                BaseOutputDirectoryPath = givenPath;
            }
        }
    }
}

