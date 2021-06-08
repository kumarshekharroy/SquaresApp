using System.Text;

namespace SquaresApp.Common.ExtentionMethods
{
    public static class StringExtentions
    {
        /// <summary>
        /// extention method over string. returns SHA256 lowercased hash of the string
        /// </summary>
        /// <param name="stringToBeEncrypted"></param>
        /// <param name="trimLR"></param>
        /// <returns></returns>
        public static string ToSHA256(this string stringToBeEncrypted, bool trimLR = true)
        {
            if (string.IsNullOrWhiteSpace(stringToBeEncrypted))
                return stringToBeEncrypted;

            using var sha256Managed = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder(64);
            var hashedBytes = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(trimLR ? stringToBeEncrypted.Trim() : stringToBeEncrypted));
            foreach (var theByte in hashedBytes)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
