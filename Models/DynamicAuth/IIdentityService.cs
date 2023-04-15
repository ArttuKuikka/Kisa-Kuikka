namespace Kisa_Kuikka.Models.DynamicAuth
{
    public interface IIdentityService
    {
        Task<IEnumerable<UserRoleViewModel>> GetUsersRolesAsync();
    }
}
