using Microsoft.AspNetCore.Identity;

namespace Kipa_plus.Models.DynamicAuth
{
    public class ApplicationRole : IdentityRole
    {
        public string Access { get; set; }
    }
}
