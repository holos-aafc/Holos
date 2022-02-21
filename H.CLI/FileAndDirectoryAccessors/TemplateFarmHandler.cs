using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models;
using H.Core.Providers;
using H.Core.Providers.Soil;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class TemplateFarmHandler
    {
        #region Fields

        private DirectoryHandler directoryHandler = new DirectoryHandler();
        private TemplateFileHandler templateFileHandler = new TemplateFileHandler();

        #endregion

        #region Public Methods

        /// <summary>
        /// Takes in the path to the Farms directory and determines if the template Farm folder exists
        /// If it does not, create a template Farm Folder and Populate it with Component Directories, 
        /// Template FIles and a Settings file.
        /// </summary>
        public void CreateTemplateFarmIfNotExists(string pathToFarmsDirectory, GeographicDataProvider geographicDataProvider)
        {
            var exampleFarmDirectoryPath = pathToFarmsDirectory + @"\" + Properties.Resources.HolosExampleFarm;
    
            if (!Directory.Exists(exampleFarmDirectoryPath))
            {
                Directory.CreateDirectory(exampleFarmDirectoryPath);
            }

            var lethbridgePolygonID = 793006;
            var lethbridgeGeographicData = geographicDataProvider.GetGeographicalData(lethbridgePolygonID);
            directoryHandler.InitializeDirectoriesAndFilesForComponents(exampleFarmDirectoryPath);
            directoryHandler.GenerateGlobalSettingsFile(exampleFarmDirectoryPath, new Farm() { PolygonId = lethbridgePolygonID, GeographicData = lethbridgeGeographicData});
        } 
        #endregion
    }
}