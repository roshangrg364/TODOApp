using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Extensions;
using TodoApp.Models;

namespace TodoApp.Controllers.api
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly DashboardServiceInterface _dashboardService;
        public DashboardApiController(DashboardServiceInterface dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var data = await _dashboardService.GetDashboardData(this.GetCurrentUserId());
                return new JsonResult(new ResponseModel { Status = StatusType.success.ToString(), IsSuccess = true, Data = data });
            }
            catch (Exception ex)

            {
                return new JsonResult(new ResponseModel { Status = StatusType.error.ToString(), IsSuccess = false, Message = ex.Message });
            }
        }
    }
}
