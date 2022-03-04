using System;
using System.Collections.Generic;
using System.Diagnostics;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Economics;

namespace H.Core.Calculators.Economics
{
    public class EconomicsHelper : UnitsOfMeasurementCalculator
    {
        #region Fields

        #endregion

        #region Constructors

        public EconomicsHelper()
        {

        }

        #endregion

        #region Public Methods

        public void ConvertValuesToMetricIfNecessary(CropEconomicData economicData, Farm farm)
        {
            //we set this to false so that we don't trace a boatload of info to output
            economicData.IsInitialized = false;

            //we don't need to convert and this is just a safety catch incase this gets called incorrectly
            if (farm.MeasurementSystemType == MeasurementSystemType.Imperial || economicData.IsConverted) return;

            var dollarsPerAcreProps = new List<string>()
            {
                nameof(economicData.CropSalesPerAcre),
                nameof(economicData.SeedCleaningAndTreatment),
                nameof(economicData.Fertilizer),
                nameof(economicData.Chemical),
                nameof(economicData.HailCropInsurance),
                nameof(economicData.TruckingMarketing),
                nameof(economicData.FuelOilLube),
                nameof(economicData.MachineryRepairs),
                nameof(economicData.BuildingRepairs),
                nameof(economicData.CustomWork),
                nameof(economicData.Labour),
                nameof(economicData.Utilities),
                nameof(economicData.OperatingInterest),
                nameof(economicData.TotalCost),
                nameof(economicData.PumpingCosts),
                nameof(economicData.HerbicideCost),
                nameof(economicData.TotalFixedCostPerUnit),
                nameof(economicData.TotalVariableCostPerUnit),
            };

            foreach (var prop in economicData.GetType().GetProperties())
            {
                //convert only certain values that are reported in $/acre originally
                if (dollarsPerAcreProps.Contains(prop.Name))
                {
                    var currentValue = (double)prop.GetValue(economicData);
                    var convertedValue = currentValue * AcresPerHectare;
                    prop.SetValue(economicData, convertedValue);
                }
            }

            ConvertExpectedMarketPriceToMetricIfNecessary(economicData, farm);
            economicData.IsInitialized = true;
            economicData.IsConverted = true;
        }

        /// <summary>
        /// this can only apply to Expected Market Price since production costs are reported in $/acre
        /// </summary>
        /// <param name="economicData">the current crop economic data</param>
        /// <param name="farm">the active farm</param>
        public void ConvertExpectedMarketPriceToMetricIfNecessary(CropEconomicData economicData, Farm farm)
        {
            if (farm.MeasurementSystemType == MeasurementSystemType.Imperial || economicData.IsConverted) return;

            switch (economicData.Unit)
            {
                case EconomicMeasurementUnits.Bushel:
                    economicData.ExpectedMarketPrice = this.ConvertMarketPriceFromDollarsPerBushelToDollarsPerKilogram(economicData);
                    break;
                case EconomicMeasurementUnits.Pound:
                    economicData.ExpectedMarketPrice = this.ConvertMarketPriceFromDollarsPerPoundToDollarsPerKilogram(economicData);
                    break;
                case EconomicMeasurementUnits.Tonne:
                    economicData.ExpectedMarketPrice = this.ConvertMarketPriceFromDollarsPerTonneToDollarsPerKilogram(economicData);
                    break;
                case EconomicMeasurementUnits.HundredWeight:
                    economicData.ExpectedMarketPrice = ConvertMarketPriceFromDollarsPerHundredWeightToDollarsKilogram(economicData);
                    break;
                case EconomicMeasurementUnits.None:
                    break;
            }
        }

        #endregion

        #region Private/Protected Methods

        private double ConvertMarketPriceFromDollarsPerHundredWeightToDollarsKilogram(CropEconomicData economicData)
        {
            if (economicData.Unit == EconomicMeasurementUnits.HundredWeight)
            {
                return economicData.ExpectedMarketPrice / KgPerHundredWeight;
            }

            throw new Exception($"{economicData.Unit} is not of type {EconomicMeasurementUnits.HundredWeight}");
        }
        private double ConvertMarketPriceFromDollarsPerBushelToDollarsPerKilogram(CropEconomicData economicData)
        {
            if (economicData.Unit == EconomicMeasurementUnits.Bushel)
            {
                return ConvertMarketPriceFromDollarsPerBushelToDollarsPerTonne(economicData) / 1000;
            }

            throw new Exception($"{economicData.Unit} is not of type {EconomicMeasurementUnits.Tonne}");
        }
        private double ConvertMarketPriceFromDollarsPerTonneToDollarsPerKilogram(CropEconomicData economicData)
        {
            if (economicData.Unit == EconomicMeasurementUnits.Tonne)
            {
                return economicData.ExpectedMarketPrice / KgPerTonne;
            }

            throw new Exception($"{economicData.Unit} is not of type {EconomicMeasurementUnits.Tonne}");
        }

        private double ConvertMarketPriceFromDollarsPerPoundToDollarsPerKilogram(CropEconomicData economicData)
        {
            if (economicData.Unit == EconomicMeasurementUnits.Pound)
            {
                return economicData.ExpectedMarketPrice * LbsPerKg;
            }

            throw new Exception($"{economicData.Unit} is not of type {EconomicMeasurementUnits.Pound}");
        }
        protected double ConvertMarketPriceFromDollarsPerBushelToDollarsPerTonne(CropEconomicData economicData)
        {
            if (economicData.Unit == EconomicMeasurementUnits.Bushel)
            {
                var pricePerBushel = economicData.ExpectedMarketPrice;
                var cropType = economicData.CropType;
                switch (cropType)
                {
                    case CropType.AlfalfaHay:
                        return pricePerBushel * AlfalfaBushelPerTonne;
                    case CropType.Barley:
                    case CropType.FeedBarley:
                    case CropType.MaltBarley:
                    case CropType.SouthernOntarioBarley:
                        return pricePerBushel * BarelyBushelPerTonne;
                    case CropType.Canola:
                    case CropType.Camelina:
                    case CropType.PolishCanola:
                    case CropType.SpringCanolaHt:
                    case CropType.ArgentineHTCanola:
                        return pricePerBushel * CanolaBushelPerTonne;
                    case CropType.Corn:
                        return pricePerBushel * CornBushelPerTonne;
                    case CropType.DryPeas:
                        return pricePerBushel * DryPeasBushelPerTonne;
                    case CropType.Flax:
                    case CropType.FlaxSeed:
                        return pricePerBushel * FlaxBushelPerTonne;
                    case CropType.FodderCorn:
                        return pricePerBushel * FodderCornBushelPerTonne;
                    case CropType.GrainCorn:
                        return pricePerBushel * GrainCornBushelPerTonne;
                    case CropType.TameGrass:
                        return pricePerBushel * HayGrassBushelPerTonne;
                    case CropType.TameMixed:
                        return pricePerBushel * HayMixedBushelPerTonne;
                    case CropType.TameLegume:
                        return pricePerBushel * LegumeBushelPerTonne;
                    case CropType.Lentils:
                        return pricePerBushel * LentilsBushelPerTonne;
                    case CropType.Mustard:
                    case CropType.BrownMustard:
                    case CropType.YellowMustard:
                    case CropType.OrientalMustard:
                        return pricePerBushel * MustardBushelPerTonne;
                    case CropType.Oats:
                    case CropType.MillingOats:
                    case CropType.SouthernOntarioOats:
                        return pricePerBushel * OatsBushelPerTonne;
                    case CropType.Peas:
                    case CropType.Chickpeas:
                    case CropType.FieldPeas:
                    case CropType.RedLentils:
                    case CropType.DesiChickpeas:
                    case CropType.KabuliChickpea:
                    case CropType.EdibleGreenPeas:
                    case CropType.EdibleYellowPeas:
                    case CropType.LargeGreenLentils:
                    case CropType.SmallKabuliChickpea:
                    case CropType.LargeKabuliChickpea:
                        return pricePerBushel * PeasBushelPerTonne;
                    case CropType.Sorghum:
                        return pricePerBushel * SorghumBushelPerTonne;
                    case CropType.DryBean:
                    case CropType.Soybeans:
                    case CropType.BeansPinto:
                    case CropType.BeansWhite:
                        return pricePerBushel * SoyBeansBushelPerTonne;
                    case CropType.Triticale:
                    case CropType.HybridFallRye:
                        return pricePerBushel * TriticaleBushelPerTonne;
                    case CropType.UndersownBarley:
                        return pricePerBushel * UnderSownBarleyBushelPerTonne;
                    case CropType.WheatBolinder:
                        return pricePerBushel * WheatBolinderBushelPerTonne;
                    case CropType.Wheat:
                    case CropType.Durum:
                    case CropType.CPSWheat:
                    case CropType.SoftWheat:
                    case CropType.SpringWheat:
                    case CropType.WinterWheat:
                    case CropType.WheatOtherSpring:
                    case CropType.WheatHardRedSpring:
                    case CropType.HardRedSpringWheat:
                    case CropType.WheatPrairieSpring:
                    case CropType.WheatNorthernHardRed:
                        return pricePerBushel * WheatBushelPerTonne;
                    case CropType.WheatGan:
                        return pricePerBushel * WheatGanBushelPerTonne;
                    default:
                        Trace.TraceInformation(
                            $"{nameof(EconomicsCalculator)}.{nameof(ConvertMarketPriceFromDollarsPerBushelToDollarsPerTonne)}: missing conversion data for '{cropType}. Returning 0;");
                        return 0;
                }
            }

            throw new Exception($"{economicData.Unit} is not of type {EconomicMeasurementUnits.Bushel}");
        }

        #endregion
    }
}
