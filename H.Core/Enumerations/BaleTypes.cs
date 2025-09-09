﻿using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    ///     Bales produced on farm are either straw or hay.
    /// </summary>
    public enum BaleTypes
    {
        /// <summary>
        ///     Hay bales have a lifespan of 5 years.
        /// </summary>
        [LocalizedDescription("EnumHay", typeof(Resources))]
        Hay,

        /// <summary>
        ///     Straw bales have a lifespan of 2 years.
        /// </summary>
        [LocalizedDescription("EnumStraw", typeof(Resources))]
        Straw
    }
}