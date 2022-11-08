using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
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

        private const string _backupNamePrefix = "holos-backup-";
        private const string _backupDateFormat = "yyyy-MM-dd_hh_mm_ss_tt";
        private string _dataBackupFileName = $"{_backupNamePrefix}{DateTime.Now.ToString(_backupDateFormat)}.json";

        private const int MaxNumberOfBackups = 5;


        /// <summary>
        /// Returns True if a backup exists inside the backup directory of the user data folder.
        /// </summary>
        private bool _backupExists => _backupFilesInDirectory.Any();

        /// <summary>
        /// An array that stores the names of the holos backup files. The array is populated inside the <see cref="GetBackupFiles"/> method. The array
        /// is sorted in descending order based on file creation time. Therefore, the backups are stored oldest -> newest with the latest backup at index 0.
        /// </summary>
        private FileInfo[] _backupFilesInDirectory { get; set; }
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

        /// <summary>
        /// Returns True if the user data is successfully loaded.
        /// </summary>
        public bool IsDataLoaded { get; set; }


        /// <summary>
        /// Returns True if a backup file was restored to recover user data.
        /// </summary>
        public bool WasBackupRestored { get; set; } = false;

        /// <summary>
        /// Returns the time a particular backup was created
        /// </summary>
        public DateTime BackupCreationTime { get; set; }

        /// <summary>
        /// Determines if a save has been successfully completed.
        /// </summary>
        public bool HasSaveCompleted { get; set; }

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

                var path = GetFullPathToStorageFile().Replace(".json", "_" + DateTime.Now.ToString(_backupDateFormat) + ".json");
                this.SaveInternal(path);
            }
        }

        /// <summary>
        /// Saves the user data asynchronously by calling the <see cref="SaveInternalAsync"/> method.
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            try
            {
                var path = GetFullPathToStorageFile();
                await SaveInternalAsync(path);
            }
            catch (Exception)
            {
                var path = GetFullPathToStorageFile().Replace(".json", "_" + DateTime.Now.ToString(_backupDateFormat) + ".json"); 
                await SaveInternalAsync(path);
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
            var userFolderPath = this.GetUserFolderPath();

            if (!Directory.Exists(userFolderPath))
            {
                Directory.CreateDirectory(userFolderPath);
            }

            var destination = Path.Combine(userFolderPath, StorageFileName);

            return destination;
        }

        /// <summary>
        /// The method looks for the filename in the <see cref="_backupFilesInDirectory"/> array and returns the full path of the backup
        /// file based on the backup number provided.
        /// </summary>
        /// <param name="backupNumber">An 0 indexed backup number. The index is used to retrieve the backup name from <see cref="_backupFilesInDirectory"/></param>
        /// <param name="backupFileName">Returns the name of the backup file based on the backup number specified.</param>
        /// <returns>The full path to the backup file.</returns>
        private string GetFullPathToBackupFile(int backupNumber, out string backupFileName)
        {
            backupFileName = _backupFilesInDirectory.ElementAt(backupNumber).ToString();
            string pathToBackupFile = CreateFileLocationPath(backupFileName, isBackupPath: true);

            return pathToBackupFile;
        }

        /// <summary>
        /// Writes user data to a file synchronously. Uses streams instead of JsonConvert.SerializeObject to write user data
        /// prevent OutOfMemoryExceptions.
        /// </summary>
        /// <param name="path"></param>
        private void SaveInternal(string path)
        {
            using (StreamWriter fileStream = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();

                // Serializer and deserializer must both have this set to Auto
                serializer.TypeNameHandling = TypeNameHandling.Auto;

                serializer.Serialize(fileStream, this.ApplicationData, typeof(ApplicationData));
            }
        }


        /// <summary>
        /// Runs a task that serializes the user data asynchronously />
        /// </summary>
        /// <param name="path">The path to the file that needs to be serialized</param>
        /// <returns></returns>
        private async Task SaveInternalAsync(string path)
        {
            await Task.Run(() =>
            {
                using (StreamWriter fileStream = File.CreateText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    // Serializer and deserializer must both have this set to Auto
                    serializer.TypeNameHandling = TypeNameHandling.Auto;

                    serializer.Serialize(fileStream, this.ApplicationData, typeof(ApplicationData));
                    Trace.TraceInformation($"{nameof(Storage)}.{nameof(SaveInternalAsync)}: data serialization completed.");
                    HasSaveCompleted = true;
                }
            });
        }


        /// <summary>
        /// Uses <see cref="JsonReader"/> and <see cref="StreamReader"/> to read a .json file that contains user's saved data.
        /// </summary>
        /// <param name="pathToStorageFile">Complete path to the file that needs to be read</param>
        /// <returns>Returns an instance of Application Data after deserializing the local .json data file.</returns>
        private ApplicationData ReadDataFile(string pathToStorageFile)
        {
            // Use streams instead of File.ReadAllText() to prevent OutOfMemoryExceptions when reading large files
            using (StreamReader r = new StreamReader(pathToStorageFile))
            {
                using (JsonReader reader = new JsonTextReader(r))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    // Serializer and deserializer must both have this set to Auto
                    serializer.TypeNameHandling = TypeNameHandling.Auto;

                    return serializer.Deserialize<ApplicationData>(reader);
                }
            }
        }

        /// <summary>
        /// Tries to load the user's .json data file. If the data file cannot be loaded, the method checks if there
        /// are any backups available. If yes tries to load the most recent backup, otherwise it sets <see cref="ApplicationData"/> to a new instance
        /// of <see cref="Models.ApplicationData"/>
        /// </summary>
        public void Load()
        {
            GetBackupFiles();

            try
            {
                string pathToStorageFile = GetFullPathToStorageFile();

                this.ApplicationData = ReadDataFile(pathToStorageFile);

                IsDataLoaded = true;
                BackupDataAfterSuccessfulLoad(pathToStorageFile);
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
                this.ApplicationData = _backupExists? LoadBackup() : new ApplicationData();
            }
        }

        /// <summary>
        /// Creates a backup of the current data file (json-data.json) and saves it in the backup folder after a data file is successfully loaded.
        /// The backup is made by creating a new copy of the current data file after that file is successfully read.
        /// </summary>
        /// <param name="pathToStorageFile">The path where the backup file must be stored.</param>
        private void BackupDataAfterSuccessfulLoad(string pathToStorageFile)
        {
            var backupFilePath = CreateFileLocationPath(_dataBackupFileName, isBackupPath: true);
            
            
            if (_backupFilesInDirectory.Length == MaxNumberOfBackups)
            {
                _backupFilesInDirectory.Last().Delete();
            }

            File.Copy(pathToStorageFile, backupFilePath);

        }

        /// <summary>
        /// Gets the backup files and populates the <see cref="_backupFilesInDirectory"/> array based on the files in the backup folder. If the backup folder doesn't
        /// exist, this method creates one for the user.
        /// </summary>
        private void GetBackupFiles()
        {
            string backupFolderPath = GetUserFolderPath(isBackupFolder: true);

            if (!Directory.Exists(backupFolderPath))
            {
                Directory.CreateDirectory(backupFolderPath);
            }

            var directoryInfo = new DirectoryInfo(backupFolderPath);
            _backupFilesInDirectory = directoryInfo.GetFiles($"{_backupNamePrefix}*.json").OrderByDescending(x => x.CreationTime).ToArray();
        }

        /// <summary>
        /// Loads the most recent backup in the backup directory. The method continues to read each backup in the directory if a file cannot be read.
        /// If no readable backup files are available, the method returns a new instance of <see cref="Models.ApplicationData"/>.
        /// </summary>
        /// <returns>The user's <see cref="ApplicationData"/> after successfully reading the user's backup file. If files cannot be read, the method
        /// returns a new instance of <see cref="Models.ApplicationData"/></returns>
        private ApplicationData LoadBackup()
        {
            var applicationData = new ApplicationData();
            int backupNumber = 0;
            while (!WasBackupRestored && backupNumber < MaxNumberOfBackups)
            {
                string pathToStorageFile = GetFullPathToStorageFile();
                string pathToBackupFile = GetFullPathToBackupFile(backupNumber, out string backupFileName);
                

                try
                {
                    MoveBackupToDataFolder(backupNumber);
                    applicationData = ReadDataFile(pathToStorageFile);

                    IsDataLoaded = true;
                    WasBackupRestored = true;
                    BackupCreationTime = GetBackupCreationTime(backupFileName);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Trace.TraceInformation("No further backups available in the backup directory. Building new storage object.");
                    break;
                }
                catch (FileNotFoundException)
                {
                    
                    Trace.TraceInformation($"Could not find backup file. Path to the file is : {pathToBackupFile}.");
                }
                catch (Exception)
                {
                    File.Delete(pathToStorageFile);
                    Trace.TraceInformation($"Could not read current backup file located at : {pathToBackupFile}. Deleting copied json-data.json file and trying to read another backup.");
                }

                backupNumber++;
            }
            return applicationData;
        }

        /// <summary>
        /// Parse a string denoting the name of a backup file to extract the date and time from the file name.
        /// </summary>
        /// <param name="backupFileName"></param>
        /// <returns></returns>
        private DateTime GetBackupCreationTime(string backupFileName)
        {
            DateTime backupTime;
            bool success = DateTime.TryParseExact(backupFileName,
                $"'{_backupNamePrefix}'{_backupDateFormat}'.json'",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out backupTime);
            return backupTime;
        }

        /// <summary>
        /// Moves a backup from the backup folder to the main user data folder.
        /// </summary>
        /// <param name="backupNumber">A number representing the backup to restore. As <see cref="_backupFilesInDirectory"/> is sorted in descending order
        /// based on when files are created (latest -> oldest), a smaller number represents a newer file. So, 0 = latest backup.</param>
        private void MoveBackupToDataFolder(int backupNumber)
        {
            string pathToBackupFile = GetFullPathToBackupFile(backupNumber, out string backupFileName);
            string pathToStorageFile = GetFullPathToStorageFile();

            File.Copy(pathToBackupFile, pathToStorageFile);
        }
        
        /// <summary>
        /// Gets the location of the holos user data folder inside the user's AppData\Local folder. This can either be the main data folder
        /// or the backup folder inside the main folder.
        /// </summary>
        /// <param name="isBackupFolder">An optional parameter regarding whether the required folder path is for the data backup folder.</param>
        /// <returns>Returns the path to the user's data folder or their backup folder if the optional parameter is set to true.</returns>
        private string GetUserFolderPath(bool isBackupFolder = false)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folderPath = isBackupFolder ? Path.Combine(localAppData, "HOLOS_4\\backups") : Path.Combine(localAppData, "HOLOS_4");
            return folderPath;
        }

        /// <summary>
        /// Save the farm file to a crash file when we can't load the farm file correctly
        /// </summary>
        public string WriteCrashFile()
        {
            var crashFilePath = CreateFileLocationPath(_baseCrashFileName);
            //crashFilePath = crashFilePath.Replace(' ', '_');

            File.Copy(this.GetFullPathToStorageFile(), crashFilePath);

            return crashFilePath;
        }

        /// <summary>
        /// Creates the path where a file might be stored. This file can be either the crash file or the backup file for the user's data.
        /// This method gets the folder path, then combines that with a cleaned string representing the file that needs to be stored.
        /// </summary>
        /// <param name="fileName">Name of the file that we need to save</param>
        /// <param name="isBackupPath">Optional argument that checks if the path to be created is for the backup folder.</param>
        /// <returns>A string representing the path (directory and file name) of a file.</returns>
        private string CreateFileLocationPath(string fileName, bool isBackupPath = false)
        {
            var folderPath = GetUserFolderPath(isBackupFolder: isBackupPath);
            var newFilePath = Path.Combine(folderPath, fileName.CleanFileNameString());
            return newFilePath;
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