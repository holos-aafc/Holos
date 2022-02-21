using System.Collections.Generic;
using H.CLI.Factorys;
using H.CLI.Interfaces;

namespace H.CLI.Parser
{
    public class ParserHandler
    {
        #region Fields
        private ParsingStrategy _parserStrat = new ParsingStrategy();
        private ComponentKeyFactory _componentFactory = new ComponentKeyFactory();
        private ComponentTemporaryInputFactory _componentTemporaryInputFactory = new ComponentTemporaryInputFactory();
        #endregion

        #region Public Methods

        /// <summary>
        /// Takes in a string[] of a key and value pair where the key is the component type and the value is the path to the component's
        /// directory (ie, the path to the Fields directory which contains a csv file for each field).
        /// Based on the type of component[keyAndValue[0]), we will set the ConcreteComponentKey and ConcreteTemporaryInput (ie, if the component type is
        /// a Shelterbelt, we will call the factory method and return a ShelterBeltKeys and a ShelterBeltTemporaryInput object)
        /// Because we want to go through every file in the component's directory, we will retrieve the list of files based on the component's directory path
        /// that is stored in the value of our key and value (keyAndValue[1])
        /// Then, we will start the parsing of the files by passing in the list of files to the GetParsedComponent method of the Parser. The result will be
        /// set to the componentList field.
        /// </summary>
        public void InitializeParser(string componentType)
        {
            _parserStrat.SetComponentKey(_componentFactory.ComponentKeysCreator(componentType));
            _parserStrat.SetComponentTemporaryInput(_componentTemporaryInputFactory.CreateComponentTemporaryInputs(componentType));
        }

        public List<List<IComponentTemporaryInput>> StartParsing(string[] files)
        {
            return _parserStrat.GetParsedComponentList(files);
        }
        #endregion
    }
}
