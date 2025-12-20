using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csRenamer.Services
{
    public static class PatternService
    {
        public static List<string> LoadPatterns(string assetRelativePath, string userFileName)
        {
            var result = new HashSet<string>();

            // Defaults (Assets)
            var basePath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            var assetPath = Path.Combine(basePath, assetRelativePath);

            if (File.Exists(assetPath))
            {
                foreach (var line in File.ReadAllLines(assetPath))
                    if (!string.IsNullOrWhiteSpace(line))
                        result.Add(line);
            }

            // User patterns (LocalFolder)
            var userPath = GetUserPatternsPath(userFileName);

            if (File.Exists(userPath))
            {
                foreach (var line in File.ReadAllLines(userPath))
                    if (!string.IsNullOrWhiteSpace(line))
                        result.Add(line);
            }

            return result.ToList();
        }

        public static bool SavePattern(string pattern, string fileName)
        {
            
            if (string.IsNullOrWhiteSpace(pattern))
                return false;

            var path = GetUserPatternsPath(fileName);

            var existing = File.Exists(path)
            ? File.ReadAllLines(path)
            : Array.Empty<string>();

            if (existing.Contains(pattern))
                return false;

            File.AppendAllLines(path, new[] { pattern });

            return true;
        }

        public static string LoadUserPatternsRaw(string fileName)
        {
            var path = GetUserPatternsPath(fileName);

            if (!File.Exists(path))
                return string.Empty;

            return string.Join(
                Environment.NewLine,
                File.ReadAllLines(path)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
            );
        }

        public static void SaveUserPatternsRaw(string fileName, string content)
        {
            var path = GetUserPatternsPath(fileName);

            var lines = content
                .Split(Environment.NewLine)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct()
                .ToArray();

            File.WriteAllLines(path, lines);
        }


        private static string GetUserPatternsPath(string fileName)
        {
            var folder = Windows.Storage.ApplicationData
                .Current.LocalFolder.Path;

            var patternsDir = Path.Combine(folder, "Patterns");
            Directory.CreateDirectory(patternsDir);

            return Path.Combine(patternsDir, fileName);
        }
    }
}
