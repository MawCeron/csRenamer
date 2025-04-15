using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace csRenamer.Services
{
    public class AnotherRenamers
    {
        public static string InsertAt(string name, string text, int position)
        {
            string newName = string.Empty;
            position = position - 1; // Adjust for 0-based index

            if (position >= 0 && position <= name.Length)
            {
                string ini = name.Substring(0, position);
                string end = name.Substring(position);
                newName = ini + text + end;
            }
            else
            {
                newName = name + text;
            }

            return newName;
        }

        public static string DeleteFrom(string name, int initialPosition, int endPosition)
        {
            initialPosition = initialPosition - 1; // Adjust for 0-based index
            endPosition = endPosition - 1; // Adjust for 0-based index

            string textIni = name.Substring(0, initialPosition);
            string textEnd = name.Substring(endPosition + 1);
            
            return textIni + textEnd;
        }
    }
}
