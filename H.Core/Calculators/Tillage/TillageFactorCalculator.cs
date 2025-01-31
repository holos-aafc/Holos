#region Imports

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Tools;

#endregion

namespace H.Core.Calculators.Tillage
{
    /// <summary>
    /// Calculates the tillage factor for various types.
    /// Table 3 -  rc factor – Alberta, Saskatchewan, Manitoba only.
    /// </summary>
    public class TillageFactorCalculator : ITillageFactorCalculator
    {
        #region Fields

        private readonly List<TillageFactorTableRow> _tillageFactorTableRows;

        #endregion

        #region Constructors

        public TillageFactorCalculator()
        {
            HTraceListener.AddTraceListener();
            _tillageFactorTableRows = new List<TillageFactorTableRow>
            {
                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                    TillageType = TillageType.Intensive,
                    TillageFactor = 1
                },

                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                    TillageType = TillageType.Reduced,
                    TillageFactor = 0.9
                },

                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown,
                    TillageType = TillageType.NoTill,
                    TillageFactor = 0.8
                },

                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.DarkBrown,
                    TillageType = TillageType.Intensive,
                    TillageFactor = 1
                },

                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.DarkBrown,
                    TillageType = TillageType.Reduced,
                    TillageFactor = 0.85
                },

                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.DarkBrown,
                    TillageType = TillageType.NoTill,
                    TillageFactor = 0.7
                },

                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                    TillageType = TillageType.Intensive,
                    TillageFactor = 1
                },

                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                    TillageType = TillageType.Reduced,
                    TillageFactor = 0.8
                },

                new TillageFactorTableRow
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                    TillageType = TillageType.NoTill,
                    TillageFactor = 0.6
                }
            };
        }

        #endregion

        #region Public Methods

        public double CalculateTillageFactor(Province province,
                                             SoilFunctionalCategory soilFunctionalCategory,
                                             TillageType tillageType,
                                             CropType cropType)
        {
            if (cropType.IsRootCrop())
            {
                return 1.13;
            }

            if (cropType.IsAnnual() && province.IsPrairieProvince() == false)
            {
                return 1;
            }


            var simplifiedSoilCategory = soilFunctionalCategory.GetSimplifiedSoilCategory();
            if (cropType.IsPerennial())
            {
                return this.CalculateTillageFactorForPerennials(simplifiedSoilCategory, province);
            }

            return this.CalculateCropTillageFactor(simplifiedSoilCategory, tillageType);
        }

        #endregion

        #region Properties

        #endregion

        #region Private Methods     

        /// <summary>
        /// For perennials, the 0.9 value is applied in the first year (year of planting), in the years, after the no-till factor is used.
        /// </summary>
        private double CalculateTillageFactorForPerennials(
            SoilFunctionalCategory soilFunctionalCategory,
            Province province)
        {
            if (province.IsPrairieProvince() == false)
            {
                return 0.9;
            }
            else
            {
                return this.CalculateCropTillageFactor(soilFunctionalCategory, TillageType.NoTill);
            }
        }

        private double CalculateCropTillageFactor(
            SoilFunctionalCategory soilFunctionalCategory, 
            TillageType tillageType)
        {
            var result = _tillageFactorTableRows.SingleOrDefault(x => x.SoilFunctionalCategory == soilFunctionalCategory && x.TillageType == tillageType);
            if (result != null)
            {
                return result.TillageFactor;
            }
            else
            {
                Trace.TraceError($"{nameof(TillageFactorCalculator)}.{nameof(CalculateCropTillageFactor)}: unable to calculate factor for {soilFunctionalCategory} and {tillageType}. Returning 1.");

                return 1;
            }
        }      

        #endregion

        #region Event Handlers

        #endregion
    }
}