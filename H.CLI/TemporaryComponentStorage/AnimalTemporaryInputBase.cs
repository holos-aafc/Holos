using System;
using H.Core.Enumerations;

namespace H.CLI.TemporaryComponentStorage
{
    public abstract class AnimalTemporaryInputBase : TemporaryInputBase
    {
        #region Properties
        
        public string GroupName { get; set; }
        public int NumberOfAnimals { get; set; }
        public string ManagementPeriodName { get; set; }
        public DateTime ManagementPeriodStartDate { get; set; }
        public DateTime ManagementPeriodEndDate { get; set; }
        public int ManagementPeriodDays { get; set; }
        public double N2ODirectEmissionFactor { get; set; }
        public double VolatilizationFraction { get; set; }
        public double YearlyManureMethaneRate { get; set; }
        public double YearlyNitrogenExcretionRate { get; set; }
        public double YearlyEntericMethaneRate { get; set; }
        public int GroupId { get; set; }
        public AnimalType GroupType { get; set; }
        public double DailyManureMethaneEmissionRate { get; set; }
        public double MethaneProducingCapacityOfManure { get; set; }
        public double MethaneConversionFactor { get; set; }
        public double VolatileSolids { get; set; } 

        #endregion
    }
}