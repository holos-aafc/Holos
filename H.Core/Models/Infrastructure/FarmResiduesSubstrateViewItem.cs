using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.Infrastructure
{
    public class FarmResiduesSubstrateViewItem : SubstrateViewItemBase
    {
        #region Fields
        
        private FarmResidueType _farmResidueType;
        private DateTime _startDate;
        private DateTime _endDate;

        #endregion

        #region Constructors

        public FarmResiduesSubstrateViewItem()
        {
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now.AddDays(1);
        }

        #endregion

        #region Properties

        public FarmResidueType FarmResidueType
        {
            get => _farmResidueType;
            set => this.SetProperty(ref _farmResidueType, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        #endregion
    }
}
