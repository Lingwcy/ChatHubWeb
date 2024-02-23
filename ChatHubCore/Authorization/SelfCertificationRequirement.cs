using Microsoft.AspNetCore.Authorization;

namespace ChatHubApi.Authorization
{
    public class SelfCertificationRequirement : IAuthorizationRequirement
    {
        public SelfCertificationRequirement()
        {
        }
    }

    public class SelfCertificationRequirementHandler : AuthorizationHandler<SelfCertificationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SelfCertificationRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SelfCertificationRequirement requirement)
        {
            var username = context.User.FindFirst(x => x.Type == "UserName");
            if (username != null)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null && httpContext.Request != null)
                {  
                    var queryParameters = httpContext.Request.Query;
                    if (queryParameters.TryGetValue("targetname", out var parameterValue))
                    {
                        var singleValue = parameterValue.FirstOrDefault();
                        if(singleValue != null) {
                            if (singleValue == username.Value) { 
                                context.Succeed(requirement); 
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
            
        }
    }
}
 