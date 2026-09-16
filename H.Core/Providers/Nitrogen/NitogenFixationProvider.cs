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

            /*
             * %NDFA - the share of a crop's nitrogen derived from the atmosphere - held as a fraction, taken from the
             * Nfixation definition in the algorithm document (Eq. 2.5.5-6, Eq. 2.6.8-12 and Eq. 2.7.7-11), sourced to
             * Karimi et al. (2020).
             *
             * Only the crops named there fix nitrogen. Anything absent from this table fixes none, which includes
             * legumes the document does not give a value for - faba beans and white beans among them.
             */
            _table = new List<NitrogenFixationResult>()
            {
                new NitrogenFixationResult() { CropType = CropType.Soybeans, Fixation = 0.55 },
                new NitrogenFixationResult() { CropType = CropType.DryPeas, Fixation = 0.54 },
                new NitrogenFixationResult() { CropType = CropType.FieldPeas, Fixation = 0.54 },
                new NitrogenFixationResult() { CropType = CropType.BeansDryField, Fixation = 0.40 },
                new NitrogenFixationResult() { CropType = CropType.Lentils, Fixation = 0.53 },
                new NitrogenFixationResult() { CropType = CropType.Chickpeas, Fixation = 0.52 },

                // The document calculates this one as the average of beans (dry field), chickpeas, dry/field peas,
                // lentils and soybeans.
                new NitrogenFixationResult() { CropType = CropType.PulseCrops, Fixation = 0.51 },

                // Perennials. These fixed no nitrogen before, because the previous code answered from IsPulseCrop()
                // and neither of them is a pulse crop.
                new NitrogenFixationResult() { CropType = CropType.TameLegume, Fixation = 0.66 },
                new NitrogenFixationResult() { CropType = CropType.TameMixed, Fixation = 0.66 },
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The share of the crop's nitrogen fixed from the atmosphere, as a fraction. Zero for a crop that fixes none.
        ///
        /// This used to ignore the table and answer a flat 0.7 for anything IsPulseCrop() recognised, and zero for
        /// everything else, so every pulse shared one value and the two perennial legumes fixed nothing at all.
        /// </summary>
        public NitrogenFixationResult GetNitrogenFixationResult(CropType cropType)
        {
            var result = _table.SingleOrDefault(entry => entry.CropType == cropType);

            return result ?? new NitrogenFixationResult() { CropType = cropType, Fixation = 0 };
        }

        #endregion
    }
}
