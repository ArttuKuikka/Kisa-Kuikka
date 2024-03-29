﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Kisa_Kuikka.Models.DynamicAuth;
using Kisa_Kuikka.Filters;
using Kisa_Kuikka.Models;
using System.Security.Claims;

namespace Kisa_Kuikka.Controllers
{
    
    [Authorize]
    [DisplayName("Käyttäjän roolien hallinta")]
    [Static]
    public class UserRoleController<TRole, TUser, TKey> : Controller
    where TRole : IdentityRole<TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly UserManager<TUser> _userManager;
        private readonly DynamicAuthorizationOptions _authorizationOptions;

        public UserRoleController(RoleManager<TRole> roleManager, UserManager<TUser> userManager, DynamicAuthorizationOptions authorizationOptions)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _authorizationOptions = authorizationOptions;
        }

        // GET: Access
        [DisplayName("Listaa käyttäjät")]
        public async Task<ActionResult> Index([FromServices] IIdentityService identityService)
        {
            var usersRoles = await identityService.GetUsersRolesAsync();
            foreach(var user in usersRoles)
            {
                var käyttäjä = await _userManager.FindByIdAsync(user.UserId);
                if(käyttäjä != null)
                {
                    var claims = await _userManager.GetClaimsAsync(käyttäjä);
                    user.Nimi = claims.FirstOrDefault(x => x.Type == "KokoNimi")?.Value ?? "[EI NIMEÄ]";
                    if(await _userManager.GetTwoFactorEnabledAsync(käyttäjä))
                    {
                        user.Has2FA = true;
                    }
                }
                else
                {
                    user.Nimi = "[EI NIMEÄ]";
                }

            }

            return View(usersRoles.OrderBy(o => o.Nimi).ToList());
        }

        // GET: Access/Edit
        [DisplayName("Muokkaa käyttäjää")]
        public async Task<ActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var userViewModel = new UserRoleViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Roles = userRoles,
                Nimi = claims.FirstOrDefault(x => x.Type == "KokoNimi")?.Value
            };

            var roles = _roleManager.Roles;
            ViewData["Roles"] = roles;

            return View(userViewModel);
        }

        [DisplayName("Poista käyttäjä")]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var claims = await _userManager.GetClaimsAsync(user);

            var userViewModel = new UserRoleViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Nimi = claims.FirstOrDefault(x => x.Type == "KokoNimi")?.Value
            };

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(UserRoleViewModel viewModel)
        {
            var user = await _userManager.FindByIdAsync(viewModel.UserId);
            if(user.UserName == _authorizationOptions.DefaultAdminUser)
            {
                return Forbid();
            }
            
            if(user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            else
            {
                return BadRequest();
            }

            return RedirectToAction("Index");
        }


        [DisplayName("Poista kaksivaiheinen tunnistus")]
        public async Task<ActionResult> Delete2FA(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var claims = await _userManager.GetClaimsAsync(user);

            var userViewModel = new UserRoleViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Nimi = claims.FirstOrDefault(x => x.Type == "KokoNimi")?.Value
            };

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete2FA(UserRoleViewModel viewModel)
        {
            var user = await _userManager.FindByIdAsync(viewModel.UserId);
            if (user.UserName == _authorizationOptions.DefaultAdminUser)
            {
                return Forbid();
            }
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{viewModel.UserId}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred disabling 2FA.");
            }


            return RedirectToAction("Index");
        }

        // POST: Access/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserRoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Roles"] = _roleManager.Roles;
                return View(viewModel);
            }

            var user = await _userManager.FindByIdAsync(viewModel.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                ViewData["Roles"] = _roleManager.Roles;
                return View();
            }

            if (user.UserName == _authorizationOptions.DefaultAdminUser)
            {
                return Forbid();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (viewModel.Roles != null)
                await _userManager.AddToRolesAsync(user, viewModel.Roles);

            if(viewModel.Nimi != null)
            {
                
                var claims = await _userManager.GetClaimsAsync(user);

                if (claims.Where(x => x.Type == "KokoNimi").Any())
                {
                    await _userManager.ReplaceClaimAsync(user, claims.First(x => x.Type == "KokoNimi"), new System.Security.Claims.Claim("KokoNimi", viewModel.Nimi));
                }
                else
                {
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("KokoNimi", viewModel.Nimi));
                }
            }

            if(viewModel.UusiSalasana!= null)
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.UusiSalasana);
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }
    }
}