using H.CLI.Interfaces;
using H.CLI.Processors;
using System;
using H.Core.Services.LandManagement;

namespace H.CLI.Factories
{
    public class ComponentProcessorFactory
    {
        #region Fields

        private readonly FieldProcessor _fieldProcessor;
        private readonly ShelterbeltProcessor _shelterbeltProcessor;
        private FieldResultsService _fieldResultsService;

        #endregion

        #region Constructors

        public ComponentProcessorFactory(FieldResultsService fieldResultsService)
        {
            if (fieldResultsService != null)
            {
                _fieldResultsService = fieldResultsService; 
            }
            else
            {
                throw new ArgumentNullException(nameof(fieldResultsService));
            }
            
            _fieldProcessor = new FieldProcessor(_fieldResultsService);
            _shelterbeltProcessor = new ShelterbeltProcessor();
        } 

        #endregion

        #region Public Methods
        /// <summary>
        /// Based on the type of Component in our farm's list of components, return the appropriate concrete Processor
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public IProcessor GetComponentProcessor(Type componentType)
        {
            switch (componentType.Name.ToUpper())
            {
                case "SHELTERBELTCOMPONENT":
                    return _shelterbeltProcessor;
                case "FIELDSYSTEMCOMPONENT":
                    return _fieldProcessor;
                default:
                    return new ShelterbeltProcessor();
            }
        } 
        #endregion
    }
}
