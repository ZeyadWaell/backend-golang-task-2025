using EasyOrder.Domain.Entities;

namespace EasyOrder.Api.Routes
{
    public static class GlobalRoutes
    {
        public const string ApiVersion = "/v1/api";
    }
    public static class OrderRoutes
    {
        public const string Base = GlobalRoutes.ApiVersion + "/orders";
        public const string GetAll = Base;
        public const string GetById = Base + "/{id}";
        public const string Create = Base;
        public const string Update = Base + "/{id}/cancel";
        public const string Delete = Base + "/{id}";
        public const string GetStatus = Base + "/{id}/status";

    }
}
