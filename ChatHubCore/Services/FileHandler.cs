

namespace ChatHubApi.Services
{
    public static class FileHandler
    {

        public static byte[] ReadImageAsBytes(string filePath)
        {
            // 确保文件存在  
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("图片文件未找到。", filePath);
            }
            // 读取图片文件为字节数组  
            return File.ReadAllBytes(filePath);
        }
    }
}
