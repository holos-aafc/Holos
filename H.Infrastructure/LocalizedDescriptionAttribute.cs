#region Imports

using System;
using System.ComponentModel;
using System.Resources;

#endregion

namespace H.Infrastructure
{
    /// <summary>
    /// Allows for a resource key to be used as the enum description. Using this key to lookup a localized string in the
    /// resource files.
    /// </summary>
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        #region Constructors

        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            if (!string.IsNullOrWhiteSpace(resourceKey))
            {
                _resourceKey = resourceKey;
            }
            else
            {
                throw new ArgumentNullException(nameof(resourceKey));
            }

            if (resourceType != null)
            {
                _resourceType = resourceType;
            }
            else
            {
                throw new ArgumentNullException(nameof(resourceType));
            }

            _resourceManager = new ResourceManager(resourceType);
        }

        #endregion

        #region Public Methods

        public override string Description
        {
            get
            {
                var description = _resourceManager.GetString(_resourceKey);

                return string.IsNullOrWhiteSpace(description) ? $"[[{_resourceKey}]]" : description;
            }
        }

        #endregion

        #region Fields

        private readonly ResourceManager _resourceManager;
        private Type _resourceType;
        private readonly string _resourceKey;

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}