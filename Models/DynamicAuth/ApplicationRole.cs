using Microsoft.AspNetCore.Identity;

namespace Kisa_Kuikka.Models.DynamicAuth
{
    public class ApplicationRole : IdentityRole
    {
        public string Access { get; set; }
    }
}
