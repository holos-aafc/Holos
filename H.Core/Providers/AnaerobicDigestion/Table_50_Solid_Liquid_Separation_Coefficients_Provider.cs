using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using System.Diagnostics;

namespace H.Core.Providers.AnaerobicDigestion
{
    /// <summary>
    /// Table 50
    ///
    /// Default values for separation coefficients (fraction in solid fraction) for solid-liquid separation of digestate
    /// <para>Source: Guilayn et al. (2019)</para>
    /// </summary>
    public class Table_50_Solid_Liquid_Separation_Coefficients_Provider
    {
        #region Fields
        #endregion

        #region Constructors

        public Table_50_Solid_Liquid_Separation_Coefficients_Provider() { }

        #endregion

        #region Properties
        #endregion

        #region Public Methods

        /// <summary>
        /// Takes a <see cref="DigestateParameters"/> enumeration and returns an instance of <see cref="SolidLiquidSeparationCoefficientsData"/>
        /// containing default values for Centrifuge and Belt Press.
        /// </summary>
        /// <param name="coefficient"></param>
        /// <returns>An instance of <see cref="SolidLiquidSeparationCoefficientsData"/> containing default values for Centrifuge and Belt Press.
        /// In case of wrong parameter, returns an empty instance.</returns>
        public SolidLiquidSeparationCoefficientsData GetSolidLiquidSeparationCoefficientInstance(DigestateParameters coefficient)
        {
            switch (coefficient)
            {
                case DigestateParameters.RawMaterial:
                    return new SolidLiquidSeparationCoefficientsData 
                    {
                        SeparationCoefficient = SeparationCoefficients.FractionRawMaterials,
                        Centrifuge = 0.29,
                        BeltPress = 0.10,
                    };

                case DigestateParameters.TotalSolids:
                    return new SolidLiquidSeparationCoefficientsData
                    {
                        SeparationCoefficient = SeparationCoefficients.FractionTotalSolids,
                        Centrifuge = 0.81,
                        BeltPress = 0.32,
                    };

                case DigestateParameters.VolatileSolids:
                    return new SolidLiquidSeparationCoefficientsData
                    {
                        SeparationCoefficient = SeparationCoefficients.FractionVolatileSolids,
                        Centrifuge = 0.83,
                        BeltPress = 0.38,
                    };

                case DigestateParameters.TotalAmmoniaNitrogen:
                    return new SolidLiquidSeparationCoefficientsData
                    {
                        SeparationCoefficient = SeparationCoefficients.FractionTotalAmmoniumNitrogen,
                        Centrifuge = 0.29,
                        BeltPress = 0.10,
                    };

                case DigestateParameters.OrganicNitrogen:
                    return new SolidLiquidSeparationCoefficientsData
                    {
                        SeparationCoefficient = SeparationCoefficients.OrganicNitrogen,
                        Centrifuge = 0.78,
                        BeltPress = 0.19,
                    };

                default:
                    {
                        Trace.TraceError($"{nameof(Table_50_Solid_Liquid_Separation_Coefficients_Provider)}.{nameof(Table_50_Solid_Liquid_Separation_Coefficients_Provider.GetSolidLiquidSeparationCoefficientInstance)} " +
                            $"invalid DigestateParameters specified : {coefficient}. Returning an empty instance of SolidLiquidSeparationCoefficientsData.");
                        return new SolidLiquidSeparationCoefficientsData();
                    }
            }
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
