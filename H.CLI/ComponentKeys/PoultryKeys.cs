using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;

namespace H.CLI.ComponentKeys
{
    public class PoultryKeys : AnimalKeyBase, IComponentKeys
    {
        #region Constructors

        public PoultryKeys() :base()
        {
            // Common keys are added in super class
            base.Keys.Add(Properties.Resources.YearlyManureMethaneRate, ImperialUnitsOfMeasurement.PoundPerHeadPerYear);
            base.Keys.Add(Properties.Resources.YearlyNitrogenExcretionRate, ImperialUnitsOfMeasurement.PoundPerHeadPerYear);
            base.Keys.Add(Properties.Resources.YearlyEntericMethaneRate, ImperialUnitsOfMeasurement.PoundPerHeadPerYear);
            base.Keys.Add(Properties.Resources.N2ODirectEmissionFactor, ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN);
            base.Keys.Add(Properties.Resources.VolatilizationFraction, null);
        }

        #endregion
    }
}
