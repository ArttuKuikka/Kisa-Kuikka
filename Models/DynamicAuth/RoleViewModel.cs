using Kipa_plus.Models.DynamicAuth.Custom;
using System.ComponentModel.DataAnnotations;

namespace Kipa_plus.Models.DynamicAuth
{
    public class RoleViewModel
    {
        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string Name { get; set; }

        public IEnumerable<MvcControllerInfo> SelectedControllers { get; set; }
        public IEnumerable<RastiControllerModel> ValitutRastit { get; set; }
    }
}
