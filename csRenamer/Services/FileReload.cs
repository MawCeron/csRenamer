using csRenamer.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace csRenamer.Services
{
    internal class FileReload
    {
        private CancellationTokenSource? _cts;

        public async Task<IList<FileListItem>> ReloadAsync(
            string path,
            int showOptions,
            string searchPattern,
            bool recursive)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            var token = _cts.Token;

            return await Task.Run(() =>
            {
                return Files.LoadFiles(path, showOptions, searchPattern, recursive, token);
            }, token);
        }



        public void Cancel()
        {
            _cts?.Cancel();
        }
    }
}
