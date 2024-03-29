﻿using Kisa_Kuikka.Data;
using Kisa_Kuikka.Models;
using Kisa_Kuikka.Models.DynamicAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Kisa_Kuikka.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Kisa_Kuikka.TagHelpers
{
    [HtmlTargetElement("secure-content")]
    public class SecureContentTagHelper : TagHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRoleAccessStore _roleAccessStore;
        private readonly DynamicAuthorizationOptions _authorizationOptions;

        public SecureContentTagHelper(ApplicationDbContext dbContext, IRoleAccessStore roleAccessStore, DynamicAuthorizationOptions authorizationOptions)
        {
            _dbContext = dbContext;
            _roleAccessStore = roleAccessStore;
            _authorizationOptions = authorizationOptions;
        }

        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-rastiid")]
        public string RastiId { get; set; }

        [HtmlAttributeName("asp-controllertype")]
        public int ControllerType { get; set; }

        [HtmlAttributeName("asp-controllergroup")]
        public string ControllerGroup { get; set; }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            var user = ViewContext.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                output.SuppressOutput();
                return;
            }

            if (user.Identity.Name.Equals(_authorizationOptions.DefaultAdminUser, StringComparison.CurrentCultureIgnoreCase))
                return;

            var roles = await (
              from usr in _dbContext.Users
              join userRole in _dbContext.UserRoles on usr.Id equals userRole.UserId
              join role in _dbContext.Roles on userRole.RoleId equals role.Id
              where usr.UserName == user.Identity.Name
              select role.Id.ToString()
          ).ToArrayAsync();

            if (RastiId == null)
            {
                var actionId = $"{Area}:{Controller}:{Action}";

                if (await _roleAccessStore.HasAccessToActionAsync(actionId, roles))
                    return;

                output.SuppressOutput();
            }
            else
            {
                int.TryParse(RastiId, out int parsedRastiId);

               

                if (await _roleAccessStore.HasAccessToCustomActionAsync(parsedRastiId, Controller, Action, ControllerType, ControllerGroup, roles))
                    return;

                output.SuppressOutput();
            }
           
        }

        


    }
}
