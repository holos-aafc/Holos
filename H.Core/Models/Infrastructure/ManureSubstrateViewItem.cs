using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.Infrastructure
{
    public class ManureSubstrateViewItem : SubstrateViewItemBase
    {
        #region Fields
        
        private AnimalType _animalType;
        private BeddingMaterialType _beddingMaterialType;
        private double _dailyManureAddedToDigester;
        private ManureSubstrateState _manureSubstrateState;

        #endregion

        #region Constructors

        public ManureSubstrateViewItem()
        {
        }

        #endregion

        #region Properties

        public AnimalType AnimalType
        {
            get => _animalType;
            set => this.SetProperty(ref _animalType, value);
        }

        public BeddingMaterialType BeddingMaterialType
        {
            get => _beddingMaterialType;
            set => this.SetProperty(ref _beddingMaterialType, value);
        }

        public double DailyManureAddedToDigester
        {
            get => _dailyManureAddedToDigester;
            set => this.SetProperty(ref _dailyManureAddedToDigester, value);
        }

        public ManureSubstrateState ManureSubstrateState
        {
            get => _manureSubstrateState;
            set => SetProperty(ref _manureSubstrateState, value);
        }

        #endregion
    }
}
