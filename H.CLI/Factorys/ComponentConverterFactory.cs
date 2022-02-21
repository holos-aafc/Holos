using H.CLI.Converters;
using H.CLI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.Factorys
{
    public class ComponentConverterFactory
    {

        #region Public Methods
        /// <summary>
        /// Based on the type of component, return the appropriate concrete converter. 
        /// If you are adding a new component, please add the appropriate concrete converter as a case below. The cases are
        /// strings that should always be written based on the list of keys in the DirectoryKeys class.
        /// </summary>
        public IConverter GetComponentConverter(string component)
        {
            if (component.ToUpper() == Properties.Resources.DefaultShelterbeltInputFolder.ToUpper())
            {
                return new ShelterbeltConverter();
            }

            if (component.ToUpper() == Properties.Resources.DefaultFieldsInputFolder.ToUpper())
            {
                return new FieldSystemInputConverter();
            }

            if (component.ToUpper() == Properties.Resources.DefaultSwineInputFolder.ToUpper())
            {
                return new SwineConverter();
            }

            if (component.ToUpper() == Properties.Resources.DefaultDairyInputFolder.ToUpper())
            {
                return new DairyConverter();
            }

            if (component.ToUpper() == Properties.Resources.DefaultSheepInputFolder.ToUpper())
            {
                return new SheepConverter();
            }


            if (component.ToUpper() == Properties.Resources.DefaultBeefInputFolder.ToUpper())
            {
                return new BeefConverter();
            }

            if (component.ToUpper() == Properties.Resources.DefaultPoultryInputFolder.ToUpper())
            {
                return new PoultryConverter();
            }
            if (component.ToUpper() == Properties.Resources.DefaultOtherLivestockInputFolder.ToUpper())
            {
                return new OtherLiveStockConverter();
            }

            else
            {
                throw new NotImplementedException();
            }
        } 
        #endregion
    }
}
