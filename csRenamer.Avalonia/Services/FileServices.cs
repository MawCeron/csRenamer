using System.Collections.ObjectModel;
using System.IO;

namespace csRenamer.Avalonia.Services;

public enum RenameStatus
{
    None,
    Ok,
    Conflict,
    Skipped
}

class FileServices
{
    public class FileItem
    {
        public string FileName { get; set; } = "";
        public string FullPath { get; set; } = "";
        public string NewName { get; set; } = "";
        public RenameStatus Status { get; set; } = RenameStatus.None;
    }

    public static ObservableCollection<FileItem> Files = new();

    public static List<FileItem> GetFiles(string dir, int mode, string pattern, bool recursive, CancellationToken token)
    {
        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        List<FileItem> files = new();

        List<string> auxiliary = string.IsNullOrEmpty(pattern)
            ? Directory.GetFileSystemEntries(dir).ToList()
            : Directory.GetFileSystemEntries(dir, pattern, searchOption).ToList();

        auxiliary.Sort(StringComparer.OrdinalIgnoreCase);

        foreach (string element in auxiliary)
        {
            token.ThrowIfCancellationRequested();
            FileAttributes attributes = File.GetAttributes(element);
            bool isHidden = (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

            switch (mode)
            {
                case 0: // Files
                    if (File.Exists(element))
                    {
                        if (!isHidden || string.IsNullOrEmpty(pattern))
                        {
                            files.Add(new FileItem
                            {
                                FileName = Path.GetFileName(element),
                                FullPath = element
                            });
                        }
                    }
                    break;
                case 1: // Folders
                    if (Directory.Exists(element))
                    {
                        if (!isHidden || string.IsNullOrEmpty(pattern))
                        {
                            files.Add(new FileItem
                            {
                                FileName = Path.GetFileName(element),
                                FullPath = element
                            });
                        }
                    }
                    break;
                case 2: // All
                    if (!isHidden || string.IsNullOrEmpty(pattern))
                    {
                        files.Add(new FileItem
                        {
                            FileName = Path.GetFileName(element),
                            FullPath = element
                        });
                    }
                    break;
            }
        }

        return files;
    }

    public static void RenameFiles()
    {
        foreach (var file in Files)
        {
            if (string.IsNullOrWhiteSpace(file.NewName) || file.NewName == file.FileName)
            {
                file.Status = RenameStatus.Skipped;
                continue;
            }

            string directory = Path.GetDirectoryName(file.FullPath)!;
            string newFullPath = Path.Combine(directory, file.NewName);

            if (File.Exists(newFullPath))
            {
                file.Status = RenameStatus.Conflict;
                continue;
            }

            try
            {
                File.Move(file.FullPath, newFullPath);
                file.FileName = file.NewName;
                file.FullPath = newFullPath;
                file.Status = RenameStatus.Ok;
            }
            catch (Exception)
            {
                file.Status = RenameStatus.Skipped;
            }
        }
    }
}
