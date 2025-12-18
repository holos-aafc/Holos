using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Calculators.Shelterbelt;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Shelterbelt
{
    //Improve Later: Forgot to check that indexes passed in are nonnegative.
    public class CircumferenceData : ModelBase
    {
        #region Fields

        private ShelterbeltCalculator ShelterbeltCalculator = new ShelterbeltCalculator();

        private bool _circumferenceGenerationOverriden;
        private double _userCircumference;

        /// <summary>
        /// Rows represent the difference trees under measurement (trees in viewmodel)
        /// Specific cells in the table represent the trunks making up the tree being measured (trunks in viewmodel)
        /// That is, the trunks at breat heigh, or 30cm for caragana.
        /// </summary>
        private List<List<double>> _table;

        #endregion


        #region Constructors

        public CircumferenceData()
        {
            _table = new List<List<double>>();
            this.InvalidRows = new Stack<int>();
            this.ValidRows = new List<bool>();
            this.InvalidColumns = new List<Stack<int>>();
            this.ValidColumns = new List<List<bool>>();
            this.QuickAccessRows = new List<int>();
            this.QuickAccessColumns = new List<List<int>>();
        }

        public CircumferenceData(CircumferenceData source)
        {
            //no need to copy calculators...

            _circumferenceGenerationOverriden = source._circumferenceGenerationOverriden;
            _userCircumference = source._userCircumference;

            _table = this.DeepCopy(source._table);
            this.ValidRows = this.DeepCopy(source.ValidRows);
            this.QuickAccessRows = this.DeepCopy(source.QuickAccessRows);
            this.InvalidRows = this.DeepCopy(source.InvalidRows);
            this.ValidColumns = this.DeepCopy(source.ValidColumns);
            this.QuickAccessColumns = this.DeepCopy(source.QuickAccessColumns);
            this.InvalidColumns = this.DeepCopy(source.InvalidColumns);
        }

        #endregion


        #region Properties

        #region needed for saving HOLOS' saving functionality to work - do not use

        public List<List<double>> Table
        {
            get { return _table; }
            set { _table = value; }
        }

        public List<bool> ValidRows { get; set; }

        public List<int> QuickAccessRows { get; set; }

        public Stack<int> InvalidRows { get; set; }

        public List<List<bool>> ValidColumns { get; set; }

        public List<List<int>> QuickAccessColumns { get; set; }

        public List<Stack<int>> InvalidColumns { get; set; }

        #endregion

        #region ViewModel Interactions

        public int WeChanged //dummy for telling if get-methods will deliver a different result
        {
            get { return 0; }
        }

        #endregion

        #region Circumference Behaviour

        //This section is not coupled to WeChanged
        public double UserCircumference
        {
            get { return _userCircumference; }
            set { this.SetProperty(ref _userCircumference, value); }
        }

        public double GeneratedCircumference
        {
            get { return this.GetCircumference(); }
        }

        public bool CircumferenceGenerationOverriden
        {
            get { return _circumferenceGenerationOverriden; }
            set
            {
                this.SetProperty(ref _circumferenceGenerationOverriden, value);
                this.RaisePropertyChanged(nameof(this.CircumferenceGenerationNotOverriden));
            }
        }

        public bool CircumferenceGenerationNotOverriden
        {
            get { return !_circumferenceGenerationOverriden; }
            set
            {
                this.SetProperty(ref _circumferenceGenerationOverriden, !value);
                this.RaisePropertyChanged(nameof(this.CircumferenceGenerationOverriden));
            }
        }

        #endregion

        #endregion


        #region Public Methods

        #region ressurecting viewmodel

        public List<int> GetTreeIndices()
        {
            //RebuildQuickAccessInformation();
            return this.QuickAccessRows;
        }

        public List<int> GetTrunkIndices(int tree)
        {
            if (tree < this.ValidRows.Count && this.ValidRows[tree])
            {
                return this.QuickAccessColumns[tree];
            }

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(this.GetTrunkIndices) +
                                               " attempted to access list of valid trunks at invalid tree.");
        }

        #endregion

        //returns the row index
        public int NewTree()
        {
            int location;
            if (this.InvalidRows.Count == 0)
            {
                _table.Add(new List<double>());
                this.InvalidColumns.Add(new Stack<int>());
                this.ValidRows.Add(true);
                this.ValidColumns.Add(new List<bool>());
                location = _table.Count - 1;
            }
            else
            {
                location = this.InvalidRows.Peek();
                this.InvalidRows.Pop();
                _table[location] = new List<double>();
                this.InvalidColumns[location] = new Stack<int>();
                this.ValidRows[location] = true;
                this.ValidColumns[location] = new List<bool>();
            }

            this.RebuildQuickAccessInformation(); //Can specialize to make more efficient here
            this.RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
            return location;
        }

        //takes the row index, returns a column index
        public int NewTrunk(int tree)
        {
            int location;
            if (this.InvalidColumns[tree]
                    .Count == 0)
            {
                _table[tree]
                    .Add(0.0);
                this.ValidColumns[tree]
                    .Add(true);
                location = _table[tree]
                               .Count - 1;
            }
            else
            {
                location = this.InvalidColumns[tree]
                               .Peek();
                this.InvalidColumns[tree]
                    .Pop();
                _table[tree][location] = 0.0;
                this.ValidColumns[tree][location] = true;
            }

            this.RebuildQuickAccessInformation(); //Can specialize to make more efficient here
            this.RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
            return location;
        }

        public void RemoveTree(int tree)
        {
            this.InvalidRows.Push(tree);
            _table[tree] = null;
            this.InvalidColumns[tree] = null;
            this.ValidColumns[tree] = null;
            this.ValidRows[tree] = false;
            this.RebuildQuickAccessInformation();
            this.RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
        }

        public void RemoveTrunk(int tree, int trunk)
        {
            this.InvalidColumns[tree]
                .Push(trunk);
            this.ValidColumns[tree][trunk] = false;
            this.RebuildQuickAccessInformation();
            this.RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
        }

        public double GetTrunkCircumference(int tree, int trunk)
        {
            if (tree < this.ValidRows.Count && this.ValidRows[tree] && trunk < this.ValidColumns[tree]
                                                                                   .Count && this.ValidColumns[tree][trunk])
            {
                return _table[tree][trunk];
            }

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(this.GetTrunkCircumference) +
                                               " was asked for a nonexistent trunk's circumference.");
        }

        public void SetTrunkCircumference(int tree, int trunk, double circumference)
        {
            if (tree < this.ValidRows.Count && this.ValidRows[tree] && trunk < this.ValidColumns[tree]
                                                                                   .Count && this.ValidColumns[tree][trunk])
            {
                _table[tree][trunk] = circumference;
                this.RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
            }
            else
            {
                throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(this.SetTrunkCircumference) +
                                                   " was asked to record a nonexistent trunk's circumference.");
            }
        }

        public double GetTreeCircumference(int tree)
        {
            if (tree < this.ValidRows.Count && this.ValidRows[tree])
            {
                if (this.QuickAccessColumns[tree]
                        .Count == 0)
                {
                    return 0.0; //The room taken up by no trunks
                }

                List<double> circumferences = new List<double>();

                foreach (int index in this.QuickAccessColumns[tree])
                {
                    circumferences.Add(_table[tree][index]);
                }

                return ShelterbeltCalculator.CalculateTreeCircumference(circumferences);
            }

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(this.GetTreeCircumference) +
                                               " was asked to calculate the circumference of a nonexistent tree.");
        }

        public double GetCircumference()
        {
            if (this.QuickAccessRows.Count == 0)
            {
                return 0.0; //room taken up by no trees
            }

            var list = new List<double>();
            foreach (var row in this.QuickAccessRows)
            {
                list.Add(this.GetTreeCircumference(row));
            }

            var meanCircumference = ShelterbeltCalculator.CalculateAverageCircumference(list);
            return meanCircumference;
        }

        public bool SingleTree()
        {
            return _table.Count - this.InvalidRows.Count == 1;
        }

        public bool SingleTrunk(int tree)
        {
            if (tree < this.ValidRows.Count && this.ValidRows[tree])
            {
                return _table[tree]
                           .Count - this.InvalidColumns[tree]
                                        .Count == 1;
            }

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(this.SingleTrunk) +
                                               " was enquired as the whethere there was exactly one trunk on a tree.");
        }

        public int TreeCount()
        {
            return _table.Count - this.InvalidRows.Count;
        }

        public int TrunkCount(int tree)
        {
            if (tree < this.ValidRows.Count && this.ValidRows[tree])
            {
                return _table[tree]
                           .Count - this.InvalidColumns[tree]
                                        .Count;
            }

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(this.TrunkCount) +
                                               " was enquired about the number of trunks on a tree.");
        }

        #endregion


        #region Private Methods

        private void RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride()
        {
            this.RaisePropertyChanged(nameof(this.WeChanged));
            this.RaisePropertyChanged(nameof(this.GeneratedCircumference));

            //This line tacked on as afterthought to cause it to toggle when user changes anything
            //within the circumference data other than the master circumferece/user field
            this.CircumferenceGenerationOverriden = false;
        }

        private void RebuildQuickAccessInformation()
        {
            this.QuickAccessRows.Clear();
            for (var i = 0; i < this.ValidRows.Count; ++i)
            {
                if (this.ValidRows[i])
                {
                    this.QuickAccessRows.Add(i);
                    this.QuickAccessColumns.Add(new List<int>());
                    this.QuickAccessColumns[i]
                        .Clear();
                    for (var j = 0;
                        j < this.ValidColumns[i]
                                .Count;
                        ++j)
                    {
                        this.QuickAccessColumns[i]
                            .Add(j);
                    }
                }
            }
        }

        /// <summary>
        /// For multilevel lists containing primitive types
        /// C# chooses the most specific overload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<List<T>> DeepCopy<T>(List<List<T>> source)
        {
            var result = new List<List<T>>();
            foreach (var a in source)
            {
                if (a == null)
                {
                    result.Add(null);
                }
                else
                {
                    result.Add(new List<T>());
                    foreach (var b in a)
                    {
                        result.Last()
                              .Add(b);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// For multilevel lists containing primitive types
        /// C# chooses the most specific overload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<Stack<T>> DeepCopy<T>(List<Stack<T>> source)
        {
            var result = new List<Stack<T>>();
            var midStack = new Stack<T>();
            foreach (var a in source)
            {
                if (a == null)
                {
                    result.Add(null);
                }
                else
                {
                    result.Add(new Stack<T>());
                    midStack.Clear();
                    foreach (var b in a) //1, 2, 3, 4, 5....
                    {
                        midStack.Push(b);
                    }

                    foreach (var b in midStack) //...5, 4, 3, 2, 1
                    {
                        result.Last()
                              .Push(b);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// For multilevel lists containing primitive types
        /// C# chooses the most specific overload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private Stack<T> DeepCopy<T>(Stack<T> source)
        {
            var result = new Stack<T>();
            var midStack = new Stack<T>();
            foreach (var a in source)
            {
                midStack.Push(a);
            }

            foreach (var a in midStack)
            {
                result.Push(a);
            }

            return result;
        }

        /// <summary>
        /// For multilevel lists containing primitive types
        /// C# chooses the most specific overload
        /// This is the least specific overload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<T> DeepCopy<T>(List<T> source)
        {
            var result = new List<T>();
            foreach (var a in source)
            {
                result.Add(a);
            }

            return result;
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}