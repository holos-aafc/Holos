using System;
using System.Collections.Generic;
using H.CLI.Interfaces;

namespace H.CLI.Handlers
{
    public class GuidComponentHandler
    {
        #region Struct
        /// <summary>
        /// A temporary struct used to store the Id of the object we are looking for as well as a reference to an IComponentTemporaryInput. This is used to
        /// create a list of objects with unique ID's in the list that is passed in to the GenerateComponentGUIDs method. Once the unique id is found, any object with
        /// the same ID will be set with the same GUID as that unique ID object because they reference the same row. 
        /// For example, two concrete ShelterbeltTemporaryInput objects
        /// may refer to the same row, therefore they should have the same row GUID. 
        /// or two FieldTemporaryInput objects may refer to the same perennial group stand and they will have the same GUID referring to that perennial group stand.
        /// </summary>
        public struct TempComponent
        {
            public int Id;
            public IComponentTemporaryInput referenceComponent;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Takes in a list of components that do not have their GUIDs set. This method iterates through the component list and performs the following algorithm:
        /// 1. If the RowId of the component is = 0 (in the case for our field, a rowId of 0 means there it is not a perennial crop so there is no need to set a GUID for
        /// the perennial group stand id. So just add that component to a new, temporary list and continue to the next component.
        /// 2. Otherwise, we compare the TempComponent objects Id's in our uniqueID list to the current components row ID, if there is a match, than we set the GUID of the TempComponent's
        /// reference component to the current components GUID because they refer to the same type of data. Then we add it to our temp list.
        /// 3. Otherwise, this is the first instance of our rowId, so we will generate a new TempComponent, set the TempComponent's ID to our
        /// current component's ID, generate a new GUID for our current component and set the reference component of the TempComponent to our current component object
        /// We add that new TempComponent to our uniqueID list and then add the current component to our temp list with its GUID set.
        /// Return a list of components that have their GUID set - applies to Shelterbelts and Field Components only.
        /// </summary>
        public List<IComponentTemporaryInput> GenerateComponentGUIDs(List<IComponentTemporaryInput> componentList)
        {
            var GuidComponentList = new List<IComponentTemporaryInput>();
            var uniqueId = new List<TempComponent>();
            foreach (var currentComponent in componentList)
            {
                if (currentComponent.GroupId == 0)
                {
                    GuidComponentList.Add(currentComponent);
                    continue;
                }
                var currentComponentGroupId = currentComponent.GroupId;

                if (uniqueId.Exists(x => x.Id == currentComponentGroupId))
                {
                    var getComponentWithSameGroupIdIndex = uniqueId.FindIndex(x => x.Id == currentComponentGroupId);
                    currentComponent.Guid = uniqueId[getComponentWithSameGroupIdIndex].referenceComponent.Guid;
         
                    GuidComponentList.Add(currentComponent);
                    continue;

                }

                else
                {
                    var tempComponent = new TempComponent();
                    tempComponent.Id = currentComponentGroupId;
                  
                    currentComponent.Guid = Guid.NewGuid();
                    tempComponent.referenceComponent = currentComponent;

                    uniqueId.Add(tempComponent);
                    GuidComponentList.Add(currentComponent);
                }
            }

            return GuidComponentList;
        } 
        #endregion

    }
}


