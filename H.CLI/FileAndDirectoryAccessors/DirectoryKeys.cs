using System.Collections.Generic;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class DirectoryKeys 
    {
        #region Fields
        /// <summary>
        /// A dictionary that contains the name of the Directory and the weight associated with that directory
        /// Fields and Shelterbelts are a higher weight because we need to process them first incase they are
        /// used in the other Components. If you would like something to be processed first, please change the weight
        /// to be higher and just the Component weights accordingly.
        /// </summary>
        public Dictionary<string, int> directoryWeights { get; set; } = new Dictionary<string, int>
        {
            {Properties.Resources.DefaultShelterbeltInputFolder, 2 },
            {Properties.Resources.DefaultFieldsInputFolder, 2 },
            {Properties.Resources.DefaultBeefInputFolder, 1 },
            {Properties.Resources.DefaultSheepInputFolder, 1 },
            {Properties.Resources.DefaultDairyInputFolder, 1 },
            {Properties.Resources.DefaultPoultryInputFolder, 1 },
            {Properties.Resources.DefaultSwineInputFolder, 1 },
            {Properties.Resources.DefaultOtherLivestockInputFolder, 1 }
        };   
        #endregion
    }
}
