namespace Kipa_plus.Models.DynamicAuth.Custom
{
    public class SubController
    {
        public string Name { get; set; }
        public IEnumerable<Action> Actions { get; set; }
    }
}
