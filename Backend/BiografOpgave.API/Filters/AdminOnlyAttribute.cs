using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BiografOpgave.API.Filters;

/// <summary>
/// Very light-weight role gate: requires authenticated user with Admin role claim
/// and confirms the user exists and has Admin role.
/// </summary>
public class AdminOnlyAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var principal = context.HttpContext.User;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ForbidResult();
            return;
        }

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            context.Result = new UnauthorizedResult();
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
