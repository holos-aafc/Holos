using System;
using System.Globalization;

namespace H.CLI.UserInput
{
    public static class CLILanguageConstants
    {
        #region Fields

        public static CultureInfo culture;
        public static string Delimiter;
        public static string OutputLanguageAddOn;
        public static string DisplayDataSeparator;
        public static string DefaultInputFileExtension = ".csv";
        public static string NotApplicableString = "N/A";
        public static string NotApplicableResultsString = "---";

        #endregion

        #region Public Methods
        public static void SetCulture(CultureInfo userCulture)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            culture = userCulture;
            switch (culture.Name)
            {
                case "fr-CA":
                    {      
                        Console.WriteLine(String.Format("Your current language is {0}. If the language is not correct, please change " +
                                                        "the language settings on your computer to English(Canada)", culture.DisplayName));
                        Console.WriteLine();
                        Delimiter = ";";
                        OutputLanguageAddOn = "-fr-CA.csv";
                        DisplayDataSeparator = ",";
                    }
                    break;

                case "en-CA":
                    {
                        Console.WriteLine(String.Format("Votre langue actuelle est {0}. Si la langue n'est pas correcte, s'il vous plaît changer de" +
                         "les paramètres de langage sur votre ordinateur pour Français(Canada)", culture.DisplayName));
                        Console.WriteLine();
                        Delimiter = ",";
                        OutputLanguageAddOn = "-en-CA.csv";
                        DisplayDataSeparator = ".";
                    }
                    break;

                case "en-US":
                    {
                        Console.WriteLine(String.Format("Votre langue actuelle est {0}. Si la langue n'est pas correcte, s'il vous plaît changer de" +
                                                        "les paramètres de langage sur votre ordinateur pour Français(Canada)", culture.DisplayName));
                        Console.WriteLine();
                        Delimiter = ",";
                        OutputLanguageAddOn = "-en-US.csv";
                        DisplayDataSeparator = ".";
                    }
                    break;

                default:
                    Console.ResetColor();
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(String.Format(Properties.Resources.LanguageNotSupported, culture.DisplayName));
                    throw new NotSupportedException(String.Format(Properties.Resources.LanguageNotSupported, culture.DisplayName)); 

            }
            Console.ResetColor();
        }

        #endregion
    }
}
