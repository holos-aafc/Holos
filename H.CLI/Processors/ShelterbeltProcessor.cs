using H.CLI.Interfaces;
using H.CLI.UserInput;
using H.Core.Calculators.Shelterbelt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.FileAndDirectoryAccessors;
using H.Core.Models;
using H.Core.Models.LandManagement.Shelterbelt;

namespace H.CLI.Processors
{
    public class ShelterbeltProcessor : IProcessor
    {
        #region Fields
        private ShelterbeltCalculator _shelterbeltCalculator = new ShelterbeltCalculator();
        private KeyConverter.KeyConverter _keyConverter = new KeyConverter.KeyConverter();
        private DirectoryHandler directoryHandler = new DirectoryHandler();
        #endregion

        #region Public Methods
        public void ProcessComponent(Farm farm, List<ComponentBase> componentList, ApplicationData applicationData)
        {
            var calculator = new ShelterbeltCalculator();
            var castedShelterbeltList = componentList.Cast<ShelterbeltComponent>().ToList();
       

            foreach (var shelterBeltComponent in castedShelterbeltList)
            {

            }
        } 
        #endregion
    }
}
