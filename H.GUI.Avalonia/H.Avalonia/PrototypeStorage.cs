using H.Avalonia.Models;
using Prism.Mvvm;
using SharpKml.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Avalonia
{
    public class PrototypeStorage : BindableBase
    {
        private bool _showSingleCoordinateResults;
        private bool _showMultipleCoordinateResults;

        /// <summary>
        /// A collection of viewitems added by the user for which data is required.
        /// </summary>
        public ObservableCollection<ClimateViewItem>? ClimateViewItems { get; set; }

        /// <summary>
        /// A collection of viewitems added by the user for which data is required.
        /// </summary>
        public ObservableCollection<SoilViewItem>? SoilViewItems { get; set; }

        /// <summary>
        /// Used in the single coordinate tab of the Soil Data View page.
        /// </summary>
        public SoilViewItem SingleSoilViewItem { get; set; }

        /// <summary>
        /// A bool to indicate whether to show the single coordinate results. The results are shown if true, the results are hidden otherwise.
        /// </summary>
        public bool ShowSingleCoordinateResults
        {
            get => _showSingleCoordinateResults;
            set => SetProperty(ref _showSingleCoordinateResults, value);
        }

        /// <summary>
        /// A bool to indicate whether to show the multiple coordinate results. The results are shown if true, the results are hidden otherwise.
        /// </summary>
        public bool ShowMultipleCoordinateResults
        {
            get => _showMultipleCoordinateResults;
            set => SetProperty(ref _showMultipleCoordinateResults, value);
        }

        public PrototypeStorage()
        {
            ClimateViewItems = new ObservableCollection<ClimateViewItem>();
            SoilViewItems = new ObservableCollection<SoilViewItem>();
            SingleSoilViewItem = new SoilViewItem();
        }
    }
}
