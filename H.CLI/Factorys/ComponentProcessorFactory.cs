using H.CLI.Interfaces;
using H.CLI.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.Factorys
{
    public class ComponentProcessorFactory
    {
        #region Fields

        private readonly FieldProcessor _fieldProcessor;
        private readonly ShelterbeltProcessor _shelterbeltProcessor;

        #endregion

        #region Constructors

        public ComponentProcessorFactory()
        {
            _fieldProcessor = new FieldProcessor();
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
