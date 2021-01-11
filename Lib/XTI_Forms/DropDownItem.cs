namespace XTI_Forms
{
    public interface DropDownItem
    {
        public object Value { get; }
        public string DisplayText { get; }
    }
    public sealed class DropDownItem<T> : DropDownItem
    {
        public DropDownItem(T value, string displayText)
        {
            Value = value;
            DisplayText = displayText;
        }

        public T Value { get; }
        public string DisplayText { get; }

        object DropDownItem.Value { get => Value; }
    }
}
