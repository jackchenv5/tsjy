using System.Security.Cryptography;
using System.Text;

namespace Faoem.Common.Utils;

internal static class UserUtils
{
    internal static string GetPasswordHash(string password, string salt)
    {
        return GetHashString(password + salt);
    }

    internal static string GetHashString(string inputString)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
        var stringBuilder = new StringBuilder();
        foreach (var b in bytes)
        {
            stringBuilder.Append(b.ToString("x2"));
        }

        var hashString = stringBuilder.ToString();
        stringBuilder.Clear();
        return hashString;
    }
}