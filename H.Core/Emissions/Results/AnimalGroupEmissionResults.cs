using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Emissions.Results
{
    /// <summary>
    /// A class to hold emission results for an animal group and all the management periods for the animal group. This class holds a collection of
    /// <see cref="GroupEmissionsByMonths"/> which is a collection of emissions results for the <see cref="AnimalGroup"/> for each month.
    /// </summary>
    public class AnimalGroupEmissionResults
    {
        #region Fields
        
        private readonly EmissionTypeConverter _emissionsConverter; 

        #endregion

        #region Constructors

        public AnimalGroupEmissionResults()
        {
            _emissionsConverter = new EmissionTypeConverter();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The <see cref="AnimalGroup"/> that caused the emissions.
        /// </summary>
        public AnimalGroup AnimalGroup { get; set; }

        /// <summary>
        /// A collection of emissions for each month created by the <see cref="AnimalGroup"/>.
        /// </summary>
        public List<GroupEmissionsByMonth> GroupEmissionsByMonths { get; set; } = new List<GroupEmissionsByMonth>();

        /// <summary>
        /// Total emissions (all types) emitted from all animals in this group over all months.
        ///
        /// (kg CO2e)
        /// </summary>
        public double TotalCarbonDioxideEquivalentEmissionsFromAnimalGroup
        {
            get
            {
                return _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e, this.TotalEntericMethane) +
                       _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsCH4, EmissionDisplayUnits.KilogramsC02e, this.TotalManureMethane) +
                       _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e, this.TotalDirectNitrousOxide) +
                       _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsN2O, EmissionDisplayUnits.KilogramsC02e, this.TotalIndirectNitrousOxide) +
                       _emissionsConverter.Convert(EmissionDisplayUnits.KilogramsC02, EmissionDisplayUnits.KilogramsC02e, this.TotalEnergyCarbonDioxide);
            }
        }

        /// <summary>
        /// (kg CO2)
        /// </summary>
        public double TotalCarbonDioxideFromAnimals
        {
            get
            {
                return this.TotalEnergyCarbonDioxide;
            }
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double TotalCarbonUptakeByAnimals()
        {
            return GroupEmissionsByMonths.Sum(groupEmisionsByMonth => groupEmisionsByMonth.TotalMonthlyCarbonUptake);
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double TotalDryMatterIntake
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(x => x.DryMatterIntake);
            }
        }

        /// <summary>
        /// The total enteric methane emitted from all animals in this group over all months.
        /// 
        /// (kg CH4)
        /// </summary>
        public double TotalEntericMethane 
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(groupEmissionsByMonth => groupEmissionsByMonth.MonthlyEntericMethaneEmission);
            }
        }

        /// <summary>
        /// The total manure methane emitted from all animals in this group over all months.
        /// 
        /// (kg CH4)
        /// </summary>
        public double TotalManureMethane
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(groupEmissionsByMonth => groupEmissionsByMonth.MonthlyManureMethaneEmission);
            }
        }

        public double TotalVolumeOfManure
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(groupEmissionsByMonth => groupEmissionsByMonth.TotalVolumeOfManureAvailableForLandApplication);
            }
        }

        /// <summary>
        /// Total direct nitrous oxide emitted from all animals in this group over all months.
        ///
        /// (kg N2O)
        /// </summary>
        public double TotalDirectNitrousOxide
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(groupEmissionsByMonth => groupEmissionsByMonth.MonthlyManureDirectN2OEmission);
            }
        }

        /// <summary>
        /// Total indirect nitrous oxide emitted from all animals in this group over all months.
        ///
        /// (kg N2O)
        /// </summary>
        public double TotalIndirectNitrousOxide
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(groupEmissionsByMonth => groupEmissionsByMonth.MonthlyManureIndirectN2OEmission);
            }
        }

        /// <summary>
        /// Total energy carbon dioxide emitted from all animals in this group over all months.
        ///
        /// (kg CO2)
        /// </summary>
        public double TotalEnergyCarbonDioxide
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(groupEmissionsByMonth => groupEmissionsByMonth.MonthlyEnergyCarbonDioxide);
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double TotalNitrogenAvailableForLandApplication
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(groupEmissionsByMonth => groupEmissionsByMonth.MonthlyNitrogenAvailableForLandApplication);
            }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double TotalCarbonExcreted
        {
            get
            {
                return this.GroupEmissionsByMonths.Sum(groupEmissionsByMonth => groupEmissionsByMonth.MonthlyFecalCarbonExcretion);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines if the animals are being fed a poor quality diet by checking each <see cref="GroupEmissionsByDay"/> to see if the DMI > DMI_max.
        ///
        /// DMI over check needs to consider JUST the dates that the animals have the specific diet
        /// </summary>
        /// <param name="managementPeriod"></param>
        public bool IsDmiOverDmiMaxForPeriod(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null)
            {
                foreach (var groupEmissionsByMonth in this.GroupEmissionsByMonths)
                {
                    var dates = groupEmissionsByMonth.GetDatesWhereDmiIsGreaterThanDmiMax().Where(x => x >= managementPeriod.Start && x <= managementPeriod.End);
                    if (dates.Any())
                    {
                        return true;
                    }
                } 
            }

            return false;
        }

        /// <summary>
        /// Get a list of dates when the animal group has a DMI > DMI_max
        /// </summary>
        public List<DateTime> GetDatesWhenGroupIsOverDmiMax()
        {
            var result = new List<DateTime>();

            foreach (var groupEmissionsForMonth in this.GroupEmissionsByMonths)
            {
                result.AddRange(groupEmissionsForMonth.GetDatesWhereDmiIsGreaterThanDmiMax());
            }

            return result;
        }

        public override string ToString()
        {
            return $"{nameof(AnimalGroup)}: {AnimalGroup}, {nameof(TotalEnergyCarbonDioxide)}: {TotalEnergyCarbonDioxide}, {nameof(GroupEmissionsByMonths)}: {GroupEmissionsByMonths}";
        }

        #endregion
    }
}