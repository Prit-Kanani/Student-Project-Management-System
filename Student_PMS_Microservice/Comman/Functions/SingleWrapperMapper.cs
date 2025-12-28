using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Comman.Functions
{
    /// <summary>
    /// Simple wrapper DTO example:
    /// public class OutputDTO<T>
    /// {
    ///     public T Data { get; set; }
    ///     public string Message { get; set; }
    /// }
    /// </summary>
    public static class SingleWrapperMapper
    {
        // cache compiled mappers: key = sourceType.FullName + "->" + destType.FullName
        private static readonly ConcurrentDictionary<string, Func<object, object>> _mapCache =
            new ConcurrentDictionary<string, Func<object, object>>();

        /// <summary>
        /// Map a single source object into TDestination. If TDestination is a generic wrapper like OutputDTO&lt;TInner&gt;,
        /// the mapper will first map source -> TInner and then wrap into OutputDTO&lt;TInner&gt;.
        /// Otherwise it maps source -> TDestination directly.
        /// </summary>
        public static TDestination Map<TDestination>(object source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            Type sourceType = source.GetType();
            Type destType = typeof(TDestination);

            // If destination is generic and has exactly one generic argument, treat it as an outer wrapper: Outer<Inner>
            if (destType.IsGenericType && destType.GetGenericArguments().Length == 1)
            {
                Type innerType = destType.GetGenericArguments()[0];

                // Build or fetch mapper from sourceType -> innerType
                var keyInner = $"{sourceType.FullName}->{innerType.FullName}";
                var innerMapper = _mapCache.GetOrAdd(keyInner, _ => CreateMapper(sourceType, innerType));

                var innerMapped = innerMapper(source);

                // Build outer wrapper from innerMapped
                var outerFactory = CreateOuterFactory(destType, innerType);
                var outerObj = outerFactory(innerMapped);

                return (TDestination)outerObj;
            }

            // Otherwise map source -> destType directly
            var key = $"{sourceType.FullName}->{destType.FullName}";
            var mapper = _mapCache.GetOrAdd(key, _ => CreateMapper(sourceType, destType));
            return (TDestination)mapper(source);
        }

        /// <summary>
        /// Create a factory that constructs an instance of outerType from an inner object (of innerType).
        /// Order: preferred props (Data/Item/Value/Payload), any writable assignable property, ctor, direct assignable, fallback.
        /// </summary>
        private static Func<object, object> CreateOuterFactory(Type outerType, Type innerType)
        {
            // Prefer specific common property names first
            var preferredNames = new[] { "Data", "Item", "Value", "Payload" };
            var preferred = outerType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .FirstOrDefault(p => preferredNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase)
                                     && p.PropertyType.IsAssignableFrom(innerType));
            if (preferred != null)
            {
                return (mappedInner) =>
                {
                    var outer = Activator.CreateInstance(outerType)!;
                    preferred.SetValue(outer, mappedInner);
                    return outer;
                };
            }

            // Any writable property assignable from innerType
            var writable = outerType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .FirstOrDefault(p => p.PropertyType.IsAssignableFrom(innerType));
            if (writable != null)
            {
                return (mappedInner) =>
                {
                    var outer = Activator.CreateInstance(outerType)!;
                    writable.SetValue(outer, mappedInner);
                    return outer;
                };
            }

            // Constructor that takes innerType (or parameter assignable from innerType)
            var ctors = outerType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var ctor = ctors.FirstOrDefault(c =>
            {
                var pars = c.GetParameters();
                return pars.Length == 1 && pars[0].ParameterType.IsAssignableFrom(innerType);
            });
            if (ctor != null)
            {
                return (mappedInner) => ctor.Invoke(new[] { mappedInner });
            }

            // If outerType is assignable from innerType, return inner directly (cast)
            if (outerType.IsAssignableFrom(innerType))
            {
                return (mappedInner) => mappedInner!;
            }

            // Last resort: attempt to set first writable property using Convert.ChangeType if needed
            var anyWritable = outerType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.CanWrite);
            if (anyWritable != null)
            {
                return (mappedInner) =>
                {
                    var outer = Activator.CreateInstance(outerType)!;
                    try
                    {
                        var propType = anyWritable.PropertyType;
                        if (propType.IsInstanceOfType(mappedInner))
                        {
                            anyWritable.SetValue(outer, mappedInner);
                        }
                        else
                        {
                            var changed = Convert.ChangeType(mappedInner, propType);
                            anyWritable.SetValue(outer, changed);
                        }
                    }
                    catch
                    {
                        // swallow and return instance even if property unset
                    }
                    return outer;
                };
            }

            // fallback: return empty instance
            return (mappedInner) => Activator.CreateInstance(outerType)!;
        }

        /// <summary>
        /// Creates a compiled mapper from sourceType -> destType using expression trees.
        /// This maps public readable source properties to writable destination properties by name (case-insensitive).
        /// If property types differ, tries Convert.ChangeType at runtime.
        /// destType must have a parameterless constructor.
        /// </summary>
        private static Func<object, object> CreateMapper(Type sourceType, Type destType)
        {
            var sourceParam = Expression.Parameter(typeof(object), "srcObj");
            var typedSource = Expression.Variable(sourceType, "src");
            var assignSrc = Expression.Assign(typedSource, Expression.Convert(sourceParam, sourceType));

            var bindings = new List<MemberBinding>();
            var destCtor = destType.GetConstructor(Type.EmptyTypes);
            if (destCtor == null)
                throw new InvalidOperationException($"Destination type {destType.FullName} must have a parameterless constructor.");

            var destProperties = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            foreach (var destProp in destProperties)
            {
                if (sourceProperties.TryGetValue(destProp.Name, out var srcProp))
                {
                    Expression srcValue = Expression.Property(typedSource, srcProp);
                    Expression valueExpr;

                    if (destProp.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                    {
                        valueExpr = srcValue;
                    }
                    else
                    {
                        // Convert.ChangeType(srcValue, destProp.PropertyType)
                        MethodInfo changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) })!;
                        var changeTypeCall = Expression.Call(changeTypeMethod, Expression.Convert(srcValue, typeof(object)), Expression.Constant(destProp.PropertyType, typeof(Type)));
                        valueExpr = Expression.Convert(changeTypeCall, destProp.PropertyType);
                    }

                    bindings.Add(Expression.Bind(destProp, valueExpr));
                }
            }

            var newDest = Expression.New(destCtor);
            var memberInit = Expression.MemberInit(newDest, bindings);
            var block = Expression.Block(new[] { typedSource }, assignSrc, Expression.Convert(memberInit, typeof(object)));
            var lambda = Expression.Lambda<Func<object, object>>(block, sourceParam);
            return lambda.Compile();
        }
    }
}

/*// Example 1: mapping UserUpdateDto -> OutputDTO<UserDto>
var result = SingleWrapperMapper.Map<OutputDTO<UserDto>>(userUpdateDto);

// Example 2: mapping SomeSource -> PlainDestination (no wrapper)
var plain = SingleWrapperMapper.Map<SomeDestination>(someSource);*/
