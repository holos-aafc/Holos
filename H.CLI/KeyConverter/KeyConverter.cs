using System.Text.RegularExpressions;

namespace H.CLI.KeyConverter
{
    public class KeyConverter
    {
        #region Public Methods
        /// <summary>
        /// Used in the FieldSystemInputConverter to convert Response (Yes or No Enum) to a bool as (true or false).
        /// </summary>
        public bool ConvertResponseToBool(string key)
        {
            if (key == "Yes")
            {
                return true;
            }

            else
                return false;
        }
        
        /// <summary>
        /// Used when reading French settings files if the user selects French as their language
        /// </summary>
        public string ConvertFrenchFormatOfDecimalsToNormal(string value)
        {
            var convertedEnglishValue = value.Replace(",", ".");
            return convertedEnglishValue;
        }

        /// <summary>
        /// Takes in a key and splits the words based on the location of Capital letters (subsequent capital letters are treated
        /// as one group. For example, you you see NORatio, it will split to become NO Ratio instead of N O Ratio
        /// Used in creating template and settings files. The function takes in a key and splits the key by capital letters and stores the result into a
        /// string[]. Than, we combine the strings in the string[] with a " ". For example, if we have the key "AverageCircumference(cm)"
        /// the key will be split into split[0]: "Average", split[1]: "Circumference(cm) and will return as "Average Circumference(cm)"
        /// </summary>
        public string ConvertTemplateKey(string key)
        {
            var convertedKey = Regex.Replace(key, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1").Trim();
            return convertedKey;
        }

        #endregion
    }
     
}


