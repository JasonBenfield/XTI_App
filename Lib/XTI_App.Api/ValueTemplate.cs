using System.Reflection;

namespace XTI_App.Api;

public interface ValueTemplate
{
    Type DataType { get; }
    IEnumerable<ObjectValueTemplate> ObjectTemplates();
}

public sealed class DictionaryValueTemplate : ValueTemplate, IEquatable<DictionaryValueTemplate>
{
    internal DictionaryValueTemplate(Type dataType)
    {
        DataType = dataType;
        var genericArgs = DataType.GetGenericArguments();
        KeyTemplate = new ValueTemplateFromType(genericArgs[0]).Template();
        ValueTemplate = new ValueTemplateFromType(genericArgs[1]).Template();
    }

    public Type DataType { get; }
    public ValueTemplate KeyTemplate { get; }
    public ValueTemplate ValueTemplate { get; }

    public IEnumerable<ObjectValueTemplate> ObjectTemplates()
        => KeyTemplate.ObjectTemplates().Union(ValueTemplate.ObjectTemplates()).Distinct();

    public override bool Equals(object? obj)
    {
        if (obj is DictionaryValueTemplate dictTempl)
        {
            return Equals(dictTempl);
        }
        return base.Equals(obj);
    }
    public bool Equals(DictionaryValueTemplate? other) => DataType.Equals(other?.DataType);
    public override int GetHashCode() => DataType.GetHashCode();

}
public sealed class SimpleValueTemplate : ValueTemplate, IEquatable<SimpleValueTemplate>
{
    private readonly string value;
    private readonly int hashCode;

    internal SimpleValueTemplate(Type dataType, bool isNullable)
    {
        DataType = dataType;
        IsNullable = isNullable;
        value = $"{DataType}|{isNullable}";
        hashCode = value.GetHashCode();
    }

    public Type DataType { get; }
    public bool IsNullable { get; }

    public IEnumerable<ObjectValueTemplate> ObjectTemplates() => new ObjectValueTemplate[0];

    public override bool Equals(object? obj) => Equals(obj as SimpleValueTemplate);

    public bool Equals(SimpleValueTemplate? other) => value == other?.value;

    public override int GetHashCode() => hashCode;

    public override string ToString() => $"{nameof(SimpleValueTemplate)} {value}";
}
public sealed class ObjectValueTemplate : ValueTemplate, IEquatable<ObjectValueTemplate>
{
    private readonly string value;
    private readonly int hashCode;

    internal ObjectValueTemplate(Type dataType)
    {
        DataType = dataType;
        PropertyTemplates = dataType.GetProperties().Select(p => new ObjectPropertyTemplate(p));
        var str = string.Join(";", PropertyTemplates);
        value = $"{DataType}|{str}";
        hashCode = value.GetHashCode();
    }

    public Type DataType { get; }
    public IEnumerable<ObjectPropertyTemplate> PropertyTemplates { get; }

    public IEnumerable<ObjectValueTemplate> ObjectTemplates()
    {
        return new ObjectValueTemplate[] { this }
            .Union
            (
                PropertyTemplates.SelectMany(pt => pt.ValueTemplate.ObjectTemplates())
            )
            .Distinct();
    }

    public override bool Equals(object? obj) => Equals(obj as ObjectValueTemplate);

    public bool Equals(ObjectValueTemplate? other) => value == other?.value;

    public override int GetHashCode() => hashCode;
}
public sealed class ObjectPropertyTemplate : IEquatable<ObjectPropertyTemplate>
{
    private readonly string value;
    private readonly int hashCode;

    internal ObjectPropertyTemplate(PropertyInfo propertyInfo)
    {
        Name = propertyInfo.Name;
        ValueTemplate = new ValueTemplateFromType(propertyInfo.PropertyType).Template();
        CanRead = propertyInfo.CanRead;
        CanWrite = propertyInfo.CanWrite;
        value = $"{Name}|{ValueTemplate}|{CanRead}|{CanWrite}";
        hashCode = value.GetHashCode();
    }

    public string Name { get; }
    public ValueTemplate ValueTemplate { get; }
    public bool CanRead { get; }
    public bool CanWrite { get; }

    public override string ToString() => $"{nameof(ObjectPropertyTemplate)} {value}";

    public override bool Equals(object? obj) => Equals(obj as ObjectPropertyTemplate);

    public bool Equals(ObjectPropertyTemplate? other) => value == other?.value;

    public override int GetHashCode() => hashCode;
}
public sealed class ArrayValueTemplate : ValueTemplate, IEquatable<ArrayValueTemplate>
{
    private readonly string value;
    private readonly int hashCode;

    internal ArrayValueTemplate(Type source)
    {
        DataType = source;
        var elType = getEnumerableElementType(source);
        ElementTemplate = new ValueTemplateFromType(elType).Template();
        value = $"{DataType}|{ElementTemplate}";
        hashCode = value.GetHashCode();
    }

    private static Type getEnumerableElementType(Type type)
    {
        Type elementType;
        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
        }
        else
        {
            elementType = type.GetGenericArguments()[0];
        }
        return elementType;
    }

    public Type DataType { get; }
    public ValueTemplate ElementTemplate { get; }

    public IEnumerable<ObjectValueTemplate> ObjectTemplates()
        => ElementTemplate.ObjectTemplates();

    public override bool Equals(object? obj) => Equals(obj as ArrayValueTemplate);

    public bool Equals(ArrayValueTemplate? other) => value == other?.value;

    public override int GetHashCode() => hashCode;

    public override string ToString() => $"{nameof(ArrayValueTemplate)} {value}";

}

public sealed class QueryableValueTemplate : ValueTemplate, IEquatable<QueryableValueTemplate>
{
    private readonly string value;
    private readonly int hashCode;

    internal QueryableValueTemplate(Type source)
    {
        DataType = source;
        ElementTemplate = new ValueTemplateFromType(source.GetGenericArguments()[0]).Template();
        value = $"{DataType}|{ElementTemplate}";
        hashCode = value.GetHashCode();
    }

    public Type DataType { get; }
    public ValueTemplate ElementTemplate { get; }

    public IEnumerable<ObjectValueTemplate> ObjectTemplates()
        => ElementTemplate.ObjectTemplates();

    public override bool Equals(object? obj) => Equals(obj as QueryableValueTemplate);

    public bool Equals(QueryableValueTemplate? other) => value == other?.value;

    public override int GetHashCode() => hashCode;

    public override string ToString() => $"{nameof(QueryableValueTemplate)} {value}";

}

public sealed class QueryOptionsTemplate : ValueTemplate, IEquatable<QueryOptionsTemplate>
{
    private readonly string value;
    private readonly int hashCode;

    internal QueryOptionsTemplate(Type source)
    {
        DataType = source;
        EntityTemplate = new ValueTemplateFromType(source.GetGenericArguments()[0]).Template();
        value = $"{DataType}|{EntityTemplate}";
        hashCode = value.GetHashCode();
    }

    public Type DataType { get; }
    public ValueTemplate EntityTemplate { get; }

    public IEnumerable<ObjectValueTemplate> ObjectTemplates()
        => EntityTemplate.ObjectTemplates();

    public override bool Equals(object? obj) => Equals(obj as QueryOptionsTemplate);

    public bool Equals(QueryOptionsTemplate? other) => value == other?.value;

    public override int GetHashCode() => hashCode;

    public override string ToString() => $"{nameof(QueryOptionsTemplate)} {value}";

}