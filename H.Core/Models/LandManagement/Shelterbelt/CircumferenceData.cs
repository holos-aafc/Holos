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

        private readonly ShelterbeltCalculator ShelterbeltCalculator = new ShelterbeltCalculator();

        private bool _circumferenceGenerationOverriden;
        private double _userCircumference;

        #endregion


        #region Constructors

        public CircumferenceData()
        {
            Table = new List<List<double>>();
            InvalidRows = new Stack<int>();
            ValidRows = new List<bool>();
            InvalidColumns = new List<Stack<int>>();
            ValidColumns = new List<List<bool>>();
            QuickAccessRows = new List<int>();
            QuickAccessColumns = new List<List<int>>();
        }

        public CircumferenceData(CircumferenceData source)
        {
            //no need to copy calculators...

            _circumferenceGenerationOverriden = source._circumferenceGenerationOverriden;
            _userCircumference = source._userCircumference;

            Table = DeepCopy(source.Table);
            ValidRows = DeepCopy(source.ValidRows);
            QuickAccessRows = DeepCopy(source.QuickAccessRows);
            InvalidRows = DeepCopy(source.InvalidRows);
            ValidColumns = DeepCopy(source.ValidColumns);
            QuickAccessColumns = DeepCopy(source.QuickAccessColumns);
            InvalidColumns = DeepCopy(source.InvalidColumns);
        }

        #endregion


        #region Properties

        #region needed for saving HOLOS' saving functionality to work - do not use

        /// <summary>
        ///     Rows represent the difference trees under measurement (trees in viewmodel)
        ///     Specific cells in the table represent the trunks making up the tree being measured (trunks in viewmodel)
        ///     That is, the trunks at breat heigh, or 30cm for caragana.
        /// </summary>
        public List<List<double>> Table { get; set; }

        public List<bool> ValidRows { get; set; }

        public List<int> QuickAccessRows { get; set; }

        public Stack<int> InvalidRows { get; set; }

        public List<List<bool>> ValidColumns { get; set; }

        public List<List<int>> QuickAccessColumns { get; set; }

        public List<Stack<int>> InvalidColumns { get; set; }

        #endregion

        #region ViewModel Interactions

        public int WeChanged => 0; //dummy for telling if get-methods will deliver a different result

        #endregion

        #region Circumference Behaviour

        //This section is not coupled to WeChanged
        public double UserCircumference
        {
            get => _userCircumference;
            set => SetProperty(ref _userCircumference, value);
        }

        public double GeneratedCircumference => GetCircumference();

        public bool CircumferenceGenerationOverriden
        {
            get => _circumferenceGenerationOverriden;
            set
            {
                SetProperty(ref _circumferenceGenerationOverriden, value);
                RaisePropertyChanged(nameof(CircumferenceGenerationNotOverriden));
            }
        }

        public bool CircumferenceGenerationNotOverriden
        {
            get => !_circumferenceGenerationOverriden;
            set
            {
                SetProperty(ref _circumferenceGenerationOverriden, !value);
                RaisePropertyChanged(nameof(CircumferenceGenerationOverriden));
            }
        }

        #endregion

        #endregion


        #region Public Methods

        #region ressurecting viewmodel

        public List<int> GetTreeIndices()
        {
            //RebuildQuickAccessInformation();
            return QuickAccessRows;
        }

        public List<int> GetTrunkIndices(int tree)
        {
            if (tree < ValidRows.Count && ValidRows[tree]) return QuickAccessColumns[tree];

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(GetTrunkIndices) +
                                               " attempted to access list of valid trunks at invalid tree.");
        }

        #endregion

        //returns the row index
        public int NewTree()
        {
            int location;
            if (InvalidRows.Count == 0)
            {
                Table.Add(new List<double>());
                InvalidColumns.Add(new Stack<int>());
                ValidRows.Add(true);
                ValidColumns.Add(new List<bool>());
                location = Table.Count - 1;
            }
            else
            {
                location = InvalidRows.Peek();
                InvalidRows.Pop();
                Table[location] = new List<double>();
                InvalidColumns[location] = new Stack<int>();
                ValidRows[location] = true;
                ValidColumns[location] = new List<bool>();
            }

            RebuildQuickAccessInformation(); //Can specialize to make more efficient here
            RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
            return location;
        }

        //takes the row index, returns a column index
        public int NewTrunk(int tree)
        {
            int location;
            if (InvalidColumns[tree]
                    .Count == 0)
            {
                Table[tree]
                    .Add(0.0);
                ValidColumns[tree]
                    .Add(true);
                location = Table[tree]
                    .Count - 1;
            }
            else
            {
                location = InvalidColumns[tree]
                    .Peek();
                InvalidColumns[tree]
                    .Pop();
                Table[tree][location] = 0.0;
                ValidColumns[tree][location] = true;
            }

            RebuildQuickAccessInformation(); //Can specialize to make more efficient here
            RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
            return location;
        }

        public void RemoveTree(int tree)
        {
            InvalidRows.Push(tree);
            Table[tree] = null;
            InvalidColumns[tree] = null;
            ValidColumns[tree] = null;
            ValidRows[tree] = false;
            RebuildQuickAccessInformation();
            RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
        }

        public void RemoveTrunk(int tree, int trunk)
        {
            InvalidColumns[tree]
                .Push(trunk);
            ValidColumns[tree][trunk] = false;
            RebuildQuickAccessInformation();
            RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
        }

        public double GetTrunkCircumference(int tree, int trunk)
        {
            if (tree < ValidRows.Count && ValidRows[tree] && trunk < ValidColumns[tree]
                    .Count && ValidColumns[tree][trunk])
                return Table[tree][trunk];

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(GetTrunkCircumference) +
                                               " was asked for a nonexistent trunk's circumference.");
        }

        public void SetTrunkCircumference(int tree, int trunk, double circumference)
        {
            if (tree < ValidRows.Count && ValidRows[tree] && trunk < ValidColumns[tree]
                    .Count && ValidColumns[tree][trunk])
            {
                Table[tree][trunk] = circumference;
                RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride();
            }
            else
            {
                throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(SetTrunkCircumference) +
                                                   " was asked to record a nonexistent trunk's circumference.");
            }
        }

        public double GetTreeCircumference(int tree)
        {
            if (tree < ValidRows.Count && ValidRows[tree])
            {
                if (QuickAccessColumns[tree]
                        .Count == 0)
                    return 0.0; //The room taken up by no trunks

                var circumferences = new List<double>();

                foreach (var index in QuickAccessColumns[tree]) circumferences.Add(Table[tree][index]);

                return ShelterbeltCalculator.CalculateTreeCircumference(circumferences);
            }

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(GetTreeCircumference) +
                                               " was asked to calculate the circumference of a nonexistent tree.");
        }

        public double GetCircumference()
        {
            if (QuickAccessRows.Count == 0) return 0.0; //room taken up by no trees

            var list = new List<double>();
            foreach (var row in QuickAccessRows) list.Add(GetTreeCircumference(row));

            var meanCircumference = ShelterbeltCalculator.CalculateAverageCircumference(list);
            return meanCircumference;
        }

        public bool SingleTree()
        {
            return Table.Count - InvalidRows.Count == 1;
        }

        public bool SingleTrunk(int tree)
        {
            if (tree < ValidRows.Count && ValidRows[tree])
                return Table[tree]
                    .Count - InvalidColumns[tree]
                    .Count == 1;

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(SingleTrunk) +
                                               " was enquired as the whethere there was exactly one trunk on a tree.");
        }

        public int TreeCount()
        {
            return Table.Count - InvalidRows.Count;
        }

        public int TrunkCount(int tree)
        {
            if (tree < ValidRows.Count && ValidRows[tree])
                return Table[tree]
                    .Count - InvalidColumns[tree]
                    .Count;

            throw new IndexOutOfRangeException(nameof(CircumferenceData) + "." + nameof(TrunkCount) +
                                               " was enquired about the number of trunks on a tree.");
        }

        #endregion


        #region Private Methods

        private void RaiseCircumferenceGenerationChangesAndToggleUserCircumferenceOverride()
        {
            RaisePropertyChanged(nameof(WeChanged));
            RaisePropertyChanged(nameof(GeneratedCircumference));

            //This line tacked on as afterthought to cause it to toggle when user changes anything
            //within the circumference data other than the master circumferece/user field
            CircumferenceGenerationOverriden = false;
        }

        private void RebuildQuickAccessInformation()
        {
            QuickAccessRows.Clear();
            for (var i = 0; i < ValidRows.Count; ++i)
                if (ValidRows[i])
                {
                    QuickAccessRows.Add(i);
                    QuickAccessColumns.Add(new List<int>());
                    QuickAccessColumns[i]
                        .Clear();
                    for (var j = 0;
                         j < ValidColumns[i]
                             .Count;
                         ++j)
                        QuickAccessColumns[i]
                            .Add(j);
                }
        }

        /// <summary>
        ///     For multilevel lists containing primitive types
        ///     C# chooses the most specific overload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<List<T>> DeepCopy<T>(List<List<T>> source)
        {
            var result = new List<List<T>>();
            foreach (var a in source)
                if (a == null)
                {
                    result.Add(null);
                }
                else
                {
                    result.Add(new List<T>());
                    foreach (var b in a)
                        result.Last()
                            .Add(b);
                }

            return result;
        }

        /// <summary>
        ///     For multilevel lists containing primitive types
        ///     C# chooses the most specific overload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<Stack<T>> DeepCopy<T>(List<Stack<T>> source)
        {
            var result = new List<Stack<T>>();
            var midStack = new Stack<T>();
            foreach (var a in source)
                if (a == null)
                {
                    result.Add(null);
                }
                else
                {
                    result.Add(new Stack<T>());
                    midStack.Clear();
                    foreach (var b in a) //1, 2, 3, 4, 5....
                        midStack.Push(b);

                    foreach (var b in midStack) //...5, 4, 3, 2, 1
                        result.Last()
                            .Push(b);
                }

            return result;
        }

        /// <summary>
        ///     For multilevel lists containing primitive types
        ///     C# chooses the most specific overload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private Stack<T> DeepCopy<T>(Stack<T> source)
        {
            var result = new Stack<T>();
            var midStack = new Stack<T>();
            foreach (var a in source) midStack.Push(a);

            foreach (var a in midStack) result.Push(a);

            return result;
        }

        /// <summary>
        ///     For multilevel lists containing primitive types
        ///     C# chooses the most specific overload
        ///     This is the least specific overload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<T> DeepCopy<T>(List<T> source)
        {
            var result = new List<T>();
            foreach (var a in source) result.Add(a);

            return result;
        }

        #endregion
    }
}