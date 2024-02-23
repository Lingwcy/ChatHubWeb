using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ChatHubApi.System
{
    public class Response
    {
        public Response(int code, object? data, string? message)
        {
            Data = JsonSerializer.Serialize(data, new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });
            Code = code;
            Message = message;
            Time = DateTime.Now.ToString("yyyy-MM-dd");
        }

        public int Code { get; set; }//业务状态码
        public string? Data { get; set; }//回传数据
        public string? Message { get; set; }//业务消息
        public string Time { get; set; }//时间戳

    }

}