using System;
using H.CLI.ComponentKeys;
using H.CLI.Interfaces;

namespace H.CLI.Factorys
{
    public class ComponentKeyFactory
    {
        #region Public Methods
        /// <summary>
        /// Based on the type of component, return the appropriate concrete ComponentKeys class. If you are adding a new component,
        /// please add the appropriate case below. The name of the case should correspond with the spelling in the DirectoryKeys.
        /// The cases below are strings that shuold always be based on the list of keys in the DirectoryKeys class.
        /// </summary>
        public IComponentKeys ComponentKeysCreator(string component)
        {
            if (component.ToUpper() == Properties.Resources.DefaultShelterbeltInputFolder.ToUpper())
            {
                return new ShelterBeltKeys();
            }

            if (component.ToUpper() == Properties.Resources.DefaultFieldsInputFolder.ToUpper())
            {
                return new FieldKeys();
            }

            if (component.ToUpper() == Properties.Resources.DefaultSwineInputFolder.ToUpper())
            {
                return new SwineKeys();
            }

            if (component.ToUpper() == Properties.Resources.DefaultDairyInputFolder.ToUpper())
            {
                return new DairyCattleKeys();
            }

            if (component.ToUpper() == Properties.Resources.DefaultSheepInputFolder.ToUpper())
            {
                return new SheepKeys();
            }


            if (component.ToUpper() == Properties.Resources.DefaultBeefInputFolder.ToUpper())
            {
                return new BeefCattleKeys();
            }

            if (component.ToUpper() == Properties.Resources.DefaultPoultryInputFolder.ToUpper())
            {
                return new PoultryKeys();
            }
            if (component.ToUpper() == Properties.Resources.DefaultOtherLivestockInputFolder.ToUpper())
            {
                return new OtherLiveStockKeys();
            }

            else
            {
                throw new NotImplementedException();
            }
           
        } 
        #endregion
    }
}
