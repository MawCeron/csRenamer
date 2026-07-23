using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace csRenamer.Avalonia.Services;

public class TreeNode : INotifyPropertyChanged
{
    private bool _isExpanded;
    private bool _isLoaded;

    public string Name { get; }
    public string FullPath { get; }
    public bool IsDrive { get; }
    public ObservableCollection<TreeNode> Children { get; } = new();

    public string Icon => IsDrive
        ? "avares://csRenamer.Avalonia/Assets/Icons/hard-drive.svg"
        : "avares://csRenamer.Avalonia/Assets/Icons/folder.svg";

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            OnPropertyChanged();
            if (value && !_isLoaded)
                LoadChildren();
        }
    }

    public TreeNode(string name, string fullPath, bool isDrive = false)
    {
        Name = name;
        FullPath = fullPath;
        IsDrive = isDrive;
    }

    private void LoadChildren()
    {
        _isLoaded = true;
        try
        {
            foreach (var dir in Directory.GetDirectories(FullPath))
            {
                try
                {
                    var dirInfo = new DirectoryInfo(dir);
                    if ((dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                        (dirInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        continue;

                    Children.Add(new TreeNode(Path.GetFileName(dir), dir));
                }
                catch (UnauthorizedAccessException)
                {
                    Children.Add(new TreeNode("[Access Denied]", dir) { _isLoaded = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading directory '{dir}': {ex.Message}");
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Children.Add(new TreeNode("[Access Denied]", FullPath) { _isLoaded = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error expanding folder '{FullPath}': {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
