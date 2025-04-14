using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csRenamer.Services
{
    public class SubstitutionsRenamer
    {
        public static string ReplaceSpaces(string name, int mode)
        {            
            switch (mode)
            {
                case 0: // Replace spaces with underscores
                    return name.Replace(" ", "_");
                    
                case 1: // Replace undescores with spaces
                    return name.Replace("_", " ");
                    
                case 2: // Replace spaces with dots
                    return name.Replace(" ", ".");
                    
                case 3: // Replace dots with spaces
                    return name.Replace(".", " ");
                    
                case 4: // Replace spaces with dashes
                    return name.Replace(" ", "-");
                    
                case 5: // Replace dashes with spaces
                    return name.Replace("-", " ");
                    
                case 6: // Remove spaces
                    return name.Replace(" ", "");
                default:
                    return name;
            }
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
