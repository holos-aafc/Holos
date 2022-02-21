using H.Core.Enumerations;
using H.Core.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 38
    /// </summary>
    public class DefaultCrudeProteinInFeedForSwineProvider_Table_38
    {
        #region Fields

        #endregion

        #region Constructors

        public DefaultCrudeProteinInFeedForSwineProvider_Table_38()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public Dictionary<AnimalType, double> GetByProvince(Province province)
        {
            switch (province)
            {
                case Province.Ontario:
                case Province.Quebec:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.210},
                        {AnimalType.SwineGrower, 0.175},
                        {AnimalType.SwineFinisher, 0.135},
                        {AnimalType.SwineDrySow, 0.135},
                        {AnimalType.SwineGilts, 0.145},
                        {AnimalType.SwineBoar, 0.135},
                        {AnimalType.SwineLactatingSow, 0.185}
                    };

                default:
                    var defaultValue = new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.220},
                        {AnimalType.SwineGrower, 0.180},
                        {AnimalType.SwineFinisher, 0.155},
                        {AnimalType.SwineDrySow, 0.145},
                        {AnimalType.SwineGilts, 0.145},
                        {AnimalType.SwineBoar, 0.135},
                        {AnimalType.SwineLactatingSow, 0.200}
                    };

                    return defaultValue;
            }
        }

        public double GetCrudeProteinInFeedForSwineGroupByProvince(Province province, AnimalType animalType)
        {
            var byProvince = this.GetByProvince(province);
            if (byProvince.ContainsKey(animalType))
            {
                return byProvince[animalType];
            }
            else
            {
                System.Diagnostics.Trace.TraceError($"{nameof(DefaultCrudeProteinInFeedForSwineProvider_Table_38)}.{nameof(GetCrudeProteinInFeedForSwineGroupByProvince)}" +
                                                    $" unable to get data for province: {province} and animal type: {animalType.GetDescription()}" +
                                                    $" Returning default value of 0.");

                return 0;
            }
        }

        #endregion
    }
}


