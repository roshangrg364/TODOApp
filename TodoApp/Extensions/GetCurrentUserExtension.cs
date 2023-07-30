using System.Security.Claims;
using System.Threading.Tasks;
using DomainModule.Entity;
using DomainModule.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;


namespace TodoApp.Extensions
{
    public static class GetCurrentUserExtension
    {
       
            public static string GetCurrentUserId(this ControllerBase controller)
            {
                return controller.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            public static async Task<User> GetCurrentUser(this ControllerBase controller)
            {
                var userId = controller.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                using var serviceScope = ServiceActivator.GetScope();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                return await userManager.FindByIdAsync(userId).ConfigureAwait(true) ?? throw new UserNotFoundException();
            }
        

    }
}
