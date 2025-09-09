using System.ComponentModel;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Models;
using H.Infrastructure;

namespace H.Core.Test;

[TestClass]
public class ConverterTest
{
    [TestMethod]
    public void Test()
    {
        var inputView = new ViewModel();

        // Mimic user entering input

        inputView.InputViewClass.SoilDepth = 20; // Currently in metric.

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
    private Farm _activeFarm;
    private InputViewClass _algorithmInputViewClass;
    private IUnitsOfMeasurementCalculator _calculator = new UnitsOfMeasurementCalculator();

    //public InputViewClass AlgorithemInputViewClass
    //{
    //    get => _calculator.ConvertToMetricFromImperial(Inches, this.InputViewClass);

    //}


    public ViewModel()
    {
        InputViewClass = new InputViewClass();
    }

    public InputViewClass InputViewClass { get; set; }

    public void InitializeViewModel()
    {
        InitializeBindingClass();
    }

    private void InitializeBindingClass()
    {
        //this.InputViewClass.SoilDepth // _calc.convert(..farm.soildata.depth)

        InputViewClass.PropertyChanged += InputViewClassOnPropertyChanged;
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
            if (e.PropertyName.Equals(nameof(InputViewClass.SoilDepth)))
            {
                //this.AlgorithemInputViewClass.SoilDepth = _calculator.GetUnitsOfMeasurementValue(
                //    _activeFarm.MeasurementSystemType, MetricUnitsOfMeasurement.Millimeters, input.SoilDepth,
                //    false);
            }
    }
}