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

        public static List<FileItem> GetFiles(string dir, int mode, string pattern)
        {
            List<FileItem> files = new List<FileItem>(); // List to store FileItem objects

            List<string> auxiliary = new List<string>();

            // Get the file system entries based on the pattern
            if (string.IsNullOrEmpty(pattern))
                auxiliary = Directory.GetFileSystemEntries(dir).ToList(); // Get both files and directories
            else
                auxiliary = Directory.GetFiles(dir, pattern).ToList(); // Only files based on the pattern

            auxiliary.Sort(StringComparer.OrdinalIgnoreCase); // Sort entries in a case-insensitive way

            // Iterate through the entries and process based on mode
            foreach (string element in auxiliary)
            {
                switch (mode)
                {
                    case 0: // Files
                        if (File.Exists(element))
                        {
                            FileItem file = new FileItem
                            {
                                FileName = Path.GetFileName(element),
                                FullPath = element
                            };
                            files.Add(file);
                        }
                        break;
                    case 1: // Folders
                        if (Directory.Exists(element))
                        {
                            FileItem folder = new FileItem
                            {
                                FileName = Path.GetFileName(element),
                                FullPath = element
                            };
                            files.Add(folder);
                        }
                        break;
                    case 2: // All (Files and Folders)
                        FileItem item = new FileItem
                        {
                            FileName = Path.GetFileName(element),
                            FullPath = element
                        };
                        files.Add(item);
                        break;
                }
            }

            return files;
        }

    }
}
