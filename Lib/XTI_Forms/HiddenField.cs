namespace XTI_Forms
{
    public sealed class HiddenField<T> : SimpleField<T>
    {
        public HiddenField(string prefix, string name, T value)
            : base(prefix, name)
        {
            SetValue(value);
        }

        public new HiddenFieldModel ToModel() => (HiddenFieldModel)base.ToModel();

        protected override SimpleFieldModel _ToModel() => new HiddenFieldModel();
    }
}
