namespace H.Avalonia.Models
{
    public class SoilViewItem : ModelBase
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public SoilViewItem()
        {
            Latitude = 0d;
            Longitude = 0d;
            IsSelected = false;
        }
    }
}