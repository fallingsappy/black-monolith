using EasyEncryption;

namespace DDrop.Utility.Cryptography
{
    public static class PasswordOperations
    {
        public static string HashPassword(string input)
        {
            return SHA.ComputeSHA256Hash(input);
        }

        public static bool PasswordsMatch(string userInput, string savedPassword)
        {
            string hashedInput = HashPassword(userInput);
            bool doPasswordsMatch = string.Equals(hashedInput, savedPassword);
            return doPasswordsMatch;
        }
    }
}
