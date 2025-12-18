#region Imports

#endregion

using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;

namespace H.Core.Models.Animals.Beef
{
    /// <summary>
    /// </summary>
    public class CowCalfComponent : AnimalComponentBase
    {
        #region Constructors

        public CowCalfComponent()
        {
            this.ComponentCategory = ComponentCategory.BeefProduction;
            this.ComponentType = ComponentType.CowCalf;
            this.ComponentNameDisplayString = Properties.Resources.TitleCowCalfComponentDisplayString;
            this.ComponentDescriptionString = Properties.Resources.ToolTipCowCalfComponent;
        }

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}