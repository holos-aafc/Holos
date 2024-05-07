using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Core.Providers.Animals;
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
        private ObservableCollection<ManureStateType> _validManureStateTypesForSelectedTypeOfAnimalManure;
        private ManureStateType _manureStateType;

        #endregion

        #region Constructors

        public ManureSubstrateViewItem()
        {
            this.ValidManureStateTypesForSelectedTypeOfAnimalManure = new ObservableCollection<ManureStateType>()
            {
                ManureStateType.NotSelected,
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Each view item must have its own collection of valid state types so the table rows presented to the user will have their own distinct collection
        /// </summary>
        public ObservableCollection<ManureStateType> ValidManureStateTypesForSelectedTypeOfAnimalManure
        {
            get => _validManureStateTypesForSelectedTypeOfAnimalManure;
            set => SetProperty(ref _validManureStateTypesForSelectedTypeOfAnimalManure, value);
        }

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

        public ManureStateType ManureStateType
        {
            get => _manureStateType;
            set => SetProperty(ref _manureStateType, value);
        }

        #endregion
    }
}
