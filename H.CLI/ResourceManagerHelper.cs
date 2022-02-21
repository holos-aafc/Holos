using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI
{
    /// <summary>
    /// Extension From Resource Manager to retrieve the "Name" based on a "Value" and it's culture info.
    /// For Example, if the value is "Nom", the extension method will return "Name"
    /// This method is called using the Resource Manager of the resource files you are using. For Example: Properties.Resources.ResourceManager.GetResourceName(...)
    /// </summary>
    public static class ResourceManagerHelper
    {
        /// <summary>
        /// Get resource name (i.e. resource key)
        /// </summary>
        /// <param name="resourceManager">the resource manager</param>
        /// <param name="value">the value to search for</param>
        /// <param name="cultureInfo">the culture (en-CA or fr-CA)</param>
        /// <returns>resource key</returns>
        public static string GetResourceName(this ResourceManager resourceManager, string value, CultureInfo cultureInfo)
        {
            //Get The Resource file based on the culture. French = Resources.fr-CA.resx. English = Resources.en-CA.resx
            //If the value is found in the resource value, return the key associated with that value
            var resourceEntry = resourceManager.GetResourceSet(cultureInfo, true, true)
                                               .OfType<DictionaryEntry>()
                                               .FirstOrDefault(dictionaryEntry => dictionaryEntry.Value.ToString().Trim().Equals(value.Trim(), StringComparison.CurrentCultureIgnoreCase));

            if (resourceEntry.Key == null)
            {
                throw new Exception(string.Format("Cannot find the key: {0} in the resource file: {1}", value, resourceManager.BaseName));
            }
            //workaround for getting rid of the prepended "Key_" or "Settings_"
            if (resourceEntry.Key.ToString().StartsWith("Key_"))
                return resourceEntry.Key.ToString().Replace("Key_", "");
            else return resourceEntry.Key.ToString().Replace("Settings_", "");
        }
    }
}

