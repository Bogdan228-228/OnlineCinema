using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace OnlineCinema.API
{
    public class HangfireAdminFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var user = httpContext?.User;

            if (httpContext?.Request.Host.Host == "localhost")
                return true;

            return user?.Identity?.IsAuthenticated == true && user.IsInRole("Admin");
        }
    }
}
