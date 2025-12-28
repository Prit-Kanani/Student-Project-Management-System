using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Comman.DTOs.CommanDTOs;

namespace Comman.Functions;


public static class ReflectionMapper
{
    // cache compiled mappers: key = sourceType.FullName + "->" + destType.FullName
    private static readonly ConcurrentDictionary<string, Func<object, object>> _mapCache =
        new ConcurrentDictionary<string, Func<object, object>>();

    public static TDestination Map<TDestination>(object source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var sourceType = source.GetType();
        var destType = typeof(TDestination);

        // if source is IEnumerable (but not string) and destination is ListResult<T>
        if (IsEnumerable(sourceType) && destType.IsGenericType &&
            destType.GetGenericTypeDefinition() == typeof(ListResult<>))
        {
            return (TDestination)MapListToWrapper(source, sourceType, destType);
        }

        var key = $"{sourceType.FullName}->{destType.FullName}";
        var mapper = _mapCache.GetOrAdd(key, _ => CreateMapper(sourceType, destType));
        return (TDestination)mapper(source);
    }

    private static object MapListToWrapper(object source, Type sourceType, Type wrapperType)
    {
        var itemDestType = wrapperType.GetGenericArguments()[0];
        var itemSourceType = GetListItemType(sourceType);

        // Map each item
        var list = (IEnumerable)source;
        var mappedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemDestType))!;

        // get mapper for item
        var key = $"{itemSourceType.FullName}->{itemDestType.FullName}";
        var itemMapper = _mapCache.GetOrAdd(key, _ => CreateMapper(itemSourceType, itemDestType));

        int count = 0;
        foreach (var item in list)
        {
            mappedList.Add(itemMapper(item));
            count++;
        }

        // build wrapper
        var wrapper = Activator.CreateInstance(wrapperType)!;
        wrapperType.GetProperty("TotalCount")!.SetValue(wrapper, count);
        wrapperType.GetProperty("Items")!.SetValue(wrapper, mappedList);

        return wrapper;
    }

    private static Func<object, object> CreateMapper(Type sourceType, Type destType)
    {
        // Build mapping expression: sourceObj => (object) new Dest { Prop = (converted) ((Source)sourceObj).Prop, ... }
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
                // try to convert srcProp value to destProp type where possible
                Expression srcValue = Expression.Property(typedSource, srcProp);
                Expression value;

                if (destProp.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    value = srcValue;
                }
                else
                {
                    // attempt Convert with System.Convert.ChangeType at runtime if possible
                    MethodInfo changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) })!;
                    var changeTypeCall = Expression.Call(changeTypeMethod, Expression.Convert(srcValue, typeof(object)), Expression.Constant(destProp.PropertyType, typeof(Type)));
                    value = Expression.Convert(changeTypeCall, destProp.PropertyType);
                }

                bindings.Add(Expression.Bind(destProp, value));
            }
        }

        var newDest = Expression.New(destCtor);
        var memberInit = Expression.MemberInit(newDest, bindings);

        var block = Expression.Block(new[] { typedSource }, assignSrc, Expression.Convert(memberInit, typeof(object)));
        var lambda = Expression.Lambda<Func<object, object>>(block, sourceParam);

        return lambda.Compile();
    }

    private static bool IsEnumerable(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }

    private static Type GetListItemType(Type type)
    {
        if (type.IsArray) return type.GetElementType()!;
        if (type.IsGenericType) return type.GetGenericArguments()[0];
        throw new InvalidOperationException("Unsupported list type: " + type.FullName);
    }
}
