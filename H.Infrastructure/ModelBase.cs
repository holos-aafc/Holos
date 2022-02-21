using System;
using System.ComponentModel;
using Prism.Mvvm;

namespace H.Infrastructure
{
    public abstract class ModelBase : BindableBase
    {
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

        #region Constructors

        protected ModelBase()
        {
            this.Guid = Guid.NewGuid();
            this.YearOfObservation = DateTime.Now.Year;
        } 

        #endregion

        #region Properties

        /// <summary>
        /// The name of the component used to distinguish between multiples instances of the same type (Field #1, Field #2, etc.)
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { this.SetProperty(ref _name, value); }
        }

        public bool NameIsFromUser
        {
            get { return _nameIsFromUser; }
            set { this.SetProperty(ref _nameIsFromUser, value); }
        }

        public string Description
        {
            get { return _description; }
            set { this.SetProperty(ref _description, value); }
        }

        public Guid Guid { get; set; }

        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { this.SetProperty(ref _dateCreated, value); }
        }

        public DateTime DateModified
        {
            get { return _dateModified; }
            set { this.SetProperty(ref _dateModified, value); }
        }

        /// <summary>
        /// Indicates if object has been initialized. For instance, a component is initialized when default groups have been added
        /// </summary>
        public bool IsInitialized
        {
            get { return _isInitialized; }
            set { this.SetProperty(ref _isInitialized, value); }
        }

        /// <summary>
        /// Used for the shelterbelt component. The time period between the <see cref="YearOfObservation"/> and the cut date is meant to mean the period of perfect management. The period between the plant date
        /// and the <see cref="YearOfObservation"/> will not be a period of perfect management.
        /// </summary>
        public virtual int YearOfObservation
        {
            get
            {
                if (_yearOfObservation < 1) //DateTime object is not made for antiquity
                    return 1;
                return _yearOfObservation;
            }
            set
            {
                this.SetProperty(ref _yearOfObservation, value);
            }
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
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((ModelBase) obj);
        }

        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                return $"{nameof(Name)}: {this.GetType().ToString() + "(empty name)"}";
            }
            else
            {
                return $"{nameof(Name)}: {Name}";
            }
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
        /// Is called whenever a property on calling class (e.g. NumberOfAnimals), or a property on contained class (e.g. ManagementPeriod.HousingDetails, etc.) is modified. This change can be monitored by parent classes
        /// and final results can be recalculated when needed.
        /// </summary>
        protected void NotifyIsDirty()
        {
            this.IsDirty = true;

            // Have to raise property change since SetProperty only raises the event if a value has changed. Here we always want to raise the event
            base.RaisePropertyChanged(nameof(this.IsDirty));
        }

        protected bool Equals(ModelBase other)
        {
            return this.Guid.Equals(other.Guid);
        }

        #endregion
    }
}