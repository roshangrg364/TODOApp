using Azure;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.ViewModel;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.Areas.Account.Controllers
{
    [Area("Account")]
    public class PermissionController : Controller
    {
        private readonly RoleServiceInterface _roleService;

        public PermissionController(RoleServiceInterface roleService)
        {
            _roleService = roleService;
        }

        [Authorize(Policy = "Role-ViewPermission")]
        public async Task<IActionResult> Index(string RoleId)
        {
            try
            {
                var allPermissions = await _roleService.GetALLPermissions(RoleId).ConfigureAwait(true);
                var permissionViewModel = new PermissionViewModel { RoleId = RoleId };
                foreach(var permission in allPermissions.Permissions)
                {
                    var moduleWisePermission = new ModuleWisePermissionViewModel { Module = permission.Module };
                    foreach(var data in permission.PermissionData)
                    {
                        moduleWisePermission.PermissionData.Add(new PermissionValuesViewModel
                        {
                            IsAssigned = data.IsAssigned,
                            Value = data.Value
                        });
                    }
                    permissionViewModel.Permissions.Add(moduleWisePermission);
                }
                return View(permissionViewModel);
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        [Authorize(Policy = "Role-ViewPermission")]
        public async Task<IActionResult> LoadPermissionView(string RoleId)
        {
            try
            {
                var allPermissions = await _roleService.GetALLPermissions(RoleId).ConfigureAwait(true);
                var permissionViewModel = new PermissionViewModel { RoleId = RoleId };
                foreach (var permission in allPermissions.Permissions)
                {
                    var moduleWisePermission = new ModuleWisePermissionViewModel { Module = permission.Module };
                    foreach (var data in permission.PermissionData)
                    {
                        moduleWisePermission.PermissionData.Add(new PermissionValuesViewModel
                        {
                            IsAssigned = data.IsAssigned,
                            Value = data.Value
                        });
                    }
                    permissionViewModel.Permissions.Add(moduleWisePermission);
                }
                return PartialView("~/Areas/Account/Views/Permission/_AssignPermissionView.cshtml", permissionViewModel);
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel { Status = StatusType.error.ToString(), IsSuccess = true, Message = ex.Message });
            }
        }

      
    }
}
