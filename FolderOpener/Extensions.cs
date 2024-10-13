using System;
using System.Collections.Generic;

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
