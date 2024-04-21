using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography;
using System.Text;

namespace ChatHubApi.Untils
{
    public static class Crypto
    {
        private static string AseKey { get;set; }
        private static string AseIv { get; set; }
        public static (string Key, string Iv) Get(IConfiguration configuration)
        {
            //从配置文件中读取密钥
            AseKey = configuration["ASEKey"];
            AseIv = configuration["ASEIv"];
            string Key=string.Empty;string Iv= string.Empty; 
            if (string.IsNullOrEmpty(AseKey) || string.IsNullOrEmpty(AseIv))
            {
                //如果密钥为空，则生成一个随机密钥
                (string key, string iv) = GenerateAESKeyAndIV();
                //保存到配置文件
                configuration["ASEKey"] = key;
                configuration["ASEIv"] = iv;
                AseKey = key;
                AseIv = iv;
                Key = key;
                Iv = iv;
                return (Key, Iv);
            }
            //同时返回密钥和IV
            return (AseKey,AseIv);
        }

        private static void GenerateRandomKey(out string key, out string iv)
        {
            //生成一个随机密钥
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.GenerateIV();
            aes.GenerateKey();
            key = Convert.ToBase64String(aes.Key);
            iv = Convert.ToBase64String(aes.IV);
        }
        private static (string key, string iv) GenerateAESKeyAndIV()
        {
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.GenerateKey();
                rijndaelManaged.GenerateIV();
                string key = Convert.ToBase64String(rijndaelManaged.Key);
                string iv = Convert.ToBase64String(rijndaelManaged.IV);

                return (key, iv);
            }
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
        public static string HashPassword(string password)
        {
            // 将密码转换为字节数组  
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // 创建SHA256哈希算法实例  
            using (SHA256 sha256 = SHA256.Create())
            {
                // 计算哈希值  
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // 将哈希值转换为十六进制字符串  
                StringBuilder hex = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    hex.AppendFormat("{0:x2}", b);
                }

                return hex.ToString();
            }
        }


    }
}
