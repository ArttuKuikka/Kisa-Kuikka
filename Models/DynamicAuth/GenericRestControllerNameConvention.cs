using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Kipa_plus.Models.DynamicAuth
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericRestControllerNameConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType &&
                controller.ControllerType.Namespace != "Kipa_plus.Controllers")
                return;

            if (controller.ControllerName.StartsWith("role", StringComparison.CurrentCultureIgnoreCase))
                controller.RouteValues["Controller"] = "Role";
            else if (controller.ControllerName.StartsWith("userrole", StringComparison.CurrentCultureIgnoreCase))
                controller.RouteValues["Controller"] = "UserRole";
        }
    }
}
