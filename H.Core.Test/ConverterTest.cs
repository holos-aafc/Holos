using System.ComponentModel;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Models;
using H.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test
{
    [TestClass]
    public class ConverterTest
    {
        [TestMethod]
        public void Test()
        {
            var inputView = new ViewModel();

            // Mimic user entering input

            inputView.InputViewClass.SoilDepth = 20;   // Currently in metric.

            // Initialize results view model
        }
    }

    public class InputViewClass : ModelBase
    {
        private double _soilDepth;

        public double SoilDepth
        {
            get => _soilDepth;
            set => SetProperty(ref _soilDepth, value);
        }
    }

    /* 
     *
     * <xaml>
     * <TextBlock Text={Binding Path=InputViewClass.SoilDepth}/>
     * <xaml/>
     */

    public class ViewModel : ModelBase
    {
        private IUnitsOfMeasurementCalculator _calculator = new UnitsOfMeasurementCalculator();
        private InputViewClass _inputViewClass;
        private InputViewClass _algorithmInputViewClass;
        private Farm _activeFarm;
        public InputViewClass InputViewClass
        {
            get => _inputViewClass;
            set => _inputViewClass = value;
        }

        //public InputViewClass AlgorithemInputViewClass
        //{
        //    get => _calculator.ConvertToMetricFromImperial(Inches, this.InputViewClass);

        //}


        public ViewModel()
        {
            this.InputViewClass = new InputViewClass();
        }

        public void InitializeViewModel()
        {
            this.InitializeBindingClass();
        }

        private void InitializeBindingClass()
        {
            //this.InputViewClass.SoilDepth // _calc.convert(..farm.soildata.depth)

            this.InputViewClass.PropertyChanged += InputViewClassOnPropertyChanged;
        }

        //not always?
        private void AlgorithmInputViewClassOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is InputViewClass input)
            {
                //if (e.PropertyName.Equals(nameof(AlgorithmInputView.soil)))
                //{
                //    //this.InputView = _calculator.GetUnitsOfMeasurementValue(...AlgorithmInputView.SoilDepth ...)
                //}
            }
        }

        //always?
        private void InputViewClassOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is InputViewClass input)
            {
                if (e.PropertyName.Equals(nameof(InputViewClass.SoilDepth)))
                {
                    //this.AlgorithemInputViewClass.SoilDepth = _calculator.GetUnitsOfMeasurementValue(
                    //    _activeFarm.MeasurementSystemType, MetricUnitsOfMeasurement.Millimeters, input.SoilDepth,
                    //    false);
                }
            }
        }
    }
}