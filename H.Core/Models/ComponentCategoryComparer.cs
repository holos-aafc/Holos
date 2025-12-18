using System.Collections;

namespace H.Core.Models
{
    public class ComponentCategoryComparer :  IComparer
    {
        private int CompareObjects(ComponentBase x, ComponentBase y)
        {
            if (x != null)
            {
                if (y != null)
                {
                    return x.ComponentType.CompareTo(y.ComponentType);
                }
            }

            return 0;
        }

        public int Compare(object x, object y)
        {
            return this.CompareObjects((ComponentBase) x, (ComponentBase) y);
        }
    }
}