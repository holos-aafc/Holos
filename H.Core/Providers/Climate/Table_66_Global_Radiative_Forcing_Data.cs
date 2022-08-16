using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public class Table_66_Global_Radiative_Forcing_Data
    {
        /// <summary>
        /// The year for the global radiative forcing value
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The emission type whose value is reuquired
        /// </summary>
        public EmissionTypes EmissionType { get; set; }

        /// <summary>
        /// The value of the emission type given a year. Unit of measurement W m-2
        /// </summary>
        public double RadiativeForcingValue { get; set; }
    }
}
