using System.Security.Cryptography;
using System.Text;

namespace SQKLocalServe.Business.Services.Auth
{
    public class RsaPasswordHasher : IPasswordHasher
    {
        private readonly RSA _rsa;
        private const int KeySize = 2048;
        private const int SaltSize = 16;

        public RsaPasswordHasher()
        {
            _rsa = RSA.Create(KeySize);
        }

        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            
            // Combine password and salt
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
            Array.Copy(passwordBytes, saltedPassword, passwordBytes.Length);
            Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            // Hash the salted password using SHA256
            byte[] hashedPassword;
            using (var sha256 = SHA256.Create())
            {
                hashedPassword = sha256.ComputeHash(saltedPassword);
            }

            // Encrypt the hash using RSA
            byte[] encryptedHash = _rsa.Encrypt(hashedPassword, RSAEncryptionPadding.OaepSHA256);

            // Combine salt and encrypted hash
            byte[] result = new byte[salt.Length + encryptedHash.Length];
            Array.Copy(salt, result, salt.Length);
            Array.Copy(encryptedHash, 0, result, salt.Length, encryptedHash.Length);

            return Convert.ToBase64String(result);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Decode the stored hash
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                // Extract salt and encrypted hash
                byte[] salt = new byte[SaltSize];
                byte[] encryptedHash = new byte[hashBytes.Length - SaltSize];
                Array.Copy(hashBytes, salt, SaltSize);
                Array.Copy(hashBytes, SaltSize, encryptedHash, 0, encryptedHash.Length);

                // Combine password and salt
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
                Array.Copy(passwordBytes, saltedPassword, passwordBytes.Length);
                Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Hash the input password
                byte[] inputHash;
                using (var sha256 = SHA256.Create())
                {
                    inputHash = sha256.ComputeHash(saltedPassword);
                }

                // Decrypt the stored hash
                byte[] decryptedHash = _rsa.Decrypt(encryptedHash, RSAEncryptionPadding.OaepSHA256);

                // Compare the hashes
                return inputHash.SequenceEqual(decryptedHash);
            }
            catch
            {
                return false;
            }
        }

        // Optional: Export/Import keys for persistence
        public string ExportPublicKey()
        {
            return Convert.ToBase64String(_rsa.ExportRSAPublicKey());
        }

        public string ExportPrivateKey()
        {
            return Convert.ToBase64String(_rsa.ExportRSAPrivateKey());
        }
    }
}