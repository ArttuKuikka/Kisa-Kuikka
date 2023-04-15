using Kisa_Kuikka.Models.DynamicAuth.Custom;
using System.Collections.Generic;

namespace Kisa_Kuikka.Models.DynamicAuth
{
    public class RoleAccess
    {
        public int Id { get; set; }

        public string RoleId { get; set; }

        public IEnumerable<MvcControllerInfo> Controllers { get; set; }

        public IEnumerable<MainController> RastiAccess { get; set; }
    }
}
