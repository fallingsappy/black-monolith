using System.Text.RegularExpressions;

namespace DDrop.Utility.Validation
{
    public static class ValidationHelper
    {
        private static readonly Regex Regex = new Regex("^[1-9]{0,}$");

        public static bool IsTextAllowed(string text)
        {
            return Regex.IsMatch(text);
        }
    }
}