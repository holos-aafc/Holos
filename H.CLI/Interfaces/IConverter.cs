using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models;
using H.Core.Models.Animals.Beef;

namespace H.CLI.Interfaces
{
    public interface IConverter
    {
        /// <summary>
        /// Parses all of the rows from each of the component input files
        /// </summary>
        /// <param name="concreteComponentList">Contains a list of temporary inputs. The temporary inputs are grouped by component type.</param>
        /// 
        /// i.e. If there were two input files; one for a <see cref="CowCalfComponent"/> and one for a <see cref="FinishingComponent"/>, the list would
        /// have two items. Each item would then have it's own separate list of rows from the input files.
        /// <param name="farm">The <see cref="Farm"/> that the components belong to</param>
        /// <returns>A list of converted components</returns>
        List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> concreteComponentList, Farm farm);
    }
}
