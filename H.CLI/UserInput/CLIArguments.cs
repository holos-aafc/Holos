using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.UserInput
{
    public class CLIArguments
    {
        #region Fields

        public static string _fileName;
        public bool _isFileNameFound; 
        public static string _settings;
        public static string _units;
        public static string _outputPath;
        public static string _polygonID;
        public static string _folderName;
        public bool _isFolderNameFound;

        #endregion

        #region Constructors

        public CLIArguments ()
        {
            _fileName = _settings = _units = _outputPath = _polygonID = _folderName = string.Empty;
            _isFileNameFound = _isFolderNameFound = false;
        }

        #endregion

        #region Properties

        public string FileName { get { return _fileName; } set {  _fileName = value; } }
        public string Settings { get { return _settings; } set { _settings = value; } }     
        public string Units { get { return _units; } set { _units = value; } }
        public string OutputPath { get { return _outputPath; } set { _outputPath = value; } }
        public string PolygonID { get { return _polygonID; } set { _polygonID = value; } }
        public string FolderName { get { return _folderName;} set { _folderName = value; } }
        public bool IsFileNameFound {  get { return _isFileNameFound; } set { _isFileNameFound = value; } }
        public bool IsFolderNameFound { get { return _isFolderNameFound; } set { _isFolderNameFound = value; } }

        #endregion

        #region Public Methods
        public void ParseArgs(string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg == "-i" && i + 1 < args.Length)
                {
                    FileName = args[i+1];
                    i++;
                }
                else if (arg == "-s" && i + 1 < args.Length)
                {
                    Settings = args[i+1];
                    i++;
                }
                else if (arg == "-u" && i + 1 < args.Length)
                {
                    Units = args[i+1];
                    i++;
                }
                else if (arg == "-o" && i + 1 < args.Length)
                {
                    OutputPath = args[i+1];
                    i++;
                }
                else if (arg == "-p" && i + 1 < args.Length)
                {
                    PolygonID = args[i+1];
                    i++;
                }
                else if (arg == "-f" && i + 1 < args.Length)
                {
                    FolderName = args[i+1];
                    i++;
                }
            }
        }

        #endregion 
    }
}
