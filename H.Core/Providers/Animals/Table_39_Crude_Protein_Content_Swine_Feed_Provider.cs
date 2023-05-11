using H.Core.Enumerations;
using H.Core.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 39. Crude protein content in feed, as fed (% of feed intake), by swine group.
    /// <para>Source: D. Beaulieu, U. of Saskatchewan, (pers. comm.)</para>
    /// </summary>
    public class Table_39_Crude_Protein_Content_Swine_Feed_Provider
    {
        #region Fields

        #endregion

        #region Constructors

        public Table_39_Crude_Protein_Content_Swine_Feed_Provider()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public Dictionary<DietType, double> GetByProvince(Province province)
        {
            switch (province)
            {
                
                case Province.BritishColumbia:
                case Province.Alberta:
                case Province.Saskatchewan:
                case Province.Manitoba:
                case Province.Ontario:
                case Province.Quebec:
                case Province.NewBrunswick:
                case Province.NovaScotia:
                case Province.PrinceEdwardIsland:
                case Province.Newfoundland:
                    return new Dictionary<DietType, double>
                    {
                        {DietType.Gestation, 14.28},
                        {DietType.Lactation, 19.07},
                        {DietType.NurseryWeanersStarter1, 23.88},
                        {DietType.NurseryWeanersStarter2, 21.45},
                        {DietType.GrowerFinisherDiet1, 20.27},
                        {DietType.GrowerFinisherDiet2, 19.89},
                        {DietType.GrowerFinisherDiet3, 19.92},
                        {DietType.GrowerFinisherDiet4, 19.66},
                        {DietType.Boar, 20.1}
                    };

               
                    }
        }

        public double GetCrudeProteinInFeedForSwineGroupByProvince(Province province, DietType dietType)
        {
            var byProvince = this.GetByProvince(province);
            if (byProvince.ContainsKey(dietType))
            {
                return byProvince[dietType];
            }
            else
            {
                System.Diagnostics.Trace.TraceError($"{nameof(Table_39_Crude_Protein_Content_Swine_Feed_Provider)}.{nameof(GetCrudeProteinInFeedForSwineGroupByProvince)}" +
                                                    $" unable to get data for province: {province} and diet type: {dietType.GetDescription()}" +
                                                    $" Returning default value of 0.");

                return 0;
            }
        }

        #endregion
    }
}


