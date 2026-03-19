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
        new();

    public static TDestination Map<TDestination>(object source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var sourceType = source.GetType();
        var destType = typeof(TDestination);

        // if source is IEnumerable (but not string) and destination is ListResult<T>
        if (IsEnumerable(sourceType) && destType.IsGenericType &&
            destType.GetGenericTypeDefinition() == typeof(ListResult<>))
        {
            return (TDestination)MapListToWrapper(source, sourceType, destType);
        }

        // if source is IEnumerable (but not string) and destination is List<T>
        if (IsEnumerable(sourceType) && destType.IsGenericType &&
            destType.GetGenericTypeDefinition() == typeof(List<>))
        {
            return (TDestination)MapList(source, sourceType, destType);
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

    private static object MapList(object source, Type sourceType, Type listType)
    {
        var itemDestType = listType.GetGenericArguments()[0];
        var itemSourceType = GetListItemType(sourceType);

        var list = (IEnumerable)source;
        var mappedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemDestType))!;

        var key = $"{itemSourceType.FullName}->{itemDestType.FullName}";
        var itemMapper = _mapCache.GetOrAdd(key, _ => CreateMapper(itemSourceType, itemDestType));

        foreach (var item in list)
        {
            mappedList.Add(itemMapper(item));
        }

        return mappedList;
    }

    private static Func<object, object> CreateMapper(Type sourceType, Type destType)
    {
        // Build mapping expression: sourceObj => (object) new Dest { Prop = (converted) ((Source)sourceObj).Prop, ... }
        var sourceParam = Expression.Parameter(typeof(object), "srcObj");
        var typedSource = Expression.Variable(sourceType, "src");
        var assignSrc = Expression.Assign(typedSource, Expression.Convert(sourceParam, sourceType));

        var bindings = new List<MemberBinding>();
        var destCtor = destType.GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException($"Destination type {destType.FullName} must have a parameterless constructor.");
        var destProperties = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite);

        var sourcePropertyList = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        var sourceProperties = sourcePropertyList
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        foreach (var destProp in destProperties)
        {
            var srcProp = ResolveSourceProperty(destType, destProp, sourceProperties, sourcePropertyList);
            if (srcProp is not null)
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
                    MethodInfo changeTypeMethod = typeof(Convert).GetMethod("ChangeType", [typeof(object), typeof(Type)])!;
                    var changeTypeCall = Expression.Call(changeTypeMethod, Expression.Convert(srcValue, typeof(object)), Expression.Constant(destProp.PropertyType, typeof(Type)));
                    value = Expression.Convert(changeTypeCall, destProp.PropertyType);
                }

                bindings.Add(Expression.Bind(destProp, value));
            }
        }

        var newDest = Expression.New(destCtor);
        var memberInit = Expression.MemberInit(newDest, bindings);

        var block = Expression.Block([typedSource], assignSrc, Expression.Convert(memberInit, typeof(object)));
        var lambda = Expression.Lambda<Func<object, object>>(block, sourceParam);

        return lambda.Compile();
    }

    private static PropertyInfo? ResolveSourceProperty(
        Type destType,
        PropertyInfo destProp,
        Dictionary<string, PropertyInfo> sourceProperties,
        PropertyInfo[] sourcePropertyList)
    {
        if (sourceProperties.TryGetValue(destProp.Name, out var directMatch))
        {
            return directMatch;
        }

        if (destType == typeof(OptionDTO))
        {
            if (destProp.Name.Equals(nameof(OptionDTO.Id), StringComparison.OrdinalIgnoreCase))
            {
                return sourcePropertyList.FirstOrDefault(p => p.IsDefined(typeof(OptionIdAttribute), inherit: true));
            }

            if (destProp.Name.Equals(nameof(OptionDTO.Name), StringComparison.OrdinalIgnoreCase))
            {
                return sourcePropertyList.FirstOrDefault(p => p.IsDefined(typeof(OptionNameAttribute), inherit: true));
            }
        }

        return null;
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
