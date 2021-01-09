namespace XTI_Forms
{
    public sealed class DropDownFieldModel : SimpleFieldModel
    {
        public string ItemCaption { get; set; }
        public DropDownItem[] Items { get; set; }
    }
}
