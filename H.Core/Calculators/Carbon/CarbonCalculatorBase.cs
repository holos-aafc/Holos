﻿using System;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using System.Collections.Generic;
using H.Core.Calculators.Nitrogen;
using H.Core.Services.Animals;
using H.Core.Enumerations;
using System.Linq;
using H.Core.Calculators.Nitrogen.NitrogenService;

namespace H.Core.Calculators.Carbon
{
    public abstract partial class CarbonCalculatorBase
    {
        #region Fields

        protected INitrogenInputCalculator icbmNitrogenInputCalculator = new ICBMNitrogenInputCalculator();
        protected INitrogenInputCalculator ipccNitrogenInputCalculator = new IPCCNitrogenInputCalculator();

        protected INitrogenService nitrogenInputCalculator = new NitrogenService();

        #endregion

        #region Constructors

        protected CarbonCalculatorBase()
        {
            this.AnimalComponentEmissionsResults = new List<AnimalComponentEmissionsResults>();
        }

        #endregion

        #region Properties

        public List<AnimalComponentEmissionsResults> AnimalComponentEmissionsResults { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Equation 2.2.2-26
        /// 
        /// Calculate amount of carbon input from all manure applications in a year.
        /// </summary>
        /// <returns>The amount of carbon input during the year (kg C ha^-1)</returns>
        public double CalculateManureCarbonInputPerHectare(
            CropViewItem viewItem)
        {
            return viewItem.GetTotalCarbonFromAppliedManure() / viewItem.Area;
        }

        #endregion

        #region Private Methods



        #endregion
    }
}