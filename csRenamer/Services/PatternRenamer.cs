
using System.IO;
using System.Text.RegularExpressions;

namespace csRenamer.Services
{
    public static class PatternRenamer
    {
        public static string RenameUsingPatterns(string originalName, string originalPath, string patternIni, string patternOut, int count, string extension = "")
        {
            string nameOnly = originalName;
            string ext = string.Empty;

            if (!string.IsNullOrEmpty(extension))
            {
                // Elimina la extensión del nombre original si existe y coincide con la extensión pasada
                if (originalName.EndsWith("." + extension, StringComparison.OrdinalIgnoreCase))
                {
                    nameOnly = Path.GetFileNameWithoutExtension(originalName);
                    ext = "." + extension;
                }
            }

            string intermediateName = ReplaceRegexGroups(originalName, patternIni, patternOut);

            if(string.IsNullOrEmpty(intermediateName))
                return string.Empty;

            intermediateName = ReplaceCounterPlaceHolders(intermediateName, count);
            intermediateName = ReplaceDatePlaceHolders(intermediateName);
            intermediateName = ReplaceDirectoyPlaceHolders(intermediateName, originalPath);
            intermediateName = ReplaceRandomPlaceHolders(intermediateName);
            intermediateName = ReplaceFileMetadataPlaceholders(intermediateName, originalName, originalPath);

            return intermediateName + ext;
        }

        private static string ReplaceRegexGroups(string originalName, string pattern, string target)
        {
            var regex = new Regex(BuildRegexPattern(pattern));
            var match = regex.Match(originalName);
            if (!match.Success)
                return string.Empty;
            
            string result = target;
            for (int i = 1; i < match.Groups.Count; i++)
            {
                result = result.Replace("{" + i + "}", match.Groups[i].Value);
            }

            return result;
        }

        private static string BuildRegexPattern(string pattern)
        {
            return pattern
                .Replace(".", @"\.")
                .Replace("[", @"\[")
                .Replace("]", @"\]")
                .Replace("(", @"\(")
                .Replace(")", @"\)")
                .Replace("?", @"\?")
                .Replace("{#}", "([0-9]*)")
                .Replace("{L}", "([a-zA-Z]*)")
                .Replace("{C}", @"([\S]*)")
                .Replace("{X}", @"([\S\s]*)")
                .Replace("{@}", "(.*)");
        }

        private static string ReplaceCounterPlaceHolders(string input, int count)
        {
            var pattern = new Regex(@"{num(\d*)(\+(\d+))?}");
            return pattern.Replace(input, match =>
            {
                int pad = string.IsNullOrEmpty(match.Groups[1].Value) ? 0 : int.Parse(match.Groups[1].Value);
                int offset = string.IsNullOrEmpty(match.Groups[3].Value) ? 0 : int.Parse(match.Groups[3].Value);

                string number = (count + offset).ToString().PadLeft(pad, '0');

                return number;
            });
        }

        private static string ReplaceDatePlaceHolders(string input)
        {
            DateTime now = DateTime.Now;

            return input
                .Replace("{date}", now.ToString("yyyyMMdd"))
                .Replace("{dateshort}", now.ToString("yyMMdd"))
                .Replace("{datedelim}", now.ToString("yyyy-MM-dd"))
                .Replace("{dateshortdelim}", now.ToString("yy-MM-dd"))
                .Replace("{year}", now.ToString("yyyy"))
                .Replace("{month}", now.ToString("MM"))
                .Replace("{monthname}", now.ToString("MMMM"))
                .Replace("{monthsimp}", now.ToString("MMM"))
                .Replace("{day}", now.ToString("dd"))
                .Replace("{dayname}", now.ToString("dddd"))
                .Replace("{daysimp}", now.ToString("ddd"));
        }

        private static string ReplaceDirectoyPlaceHolders(string input, string originalPath)
        {
            string directoryName = Path.GetFileName(Path.GetDirectoryName(originalPath));
            return input
                .Replace("{dir}", directoryName);
        }

        private static string ReplaceRandomPlaceHolders(string input)
        {
            var pattern = new Regex(@"{rand(?:(\d+)-(\d+)|(\d+))?(?:,(\d+))?}");
            return pattern.Replace(input, match =>
            {
                int min = 0, max = 100, padding = 0;

                if (!string.IsNullOrEmpty(match.Groups[1].Value) && !string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    min = int.Parse(match.Groups[1].Value);
                    max = int.Parse(match.Groups[2].Value);
                }
                else if (!string.IsNullOrEmpty(match.Groups[3].Value))
                {
                    max = int.Parse(match.Groups[3].Value);
                }

                if (!string.IsNullOrEmpty(match.Groups[4].Value))
                {
                    padding = int.Parse(match.Groups[4].Value);
                }

                int rand = new Random().Next(min, max + 1);
                return rand.ToString().PadLeft(padding, '0');
            });
        }

        private static string ReplaceFileMetadataPlaceholders(string input, string name, string path)
        {
            DateTime created = File.GetCreationTime(path);
            DateTime modified = File.GetLastWriteTime(path);

            return input
                // Created
                .Replace("{createdate}", created.ToString("yyyyMMdd"))
                .Replace("{createdatedelim}", created.ToString("yyyy-MM-dd"))
                .Replace("{createyear}", created.ToString("yyyy"))
                .Replace("{createmonth}", created.ToString("MM"))
                .Replace("{createmonthname}", created.ToString("MMMM"))
                .Replace("{createmonthsimp}", created.ToString("MMM"))
                .Replace("{createday}", created.ToString("dd"))
                .Replace("{createdayname}", created.ToString("dddd"))
                .Replace("{createdaysimp}", created.ToString("ddd"))
                // Modified
                .Replace("{modifydate}", modified.ToString("yyyyMMdd"))
                .Replace("{modifydatedelim}", modified.ToString("yyyy-MM-dd"))
                .Replace("{modifyyear}", modified.ToString("yyyy"))
                .Replace("{modifymonth}", modified.ToString("MM"))
                .Replace("{modifymonthname}", modified.ToString("MMMM"))
                .Replace("{modifymonthsimp}", modified.ToString("MMM"))
                .Replace("{modifyday}", modified.ToString("dd"))
                .Replace("{modifydayname}", modified.ToString("dddd"))
                .Replace("{modifydaysimp}", modified.ToString("ddd"));
        }
    }
}
