﻿using System.Collections;

namespace H.Core.Models
{
    public class ComponentCategoryComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return CompareObjects((ComponentBase)x, (ComponentBase)y);
        }

        private int CompareObjects(ComponentBase x, ComponentBase y)
        {
            if (x != null)
                if (y != null)
                    return x.ComponentType.CompareTo(y.ComponentType);

            return 0;
        }
    }
}