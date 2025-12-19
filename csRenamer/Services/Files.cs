using csRenamer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csRenamer.Services
{
    internal class Files
    {
        internal static List<FileListItem> LoadFiles(string selectedPath, int showOptions, string searchPattern, bool recursive, CancellationToken token)
        {
            var result = new List<FileListItem>();

            var searchOption = recursive
                ? System.IO.SearchOption.AllDirectories
                : System.IO.SearchOption.TopDirectoryOnly;

            bool includeHidden = string.IsNullOrWhiteSpace(searchPattern);
            if (includeHidden)
                searchPattern = "*";

            IEnumerable<string> entries;

            try
            {
                entries = Directory.EnumerateFileSystemEntries(selectedPath, searchPattern, searchOption);
                
            }
            catch { return result; }

            foreach (var path in entries)
            {
                token.ThrowIfCancellationRequested();
                
                var attributes = File.GetAttributes(path);
                
                if (!includeHidden
                    && (attributes & FileAttributes.Hidden) != 0)
                    continue;

                bool isDirectory = (attributes & FileAttributes.Directory) != 0;

                if (showOptions == 0 && isDirectory) continue;
                if (showOptions == 1 && !isDirectory) continue;

                result.Add(new FileListItem
                {
                    Name = Path.GetFileName(path),
                    FullPath = path,
                    Extension = isDirectory ? string.Empty : Path.GetExtension(path),
                    NewName = Path.GetFileName(path),
                });
            }

            return result;
        }
    }
}
