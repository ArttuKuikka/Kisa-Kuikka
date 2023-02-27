using Kipa_plus.Models.DynamicAuth.Custom;
using System.Collections.Generic;

namespace Kipa_plus.Models.DynamicAuth
{
    public class RoleAccess
    {
        public int Id { get; set; }

        public string RoleId { get; set; }

        public IEnumerable<MvcControllerInfo> Controllers { get; set; }

        public IEnumerable<RastiControllerModel> RastiAccess { get; set; }
    }
}
