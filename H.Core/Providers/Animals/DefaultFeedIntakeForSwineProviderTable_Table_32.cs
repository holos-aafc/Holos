using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    public class DefaultFeedIntakeForSwineProviderTable_Table_32
    {
        #region Fields


        #endregion

        #region Contructors

        public DefaultFeedIntakeForSwineProviderTable_Table_32()
        {
        }

        #endregion

        #region Public Methods

        public double GetFeedIntakeAmount(AnimalType animalType, Province province)
        {
            var byProvince = this.GetByProvince(province);
            if (byProvince.ContainsKey(animalType))
            {
                return byProvince[animalType];
            }
            else
            {
                System.Diagnostics.Trace.TraceError($"{nameof(DefaultFeedIntakeForSwineProviderTable_Table_32)}.{nameof(GetFeedIntakeAmount)}" +
                                                    $" unable to get data for province: {province} and animal type: {animalType.GetDescription()}" +
                                                    $" Returning default value of 0.");

                return 0;
            }
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
                        {AnimalType.SwineStarter, 0.7},
                        {AnimalType.SwinePiglets, 0.7},
                        {AnimalType.SwineGrower, 2},
                        {AnimalType.SwineFinisher, 3},
                        {AnimalType.SwineDrySow, 2.55},
                        {AnimalType.SwineSows, 2.55},
                        {AnimalType.SwineGilts, 2.55},
                        {AnimalType.SwineBoar, 2.55},
                        {AnimalType.SwineLactatingSow, 6.11}
                    };

                case Province.Ontario:
                case Province.Quebec:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.65},
                        {AnimalType.SwinePiglets, 0.65},
                        {AnimalType.SwineGrower, 2},
                        {AnimalType.SwineFinisher, 2.8},
                        {AnimalType.SwineDrySow, 2.45},
                        {AnimalType.SwineSows, 2.45},
                        {AnimalType.SwineGilts, 2.45},
                        {AnimalType.SwineBoar, 2.45},
                        {AnimalType.SwineLactatingSow, 5.85}
                    };

                default:
                    var defaultValue = new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0},
                        {AnimalType.SwinePiglets, 0},
                        {AnimalType.SwineGrower, 0 },
                        {AnimalType.SwineFinisher, 0},
                        {AnimalType.SwineDrySow, 0},
                        {AnimalType.SwineSows, 0},
                        {AnimalType.SwineGilts, 0},
                        {AnimalType.SwineBoar, 0},
                        {AnimalType.SwineLactatingSow, 0}
                    };

                    System.Diagnostics.Trace.TraceError($"{nameof(DefaultFeedIntakeForSwineProviderTable_Table_32)}.{nameof(DefaultFeedIntakeForSwineProviderTable_Table_32.GetByProvince)}" +
                    $" unable to get data for province: {province}." +
                    $" Returning default value of {defaultValue}.");
                    return defaultValue;
            }
        }

        #endregion
    }
}