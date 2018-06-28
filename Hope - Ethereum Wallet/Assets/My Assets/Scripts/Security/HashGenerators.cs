using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Security
{

    public static class HashGenerators
    {

        public static string GetMd5Hash(this string input) => GetHash<MD5>(input);

        public static string GetSha1Hash(this string input) => GetHash<SHA1>(input);

        public static string GetSha256Hash(this string input) => GetHash<SHA256>(input);

        public static string GetSha384Hash(this string input) => GetHash<SHA384>(input);

        public static string GetSha512Hash(this string input) => GetHash<SHA512>(input);

        private static string GetHash<T>(string input) where T : HashAlgorithm
        {
            using (T hash = (T)CryptoConfig.CreateFromName(typeof(T).ToString()))
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }
    }

}