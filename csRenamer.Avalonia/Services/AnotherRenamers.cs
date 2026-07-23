

namespace csRenamer.Avalonia.Services
{
    public class AnotherRenamers
    {
        public static string InsertAt(string name, string text, int position)
        {
            position = Math.Clamp(position, 0, name.Length);
            return name.Insert(position, text);
        }

        public static string DeleteFrom(string name, int from, int to)
        {
            from = Math.Clamp(from - 1, 0, name.Length - 1);
            to = Math.Clamp(to - 1, from, name.Length - 1);
            return name.Remove(from, to - from + 1);
        }
    }
}
