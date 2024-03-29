﻿using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Kisa_Kuikka.Models.DynamicAuth
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericRestControllerNameConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType &&
                controller.ControllerType.Namespace != "Kisa_Kuikka.Controllers")
                return;

            if (controller.ControllerName.StartsWith("role", StringComparison.CurrentCultureIgnoreCase))
                controller.RouteValues["Controller"] = "Role";
            else if (controller.ControllerName.StartsWith("userrole", StringComparison.CurrentCultureIgnoreCase))
                controller.RouteValues["Controller"] = "UserRole";
        }
    }
}
