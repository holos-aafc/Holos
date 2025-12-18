using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Shelterbelt
{
    /// <summary>
    /// Represents one row of trees in a shelterbelt.
    /// </summary>
    public class RowData : ModelBase
    {
        #region Constructors

        public RowData()
        {
            _treeGroupData = new ObservableCollection<TreeGroupData>();

            //Assign Default Values
            _length = 100;
            this.Name = "Row";
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
            get { return _length; }
            set { this.SetProperty(ref _length, value); }
        }

        public ObservableCollection<TreeGroupData> TreeGroupData
        {
            get { return _treeGroupData; }
            set { this.SetProperty(ref _treeGroupData, value); }
        }

        public Guid ParentShelterbeltComponent
        {
            get { return _parentShelterbeltComponent; }
            set { this.SetProperty(ref _parentShelterbeltComponent, value); }
        }

        #endregion

        #region Public Methods

        public string GenerateAutonym()
        {
            var genauto = "";
            var woke = false;
            var seen = new Dictionary<string, bool>();
            for (var i = 0; i < this.TreeGroupData.Count; ++i)
            {
                bool saw;
                seen.TryGetValue(this.TreeGroupData[i]
                                     .GetCorrectName(), out saw);
                if (!saw)
                {
                    if (woke)
                    {
                        genauto += ", ";
                    }
                    else
                    {
                        woke = true;
                    }

                    genauto += this.TreeGroupData[i]
                                   .GetCorrectName();
                    seen[this.TreeGroupData[i]
                             .GetCorrectName()] = true;
                }
            }

            return genauto;
        }

        public string GetCorrectName()
        {
            if (this.NameIsFromUser)
            {
                return this.Name;
            }

            return this.GenerateAutonym();
        }

        public TreeGroupData NewTreeGroupData()
        {
            var a = new TreeGroupData();
            a.ParentRowData = this.Guid;
            a.GrandParentShelterbeltComponent = this.ParentShelterbeltComponent;
            a.YearOfObservation = this.YearOfObservation;            
            _treeGroupData.Add(a);
            return a;
        }

        public void RemoveTreeGroupData(TreeGroupData treeGroupData)
        {
            this.TreeGroupData.Remove(treeGroupData);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}