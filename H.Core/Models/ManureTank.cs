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

        private AnimalType _animalType;

        #endregion

        #region Properties

        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
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