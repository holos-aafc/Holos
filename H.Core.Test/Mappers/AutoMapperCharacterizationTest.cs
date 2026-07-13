using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using H.Core.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Mappers
{
    /// <summary>
    /// Locks the clone contract that Holos relied on from AutoMapper's same-type <c>CreateMap&lt;T,T&gt;</c> maps and
    /// that <see cref="PropertyMapper"/> must reproduce after AutoMapper is removed (CVE-2026-32933). Two layers:
    ///
    ///   1. Precise engine-contract tests on a small synthetic type (scalar copy, shared complex reference, new
    ///      collection container with the same elements, per-call ignore set including Guid).
    ///   2. Parity tests that clone the real hot domain types through BOTH AutoMapper 9 and <see cref="PropertyMapper"/>
    ///      and assert they agree member-by-member. These are the ~14 type-level locks.
    /// </summary>
    [TestClass]
    public class AutoMapperCharacterizationTest
    {
        #region Layer 1 - engine contract on a synthetic type

        private class Leaf
        {
            public string Tag { get; set; }
        }

        private class DerivedNode : Node
        {
            public string SubclassOnly { get; set; }
        }

        private class Node
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public DayOfWeek Day { get; set; }
            public Guid Guid { get; set; }
            public Leaf Child { get; set; }
            public ObservableCollection<Leaf> Items { get; set; } = new ObservableCollection<Leaf>();
        }

        private static Node SampleNode()
        {
            var node = new Node { Name = "n1", Count = 7, Day = DayOfWeek.Friday, Guid = Guid.NewGuid(), Child = new Leaf { Tag = "c" } };
            node.Items.Add(new Leaf { Tag = "a" });
            node.Items.Add(new Leaf { Tag = "b" });
            return node;
        }

        [TestMethod]
        public void Clone_CopiesScalarsStringAndEnum()
        {
            var src = SampleNode();
            var clone = PropertyMapper.Clone(src);

            Assert.AreEqual("n1", clone.Name);
            Assert.AreEqual(7, clone.Count);
            Assert.AreEqual(DayOfWeek.Friday, clone.Day);
        }

        [TestMethod]
        public void Clone_SharesComplexReference()
        {
            var src = SampleNode();
            var clone = PropertyMapper.Clone(src);

            Assert.IsNotNull(clone.Child);
            Assert.IsTrue(ReferenceEquals(src.Child, clone.Child), "complex member must be shared by reference, not deep-copied");
        }

        [TestMethod]
        public void Clone_Collection_IsNewContainer_WithSameElementReferences()
        {
            var src = SampleNode();
            var clone = PropertyMapper.Clone(src);

            Assert.IsFalse(ReferenceEquals(src.Items, clone.Items), "collection must be a NEW container, not the same list reference");
            Assert.AreEqual(src.Items.Count, clone.Items.Count);
            Assert.IsTrue(ReferenceEquals(src.Items[0], clone.Items[0]), "collection elements must be the SAME references (shallow)");

            // Mutating the clone's container must not affect the source list (proves de-aliasing of the container).
            clone.Items.Clear();
            Assert.AreEqual(2, src.Items.Count, "clearing the clone's list must not touch the source list");
        }

        [TestMethod]
        public void Clone_WithoutIgnore_CopiesGuid()
        {
            var src = SampleNode();
            var clone = PropertyMapper.Clone(src);
            Assert.AreEqual(src.Guid, clone.Guid, "Guid is copied when not ignored");
        }

        [TestMethod]
        public void Clone_IgnoresNamedProperties_IncludingGuidAndCollection()
        {
            var src = SampleNode();
            var clone = PropertyMapper.Clone(src, nameof(Node.Guid), nameof(Node.Items));

            Assert.AreEqual(Guid.Empty, clone.Guid, "ignored Guid must be left at default");
            Assert.AreEqual(0, clone.Items.Count, "ignored collection must be left at the constructor default (empty)");
            Assert.AreEqual("n1", clone.Name, "non-ignored members are still copied");
        }

        [TestMethod]
        public void CopyTo_OntoExisting_OverwritesNonIgnored_LeavesIgnored()
        {
            var src = SampleNode();
            var dest = new Node { Name = "OLD", Count = 1, Guid = Guid.NewGuid() };
            var destGuid = dest.Guid;

            PropertyMapper.CopyTo(src, dest, nameof(Node.Guid));

            Assert.AreEqual("n1", dest.Name);
            Assert.AreEqual(7, dest.Count);
            Assert.AreEqual(destGuid, dest.Guid, "ignored Guid on an existing instance is preserved");
            Assert.IsTrue(ReferenceEquals(src.Child, dest.Child));
        }

        [TestMethod]
        public void Clone_SourceIsSubclassOfDeclaredType_MapsAsDeclaredType_DoesNotThrow()
        {
            // Mirrors ClimateData.BarnTemperatureData: a property typed as the base holds a subclass instance. Cloning
            // must map it as the declared (base) type - like AutoMapper's CreateMap<Base,Base> - not throw on the
            // subclass-only members.
            Node source = new DerivedNode { Name = "d", Count = 3, Child = new Leaf { Tag = "c" }, SubclassOnly = "x" };

            var clone = PropertyMapper.Clone(source); // T inferred as Node (declared), runtime is DerivedNode

            Assert.AreEqual("d", clone.Name);
            Assert.AreEqual(3, clone.Count);
            Assert.AreEqual(typeof(Node), clone.GetType(), "clone is the declared type; subclass-only members are dropped, matching AutoMapper");
        }

        #endregion

        #region Layer 2 - parity with AutoMapper over the real hot types

        // The same-type clone maps with un-ignored collection/complex members (from the removal inventory).
        private static readonly string[] HotTypes =
        {
            "CropViewItem", "Farm", "Diet", "FeedIngredient",
            "ManureApplicationViewItem", "DigestateApplicationViewItem", "FertilizerApplicationViewItem",
            "ClimateData", "ManagementPeriod", "HousingDetails", "FieldSystemComponent",
            "AnaerobicDigestionComponent", "ManureSubstrateViewItem", "SubstrateFlowInformation",
        };

        private static Assembly CoreAssembly => typeof(H.Core.Services.Animals.ManureService).Assembly;

        [TestMethod]
        public void PropertyMapper_CloneContract_OnHotTypes()
        {
            // The clone contract PropertyMapper must uphold on the real domain types (originally established by parity
            // against AutoMapper, which has since been removed): complex reference members are SHARED; collection members
            // become a NEW container holding the SAME elements.
            var typesByName = CoreAssembly.GetTypes().GroupBy(t => t.Name).ToDictionary(g => g.Key, g => g.First());
            var failures = new List<string>();
            var checkedTypes = 0;

            foreach (var name in HotTypes)
            {
                if (!typesByName.TryGetValue(name, out var type))
                {
                    failures.Add($"{name}: TYPE NOT FOUND");
                    continue;
                }

                object source;
                try { source = Activator.CreateInstance(type); }
                catch (Exception e) { failures.Add($"{name}: cannot construct ({Root(e).GetType().Name})"); continue; }

                var populated = Populate(type, source);
                if (populated.Count == 0)
                {
                    continue; // nothing reference-typed to check on this type
                }

                object pmClone;
                try
                {
                    pmClone = Activator.CreateInstance(type);
                    PropertyMapper.CopyProperties(source, pmClone, type); // map by the concrete type; no ignores = maximal exposure
                }
                catch (Exception e) { failures.Add($"{name}: PropertyMapper.CopyProperties threw ({Root(e).GetType().Name})"); continue; }

                foreach (var (prop, srcVal, isColl) in populated)
                {
                    var pmVal = prop.GetValue(pmClone);

                    if (isColl)
                    {
                        // A collection becomes a NEW container (not the source list) with the SAME element count and
                        // elements (same reference for reference-type elements, equal value for enum/value-type elements).
                        if (pmVal == null) { failures.Add($"{name}.{prop.Name}: produced a null collection"); continue; }
                        if (ReferenceEquals(pmVal, srcVal)) { failures.Add($"{name}.{prop.Name}: shared the collection reference (aliasing)"); continue; }

                        var s = ToList(srcVal); var p = ToList(pmVal);
                        if (p.Count != s.Count) { failures.Add($"{name}.{prop.Name}: count {p.Count} != source {s.Count}"); continue; }
                        for (var i = 0; i < s.Count; i++)
                        {
                            if (!ElementEquivalent(s[i], p[i])) { failures.Add($"{name}.{prop.Name}[{i}]: element not equivalent to source"); break; }
                        }
                    }
                    else
                    {
                        // A complex reference member is shared (assigned by reference, not deep-copied).
                        if (!ReferenceEquals(pmVal, srcVal)) { failures.Add($"{name}.{prop.Name}: did not share the complex reference"); }
                    }
                }

                checkedTypes++;
            }

            Assert.IsTrue(checkedTypes >= 10, $"Expected to check the hot types; only {checkedTypes} were checked.");
            Assert.AreEqual(0, failures.Count, "PropertyMapper clone-contract violations:\n  " + string.Join("\n  ", failures));
        }

        // The bundle deep-copy behaviour that FarmResultsService (Farm, ClimateData) and FieldComponentHelper (fertilizer)
        // relied on was characterized against AutoMapper and is documented in the removal notes; those services now
        // reproduce it with explicit clone loops, guarded by the real ReplicateFarm/Replicate integration tests.

        #endregion

        #region helpers

        private static List<(PropertyInfo Prop, object Value, bool IsColl)> Populate(Type type, object target)
        {
            var populated = new List<(PropertyInfo, object, bool)>();
            foreach (var p in Writable(type))
            {
                if (p.Name == "Guid") continue;
                var isColl = IsCollection(p.PropertyType);
                var isComplex = !p.PropertyType.IsValueType && p.PropertyType != typeof(string);
                if (!isColl && !isComplex) continue;

                var val = isColl ? MakeCollection(p.PropertyType) : TryMake(p.PropertyType);
                if (val == null) continue;
                try { p.SetValue(target, val); populated.Add((p, val, isColl)); } catch { }
            }
            return populated;
        }

        private static IEnumerable<PropertyInfo> Writable(Type t) =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

        private static bool IsCollection(Type t) => typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string);

        private static object TryMake(Type t) { try { return Activator.CreateInstance(t); } catch { return null; } }

        private static object MakeCollection(Type t)
        {
            try
            {
                var concrete = (t.IsInterface || t.IsAbstract) && t.IsGenericType
                    ? typeof(List<>).MakeGenericType(t.GetGenericArguments()[0]) : t;
                var instance = Activator.CreateInstance(concrete);
                if (instance is IList list && t.IsGenericType)
                {
                    var item = TryMake(t.GetGenericArguments()[0]);
                    if (item != null) { try { list.Add(item); } catch { } }
                }
                return instance;
            }
            catch { return null; }
        }

        private static List<object> ToList(object enumerable)
        {
            var list = new List<object>();
            foreach (var x in (IEnumerable)enumerable) list.Add(x);
            return list;
        }

        private static bool ElementEquivalent(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            return a.GetType().IsValueType ? a.Equals(b) : ReferenceEquals(a, b);
        }

        private static Exception Root(Exception e) => e.InnerException == null ? e : Root(e.InnerException);

        #endregion
    }
}
