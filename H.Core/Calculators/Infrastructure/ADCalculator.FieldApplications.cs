using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Calculators.Infrastructure
{
    public partial class ADCalculator
    {
        // Get fraction of digestate used and use that to get fraction of N or C used
        public double GetAmountOfNitrogenInFieldApplication(CropViewItem cropViewItem,
            DigestateApplicationViewItem viewItem, List<DigestorDailyOutput> digestorDailyOutputs)
        {
            var amountOfDigestate = viewItem.AmountAppliedPerHectare * cropViewItem.Area;
            var digestateType = viewItem.DigestateState;


            var flowRateOfAllSubstrates = digestorDailyOutputs.Sum(x => x.FlowRateOfAllSubstratesInDigestate);
            var flowRateOfLiquidFraction = digestorDailyOutputs.Sum(x => x.FlowRateLiquidFraction);
            var flowRateOfSolidFraction = digestorDailyOutputs.Sum(x => x.FlowRateSolidFraction);

            throw new NotImplementedException();
        }
    }
}