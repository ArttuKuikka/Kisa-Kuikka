using Microsoft.AspNetCore.Identity;

namespace Kipa_plus.Services
{
    public interface IilmoitusService
    {
        Task<int> SendNotifToRoleIdsAsync(string[]? RoleIds, string? title, string? message, string? refurl);
        Task<bool> SendNotifToUser(IdentityUser user, string? title, string? message, string? refurl);
    }
}
