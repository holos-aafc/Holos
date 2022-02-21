using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Calculators.Climate
{
    public class ClimateParameterDailyResult
    {
        #region Properties
        
        public double ClimateParameter { get; set; }

        public double InputTemperature { get; set; }
        public double InputPrecipitation { get; set; }
        public double InputEvapotranspiration { get; set; }
        public int JulianDay { get; set; }
        public double GreenAreaIndex { get; set; }
        public double SurfaceTemperature  { get; set; }
        public double SoilTemperature { get; set; }
        public double CropCoefficient { get; set; }
        public double CropInterception { get; set; }
        public double VolumetricSoilWaterContent { get; set; }
        public double ActualEvapotranspiration { get; set; }
        public double DeepPercolation { get; set; }
        public double ReferenceEvapotranspiration { get; set; }
        public double SoilAvailableWater { get; set; }
        public double WaterStorage { get; set; }
        public double ClimateParamterTemperature { get; set; }
        public double ClimateParameterWater { get; set; }
        public double FieldCapacity { get; set; }
        public double WiltingPoint { get; set; }

        #endregion

        public override string ToString()
        {
            return $"{nameof(ClimateParameter)}: {this.ClimateParameter}";
        }
    }
}
