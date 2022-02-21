using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.CLI.Interfaces
{
    /// <summary>
    /// Has a field for a Dictionary that takes a string (the key or header) and the Imperial Units Of Measurement
    /// associated with that key. For example, if our key is "RowLength(m)", the imperial unit
    /// that correspond to meters would be Yards(ImperialUnitsOfMeasurement.Yards)
    /// 
    /// Component keys are used to define the template (default) input component files. These template files are meant to demonstrate to the user
    ///  how the CLI system expects data to be input for processing.
    /// </summary>
    public interface IComponentKeys
    {   
        Dictionary<string, ImperialUnitsOfMeasurement?> keys { get; set; }

        /// <summary>
        /// Adding new header columns to the CSV files means that Holos can't find the data it is looking for in old component files and will crash.
        /// By checking if some headers are optional we can allow old component files to still work with Holos. 
        /// </summary>
        /// <param name="s">The name of the header that we wish to check</param>
        /// <returns>True if header s is optional, false otherwise</returns>
        bool IsHeaderOptional(string s);

        Dictionary<string, bool> MissingHeaders { get; set; }
    }
}
