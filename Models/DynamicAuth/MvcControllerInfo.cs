namespace Kipa_plus.Models.DynamicAuth
{
    public class MvcControllerInfo
    {
        public string Id => $"{AreaName}:{Name}";

        public string Name { get; set; }

        public string? DisplayName { get; set; }

        public string? AreaName { get; set; }

        public IEnumerable<MvcActionInfo> Actions { get; set; }
    }
}
