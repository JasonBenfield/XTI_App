using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class ProductGroup : AppApiGroup
    {
        public ProductGroup(AppApi api, IAppApiUser user)
            : base
            (
                api,
                new NameFromGroupClassName(nameof(ProductGroup)).Value,
                ModifierCategoryName.Default,
                api.Access
                  .WithDenied(FakeAppRoles.Instance.Viewer),
                user,
                (n, a, u) => new AppApiActionCollection(n, a, u)
            )
        {
            var actions = Actions<AppApiActionCollection>();
            GetInfo = actions.Add
            (
                "GetInfo",
                () => new GetInfoAction()
            );
            AddProduct = actions.Add
            (
                nameof(AddProduct),
                () => new AddProductValidation(),
                () => new AddProductAction()
            );
            Product = actions.Add
            (
                "Product",
                () => new ProductAction(),
                "Get Product Information"
            );
        }
        public AppApiAction<EmptyRequest, string> GetInfo { get; }
        public AppApiAction<AddProductModel, int> AddProduct { get; }
        public AppApiAction<int, Product> Product { get; }
    }

}
