using System.Collections.Generic;
using H.CLI.Interfaces;
using H.Core.Models;

namespace H.CLI.Converters
{
    public class ConvertingComponentStrategy
    {
        #region Fields
        public IConverter _converter { get; set; } 
        #endregion

        #region Constructors
        public ConvertingComponentStrategy() { }

        public ConvertingComponentStrategy(IConverter converter)
        {
            _converter = converter;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the converter to the concrete converter that will be passed in. It takes an interface to so it can be more flexible
        /// and can take in any concrete converter that implements the IConverter interface
        /// </summary>
        public void SetConvertingComponentStrategy(IConverter converter)
        {
            _converter = converter;
        }

        /// <summary>
        /// Based on the concrete converter that has been set, the appropriate ConvertParsedComponent method will be called
        /// Takes in the parsedConcreteComponentList where the GUIDs have been set.
        /// </summary>
        public List<ComponentBase> ConvertComponent(List<List<IComponentTemporaryInput>> parsedConcreteComponentListWithGuidSet, Farm farm)
        {
            return _converter.ConvertParsedComponent(parsedConcreteComponentListWithGuidSet, farm);
        } 
        #endregion

    }
}
