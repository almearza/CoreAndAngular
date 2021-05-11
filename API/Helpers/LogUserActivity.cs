using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        // private readonly IUserRepository _userRepository;
        // public LogUserActivity(IUserRepository userRepository)
        // {
        //     _userRepository = userRepository;

        // }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user =await repo.GetUserByIdAsync(resultContext.HttpContext.User.GetUserId());
            user.LastActive=DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}