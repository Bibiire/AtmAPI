using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Application.ConfigSettings;
using Microsoft.Extensions.Options;
using Application.InfraInterfaces;

namespace Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly ILogger<EncryptionService> _logger;
        private readonly DevApiSettings _devApiSettings;

        public EncryptionService(ILogger<EncryptionService> logger,IOptions<DevApiSettings> devApiSettings)
        {
            _logger = logger;
            _devApiSettings = devApiSettings.Value;
        }

        /// <summary>
        /// Encrypt fcmb api payload
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string AES_EncryptV2(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            var aesAlg = NewRijndaelManaged();
            string salt = _devApiSettings.ProductKeys.Password;

            var saltBytes = Encoding.ASCII.GetBytes(salt.PadLeft(24, '0'));

            var keyBytes = new byte[24];
            Array.Copy(saltBytes, keyBytes, Math.Min(keyBytes.Length, saltBytes.Length));
            var ivBytes = Encoding.ASCII.GetBytes(_devApiSettings.ProductKeys.VectorKey);

            var encryptor = aesAlg.CreateEncryptor(keyBytes, ivBytes);
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        /// <summary>
        /// Decrypt FCMB api payload
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public (bool, string) AES_DecryptV2(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");

            if (!IsBase64String(cipherText)) return (false, "The Encrypted string is not base64 encoded");
            string text;

            var aesAlg = NewRijndaelManaged();
            string salt = _devApiSettings.ProductKeys.Password;

            var saltBytes = Encoding.ASCII.GetBytes(salt.PadLeft(24, '0'));

            var keyBytes = new byte[24];
            Array.Copy(saltBytes, keyBytes, Math.Min(keyBytes.Length, saltBytes.Length));
            var ivBytes = Encoding.ASCII.GetBytes(_devApiSettings.ProductKeys.VectorKey);

            var decryptor = aesAlg.CreateDecryptor(keyBytes, ivBytes);
            var cipher = Convert.FromBase64String(cipherText);

            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        text = srDecrypt.ReadToEnd();
                    }
                }
            }
            return (true, text);
        }

        private static RijndaelManaged NewRijndaelManaged()
        {

            return new RijndaelManaged()
            {
                KeySize = 192,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

        }

        private static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return base64String.Length % 4 == 0 &&
               Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public string SHA512(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("x2"));
                return hashedInputStringBuilder.ToString();
            }
        }
    }
}
