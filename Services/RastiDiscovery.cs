using Kisa_Kuikka.Controllers;
using Kisa_Kuikka.Data;
using Kisa_Kuikka.Models.DynamicAuth.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;

namespace Kisa_Kuikka.Services
{
    public class RastiDiscovery //TODO: vaihda käyttämään subcontroller attributeja
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly ApplicationDbContext _context;
        public RastiDiscovery(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, ApplicationDbContext context)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _context = context;
        }
        public IEnumerable<MainController> GetRastit()
        {
            var Rastit = new List<MainController>();

            var RastiController1 = _actionDescriptorCollectionProvider
                .ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Select(descriptor => descriptor)
                .Where(descriptor => descriptor.ControllerTypeInfo.FullName.Contains(nameof(RastiController)))
                .ToList();

            var TehtäväController = _actionDescriptorCollectionProvider
                .ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Select(descriptor => descriptor)
                .Where(descriptor => descriptor.ControllerTypeInfo.FullName.Contains(nameof(TehtavaController)))
                .ToList();

            var TagController1 = _actionDescriptorCollectionProvider
                .ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Select(descriptor => descriptor)
                .Where(descriptor => descriptor.ControllerTypeInfo.FullName.Contains(nameof(TagController)))
                .ToList();


            //hanki rasticontroller kaikki methodit
            var actions = new List<Models.DynamicAuth.Custom.Action>();

            var actionDescriptor = RastiController1.First();
            var controllerTypeInfo = actionDescriptor.ControllerTypeInfo;

            foreach (var descriptor in RastiController1.GroupBy(a => a.ActionName).Select(g => g.First()))
            {
                var methodInfo = descriptor.MethodInfo;
                if (IsProtectedAction(controllerTypeInfo, methodInfo))
                    actions.Add(new Models.DynamicAuth.Custom.Action
                    {
                        DisplayName = methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? descriptor.ActionName,
                        Name = descriptor.ActionName
                    });
            }

            var subcontrollers = new List<SubController>();


            //kaikki tehtävä controller methodit ja nimi
            var TehtController = new SubController();

            
            
            var tehtactions = new List<Models.DynamicAuth.Custom.Action>();

            var TehtactionDescriptor = TehtäväController.First();
            var TehtcontrollerTypeInfo = TehtactionDescriptor.ControllerTypeInfo;

            string tehtControllerName;
            if (TehtcontrollerTypeInfo.Name.EndsWith("Controller"))
            {
                tehtControllerName = TehtcontrollerTypeInfo.Name.Replace("Controller", string.Empty);
            }
            else
            {
                tehtControllerName= TehtcontrollerTypeInfo.Name;

            }

            TehtController.Name= tehtControllerName;
            TehtController.DisplayName = TehtcontrollerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            foreach (var descriptor in TehtäväController.GroupBy(a => a.ActionName).Select(g => g.First()))
            {
                var methodInfo = descriptor.MethodInfo;
                if (IsProtectedAction(TehtcontrollerTypeInfo, methodInfo))
                    tehtactions.Add(new Models.DynamicAuth.Custom.Action
                    {
                        DisplayName = methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? descriptor.ActionName,
                        Name = descriptor.ActionName
                    });
            }
            TehtController.Actions = tehtactions;
            subcontrollers.Add(TehtController);

            //kaikki tag controller methodit ja nimi
            var TagiController = new SubController();

            
            var tagactions = new List<Models.DynamicAuth.Custom.Action>();

            var tagactionDescriptor = TagController1.First();
            var tagcontrollerTypeInfo = tagactionDescriptor.ControllerTypeInfo;

            string tagControllerName;
            if (tagcontrollerTypeInfo.Name.EndsWith("Controller"))
            {
                tagControllerName = tagcontrollerTypeInfo.Name.Replace("Controller", string.Empty);
            }
            else
            {
                tagControllerName = tagcontrollerTypeInfo.Name;

            }

            TagiController.Name = tagControllerName;
            TagiController.DisplayName = tagcontrollerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            foreach (var descriptor in TagController1.GroupBy(a => a.ActionName).Select(g => g.First()))
            {
                var methodInfo = descriptor.MethodInfo;
                if (IsProtectedAction(tagcontrollerTypeInfo, methodInfo))
                    tagactions.Add(new Models.DynamicAuth.Custom.Action
                    {
                        DisplayName = methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? descriptor.ActionName,
                        Name = descriptor.ActionName
                    });
            }
            TagiController.Actions = tagactions;
            subcontrollers.Add(TagiController);








            foreach (var rasti in _context.Rasti)
            {
                var RCM = new MainController() { Name = rasti.NumeroJaNimi, RastiId = (int)rasti.Id };

                RCM.Actions = actions;
                RCM.SubControllers = subcontrollers;

                Rastit.Add(RCM);
            }

            return Rastit;
        }

        private static bool IsProtectedAction(MemberInfo controllerTypeInfo, MemberInfo actionMethodInfo)
        {
            if (actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(true) != null)
                return false;

            if (controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>(true) != null)
                return true;

            if (actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>(true) != null)
                return true;

            return false;
        }
    }
}
