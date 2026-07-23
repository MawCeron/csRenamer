namespace csRenamer.Avalonia.Services;

static class FolderExplorer
{
    public static List<TreeNode> GetDrives()
    {
        var drives = new List<TreeNode>();
        foreach (var drive in DriveInfo.GetDrives())
        {
            if (drive.IsReady)
                drives.Add(new TreeNode(drive.Name, drive.Name, isDrive: true));
        }
        return drives;
    }
}
