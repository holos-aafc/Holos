using System;

namespace H.Infrastructure
{
    public static class MathHelpers
    {
        public static int Closest(int[] values, int target)
        {
            int index = Array.BinarySearch(values, target);

            if (index >= 0)
            {
                // Exact match, target in values
                return values[index];
            }
            else if (index == -1)
            {
                // Target is less than any item in values
                return values[0];
            }
            else if (index < -values.Length)
            {
                // Target is greater than any item in values
                return values[values.Length - 1];
            }
            else
            {
                // Target is in [left..right] range
                // C# specific range encoding
                int left = ~index - 1;
                int right = ~index;

                if (Math.Abs(values[left] - target) < Math.Abs(values[right] - target))
                    return values[left];
                else
                    return values[right];
            }
        }
    }
}