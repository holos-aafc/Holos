namespace H.Core.Models
{
    public partial class Farm
    {
        #region Fields

        private bool _showAvailableComponentsList;
        private bool _showExportEmissionsInFinalReport;

        #endregion

        #region Properties

        /// <summary>
        /// Enables the showing/hiding of the list of available components on the component selection view. This allows users with smaller screen sizes to hide this
        /// additional information.
        /// </summary>
        public bool ShowAvailableComponentsList
        {
            get => _showAvailableComponentsList;
            set => SetProperty(ref _showAvailableComponentsList, value);
        }

        /// <summary>
        /// Includes/excludes the report of emissions related to exports from the farm (e.g. manure and/or crop residue exports_
        /// </summary>
        public bool ShowExportEmissionsInFinalReport
        {
            get => _showExportEmissionsInFinalReport;
            set => SetProperty(ref _showExportEmissionsInFinalReport, value);
        }

        #endregion
    }
}