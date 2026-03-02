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
            // 使用 PBKDF2 进行密码哈希，比 SHA256 更安全
            // 生成随机盐
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // 使用 PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);

                // 组合盐和哈希值存储
                byte[] hashBytes = new byte[salt.Length + hash.Length];
                Array.Copy(salt, 0, hashBytes, 0, salt.Length);
                Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 验证密码是否匹配
        /// </summary>
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                // 提取盐
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, salt.Length);

                // 使用相同的盐和迭代次数验证
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);

                    // 比较哈希值
                    for (int i = 0; i < hash.Length; i++)
                    {
                        if (hashBytes[salt.Length + i] != hash[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch
            {
                // 如果格式不正确，回退到旧的 SHA256 验证（用于兼容旧数据）
                return HashPassword(password) == hashedPassword;
            }
        }


    }
}
