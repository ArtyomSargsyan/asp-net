using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ToDoApi.Middleware
{
    public class AdminRoleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminRoleMiddleware> _logger;

        public AdminRoleMiddleware(RequestDelegate next, ILogger<AdminRoleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/admin"))
            {
                var user = context.User;
                var role = user?.FindFirst("Role")?.Value ?? user?.FindFirst(ClaimTypes.Role)?.Value;

                if (user?.Identity?.IsAuthenticated != true)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Unauthorized" });
                    return;
                }

                if (role != "Admin")
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new { message = "Forbidden: Admins only" });
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class AdminRoleMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminRole(this IApplicationBuilder builder) =>
            builder.UseMiddleware<AdminRoleMiddleware>();
    }
}
