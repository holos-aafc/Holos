using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Providers.AnaerobicDigestion
{
    // Provider that calculates the Biodegraded fraction during storage.
    public class BiodegradedFractionDuringStorageData
    {
        /// <summary>
        /// Represents biodegraded fraction during storage of digestate in various states.
        /// </summary>
        public double NoCover { get; set; }

        /// <summary>
        /// Represents biodegraded fraction during storage of digestate in various states.
        /// </summary>
        public double Cover { get; set; }
    }
}
