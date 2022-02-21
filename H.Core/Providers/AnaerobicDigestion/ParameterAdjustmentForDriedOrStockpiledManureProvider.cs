using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Providers.AnaerobicDigestion
{
    public class ParameterAdjustmentForDriedOrStockpiledManureProvider
    {
        #region Fields
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        /// <summary>
        /// Method returns a <see cref="ParameterAdjustmentForDriedOrStockpiledManureData"/> based on the parameter. The only valid parameters are
        /// <see cref="ManureStateType.DeepBedding"/> (drying in pads) and <see cref="ManureStateType.SolidStorage"/> (stockpiling).
        /// </summary>
        /// <param name="manureStateType">The manure state type. This is either Deepbedding/Drying in pads or SolidStorage/Stockpiling.</param>
        /// <returns>Returns an instance of <see cref="ParameterAdjustmentForDriedOrStockpiledManureData"/> containing the parameter values.
        /// If incorrect paramter is specified, the method returns an empty instance.</returns>
        public ParameterAdjustmentForDriedOrStockpiledManureData GetParametersAdjustmentInstance(ManureStateType manureStateType)
        {
            if (manureStateType == ManureStateType.DeepBedding)
            {
                return (new ParameterAdjustmentForDriedOrStockpiledManureData
                {
                    VolatileSolidsReductionFactor = 0.65,
                    HydrolysisRateOfSubstrate = 0.06
                });
            }

            else if (manureStateType == ManureStateType.SolidStorage)
            {
                return (new ParameterAdjustmentForDriedOrStockpiledManureData 
                {
                    VolatileSolidsReductionFactor = 0.9,
                    HydrolysisRateOfSubstrate = 0.05,
                });
            }

            else
            {
                Trace.TraceError($"{nameof(ParameterAdjustmentForDriedOrStockpiledManureProvider)}.{nameof(ParameterAdjustmentForDriedOrStockpiledManureProvider.GetParametersAdjustmentInstance)}" +
                    $" does not contain ManureStateType: {manureStateType}. Please specify either {nameof(ManureStateType.DeepBedding)} or {nameof(ManureStateType.SolidStorage)}" +
                    $" returning empty object instance.");
                return new ParameterAdjustmentForDriedOrStockpiledManureData();
            }
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
