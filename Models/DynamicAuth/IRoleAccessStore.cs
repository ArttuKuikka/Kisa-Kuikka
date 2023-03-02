using System.IO;

namespace Kipa_plus.Models.DynamicAuth
{
    public interface IRoleAccessStore
    {
        Task<bool> AddRoleAccessAsync(RoleAccess roleAccess);

        Task<bool> EditRoleAccessAsync(RoleAccess roleAccess);

        Task<bool> RemoveRoleAccessAsync(string roleId);

        Task<RoleAccess> GetRoleAccessAsync(string roleId);

        Task<bool> HasAccessToActionAsync(string actionId, params string[] roles);
        Task<bool> HasAccessToCustomActionAsync(int rastiId, string controller, string action, int controllerType, string controllerGroup, params string[] roles);

        Task<List<int>> HasAccessToRastiIdsAsync(params string[] roles);
    }
}
