using Newtonsoft.Json;

namespace Kisa_Kuikka.Models.DynamicAuth.Custom
{
    public class SubController
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string? DisplayName { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
}
