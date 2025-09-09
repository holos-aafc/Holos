﻿using H.Core.Enumerations;

namespace H.Core.Providers.AnaerobicDigestion
{
    /// <summary>
    ///     Table 45
    ///     Parameter adjustments for dried or stockpiled manure entering the anaerobic digester
    ///     <para>Source: Gopalan et al. (2013)</para>
    /// </summary>
    public class Table_45_Parameter_Adjustments_For_Manure_Provider
    {
        #region Public Methods

        /// <summary>
        ///     Method returns a <see cref="Table_45_Parameter_Adjustments_For_Manure_Data" /> based on the parameter. The only
        ///     valid parameters are
        ///     <see cref="ManureStateType.DeepBedding" /> (drying in pads) and <see cref="ManureStateType.SolidStorage" />
        ///     (stockpiling).
        /// </summary>
        /// <param name="manureStateType">
        ///     The manure state type. This is either Deep bedding/Drying in pads or
        ///     SolidStorage/Stockpiling.
        /// </param>
        /// <returns>
        ///     Returns an instance of <see cref="Table_45_Parameter_Adjustments_For_Manure_Data" /> containing the parameter
        ///     values.
        ///     If incorrect parameter is specified, the method returns an empty instance.
        /// </returns>
        public Table_45_Parameter_Adjustments_For_Manure_Data GetParametersAdjustmentInstance(
            ManureStateType manureStateType)
        {
            // VolatileSolidsReductionFactor = Footnote 1
            // HydrolysisRateOfSubstrate = Footnote 2
            if (manureStateType == ManureStateType.DeepBedding)
                return new Table_45_Parameter_Adjustments_For_Manure_Data
                {
                    VolatileSolidsReductionFactor = 0.65,
                    HydrolysisRateOfSubstrate = 0.06
                };

            // All other solid types

            return new Table_45_Parameter_Adjustments_For_Manure_Data
            {
                VolatileSolidsReductionFactor = 0.9,
                HydrolysisRateOfSubstrate = 0.05
            };
        }

        #endregion

        #region MyRegion

        /*
         * Footnote 1: Composting was not considered as a pre-digester manure management/storage option as it is unlikely
            that a producer would go to the time and expense of composting manure to then add it to an AD system.
         * Footnote 2: The VSreductionfactor parameter is based on estimated reductions in biomethane potential (BMP)
            reported by Gopalan et al. (2013)

         */

        #endregion
    }
}