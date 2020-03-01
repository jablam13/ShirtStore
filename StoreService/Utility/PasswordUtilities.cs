using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StoreService.Utility
{
    public static class PasswordUtilities
    {
        private const byte SaltLength = 24;
        private const int Iterations = 60000;
        private const byte KeyLength = 24;
        private static readonly RandomNumberGenerator RandomSource = RandomNumberGenerator.Create();

        public static string GenerateHash(string value, string salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(value), Convert.FromBase64String(salt), Iterations);

            var hash = pbkdf2.GetBytes(KeyLength);

            if (pbkdf2 is IDisposable)
            {
                ((IDisposable)pbkdf2).Dispose();
            }
            var hashBase64String = Convert.ToBase64String(hash);

            return hashBase64String;
        }

        public static string GenerateSalt()
        {
            var salt = Random(SaltLength);
            return Convert.ToBase64String(salt);
        }

        private static byte[] Random(int bytes)
        {
            var ret = new byte[bytes];
            lock (RandomSource)
            {
                RandomSource.GetBytes(ret);
            }
            return ret;
        }
    }
}
