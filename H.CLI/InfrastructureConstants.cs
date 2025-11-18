using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;
using H.CLI.Properties; 

namespace H.CLI
{
    public static class InfrastructureConstants
    {
        public static string BaseOutputDirectoryPath { get; set; }
        public static string CommandLinePromptPrefix = "--> ";

        public static bool CheckOutputDirectoryPath(string givenPath, IDriveInfoWrapper givenPathDriveInfo, string farmsFoldersPath)
        {
            if (string.IsNullOrEmpty(givenPath))
            {
                if (!Directory.Exists(farmsFoldersPath))
                {
                    Directory.CreateDirectory(farmsFoldersPath);
                    BaseOutputDirectoryPath = Path.Combine(farmsFoldersPath, Properties.Resources.Outputs);

                    return false;
                }

                var existingDirectories = Directory.GetDirectories(farmsFoldersPath);
                var outputFolderExists = existingDirectories.Any(x => x.IndexOf(Properties.Resources.Outputs, StringComparison.InvariantCultureIgnoreCase) >= 0);
                if (outputFolderExists)
                {
                    var newOutputDirectory = Path.Combine(farmsFoldersPath, Properties.Resources.Outputs + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
                    Directory.CreateDirectory(newOutputDirectory);
                    BaseOutputDirectoryPath = newOutputDirectory;
                }
                else
                {
                    BaseOutputDirectoryPath = Path.Combine(farmsFoldersPath, Properties.Resources.Outputs);
                }

                return false;
            }
            else if (givenPathDriveInfo.DriveType == DriveType.Network)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Properties.Resources.CannotWriteToNetworkDrive);
                Console.ForegroundColor = ConsoleColor.White;
                BaseOutputDirectoryPath = farmsFoldersPath;
                return false;
            }
            else
            {
                BaseOutputDirectoryPath = givenPath;
                return true;
            }
        }
    }
}

