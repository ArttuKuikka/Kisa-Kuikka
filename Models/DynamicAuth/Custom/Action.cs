using Newtonsoft.Json;

namespace Kisa_Kuikka.Models.DynamicAuth.Custom
{
    public class Action
    {
        public string Name { get; set; }

        [JsonIgnore]
        public string? DisplayName { get; set; }
        
    }
}
