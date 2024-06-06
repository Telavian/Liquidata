namespace Liquidata.Common.Extensions;

public static class ValidationExtensions
{
    public static bool IsDefined(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNotDefined(this string? value)
    {
        return !value.IsDefined();
    }

    public static bool IsDefined(this Guid? value)
    {
        return value is not null && value.Value != Guid.Empty;
    }

    public static bool IsNotDefined(this Guid? value)
    {
        return !value.IsDefined();
    }
}
