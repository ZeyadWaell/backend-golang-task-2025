
namespace EasyOrderProduct.Api.Routes
{
    public static class GlobalRoutes
    {
        public const string ApiVersion = "/v1/api";
    }
    public static class ProductRoutes
    {
        public const string Base = GlobalRoutes.ApiVersion + "/products";
        public const string GetAll = Base;
        public const string GetById = Base + "/{id}";
        public const string Create = Base;
        public const string Update = Base + "/{id}";
        public const string CheckInventory = Base + "/{id}/inventory";
    }
}
