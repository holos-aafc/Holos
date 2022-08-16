using System;
using System.Collections.Generic;

namespace H.Core.Emissions
{
    /// <summary>
    /// Implements both Table 68 and 69 from the Algorithm Document.
    /// <para>Table 68. Relative uncertainties for each emission category.</para>
    /// <para>Table 69. Uncertainty categories and associated estimates.</para>
    /// </summary>
    public class Table_68_69_Expression_Of_Uncertainty_Calculator
    {
        /// <summary>
        /// Equation 9.1.1-1
        /// </summary>
        /// <param name="emissionEstimatePairedWithIndividualUncertainty">Emission estimate paired with individual uncertainty</param>
        /// <returns>Uncertainty associated with net farm emission estimate</returns>
        public double CalculateUncertaintyAssociatedWithNetFarmEmissionEstimate(
            List<Tuple<double, double>> emissionEstimatePairedWithIndividualUncertainty)
        {
            double denominator = 0;
            double numerator = 0;
            double temp;
            for (var i = 0; i < emissionEstimatePairedWithIndividualUncertainty.Count; ++i)
            {
                temp = emissionEstimatePairedWithIndividualUncertainty[i]
                           .Item1 *
                       emissionEstimatePairedWithIndividualUncertainty[i]
                           .Item2;
                numerator += temp * temp;
                denominator += emissionEstimatePairedWithIndividualUncertainty[i]
                                   .Item1 *
                               emissionEstimatePairedWithIndividualUncertainty[i]
                                   .Item1;
            }

            numerator = Math.Sqrt(numerator);
            denominator = Math.Sqrt(denominator);
            return numerator / denominator;
        }

        public double EntericMethaneUncertainty
        {
            get
            {
                return 20;
            }
        }

        public double ManureMethaneUncertainty
        {
            get
            {
                return 20;
            }
        }

        public double ManureDirectNitrousOxideUncertainty
        {
            get
            {
                return 40;
            }
        }

        public double ManureIndirectNitrousOxideUncertainty
        {
            get
            {
                return 60;
            }
        }

        public double EnergyCarbonDioxideUncertainty
        {
            get
            {
                return 40;
            }
        }
    }
}