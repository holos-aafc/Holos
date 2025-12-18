using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Providers.Animals;

namespace H.Core.Models.Infrastructure
{
    public class ManureSubstrateViewItem : SubstrateViewItemBase
    {
        #region Constructors

        public ManureSubstrateViewItem()
        {
            ValidManureStateTypesForSelectedTypeOfAnimalManure = new ObservableCollection<ManureStateType>
            {
                ManureStateType.NotSelected
            };

            ManureCompositionData = new DefaultManureCompositionData();
        }

        #endregion

        #region Fields

        private AnimalType _animalType;
        private BeddingMaterialType _beddingMaterialType;
        private double _dailyManureAddedToDigester;
        private ManureSubstrateState _manureSubstrateState;
        private ObservableCollection<ManureStateType> _validManureStateTypesForSelectedTypeOfAnimalManure;
        private ManureStateType _manureStateType;
        private DefaultManureCompositionData _manureCompositionData;

        #endregion

        #region Properties

        /// <summary>
        ///     Each view item must have its own collection of valid state types so the table rows presented to the user will have
        ///     their own distinct collection
        /// </summary>
        public ObservableCollection<ManureStateType> ValidManureStateTypesForSelectedTypeOfAnimalManure
        {
            get => _validManureStateTypesForSelectedTypeOfAnimalManure;
            set => SetProperty(ref _validManureStateTypesForSelectedTypeOfAnimalManure, value);
        }

        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
        }

        public BeddingMaterialType BeddingMaterialType
        {
            get => _beddingMaterialType;
            set => SetProperty(ref _beddingMaterialType, value);
        }

        public double DailyManureAddedToDigester
        {
            get => _dailyManureAddedToDigester;
            set => SetProperty(ref _dailyManureAddedToDigester, value);
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

        public DefaultManureCompositionData ManureCompositionData
        {
            get => _manureCompositionData;
            set => SetProperty(ref _manureCompositionData, value);
        }

        #endregion
    }
}