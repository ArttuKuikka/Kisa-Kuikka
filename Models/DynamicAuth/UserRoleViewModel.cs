using System.ComponentModel.DataAnnotations;

namespace Kipa_plus.Models.DynamicAuth
{
    public class UserRoleViewModel
    {
        [Required]
        public string UserId { get; set; }

        public string UserName { get; set; }

        public IList<string> Roles { get; set; }
    }
}
