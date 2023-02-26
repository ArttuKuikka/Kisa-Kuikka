namespace Kipa_plus.Models.DynamicAuth.Custom
{
    public class RastiControllerModel
    {
        public int RastiId { get; set; }
        public string RastiNimi { get; set; }

        public IEnumerable<Action> Actions { get; set; }
        public IEnumerable<SubController> SubControllers { get; set; }
    }
}
