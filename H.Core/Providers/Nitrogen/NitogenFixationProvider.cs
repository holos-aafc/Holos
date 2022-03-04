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
                new NitrogenFixationResult() { CropType = CropType.HayLegume, Fixation =  86},
                new NitrogenFixationResult() { CropType = CropType.HayMixed, Fixation =  12},
                new NitrogenFixationResult() { CropType = CropType.Pasture, Fixation = 2 },
                new NitrogenFixationResult() { CropType = CropType.ForageForSeed, Fixation = 4 },
                new NitrogenFixationResult() { CropType = CropType.RangelandNative, Fixation = 0 },
                new NitrogenFixationResult() { CropType = CropType.SeededGrassland, Fixation = 2 },
                new NitrogenFixationResult() { CropType = CropType.TameGrass, Fixation = 0 },
                new NitrogenFixationResult() { CropType = CropType.TameLegume, Fixation = 86 },
                new NitrogenFixationResult() { CropType = CropType.TameMixed, Fixation = 12 },
                new NitrogenFixationResult() { CropType = CropType.ForageForSeed, Fixation = 4 },
            };
        }

        #endregion

        #region Public Methods

        public NitrogenFixationResult GetNitrogenFixationResult(CropType cropType)
        {
            const double defaultValue = 0;

            var result = _table.FirstOrDefault(fixationResult => fixationResult.CropType == cropType);
            if (result != null)
            {
                return result;
            }
            else
            {
                Trace.TraceError($"{nameof(NitogenFixationProvider)}.{nameof(GetNitrogenFixationResult)} no result found for crop type: '{cropType.GetDescription()}'. Returning default of {defaultValue}.");

                return new NitrogenFixationResult() {Fixation = defaultValue };
            }
        }

        #endregion
    }
}