namespace EasyOrderIdentity.Api.Routes
{
    public static class GlobalRoutes
    {
        public const string ApiVersion = "/v1/api";
    }
    public static class AuthRoutes
    {
        public const string Base = GlobalRoutes.ApiVersion + "/users";
        public const string Login = Base + "/login";
        public const string CreateUser = Base;
        public const string GetUser = Base + "/{id}";
        public const string UpdateUser = Base + "/{id}";
    }

}
