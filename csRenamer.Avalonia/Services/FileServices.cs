using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

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
    public class FileItem : INotifyPropertyChanged
    {
        private string _fileName = "";
        private string _fullPath = "";
        private string _newName = "";
        private RenameStatus _status = RenameStatus.None;

        public string FileName
        {
            get => _fileName;
            set { _fileName = value; OnPropertyChanged(); }
        }

        public string FullPath
        {
            get => _fullPath;
            set { _fullPath = value; OnPropertyChanged(); }
        }

        public string NewName
        {
            get => _newName;
            set { _newName = value; OnPropertyChanged(); }
        }

        public RenameStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); OnPropertyChanged(nameof(StatusIcon)); }
        }

        public string StatusIcon => Status switch
        {
            RenameStatus.Ok => "avares://csRenamer.Avalonia/Assets/Icons/check.svg",
            RenameStatus.Conflict => "avares://csRenamer.Avalonia/Assets/Icons/alert.svg",
            _ => ""
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
