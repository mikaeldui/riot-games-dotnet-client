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

        public static string FirstCharToLower(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToLower(), input.AsSpan(1))
            };

        public static string[] SplitAndRemoveEmptyEntries(this string input, char separator) =>
            input.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        public static string Replace(this string input, IEnumerable<KeyValuePair<string, string>> map)
        {
            foreach (var knownWord in map)
                input = input.Replace(knownWord.Key, knownWord.Value);
            return input;
        }

        /// <summary> Makes the string look like "ThisIsAString".</summary>
        public static string ToPascalCase(this string input)
        {
            if (input.Contains('-')) // kebab
            {
                return String.Join("", input.SplitAndRemoveEmptyEntries('-').Select(p => p.FirstCharToUpper()));
            }
            else if (input.Contains('_')) // snake
            {
                return String.Join("", input.SplitAndRemoveEmptyEntries('_').Select(p => p.FirstCharToUpper()));
            }
            else if (input[0].IsLower()) // camel
            {
                return input.FirstCharToUpper();
            }
            else if (input.IsScreaming()) // scream
            {
                return input.ToLower().FirstCharToUpper();
            }
            else return input; // No idea, just return it.
        }

        /// <summary> Makes the string look like "thisIsAString".</summary>
        public static string ToCamelCase(this string input) => input.ToPascalCase().FirstCharToLower();

        public static string Remove(this string input, string toRemove) => input.Replace(toRemove, "");

        public static bool IsLower(this char input) => input.ToString() == input.ToString().ToLower();

        public static bool IsScreaming(this string input) => input.All(c => c.ToString() == c.ToString().ToUpper());

        public static string EndWith(this string input, string end) => input.EndsWith(end) ? input : input + end;
    }

    public static class ArrayExtensions
    {
        public static T[] Concat<T>(this T[] source, T[] secondArray) =>
            source.Concat(secondArray).ToArray();
    }
}
