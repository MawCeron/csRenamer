using csRenamer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csRenamer.Services
{
    internal class Files
    {
        public static List<FileListItem> FileItems = new List<FileListItem>();

        internal static void LoadFiles(string selectedPath, int showOptions, string selectionPattern, bool recursive, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
