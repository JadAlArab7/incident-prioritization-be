using System.Security.Cryptography;

namespace Incident.Infrastructure;

public interface IPasswordHasher
{
    (byte[] hash, byte[] salt) HashPassword(string password);
    bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
}

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 32;
    private const int HashSize = 32;
    private const int Iterations = 150000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public (byte[] hash, byte[] salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            Algorithm,
            HashSize
        );

        return (hash, salt);
    }

    public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            password,
            storedSalt,
            Iterations,
            Algorithm,
            HashSize
        );

        return CryptographicOperations.FixedTimeEquals(hashToCompare, storedHash);
    }
}