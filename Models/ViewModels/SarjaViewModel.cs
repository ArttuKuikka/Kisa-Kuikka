using Newtonsoft.Json.Linq;

namespace Kipa_plus.Models.ViewModels
{
    public class SarjaViewModel: Sarja
    {
       public List<Rasti>? Rastit { get; set; }
    }
}
