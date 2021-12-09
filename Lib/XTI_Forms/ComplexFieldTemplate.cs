namespace XTI_Forms;

public sealed class ComplexFieldTemplate : IEquatable<ComplexFieldTemplate>, IComplexField
{
    public ComplexFieldTemplate(string typeName, IEnumerable<IField> fields)
    {
        TypeName = typeName;
        Fields = fields.Select(f => f.ToModel()).ToArray();
    }

    public string TypeName { get; }
    public FieldModel[] Fields { get; }

    public override bool Equals(object? obj)
    {
        if (obj is ComplexFieldTemplate templ)
        {
            return Equals(templ);
        }
        return base.Equals(obj);
    }
    public bool Equals(ComplexFieldTemplate? other) => TypeName == other?.TypeName;
    public override int GetHashCode() => TypeName.GetHashCode();
}