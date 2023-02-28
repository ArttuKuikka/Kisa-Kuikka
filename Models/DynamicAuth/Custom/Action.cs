using Newtonsoft.Json;

namespace Kipa_plus.Models.DynamicAuth.Custom
{
    public class Action
    {
        public string Name { get; set; }

        [JsonIgnore]
        public string? DisplayName { get; set; }
        
    }
}
