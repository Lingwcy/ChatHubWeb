using ChatHubApi.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.VisualBasic.FileIO;
using System.Security.Policy;

namespace ChatHubApi.Controllers.Font.File
{
    /// <summary>
    /// 文件服务    
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileController : ControllerBase
    {
        readonly IConfiguration _configuration;
        public FileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        //[AllowAnonymous]
        public IActionResult Upload([FromBody] UploadFileModel md)
        {
            byte[] imageBytes = Convert.FromBase64String(md.base64String);
            //将imageBytes转化为IFormFile
            var file = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, md.fileName , md.fileName);
            string guid = Guid.NewGuid().ToString("N");
            string path = $"Files/images/{guid}-{md.fileName}";
            string physicPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            string dir = Path.GetDirectoryName(physicPath);
            if (!Directory.Exists(dir))Directory.CreateDirectory(dir);
            using (FileStream fs = new FileStream(physicPath, FileMode.Create))
            {
                file.CopyTo(fs);
            }
            string url = path;
            var res = new
            {
                name = $"{guid}-{md.fileName}",
                url = path,
            };
            return Ok(new Response(1, res, "上传成功!"));

        }

        [HttpGet]
        public IActionResult GetImage(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files/images", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "image/png"); // 根据图片的实际MIME类型修改  
        }


    }
}
