using Microsoft.AspNetCore.Identity;

namespace Kisa_Kuikka.Services
{
    public interface IilmoitusService
    {
        Task<List<string>> GetRoleIdsFromRastiIds(int[]? RastiIds);
        Task<int> SendNotifToRastiIdsAsync(int[]? RastiIds, string? title, string? message, string? refurl, bool? ÄläLähetäValtuudetOmaaville);
        Task<int> SendNotifToRoleIdsAsync(string[]? RoleIds, string? title, string? message, string? refurl);
        Task<bool> SendNotifToUser(IdentityUser user, string? title, string? message, string? refurl);
    }
}
