using Comman.DTOs.CommanDTOs;

namespace Comman.Functions;

public static class OptionMapper
{
    public static OptionDTO ToOption<T>(T entity)
    {
        var type = typeof(T);

        var idProp = type.GetProperties()
            .FirstOrDefault(p => p.IsDefined(typeof(OptionIdAttribute), false));

        var nameProp = type.GetProperties()
            .FirstOrDefault(p => p.IsDefined(typeof(OptionNameAttribute), false));

        if (idProp == null)
            throw new Exception($"No [OptionId] found on {type.Name}");

        if (nameProp == null)
            throw new Exception($"No [OptionName] found on {type.Name}");

        return new OptionDTO
        {
            Id = Convert.ToInt32(idProp.GetValue(entity)),
            Name = nameProp.GetValue(entity)?.ToString() ?? string.Empty
        };
    }

    public static List<OptionDTO> ToOptionList<T>(IEnumerable<T> items)
        => [.. items.Select(ToOption)];
}

