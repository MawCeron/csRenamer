using System.IO;

namespace csRenamer.Services
{
    class FileServices
    {
        public class FileItem
        {
            public string FileName { get; set; }
            public string FullPath { get; set; }
            public string NewName { get; set; }
        }

        public static List<FileItem> Files = new List<FileItem>();

        public static List<FileItem> GetFiles(string dir, int mode, string pattern, bool recursive, CancellationToken token)
        {
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            List<FileItem> files = new List<FileItem>();

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
                // Validate that NewName is not empty and is different from the current name
                if (string.IsNullOrWhiteSpace(file.NewName) || file.NewName == file.FileName)
                    continue;

                string directory = Path.GetDirectoryName(file.FullPath)!;
                string newFullPath = Path.Combine(directory, file.NewName);

                // Check if a file with the new name already exists
                if (File.Exists(newFullPath))
                    continue;

                try
                {
                    File.Move(file.FullPath, newFullPath);

                    // Update properties if the rename was successful
                    file.FileName = file.NewName;
                    file.FullPath = newFullPath;
                }
                catch (Exception)
                {
                    // If an error occurs, skip and leave the FileItem unchanged
                    continue;
                }
            }
        }

    }
}
