namespace XTI_Forms
{
    public class ComplexFieldModel : FieldModel, IComplexField
    {
        public string TypeName { get; set; }
        public FieldModel[] Fields { get; set; }

    }
}
