namespace Kisa_Kuikka.Models.DynamicAuth
{
    public interface IMvcControllerDiscovery
    {
        IEnumerable<MvcControllerInfo> GetControllers();
    }
}
