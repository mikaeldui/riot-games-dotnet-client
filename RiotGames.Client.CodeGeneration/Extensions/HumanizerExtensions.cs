using Humanizer;

namespace RiotGames.Client.CodeGeneration
{
    public static class HumanizerExtensions
    {
        public static void PluralizeLast(this string[] source) =>
            source[source.Length - 1] = source[source.Length - 1].Pluralize();

        public static void SingularizeLast(this string[] source) =>
            source[source.Length - 1] = source[source.Length - 1].Singularize();
    }
}
