using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class ReadSettingsFile
    {
        public Dictionary<string, string> GlobalSettingsDictionary { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Takes in a path to a global settings file in a farm and processes the data. The results are stored in a dictionary, with key to value pairs.
        /// The data is split using a comma in the example format: "Output Directory, :c\Holos..."
        /// If the file is not found, throw a FileNotFoundException
        /// </summary>
        public void ReadGlobalSettings(string pathToGlobalSettingsFileInAFarm)
        {
            try
            {
                using (FileStream fs = new FileStream(pathToGlobalSettingsFileInAFarm, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line.StartsWith("#") || String.IsNullOrWhiteSpace(line))
                            {

                                continue;

                            }

                            else
                            {
                                if (line.Contains("("))
                                {
                                    var startIndex = line.IndexOf('(');
                                    var countToRemove = line.LastIndexOf(')') - startIndex + 1;
                                    line = line.Remove(startIndex, countToRemove);

                                }
                                var keyAndValueForGlobalSettings = line.Split('=');
                                GlobalSettingsDictionary.Add(keyAndValueForGlobalSettings[0].Trim(), keyAndValueForGlobalSettings[1].Trim());
                            }

                        }

                    }
                }
            }

            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Source: {0}", ex.Source);
                throw new FileNotFoundException();
            }
        }

      
    }
}
