namespace Kisa_Kuikka.Models.DynamicAuth
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
