using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kipa_plus.Models.DynamicAuth
{
    public class UserRoleViewModel
    {
        [Required]
        public string UserId { get; set; }

        [DisplayName("Sähköposti")]
        public string UserName { get; set; }
        [DisplayName("Roolit")]
        public IList<string>? Roles { get; set; }

        public string? Nimi { get; set; }

        [StringLength(100, ErrorMessage = "Salasanan pitää olla ainakin {2} merkkiä pitkä ja saa olla enintään {1} merkkiä pitkä", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? UusiSalasana { get; set; }

        public bool Has2FA { get; set; }

        public UserRoleViewModel() 
        {
            Has2FA = false;
        }
    }
}
