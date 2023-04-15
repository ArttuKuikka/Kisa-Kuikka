using Kisa_Kuikka.Models.DynamicAuth;
using Kisa_Kuikka.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Kisa_Kuikka.Models;
using Kisa_Kuikka.Services;
using Kisa_Kuikka.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Kisa_Kuikka.Models.DynamicAuth.Custom;

namespace Kisa_Kuikka.Controllers
{
    [Authorize]
    [Static]
    [DisplayName("Roolien hallinta")]
    public class RoleController<TRole, TKey> : Controller
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly IMvcControllerDiscovery _mvcControllerDiscovery;
        private readonly IRoleAccessStore _roleAccessStore;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly ApplicationDbContext _context;

        public RoleController(
            IMvcControllerDiscovery mvcControllerDiscovery,
            IRoleAccessStore roleAccessStore,
            RoleManager<TRole> roleManager,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            ApplicationDbContext context)
        {
            _mvcControllerDiscovery = mvcControllerDiscovery;
            _roleAccessStore = roleAccessStore;
            _roleManager = roleManager;
            _context = context;
            _actionDescriptorCollectionProvider= actionDescriptorCollectionProvider;
        }

        // GET: Role
        [DisplayName("Listaa roolit")]
        public ActionResult Index()
        {
            var roles = _roleManager.Roles;

            return View(roles);
        }

        [DisplayName("Luo")]
        // GET: Role/Create
        public ActionResult Create()
        {
            var controllers = _mvcControllerDiscovery.GetControllers();
            var rastit = new RastiDiscovery(_actionDescriptorCollectionProvider, _context);
            
            return View(new RoleViewModel() { KaikkiControllers = controllers, KaikkiRastit = rastit.GetRastit()});
        }

        // POST: Role/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.KaikkiControllers = _mvcControllerDiscovery.GetControllers();
                var rastit = new RastiDiscovery(_actionDescriptorCollectionProvider, _context);
                viewModel.KaikkiRastit = rastit.GetRastit();
                return View(viewModel);
            }

            //var role1 = new IdentityRole { Name = viewModel.Name };
            var role = (TRole)Activator.CreateInstance(DynamicAuthorizationOptions.RoleType);
            role.GetType().GetProperty("Name")?.SetValue(role, viewModel.Name, null);

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                viewModel.KaikkiControllers = _mvcControllerDiscovery.GetControllers();
                var rastit = new RastiDiscovery(_actionDescriptorCollectionProvider, _context);
                viewModel.KaikkiRastit = rastit.GetRastit();
                return View(viewModel);
            }

            if (viewModel.SelectedControllers != null && viewModel.SelectedControllers.Any())
            {
                foreach (var controller in viewModel.SelectedControllers)
                    foreach (var action in controller.Actions)
                        action.ControllerId = controller.Id;

               
            }
            var roleAccess = new RoleAccess
            {
                Controllers = viewModel.SelectedControllers?.ToList(),
                RoleId = role.GetType().GetProperty("Id")?.GetValue(role).ToString(),
                RastiAccess = viewModel.ValitutRastit?.ToList()
            };
            await _roleAccessStore.AddRoleAccessAsync(roleAccess);


            return RedirectToAction(nameof(Index));
        }

        // GET: Role/Edit/5
        [DisplayName("Muokkaa")]
        public async Task<ActionResult> Edit(string id)
        {
            

            var rastit = new RastiDiscovery(_actionDescriptorCollectionProvider, _context);
            

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var accessList = await _roleAccessStore.GetRoleAccessAsync(role.Id.ToString());
            var viewModel = new RoleViewModel
            {
                Name = role.Name,
                SelectedControllers = accessList?.Controllers,
                ValitutRastit = accessList?.RastiAccess,
                KaikkiControllers = _mvcControllerDiscovery.GetControllers(),
                KaikkiRastit = rastit.GetRastit(),
        };

            return View(viewModel);
        }

        // POST: Role/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, RoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.KaikkiControllers = _mvcControllerDiscovery.GetControllers();
                var rastit = new RastiDiscovery(_actionDescriptorCollectionProvider, _context);
                viewModel.KaikkiRastit = rastit.GetRastit();

                return View(viewModel);
            }
            else
            {
                // Check role exit
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ModelState.AddModelError("", "Role not found");
                ViewData["Controllers"] = _mvcControllerDiscovery.GetControllers();
                return View();
            }

            // Update role if role's name is changed
            if (role.Name != viewModel.Name)
            {
                role.Name = viewModel.Name;
                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                        viewModel.KaikkiControllers = _mvcControllerDiscovery.GetControllers();
                        var rastit = new RastiDiscovery(_actionDescriptorCollectionProvider, _context);
                        viewModel.KaikkiRastit = rastit.GetRastit();
                        return View(viewModel);
                }
            }

            // Update role access list
            if (viewModel.SelectedControllers != null && viewModel.SelectedControllers.Any())
            {
                foreach (var controller in viewModel.SelectedControllers)
                    foreach (var action in controller.Actions)
                        action.ControllerId = controller.Id;
            }

            var roleAccess = new RoleAccess
            {
                Controllers = viewModel.SelectedControllers?.ToList(),
                RoleId = role.Id.ToString(),
                RastiAccess = viewModel.ValitutRastit?.ToList()
            };
            await _roleAccessStore.EditRoleAccessAsync(roleAccess);
            }

            

            return RedirectToAction(nameof(Index));
        }

        // POST: Role/Delete/5
        [HttpDelete("role/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ModelState.AddModelError("Error", "Role not found");
                return BadRequest(ModelState);
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("Error", error.Description);

                return BadRequest(ModelState);
            }

            await _roleAccessStore.RemoveRoleAccessAsync(role.Id.ToString());

            return Ok(new { });
        }
    }
}