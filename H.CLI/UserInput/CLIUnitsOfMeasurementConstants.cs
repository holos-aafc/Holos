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

        public static void PromptUserForUnitsOfMeasurement()
        {
            int userChosenMeasurement;
            string userChosenMeasurementString;
            do
            {
                userChosenMeasurement = 0;
                //Need to display this in both French and English at the start because we do not know their preferred language yet
                Console.WriteLine(Properties.Resources.PromptUserForUnitsOfMeasurement);
                Console.WriteLine("Veuillez entrer le nombre correspondant à vos unités de mesure préférées(métrique = 1, impérial = 2");
                userChosenMeasurementString = Console.ReadLine();
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
