using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum SeparationCoefficients
    {
        /// <summary>
        /// Symbol: 𝛼𝑓𝑙𝑜𝑤
        /// Fraction of raw material in solid fraction following solid-liquid separation
        /// </summary>
        [LocalizedDescription("EnumFractionRawMaterials", typeof(Resources))]
        FractionRawMaterials,

        /// <summary>
        /// Symbol: 𝛼𝑇𝑆
        /// Fraction of total solids in solid fraction following solid-liquid separation
        /// </summary>
        [LocalizedDescription("EnumFractionTotalSolids", typeof(Resources))]
        FractionTotalSolids,

        /// <summary>
        /// Symbol: 𝛼𝑉𝑆
        /// Fraction of volatile solids in solid fraction following solid-liquid separation
        /// </summary>
        [LocalizedDescription("EnumFractionVolatileSolids", typeof(Resources))]
        FractionVolatileSolids,

        /// <summary>
        /// Symbol: 𝛼𝑇𝐴𝑁
        /// Fraction of total ammonium nitrogen in solid fraction following solid-liquid separation
        /// </summary>
        [LocalizedDescription("EnumFractionTotalAmmoniumNitrogen", typeof(Resources))]
        FractionTotalAmmoniumNitrogen,

        /// <summary>
        /// Symbol: 𝛼𝑁𝑜𝑟𝑔
        /// Fraction of organic nitrogen in solid fraction following solid-liquid separation
        /// </summary>
        [LocalizedDescription("EnumFractionOrganicNitrogen", typeof(Resources))]
        OrganicNitrogen,
    }
}
