﻿#region Imports

using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Properties;
using H.Infrastructure;

#endregion

namespace H.Core.Models.LandManagement.Rotation
{
    public class MappedFieldComponents : ModelBase
    {
        private CropViewItem _cropViewItem;
        private FieldSystemComponent _fieldSystemComponent;

        public CropViewItem CropViewItem
        {
            get => _cropViewItem;
            set => SetProperty(ref _cropViewItem, value);
        }

        public FieldSystemComponent FieldSystemComponent
        {
            get => _fieldSystemComponent;
            set => SetProperty(ref _fieldSystemComponent, value);
        }
    }

    /// <summary>
    ///     This component is a 'container' that will create a number of <see cref="FieldSystemComponent" />s to represent a
    ///     rotation. The <see cref="FieldSystemComponent" />s that
    ///     are created are then added to the Farm and will be processed as if the user had manually created the
    ///     <see cref="FieldSystemComponent" />s individually. This component is
    ///     therefore a 'convenience' component from the perspective of the user.
    /// </summary>
    public class RotationComponent : ComponentBase
    {
        #region Constructors

        public RotationComponent()
        {
            FieldSystemComponent = new FieldSystemComponent();

            ComponentNameDisplayString = Resources.TitleCropRotation;
            ComponentCategory = ComponentCategory.LandManagement;
            ComponentType = ComponentType.Rotation;
            ComponentDescriptionString = Resources.TooltipRotationComponent;

            // By default, crops in the rotation are shifted left over the field components belonging to this rotation
            ShiftLeft = true;

            FieldSystemComponentsMappedByViewItems = new ObservableCollection<MappedFieldComponents>();
        }

        #endregion

        #region Fields

        private TillageType _tillageType;

        private bool _keepRotationOnSingleField;
        private bool _shiftLeft;

        private FieldSystemComponent _fieldSystemComponent;

        private ObservableCollection<MappedFieldComponents> _fieldSystemComponentsMappedByViewItems;

        #endregion

        #region Properties

        /// <summary>
        ///     This is a separate collection containing mapped field system components and is not the same as the list
        ///     (collection) found on the Farm object. This list contains all the field components
        ///     that are associated with this rotation component.
        /// </summary>
        public ObservableCollection<MappedFieldComponents> FieldSystemComponentsMappedByViewItems
        {
            get => _fieldSystemComponentsMappedByViewItems;
            set => SetProperty(ref _fieldSystemComponentsMappedByViewItems, value);
        }

        /// <summary>
        ///     A <see cref="RotationComponent" /> simply holds a collection of <see cref="FieldSystemComponents" />.
        /// </summary>
        public ObservableCollection<FieldSystemComponent> FieldSystemComponents { get; set; } =
            new ObservableCollection<FieldSystemComponent>();

        /// <summary>
        ///     This is a 'container' to hold crop view items (one for each item in the rotation). This is required since the
        ///     rotation component view model inherits from the field system view model which
        ///     needs an existing <see cref="FieldSystemComponent" /> to properly initialize.
        /// </summary>
        public FieldSystemComponent FieldSystemComponent
        {
            get => _fieldSystemComponent;
            set => SetProperty(ref _fieldSystemComponent, value);
        }

        /// <summary>
        ///     Allow the user to specify if corps will be shifted left or right when rotating crops over multiple fields. This
        ///     also allows the user to specify whether or not the
        ///     yield input file assumes a left or right shift of crops year by year.
        /// </summary>
        public bool ShiftLeft
        {
            get => _shiftLeft;
            set => SetProperty(ref _shiftLeft, value);
        }

        public TillageType TillageType
        {
            get => _tillageType;
            set => SetProperty(ref _tillageType, value);
        }

        public bool KeepRotationOnSingleField
        {
            get => _keepRotationOnSingleField;
            set => SetProperty(ref _keepRotationOnSingleField, value);
        }

        #endregion
    }
}