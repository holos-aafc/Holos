using System;
using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;

namespace H.CLI.Factorys
{
    public class ComponentTemporaryInputFactory
    {
        #region Public Methods
        /// Based on the type of component, return the appropriate concrete TemporaryInput class. If you are adding a new component,
        /// please add the appropriate case below. The cases below are strings that should always 
        /// be based on the list of keys in the DirectoryKeys class
        /// </summary>
        public IComponentTemporaryInput CreateComponentTemporaryInputs(string component)
        {
            
            if (component.ToUpper() == Properties.Resources.DefaultShelterbeltInputFolder.ToUpper())
            {
                return new ShelterBeltTemporaryInput();
            }

            if (component.ToUpper() == Properties.Resources.DefaultFieldsInputFolder.ToUpper())
            {
                return new FieldTemporaryInput();
            }

            if (component.ToUpper() == Properties.Resources.DefaultSwineInputFolder.ToUpper())
            {
                return new SwineTemporaryInput();
            }

            if (component.ToUpper() == Properties.Resources.DefaultDairyInputFolder.ToUpper())
            {
                return new DairyTemporaryInput();
            }

            if (component.ToUpper() == Properties.Resources.DefaultSheepInputFolder.ToUpper())
            {
                return new SheepTemporaryInput();
            }


            if (component.ToUpper() == Properties.Resources.DefaultBeefInputFolder.ToUpper())
            {
                return new BeefCattleTemporaryInput();
            }

            if (component.ToUpper() == Properties.Resources.DefaultPoultryInputFolder.ToUpper())
            {
                return new PoultryTemporaryInput();
            }
            if (component.ToUpper() == Properties.Resources.DefaultOtherLivestockInputFolder.ToUpper())
            {
                return new OtherLiveStockTemporaryInput();
            }

            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
        #endregion

