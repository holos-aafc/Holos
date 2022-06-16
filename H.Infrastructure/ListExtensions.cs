#region Imports

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace H.Infrastructure
{
    /// <summary>
    /// </summary>
    public static class ListExtensions
    {
        #region Constructors

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public static List<T> ShiftLeft<T>(this List<T> list, int shiftBy)
        {
            if (list.Count <= shiftBy)
            {
                return list;
            }

            var result = list.GetRange(shiftBy, list.Count - shiftBy);
            result.AddRange(list.GetRange(0, shiftBy));
            return result;
        }

        public static List<T> ShiftRight<T>(this List<T> list, int shiftBy)
        {
            if (list.Count <= shiftBy)
            {
                return list;
            }

            var result = list.GetRange(list.Count - shiftBy, shiftBy);
            result.AddRange(list.GetRange(0, list.Count - shiftBy));
            return result;
        }

        public static double WeightedAverage<T>(this IEnumerable<T> records, 
                                                Func<T, double> value,
                                                Func<T, double> weight)
        {
            var weightedValueSum = records.Sum(record => value(record) * weight(record));
            var weightSum = records.Sum(record => weight(record));

            if (Math.Abs(weightSum) > double.Epsilon)
            {
                return weightedValueSum / weightSum;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}