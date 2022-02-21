using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Enumerations
{
    public static class ManureStateTypeExtensions
    {
        #region Public Methods

        public static bool IsGrazingArea(this ManureStateType manureStateType)
        {
            return manureStateType == ManureStateType.Paddock ||
                   manureStateType == ManureStateType.Range ||
                   manureStateType == ManureStateType.Pasture;
        }

        /// <summary>
        /// Indicates if the storage type being used houses liquid manure.
        /// </summary>
        /// <param name="manureStateType">The manure storage type being used</param>
        /// <returns>True if the storage type is for liquid manure, false otherwise</returns>
        public static bool IsLiquidManure(this ManureStateType manureStateType)
        {
            return manureStateType == ManureStateType.Liquid ||
                   manureStateType == ManureStateType.LiquidCrust ||
                   manureStateType == ManureStateType.LiquidNoCrust ||
                   manureStateType == ManureStateType.LiquidWithNaturalCrust ||
                   manureStateType == ManureStateType.Slurry ||
                   manureStateType == ManureStateType.SlurryWithNaturalCrust ||
                   manureStateType == ManureStateType.SlurryWithoutNaturalCrust ||
                   manureStateType == ManureStateType.LiquidWithSolidCover ||
                   manureStateType == ManureStateType.LiquidSeparated;
        }

        public static bool IsCompost(this ManureStateType manureStateType)
        {
            return manureStateType == ManureStateType.CompostIntensive ||
                   manureStateType == ManureStateType.CompostPassive ||
                   manureStateType == ManureStateType.Composted;
        }

        /// <summary>
        /// Indicates if the storage type being used houses solid manure.
        /// </summary>
        /// <param name="manureStateType">The manure storage type being used</param>
        /// <returns>True if the storage type is for solid manure, false otherwise</returns>
        public static bool IsSolidManure(this ManureStateType manureStateType)
        {
            return manureStateType.IsLiquidManure() == false;
        }

        /// <summary>
        /// Dairy manure systems can be covered with a lid/cap etc.
        /// </summary>
        public static bool IsCoveredSystem(this ManureStateType manureStateType)
        {
            return manureStateType == ManureStateType.LiquidWithNaturalCrust ||
                   manureStateType == ManureStateType.LiquidWithSolidCover;
        }

        #endregion
    }
}
