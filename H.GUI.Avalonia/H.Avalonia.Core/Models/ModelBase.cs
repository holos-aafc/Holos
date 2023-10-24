using Prism.Mvvm;

namespace H.Avalonia.Core.Models
{
    /// <summary>
    /// The base class for various model classes.
    /// </summary>
    public class ModelBase : BindableBase
    {
        private double _longitude;
        private double _latitude;
        private bool _isSelected;

        /// <summary>
        /// The latitude value specified by the user.
        /// </summary>
        public double Latitude
        {
            get => _latitude;
            set
            {
                if (value is < -90 or > 90) value = 0;
                SetProperty(ref _latitude, value);
            }
        }

        /// <summary>
        /// The longitude value specified by the user.
        /// </summary>
        public double Longitude
        {
            get => _longitude;
            set
            {
                if (value is < -180 or > 180) value = 0;
                SetProperty(ref _longitude, value);
            }
        }
        
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
