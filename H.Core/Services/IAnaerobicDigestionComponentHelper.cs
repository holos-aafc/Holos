using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Infrastructure;

namespace H.Core.Services
{
    public interface IAnaerobicDigestionComponentHelper
    {

        string GetUniqueAnaerobicDigestionName(IEnumerable<AnaerobicDigestionComponent> components);

        void InitializeAnaerobicDigestionComponent(AnaerobicDigestionComponent component, Farm activeFarm);

        /// <summary>
        /// Maps the passed <see cref="AnaerobicDigestionComponent"/> to a new instance of <see cref="AnaerobicDigestionComponent"/> while maintaining necessary relationships with a replicated <see cref="IEnumerable{AnimalComponentBase}"/> of <see cref="AnimalComponentBase"/> objects.
        /// </summary>
        /// <param name="component">The original <see cref="AnaerobicDigestionComponent"/> that will be replicated</param>
        /// <param name="replicatedAnimalComponents">The replicated <see cref="IEnumerable{AnimalComponentBase}"/> containing <see cref="AnimalComponentBase"/> that will be used to maintain the relationship between the <see cref="AnaerobicDigestionComponent"/> and the <see cref="AnimalComponentBase"/> within the farm.</param>
        /// <returns></returns>
        AnaerobicDigestionComponent Replicate(AnaerobicDigestionComponent component, IEnumerable<AnimalComponentBase> replicatedAnimalComponents);

        /// <summary>
        /// Maps the passed <see cref="AnaerobicDigestionComponent"/> to a new instance of <see cref="AnaerobicDigestionComponent"/> while maintaining necessary relationships with a replicated <see cref="IEnumerable{AnimalComponentBase}"/> of <see cref="AnimalComponentBase"/> objects.
        /// </summary>
        /// <param name="source">The original <see cref="AnaerobicDigestionComponent"/> that will be replicated</param>
        /// <param name="destination">The new <see cref="AnaerobicDigestionComponent"/> that will have its data associated with replicated <see cref="AnimalComponentBase"/>/param>
        /// <param name="replicatedAnimalComponents">The replicated <see cref="IEnumerable{AnimalComponentBase}"/> containing <see cref="AnimalComponentBase"/> that will be used to maintain the relationship between the <see cref="AnaerobicDigestionComponent"/> and the <see cref="AnimalComponentBase"/> within the farm.</param>
        void Replicate(ComponentBase source, ComponentBase destination, IEnumerable<AnimalComponentBase> replicatedAnimalComponents);

    }
}
