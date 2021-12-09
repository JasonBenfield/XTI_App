namespace XTI_Forms;

public sealed class DropDownField<T> : SimpleField<T>
{
    public DropDownField(string prefix, string name)
        : base(prefix, name)
    {
    }

    public string ItemCaption { get; set; } = "";

    private readonly List<DropDownItem<T>> items = new List<DropDownItem<T>>();
    public DropDownItem<T>[] Items() => items.ToArray();

    public DropDownField<T> AddItems(params DropDownItem<T>[] itemsToAdd)
        => AddItems((IEnumerable<DropDownItem<T>>)itemsToAdd);

    public DropDownField<T> AddItems(IEnumerable<DropDownItem<T>> itemsToAdd)
    {
        if (itemsToAdd != null)
        {
            items.AddRange(itemsToAdd);
        }
        return this;
    }

    public new DropDownFieldModel ToModel() => (DropDownFieldModel)base.ToModel();

    protected override SimpleFieldModel _ToModel() => new DropDownFieldModel
    {
        ItemCaption = ItemCaption,
        Items = Items()
    };
}