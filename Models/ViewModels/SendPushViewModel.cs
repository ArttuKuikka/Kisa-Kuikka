using Microsoft.AspNetCore.Identity;

namespace Kisa_Kuikka.Models.ViewModels
{
    public class SendPushViewModel
    {
        public string? message { get; set; }
        public string title { get; set; }
        public string? refUrl { get; set; }
        public List<CheckboxViewModel>? Roles { get; set; }
        public List<CheckboxViewModel>? Rastit { get; set; }
    }
}
