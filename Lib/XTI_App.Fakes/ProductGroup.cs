using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class ProductGroup : AppApiGroupWrapper
{
    public ProductGroup(AppApiGroup source) : base(source)
    {
        GetInfo = source.AddAction
        (
            nameof(GetInfo),
            () => new GetInfoAction()
        );
        AddProduct = source.AddAction
        (
            nameof(AddProduct),
            () => new AddProductAction(),
            () => new AddProductValidation()
        );
        Product = source.AddAction
        (
            nameof(Product),
            () => new ProductAction(),
            friendlyName: "Get Product Information"
        );
    }
    public AppApiAction<EmptyRequest, string> GetInfo { get; }
    public AppApiAction<AddProductModel, int> AddProduct { get; }
    public AppApiAction<int, Product> Product { get; }
}