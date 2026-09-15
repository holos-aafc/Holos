using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.UserInput
{
    public static class CLIUnitsOfMeasurementConstants
    {
        #region Fields
        public static MeasurementSystemType measurementSystem;
        #endregion
    
        #region Public Methods

        public static void PromptUserForUnitsOfMeasurement(string argUnits)
        {
            if (!string.IsNullOrEmpty(argUnits))
            {
                string argUnitsLower = argUnits.ToLower();

                if (argUnitsLower == "metric" || argUnitsLower == "m")
                {
                    measurementSystem = MeasurementSystemType.Metric;
                    return;
                }
                else if (argUnitsLower == "imperial" || argUnitsLower == "i") 
                {
                    measurementSystem = MeasurementSystemType.Imperial;
                    return;
                }
                else
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Properties.Resources.InvalidUnitsArgument, argUnits);
                    Console.ForegroundColor = ConsoleColor.White;
                }       
            }

            int userChosenMeasurement;
            string userChosenMeasurementString;
            do
            {
                userChosenMeasurement = 0;
                //Need to display this in both French and English at the start because we do not know their preferred language yet
                Console.WriteLine();
                Console.WriteLine(Properties.Resources.PromptUserForUnitsOfMeasurement);
                userChosenMeasurementString = Console.ReadLine();

                // Null means the input stream has ended - nobody is there to answer. Re-prompting would spin
                // forever against a closed stream, so fall back to metric, which is what -u metric selects.
                if (userChosenMeasurementString == null)
                {
                    measurementSystem = MeasurementSystemType.Metric;
                    return;
                }

                int.TryParse(userChosenMeasurementString, out userChosenMeasurement);

            } while (userChosenMeasurement < 0 || userChosenMeasurement > 2 || !int.TryParse(userChosenMeasurementString, out userChosenMeasurement));

            if (userChosenMeasurement == 1)
            {
                measurementSystem = MeasurementSystemType.Metric;
            }

            if (userChosenMeasurement == 2)
            {
                measurementSystem = MeasurementSystemType.Imperial;
            }

        } 
        #endregion
    }
}
