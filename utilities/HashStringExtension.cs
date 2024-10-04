namespace Cityinfo.API.utilities;

public static class HashStringExtension
{
    public static string ToHashString(this string value)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(value);
    }

    public static bool ToVerifyHash(this string value, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(value,hash);
    }
}
