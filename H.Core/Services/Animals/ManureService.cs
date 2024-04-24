using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Services.LandManagement;
using H.Infrastructure;

namespace H.Core.Services.Animals
{
    /// <summary>
    /// Keep manure calculations separate from <see cref="FarmResultsService"/>
    /// </summary>
    public class ManureService : IManureService
    {
        #region Fields

        private readonly ManureHandlingSystemProvider _manureHandlingSystemProvider = new ManureHandlingSystemProvider();
        private readonly List<ManureTank> _manureTanks;
        private readonly List<AnimalType> _validManureTypes = new List<AnimalType>()
        {
            AnimalType.NotSelected,
            AnimalType.Beef,
            AnimalType.Dairy,
            AnimalType.Swine,
            AnimalType.Sheep,
            AnimalType.Poultry,
            AnimalType.Llamas,
            AnimalType.Alpacas,
            AnimalType.Deer,
            AnimalType.Elk,
            AnimalType.Goats,
            AnimalType.Horses,
            AnimalType.Mules,
            AnimalType.Bison,
        };

        private readonly List<ManureApplicationTypes> _validManureApplicationTypes = new List<ManureApplicationTypes>()
        {
            ManureApplicationTypes.TilledLandSolidSpread,
            ManureApplicationTypes.UntilledLandSolidSpread,
            ManureApplicationTypes.SlurryBroadcasting,
            ManureApplicationTypes.DropHoseBanding,
            ManureApplicationTypes.ShallowInjection,
            ManureApplicationTypes.DeepInjection,
        };

        private readonly List<ManureLocationSourceType> _validManureLocationSourceTypes = new List<ManureLocationSourceType>()
        {
            ManureLocationSourceType.NotSelected,
            ManureLocationSourceType.Livestock,
            ManureLocationSourceType.Imported,
        };

        private readonly IMapper _manureCompositionMapper;
        private List<AnimalComponentEmissionsResults> _animalComponentEmissionsResults;

        #endregion

        #region Constructors

        public ManureService()
        {
            _manureTanks = new List<ManureTank>();

            var manureCompositionMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<DefaultManureCompositionData, DefaultManureCompositionData>()
                    .ForMember(y => y.Guid, z => z.Ignore());
            });

            _manureCompositionMapper = manureCompositionMapperConfiguration.CreateMapper();
        }

        #endregion

        #region Public Methods

        public double GetFractionOfTotalManureUsedFromLandApplication(CropViewItem cropViewItem, ManureApplicationViewItem manureApplicationViewItem)
        {
            var totalVolumeOfManureCreated = this.GetTotalVolumeCreated(manureApplicationViewItem.DateOfApplication.Year, manureApplicationViewItem.AnimalType);
            var totalVolumeFromApplication = manureApplicationViewItem.AmountOfManureAppliedPerHectare * cropViewItem.Area;

            if (totalVolumeOfManureCreated == 0)
            {
                return 0;
            }
            else
            {
                return totalVolumeFromApplication / totalVolumeOfManureCreated;
            }
        }

        public double GetFractionOfTotalManureUsedFromExports(double amountExported, AnimalType animalType, int year)
        {
            var result = 0d;

            var totalVolumeOfManureCreated = this.GetTotalVolumeCreated(year, animalType);

            result = amountExported / totalVolumeOfManureCreated;

            return result;
        }

        public double GetAmountOfTanUsedDuringLandApplication(CropViewItem cropViewItem, ManureApplicationViewItem manualApplicationViewItem)
        {
            var fractionUsed = this.GetFractionOfTotalManureUsedFromLandApplication(cropViewItem, manualApplicationViewItem);
            var totalTANCreated = this.GetTotalTANCreated(manualApplicationViewItem.DateOfApplication.Year, manualApplicationViewItem.AnimalType);


            return fractionUsed * totalTANCreated;
        }

        public double GetAmountOfTanUsedDuringLandApplications(CropViewItem cropViewItem)
        {
            var result = 0d;

            foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
            {
                result += this.GetAmountOfTanUsedDuringLandApplication(cropViewItem, manureApplicationViewItem);
            }

            return result;
        }

        public double GetTotalTANExportedByAnimalType(
            AnimalType animalType,
            Farm farm,
            int year)
        {
            var tanExportedByFarm = this.GetTANExportedForFarm(farm, year);
            var tanByAnimalType = tanExportedByFarm.SingleOrDefault(x => x.Item2.GetCategory() == animalType.GetCategory());
            if (tanByAnimalType != null)
            {
                return tanByAnimalType.Item1;
            }
            else
            {
                return 0;
            }
        }

        public List<Tuple<double, AnimalType>> GetTANExportedForFarm(
            Farm farm,
            int year)
        {
            var result = new List<Tuple<double, AnimalType>>();

            var typesOfManureExported = this.GetManureTypesExported(farm, year);
            foreach (var typeExported in typesOfManureExported)
            {
                var tanByType = 0d;
                var exportsByType = farm.ManureExportViewItems.Where(x => x.AnimalType.GetCategory() == typeExported);
                foreach (var manureExportViewItem in exportsByType)
                {
                    tanByType += this.GetAmountOfTanExported(manureExportViewItem, year);
                }

                result.Add(new Tuple<double, AnimalType>(tanByType, typeExported));
            }

            return result;
        }

        public List<int> GetYearsWithManureApplied(Farm farm)
        {
            var s = farm.GetFieldSystemDetailsStageState();

            throw new NotImplementedException();
        }

        public double GetAmountOfTanExported(ManureExportViewItem manureExportViewItem, int year)
        {
            var result = 0d;

            var fractionUsed = this.GetFractionOfTotalManureUsedFromExports(manureExportViewItem.Amount, manureExportViewItem.AnimalType, year);
            var totalTanCreated = this.GetTotalTANCreated(manureExportViewItem.DateOfExport.Year, manureExportViewItem.AnimalType);

            result = fractionUsed * totalTanCreated;

            return result;
        }

        public double GetTotalVolumeOfManureExported(int year, Farm farm, AnimalType animalType)
        {
            var total = 0d;

            total = farm.ManureExportViewItems.Where(x => x.DateOfExport.Year == year && x.AnimalType == animalType).Sum(y => y.Amount);

            return total;
        }

        public double GetTotalVolumeOfManureExported(int year, Farm farm)
        {
            var total = 0d;

            total = farm.ManureExportViewItems.Where(x => x.DateOfExport.Year == year).Sum(y => y.Amount);

            return total;
        }

        public List<ManureApplicationTypes> GetValidManureApplicationTypes()
        {
            return _validManureApplicationTypes;
        }

        public List<AnimalType> GetValidManureTypes()
        {
            return _validManureTypes;
        }

        public List<AnimalType> GetManureCategoriesProducedOnFarm(Farm farm)
        {
            var animalTypes = new List<AnimalType>();

            foreach (var animalComponent in farm.AnimalComponents)
            {
                foreach (var animalComponentGroup in animalComponent.Groups)
                {
                    var groupType = animalComponentGroup.GroupType;
                    var category = groupType.GetCategory();

                    if (animalTypes.Contains(category) == false && groupType != AnimalType.NotSelected)
                    {
                        animalTypes.Add(category);
                    }
                }
            }

            return animalTypes;
        }

        public List<ManureStateType> GetManureStatesProducedOnFarm(Farm farm)
        {
            var animalTypes = new List<ManureStateType>();

            foreach (var animalComponent in farm.AnimalComponents)
            {
                foreach (var animalComponentGroup in animalComponent.Groups)
                {
                    foreach (var managementPeriod in animalComponentGroup.ManagementPeriods)
                    {
                        var stateType = managementPeriod.ManureDetails.StateType;
                        if (animalTypes.Contains(stateType) == false && stateType != ManureStateType.NotSelected)
                        {
                            animalTypes.Add(stateType);
                        }
                    }
                }
            }

            return animalTypes;
        }

        public List<AnimalType> GetManureTypesImported(Farm farm, int year)
        {
            var animalTypes = new List<AnimalType>();

            var viewItemsForYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in viewItemsForYear)
            {
                foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems.Where(x => x.IsImportedManure() && x.DateOfApplication.Year == year))
                {
                    var category = manureApplicationViewItem.AnimalType.GetCategory();
                    if (animalTypes.Contains(category) == false)
                    {
                        animalTypes.Add(category);
                    }
                }
            }

            return animalTypes;
        }

        public List<AnimalType> GetManureTypesExported(Farm farm, int year)
        {
            var animalTypes = new List<AnimalType>();

            foreach (var farmManureExportViewItem in farm.ManureExportViewItems.Where(x => x.DateOfExport.Year == year))
            {
                var category = farmManureExportViewItem.AnimalType.GetCategory();
                if (animalTypes.Contains(category) == false)
                {
                    animalTypes.Add(category);
                }
            }

            return animalTypes;
        }

        public List<ManureLocationSourceType> GetValidManureLocationSourceTypes()
        {
            return _validManureLocationSourceTypes;
        }

        public List<ManureStateType> GetValidManureStateTypes(
            Farm farm,
            ManureLocationSourceType locationSourceType,
            AnimalType animalType)
        {
            var result = new List<ManureStateType>();

            if (farm == null)
            {
                return result;
            }

            // If manure is imported, all options should be made available to the user
            if (locationSourceType == ManureLocationSourceType.Imported)
            {
                var typesForAnimal = _manureHandlingSystemProvider.GetValidManureStateTypesByAnimalCategory(animalType).Where(x => x != ManureStateType.Pasture);
                result.AddRange(typesForAnimal);
            }
            else
            {
                var typesForExistingSystems = farm.GetManureStateTypesInUseOnFarm(animalType);
                result.AddRange(typesForExistingSystems);
            }

            return result;
        }

        /// <summary>
        /// When a user adds a manure application, the choices available for the handling system will vary depending in the animal type (beef, dairy, etc.)
        /// </summary>
        public void SetValidManureStateTypes(ManureItemBase manureItemBase, Farm farm)
        {
            // Can't collect manure from a field and apply to another field or export it
            var types = this.GetValidManureStateTypes(farm, manureItemBase.ManureLocationSourceType, manureItemBase.AnimalType).Where(x => x != ManureStateType.Pasture);
            manureItemBase.ValidManureStateTypesForSelectedTypeOfAnimalManure.UpdateItems(types);

            // Set the selected item to the first item in the updated list
            manureItemBase.ManureStateType = manureItemBase.ValidManureStateTypesForSelectedTypeOfAnimalManure.FirstOrDefault();
        }

        public double GetVolumeAvailableForExport(int year)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year);
            foreach (var manureTank in tank)
            {
                amount += manureTank.VolumeRemainingInTank;
            }

            return amount;
        }

        public double GetTotalVolumeCreated(int year, AnimalType animalType)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year && x.AnimalType.GetCategory() == animalType.GetCategory());
            foreach (var manureTank in tank)
            {
                amount += manureTank.VolumeOfManureAvailableForLandApplication;
            }

            return amount;
        }

        public double GetTotalTANCreated(int year)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year);
            foreach (var manureTank in tank)
            {
                amount += manureTank.TotalTanAvailableForLandApplication;
            }

            return amount;
        }

        public double GetTotalTANCreated(int year, AnimalType animalType)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year && x.AnimalType.GetCategory() == animalType.GetCategory());
            foreach (var manureTank in tank)
            {
                amount += manureTank.TotalTanAvailableForLandApplication;
            }

            return amount;
        }

        public double GetTotalNitrogenCreated(int year)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year);
            foreach (var manureTank in tank)
            {
                amount += manureTank.TotalNitrogenAvailableForLandApplication;
            }

            return amount;
        }

        public double GetTotalCarbonCreated(int year)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year);
            foreach (var manureTank in tank)
            {
                amount += manureTank.TotalAmountOfCarbonInStoredManure;
            }

            return amount;
        }

        public double GetTotalCarbonFromImportedManure(Farm farm, int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                result += cropViewItem.GetTotalCarbonFromAppliedManure(ManureLocationSourceType.Imported);
            }

            return result;
        }

        public double GetTotalCarbonInputsFromLivestockManureApplications(Farm farm, int year)
        {
            var items = farm.GetCropViewItemsByYear(year);

            var result = 0d;

            foreach (var cropViewItem in items)
            {
                result += cropViewItem.GetTotalCarbonFromAppliedManure(ManureLocationSourceType.Livestock);
            }

            return result;
        }

        public double GetTotalCarbonRemainingForFarm(Farm farm, int year, CropViewItem viewItem)
        {
            var result = 0d;

            var totalCarbonCreated = this.GetTotalCarbonCreated(year);
            var totalCarbonImported = this.GetTotalCarbonFromImportedManure(farm, year);
            var totalCarbonExported = this.GetTotalCarbonFromExportedManure(year, farm);
            var totalCarbonApplied = this.GetTotalCarbonInputsFromLivestockManureApplications(farm, year);

            // If noting was created on the farm then there is nothing remaining for imported manure since it is all applied when imported
            if (totalCarbonCreated == 0 &&  totalCarbonImported > 0)
            {
                return 0;
            }

            result = totalCarbonCreated + totalCarbonImported - totalCarbonApplied - totalCarbonExported;
            if (result < 0)
            {
                return 0;
            }

            return result;
        }

        public double GetTotalCarbonRemainingForField(Farm farm, int year, CropViewItem viewItem)
        {
            var totalRemainingForFarm = this.GetTotalCarbonRemainingForFarm(farm, year, viewItem);

            var totalAreaOfFarm = farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, viewItem.Year);
            var result = totalRemainingForFarm * (viewItem.Area / totalAreaOfFarm);

            return result;
        }

        public double GetTotalManureCarbonInputsForField(Farm farm, int year, CropViewItem viewItem)
        {
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var inputsFromLocalManure = 0d;

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return 0;
            }

            if (field.HasLivestockManureApplicationsInYear(year))
            {
                inputsFromLocalManure = viewItem.GetTotalCarbonFromAppliedManure(ManureLocationSourceType.Livestock);
            }

            var inputsFromImportedManure = 0d;
            if (field.HasImportedManureApplicationsInYear(year))
            {
                inputsFromImportedManure = viewItem.GetTotalCarbonFromAppliedManure(ManureLocationSourceType.Imported);
            }

            var remaining = this.GetTotalCarbonRemainingForField(farm, year, viewItem);

            var result = remaining + inputsFromImportedManure + inputsFromLocalManure;

            return result / viewItem.Area;
        }

        public double GetTotalNitrogenCreated(int year, AnimalType animalType)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year && x.AnimalType.GetCategory() == animalType.GetCategory());
            foreach (var manureTank in tank)
            {
                amount += manureTank.TotalNitrogenAvailableForLandApplication;
            }

            return amount;
        }

        public double GetTotalVolumeCreated(int year)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year);
            foreach (var manureTank in tank)
            {
                amount += manureTank.VolumeOfManureAvailableForLandApplication;
            }

            return amount;
        }

        public double GetTotalNitrogenAppliedToAllFields(int year)
        {
            var amount = 0d;

            

            var tank = _manureTanks.Where(x => x.Year == year);
            foreach (var manureTank in tank)
            {
                amount += manureTank.NitrogenSumOfAllManureApplicationsMade;
            }

            return amount;
        }

        public double GetTotalNitrogenAppliedToAllFields(int year, AnimalType animalType)
        {
            var amount = 0d;

            var tank = _manureTanks.Where(x => x.Year == year && x.AnimalType.GetCategory() == animalType.GetCategory());
            foreach (var manureTank in tank)
            {
                amount += manureTank.NitrogenSumOfAllManureApplicationsMade;
            }

            return amount;
        }

        public List<Tuple<double, AnimalType>> GetTotalTanAppliedToField(int year, CropViewItem cropViewItem)
        {
            var results = new List<Tuple<double, AnimalType>>();

            var typesOfManureAppliedToThisField = cropViewItem.ManureApplicationViewItems.Select(x => x.AnimalType);
            foreach (var animalType in typesOfManureAppliedToThisField)
            {
                var totalByAnimalType = 0d;

                var manureApplicationsByType = cropViewItem.ManureApplicationViewItems.Where(x => x.AnimalType == animalType);
                foreach (var manureApplicationViewItem in manureApplicationsByType)
                {
                    var totalTANCreated = this.GetTotalTANCreated(year, manureApplicationViewItem.AnimalType);
                    var fractionUsed = this.GetFractionOfTotalManureUsedFromLandApplication(cropViewItem, manureApplicationViewItem);

                    totalByAnimalType += totalTANCreated * fractionUsed;
                }

                results.Add(new Tuple<double, AnimalType>(totalByAnimalType, animalType));
            }

            return results;
        }

        public List<Tuple<double, AnimalType>> GetTotalTanAppliedToAllFields(int year, List<CropViewItem> viewItems)
        {
            var result = new List<Tuple<double, AnimalType>>();

            foreach (var cropViewItem in viewItems)
            {
                var resultsForField = this.GetTotalTanAppliedToField(year, cropViewItem);
                result.AddRange(resultsForField);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-4
        ///
        /// (kg N)
        /// </summary>
        public double GetTotalManureNitrogenRemainingForFarmAndYear(int year, Farm farm)
        {
            var totalAvailableNitrogen = this.GetTotalNitrogenCreated(year);

            var items = farm.GetCropViewItemsByYear(year, false);
            var localSourcedNitrogenApplied = 0d;
            var importedNitrogenApplied = 0d;
            foreach (var cropViewItem in items)
            {
                foreach (var manureApplicationViewItem in cropViewItem.GetLocalSourcedApplications(year))
                {
                    localSourcedNitrogenApplied += manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area;
                }

                foreach (var manureApplicationViewItem in cropViewItem.GetManureImportsByYear(year))
                {
                    importedNitrogenApplied += manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area;
                }
            }

            var totalAppliedNitrogen = localSourcedNitrogenApplied;//this.GetTotalNitrogenAppliedToAllFields(year);
            var totalExportedNitrogen = this.GetTotalNitrogenFromExportedManure(year, farm);

            // If all manure used was imported and none from local sources were used or created then there is no remaining N since all imports are used
            if (totalAvailableNitrogen == 0 && totalAppliedNitrogen == 0 && importedNitrogenApplied > 0)
            {
                return 0;
            }

            return totalAvailableNitrogen - (totalAppliedNitrogen  - importedNitrogenApplied)- totalExportedNitrogen;
        }

        public double GetTotalVolumeRemainingForFarmAndYear(int year, Farm farm)
        {
            var totalAvailableVolume = this.GetTotalVolumeCreated(year);

            var items = farm.GetCropViewItemsByYear(year, false);
            var localSourcedVolumeApplied = 0d;
            var importedVolumeApplied = 0d;
            foreach (var cropViewItem in items)
            {
                foreach (var manureApplicationViewItem in cropViewItem.GetLocalSourcedApplications(year))
                {
                    localSourcedVolumeApplied += manureApplicationViewItem.AmountOfManureAppliedPerHectare * cropViewItem.Area;
                }

                foreach (var manureApplicationViewItem in cropViewItem.GetManureImportsByYear(year))
                {
                    importedVolumeApplied += manureApplicationViewItem.AmountOfManureAppliedPerHectare * cropViewItem.Area;
                }
            }

            var totalVolumeApplied = localSourcedVolumeApplied;
            var totalExportedVolume = this.GetTotalVolumeFromExportedManure(year, farm);

            // If all manure used was imported and none from local sources were used or created then there is no remaining N since all imports are used
            if (totalAvailableVolume == 0 && totalVolumeApplied == 0 && importedVolumeApplied > 0)
            {
                return 0;
            }

            return totalAvailableVolume - (totalVolumeApplied - importedVolumeApplied) - totalExportedVolume;
        }

        public double GetTotalNitrogenFromExportedManure(int year, Farm farm)
        {
            var result = 0d;

            foreach (var manureExportViewItem in farm.ManureExportViewItems.Where(x => x.DateOfExport.Year == year))
            {
                var nitrogenContent = 0d;
                var amountOfManure = manureExportViewItem.Amount;
                if (manureExportViewItem.DefaultManureCompositionData != null)
                {
                    nitrogenContent = manureExportViewItem.DefaultManureCompositionData.NitrogenContent;
                }

                result += (amountOfManure * nitrogenContent);
            }

            return result;
        }

        public double GetTotalVolumeFromExportedManure(int year, Farm farm)
        {
            var result = 0d;

            foreach (var manureExportViewItem in farm.ManureExportViewItems.Where(x => x.DateOfExport.Year == year))
            {
                var amountOfManure = manureExportViewItem.Amount;

                result += amountOfManure;
            }

            return result;
        }

        public double GetTotalCarbonFromExportedManure(int year, Farm farm)
        {
            var result = 0d;

            foreach (var manureExportViewItem in farm.ManureExportViewItems.Where(x => x.DateOfExport.Year == year))
            {
                var carbonContent = 0d;
                var amountOfManure = manureExportViewItem.Amount;
                if (manureExportViewItem.DefaultManureCompositionData != null)
                {
                    carbonContent = manureExportViewItem.DefaultManureCompositionData.CarbonContent;
                }

                result += (amountOfManure * carbonContent);
            }

            return result;
        }


        public double GetTotalNitrogenFromExportedManure(int year, Farm farm, AnimalType animalType)
        {
            var result = 0d;

            foreach (var manureExportViewItem in farm.ManureExportViewItems.Where(x => x.AnimalType.GetCategory() == animalType.GetCategory() && x.DateOfExport.Year == year))
            {
                var nitrogenContent = 0d;
                var amountOfManure = manureExportViewItem.Amount;
                if (manureExportViewItem.DefaultManureCompositionData != null)
                {
                    nitrogenContent = manureExportViewItem.DefaultManureCompositionData.NitrogenContent;
                }

                result += (amountOfManure * nitrogenContent);
            }

            return result;
        }

        public double GetTotalNitrogenFromManureImports(int year, Farm farm, AnimalType animalType)
        {
            var totalNitrogen = 0d;

            var viewItemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in viewItemsByYear)
            {
                var manureImports = cropViewItem.ManureApplicationViewItems.Where(x => x.IsImportedManure() && x.AnimalType.GetCategory() == animalType.GetCategory() && x.DateOfApplication.Year == year);
                foreach (var manureApplicationViewItem in manureImports)
                {
                    totalNitrogen += (manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area);
                }
            }

            return totalNitrogen;
        }

        public int GetYearHighestVolumeRemaining(AnimalType animalType)
        {
            var category = animalType.GetCategory();
            var tanksOrderedByAvailableManure = _manureTanks.Where(y => y.AnimalType.GetCategory() == category).OrderByDescending(x => x.VolumeRemainingInTank).ToList();

            if (tanksOrderedByAvailableManure.Any())
            {
                return tanksOrderedByAvailableManure.First().Year;
            }
            else
            {
                return DateTime.Now.Year;
            }
        }

        public DefaultManureCompositionData GetManureCompositionData(ManureItemBase manureItemBase, Farm farm)
        {
            if (manureItemBase != null && farm != null)
            {
                var manureComposition = farm.GetManureCompositionData(
                    manureStateType: manureItemBase.ManureStateType,
                    animalType: manureItemBase.AnimalType);

                // Make a copy
                var mappedItem = _manureCompositionMapper.Map<DefaultManureCompositionData>(manureComposition);

                return mappedItem;
            }
            else
            {
                return new DefaultManureCompositionData();
            }
        }

        public double GetVolumeAvailableForExport(int year, Farm farm, AnimalType animalType)
        {
            var amount = 0d;

            var tank = _manureTanks.SingleOrDefault(x => x.AnimalType == animalType && x.Year == year);
            if (tank != null)
            {
                amount = tank.VolumeRemainingInTank;
            }

            return amount;
        }

        public void Initialize(Farm farm, List<AnimalComponentEmissionsResults> animalComponentEmissions)
        {
            // Clear tanks since animal management may have changed
            _manureTanks.Clear();

            _animalComponentEmissionsResults = animalComponentEmissions;

            var animalTypes = this.GetManureCategoriesProducedOnFarm(farm);
            var years = farm.GetYearsWithAnimals();
            var stateTypes = this.GetManureStatesProducedOnFarm(farm);

            foreach (var year in years)
            {
                foreach (var animalType in animalTypes)
                {
                    foreach (var manureStateType in stateTypes)
                    {
                        this.GetTank(animalType, year, farm, manureStateType);
                    }
                }
            }
        }

        public ManureTank GetTank(AnimalType animalType, int year, Farm farm, ManureStateType manureStateType)
        {
            var tank = this.GetManureTankInternal(animalType, year, manureStateType);
            this.ResetTank(tank, farm);
            this.UpdateAmountsUsed(tank, farm, manureStateType);

            return tank;
        }

        public List<MonthlyManureSpreadingData> GetMonthlyManureSpreadingData(
            CropViewItem viewItem,
            Farm farm)
        {
            var result = new List<MonthlyManureSpreadingData>();

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null || (field.HasLivestockManureApplicationsInYear(viewItem.Year) == false && field.HasImportedManureApplicationsInYear(viewItem.Year) == false))
            {
                return result;
            }

            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                var totalVolume = manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area;

                // When volume is 0, only the amount of N may have been entered. Calculate volume now.
                if (totalVolume == 0)
                {
                    var calculatedVolumePerHectare = manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare;
                    if (manureApplicationViewItem.DefaultManureCompositionData.NitrogenContent > 0)
                    {
                        totalVolume = calculatedVolumePerHectare / manureApplicationViewItem.DefaultManureCompositionData.NitrogenContent;
                    }
                }

                var resultItem = new MonthlyManureSpreadingData
                {
                    Year = manureApplicationViewItem.DateOfApplication.Year,
                    Month = manureApplicationViewItem.DateOfApplication.Month,
                    TotalVolume = totalVolume
                };

                result.Add(resultItem);
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the amounts of manure (and amounts of applied nitrogen) whenever a manure application is added, removed, or updated.
        /// </summary>
        /// <param name="manureTank">The <see cref="ManureTank"/> that will have volume and nitrogen amounts subtracted from</param>
        /// <param name="farm">The <see cref="Farm"/> where the application was made</param>
        /// <param name="manureStateType"></param>
        private void UpdateAmountsUsed(ManureTank manureTank, Farm farm, ManureStateType manureStateType)
        {
            // Iterate over each field and total the land applied manure
            foreach (var farmFieldSystemComponent in farm.FieldSystemComponents)
            {
                foreach (var cropViewItem in farmFieldSystemComponent.CropViewItems)
                {
                    foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems.Where(x => x.ManureStateType == manureStateType && x.DateOfApplication.Year == manureTank.Year))
                    {
                        // If the manure was imported from off-farm, we don't update/reduce the amounts in the storage tanks
                        if (manureApplicationViewItem.IsImportedManure())
                        {
                            continue;
                        }

                        // Account for the total nitrogen that was applied and removed from the tank
                        var amountOfNitrogenAppliedPerHectare = manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare;
                        var totalAmountOfNitrogen = amountOfNitrogenAppliedPerHectare * cropViewItem.Area;
                        manureTank.NitrogenSumOfAllManureApplicationsMade += totalAmountOfNitrogen;

                        // Account for the total volume that was applied and removed from the tank
                        var amountOfManureAppliedPerHectare = manureApplicationViewItem.AmountOfManureAppliedPerHectare;
                        var totalVolume = amountOfManureAppliedPerHectare * cropViewItem.Area;
                        manureTank.VolumeSumOfAllManureApplicationsMade += totalVolume;
                    }
                }
            }
        }

        /// <summary>
        /// Set the state of the manure tank as if there had been no field applications made on the farm.
        /// </summary>
        /// <param name="manureTank">The <see cref="ManureTank"/> that should have all properties reset</param>
        /// <param name="animalComponentResults">The results that will be used to set the starting state of the tank</param>
        /// <param name="manureStateType"></param>
        private void SetStartingStateOfManureTank(
            ManureTank manureTank,
            List<AnimalComponentEmissionsResults> animalComponentResults,
            ManureStateType manureStateType)
        {
            manureTank.ResetTank();

            var targetGroupEmissions = this.GetTargetEmissions(animalComponentResults, manureTank.Year, manureStateType);
            foreach (var groupEmissionsByMonth in targetGroupEmissions)
            {
                manureTank.TotalOrganicNitrogenAvailableForLandApplication += groupEmissionsByMonth.TotalOrganicNitrogenAvailableCreatedInMonth;
                manureTank.TotalTanAvailableForLandApplication += groupEmissionsByMonth.TotalAmountOfTanInStoredManureAvailableForMonth;
                manureTank.TotalAmountOfCarbonInStoredManure += groupEmissionsByMonth.TotalAmountOfCarbonInStoredManureAvailableForMonth;

                // Before any nitrogen from any manure applications have been subtracted from the tank, these two values will be the same
                manureTank.TotalNitrogenAvailableForLandApplication += groupEmissionsByMonth.TotalAmountOfNitrogenInStoredManureAvailableForMonth;
                manureTank.TotalNitrogenAvailableAfterAllLandApplications += groupEmissionsByMonth.TotalAmountOfNitrogenInStoredManureAvailableForMonth;

                // Before any volume of manure from field applications have been subtracted from the tank, these two values will be the same
                manureTank.VolumeOfManureAvailableForLandApplication += groupEmissionsByMonth.TotalVolumeOfManureAvailableForLandApplication * 1000;
                manureTank.VolumeRemainingInTank += groupEmissionsByMonth.TotalVolumeOfManureAvailableForLandApplication * 1000;
            }
        }

        private void ResetTank(ManureTank manureTank, Farm farm)
        {
            var years = new List<int>();
            foreach (var animalComponent in farm.AnimalComponents)
            {
                foreach (var animalComponentGroup in animalComponent.Groups)
                {
                    foreach (var managementPeriod in animalComponentGroup.ManagementPeriods)
                    {
                        for (int i = managementPeriod.Start.Year; i <= managementPeriod.End.Year; i++)
                        {
                            years.Add(i);
                        }
                    }
                }
            }

            var animalType = manureTank.AnimalType;
            var manureType = manureTank.ManureStateType;
            var distinctYears = years.Distinct().ToList();

            var category = animalType.GetComponentCategoryFromAnimalType();
            var resultsForType = _animalComponentEmissionsResults.GetByCategory(category);

            foreach (var year in distinctYears)
            {
                var tank = this.GetManureTankInternal(animalType, year, manureType);
                this.SetStartingStateOfManureTank(tank, resultsForType, manureType);
            }
        }

        /// <summary>
        /// Get the tank for the year that the application was made.
        /// </summary>
        /// <param name="animalType">The tank associated with this type of animal</param>
        /// <param name="year">The year of the tank</param>
        /// <param name="manureStateType"></param>
        /// <returns>The manure tank associated with animal type and the year</returns>
        private ManureTank GetManureTankInternal(AnimalType animalType, int year, ManureStateType manureStateType)
        {
            var tank = _manureTanks.SingleOrDefault(x => x.AnimalType.GetCategory() == animalType.GetCategory() && x.Year == year && x.ManureStateType == manureStateType);
            if (tank == null)
            {
                // If no tank exists for this year, create one now
                tank = new ManureTank() { AnimalType = animalType, Year = year, ManureStateType = manureStateType };

                _manureTanks.Add(tank);
            }

            return tank;
        }

        /// <summary>
        /// Returns all <see cref="GroupEmissionsByMonth"/> that are not housing animals on pasture and the result of management periods that have the same year as the <see cref="ManureTank"/>.
        /// </summary>
        /// <param name="animalComponentEmissionsResults">The emission results for all components with the same livestock type</param>
        /// <param name="yearOfTank">The year of <see cref="ManureTank"/></param>
        /// <param name="manureStateType"></param>
        /// <returns>A list of <see cref="GroupEmissionsByMonth"/> that match the above criteria</returns>
        private List<GroupEmissionsByMonth> GetTargetEmissions(
            List<AnimalComponentEmissionsResults> animalComponentEmissionsResults, int yearOfTank,
            ManureStateType manureStateType)
        {
            var result = new List<GroupEmissionsByMonth>();

            foreach (var animalComponentEmissionsResult in animalComponentEmissionsResults)
            {
                foreach (var allGroupEmissions in animalComponentEmissionsResult.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsByMonth in allGroupEmissions.GroupEmissionsByMonths)
                    {
                        var managementPeriod = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod;

                        if (managementPeriod.HousingDetails.HousingType.IsPasture() == false && groupEmissionsByMonth.MonthsAndDaysData.Year == yearOfTank && managementPeriod.ManureDetails.StateType == manureStateType)
                        {
                            result.Add(groupEmissionsByMonth);
                        }
                    }
                }
            }

            return result;
        }

        #endregion
    }
}