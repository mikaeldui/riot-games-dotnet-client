using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Client.CodeGeneration
{
    internal static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        public static string[] SplitAndRemoveEmptyEntries(this string input, char separator) =>
            input.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        public static string Replace(this string input, IEnumerable<KeyValuePair<string, string>> map)
        {
            foreach (var knownWord in map)
                input = input.Replace(knownWord.Key, knownWord.Value);
            return input;
        }

        public static string ToPascalCase(this string input)
        {
            if (input.Contains("-")) // kebab
            {
                return String.Join("", input.SplitAndRemoveEmptyEntries('-').Select(p => p.FirstCharToUpper()));
            }
            else if (input.Contains("_"))
            {
                return String.Join("", input.SplitAndRemoveEmptyEntries('_').Select(p => p.FirstCharToUpper()));
            }
            else if (input[0].ToString() == input[0].ToString().ToLower())
            {
                return input.FirstCharToUpper();
            }
            else return input;
        }

        public static string Remove(this string input, string toRemove) =>
            input.Replace(toRemove, "");
    }
}
