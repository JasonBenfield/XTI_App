using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class ProductGroup : AppApiGroupWrapper
{
    public ProductGroup(AppApiGroup source) : base(source)
    {
        GetInfo = source.AddAction<EmptyRequest, string>()
            .Named(nameof(GetInfo))
            .WithExecution(() => new GetInfoAction())
            .Build();
        AddProduct = source.AddAction<AddProductModel, int>()
            .Named(nameof(AddProduct))
            .WithExecution(() => new AddProductAction())
            .WithValidation(() => new AddProductValidation())
            .Build();
        Product = source.AddAction<int, Product>()
            .Named(nameof(Product))
            .WithExecution(() => new ProductAction())
            .WithFriendlyName("Get Product Information")
            .Build();
    }
    public AppApiAction<EmptyRequest, string> GetInfo { get; }
    public AppApiAction<AddProductModel, int> AddProduct { get; }
    public AppApiAction<int, Product> Product { get; }
}