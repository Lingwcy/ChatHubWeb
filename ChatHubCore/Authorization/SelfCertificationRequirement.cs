using construct.Application.System.FontServices.Friends.Model;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

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
        private readonly ILogger<SelfCertificationRequirementHandler> _logger;

        public SelfCertificationRequirementHandler(IHttpContextAccessor httpContextAccessor, ILogger<SelfCertificationRequirementHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, SelfCertificationRequirement requirement)
        {
            var usernameClaim = context.User.FindFirst(x => x.Type == "UserName");
            if (usernameClaim != null)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null && httpContext.Request != null)
                {
                    if (httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                    {
                        // 尝试从 POST 请求体中反序列化出 username  
                        var reader = new StreamReader(httpContext.Request.Body);
                        var requestBody = await reader.ReadToEndAsync();
                        httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                        // 正则表达式模式，匹配"username":"value"格式的字符串  
                        var pattern = @"""(?i)xusername"":""([^""]+)""";
                        var match = Regex.Match(requestBody, pattern);
                        if (match.Success)
                        {
                            // 提取username的值  
                            string username = match.Groups[1].Value;

                            _logger.LogWarning($"Body中的username:{username} jwt中的username:{usernameClaim.Value} ");
                            if(username == usernameClaim.Value)
                            {
                                _logger.LogInformation($"{usernameClaim.Value} 执行私有post命令成功");
                                context.Succeed(requirement);
                            }
                        }
                    }
                    else
                    {
                        // 处理 GET 或其他类型的请求，就像之前的代码那样  
                        var queryParameters = httpContext.Request.Query;
                        if (queryParameters.TryGetValue("xusername", out var parameterValue))
                        {
                            var singleValue = parameterValue.FirstOrDefault();
                            if (singleValue != null && singleValue == usernameClaim.Value)
                            {
                                _logger.LogInformation($"{usernameClaim.Value} 执行私有get命令成功");
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
 