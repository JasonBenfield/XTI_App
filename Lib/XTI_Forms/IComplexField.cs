namespace XTI_Forms
{
    public interface IComplexField
    {
        public string TypeName { get; }
        public FieldModel[] Fields { get; }
    }
}
