using System.Text;
using System.Text.RegularExpressions;
using Common.Gprc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Common;

public class PasswordValidator
{
    private static readonly byte[] _salt = Encoding.ASCII.GetBytes("salt");
    public static void ValidatePassword(string password, ValidationExceptionBuilder validationExceptionBuilder)
    {
        const string errorMessage = "Password must be at least 8 characters long, must contain at least 1 alphabetic character and at least 1 digit";
        if (!Regex.IsMatch(password, "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$"))
        {
            validationExceptionBuilder.AddError("Password", errorMessage);
        }
    }
    public static string HashPassword(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: _salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
        ));
    }
}
