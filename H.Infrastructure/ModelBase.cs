using System;
using Prism.Mvvm;

namespace H.Infrastructure
{
    public abstract class ModelBase : BindableBase
    {
        #region Constructors

        protected ModelBase()
        {
            Guid = Guid.NewGuid();
            YearOfObservation = DateTime.Now.Year;
        }

        #endregion

        #region Fields

        private string _name;
        private bool _nameIsFromUser;
        private bool _isInitialized;
        private string _description;
        private bool _isDirty;

        private DateTime _dateCreated;
        private DateTime _dateModified;

        protected int _yearOfObservation;

        #endregion

        #region Properties

        /// <summary>
        ///     The name of the component used to distinguish between multiples instances of the same type (Field #1, Field #2,
        ///     etc.)
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool NameIsFromUser
        {
            get => _nameIsFromUser;
            set => SetProperty(ref _nameIsFromUser, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public Guid Guid { get; set; }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => SetProperty(ref _dateCreated, value);
        }

        public DateTime DateModified
        {
            get => _dateModified;
            set => SetProperty(ref _dateModified, value);
        }

        /// <summary>
        ///     Indicates if object has been initialized. For instance, a component is initialized when default groups have been
        ///     added
        /// </summary>
        public bool IsInitialized
        {
            get => _isInitialized;
            set => SetProperty(ref _isInitialized, value);
        }

        /// <summary>
        ///     Used for the shelterbelt component. The time period between the <see cref="YearOfObservation" /> and the cut date
        ///     is meant to mean the period of perfect management. The period between the plant date
        ///     and the <see cref="YearOfObservation" /> will not be a period of perfect management.
        /// </summary>
        public virtual int YearOfObservation
        {
            get
            {
                if (_yearOfObservation < 1) //DateTime object is not made for antiquity
                    return 1;
                return _yearOfObservation;
            }
            set => SetProperty(ref _yearOfObservation, value);
        }

        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != GetType()) return false;

            return Equals((ModelBase)obj);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name)) return $"{nameof(Name)}: {GetType() + "(empty name)"}";

            return $"{nameof(Name)}: {Name}";
        }

        public static bool operator ==(ModelBase left, ModelBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModelBase left, ModelBase right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Is called whenever a property on calling class (e.g. NumberOfAnimals), or a property on contained class (e.g.
        ///     ManagementPeriod.HousingDetails, etc.) is modified. This change can be monitored by parent classes
        ///     and final results can be recalculated when needed.
        /// </summary>
        protected void NotifyIsDirty()
        {
            IsDirty = true;

            // Have to raise property change since SetProperty only raises the event if a value has changed. Here we always want to raise the event
            RaisePropertyChanged(nameof(IsDirty));
        }

        protected bool Equals(ModelBase other)
        {
            return Guid.Equals(other.Guid);
        }

        #endregion
    }
}