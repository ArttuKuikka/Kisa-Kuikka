namespace Kisa_Kuikka.Models.DynamicAuth.Custom
{
    public class MainController
    {
        public int? RastiId { get; set; }
        public string Name { get; set; }

        public IEnumerable<Action>? Actions { get; set; }
        public IEnumerable<SubController>? SubControllers { get; set; }
    }
}

