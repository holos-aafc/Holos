#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using H.Core.Enumerations;
using H.Core.Properties;
using H.Core.Providers.Animals;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Feed
{
    /// <summary>
    /// </summary>
    public class DietProvider : IDietProvider
    {
        #region Fields

        private Table_42_Crude_Protein_Content_Swine_Feed_Provider _swineCrudeProteinProvider;
        private readonly IFeedIngredientProvider _feedIngredientProvider;

        #endregion

        #region Constructors

        public DietProvider()
        {
            _feedIngredientProvider = new FeedIngredientProvider();
            _swineCrudeProteinProvider = new Table_42_Crude_Protein_Content_Swine_Feed_Provider();
        }

        #endregion

        #region Public Methods

        public List<AnimalType> GetValidAnimalDietTypes(AnimalType animalType)
        {
            if (animalType.IsBeefCattleType())
            {
                return new List<AnimalType>()
                {
                    AnimalType.BeefBackgrounder,
                    AnimalType.BeefFinisher,
                    AnimalType.BeefCow,
                    AnimalType.BeefBulls,
                    AnimalType.Stockers,
                };
            }

            if (animalType.IsDairyCattleType())
            {
                return new List<AnimalType>()
                {
                    AnimalType.DairyDryCow,
                    AnimalType.DairyHeifers,
                    AnimalType.DairyLactatingCow,
                };
            }

            // Sheep default diets don't specify which diets belong to which animal groups. Use these diets for all sheep groups
            if (animalType.IsSheepType())
            {
                return new List<AnimalType>()
                {
                    AnimalType.Sheep,
                };
            }

            if (animalType.IsSwineType())
            {
                return new List<AnimalType>()
                {
                    AnimalType.Swine,
                    AnimalType.SwineBoar,
                    AnimalType.SwineDrySow,
                    AnimalType.SwineFinisher,
                    AnimalType.SwineGrower,
                    AnimalType.SwineLactatingSow,
                    AnimalType.SwineStarter,
                };
            }

            return new List<AnimalType>();
        }

        public List<Diet> GetDiets()
        {
            var diets = new List<Diet>();

            /*
             * Cow-calf diets
             */
            diets.AddRange(this.CreateBeefCowDiets());
            diets.AddRange(this.CreateBeefBullDiets());

            /*
             * Backgrounding diets
             */
            diets.AddRange(this.CreateBeefBackgroundingDiets());
            diets.AddRange(this.CreateBeefStockerDiets());

            /*
             * Finishing diets
             */
            diets.AddRange(this.CreateBeefFinishingDiets());

            /*
             * Dairy diets
             */

            diets.AddRange(this.CreateDairyLactatingCowDiets());
            diets.AddRange(this.CreateDairyDryCowDiets());
            diets.AddRange(this.CreateDairyHeiferDiets());

            /*
             * Swine diets
             */
            diets.AddRange(this.CreateSwineDiets());

            /*
             * Sheep diets
             */
            diets.AddRange(this.GetSheepDiets());

            // For animals that don't have default diets, we still need to set the diet to a placeholder (non-null) value.
            diets.Add(this.GetNoDiet());

            // This needs to be set so readonly style can be used in diet creator view
            foreach (var diet in diets)
            {
                foreach (var ingredient in diet.Ingredients)
                {
                    ingredient.IsReadonly = true;
                }
            }

            return diets;
        }

        #endregion

        #region Private Methods

        private IEnumerable<Diet> CreateDairyHeiferDiets()
        {
            var dairyIngredients = _feedIngredientProvider.GetDairyFeedIngredients().ToList();

            var diets = new List<Diet>();

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Properties.Resources.LabelHighFiberDiet,
                DietType = DietType.HighFiber,
                AnimalType = AnimalType.DairyHeifers,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.GrassesCoolHayMature), 50),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrainRolled), 45),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealExpellers), 5),
                },

                Forage = 87.6,
                MethaneConversionFactor = 0.07,
            });

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Properties.Resources.LabelLowFiberDiet,
                DietType = DietType.LowFiber,
                AnimalType = AnimalType.DairyHeifers,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowSilageNormal), 50),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrainRolled), 42),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealMechExtracted), 8),
                },

                Forage = 63,
                MethaneConversionFactor = 0.063,
            });

            return diets;
        }

        private IEnumerable<Diet> CreateDairyDryCowDiets()
        {
            var dairyIngredients = _feedIngredientProvider.GetDairyFeedIngredients().ToList();

            var diets = new List<Diet>();

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Properties.Resources.LabelCloseUpDiet,
                DietType = DietType.CloseUp,
                AnimalType = AnimalType.DairyDryCow,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowSilageNormal), 48),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.GrassLegumeMixturesPredomLegumesSilageMidMaturity), 23),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowGrainCrackedDry), 12),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealMechExtracted), 9),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowGlutenMealDried), 8),                   
                },

                Forage = 52.4,
                MethaneConversionFactor = 0.063,
            });

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Properties.Resources.LabelFarOffDiet,
                DietType = DietType.FarOff,
                AnimalType = AnimalType.DairyDryCow,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.GrassLegumeMixturesPredomLegumesHayMidMaturity), 57),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrainRolled), 38),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealExpellers), 5),
                },

                Forage = 90.2,
                MethaneConversionFactor = 0.070,
            });

            return diets;
        }

        private IEnumerable<Diet> CreateDairyLactatingCowDiets()
        {
            var dairyIngredients = _feedIngredientProvider.GetDairyFeedIngredients().ToList();

            var diets = new List<Diet>();

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Properties.Resources.LabelLegumeForageBasedDiet,
                DietType = DietType.LegumeForageBased,
                AnimalType = AnimalType.DairyLactatingCow,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.LegumesForageHayMature), 22),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.LegumesForageSilageAllSamples), 22),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrainRolled), 45),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealMechExtracted), 2),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.SoybeanHulls), 5),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.MolassesBeetSugar), 1),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowGlutenFeedDried), 3),
                },

                Forage = 77.8,
                MethaneConversionFactor = 0.056,
            });

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Properties.Resources.LabelBarleySilageBasedDiet,
                DietType = DietType.BarleySilageBased,
                AnimalType = AnimalType.DairyLactatingCow,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.BarleySilageHeaded), 49),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.LegumesForageHayMidMaturity), 5),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrainRolled), 17),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowGrainRolledHighMoisture), 13),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealMechExtracted), 5),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealExpellers), 4),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.BeetSugarPulpDried), 2),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.MolassesSugarCane), 1),
                },

                Forage = 60.5,
                MethaneConversionFactor = 0.056,
            });

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Properties.Resources.LabelCornSilageBasedDiet,
                DietType = DietType.CornSilageBased,
                AnimalType = AnimalType.DairyLactatingCow,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowSilageNormal), 55),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.GrassesCoolHayMidMaturity), 6),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowGrainGraundDry), 11),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealSolvent48), 12),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.SoybeanHulls), 5),
                    _feedIngredientProvider.CopyIngredient(dairyIngredients.Single(x => x.IngredientType == IngredientType.CornYellowGlutenFeedDried), 11),
                },

                Forage = 59.1,
                MethaneConversionFactor = 0.056,
            });

            return diets;
        }

        private IEnumerable<Diet> CreateBeefBackgroundingDiets()
        {
            var beefIngredients = _feedIngredientProvider.GetBeefFeedIngredients().ToList();

            var diets = new List<Diet>();

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelSlowGrowthDiet,
                DietType = DietType.SlowGrowth,
                AnimalType = AnimalType.BeefBackgrounder,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 69.4,
                //CrudeProtein = 11.6,
                //Forage = 65,
                //Starch = 28.5,
                //Fat = 3.3,
                //MetabolizableEnergy = 2.51,
                //Ndf = 40.5,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleySilage), 65),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.CornGrain), 35)
                },

                MethaneConversionFactor = 0.063,
            });

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelMediumGrowthDiet,
                DietType = DietType.MediumGrowth,
                AnimalType = AnimalType.BeefBackgrounder,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 70.7,
                //CrudeProtein = 12.6,
                //Forage = 60,
                //Starch = 31.3,
                //Fat = 3.3,
                //MetabolizableEnergy = 2.56,
                //Ndf = 38.5,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleySilage), 65),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrain), 35)
                },

                MethaneConversionFactor = 0.063,
            });

            return diets;
        }

        private IEnumerable<Diet> CreateBeefFinishingDiets()
        {
            var beefIngredients = _feedIngredientProvider.GetBeefFeedIngredients().ToList();

            var diets = new List<Diet>();

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelBarleyGrainBasedDiet,
                DietType = DietType.BarleyGrainBased,
                AnimalType = AnimalType.BeefFinisher,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 80.4,
                //CrudeProtein = 12.6,
                //Forage = 7.5,
                //Starch = 52.1,
                //Fat = 2.3,
                //MetabolizableEnergy = 2.91,
                //Ndf = 21,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleySilage), 10),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrain), 90),
                },

                MethaneConversionFactor = 0.035,
            });

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelCornGrainBasedDiet,
                DietType = DietType.CornGrainBased,
                AnimalType = AnimalType.BeefFinisher,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 83.9,
                //CrudeProtein = 12.6,
                //Forage = 7.5,
                //Starch = 65.9,
                //Fat = 3.7,
                //MetabolizableEnergy = 3.03,
                //Ndf = 13.5,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleySilage), 10),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.CornGrain), 88.7),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.Urea), 1.3)
                },

                MethaneConversionFactor = 0.03,
            });

            return diets;
        }

        /// <summary>
        /// Implements: Table 23. Examples of NEmf content of typical diets fed to cattle for estimation of dry matter intake (IPCC, 2019, Table 10.8a).
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Diet> CreateBeefCowDiets()
        {
            /*
             * Footnote 1: The low, moderate and high quality forage diets are equivalent to the low, medium and high energy/protein diets, respectively, for beef cows in Table 21.
               
               Footnote 2: If the model user wants to formulate their own diet, the feed energy content {NEmf (MJkg-1 DM)] 
               can be calculated from the default feed tables built into Holos. The values are included in the default 
               table as NEma and NEga (Mcal kg-1); therefore, NEmf can be calculated as: NEmf (MJ kg-1 DM)=[NEma+NEga]*4.184 (conversion factor for Mcal to MJ).
             */
            var beefIngredients = _feedIngredientProvider.GetBeefFeedIngredients().ToList();

            var diets = new List<Diet>();

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LowEnergyProtein,
                DietType = DietType.LowEnergyAndProtein,
                AnimalType = AnimalType.BeefCow,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 47,
                //CrudeProtein = 6,
                //Forage = 100,
                //Starch = 5.5,
                //Fat = 1.4,
                //MetabolizableEnergy = 1.73,
                //Ndf = 71.4,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.NativePrairieHay), 100),
                },

                MethaneConversionFactor = 0.07,
                DietaryNetEnergyConcentration = 4.5,
            });

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelMediumEnergyProteinDiet,
                DietType = DietType.MediumEnergyAndProtein,
                AnimalType = AnimalType.BeefCow,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 54.6,
                //CrudeProtein = 12.4,
                //Forage = 97,
                //Starch = 7.1,
                //Fat = 1.8,
                //MetabolizableEnergy = 1.98,
                //Ndf = 53.5,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.AlfalfaHay), 32),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.MeadowHay), 65),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrain), 3)
                },

                MethaneConversionFactor = 0.070,
                DietaryNetEnergyConcentration = 6,
            });

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelHighEnergyProteinDiet,
                DietType = DietType.HighEnergyAndProtein,
                AnimalType = AnimalType.BeefCow,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 62.8,
                //CrudeProtein = 17.7,
                //Forage = 85,
                //Starch = 9.9,
                //Fat = 2.2,
                //MetabolizableEnergy = 2.14,
                //Ndf = 45.1,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.OrchardgrassHay), 60),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.AlfalfaHay), 20),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrain), 20),
                },

                MethaneConversionFactor = 0.070,
                DietaryNetEnergyConcentration = 7.0,
            });

            return diets;
        }

        private IEnumerable<Diet> CreateBeefStockerDiets()
        {
            var beefIngredients = _feedIngredientProvider.GetBeefFeedIngredients().ToList();

            var diets = new List<Diet>();

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LowEnergyProtein,
                DietType = DietType.LowEnergyAndProtein,
                AnimalType = AnimalType.Stockers,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 47,
                //CrudeProtein = 6,
                //Forage = 100,
                //Starch = 5.5,
                //Fat = 1.4,
                //MetabolizableEnergy = 1.73,
                //Ndf = 71.4,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.NativePrairieHay), 100),
                },

                MethaneConversionFactor = 0.07,
            });

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelMediumEnergyProteinDiet,
                DietType = DietType.MediumEnergyAndProtein,
                AnimalType = AnimalType.Stockers,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 54.6,
                //CrudeProtein = 12.4,
                //Forage = 97,
                //Starch = 7.1,
                //Fat = 1.8,
                //MetabolizableEnergy = 1.98,
                //Ndf = 53.5,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.AlfalfaHay), 32),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.MeadowHay), 65),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrain), 3)
                },

                MethaneConversionFactor = 0.07,
            });

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelHighEnergyProteinDiet,
                DietType = DietType.HighEnergyAndProtein,
                AnimalType = AnimalType.Stockers,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 62.8,
                //CrudeProtein = 17.7,
                //Forage = 85,
                //Starch = 9.9,
                //Fat = 2.2,
                //MetabolizableEnergy = 2.14,
                //Ndf = 45.1,                

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.OrchardgrassHay), 60),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.AlfalfaHay), 20),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrain), 20),
                },

                MethaneConversionFactor = 0.07,
            });

            return diets;
        }

        private IEnumerable<Diet> CreateBeefBullDiets()
        {
            var beefIngredients = _feedIngredientProvider.GetBeefFeedIngredients().ToList();

            var diets = new List<Diet>();

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LowEnergyProtein,
                DietType = DietType.LowEnergyAndProtein,
                AnimalType = AnimalType.BeefBulls,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 47,
                //CrudeProtein = 6,
                //Forage = 100,
                //Starch = 5.5,
                //Fat = 1.4,
                //MetabolizableEnergy = 1.73,
                //Ndf = 71.4,
                //MethaneConversionFactor = 0.07,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.NativePrairieHay), 100),
                },

                MethaneConversionFactor = 0.07,
            });

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelMediumEnergyProteinDiet,
                DietType = DietType.MediumEnergyAndProtein,
                AnimalType = AnimalType.BeefBulls,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 54.6,
                //CrudeProtein = 12.4,
                //Forage = 97,
                //Starch = 7.1,
                //Fat = 1.8,
                //MetabolizableEnergy = 1.98,
                //Ndf = 53.5,
                //MethaneConversionFactor = 0.065,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.AlfalfaHay), 32),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.MeadowHay), 65),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrain), 3)
                },

                MethaneConversionFactor = 0.065,
            });

            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelHighEnergyProteinDiet,
                DietType = DietType.HighEnergyAndProtein,
                AnimalType = AnimalType.BeefBulls,

                // This is the breakdown of the diet if ingredients are not added.
                //TotalDigestibleNutrient = 62.8,
                //CrudeProtein = 17.7,
                //Forage = 85,
                //Starch = 9.9,
                //Fat = 2.2,
                //MetabolizableEnergy = 2.14,
                //Ndf = 45.1,
                //MethaneConversionFactor = 0.065,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.OrchardgrassHay), 60),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.AlfalfaHay), 20),
                    _feedIngredientProvider.CopyIngredient(beefIngredients.Single(x => x.IngredientType == IngredientType.BarleyGrain), 20),
                },

                MethaneConversionFactor = 0.065,
            });

            return diets;
        }

        /// <summary>
        /// Swine diets (all diets) are defined in the diet provider. Some diets should not be displayed in the diet formulator since not all animal types allow for
        /// custom diets. Any diet that should be hidden from user in diet formulator view should have the HideFromUserInDietFormulator property set to true.
        /// </summary>
        private IEnumerable<Diet> CreateSwineDiets()
        {
            var swineIngredients = _feedIngredientProvider.GetSwineFeedIngredients();
            
            var diets = new List<Diet>();

            // Diet A - Gestation (114 days)
            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelGestationDiet,
                DietType = DietType.Gestation,
                AnimalType = AnimalType.Swine,
                Comments = Resources.LabelDietA,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatBran), 14),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatShorts), 3),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.Barley), 62.2),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealDehulledExpelled), 4),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealExpelled), 3),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FieldPeas), 6),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SugarBeetPulp), 5.6),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaFullFat), 0.4),
                },

                DailyDryMatterFeedIntakeOfFeed = 2.49,
                CrudeProtein = 14.28,
            });

            // Diet B - Lactation (21 days)
            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelLactationDiet,
                DietType = DietType.Lactation,
                AnimalType = AnimalType.Swine,
                Comments = Resources.LabelDietB,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatBran), 41),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.Barley), 21.8),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil), 9),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealDehulledExpelled), 9),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealExpelled), 5),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FieldPeas), 10),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaFullFat), 1.8),
                },

                DailyDryMatterFeedIntakeOfFeed = 6.59,
                CrudeProtein = 19.07,
            });

            // Diet C1 - Nursery weaners (starter diet 1)
            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelNurseryWeanersStarterDiet1,
                DietType = DietType.NurseryWeanersStarter1,
                AnimalType = AnimalType.Swine,
                Comments = Resources.LabelDietC1,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatBran), 39),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil), 11.38),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealDehulledExpelled), 20),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FieldPeas), 11),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaFullFat), 1.2),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheyPermeateLactose80), 10),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FishMealCombined), 5),
                },

                DailyDryMatterFeedIntakeOfFeed = 0.80,
                CrudeProtein = 23.88,
            });

            // Diet C2 - Nursery weaners (starter diet 2)
            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelNurseryWeanersStarterDiet2,
                DietType = DietType.NurseryWeanersStarter2,
                AnimalType = AnimalType.Swine,
                Comments = Resources.LabelDietC2,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatBran), 33.76),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.Barley), 20),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil), 10),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealDehulledExpelled), 14),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealExpelled), 7),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FieldPeas), 10),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaFullFat), 2.5),
                },

                DailyDryMatterFeedIntakeOfFeed = 1.17,
                CrudeProtein = 21.45,
            });

            // Diet D1 - Grower/Finisher diet 1
            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelGrowerFinisherDiet1,
                DietType = DietType.GrowerFinisherDiet1,
                AnimalType = AnimalType.Swine,
                Comments = Resources.LabelDietD1,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatBran), 32.53),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.Barley), 24),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil), 12),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealDehulledExpelled), 10),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealExpelled), 6),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FieldPeas), 12),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaFullFat), 1.2),
                },

                DailyDryMatterFeedIntakeOfFeed = 1.68,
                CrudeProtein = 20.27,
            });

            // Diet D2 - Grower/Finisher diet 2
            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelGrowerFinisherDiet2,
                DietType = DietType.GrowerFinisherDiet2,
                AnimalType = AnimalType.Swine,
                Comments = Resources.LabelDietD2,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatBran), 32.2),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.Barley), 26.79),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil), 12),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealDehulledExpelled), 8),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealExpelled), 8),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FieldPeas), 10),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaFullFat), 1.1),
                },

                DailyDryMatterFeedIntakeOfFeed = 2.13,
                CrudeProtein = 19.89,
            });

            // Diet D3 - Grower/Finisher diet 3
            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelGrowerFinisherDiet3,
                DietType = DietType.GrowerFinisherDiet3,
                AnimalType = AnimalType.Swine,
                Comments = Resources.LabelDietD3,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatBran), 25.2),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatShorts), 6.3),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.Barley), 28),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil), 12),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealDehulledExpelled), 8),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealExpelled), 8),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FieldPeas), 10),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaFullFat), 0.8),
                },

                DailyDryMatterFeedIntakeOfFeed = 2.55,
                CrudeProtein = 19.92,
            });

            // Diet D4 - Grower/Finisher diet 4
            diets.Add(new Diet
            {
                IsDefaultDiet = true,
                Name = Resources.LabelGrowerFinisherDiet4,
                DietType = DietType.GrowerFinisherDiet4,
                AnimalType = AnimalType.Swine,
                Comments = Resources.LabelDietD4,

                Ingredients = new ObservableCollection<FeedIngredient>()
                {
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatBran), 19.1),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.WheatShorts), 11.8),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.Barley), 30),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil), 15),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.SoybeanMealDehulledExpelled), 6),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaMealExpelled), 8),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.FieldPeas), 8),
                    _feedIngredientProvider.CopyIngredient(swineIngredients.Single(x => x.IngredientType == IngredientType.CanolaFullFat), 0.8),
                },

                DailyDryMatterFeedIntakeOfFeed = 2.93,
                CrudeProtein = 19.66,
            });

            return diets;
        }

        private IEnumerable<Diet> GetSheepDiets()
        {
            var diets = new List<Diet>();

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Core.Properties.Resources.GoodQualityForage,
                DietType = DietType.GoodQualityForage,
                AnimalType = AnimalType.Sheep,
                TotalDigestibleNutrient = 65,
                CrudeProtein = 18,
                Ash = 8,
                MethaneConversionFactor = 0.058,
            });

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Core.Properties.Resources.AverageQualityForage,
                AnimalType = AnimalType.Sheep,
                DietType = DietType.AverageQualityForage,
                TotalDigestibleNutrient = 55,
                CrudeProtein = 12,
                Ash = 8,
                MethaneConversionFactor = 0.067,
            });

            diets.Add(new Diet()
            {
                IsDefaultDiet = true,
                Name = Core.Properties.Resources.PoorQualityForage,
                DietType = DietType.PoorQualityForage,
                AnimalType = AnimalType.Sheep,
                TotalDigestibleNutrient = 45,
                CrudeProtein = 6,
                Ash = 8,
                MethaneConversionFactor = 0.074,
            });

            return diets;
        }

        /// <summary>
        /// Some animal groups will not have a diet (poultry, other livestock, suckling pigs, etc.). In these cases, a non-null diet must still be set.
        /// </summary>
        public Diet GetNoDiet()
        {
            return new Diet()
            {
                Name = Resources.None,
                IsCustomPlaceholderDiet = true,
                AnimalType = AnimalType.NotSelected,
            };
        }

        #endregion
    }
}