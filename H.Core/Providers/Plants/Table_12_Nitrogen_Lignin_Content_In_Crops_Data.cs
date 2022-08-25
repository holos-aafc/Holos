using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class Table_12_Nitrogen_Lignin_Content_In_Crops_Data
    {
        /// <summary>
        /// The crop type for which we need the various information and values.
        /// </summary>
        public CropType CropType { get; set; }

        /// <summary>
        /// The intercept value given the crop type. Taken from national inventory numbers.
        /// </summary>
        public double InterceptValue { get; set; }

        /// <summary>
        /// The slop value given the crop type. Taken from national inventory numbers.
        /// </summary>
        public double SlopeValue { get; set; }

        /// <summary>
        /// Shoot to root ratio of the crop. The ratio of below-ground root biomass to above-ground shoot  
        /// </summary>
        public double RSTRatio { get; set; }

        /// <summary>
        /// Nitrogen Content of residues. Unit of measurement = Proportion of Carbon content
        /// </summary>
        public double NitrogenContentResidues { get; set; }

        /// <summary>
        /// Lignin content of residue. Unit of measurement = Proportion of Carbon content
        /// </summary>
        public double LigninContentResidues { get; set; }

        /// <summary>
        /// Moisure content of crop.
        /// Unit of Measurement = %
        /// </summary>
        public double MoistureContent { get; set; }

    }
}
