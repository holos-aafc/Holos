using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals.Table_69
{
    public interface ITable_69_Provider
    {
        Table_69_Data GetData(AnimalType animalType, Province province, int year);
    }
}