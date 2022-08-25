using H.Core.Enumerations;
using H.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 34. Volatile solid excretion for performance standard diets for each pig group, by province.
    /// <para>Greenhouse Gas System Pork Protocol 2006</para>
    /// </summary>
    public class Table_34_Swine_VS_Excretion_For_Diets_Provider
    {
        #region Constructors
        
        public Table_34_Swine_VS_Excretion_For_Diets_Provider()
        {
            HTraceListener.AddTraceListener();
        } 

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Volatile solids excreted (kg VS (kg feed)^-1)</returns>
        public double Get(Province province, AnimalType animalType)
        {
            var byProvince = this.GetByProvince(province);

            return byProvince.FirstOrDefault(x => x.Key == animalType).Value;
        }
        
        public Dictionary<AnimalType, double> GetByProvince(Province province)
        {
            switch (province)
            {
                case Province.Alberta:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.1504},
                        {AnimalType.SwinePiglets, 0.1504},
                        {AnimalType.SwineGrower, 0.1389},
                        {AnimalType.SwineFinisher, 0.1389},
                        {AnimalType.SwineDrySow, 0.1228},
                        {AnimalType.SwineSows, 0.1228},
                        {AnimalType.SwineGilts, 0.1228},
                        {AnimalType.SwineBoar, 0.1228},
                        {AnimalType.SwineLactatingSow, 0.1228}
                    };
                    
                case Province.BritishColumbia:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.1446},
                        {AnimalType.SwinePiglets, 0.1446},
                        {AnimalType.SwineGrower, 0.1391 },
                        {AnimalType.SwineFinisher, 0.1391 },
                        {AnimalType.SwineDrySow, 0.1227},
                        {AnimalType.SwineSows, 0.1227},
                        {AnimalType.SwineGilts, 0.1227},
                        {AnimalType.SwineBoar, 0.1227},
                        {AnimalType.SwineLactatingSow, 0.1227 }
                    };

                case Province.Saskatchewan:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.1292},
                        {AnimalType.SwinePiglets, 0.1292},
                        {AnimalType.SwineGrower,0.1539},
                        {AnimalType.SwineFinisher, 0.1539},
                        {AnimalType.SwineDrySow, 0.1321 },
                        {AnimalType.SwineSows, 0.1321 },
                        {AnimalType.SwineGilts, 0.1321 },
                        {AnimalType.SwineBoar, 0.1321 },
                        {AnimalType.SwineLactatingSow, 0.1321 }
                    };

                case Province.Manitoba:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.1034},
                        {AnimalType.SwinePiglets, 0.1034},
                        {AnimalType.SwineGrower, 0.1514},
                        {AnimalType.SwineFinisher, 0.1514},
                        {AnimalType.SwineDrySow, 0.1406 },
                        {AnimalType.SwineSows, 0.1406 },
                        {AnimalType.SwineGilts, 0.1406 },
                        {AnimalType.SwineBoar, 0.1406 },
                        {AnimalType.SwineLactatingSow, 0.1406 }
                    };
                case Province.Ontario:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.0985},
                        {AnimalType.SwinePiglets, 0.0985},
                        {AnimalType.SwineGrower, 0.1034},
                        {AnimalType.SwineFinisher, 0.1034 },
                        {AnimalType.SwineDrySow, 0.0712},
                        {AnimalType.SwineSows, 0.0712},
                        {AnimalType.SwineGilts, 0.0712},
                        {AnimalType.SwineBoar, 0.0712},
                        {AnimalType.SwineLactatingSow, 0.0712}
                    };

                case Province.Quebec:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.0845},
                        {AnimalType.SwinePiglets, 0.0845},
                        {AnimalType.SwineGrower, 0.1097 },
                        {AnimalType.SwineFinisher, 0.1097 },
                        {AnimalType.SwineDrySow, 0.1053},
                        {AnimalType.SwineSows, 0.1053},
                        {AnimalType.SwineGilts, 0.1053},
                        {AnimalType.SwineBoar, 0.1053},
                        {AnimalType.SwineLactatingSow, 0.1053}
                    };

                case Province.Newfoundland:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.0936},
                        {AnimalType.SwinePiglets, 0.0936},
                        {AnimalType.SwineGrower, 0.1354},
                        {AnimalType.SwineFinisher, 0.1354},
                        {AnimalType.SwineDrySow, 0.1232},
                        {AnimalType.SwineSows, 0.1232},
                        {AnimalType.SwineGilts, 0.1232},
                        {AnimalType.SwineBoar, 0.1232},
                        {AnimalType.SwineLactatingSow, 0.1232}
                    };

                case Province.NovaScotia:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.0886},
                        {AnimalType.SwinePiglets, 0.0886},
                        {AnimalType.SwineGrower, 0.1478 },
                        {AnimalType.SwineFinisher, 0.1478 },
                        {AnimalType.SwineDrySow, 0.1243},
                        {AnimalType.SwineSows, 0.1243},
                        {AnimalType.SwineGilts, 0.1243},
                        {AnimalType.SwineBoar, 0.1243},
                        {AnimalType.SwineLactatingSow, 0.1243 }
                    };
                case Province.NewBrunswick:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.0949},
                        {AnimalType.SwinePiglets, 0.0949},
                        {AnimalType.SwineGrower, 0.1525 },
                        {AnimalType.SwineFinisher, 0.1525},
                        {AnimalType.SwineDrySow, 0.1278 },
                        {AnimalType.SwineSows, 0.1278 },
                        {AnimalType.SwineGilts, 0.1278 },
                        {AnimalType.SwineBoar, 0.1278 },
                        {AnimalType.SwineLactatingSow, 0.1278 }
                    };
                case Province.PrinceEdwardIsland:
                    return new Dictionary<AnimalType, double>
                    {
                        {AnimalType.SwineStarter, 0.0966},
                        {AnimalType.SwinePiglets, 0.0966},
                        {AnimalType.SwineGrower, 0.1470 },
                        {AnimalType.SwineFinisher, 0.1470},
                        {AnimalType.SwineDrySow, 0.1243},
                        {AnimalType.SwineSows, 0.1243},
                        {AnimalType.SwineGilts, 0.1243},
                        {AnimalType.SwineBoar, 0.1243},
                        {AnimalType.SwineLactatingSow, 0.1243}
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
                        {AnimalType.SwineBoar, 0.1243},
                        {AnimalType.SwineLactatingSow, 0}
                    };

                    System.Diagnostics.Trace.TraceError($"{nameof(Table_34_Swine_VS_Excretion_For_Diets_Provider)}.{nameof(Table_34_Swine_VS_Excretion_For_Diets_Provider.GetByProvince)}" +
                    $" unable to get data for province: {province}." +
                    $" Returning default value of {defaultValue}.");
                    return defaultValue;
            }
        }
    }
}


                    
               
               


  
