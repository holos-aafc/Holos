using H.Infrastructure;
using Prism.Mvvm;

namespace H.Core.Models.Results
{
    public class ResultsViewItemBase : ModelBase
    {
        #region Fields

        private ComponentBase _component;
        private string _groupingString;

        #endregion

        #region Properties

        public ComponentBase Component
        {
            get => _component;
            set => SetProperty(ref _component, value);
        }

        public string GroupingString
        {
            get => _groupingString;
            set => SetProperty(ref _groupingString, value);
        }

        #endregion
    }
}