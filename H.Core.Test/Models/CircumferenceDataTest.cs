using System.Collections.Generic;
using System.Linq;
using H.Core.Models.LandManagement.Shelterbelt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models
{
    [TestClass]
    public class CircumferenceDataTest
    {
        [TestMethod]
        public void CopyConstructorDoesDeepCopy()
        {
            var a = new CircumferenceData();
            var b = new CircumferenceData(a);

            //Separate
            Assert.AreNotSame(a.Table, b.Table);
            Assert.AreNotSame(a.ValidRows, b.ValidRows);
            Assert.AreNotSame(a.QuickAccessRows, b.QuickAccessRows);
            Assert.AreNotSame(a.InvalidRows, b.InvalidRows);
            Assert.AreNotSame(a.ValidColumns, b.ValidColumns);
            Assert.AreNotSame(a.QuickAccessColumns, b.QuickAccessColumns);
            Assert.AreNotSame(a.InvalidColumns, b.InvalidColumns);
            Assert.AreNotSame(a.UserCircumference, b.UserCircumference);
            Assert.AreNotSame(a.CircumferenceGenerationOverriden, b.CircumferenceGenerationOverriden);

            //Identical
            if (!this.DeepEqual(a.Table, b.Table))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.ValidRows, b.ValidRows))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.QuickAccessRows, b.QuickAccessRows))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.InvalidRows, b.InvalidRows))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.ValidColumns, b.ValidColumns))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.QuickAccessColumns, b.QuickAccessColumns))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.InvalidColumns, b.InvalidColumns))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.UserCircumference, b.UserCircumference))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.CircumferenceGenerationOverriden, b.CircumferenceGenerationOverriden))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CopyConstructorDoesDeepCopyWithNullValues()
        {
            var a = new CircumferenceData();
            var tree1 = a.NewTree();
            var tree2 = a.NewTree();
            var tree3 = a.NewTree();
            var trunk11 = a.NewTrunk(tree1);
            var trunk12 = a.NewTrunk(tree1);
            var trunk21 = a.NewTrunk(tree2);
            a.RemoveTree(tree3);
            var b = new CircumferenceData(a);

            //Separate
            Assert.AreNotSame(a.Table, b.Table);
            Assert.AreNotSame(a.ValidRows, b.ValidRows);
            Assert.AreNotSame(a.QuickAccessRows, b.QuickAccessRows);
            Assert.AreNotSame(a.InvalidRows, b.InvalidRows);
            Assert.AreNotSame(a.ValidColumns, b.ValidColumns);
            Assert.AreNotSame(a.QuickAccessColumns, b.QuickAccessColumns);
            Assert.AreNotSame(a.InvalidColumns, b.InvalidColumns);
            Assert.AreNotSame(a.UserCircumference, b.UserCircumference);
            Assert.AreNotSame(a.CircumferenceGenerationOverriden, b.CircumferenceGenerationOverriden);

            //Identical
            if (!this.DeepEqual(a.Table, b.Table))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.ValidRows, b.ValidRows))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.QuickAccessRows, b.QuickAccessRows))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.InvalidRows, b.InvalidRows))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.ValidColumns, b.ValidColumns))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.QuickAccessColumns, b.QuickAccessColumns))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.InvalidColumns, b.InvalidColumns))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.UserCircumference, b.UserCircumference))
            {
                Assert.Fail();
            }

            if (!this.DeepEqual(a.CircumferenceGenerationOverriden, b.CircumferenceGenerationOverriden))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TableInformationNotNullOnInitialization()
        {
            var a = new CircumferenceData();
            Assert.IsNotNull(a.Table);
            Assert.IsNotNull(a.ValidRows);
            Assert.IsNotNull(a.InvalidRows);
            Assert.IsNotNull(a.InvalidColumns);
            Assert.IsNotNull(a.ValidColumns);
            Assert.IsNotNull(a.QuickAccessRows);
            Assert.IsNotNull(a.QuickAccessColumns);
        }

        [TestMethod]
        public void GetTreeIndicesReturnsCorrectTreeIndice()
        {
            //setup of CircumferenceData
            var data = new CircumferenceData();
            var trees = new List<int>();
            for (var i = 0; i < 10; ++i)
            {
                trees.Add(data.NewTree());
            }

            var trunks = new List<List<int>>();
            for (var indicesIndice = 0;
                indicesIndice < data.GetTreeIndices()
                                    .Count;
                ++indicesIndice)
            {
                var indice = data.GetTreeIndices()[indicesIndice];
                trunks.Add(new List<int>());
                trunks.Last()
                      .Add(indice); //add indice as first column
                for (var i = 0; i < 10; ++i)
                {
                    trunks.Last()
                          .Add(data.NewTrunk(indice));
                    data.SetTrunkCircumference(indice, trunks.Last()
                                                             .Last(), indice - trunks.Last()
                                                                                     .Last());
                }
            }

            //Here comes the test - this is actually a stricter condition (the sequence need not be equal)
            //If this test ever fails writing the actual condition: That all indices are present without repetition,
            //regardless of the order, may be easier than forcing the strict condition.
            var a = data.GetTreeIndices();
            Assert.IsTrue(trees.SequenceEqual(a));
        }

        #region C# lacks easy chained deep equality needed for testing

        //C# only shallowly decides where the innner DeepEqual shall lead - it always leads to this one
        //therefore I have defined double layered things too
        public bool DeepEqual<T>(T a, T b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            return a.Equals(b);
        }


        public bool DeepEqual<T>(List<T> a, List<T> b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Count != b.Count)
            {
                return false;
            }

            for (var i = 0; i < a.Count; ++i)
            {
                if (!this.DeepEqual(a[i], b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DeepEqual<T>(List<List<T>> a, List<List<T>> b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Count != b.Count)
            {
                return false;
            }

            for (var i = 0; i < a.Count; ++i)
            {
                if (!this.DeepEqual(a[i], b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DeepEqual<T>(List<Stack<T>> a, List<Stack<T>> b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Count != b.Count)
            {
                return false;
            }

            for (var i = 0; i < a.Count; ++i)
            {
                if (!this.DeepEqual(a[i], b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DeepEqual<T>(Stack<T> a, Stack<T> b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Count != b.Count)
            {
                return false;
            }

            while (a.Count > 0)
                if (!this.DeepEqual(a.Pop(), b.Pop()))
                {
                    return false;
                }

            return true;
        }

        #endregion

        //[TestMethod]
        //public void GetTrunkIndicesReturnsCorrectTrunkIndices()
        //{
        //    //setup of CircumferenceData
        //    CircumferenceData data = new CircumferenceData();
        //    List<int> trees = new List<int>();
        //    for (int i = 0; i < 10; ++i)
        //        trees.Add(data.NewTree());
        //    List<List<int>> trunks = new List<List<int>>();
        //    for (int indicesIndice = 0; indicesIndice < data.GetTreeIndices().Count; ++indicesIndice)
        //    {
        //        int indice = data.GetTreeIndices()[indicesIndice];
        //        trunks.Add(new List<int>());
        //        trunks.Last().Add(indice); //add indice as first column
        //        for (int i = 0; i < 10; ++i)
        //        {
        //            trunks.Last().Add(data.NewTrunk(indice));
        //            data.SetTrunkCircumference(indice, trunks.Last().Last(), indice - trunks.Last().Last());
        //        }
        //    }
        //    //Here comes the test - this is actually a stricter condition (the sequence need not be equal)
        //    //If this test ever fails writing the actual condition: That all indices are present without repetition,
        //    //regardless of the order, may be easier than forcing the strict condition.

        //}
    }
}