using System.Security.Cryptography;
using System.Text;

namespace Apteryx.Routing.Role.Authority
{
    public class AES256Handler
    {
        ///// <summary>
        ///// AES加密
        ///// </summary>
        ///// <param name="encryptStr">明文</param>
        ///// <param name="key">密钥</param>
        ///// <returns></returns>
        //public string Encrypt(string encryptStr, string key)
        //{
        //    byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
        //    byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptStr);
        //    RijndaelManaged rDel = new RijndaelManaged();
        //    rDel.Key = keyArray;
        //    rDel.Mode = CipherMode.ECB;
        //    rDel.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = rDel.CreateEncryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        //    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        //}

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptStr">明文字符串</param>
        /// <param name="key">秘钥</param>
        /// <param name="iv">加密辅助向量</param>
        /// <returns>密文</returns>
        public string Encrypt(string encryptStr, string key, string iv)
        {
            var aesCipher = Aes.Create();
            aesCipher.Mode = CipherMode.CBC;
            aesCipher.Padding = PaddingMode.PKCS7;
            aesCipher.KeySize = 256;
            aesCipher.BlockSize = 128;
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            if (keyBytes.Length != 32)
                throw new ArgumentException("Key length must be 32 bytes.");
            if (ivBytes.Length != 16)
                throw new ArgumentException("IV length must be 16 bytes.");

            aesCipher.Key = keyBytes;
            aesCipher.IV = ivBytes;
            ICryptoTransform transform = aesCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(encryptStr);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        ///// <summary>
        ///// AES解密
        ///// </summary>
        ///// <param name="decryptStr">密文</param>
        ///// <param name="key">密钥</param>
        ///// <returns></returns>
        //public string Decrypt(string decryptStr, string key)
        //{
        //    byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
        //    byte[] toEncryptArray = Convert.FromBase64String(decryptStr);
        //    RijndaelManaged rDel = new RijndaelManaged();
        //    rDel.Key = keyArray;
        //    rDel.Mode = CipherMode.ECB;
        //    rDel.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = rDel.CreateDecryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        //    return UTF8Encoding.UTF8.GetString(resultArray);
        //}

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">加密字符串</param>
        /// <param name="key">秘钥</param>
        /// <param name="iv">加密辅助向量</param>
        /// <returns>明文</returns>
        public string Decrypt(string decryptStr, string key, string iv)
        {
            var aesCipher = Aes.Create();
            aesCipher.Mode = CipherMode.CBC;
            aesCipher.Padding = PaddingMode.PKCS7;
            aesCipher.KeySize = 256;
            aesCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(decryptStr);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            if (keyBytes.Length != 32)
                throw new ArgumentException("Key length must be 16 bytes.");
            if (ivBytes.Length != 16)
                throw new ArgumentException("IV length must be 16 bytes.");

            aesCipher.Key = keyBytes;
            aesCipher.IV = ivBytes;
            ICryptoTransform transform = aesCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

    }
}
