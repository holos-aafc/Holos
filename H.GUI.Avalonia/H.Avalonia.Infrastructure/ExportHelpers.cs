using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace H.Avalonia.Infrastructure
{
    /// <summary>
    /// Extension methods that help in exporting data from the program.
    /// </summary>
    public class ExportHelpers
    {
        /// <summary>
        /// The path to which data is to be exported.
        /// </summary>
        public string? ExportPath { get; set; }
        
        /// <summary>
        /// Exports data from a CSV file based on a class map of a class using <see cref="CsvWriter"/>. An <see cref="ExportPath"/> must be set for the data to be imported.
        /// </summary>
        /// <param name="collection">The collection containing the data to be exported.</param>
        /// <param name="classMap">A map of the properties in the class. This helps in allowing <see cref="CsvWriter"/> to export class properties correctly.</param>
        /// <typeparam name="T"></typeparam>
        public void ExportToCSV<T>(ObservableCollection<T> collection, ClassMap<T> classMap)
        {
            if (ExportPath == null) return;

            using var writer = new StreamWriter(ExportPath);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvWriter.Context.RegisterClassMap(classMap);

            csvWriter.WriteRecords(collection);
        }
    }
}
