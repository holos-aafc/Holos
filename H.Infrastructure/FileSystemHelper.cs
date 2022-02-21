using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Infrastructure
{
    public static class FileSystemHelper
    {
        // List all the files in a folder (and its subfolders):
        // 'allFiles' is my StringCollection containing the complete list of all files
        // Why a StringCollection? Because arrays have a fixed size and using a StringCollection is the only way to list all the folder and subfolder files.
        // 'path' the selected folder
        //
        // 'ext' the list of extensions to filter the files listed
        //
        // 'isRecursiveScan' a boolean allowing me to accept subfolder scans
        //
        public static StringCollection ListAllFiles(StringCollection allFiles, string path, string ext, bool isRecursiveScan)
        {
            // listFilesCurrDir: Table containing the list of files in the 'path' folder
            string[] listFilesCurrDir;
            try
            {
                listFilesCurrDir = Directory.GetFiles(path, ext);
            }
            catch (Exception)
            {

                return null;
            }

            // read the array 'listFilesCurrDir'
            foreach (string rowFile in listFilesCurrDir)
            {
                // If the file is not already in the 'allFiles' list
                if (allFiles.Contains(rowFile) == false)
                {
                    // Add the file (at least its address) to 'allFiles'
                    allFiles.Add(rowFile);
                }
            }
            // Clear the 'listFilesCurrDir' table for the next list of subfolders
            listFilesCurrDir = null;

            // If you allow subfolders to be read
            if (isRecursiveScan)
            {
                // List all the subfolders present in the 'path'
                string[] listDirCurrDir = Directory.GetDirectories(path);

                // if there are subfolders (if the list is not empty)
                if (listDirCurrDir.Length != 0)
                {
                    // read the array 'listDirCurrDir'
                    foreach (string rowDir in listDirCurrDir)
                    {
                        // Restart the procedure to scan each subfolder
                        ListAllFiles(allFiles, rowDir, ext, isRecursiveScan);
                    }
                }

                // Clear the 'listDirCurrDir' table for the next list of subfolders
                listDirCurrDir = null;

            }
            // return 'allFiles'
            return allFiles;
        }

        public static  bool IsFileInUse(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("'path' cannot be null or empty.", "path");

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read)) { }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
    }
}
