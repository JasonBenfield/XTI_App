using System.ComponentModel;
using System.Globalization;

namespace XTI_App.Abstractions;

public sealed class AppKeyTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var str = value as string;
        return str != null
            ? AppKey.Parse(str)
            : base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(AppKey);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        var appKey = (value as AppKey) ?? AppKey.Unknown;
        return destinationType == typeof(string)
            ? appKey.Serialize()
            : base.ConvertTo(context, culture, value, destinationType);
    }
}