#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using H.Core.Enumerations;

#endregion

namespace H.Core.Models.LandManagement.Shelterbelt
{
    public class ShelterbeltComponent : ComponentBase
    {
        #region Constructors

        public ShelterbeltComponent()
        {
            this.ComponentNameDisplayString = Properties.Resources.LabelShelterbelt;
            this.ComponentDescriptionString = Properties.Resources.ToolTipShelterbeltComponent;
            this.ComponentCategory = ComponentCategory.LandManagement;
            this.ComponentType = ComponentType.Shelterbelt;
            _rowData = new ObservableCollection<RowData>();
            _trannumData = new ObservableCollection<TrannumData>();
            //this.Name = Properties.Resources.LabelShelterbelt;
            //this.FakeName = this.Name;
            ForceSetYearOfObservation(DateTime.Now.Year);
        }

        #endregion

        #region Fields

        private ObservableCollection<RowData> _rowData;
        private string _fakeName;
        private HardinessZone hardinessZone;
        private int ecoDistrictId;
        private ObservableCollection<TrannumData> _trannumData;

        #endregion

        #region Properties

        public bool StageStateSet { get; set; }

        public ObservableCollection<RowData> RowData
        {
            get { return _rowData; }
            set { this.SetProperty(ref _rowData, value); }
        }

        public string FakeName
        {
            get { return _fakeName; }
            set { this.SetProperty(ref _fakeName, value); }
        }

        public HardinessZone HardinessZone
        {
            get { return hardinessZone; }
            set { this.SetProperty(ref hardinessZone, value); }
        }

        public int EcoDistrictId
        {
            get { return ecoDistrictId; }
            set { this.SetProperty(ref ecoDistrictId, value); }
        }

        public ObservableCollection<TrannumData> TrannumData
        {
            get { return _trannumData; }
            set { this.SetProperty(ref _trannumData, value); }
        }

        /// <summary>
        /// This property is mirrored on all shelterbelt component model/data objects.
        /// </summary>
        public override int YearOfObservation {
            get => base.YearOfObservation;
            set
            {
                if (value == _yearOfObservation)
                    return;
                ForceSetYearOfObservation(value);
                RaisePropertyChanged(nameof(YearOfObservation));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a <see cref="H.Core.Models.LandManagement.Shelterbelt.TrannumData"/> for each year of the <see cref="ShelterbeltComponent"/>'s history.
        ///
        /// <returns>Note that a <see cref="Shelterbelt.TrannumData"/> will be created for each group of trees in the row (in the same year).</returns>
        /// </summary>
        public void BuildTrannums()
        {
            this.TrannumData.Clear();
            var shelterbelt = this;
            foreach (var row in this.RowData)
            foreach (var treeGroup in row.TreeGroupData)
            {
                for (var year = treeGroup.PlantYear; year <= treeGroup.CutYear; year = year + 1)
                {
                    var trannumData = new TrannumData(shelterbelt, row, treeGroup, year);
                    this.TrannumData.Add(trannumData);
                }
            }
        }

        public string GenerateAutonym()
        {
            var genauto = "";
            var woke = false;
            var seen = new Dictionary<string, bool>();
            foreach (var row in this.RowData)
            {
                if (!row.NameIsFromUser)
                {
                    foreach (var treeGroup in row.TreeGroupData)
                    {
                        bool saw;
                        seen.TryGetValue(treeGroup.GetCorrectName(), out saw);
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

                            genauto += treeGroup.GetCorrectName();
                            seen[treeGroup.GetCorrectName()] = true;
                        }
                    }
                }
                else
                {
                    bool saw;
                    seen.TryGetValue(row.GetCorrectName(), out saw);
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

                        genauto += row.GetCorrectName();
                        seen[row.GetCorrectName()] = true;
                    }
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

        public RowData NewRowData()
        {
            var a = new RowData();
            a.ParentShelterbeltComponent = this.Guid;
            a.YearOfObservation = this.YearOfObservation;
            _rowData.Add(a);
            return a;
        }

        public void RemoveRowData(RowData rowData)
        {
            this.RowData.Remove(rowData);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the year of observation and the same property on models attatched to the shelterbelt component to the value.
        /// </summary>
        private void ForceSetYearOfObservation(int value)
        {
            _yearOfObservation = value; //We are called in the property, must use field.

            //for consistent behaviour on the details screen, trannums shouldn't update until the user rebuilds them. Uncommenting this would make them update.
            //foreach (var trannum in TrannumData)
            //    trannum.YearOfObservation = _yearOfObservation;

            if (this.RowData == null)
            {
                return;
            }

            foreach (var row in RowData)
                row.YearOfObservation = _yearOfObservation;
            //hacky, but need to err on the side of time during the last week instead of making Row do this:
            foreach (var row in RowData)
            {
                foreach (var treegroup in row.TreeGroupData)
                    treegroup.YearOfObservation = _yearOfObservation;
            }
            RaisePropertyChanged(nameof(YearOfObservation));
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}