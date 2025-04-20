using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WebAPI.Helpers;

public class PasswordHashHelper
{
    public static string GetSalt()
    {
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
        string b64Salt = Convert.ToBase64String(salt);

        return b64Salt;
    }

    public static string GetHash(string password, string b64Salt)
    {
        byte[] salt = Convert.FromBase64String(b64Salt);

        byte[] hash =
            KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8);
        string b64Hash = Convert.ToBase64String(hash);

        return b64Hash;
    }
}