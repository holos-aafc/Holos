using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Rotation;
using H.Core.Services.LandManagement;
using H.Infrastructure;

namespace H.Core.Services
{
    public class RotationComponentHelper
    {
        #region Fields
        
        private readonly IMapper _cropViewItemMapper;
        private readonly FieldComponentHelper _fieldComponentHelper;
        private readonly FieldResultsService _fieldResultsService;

        #endregion

        #region Constructors
        
        public RotationComponentHelper()
        {
            _fieldComponentHelper = new FieldComponentHelper();

            _fieldResultsService = new FieldResultsService();

            var cropViewItemMappingConfiguration = new MapperConfiguration(configuration =>
            {
                configuration.CreateMap<CropViewItem, CropViewItem>()
                    .ForMember(x => x.IsInitialized, options => options.Ignore())
                    .ForMember(x => x.Guid, options => options.Ignore());
            });

            _cropViewItemMapper = cropViewItemMappingConfiguration.CreateMapper();
        }

        #endregion

        public void CreateRotation(IEnumerable<CropViewItem> cropViewItems, RotationComponent rotationComponent, GlobalSettings globalSettings, Farm farm)
        {
            foreach (var cropViewItem in cropViewItems)
            {
                this.AddCropToRotation(cropViewItem, farm, rotationComponent, globalSettings);
            }
        }

        public void ApplyRotations(Farm farm, RotationComponent rotationComponent)
        {
            var cropsForRotation = rotationComponent.FieldSystemComponent.CropViewItems;
            if (cropsForRotation.Any() == false)
            {
                return;
            }

            /*
             * This is a separate list (collection) containing field system components and is not the same as the list (collection) found on the Farm object. This list contains all the field components
             * that are associated with this rotation component.
             */

            var listOfFieldComponentsFromFarm = new List<FieldSystemComponent>();

            foreach (var mapping in rotationComponent.FieldSystemComponentsMappedByViewItems)
            {
                var component = farm.GetFieldSystemComponent(mapping.FieldSystemComponent.Guid);
                if (component is FieldSystemComponent fieldComponentFromFarm)
                {
                    listOfFieldComponentsFromFarm.Add(fieldComponentFromFarm);
                }
            }

            // Reset view items for each field system component since we need to start fresh.
            foreach (var otherFieldComponent in listOfFieldComponentsFromFarm)
            {
                otherFieldComponent.CropViewItems.Clear();
            }


            // Shift the crops across the field component
            var shiftedViewItems = this.CreateShiftedRotations(cropsForRotation.ToList(), rotationComponent).ToList();
            for (int i = listOfFieldComponentsFromFarm.Count - 1; i >= 0; i--)
            {
                var fieldSystemComponentAtIndex = listOfFieldComponentsFromFarm.ElementAt(i);
                var shiftedViewItemsAtIndex = shiftedViewItems.ElementAt(i);

                fieldSystemComponentAtIndex.CropViewItems.AddRange(shiftedViewItemsAtIndex);
            }

            // Update the mapped field components and view items of the rotation component
            for (int i = listOfFieldComponentsFromFarm.Count - 1; i >= 0; i--)
            {
                var mappedItem = rotationComponent.FieldSystemComponentsMappedByViewItems.ElementAt(i);
                mappedItem.FieldSystemComponent.CropViewItems.Clear();
                var shiftedViewItemsAtIndex = shiftedViewItems.ElementAt(i);
                mappedItem.FieldSystemComponent.CropViewItems.AddRange(shiftedViewItemsAtIndex);
            }

            // Rename fields in sequential order (both the mapped property and the actual field components)
            for (int i = 0; i < rotationComponent.FieldSystemComponentsMappedByViewItems.Count; i++)
            {
                var fieldNumber = i + 1;

                var mapping = rotationComponent.FieldSystemComponentsMappedByViewItems.ElementAt(i);

                // Add in name of crop for more recent year since user must see what is being grown when they are putting animals on field from a rotation
                var cropName = mapping.FieldSystemComponent.CropViewItems.OrderByDescending(x => x.Year).First().CropType.GetDescription();
                var name = rotationComponent.Name + " [Field #" + fieldNumber + "] - " + cropName;
                mapping.FieldSystemComponent.Name = name;
                var fieldComponentFromFarm = farm.GetFieldSystemComponent(mapping.FieldSystemComponent.Guid);
                fieldComponentFromFarm.Name = name;

                fieldComponentFromFarm.GroupPath = rotationComponent.Name;
            }
        }

        public void AddCropToRotation(CropViewItem crop, Farm farm, RotationComponent rotationComponent, GlobalSettings globalSettings)
        {
            crop.Name = rotationComponent.FieldSystemComponent.Name;
            crop.Area = rotationComponent.FieldSystemComponent.FieldArea;

            var coverCrop = new CropViewItem();
            coverCrop.CropType = CropType.None;

            var year = 0;
            if (rotationComponent.FieldSystemComponent.CropViewItems.Any() == false)
            {
                year = rotationComponent.FieldSystemComponent.YearOfObservation;
            }
            else
            {
                year = rotationComponent.FieldSystemComponent.CropViewItems.OrderBy(x => x.Year).First().Year - 1;
            }

            crop.Year = year;
            coverCrop.Year = year;

            if (rotationComponent.FieldSystemComponent.BeginOrderingAtStartYearOfRotation)
            {
                rotationComponent.FieldSystemComponent.CropViewItems.Add(crop);
                rotationComponent.FieldSystemComponent.CoverCrops.Add(coverCrop);
            }
            else
            {
                rotationComponent.FieldSystemComponent.CropViewItems.Insert(0, crop);
                rotationComponent.FieldSystemComponent.CoverCrops.Insert(0, coverCrop);
            }

            var fieldSystemComponent = new FieldSystemComponent();
            fieldSystemComponent.HideComponentInListOfMyComponents = true;
            fieldSystemComponent.IsPartOfRotationComponent = true;
            fieldSystemComponent.BeginOrderingAtStartYearOfRotation = fieldSystemComponent.BeginOrderingAtStartYearOfRotation;

            _fieldComponentHelper.InitializeComponent(fieldSystemComponent, farm);

            var mapping = new MappedFieldComponents()
            {
                CropViewItem = crop,
                FieldSystemComponent = fieldSystemComponent,
            };

            rotationComponent.FieldSystemComponentsMappedByViewItems.Add(mapping);
            farm.Components.Add(fieldSystemComponent);

            // Make final adjustments after base initialization completed
            fieldSystemComponent.StartYear = rotationComponent.FieldSystemComponent.StartYear;
            fieldSystemComponent.EndYear = rotationComponent.FieldSystemComponent.EndYear;
            fieldSystemComponent.FieldArea = rotationComponent.FieldSystemComponent.FieldArea;

            // Assign the same name as that of the field component until a better name can be used
            crop.Name = fieldSystemComponent.Name;
            crop.Area = fieldSystemComponent.FieldArea;


            this.ApplyRotations(farm, rotationComponent);
        }

        public void AssignYears(IEnumerable<CropViewItem> viewItems, FieldSystemComponent fieldSystemComponent)
        {
            // Adjust years of all crops
            var totalViewItems = viewItems.Count();

            if (fieldSystemComponent.BeginOrderingAtStartYearOfRotation == false)
            {
                for (var i = 0; i < totalViewItems; i++)
                {
                    var viewItem = viewItems.ElementAt(totalViewItems - (i + 1));
                    viewItem.Year = fieldSystemComponent.EndYear - i;
                }
            }
            else
            {
                for (int i = 0; i < viewItems.Count(); i++)
                {
                    var viewItem = viewItems.ElementAt(i);
                    viewItem.Year = fieldSystemComponent.StartYear + i;
                }
            }
        }

        /// <summary>
        /// Create a list of lists where each inner list is one specific ordering of crops that are the result of a 'shift' of the
        /// crops in the rotation.
        /// </summary>
        public IEnumerable<IEnumerable<CropViewItem>> CreateShiftedRotations(List<CropViewItem> cropsThatDefineTheRotation, RotationComponent rotationComponent)
        {
            var results = new List<List<CropViewItem>>();
            var shifts = new List<List<CropViewItem>>();

            for (int i = 0; i < cropsThatDefineTheRotation.Count; i++)
            {
                /*
                 * This is how Holos has always shifted the crops. But yield input files might be shifted right which would cause improper yields being set
                 * if the correct shift direction is not set
                 */

                if (rotationComponent.ShiftLeft)
                {
                    shifts.Add(new List<CropViewItem>(cropsThatDefineTheRotation.ShiftLeft(i)));
                }
                else
                {
                    shifts.Add(new List<CropViewItem>(cropsThatDefineTheRotation.ShiftRight(i)));
                }
            }

            // Map all of the properties from the rotation crop to the field crop
            foreach (var shift in shifts)
            {
                var result = new List<CropViewItem>();

                foreach (var shiftedViewItem in shift)
                {
                    var viewItem = new CropViewItem();
                    _cropViewItemMapper.Map(shiftedViewItem, viewItem);
                    result.Add(viewItem);
                }

                this.AssignYears(result, rotationComponent.FieldSystemComponent);

                foreach (var cropViewItem in result)
                {
                    cropViewItem.IsInitialized = true;
                }

                results.Add(result);
            }

            return results;
        }
    }
}