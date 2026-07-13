using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Core.Mappers
{
    /// <summary>
    /// Convention-based, same-type object copier that reproduces the behaviour Holos relied on from AutoMapper's
    /// same-type <c>CreateMap&lt;T,T&gt;</c> clones, without the AutoMapper dependency (removes CVE-2026-32933).
    ///
    /// Behaviour is defined by characterization against AutoMapper 9 (see the mapper contract tests):
    ///   - a scalar / string / enum property is copied by value;
    ///   - a collection property becomes a <b>new container of the same runtime type holding the same element
    ///     references</b> (a shallow list copy - new list, same items);
    ///   - any other reference-type ("complex") property is assigned by <b>shared reference</b> (not deep-copied);
    ///   - properties named in the per-call ignore set are left untouched. Note <c>Guid</c> is <b>not</b> special-cased
    ///     here (unlike the Holos v5 mapper): call sites that need to preserve identity pass "Guid" in the ignore set,
    ///     exactly as they passed <c>ForMember(x =&gt; x.Guid, o =&gt; o.Ignore())</c> to AutoMapper.
    ///
    /// This copier is non-recursive by construction, so it cannot reintroduce the uncontrolled-recursion DoS that
    /// motivated removing AutoMapper.
    /// </summary>
    public static class PropertyMapper
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Creates a new <typeparamref name="T"/> and copies all non-ignored matching properties from
        /// <paramref name="source"/> onto it.
        /// </summary>
        public static T Clone<T>(T source, params string[] ignoredProperties) where T : new()
        {
            if (source == null)
            {
                return default(T);
            }

            var destination = new T();
            CopyTo(source, destination, ignoredProperties);
            return destination;
        }

        /// <summary>
        /// Copies all non-ignored matching properties from <paramref name="source"/> onto the existing
        /// <paramref name="destination"/> instance.
        /// </summary>
        public static void CopyTo<T>(T source, T destination, params string[] ignoredProperties)
        {
            // Copy the properties of the DECLARED map type T (not the runtime type). AutoMapper's CreateMap<T,T> maps
            // exactly T's properties, so if a base-typed map is applied to derived instances (e.g. AnimalComponentBase),
            // only the base properties are copied - and a subclass-only property is never applied to a base destination.
            CopyProperties(source, destination, typeof(T), ignoredProperties);
        }

        /// <summary>
        /// Copies the writable properties of <paramref name="mapType"/> from <paramref name="source"/> onto
        /// <paramref name="destination"/> (both must be assignable to <paramref name="mapType"/>). This is the engine
        /// behind <see cref="CopyTo{T}"/>; call it directly when the map type is only known at runtime.
        /// </summary>
        public static void CopyProperties(object source, object destination, Type mapType, params string[] ignoredProperties)
        {
            if (source == null || destination == null)
            {
                return;
            }

            HashSet<string> ignore = null;
            if (ignoredProperties != null && ignoredProperties.Length > 0)
            {
                ignore = new HashSet<string>(ignoredProperties);
            }

            foreach (var property in GetCopyableProperties(mapType))
            {
                if (ignore != null && ignore.Contains(property.Name))
                {
                    continue;
                }

                var value = property.GetValue(source);
                if (value == null)
                {
                    // Leave the destination's default in place rather than overwriting with null.
                    continue;
                }

                if (IsCollection(property.PropertyType))
                {
                    // New container, same elements. If it cannot be reconstructed, fall back to sharing the reference.
                    var rewrapped = CopyCollectionShallow(value);
                    property.SetValue(destination, rewrapped ?? value);
                }
                else
                {
                    // Scalars/enums copied by value; complex reference types assigned by shared reference.
                    property.SetValue(destination, value);
                }
            }
        }

        private static PropertyInfo[] GetCopyableProperties(Type type)
        {
            return _propertyCache.GetOrAdd(type, t => t
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                .ToArray());
        }

        private static bool IsCollection(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }

        /// <summary>
        /// Returns a new collection of the same runtime type as <paramref name="source"/>, holding the same element
        /// references (shallow). Covers <see cref="IList"/> and <see cref="IDictionary"/> - i.e. ObservableCollection,
        /// List and Dictionary. Returns null if the collection type has no usable parameterless constructor or is an
        /// unsupported shape, so the caller can fall back to sharing the original reference.
        /// </summary>
        private static object CopyCollectionShallow(object source)
        {
            var type = source.GetType();

            object instance;
            try
            {
                instance = Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }

            if (instance is IList list)
            {
                foreach (var item in (IEnumerable)source)
                {
                    list.Add(item);
                }

                return list;
            }

            if (instance is IDictionary dictionary)
            {
                foreach (DictionaryEntry entry in (IDictionary)source)
                {
                    dictionary.Add(entry.Key, entry.Value);
                }

                return dictionary;
            }

            return null;
        }
    }
}
