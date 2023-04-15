using Newtonsoft.Json.Linq;

namespace Kisa_Kuikka.Models.ViewModels
{
    public class SarjaViewModel: Sarja
    {
       public List<Rasti>? Rastit { get; set; }
    }
}
