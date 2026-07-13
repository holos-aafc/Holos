using System;

namespace H.Core.Mappers
{
    /// <summary>
    /// Thin adapter that replaces an AutoMapper <c>IMapper</c> built from a single same-type
    /// <c>CreateMap&lt;T,T&gt;</c> plus an ignore set (and optional <c>ConstructUsing</c> factory). It holds the ignore
    /// set once (as the AutoMapper configuration did) and forwards to <see cref="PropertyMapper"/>, so existing call
    /// sites keep their <c>Map(source)</c> / <c>Map(source, destination)</c> shape. Behaviour is verified against
    /// AutoMapper by the mapper contract tests.
    /// </summary>
    public class ModelMapper<T> where T : new()
    {
        private readonly string[] _ignoredProperties;
        private readonly Func<T> _factory;

        public ModelMapper(params string[] ignoredProperties)
            : this(null, ignoredProperties)
        {
        }

        /// <summary>
        /// Overload for maps that used <c>ConstructUsing</c> with a non-default constructor (e.g.
        /// <c>new EvapotranspirationData(false)</c>). The factory builds the destination for the <see cref="Map(T)"/>
        /// (clone) overload.
        /// </summary>
        public ModelMapper(Func<T> factory, params string[] ignoredProperties)
        {
            _factory = factory;
            _ignoredProperties = ignoredProperties ?? new string[0];
        }

        /// <summary>Creates a new <typeparamref name="T"/> from <paramref name="source"/> (replaces <c>Map&lt;T&gt;(source)</c>).</summary>
        public T Map(T source)
        {
            if (source == null)
            {
                return default(T);
            }

            var destination = _factory != null ? _factory() : new T();
            PropertyMapper.CopyTo(source, destination, _ignoredProperties);
            return destination;
        }

        /// <summary>
        /// Copies <paramref name="source"/> onto the existing <paramref name="destination"/> and returns it (replaces
        /// AutoMapper's <c>Map(source, destination)</c>, which also returns the destination).
        /// </summary>
        public T Map(T source, T destination)
        {
            PropertyMapper.CopyTo(source, destination, _ignoredProperties);
            return destination;
        }
    }
}
