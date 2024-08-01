using System.Collections.Generic;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services
{
    public interface IFieldComponentHelper
    {
        void InitializeComponent(FieldSystemComponent component, Farm farm);
        void Replicate(ComponentBase copyFrom, ComponentBase copyTo);
        FieldSystemComponent Replicate(FieldSystemComponent component);
        string GetUniqueFieldName(IEnumerable<FieldSystemComponent> components);
    }
}