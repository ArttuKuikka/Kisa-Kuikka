using Kisa_Kuikka.Models.DynamicAuth.Custom;
using System.ComponentModel.DataAnnotations;

namespace Kisa_Kuikka.Models.DynamicAuth
{
    public class RoleViewModel
    {
        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string Name { get; set; }

        public IEnumerable<MvcControllerInfo>? SelectedControllers { get; set; }
        public IEnumerable<MainController>? ValitutRastit { get; set; }

        public IEnumerable<MvcControllerInfo>? KaikkiControllers { get; set; }
        public IEnumerable<MainController>? KaikkiRastit { get; set; }
    }
}
