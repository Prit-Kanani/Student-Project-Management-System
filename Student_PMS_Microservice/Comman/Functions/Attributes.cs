using System;
namespace Comman.Functions;

[AttributeUsage(AttributeTargets.Property)]
public sealed class OptionIdAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class OptionNameAttribute : Attribute
{
}
