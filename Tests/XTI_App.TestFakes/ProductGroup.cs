using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class ProductGroup : AppApiGroupWrapper
    {
        public ProductGroup(AppApiGroup source) : base(source)
        {
            var actions = new AppApiActionFactory(source);
            GetInfo = source.AddAction
            (
                actions.Action
                (
                    nameof(GetInfo),
                    () => new GetInfoAction()
                )
            );
            AddProduct = source.AddAction
            (
                actions.Action
                (
                    nameof(AddProduct),
                    () => new AddProductValidation(),
                    () => new AddProductAction()
                )
            );
            Product = source.AddAction
            (
                actions.Action
                (
                    nameof(Product),
                    () => new ProductAction(),
                    "Get Product Information"
                )
            );
        }
        public AppApiAction<EmptyRequest, string> GetInfo { get; }
        public AppApiAction<AddProductModel, int> AddProduct { get; }
        public AppApiAction<int, Product> Product { get; }
    }

}
