using SanKalpa.Domain.Services;
using System.Security.Cryptography;

namespace SanKalpa.Infrastructure.Services;

internal sealed class PasswordHashService : IPasswordHashService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;
    private readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool VerifyHashedPassword(string hashedPassword, string password)
    {
        string[] parts = hashedPassword.Split('-');
        byte[] hash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        #region Information
        /*
        if only use SequenceEqual, it will be vulnerable to timing attack
        they can use how long it takes to compare and guess the password

        return hash.SequenceEqual(inputHash);
        */
        #endregion

        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}
