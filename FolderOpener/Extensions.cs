using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FolderOpener
{
    public static class Extensions
    {
        #region: SHFileOperation stuff for MoveDirectory (ChatGPT Code)
        // SHFileOperation flags
        private const int FO_MOVE = 0x0001;
        private const int FO_COPY = 0x0002;
        private const int FO_DELETE = 0x0003;
        private const int FO_RENAME = 0x0004;

        private const int FOF_NOCONFIRMATION = 0x0010; // No confirmation
        private const int FOF_NOERRORUI = 0x0400; // No error UI
        private const int FOF_SILENT = 0x0004; // No progress UI
        private const int FOF_ALLOWUNDO = 0x0040; // Allow undo

        // SHFileOperation structure
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public int wFunc;
            [MarshalAs(UnmanagedType.LPTStr)] public string pFrom;
            [MarshalAs(UnmanagedType.LPTStr)] public string pTo;
            public ushort fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public int lProgress;
        }

        // Importing the SHFileOperation function from shell32.dll
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
        #endregion
        public static void MoveDirectory(string sourceDir, string targetDir)
        {
            // ChatGPT Code
            // Ensure the source directory ends with a backslash
            sourceDir = sourceDir.TrimEnd('\\') + '\\';
            targetDir = targetDir.TrimEnd('\\') + '\\';

            SHFILEOPSTRUCT shFileOp = new SHFILEOPSTRUCT
            {
                hwnd = IntPtr.Zero,
                wFunc = FO_MOVE,
                pFrom = sourceDir + '\0', // Add a null terminator
                pTo = targetDir + '\0', // Add a null terminator
                fFlags = FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_SILENT,
                fAnyOperationsAborted = false,
                hNameMappings = IntPtr.Zero,
                lProgress = 0
            };

            // Call SHFileOperation
            int result = SHFileOperation(ref shFileOp);

            if (result != 0)
            {
                throw new Exception($"Failed to move {sourceDir} to {targetDir}");
            }
        }

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
                // chatgpt code
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
