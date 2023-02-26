using Kipa_plus.Controllers;
using Kipa_plus.Data;
using Kipa_plus.Models.DynamicAuth.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;

namespace Kipa_plus.Services
{
    public class RastiDiscovery
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly ApplicationDbContext _context;
        public RastiDiscovery(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, ApplicationDbContext context)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _context = context;
        }
        public IEnumerable<RastiControllerModel> GetRastit()
        {
            var Rastit = new List<RastiControllerModel>();

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
                        Nimi = methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? descriptor.ActionName,
                        ActionName = descriptor.ActionName
                    });
            }

            var subcontrollers = new List<SubController>();


            //kaikki tehtävä controller methodit ja nimi
            var TehtController = new SubController();

            TehtController.Nimi = "Tehtävä";
            var tehtactions = new List<Models.DynamicAuth.Custom.Action>();

            var TehtactionDescriptor = TehtäväController.First();
            var TehtcontrollerTypeInfo = TehtactionDescriptor.ControllerTypeInfo;

            foreach (var descriptor in TehtäväController.GroupBy(a => a.ActionName).Select(g => g.First()))
            {
                var methodInfo = descriptor.MethodInfo;
                if (IsProtectedAction(TehtcontrollerTypeInfo, methodInfo))
                    tehtactions.Add(new Models.DynamicAuth.Custom.Action
                    {
                        Nimi = methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? descriptor.ActionName,
                        ActionName = descriptor.ActionName
                    });
            }
            TehtController.Actions = tehtactions;
            subcontrollers.Add(TehtController);

            //kaikki tag controller methodit ja nimi
            var TagiController = new SubController();

            TagiController.Nimi = "Tag";
            var tagactions = new List<Models.DynamicAuth.Custom.Action>();

            var tagactionDescriptor = TagController1.First();
            var tagcontrollerTypeInfo = tagactionDescriptor.ControllerTypeInfo;

            foreach (var descriptor in TagController1.GroupBy(a => a.ActionName).Select(g => g.First()))
            {
                var methodInfo = descriptor.MethodInfo;
                if (IsProtectedAction(tagcontrollerTypeInfo, methodInfo))
                    tagactions.Add(new Models.DynamicAuth.Custom.Action
                    {
                        Nimi = methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? descriptor.ActionName,
                        ActionName = descriptor.ActionName
                    });
            }
            TagiController.Actions = tagactions;
            subcontrollers.Add(TagiController);








            foreach (var rasti in _context.Rasti)
            {
                var RCM = new RastiControllerModel() { RastiNimi = rasti.Nimi, RastiId = (int)rasti.Id };

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
