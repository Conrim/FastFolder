using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bitmap = System.Drawing.Bitmap;
using System.Drawing.Imaging;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;
using System.Windows.Interop;
using SDIcon = System.Drawing.Icon;

namespace FolderOpener
{
    public static class Extensions
    {
        public static void ReplaceRange<T>(this List<T> list1, int index, T[] array2)
        {
            list1.RemoveRange(index, array2.Length);
            list1.InsertRange(index, array2);
        }

        public static T[] SubArray<T>(this T[] array, uint offset, uint length)
        {
            return SubArray(array, (int)offset, (int)length);
        }
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
        public static int GetContentHash(this byte[] arr)
        {
            unchecked
            {
                // chatgpt codee
                int hash = 17;
                foreach (byte b in arr)
                {
                    hash = hash * 31 + b;
                }
                return hash;
            }
        }
    }
}
