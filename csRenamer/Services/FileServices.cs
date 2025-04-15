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

        public static List<FileItem> Files = new List<FileItem>(); // Static list to store FileItem objects
        public static List<FileItem> GetFiles(string dir, int mode, string pattern, CancellationToken token)
        {
            List<FileItem> files = new List<FileItem>(); // List to store FileItem objects

            List<string> auxiliary = new List<string>();

            // Get the file system entries based on the pattern
            if (string.IsNullOrEmpty(pattern))
                auxiliary = Directory.GetFileSystemEntries(dir).ToList(); // Get both files and directories
            else
                auxiliary = Directory.GetFileSystemEntries(dir, pattern, SearchOption.TopDirectoryOnly).ToList(); // Only files based on the pattern

            auxiliary.Sort(StringComparer.OrdinalIgnoreCase); // Sort entries in a case-insensitive way

            // Iterate through the entries and process based on mode
            foreach (string element in auxiliary)
            {
                token.ThrowIfCancellationRequested(); // Check for cancellation
                FileAttributes attributes = File.GetAttributes(element);
                bool isHidden = (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

                switch (mode)
                {
                    case 0: // Files
                        if (File.Exists(element))
                        {
                            if (!isHidden || string.IsNullOrEmpty(pattern))
                            {
                                FileItem file = new FileItem
                                {
                                    FileName = Path.GetFileName(element),
                                    FullPath = element
                                };
                                files.Add(file);
                            }
                        }
                        break;
                    case 1: // Folders
                        if (Directory.Exists(element))
                        {
                            if (!isHidden || string.IsNullOrEmpty(pattern))
                            {
                                FileItem folder = new FileItem
                                {
                                    FileName = Path.GetFileName(element),
                                    FullPath = element
                                };
                                files.Add(folder);
                            }
                        }
                        break;
                    case 2: // All (Files and Folders)
                        if (!isHidden || string.IsNullOrEmpty(pattern))
                        {
                            FileItem item = new FileItem
                            {
                                FileName = Path.GetFileName(element),
                                FullPath = element
                            };
                            files.Add(item);
                        }
                        break;
                }
            }

            return files;
        }

        public static List<FileItem> GetFilesRecursively(string dir, int mode, string pattern, CancellationToken token)
        {
            List<FileItem> files = new List<FileItem>(); // List to store FileItem objects

            List<string> auxiliary = new List<string>();

            // Get the file system entries based on the pattern
            if (string.IsNullOrEmpty(pattern))
                auxiliary = Directory.GetFileSystemEntries(dir, "*", SearchOption.AllDirectories).ToList(); // Get both files and directories
            else
                auxiliary = Directory.GetFileSystemEntries(dir, pattern, SearchOption.AllDirectories).ToList(); // Only files based on the pattern

            auxiliary.Sort(StringComparer.OrdinalIgnoreCase); // Sort entries in a case-insensitive way

            // Iterate through the entries and process based on mode
            foreach (string element in auxiliary)
            {
                token.ThrowIfCancellationRequested(); // Check for cancellation
                FileAttributes attributes = File.GetAttributes(element);
                bool isHidden = (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

                switch (mode)
                {
                    case 0: // Files
                        if (File.Exists(element))
                        {
                            if (!isHidden || string.IsNullOrEmpty(pattern))
                            {
                                FileItem file = new FileItem
                                {
                                    FileName = Path.GetFileName(element),
                                    FullPath = element
                                };
                                files.Add(file);
                            }
                        }
                        break;
                    case 1: // Folders
                        if (Directory.Exists(element))
                        {
                            if (!isHidden || string.IsNullOrEmpty(pattern))
                            {
                                FileItem folder = new FileItem
                                {
                                    FileName = Path.GetFileName(element),
                                    FullPath = element
                                };
                                files.Add(folder);
                            }
                        }
                        break;
                    case 2: // All (Files and Folders)
                        if (!isHidden || string.IsNullOrEmpty(pattern))
                        {
                            FileItem item = new FileItem
                            {
                                FileName = Path.GetFileName(element),
                                FullPath = element
                            };
                            files.Add(item);
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
