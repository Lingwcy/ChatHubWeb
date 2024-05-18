using ChatHubApi.System.Entity.Font;
using ChatHubApi.Untils;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System.Security.Cryptography;
using System.Text;

namespace ChatHubApi.Middleware
{
    public class CryptoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISqlSugarClient _db;

        public CryptoMiddleware(RequestDelegate next, ISqlSugarClient db)
        {
            _next = next;
            _db = db;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var path = context.Request.Path;
            if (path.StartsWithSegments("/MyHub") || path.StartsWithSegments("/File") || path.StartsWithSegments("/admin"))
            {
                await _next(context); return;
            }

            //如果请求体中有key则是客户端第一次发起请求来存储key
            string clientIdentifier = context.Request.Headers["Identifier"];
            string Key = context.Request.Headers["Key"].FirstOrDefault();
            //KEY默认是Empty,只有在第一次发送key的时候才会填充为key
            if (Key != "Empty" && Key != null)
            {
                //发现key 绕过此次解密流程
                sysClientKeys newClient = new sysClientKeys { Key = Key, Identifier = clientIdentifier };
                _db.Insertable<sysClientKeys>(newClient).ExecuteCommand();
                await _next(context); return;
            }
            //Key为空。证明是已经经过传递key的客户端
            var client = _db.Queryable<sysClientKeys>().First(x => x.Identifier == clientIdentifier);
            if (client == null) return;


            // 读取原始的请求体  
            var originalBodyStream = context.Request.Body;
            using (var reader = new StreamReader(originalBodyStream))
            {
                var encryptedRequestBody = await reader.ReadToEndAsync();

                // 解密请求体  
                var decryptedRequestBody = Crypto.DecryptByAES(encryptedRequestBody, client.Key, client.Key);

                // 将解密后的数据写入一个新的内存流  
                var decryptedStream = new MemoryStream(Encoding.UTF8.GetBytes(decryptedRequestBody));
                decryptedStream.Position = 0;

                // 将新的内存流赋值给context.Request.Body  
                context.Request.Body = decryptedStream;
                context.Request.ContentType = "application/json";

            }

            // 调用下一个中间件  
            await _next(context);

        }


    }
    public static class CryptoMiddlewareExtensions
    {
        public static IApplicationBuilder UseCrypto(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CryptoMiddleware>();
        }

    }
}
