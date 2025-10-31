using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MovieMatch.Api.Filters
{
    public sealed class RequireAuthUnlessInMemory : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public RequireAuthUnlessInMemory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var useInMemory = _configuration.GetValue<bool>("UseInMemory");
            if (useInMemory)
            {
                return Task.CompletedTask; // allow anonymous in in-memory mode
            }

            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
            }
            return Task.CompletedTask;
        }
    }
}

