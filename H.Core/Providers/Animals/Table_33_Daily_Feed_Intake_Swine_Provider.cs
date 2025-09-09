using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Enumerations;
using H.Core.Providers.Feed;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    ///     Table 33. Daily feed intake (as fed) for each pig group, by province.
    ///     <para>Source: Greenhouse Gas System Pork Protocol (2006)</para>
    ///     No longer used, feed intake amounts are now associated with individual diets from the the
    ///     <see cref="DietProvider" />.
    /// </summary>
    public class Table_33_Daily_Feed_Intake_Swine_Provider
    {
        #region Public Methods

        public double GetFeedIntakeAmount(AnimalType animalType, Province province)
        {
            var byProvince = GetByProvince(province);
            if (byProvince.ContainsKey(animalType)) return byProvince[animalType];

            Trace.TraceError($"{nameof(Table_33_Daily_Feed_Intake_Swine_Provider)}.{nameof(GetFeedIntakeAmount)}" +
                             $" unable to get data for province: {province} and animal type: {animalType.GetDescription()}" +
                             " Returning default value of 0.");

            return 0;
        }

        public Dictionary<AnimalType, double> GetByProvince(Province province)
        {
            switch (province)
            {
                case Province.BritishColumbia:
                case Province.Alberta:
                case Province.Saskatchewan:
                case Province.Manitoba:
                case Province.Newfoundland:
                case Province.NovaScotia:
                case Province.NewBrunswick:
                case Province.PrinceEdwardIsland:
                    return new Dictionary<AnimalType, double>
                    {
                        { AnimalType.SwineStarter, 0.7 },
                        { AnimalType.SwinePiglets, 0.7 },
                        { AnimalType.SwineGrower, 2 },
                        { AnimalType.SwineFinisher, 3 },
                        { AnimalType.SwineDrySow, 2.55 },
                        { AnimalType.SwineSows, 2.55 },
                        { AnimalType.SwineGilts, 2.55 },
                        { AnimalType.SwineBoar, 2.55 },
                        { AnimalType.SwineLactatingSow, 6.11 }
                    };

                case Province.Ontario:
                case Province.Quebec:
                    return new Dictionary<AnimalType, double>
                    {
                        { AnimalType.SwineStarter, 0.65 },
                        { AnimalType.SwinePiglets, 0.65 },
                        { AnimalType.SwineGrower, 2 },
                        { AnimalType.SwineFinisher, 2.8 },
                        { AnimalType.SwineDrySow, 2.45 },
                        { AnimalType.SwineSows, 2.45 },
                        { AnimalType.SwineGilts, 2.45 },
                        { AnimalType.SwineBoar, 2.45 },
                        { AnimalType.SwineLactatingSow, 5.85 }
                    };

                default:
                    var defaultValue = new Dictionary<AnimalType, double>
                    {
                        { AnimalType.SwineStarter, 0 },
                        { AnimalType.SwinePiglets, 0 },
                        { AnimalType.SwineGrower, 0 },
                        { AnimalType.SwineFinisher, 0 },
                        { AnimalType.SwineDrySow, 0 },
                        { AnimalType.SwineSows, 0 },
                        { AnimalType.SwineGilts, 0 },
                        { AnimalType.SwineBoar, 0 },
                        { AnimalType.SwineLactatingSow, 0 }
                    };

                    Trace.TraceError($"{nameof(Table_33_Daily_Feed_Intake_Swine_Provider)}.{nameof(GetByProvince)}" +
                                     $" unable to get data for province: {province}." +
                                     $" Returning default value of {defaultValue}.");
                    return defaultValue;
            }
        }

        #endregion
    }
}