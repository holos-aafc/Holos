using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;
using H.Core.Models;
using H.Core.Models.LandManagement.Shelterbelt;

namespace H.CLI.Converters
{
    public class ShelterbeltConverter : IConverter
    {
        #region Fields
        public List<ComponentBase> ShelterBeltComponents { get; set; } = new List<ComponentBase>();

        #endregion

        #region Constructors
        public ShelterbeltConverter() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Casts the list of lists into ShelterBeltTemporaryInput's and processes the data for each Shelterbelt file into a ShelterbeltComponent
        /// </summary>
        /// <param name="shelterBeltList"></param>
        /// <returns></returns>
        public List<ComponentBase> ConvertParsedComponent(List<List<IComponentTemporaryInput>> shelterBeltList, Farm farm)
        {
            var castedShelterBeltList = new List<List<IGrouping<int, ShelterBeltTemporaryInput>>>();
            foreach (var list in shelterBeltList)
            {
                var groupedShelterbelt = list.Cast<ShelterBeltTemporaryInput>().GroupBy(x => x.RowID).ToList();
                castedShelterBeltList.Add(groupedShelterbelt);
            }
         
            foreach (var groupedShelterbelt in castedShelterBeltList)
            {
                bool setShelterNameOnce = false;
                var tempShelterBeltComponent = new ShelterbeltComponent()
                {
                    Guid = Guid.NewGuid(),
                    RowData = new ObservableCollection<RowData>(),   
                };

                foreach (var group in groupedShelterbelt)
                {
                    bool rowHasBeenSet = false;
                    var tempRowData = new RowData();

                    foreach (var tempShelterbeltObject in group)
                    {
                        //create one row because they are grouped by row id
                        if (!setShelterNameOnce)
                        {
                            tempShelterBeltComponent.FakeName = tempShelterbeltObject.Name;
                            tempShelterBeltComponent.HardinessZone = tempShelterbeltObject.HardinessZone;
                            tempShelterBeltComponent.EcoDistrictId = tempShelterbeltObject.EcodistrictID;
                            tempShelterBeltComponent.ComponentType = ComponentType.Shelterbelt;
                            setShelterNameOnce = true;
                        }

                        if (!rowHasBeenSet)
                        {
                            tempRowData.ParentShelterbeltComponent = tempShelterBeltComponent.Guid;
                            tempRowData.Guid = tempShelterbeltObject.Guid;
                            tempRowData.Length = tempShelterbeltObject.RowLength;
                            tempRowData.Name = tempShelterbeltObject.RowName;
                            tempRowData.NameIsFromUser = true;
                            tempRowData.YearOfObservation = tempShelterbeltObject.YearOfObservation;
                            rowHasBeenSet = true;
                        }
                            
                        //for each group that has the same row ID, create a new tree group and add that tree group to it
                        var tempTreeData = new TreeGroupData()
                        {
                            ParentRowData = tempRowData.Guid,
                            GrandParentShelterbeltComponent = tempShelterBeltComponent.Guid,
                            Guid = Guid.NewGuid(),
                            CircumferenceData = new CircumferenceData()
                            {
                                UserCircumference = tempShelterbeltObject.AverageCircumference,
                                CircumferenceGenerationOverriden = true,
                            },

                            PlantYear = tempShelterbeltObject.PlantYear,
                            PlantedTreeCount = tempShelterbeltObject.PlantedTreeCount,
                            CutYear = tempShelterbeltObject.CutYear,
                            TreeSpecies = tempShelterbeltObject.Species,
                            LiveTreeCount = tempShelterbeltObject.LiveTreeCount,
                            PlantedTreeSpacing = tempShelterbeltObject.PlantedTreeSpacing     
                        };

                        tempRowData.TreeGroupData.Add(tempTreeData); 
                    }
                      tempShelterBeltComponent.RowData.Add(tempRowData);
                }
                ShelterBeltComponents.Add(tempShelterBeltComponent);
                #endregion
            }
            return ShelterBeltComponents;
        }
    }
}

