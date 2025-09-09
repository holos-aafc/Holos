using System;
using System.Collections;

namespace H.Core.Models
{
    public class GroupPathComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return CompareObjects((ComponentBase)x, (ComponentBase)y);
        }

        private int CompareObjects(ComponentBase x, ComponentBase y)
        {
            if (x != null)
                if (y != null)
                    return string.Compare(x.GroupPath, y.GroupPath, StringComparison.Ordinal);

            return 0;
        }
    }
}