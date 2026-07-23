using System.Globalization;
using System.Text;

namespace csRenamer.Avalonia.Services
{
    public class SubstitutionsRenamer
    {
        private static readonly (string from, string to)[] SpaceReplacements =
        [
            (" ", "_"),  // 0: spaces to underscores
            ("_", " "),  // 1: underscores to spaces
            (" ", "."),  // 2: spaces to dots
            (".", " "),  // 3: dots to spaces
            (" ", "-"),  // 4: spaces to dashes
            ("-", " "),  // 5: dashes to spaces
            (" ", ""),   // 6: remove spaces
        ];

        public static string ReplaceSpaces(string name, int mode)
        {
            if (mode < 0 || mode >= SpaceReplacements.Length)
                return name;
            var (from, to) = SpaceReplacements[mode];
            return name.Replace(from, to);
        }

        public static string ReplaceWith(string name, string original, string replaced)
        {
            return name.Replace(original, replaced);
        }

        public static string ReplaceCapitalization(string name, int mode)
        {
            switch (mode)
            {
                case 0: // UPPER
                    return name.ToUpper();

                case 1: // lower
                    return name.ToLower();

                case 2: // Capitalize (first letter only)
                    return char.ToUpper(name[0]) + name.Substring(1).ToLower();

                case 3: // Title Case (each word capitalized)
                    return string.Join(" ",
                        name.Split(' ')
                            .Select(word =>
                                string.IsNullOrEmpty(word)
                                    ? word
                                    : char.ToUpper(word[0]) + word.Substring(1).ToLower()));

                default:
                    return name;
            }
        }

        public static string RemoveAccents(string name)
        {
            string normalizedString = name.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string RemoveDuplicatedSymbols(string name)
        {
            var symbols = new List<string> { " ", "_", "-", ".", "(", ")", "[", "]", "{", "}", "/", "\\" };

            foreach (var symbol in symbols)
            {
                while (name.Contains(symbol + symbol))
                {
                    name = name.Replace(symbol + symbol, symbol);
                }
            }

            return name;
        }
    }
}
