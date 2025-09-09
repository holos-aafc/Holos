using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Shelterbelt
{
    /// <summary>
    ///     Represents one row of trees in a shelterbelt.
    /// </summary>
    public class RowData : ModelBase
    {
        #region Constructors

        public RowData()
        {
            _treeGroupData = new ObservableCollection<TreeGroupData>();

            //Assign Default Values
            _length = 100;
            Name = "Row";
        }

        #endregion

        #region Fields

        private ObservableCollection<TreeGroupData> _treeGroupData;

        private double _length;

        private Guid _parentShelterbeltComponent;

        #endregion

        #region Properties

        public double Length
        {
            get => _length;
            set => SetProperty(ref _length, value);
        }

        public ObservableCollection<TreeGroupData> TreeGroupData
        {
            get => _treeGroupData;
            set => SetProperty(ref _treeGroupData, value);
        }

        public Guid ParentShelterbeltComponent
        {
            get => _parentShelterbeltComponent;
            set => SetProperty(ref _parentShelterbeltComponent, value);
        }

        #endregion

        #region Public Methods

        public string GenerateAutonym()
        {
            var genauto = "";
            var woke = false;
            var seen = new Dictionary<string, bool>();
            for (var i = 0; i < TreeGroupData.Count; ++i)
            {
                bool saw;
                seen.TryGetValue(TreeGroupData[i]
                    .GetCorrectName(), out saw);
                if (!saw)
                {
                    if (woke)
                        genauto += ", ";
                    else
                        woke = true;

                    genauto += TreeGroupData[i]
                        .GetCorrectName();
                    seen[TreeGroupData[i]
                        .GetCorrectName()] = true;
                }
            }

            return genauto;
        }

        public string GetCorrectName()
        {
            if (NameIsFromUser) return Name;

            return GenerateAutonym();
        }

        public TreeGroupData NewTreeGroupData()
        {
            var a = new TreeGroupData();
            a.ParentRowData = Guid;
            a.GrandParentShelterbeltComponent = ParentShelterbeltComponent;
            a.YearOfObservation = YearOfObservation;
            _treeGroupData.Add(a);
            return a;
        }

        public void RemoveTreeGroupData(TreeGroupData treeGroupData)
        {
            TreeGroupData.Remove(treeGroupData);
        }

        #endregion
    }
}