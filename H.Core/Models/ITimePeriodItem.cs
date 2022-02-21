using System;

namespace H.Core.Models
{
    public interface ITimePeriodItem
    {
        DateTime Start { get; set; }
        DateTime End { get; set; }
        TimeSpan Duration { get; }
        string Name { get; set; }
    }
}