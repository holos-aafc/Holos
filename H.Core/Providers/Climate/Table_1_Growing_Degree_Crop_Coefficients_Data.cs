﻿using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public class Table_1_Growing_Degree_Crop_Coefficients_Data
    {
        public CropType Crop { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }
        public double E { get; set; }
    }
}