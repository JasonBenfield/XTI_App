namespace XTI_App.Api;

public sealed record EnumValueItem(string Name, object Value);

public sealed class EnumValueTemplate : ValueTemplate
{
    internal EnumValueTemplate(Type dataType)
    {
        DataType = dataType;
        var values = Enum.GetValues(dataType);
        var enumValues = new List<EnumValueItem>();
        foreach(var value in values)
        {
            var name = Enum.GetName(dataType, value) ?? "";
            enumValues.Add(new EnumValueItem(name, value));
        }
        EnumValues = enumValues.ToArray();
    }

    public Type DataType { get; }

    public EnumValueItem[] EnumValues { get; }

    public IEnumerable<ObjectValueTemplate> ObjectTemplates() => new ObjectValueTemplate[0];
}
