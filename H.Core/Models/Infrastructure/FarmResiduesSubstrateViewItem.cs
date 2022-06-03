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
        private FarmResidueType _farmResidueType;

        public FarmResidueType FarmResidueType
        {
            get => _farmResidueType;
            set => this.SetProperty(ref _farmResidueType, value);
        }
    }
}
