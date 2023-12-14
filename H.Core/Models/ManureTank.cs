using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models
{
    /// <summary>
    /// A storage tank for a particular type of manure (beef, dairy, etc.)
    /// </summary>
    public class ManureTank : StorageTankBase
    {
        #region Fields

        private ManureStateType _manureStateType;
        private AnimalType _animalType;

        #endregion

        #region Properties

        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
        }

        public ManureStateType ManureStateType
        {
            get => _manureStateType;
            set => SetProperty(ref _manureStateType, value);
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(AnimalType)}: {AnimalType}";
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}