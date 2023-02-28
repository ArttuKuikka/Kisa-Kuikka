namespace Kipa_plus.Models.DynamicAuth
{
    public interface IIdentityService
    {
        Task<IEnumerable<UserRoleViewModel>> GetUsersRolesAsync();
    }
}
