using System.Collections.ObjectModel;
using System.Globalization;
using CsvHelper.Configuration;
using CSVReader = CsvHelper.CsvReader;

namespace H.Avalonia.Infrastructure;

/// <summary>
/// Extension methods that help in exporting data from the program.
/// </summary>
public class ImportHelpers
{
    public string? ImportPath { get; set; }

    /// <summary>
    /// Imports data from a CSV file based on a class map of a class using <see cref="CSVReader"/>. An <see cref="ImportPath"/> must be set for the data to be imported.
    /// </summary>
    /// <param name="classMap">A map of the class which indicates the various properties of a class that needs to be imported.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>An observablecollection containing the imported data.</returns>
    public ObservableCollection<T>? ImportFromCsv<T>(ClassMap<T> classMap)
    {
        if (ImportPath == null) return null;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ConvertToImportFormat(),
        };
        
        using var reader = new StreamReader(ImportPath);
        using var csvReader = new CSVReader(reader, config);
        csvReader.Context.RegisterClassMap(classMap);
        var result = csvReader.GetRecords<T>();
        return new ObservableCollection<T>(result);
    }
}