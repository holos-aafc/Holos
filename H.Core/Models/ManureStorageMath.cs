namespace H.Core.Models
{
    /// <summary>
    /// Pure day-to-day manure storage math shared by the animal-results storage phase and the <see cref="ManureTank"/>
    /// domain object: the net-of-removals volume carryover and the bounded removal fraction of Equation 4.1.3-6/7.
    /// Extracted so both the biology (in the animal service) and the physical tank accounting compute the storage
    /// dynamics from a single definition (issue #451 follow-up).
    /// </summary>
    public static class ManureStorageMath
    {
        #region Fields

        /// <summary>
        /// The maximum fraction of the manure in a tank that can be emptied in a single day.
        /// </summary>
        public const double MaximumRemovalFraction = 0.95; 

        #endregion

        #region Public Methods

        /// <summary>
        /// Today's net-of-removals volume in storage: today's inflow plus yesterday's remaining volume reduced by the
        /// fraction of manure removed yesterday (Equation 4.1.3-7).
        /// </summary>
        public static double NetAmountInStorage(
            double netAmountInStorageOnPreviousDay,
            double amountFlowingIntoStorage,
            double fractionRemovedOnPreviousDay)
        {
            return amountFlowingIntoStorage +
                   (netAmountInStorageOnPreviousDay * (1 - fractionRemovedOnPreviousDay));
        }

        /// <summary>
        /// Bounds a removal fraction to the physically valid range: never negative, and never above the daily emptying
        /// cap (<see cref="MaximumRemovalFraction"/>).
        /// </summary>
        public static double BoundRemovalFraction(double fraction)
        {
            if (fraction < 0)
            {
                return 0;
            }

            if (fraction > MaximumRemovalFraction)
            {
                return MaximumRemovalFraction;
            }

            return fraction;
        } 

        #endregion
    }
}
