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
    /// Table 42. Crude protein content in feed, as fed, for each pig group, by province.
    /// <para>Source: Greenhouse Gas System Pork Protocol (2006)</para>
    /// </summary>
    public class Table_42_Crude_Protein_Content_Swine_Feed_Provider
    {
        #region Fields

        #endregion

        #region Constructors

        public Table_42_Crude_Protein_Content_Swine_Feed_Provider()
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
                System.Diagnostics.Trace.TraceError($"{nameof(Table_42_Crude_Protein_Content_Swine_Feed_Provider)}.{nameof(GetCrudeProteinInFeedForSwineGroupByProvince)}" +
                                                    $" unable to get data for province: {province} and animal type: {animalType.GetDescription()}" +
                                                    $" Returning default value of 0.");

                return 0;
            }
        }

        #endregion
    }
}


