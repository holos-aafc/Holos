using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models.Infrastructure;

namespace H.Core.Services
{
    public interface IAnaerobicDigestionComponentHelper
    {

        string GetUniqueAnaerobicDigestionName(IEnumerable<AnaerobicDigestionComponent> components);

    }
}
