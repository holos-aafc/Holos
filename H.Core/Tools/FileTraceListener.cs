using System;
using System.Diagnostics;
using System.IO;

namespace H.Core.Tools
{
    public class FileTraceListener
    {
        private const string _logFilesPrefix = "holos-logs";

        public TextWriterTraceListener TraceListener { get; set; }

        public FileTraceListener()
        {
            Initialize();
        }

        private void Initialize()
        {
            var logFile = Path.Combine(GetLogFolderPath(), $"{_logFilesPrefix}-{DateTime.Now:yyyy-MM-dd}.log");
            
            // Ensure log directory exists
            var logDirectory = Path.GetDirectoryName(logFile);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            TraceListener = new TextWriterTraceListener(logFile)
            {
                Name = "HolosLogTraceListener",
                TraceOutputOptions = TraceOptions.DateTime,
            };
            
            Trace.Listeners.Add(TraceListener);
        }

        private string GetLogFolderPath()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var logfilesPath = Path.Combine(localAppData, @"HOLOS_4\logfiles");
            return logfilesPath;
        }
    }
}
