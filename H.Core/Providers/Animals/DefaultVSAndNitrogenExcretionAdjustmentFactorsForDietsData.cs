namespace H.Core.Providers.Animals
{
    public class DefaultVSAndNitrogenExcretionAdjustmentFactorsForDietsData
    {
        #region Methods

        public override string ToString()
        {
            return
                $"{nameof(Name)}: {Name}, {nameof(VSAdjustment)}: {VSAdjustment}, {nameof(NitrogenExcretedAdjustment)}: {NitrogenExcretedAdjustment}";
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public double VSAdjustment { get; set; }
        public double NitrogenExcretedAdjustment { get; set; }

        #endregion
    }
}