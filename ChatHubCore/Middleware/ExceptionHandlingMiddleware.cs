using Newtonsoft.Json;
using System.Net;

namespace ChatHubApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);


            }
        }
        private Task HandleException(HttpContext context, Exception ex)
        {
            // 区分业务异常和系统异常
            if (ex is BusinessException businessEx)
            {
                // 业务异常：记录日志，返回 400 和具体错误消息
                _logger.LogWarning("Business exception: {Message}", businessEx.Message);
                var errorMessageObject = new { Message = businessEx.Message, Code = businessEx.Code };
                var errorMessage = JsonConvert.SerializeObject(errorMessageObject);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(errorMessage);
            }

            // 系统异常：记录详细错误信息到日志，但不对外暴露
            _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);

            // 对外返回通用错误消息，避免泄露内部系统信息
            var systemErrorObject = new { Message = "服务器内部错误，请稍后重试", Code = "system_error" };
            var systemErrorMessage = JsonConvert.SerializeObject(systemErrorObject);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(systemErrorMessage);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }

    }
}
