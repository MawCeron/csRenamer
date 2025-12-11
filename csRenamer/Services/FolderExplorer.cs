using csRenamer.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace csRenamer.Services
{
    public static class FolderExplorer
    {
        public static void LoadDrives(TreeView folderTreeView)
        {
            folderTreeView.RootNodes.Clear();

            var drives = DriveInfo.GetDrives().Where(d => d.IsReady);

            foreach (var drive in drives)
            {
                var driveItem = new FolderTreeItem
                {
                    Name = string.IsNullOrWhiteSpace(drive.VolumeLabel)
                    ? drive.Name
                    : $"{drive.Name} ({drive.VolumeLabel})",
                    FullPath = drive.Name,
                    IconGlyph = "\uEDA2"
                };

                var node = new TreeViewNode
                {
                    Content = driveItem,
                    HasUnrealizedChildren = true
                };

                folderTreeView.RootNodes.Add(node);
            }
        }

        internal static bool HasSubfolders(string path)
        {
            try
            {
                return Directory.GetDirectories(path).Any();
            }
            catch
            {
                return false;
            }
        }
    }
}
