using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Emissions;
using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.UserInput;
using H.Core;
using H.Core.Emissions.Results;
using H.Core.Models;

namespace H.CLI.Results
{
    /// <summary>
    /// Outputs results to file. Outputs are in MG CO2e for one report and outputs are in Kg GHG for the other report that is written to the output directory.
    /// </summary>
    public class GHGAndCO2Results
    {
        #region Fields

        #endregion

        #region Constructors

        public GHGAndCO2Results(ApplicationData applicationData)
        {
        }

        #endregion

        #region Properties
        //All Farms
        public double TotalEntericMethaneEmissionForAllFarms { get; set; }
        public double TotalManureMethaneEmissionForAllFarms { get; set; }
        public double TotalDirectNitrousOxideEmissionForAllFarms { get; set; }
        public double TotalIndirectNitrousOxideEmissionForAllFarms { get; set; }
        public double TotalEnergyCarbonDioxideEmissionsForAllFarms { get; set; }
        public double Total { get; set; }

        //One Farm
        public double FarmTotalEntericMethaneEmission { get; set; }
        public double FarmTotalManureMethaneEmission { get; set; }
        public double FarmTotalDirectNitrousOxideEmission { get; set; }
        public double FarmTotalIndirectNitrousOxideEmission { get; set; }
        public double FarmTotalEnergyCarbonDioxideEmission { get; set; }
        public string FarmUncertainty { get; set; }
        public double FarmSubTotal { get; set; }

        //One Component
        public double ComponentTotalEntericMethaneEmission { get; set; }
        public double ComponentTotalManureMethaneEmission { get; set; }
        public double ComponentTotalDirectNitrousOxideEmission { get; set; }
        public double ComponentTotalIndirectNitrousOxideEmission { get; set; }
        public double ComponentTotalEnergyCarbonDioxideEmission { get; set; }
        public double ComponentSubTotal { get; set; }

        //One Animal Group
        public double AnimalGroupTotalEntericMethaneEmission { get; set; }
        public double AnimalGroupTotalManureMethaneEmission { get; set; }
        public double AnimalGroupTotalDirectNitrousOxideEmission { get; set; }
        public double AnimalGroupTotalIndirectNitrousOxideEmission { get; set; }
        public double AnimalGroupTotalEnergyCarbonDioxideEmission { get; set; }
        public double AnimalGroupSubTotal { get; set; }

        //One Animal SubGroup
        public double AnimalSubGroupTotalEntericMethaneEmission { get; set; }
        public double AnimalSubGroupTotalManureMethaneEmission { get; set; }
        public double AnimalSubGroupTotalDirectNitrousOxideEmission { get; set; }
        public double AnimalSubGroupTotalIndirectNitrousOxideEmission { get; set; }
        public double AnimalSubGroupTotalEnergyCarbonDioxideEmission { get; set; }
        public double AnimalSubGroupSubTotal { get; set; }

        //Monthly Data
        public double MonthlyEntericMethane { get; set; }
        public double MonthlyManureMethane { get; set; }
        public double MonthlyDirectNitrousOxide { get; set; }
        public double MonthlyIndirectNitrousOxide { get; set; }
        public double MonthTotal { get; set; }

        #endregion
        //Counter
        public double EmissionFileCounter { get; set; }

        #region Calculators
        private readonly Table_68_69_Expression_Of_Uncertainty_Calculator _uncertaintyCalculator = new Table_68_69_Expression_Of_Uncertainty_Calculator();
        #endregion

        #region Constants
        private readonly double entericManureCH4UncertaintyEstimate = CoreConstants.EntericManureCH4UncertaintyEstimate;
        private readonly double manureCH4UncertaintyEstimate = CoreConstants.ManureCH4UncertaintyEstimate;
        private readonly double directN2OUncertaintyEstimate = CoreConstants.DirectN2OUncertaintyEstimate;
        private readonly double indirectN2OUncertaintyEstimate = CoreConstants.IndirectN2OUncertaintyEstimate;
        private readonly double energyCO2UncertaintyEstimate = CoreConstants.EntericManureCH4UncertaintyEstimate;

        private const int roundingDigits = 4;
        #endregion

        /// <summary>
        /// Gets the headers for yearly results based on the emission counter (0 = GHG, 1 = CO2E)
        /// </summary>
        /// <param name="applicationData"></param>
        public string GetHeadersAllFarms(ApplicationData applicationData)
        {
            switch (EmissionFileCounter)
            {
                case 0:
                    return
                    String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.EntericCH4 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ManureCH4 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.DirectN2O + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.IndirectN2O + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.EnergyCO2 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.CO2 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.SubTotal + CLILanguageConstants.Delimiter, applicationData.DisplayUnitStrings.KilogramsGhgs);
                case 1:
                    return
                       String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ComponentGroupName + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.EntericCH4 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.ManureCH4 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.DirectN2O + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.IndirectN2O + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.EnergyCO2 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.CO2 + CLILanguageConstants.Delimiter +
                                                       Properties.Resources.SubTotal + CLILanguageConstants.Delimiter, applicationData.DisplayUnitStrings.MegagramsCO2e);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the headers for monthly results based on the emission counter (0 = GHG, 1 = CO2E)
        /// </summary>
        /// <param name="applicationData"></param>
        public string GetHeadersEachFarmMonthly(ApplicationData applicationData)
        {
            switch (EmissionFileCounter)
            {
                case 0:
                    return String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.GroupType + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.GroupName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.Month + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.EntericCH4 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ManureCH4 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.DirectN2O + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.IndirectN2O + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.EnergyCO2 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.CO2 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.SubTotal + CLILanguageConstants.Delimiter, applicationData.DisplayUnitStrings.KilogramsGhgs);
                case 1:
                    return String.Format(Properties.Resources.FarmName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ComponentCategory + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.GroupType + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.GroupName + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.Month + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.EntericCH4 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.ManureCH4 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.DirectN2O + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.IndirectN2O + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.EnergyCO2 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.CO2 + CLILanguageConstants.Delimiter +
                                                           Properties.Resources.SubTotal + CLILanguageConstants.Delimiter, applicationData.DisplayUnitStrings.MegagramsCO2e);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Takes in a list of the emission results for ALL THE FARMS based on the emission file counter (0 = GHG, 1 = CO2E)
        /// CO2E values get multipled by the appropriate conversion factor
        /// Based on the relevant data, we calculate the Sum of the relevant data for ALL THE FARMS.
        /// </summary>
        public void CalculateTotalsForAllFarms(List<KeyValuePair<string, List<AnimalComponentEmissionsResults>>> _emissionResultsForAllFarms)
        {
            switch (EmissionFileCounter)
            {
                case 0:
                    TotalEntericMethaneEmissionForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.TotalEntericMethaneEmission));
                    TotalManureMethaneEmissionForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.TotalManureMethaneEmission));

                    TotalDirectNitrousOxideEmissionForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.TotalDirectNitrousOxideEmission));
                    TotalIndirectNitrousOxideEmissionForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.TotalIndirectN2OEmission));

                    TotalEnergyCarbonDioxideEmissionsForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.TotalEnergyCarbonDioxide)));

                    Total = TotalEntericMethaneEmissionForAllFarms +
                            TotalManureMethaneEmissionForAllFarms +
                            TotalDirectNitrousOxideEmissionForAllFarms +
                            TotalIndirectNitrousOxideEmissionForAllFarms +
                            TotalEnergyCarbonDioxideEmissionsForAllFarms;
                    break;

                case 1:
                    TotalEntericMethaneEmissionForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.TotalEntericMethaneEmission)) * CoreConstants.CH4ToCO2eConversionFactor;
                    TotalManureMethaneEmissionForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.TotalManureMethaneEmission)) * CoreConstants.CH4ToCO2eConversionFactor;

                    TotalDirectNitrousOxideEmissionForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.TotalDirectNitrousOxideEmission)) * CoreConstants.N2OToCO2eConversionFactor;
                    TotalIndirectNitrousOxideEmissionForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.TotalIndirectN2OEmission)) * CoreConstants.N2OToCO2eConversionFactor;

                    TotalEnergyCarbonDioxideEmissionsForAllFarms = _emissionResultsForAllFarms.Sum(x => x.Value.Sum(y => y.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.TotalEnergyCarbonDioxide)));

                    Total = TotalEntericMethaneEmissionForAllFarms +
                            TotalManureMethaneEmissionForAllFarms +
                            TotalDirectNitrousOxideEmissionForAllFarms +
                            TotalIndirectNitrousOxideEmissionForAllFarms +
                            TotalEnergyCarbonDioxideEmissionsForAllFarms;
                    break;
            }
        }

        /// <summary>
        /// Takes in a list of the emission results for ONE FARM based on the emission file counter (0 = GHG, 1 = CO2E)
        /// CO2E values get multiplied by the appropriate conversion factor
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE FARM.
        /// </summary>
        public void CalculateTotalsForAFarm(IGrouping<string, KeyValuePair<string, List<AnimalComponentEmissionsResults>>> groupedComponentsForAFarm)
        {
            switch (EmissionFileCounter)
            {
                case 0:
                    var farmUncertaintyEmissionsCategoryToUncertaintyValueGHG = new List<Tuple<double, double>>();

                    FarmTotalEntericMethaneEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.TotalEntericMethaneEmission));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueGHG.Add(Tuple.Create(FarmTotalEntericMethaneEmission, entericManureCH4UncertaintyEstimate));

                    FarmTotalManureMethaneEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.TotalManureMethaneEmission));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueGHG.Add(Tuple.Create(FarmTotalManureMethaneEmission, manureCH4UncertaintyEstimate));

                    FarmTotalDirectNitrousOxideEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.TotalDirectNitrousOxideEmission));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueGHG.Add(Tuple.Create(FarmTotalDirectNitrousOxideEmission, directN2OUncertaintyEstimate));

                    FarmTotalIndirectNitrousOxideEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.TotalIndirectN2OEmission));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueGHG.Add(Tuple.Create(FarmTotalDirectNitrousOxideEmission, indirectN2OUncertaintyEstimate));

                    FarmTotalEnergyCarbonDioxideEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.TotalEnergyCarbonDioxide)));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueGHG.Add(Tuple.Create(FarmTotalDirectNitrousOxideEmission, energyCO2UncertaintyEstimate));

                    FarmUncertainty = ConvertUncertaintyValueToPercentage(_uncertaintyCalculator.CalculateUncertaintyAssociatedWithNetFarmEmissionEstimate(farmUncertaintyEmissionsCategoryToUncertaintyValueGHG));

                    FarmSubTotal = FarmTotalEntericMethaneEmission +
                                   FarmTotalManureMethaneEmission +
                                   FarmTotalDirectNitrousOxideEmission +
                                   FarmTotalIndirectNitrousOxideEmission +
                                   FarmTotalEnergyCarbonDioxideEmission;
                    break;
                case 1:
                    var farmUncertaintyEmissionsCategoryToUncertaintyValueCO2 = new List<Tuple<double, double>>();

                    FarmTotalEntericMethaneEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.TotalEntericMethaneEmission));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueCO2.Add(Tuple.Create(FarmTotalEntericMethaneEmission, entericManureCH4UncertaintyEstimate));
                    FarmTotalEntericMethaneEmission = FarmTotalEntericMethaneEmission * CoreConstants.CH4ToCO2eConversionFactor;

                    FarmTotalManureMethaneEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.TotalManureMethaneEmission));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueCO2.Add(Tuple.Create(FarmTotalManureMethaneEmission, manureCH4UncertaintyEstimate));
                    FarmTotalManureMethaneEmission = FarmTotalManureMethaneEmission * CoreConstants.CH4ToCO2eConversionFactor;

                    FarmTotalDirectNitrousOxideEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.TotalDirectNitrousOxideEmission));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueCO2.Add(Tuple.Create(FarmTotalDirectNitrousOxideEmission, directN2OUncertaintyEstimate));
                    FarmTotalDirectNitrousOxideEmission = FarmTotalDirectNitrousOxideEmission * CoreConstants.N2OToCO2eConversionFactor;

                    FarmTotalIndirectNitrousOxideEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.TotalIndirectN2OEmission));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueCO2.Add(Tuple.Create(FarmTotalDirectNitrousOxideEmission, indirectN2OUncertaintyEstimate));
                    FarmTotalIndirectNitrousOxideEmission = FarmTotalIndirectNitrousOxideEmission * CoreConstants.N2OToCO2eConversionFactor;

                    FarmTotalEnergyCarbonDioxideEmission = groupedComponentsForAFarm.Sum(x => x.Value.Sum(y => y.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.TotalEnergyCarbonDioxide)));
                    farmUncertaintyEmissionsCategoryToUncertaintyValueCO2.Add(Tuple.Create(FarmTotalDirectNitrousOxideEmission, energyCO2UncertaintyEstimate));

                    FarmUncertainty = ConvertUncertaintyValueToPercentage(_uncertaintyCalculator.CalculateUncertaintyAssociatedWithNetFarmEmissionEstimate(farmUncertaintyEmissionsCategoryToUncertaintyValueCO2));

                    FarmSubTotal = FarmTotalEntericMethaneEmission +
                                   FarmTotalManureMethaneEmission +
                                   FarmTotalDirectNitrousOxideEmission +
                                   FarmTotalIndirectNitrousOxideEmission +
                                   FarmTotalEnergyCarbonDioxideEmission;
                    break;
            }

        }

        /// <summary>
        /// Takes in a list of the emission results for ONE COMPONENT based on the emission file counter (0 = GHG, 1 = CO2E)
        /// CO2E values get multipled by the appropriate conversion factor
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE COMPONENT
        /// </summary>
        public void CalculateTotalsForOneComponent(IGrouping<ComponentCategory, AnimalComponentEmissionsResults> componentGroup)
        {
            switch (EmissionFileCounter)
            {
                case 0:
                    ComponentTotalEntericMethaneEmission = componentGroup.Sum(x => x.TotalEntericMethaneEmission);
                    ComponentTotalManureMethaneEmission = componentGroup.Sum(x => x.TotalManureMethaneEmission);

                    ComponentTotalDirectNitrousOxideEmission = componentGroup.Sum(x => x.TotalDirectNitrousOxideEmission);
                    ComponentTotalIndirectNitrousOxideEmission = componentGroup.Sum(x => x.TotalIndirectN2OEmission);

                    ComponentTotalEnergyCarbonDioxideEmission = componentGroup.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.TotalEnergyCarbonDioxide));

                    ComponentSubTotal = ComponentTotalEntericMethaneEmission +
                                        ComponentTotalManureMethaneEmission +
                                        ComponentTotalDirectNitrousOxideEmission +
                                        ComponentTotalIndirectNitrousOxideEmission +
                                        ComponentTotalEnergyCarbonDioxideEmission;
                    break;

                case 1:
                    ComponentTotalEntericMethaneEmission = componentGroup.Sum(x => x.TotalEntericMethaneEmission) * CoreConstants.CH4ToCO2eConversionFactor;
                    ComponentTotalManureMethaneEmission = componentGroup.Sum(x => x.TotalManureMethaneEmission) * CoreConstants.CH4ToCO2eConversionFactor;

                    ComponentTotalDirectNitrousOxideEmission = componentGroup.Sum(x => x.TotalDirectNitrousOxideEmission) * CoreConstants.N2OToCO2eConversionFactor;
                    ComponentTotalIndirectNitrousOxideEmission = componentGroup.Sum(x => x.TotalIndirectN2OEmission) * CoreConstants.N2OToCO2eConversionFactor;

                    ComponentTotalEnergyCarbonDioxideEmission = componentGroup.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.TotalEnergyCarbonDioxide));

                    ComponentSubTotal = ComponentTotalEntericMethaneEmission +
                                        ComponentTotalManureMethaneEmission +
                                        ComponentTotalDirectNitrousOxideEmission +
                                        ComponentTotalIndirectNitrousOxideEmission +
                                        ComponentTotalEnergyCarbonDioxideEmission;
                    break;
            }
        }

        /// <summary>
        /// Takes in a list of the emission results for ONE ANIMAL GROUP based on the emission file counter (0 = GHG, 1 = CO2E)
        /// CO2E values get multipled by the appropriate conversion factor
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE ANIMAL GROUP
        /// </summary>
        public void CalculateTotalsForOneAnimalGroup(IGrouping<AnimalType, AnimalGroupEmissionResults> animalGroup)
        {
            switch (EmissionFileCounter)
            {
                case 0:
                    AnimalGroupTotalEntericMethaneEmission = animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyEntericMethaneEmission));
                    AnimalGroupTotalManureMethaneEmission = animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyManureMethaneEmission));

                    AnimalGroupTotalDirectNitrousOxideEmission = animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyManureDirectN2OEmission));
                    AnimalGroupTotalIndirectNitrousOxideEmission = animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyManureIndirectN2OEmission));

                    AnimalGroupTotalEnergyCarbonDioxideEmission = animalGroup.Sum(x => x.TotalEnergyCarbonDioxide);

                    AnimalGroupSubTotal = AnimalGroupTotalEntericMethaneEmission +
                                          AnimalGroupTotalManureMethaneEmission +
                                          AnimalGroupTotalDirectNitrousOxideEmission +
                                          AnimalGroupTotalIndirectNitrousOxideEmission +
                                          AnimalGroupTotalEnergyCarbonDioxideEmission;

                    break;

                case 1:
                    AnimalGroupTotalEntericMethaneEmission = animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyEntericMethaneEmission)) * CoreConstants.CH4ToCO2eConversionFactor;
                    AnimalGroupTotalManureMethaneEmission = animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyManureMethaneEmission)) * CoreConstants.CH4ToCO2eConversionFactor;

                    AnimalGroupTotalDirectNitrousOxideEmission = animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyManureDirectN2OEmission)) * CoreConstants.N2OToCO2eConversionFactor;
                    AnimalGroupTotalIndirectNitrousOxideEmission = animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyManureIndirectN2OEmission)) * CoreConstants.N2OToCO2eConversionFactor;

                    AnimalGroupTotalEnergyCarbonDioxideEmission = animalGroup.Sum(x => x.TotalEnergyCarbonDioxide);

                    AnimalGroupSubTotal = AnimalGroupTotalEntericMethaneEmission +
                                          AnimalGroupTotalManureMethaneEmission +
                                          AnimalGroupTotalDirectNitrousOxideEmission +
                                          AnimalGroupTotalIndirectNitrousOxideEmission +
                                          AnimalGroupTotalEnergyCarbonDioxideEmission;

                    break;

            }
        }

        public void CalculateTotalsForOneAnimalSubGroup(AnimalGroupEmissionResults animalSubGroup)
        {
            switch (EmissionFileCounter)
            {
                case 0:
                    AnimalSubGroupTotalEntericMethaneEmission = animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyEntericMethaneEmission);
                    AnimalSubGroupTotalManureMethaneEmission = animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyManureMethaneEmission);

                    AnimalSubGroupTotalDirectNitrousOxideEmission = animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyManureDirectN2OEmission);
                    AnimalSubGroupTotalIndirectNitrousOxideEmission = animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyManureIndirectN2OEmission);

                    AnimalSubGroupSubTotal = AnimalSubGroupTotalEntericMethaneEmission +
                                             AnimalSubGroupTotalManureMethaneEmission +
                                             AnimalSubGroupTotalDirectNitrousOxideEmission +
                                             AnimalSubGroupTotalIndirectNitrousOxideEmission;
                    break;

                case 1:
                    AnimalSubGroupTotalEntericMethaneEmission = animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyEntericMethaneEmission) * CoreConstants.CH4ToCO2eConversionFactor;
                    AnimalSubGroupTotalManureMethaneEmission = animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyManureMethaneEmission) * CoreConstants.CH4ToCO2eConversionFactor;

                    AnimalSubGroupTotalDirectNitrousOxideEmission = animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyManureDirectN2OEmission) * CoreConstants.N2OToCO2eConversionFactor;
                    AnimalSubGroupTotalIndirectNitrousOxideEmission = animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyManureIndirectN2OEmission) * CoreConstants.N2OToCO2eConversionFactor;

                    AnimalSubGroupSubTotal = AnimalSubGroupTotalEntericMethaneEmission +
                                             AnimalSubGroupTotalManureMethaneEmission +
                                             AnimalSubGroupTotalDirectNitrousOxideEmission +
                                             AnimalSubGroupTotalIndirectNitrousOxideEmission;

                    break;
            }
        }

        /// <summary>
        /// Takes in a list of the emission results for ONE MONTH based on the emission file counter (0 = GHG, 1 = CO2E)
        /// CO2E values get multipled by the appropriate conversion factor
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE MONTH
        /// </summary>
        public void CalculateMonthlyEmissionsForAnimalGroup(IGrouping<int, GroupEmissionsByMonth> monthlyEmissions)
        {
            switch (EmissionFileCounter)
            {
                case 0:
                    MonthlyEntericMethane = monthlyEmissions.Sum(x => x.MonthlyEntericMethaneEmission);
                    MonthlyManureMethane = monthlyEmissions.Sum(x => x.MonthlyManureMethaneEmission);

                    MonthlyDirectNitrousOxide = monthlyEmissions.Sum(x => x.MonthlyManureDirectN2OEmission);
                    MonthlyIndirectNitrousOxide = monthlyEmissions.Sum(x => x.MonthlyManureIndirectN2OEmission);

                    MonthTotal = MonthlyEntericMethane + MonthlyManureMethane + MonthlyDirectNitrousOxide + MonthlyIndirectNitrousOxide;

                    break;
                case 1:
                    MonthlyEntericMethane = monthlyEmissions.Sum(x => x.MonthlyEntericMethaneEmission) * CoreConstants.CH4ToCO2eConversionFactor;
                    MonthlyManureMethane = monthlyEmissions.Sum(x => x.MonthlyManureMethaneEmission) * CoreConstants.CH4ToCO2eConversionFactor;

                    MonthlyDirectNitrousOxide = monthlyEmissions.Sum(x => x.MonthlyManureDirectN2OEmission) * CoreConstants.N2OToCO2eConversionFactor;
                    MonthlyIndirectNitrousOxide = monthlyEmissions.Sum(x => x.MonthlyManureIndirectN2OEmission) * CoreConstants.N2OToCO2eConversionFactor;

                    MonthTotal = MonthlyEntericMethane +
                                 MonthlyManureMethane +
                                 MonthlyDirectNitrousOxide +
                                 MonthlyIndirectNitrousOxide;

                    break;
            }
        }

        /// <summary>
        /// Returns the appropriate uncertainty value percentage for each Farm.
        /// </summary>
        public string ConvertUncertaintyValueToPercentage(double uncertaintyValue)
        {
            switch (Math.Floor(uncertaintyValue))
            {
                case 1:
                    return "+/- <20%";
                case 2:
                    return "+/- <40%";
                case 3:
                    return "+/- <60%";
                case 4:
                    return "+/- >60%";
                default:
                    return "N/A";
            }
        }

    }
}
