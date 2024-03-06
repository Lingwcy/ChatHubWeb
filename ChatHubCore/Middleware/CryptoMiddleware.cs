using ChatHubApi.System.Entity.Font;
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
            if (path.StartsWithSegments("/MyHub"))
            {
                await _next(context); return;
            }

            //如果请求体中有key则是客户端第一次发起请求来存储key
            string clientIdentifier = context.Request.Headers["Identifier"];
            string Key = context.Request.Headers["Key"].FirstOrDefault();
            //KEY默认是Empty,只有在第一次发送key的时候才会填充为key
            if(Key != "Empty" && Key != null)
            {
                //发现key 绕过此次解密流程
                sysClientKeys newClient = new sysClientKeys { Key = Key,Identifier = clientIdentifier };
                _db.Insertable<sysClientKeys>(newClient).ExecuteCommand();
                await _next(context); return;
            }
            //Key为空。证明是已经经过传递key的客户端
            var client =  _db.Queryable<sysClientKeys>().First(x => x.Identifier == clientIdentifier);
            if (client == null) return;


            // 读取原始的请求体  
            var originalBodyStream = context.Request.Body;
            using (var reader = new StreamReader(originalBodyStream))
            {
                var encryptedRequestBody = await reader.ReadToEndAsync();

                // 解密请求体  
                var decryptedRequestBody = DecryptByAES(encryptedRequestBody, client.Key, client.Key);

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

        /// <summary>  
        /// AES加密算法  
        /// </summary>  
        /// <param name="input">明文字符串</param>  
        /// <returns>字符串</returns>  
        public static string EncryptByAES(string input, string key, string iv)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.FeedbackSize = 128;
                rijndaelManaged.Key = Encoding.UTF8.GetBytes(key);
                rijndaelManaged.IV = Encoding.UTF8.GetBytes(iv);
                ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        byte[] bytes = msEncrypt.ToArray();
                        return Convert.ToBase64String(bytes);
                    }
                }
            }
        }
        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="input">密文字节数组</param>  
        /// <returns>返回解密后的字符串</returns>  
        public static string DecryptByAES(string input, string key, string iv)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            var buffer = Convert.FromBase64String(input);
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.FeedbackSize = 128;
                rijndaelManaged.Key = Encoding.UTF8.GetBytes(key);
                rijndaelManaged.IV = Encoding.UTF8.GetBytes(iv);
                ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (MemoryStream msEncrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srEncrypt = new StreamReader(csEncrypt))
                        {
                            return srEncrypt.ReadToEnd();
                        }
                    }
                }
            }
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
