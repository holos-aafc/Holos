using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Logging;

namespace H.Core.Tools
{
    public class FileTraceListener
    {
        private const string _logFilesPrefix = "holos-logs";

        public FileLogTraceListener TraceListener { get; set; }

        public FileTraceListener()
        {
            Initialize();
        }

        private void Initialize()
        {
            TraceListener = new FileLogTraceListener
            {
                Name = "HolosLogTraceListener",
                CustomLocation = GetLogFolderPath(), 
                BaseFileName = _logFilesPrefix,
                LogFileCreationSchedule = LogFileCreationScheduleOption.Daily, 
                TraceOutputOptions = TraceOptions.DateTime,
                AutoFlush = false,
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
