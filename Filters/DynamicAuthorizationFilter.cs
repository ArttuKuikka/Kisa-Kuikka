using Kipa_plus.Data;
using Kipa_plus.Models.DynamicAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Kipa_plus.Models;
using Kipa_plus.Models.DynamicAuth.Custom;

namespace Kipa_plus.Filters
{


    public class DynamicAuthorizationFilter<TDbContext> : DynamicAuthorizationFilter<TDbContext, IdentityUser, IdentityRole, string>
   where TDbContext : IdentityDbContext
    {
        public DynamicAuthorizationFilter(
            DynamicAuthorizationOptions authorizationOptions,
            TDbContext dbContext,
            IRoleAccessStore roleAccessStore,
            ApplicationDbContext context
        ) : base(authorizationOptions, dbContext, context, roleAccessStore)
        {
        }
    }

    public class DynamicAuthorizationFilter<TDbContext, TUser> : DynamicAuthorizationFilter<TDbContext, TUser, IdentityRole, string>
        where TDbContext : IdentityDbContext<TUser>
        where TUser : IdentityUser
    {
        public DynamicAuthorizationFilter(
            DynamicAuthorizationOptions authorizationOptions,
            TDbContext dbContext,
            ApplicationDbContext context,
            IRoleAccessStore roleAccessStore)
            : base(authorizationOptions, dbContext, context, roleAccessStore)
        {
        }
    }

    public class DynamicAuthorizationFilter<TDbContext, TUser, TRole, TKey>
        : DynamicAuthorizationFilter<TDbContext, TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
        where TDbContext : IdentityDbContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public DynamicAuthorizationFilter(
            DynamicAuthorizationOptions authorizationOptions,
            TDbContext dbContext,
            ApplicationDbContext context,
            IRoleAccessStore roleAccessStore)
            : base(authorizationOptions, dbContext, roleAccessStore, context)
        {
        }
    }

    public class DynamicAuthorizationFilter<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IAsyncAuthorizationFilter
        where TDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        private readonly DynamicAuthorizationOptions _authorizationOptions;
        private readonly TDbContext _dbContext;
        private readonly IRoleAccessStore _roleAccessStore;
        private readonly ApplicationDbContext _context;

        public DynamicAuthorizationFilter(
            DynamicAuthorizationOptions authorizationOptions,
            TDbContext dbContext,
            IRoleAccessStore roleAccessStore,
            ApplicationDbContext context
        )
        {
            _authorizationOptions = authorizationOptions;
            _roleAccessStore = roleAccessStore;
            _dbContext = dbContext;
            _context = context;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {



            if (!IsProtectedAction(context))
            {
                //jos tag tilastot jaetaan niin skippaa auth sille sivulle
                if (!IsUserAuthenticated(context))
                {
                    var ctrname = context.RouteData?.Values?["controller"];
                    if (ctrname != null)
                    {
                        if (ctrname.ToString() == "TagTilastot")
                        {
                            var kisa = await _context.Kisa.FindAsync(1); //lisää tähän että hakee kisan oikeasti
                            if (kisa != null)
                            {
                                if (!kisa.JaaTagTilastot)
                                {
                                    context.Result = new ForbidResult();
                                    return;
                                }
                            }
                        }
                    }
                }
               
                return;
            }
                

            if (!IsUserAuthenticated(context))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (IsAllowAllAuthorized(context))
            {
                return;
            }

            var userName = context.HttpContext.User.Identity.Name;
            if (userName.Equals(_authorizationOptions.DefaultAdminUser, StringComparison.CurrentCultureIgnoreCase))
                return;

            var actionId = GetActionId(context);

            var roles = await (
                from user in _dbContext.Users
                join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                join role in _dbContext.Roles on userRole.RoleId equals role.Id
                where user.UserName == userName
                select role.Id.ToString()
            ).ToArrayAsync();



            if (IsCustomController(context))
            {
                var rastiId = context.HttpContext.Request.Query.Where(x => x.Key == "RastiId")?.FirstOrDefault().Value;



                int.TryParse(rastiId, out int parsedRastiId);


                var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;


                var controllertype = GetCustomControllerType(context); //1=MainController 2=Subcontroller 0=null
                var controllerGroup = GetCustomControllerGroup(context);


                if (await _roleAccessStore.HasAccessToCustomActionAsync(parsedRastiId, controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName, controllertype, controllerGroup, roles))
                    return;
            }
            else
            {
                if (await _roleAccessStore.HasAccessToActionAsync(actionId, roles))
                    return;
            }


            context.Result = new ForbidResult();
        }



        private static bool IsProtectedAction(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return false;

            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
                return false;

            var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
            var actionMethodInfo = controllerActionDescriptor.MethodInfo;

            var authorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>();
            if (authorizeAttribute != null)
                return true;

            authorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>();
            if (authorizeAttribute != null)
                return true;

            return false;
        }

        private static bool IsCustomController(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            var maincontroller = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<MainControllerAttribute>();
            if (maincontroller != null) return true;

            var subcontroller = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<SubControllerAttribute>();
            if (subcontroller != null) return true;

            return false;
        }

        private static int GetCustomControllerType(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            var maincontroller = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<MainControllerAttribute>();
            if (maincontroller != null) return 1;

            var subcontroller = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<SubControllerAttribute>();
            if (subcontroller != null) return 2;

            return 0;
        }

        private static string GetCustomControllerGroup(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            var maincontroller = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<MainControllerAttribute>();
            if (maincontroller != null) return maincontroller.Group;

            var subcontroller = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<SubControllerAttribute>();
            if (subcontroller != null) return subcontroller.Group;

            return string.Empty;
        }

        private static bool IsAllowAllAuthorized(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            if (controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AllowAllAuthorizedAttribute>() != null) return true;
            else
            {
                return false;
            }
        }



        private static bool IsUserAuthenticated(AuthorizationFilterContext context)
        {
            return context.HttpContext.User.Identity.IsAuthenticated;
        }

        private static string GetActionId(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var area = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue;
            var controller = controllerActionDescriptor.ControllerName;
            var action = controllerActionDescriptor.ActionName;

            return $"{area}:{controller}:{action}";
        }
    }

}

