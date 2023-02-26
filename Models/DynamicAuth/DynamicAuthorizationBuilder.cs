namespace Kipa_plus.Models.DynamicAuth
{
    internal class DynamicAuthorizationBuilder : IDynamicAuthorizationBuilder
    {
        public DynamicAuthorizationBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
