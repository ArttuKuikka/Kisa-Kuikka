using Kipa_plus.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Kipa_plus.Models.DynamicAuth
{
    public class GenericRestControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var roleControllerType = typeof(RoleController<,>).MakeGenericType(DynamicAuthorizationOptions.RoleType,
                DynamicAuthorizationOptions.KeyType).GetTypeInfo();

            var userRoleControllerType = typeof(UserRoleController<,,>).MakeGenericType(
                DynamicAuthorizationOptions.RoleType,
                DynamicAuthorizationOptions.UserType,
                DynamicAuthorizationOptions.KeyType).GetTypeInfo();

            feature.Controllers.Add(roleControllerType);
            feature.Controllers.Add(userRoleControllerType);
        }
    }
}
