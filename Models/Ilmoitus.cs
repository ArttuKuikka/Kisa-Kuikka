using Microsoft.AspNetCore.Identity;

namespace Kipa_plus.Models
{
    public class Ilmoitus
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Message { get; set; }
        public string? RefUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public IdentityUser User { get; set; }
        public bool Luettu { get; set; }
    }
}
