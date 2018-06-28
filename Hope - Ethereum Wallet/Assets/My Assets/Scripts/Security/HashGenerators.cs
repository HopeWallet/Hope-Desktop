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

        public static string GetMd5Hash(this string input)
        {
            using (MD5 md5 = MD5.Create())
                return GetHash(md5, input);
        }

        public static string GetSha1Hash(this string input)
        {
            using (SHA1 sha1 = SHA1.Create())
                return GetHash(sha1, input);
        }

        public static string GetSha256Hash(this string input)
        {
            using (SHA256 sha256 = SHA256.Create())
                return GetHash(sha256, input);
        }

        public static string GetSha512Hash(this string input)
        {
            using (SHA512 sha512 = SHA512.Create())
                return GetHash(sha512, input);
        }

        private static string GetHash(HashAlgorithm hash, string input)
        {
            byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();
        }

    }

    public enum HashFunctions
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }

}