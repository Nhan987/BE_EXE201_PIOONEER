using System.Security.Cryptography;
using System.Text;

namespace Tools;

public static class EncryptPassword
{
    public static string Encrypt(string password)
    {
        using (var sha512 = new System.Security.Cryptography.SHA512Managed())
        {
            var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}