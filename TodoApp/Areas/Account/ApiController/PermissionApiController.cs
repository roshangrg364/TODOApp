using DomainModule.Service;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.ViewModel;

namespace TodoApp.Areas.Account.ApiController
{
    [Route("api/permissionapi")]
    [ApiController]
    public class PermissionApiController : ControllerBase
    {

        private readonly RoleServiceInterface _roleService;

        public PermissionApiController(RoleServiceInterface roleService)
        {
            _roleService = roleService;
        }
        [Authorize(Policy = "Role-AssignPermission")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignPermission([FromBody] PermissionAssignViewModel model)
        {
            try
            {
                await _roleService.AssignPermission(model.roleId, model.permission).ConfigureAwait(true);
                return Ok(new ResponseModel { Status = StatusType.success.ToString(), IsSuccess = true, Message = $"Permission {model.permission} assinged successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel { Status = StatusType.error.ToString(), IsSuccess = true, Message = ex.Message });
            }
        }

        [Authorize(Policy = "Role-UnAssignPermission")]
        [HttpPost("un-assign")]
        public async Task<IActionResult> UnAssignPermission([FromBody] PermissionAssignViewModel model)
        {
            try
            {
                await _roleService.UnAssignPermission(model.roleId, model.permission).ConfigureAwait(true);
                return Ok(new ResponseModel { Status = StatusType.success.ToString(), IsSuccess = true, Message = $"Permission {model.permission} unassinged successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel { Status = StatusType.error.ToString(), IsSuccess = true, Message = ex.Message });
            }
        }
        [Authorize(Policy = "Role-AssignPermission")]
        [HttpPost("assign-all")]
        public async Task<IActionResult> AssignAllPermissionOfModule([FromBody] ModuleAssignViewModel model)
        {
            try
            {
                await _roleService.AssignAllPermissionOfModule(model.roleId, model.module).ConfigureAwait(true);
                return Ok(new ResponseModel { Status = StatusType.success.ToString(), IsSuccess = true, Message = $"Module {model.module}  assigned successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel { Status = StatusType.error.ToString(), IsSuccess = true, Message = ex.Message });
            }
        }

        [Authorize(Policy = "Role-UnAssignPermission")]
        [HttpPost("unassign-all")]
        public async Task<IActionResult> UnAssignAllPermissionOfModule([FromBody]ModuleAssignViewModel model)
        {
            try
            {
                await _roleService.UnAssignPermissionOfModule(model.roleId, model.module).ConfigureAwait(true);
                return Ok(new ResponseModel { Status = StatusType.success.ToString(), IsSuccess = true, Message = $"Module {model.module} unassinged successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel { Status = StatusType.error.ToString(), IsSuccess = true, Message = ex.Message });
            }
        }

        
    }
    
}
