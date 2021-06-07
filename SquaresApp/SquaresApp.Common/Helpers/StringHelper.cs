using System;
using System.Text;

namespace SquaresApp.Common.Helpers
{
    public static class StringHelper
    {
        private const string _alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _small_alphabets = "abcdefghijklmnopqrstuvwxyz";
        private const string _numbers = "1234567890";

        private static readonly Random random = new Random();
        public static string GenerateRandomString(bool isAlphaNumeric = false, bool includeLowerCase = false, int length = 6)
        {
            StringBuilder characters = new StringBuilder(_numbers);
            if (isAlphaNumeric)
            {
                characters.Append(_alphabets);
                if (includeLowerCase)
                    characters.Append(_small_alphabets);
            }

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(0, characters.Length)]);
            }
            return result.ToString();
        }

    }
}
