using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using H.Core.Models;
using H.Core.Tools;
using H.Infrastructure;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace H.Core
{
    public class Storage : BindableBase
    {
        #region Fields

        private ApplicationData _applicationData;

        private const string StorageFileName = "json-data.json";
        private const string exportedFileExtension = ".json";

        private string _baseCrashFileName = $"holos-crash-{DateTime.Now}.json";

        #endregion

        #region Constructors

        public Storage()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Properties
        public ApplicationData ApplicationData
        {
            get { return _applicationData; }
            set { this.SetProperty(ref _applicationData, value); }
        }

        #endregion

        #region Public Methods

        public void Save()
        {
            try
            {
                var path = GetFullPathToStorageFile();
                this.SaveInternal(path);
            }
            catch (Exception e)
            {
                // Saving to the expected file location might fail if the file is already opened by another process (backup software etc.)

                // Save to alternate file instead during exception

                var path = DateTime.Now.ToString() + "_" + GetFullPathToStorageFile();
                this.SaveInternal(path);
            }
        }

        public void ClearStorage()
        {
            if (File.Exists(this.GetFullPathToStorageFile()))
            {
                File.Delete(this.GetFullPathToStorageFile());
            }
        }

        public string GetFullPathToStorageFile()
        {
            var userFilePath = this.GetUserFilePath();

            if (!Directory.Exists(userFilePath))
            {
                Directory.CreateDirectory(userFilePath);
            }

            var destination = Path.Combine(userFilePath, StorageFileName);

            return destination;
        }

        private void SaveInternal(string path)
        {
            // Use streams instead of JsonConvert.SerializeObject to prevent OutOfMemoryExceptions
            using (StreamWriter fileStream = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();

                // Serializer and deserializer must both have this set to Auto
                serializer.TypeNameHandling = TypeNameHandling.Auto;

                serializer.Serialize(fileStream, this.ApplicationData, typeof(ApplicationData));
            }
        }

        public void Load()
        {
            try
            {
                // Use streams instead of File.ReadAllText() to prevent OutOfMemoryExceptions when reading large files
                using (StreamReader r = new StreamReader(this.GetFullPathToStorageFile()))
                {
                    using (JsonReader reader = new JsonTextReader(r))
                    {
                        JsonSerializer serializer = new JsonSerializer();

                        // Serializer and deserializer must both have this set to Auto
                        serializer.TypeNameHandling = TypeNameHandling.Auto;

                        this.ApplicationData = serializer.Deserialize<ApplicationData>(reader);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Trace.TraceInformation("No storage file found. Building new storage object.");

                this.ApplicationData = new ApplicationData();
            }
            catch (Exception e)
            {
                // Write exception to file here
                this.WriteExceptionToFile(e);

                // Save the farm that can't be deserialized correctly to a crash file
                this.WriteCrashFile();
                File.Delete(this.GetFullPathToStorageFile());
                this.ApplicationData = new ApplicationData();
            }
        }

        private string GetUserFilePath()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userFilePath = Path.Combine(localAppData, "HOLOS_4");
            return userFilePath;
        }

        /// <summary>
        /// Save the farm file to a crash file when we can't load the farm file correctly
        /// </summary>
        public string WriteCrashFile()
        {
            var userFilePath = this.GetUserFilePath();
            _baseCrashFileName = _baseCrashFileName.Replace(':', '_');
            _baseCrashFileName = _baseCrashFileName.Replace('/', '_');
            var crashFilePath = Path.Combine(userFilePath, _baseCrashFileName);
            crashFilePath = crashFilePath.Replace(' ', '_');

            File.Copy(this.GetFullPathToStorageFile(), crashFilePath);

            return crashFilePath;
        }

        public void WriteExceptionToFile(Exception e)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userFilePath = Path.Combine(localAppData, "HOLOS_4");

            var fullpath = Path.Combine(userFilePath, "logfile.txt");

            if (!File.Exists(fullpath))
            {
                using (StreamWriter sw = File.CreateText(fullpath))
                {
                    sw.WriteLine(e.ToString());
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(fullpath))
                {
                    sw.WriteLine(e.ToString());
                }
            }
        }

        public IEnumerable<Farm> GetFarmsFromExportFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Enumerable.Empty<Farm>();
            }

            var jsonData = string.Empty;
            var success = true;

            try
            {
                jsonData = File.ReadAllText(fileName);
            }
            catch (FileNotFoundException)
            {
                success = false;
            }

            if (success)
            {
                ObservableCollection<Farm> farms = new ObservableCollection<Farm>();

                try
                {
                    /*
                     * Replace namespaces in old farm files so deserialization succeeds (some classes were moved from H project to H.Core so any old farm files will not deserialize without
                     * these adjustments.
                     */

                    jsonData = jsonData.Replace(@"H.Views.DetailViews.LandManagement.FieldSystem.FieldSystemDetailsStageState, H", "H.Core.Models.LandManagement.Fields.FieldSystemDetailsStageState, H.Core");
                    jsonData = jsonData.Replace(@"H.Models.LandManagement.Fields.FieldSystemComponent, H", "H.Core.Models.LandManagement.Fields.FieldSystemComponent, H.Core");
                    jsonData = jsonData.Replace(@"H.Models.LandManagement.Rotation.RotationComponent, H", "H.Core.Models.LandManagement.Rotation.RotationComponent, H.Core");

                    farms = JsonConvert.DeserializeObject<ObservableCollection<Farm>>(jsonData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                }
                catch (Exception e)
                {
                    Trace.TraceError($"{e.Message}");
                    if (e.InnerException != null)
                    {
                        Trace.TraceError($"{e.InnerException.ToString()}");
                    }
                }

                return farms;
            }

            return Enumerable.Empty<Farm>();
        }

        /// <summary>
        /// Imports farms from a directory. Searches contained directories as well.
        /// </summary>
        public IEnumerable<Farm> GetExportedFarmsFromDirectoryRecursively(string path)
        {
            var result = new List<Farm>();

            var stringCollection = new StringCollection();
            var files = FileSystemHelper.ListAllFiles(stringCollection, path, $"*{exportedFileExtension}", isRecursiveScan: true);
            if (files == null)
            {
                return result;
            }

            foreach (var file in files)
            {
                var farmsFromFile = this.GetFarmsFromExportFile(file);
                result.AddRange(farmsFromFile);
            }

            return result;
        }

        public void Import(List<Farm> farmsToImport)
        {
            // Check for name clashes
            foreach (var farmToImport in farmsToImport)
            {
                var importedFarmName = farmToImport.Name;
                if (this.ApplicationData.Farms.Any(x => x.Name.Equals(importedFarmName)))
                {
                    farmToImport.Name = farmToImport.Name + $"_Imported_{DateTime.Now.ToShortDateString()}";
                }

                // Assign a unique GUID since a user might export then import that same farm in which case the GUID would be the same - prevent this situation.
                farmToImport.Guid = Guid.NewGuid();
            }

            this.ApplicationData.Farms.AddRange(farmsToImport);
        }

        #endregion
    }
}