using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using H.Core.Converters;
using H.Core.Enumerations;

namespace H.Core.Emissions.Results
{
    public static class GroupEmissionsByMonthExtensions
    {
        private static readonly EmissionTypeConverter _emissionsConverter;

        static GroupEmissionsByMonthExtensions()
        {
            _emissionsConverter = new EmissionTypeConverter();
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public static double TotalMonthlyEntericMethane(this IEnumerable<GroupEmissionsByMonth> emissionsByMonth)
        {
            return emissionsByMonth.Sum(x => x.MonthlyEntericMethaneEmission);
        }

        /// <summary>
        /// (kg CH4)
        /// </summary>
        public static double TotalMonthlyManureMethane(this IEnumerable<GroupEmissionsByMonth> emissionsByMonth)
        {
            return emissionsByMonth.Sum(x => x.MonthlyManureMethaneEmission);
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public static double TotalDirectNitrousOxide(this IEnumerable<GroupEmissionsByMonth> emissionsByMonth)
        {
            return emissionsByMonth.Sum(x => x.MonthlyDirectN2OEmission);
        }

        /// <summary>
        /// (kg N2O)
        /// </summary>
        public static double TotalIndirectNitrousOxide(this IEnumerable<GroupEmissionsByMonth> emissionsByMonth)
        {
            return emissionsByMonth.Sum(x => x.MonthlyIndirectN2OEmission);
        }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public static double TotalEnergyCarbonDioxide(this IEnumerable<GroupEmissionsByMonth> emissionsByMonth)
        {
            return emissionsByMonth.Sum(x => x.MonthlyEnergyCarbonDioxide);
        }

        public static double TotalEmissionsAsCarbonDioxideEquivalents(this IEnumerable<GroupEmissionsByMonth> emissionsByMonths)
        {
            return _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e, emissionsByMonths.TotalMonthlyEntericMethane()) +
                   _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e, emissionsByMonths.TotalMonthlyManureMethane()) +
                   _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e, emissionsByMonths.TotalDirectNitrousOxide()) +
                   _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e, emissionsByMonths.TotalIndirectNitrousOxide()) +
                   _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsC02, EmissionDisplayUnits.KilogramsC02e, emissionsByMonths.TotalEnergyCarbonDioxide());
        }
    }
}