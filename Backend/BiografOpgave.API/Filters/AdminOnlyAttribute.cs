using Microsoft.AspNetCore.Mvc.Filters;

namespace BiografOpgave.API.Filters;

/// <summary>
/// Very light-weight role gate: requires headers X-User-Id and X-User-Role=Admin
/// and confirms the user exists and has Admin role.
/// </summary>
public class AdminOnlyAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var headers = context.HttpContext.Request.Headers;
        if (!headers.TryGetValue("X-User-Id", out var userIdRaw) ||
            !int.TryParse(userIdRaw, out var userId) ||
            !headers.TryGetValue("X-User-Role", out var roleRaw))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!string.Equals(roleRaw.ToString(), "Admin", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ForbidResult();
            return;
        }

        var userService = context.HttpContext.RequestServices.GetService<IUserService>();
        var user = userService == null ? null : await userService.GetById(userId);
        if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
