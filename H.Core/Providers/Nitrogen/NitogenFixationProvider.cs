using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Nitrogen
{
    public class NitogenFixationProvider
    {
        #region Fields

        private readonly List<NitrogenFixationResult> _table;

        #endregion

        #region Constructors

        public NitogenFixationProvider()
        {
            HTraceListener.AddTraceListener();
            _table = new List<NitrogenFixationResult>()
            {
                new NitrogenFixationResult() { CropType = CropType.Soybeans, Fixation = 108 },
                new NitrogenFixationResult() { CropType = CropType.Peas, Fixation = 126 },
                new NitrogenFixationResult() { CropType = CropType.Lentils, Fixation = 40 },
                new NitrogenFixationResult() { CropType = CropType.ColouredWhiteFabaBeans, Fixation = 129 },
                new NitrogenFixationResult() { CropType = CropType.Chickpeas, Fixation = 61 },
                new NitrogenFixationResult() { CropType = CropType.BeansDryField, Fixation = 38 },
                new NitrogenFixationResult() { CropType = CropType.WhiteBeans, Fixation = 38 },
                new NitrogenFixationResult() { CropType = CropType.BeansWhite, Fixation = 38 },
                new NitrogenFixationResult() { CropType = CropType.FabaBeans, Fixation = 129 },                
                new NitrogenFixationResult() { CropType = CropType.DryPeas, Fixation = 126 },
                new NitrogenFixationResult() { CropType = CropType.FieldPeas, Fixation = 126 },
                new NitrogenFixationResult() { CropType = CropType.PulseCrops, Fixation = 9 },

                new NitrogenFixationResult() { CropType = CropType.Vegetables, Fixation = 10 },

                // Other field crops
                new NitrogenFixationResult() { CropType = CropType.Safflower, Fixation = 9 },
                new NitrogenFixationResult() { CropType = CropType.SunflowerSeed, Fixation = 9 },
                new NitrogenFixationResult() { CropType = CropType.Tobacco, Fixation = 9 },
                new NitrogenFixationResult() { CropType = CropType.BerriesAndGrapes, Fixation = 9 },
                new NitrogenFixationResult() { CropType = CropType.OtherFieldCrops, Fixation = 9 },

                // Perennials
                new NitrogenFixationResult() { CropType = CropType.TameLegume, Fixation =  86},
                new NitrogenFixationResult() { CropType = CropType.TameMixed, Fixation =  12},
                new NitrogenFixationResult() { CropType = CropType.SeededGrassland, Fixation = 2 },
                new NitrogenFixationResult() { CropType = CropType.ForageForSeed, Fixation = 4 },
            };
        }

        #endregion

        #region Public Methods

        public NitrogenFixationResult GetNitrogenFixationResult(CropType cropType)
        {
            // Table values are no longer used. A default of 0.7 is used for legumous crops and 0 for non-legumous crops
            if (cropType.IsPulseCrop())
            {
                return new NitrogenFixationResult() {Fixation = 0.7};
            }
            else
            {
                return new NitrogenFixationResult() { Fixation = 0 };
            }
        }

        #endregion
    }
}