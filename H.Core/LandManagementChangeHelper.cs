using System;
using H.Core.Enumerations;

namespace H.Core
{
    public class LandManagementChangeHelper
    {
        public TillagePracticeChangeType GetTillagePracticeChangeType(TillageType currentTillageType, TillageType pastTillageType)
        {
            if (pastTillageType == TillageType.Intensive && currentTillageType == TillageType.Reduced)
            {
                return TillagePracticeChangeType.IntenseToReduced;
            }
            else if (pastTillageType == TillageType.Intensive && currentTillageType == TillageType.NoTill)
            {
                return TillagePracticeChangeType.IntenseToNone;
            }
            else if (pastTillageType == TillageType.Reduced && currentTillageType == TillageType.NoTill)
            {
                return TillagePracticeChangeType.ReducedToNone;
            }
            else if (pastTillageType == TillageType.Reduced && currentTillageType == TillageType.Intensive)
            {
                return TillagePracticeChangeType.ReducedToIntense;
            }
            else if (pastTillageType == TillageType.NoTill && currentTillageType == TillageType.Reduced)
            {
                return TillagePracticeChangeType.NoneToReduced;
            }
            else
            {
                return TillagePracticeChangeType.NoneToIntense;
            }
        }

        /// <summary>
        /// Methodology taken from version 3 file: 'LandManagementDefaults.vb' line #118 - 125. Version 3 only considered the total area of fallow land (past/current) when
        /// looking up values for LumC and K.
        /// 
        /// Version 3 parsed file: 'SoilCarbonDefaults.csv' to get values.
        /// 
        /// Columns in file have the following naming convention:
        /// 
        /// LDF = LumC value for a Decrease in Fallow area
        /// KDF = K value for a Decrease in Fallow area
        /// 
        /// Column O 'LDF' in 'SoilCarbonDefaults.csv' corresponds to lumCMax values in table 2 for fallow to continuous cropping and column Q corresponds to lumCMax values in table 2 for
        /// continuous cropping to fallow.
        /// </summary>
        public FallowPracticeChangeType GetFallowPracticeChangeType(double pastFallowArea, double currentFallowArea)
        {
            if (currentFallowArea < pastFallowArea)
            {
                return FallowPracticeChangeType.FallowCroppingToContinous;
            }
            else
            {
                return FallowPracticeChangeType.ContinousToFallowCropping;
            }
        }

        /// <summary>
        /// Used for perennial crops and grasslands.
        /// </summary>
        public PerennialCroppingChangeType GetPerennialCroppingChangeType(double pastArea, double currentArea)
        {
            if (currentArea < pastArea)
            {
                return PerennialCroppingChangeType.DecreaseInPerennialCroppingArea;
            }
            else
            {
                return PerennialCroppingChangeType.IncreaseInPerennialCroppingArea;
            }
        }
    }
}