using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;
using H.CLI.Factorys;
using System.Text.RegularExpressions;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class TemplateFileHandler
    {
        #region Fields
        private ComponentKeyFactory _componentKeyFactory = new ComponentKeyFactory();
        private ExcelInitializer _excel = new ExcelInitializer();
        private DirectoryKeys _directoryKeys = new DirectoryKeys();
       
        #endregion


        #region Public Methods

        /// <summary>
        /// Responsible for getting the component type from the filePath. This method is used in the validateTemplateFiles 
        /// method below to create the appropriate template files for each component directory. It is also used in our ParserHandler
        /// to initialize the ParserStrategy object (concrete ComponentKey and TemporaryInput)
        /// Determines if there is a match associated with the key(case insensitive).
        /// </summary>
        public string checkFileNameForComponentType(string filePath)
        {
            foreach (var key in _directoryKeys.directoryWeights.Keys)
            {
                var searchKey = key.Remove(key.LastIndexOf('s'));
                var match = Regex.Match(filePath, "(?i)" + searchKey);
                if (match.Success)
                {
                    return key;
                }

                else
                    continue;
            }
            return null;
        }


        /// <summary>
        /// For each component directory in the corresponding farmDirectoryPath, get the list of all the files. If there is no file that contains
        /// "_Example", than create a new template file based on the component type (retrieved from the fileName). The corresponding
        /// ComponentKey will also be set based on the component type and passed in to the SetTemplateExcelFile method.
        /// </summary>
        public void validateTemplateFiles(List<string> validComponentDirectoryPaths)
        {
            
            foreach (var componentDirectoryPath in validComponentDirectoryPaths)
            {
                var files = Directory.GetFileSystemEntries(componentDirectoryPath);
                var isATemplateFileList = files.Where(file => file.ToUpper().Contains(Path.GetFileName(String.Format(componentDirectoryPath) + "_Example").ToUpper()));
                if (!isATemplateFileList.Any())
                {
                    var componentType = _directoryKeys.directoryWeights.SingleOrDefault(x => x.Key.ToUpper() == Path.GetFileName(componentDirectoryPath).ToUpper()).Key;  
                    var componentKey = _componentKeyFactory.ComponentKeysCreator(componentType);
                    _excel.SetTemplateFile(componentDirectoryPath, componentType, componentKey.keys);
                }
                else
                    continue;
            }
        } 
        #endregion
    }
}
