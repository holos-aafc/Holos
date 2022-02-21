using System.Collections.Generic;
using H.CLI.Interfaces;

namespace H.CLI.Parser
{
    public class ParsingStrategy
    {
        #region Fields
        public Parser _parser { get; set; } = new Parser(); 
        #endregion

        #region Constructor
        public ParsingStrategy() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Based on the concrete IComponentKeys object passed in from the ParserHandler, this class will set the parser's key to that concrete key. (Ie, ShelterbeltKeys, FieldKeys)
        /// </summary>

        public void SetComponentKey(IComponentKeys keyList)
        {
            _parser.ComponentKey = keyList;
        }

        /// <summary>
        /// Based on the concrete IComponentTemporaryInput object passed in from the ParserHandler, this class will set the parser's temporary input to that concrete object
        /// </summary>
   
        public void SetComponentTemporaryInput(IComponentTemporaryInput inputType)
        {
            _parser.ComponentTemporaryInput = inputType;
        }

        /// <summary>
        /// Takes in a list of files that correspond to all the files in a components directory (ie, all the file in the Shelterbelt directory) that is
        /// retrieved from the ParserHandler. This method will call the Parse onto that list of files and return the parsed component list.
        /// </summary>
        public List<List<IComponentTemporaryInput>> GetParsedComponentList(string[] fileList)
        {
            var parsedComponentList = _parser.Parse(fileList);
            return parsedComponentList;
        } 
        #endregion
    }
}
