using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
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

using TEncoding = System.Text.Encoding;

using Bitmap = System.Drawing.Bitmap;
using System.Reflection;
using System.Collections.Specialized;

using System.Drawing.Imaging;

namespace FolderOpener
{
    public class Folder
    {
        public static List<string> Items
        {
            get
            {
                if (_items == null)
                {
                    LoadFromCache();
                }
                return _items;
            }
        }
        private static byte[] _fileData = null;
        private static byte[] fileData
        {
            get
            {
                if (_fileData == null)
                {
                    LoadFromCache();
                }
                return _fileData;
            }
        }
        private static uint[] _pathPtrs = null;
        private static uint[] pathPtrs
        {
            get
            {
                if (_pathPtrs == null)
                {
                    LoadFromCache();
                }
                return _pathPtrs;
            }
        }
        private static uint[] _iconPtrs = null;
        private static uint[] iconPtrs
        {
            get
            {
                if (_iconPtrs == null)
                {
                    LoadFromCache();
                }
                return _iconPtrs;
            }
        }
        private static List<string> _items = null;
        
        public static void CreateCache()
        {
            _items = Directory.GetFiles(Constants.Cwd).Concat(Directory.GetDirectories(Constants.Cwd)).ToList();
            uint itemCount = (uint)_items.Count;
            List<byte> data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(itemCount));
            uint sizeOfCount = sizeof(uint);
            uint sizeOfPtr = sizeof(uint);
            uint sizeOfPtrs = itemCount * sizeOfPtr;

            data.AddRange(new byte[sizeOfPtrs * 2]);

            for (uint i = 0; i < _items.Count; i++)
            {
                byte[] stringBytes = TEncoding.UTF8.GetBytes(_items[(int)i]);
                uint ptrPos = sizeOfCount + sizeOfPtr * i;
                addPtrAndObjData(data, stringBytes, ptrPos);
            }

            Dictionary<int, uint> iconPtrs = new Dictionary<int, uint>();
            for (uint i = 0; i < _items.Count; i++)
            {
                byte[] iconBytes = extractIcon(i);
                int iconHash = iconBytes.GetContentHash();
                uint ptrPos = sizeOfCount + sizeOfPtrs + sizeOfPtr * i;
                if (iconPtrs.ContainsKey(iconHash))
                {
                    data.ReplaceRange((int)ptrPos, BitConverter.GetBytes(iconPtrs[iconHash]));
                    continue;
                }
                iconPtrs[iconHash] = addPtrAndObjData(data, iconBytes, ptrPos);
            }
            writeBinaryFile(data.ToArray(), Constants.CachePath);
        }
        public static void LoadFromCache()
        {
            if (!File.Exists(Constants.CachePath))
            {
                CreateCache();
            }

            // load data from file
            using (FileStream file = new FileStream(Constants.CachePath, FileMode.Open))
            {
                _fileData = new byte[file.Length];
                file.Read(fileData, 0, (int)file.Length);
            }
            uint itemCount = readFromFile(0, sizeof(uint), BitConverter.ToUInt32);
            _pathPtrs = readListFromFile(sizeof(uint), itemCount, sizeof(uint), BitConverter.ToUInt32);
            _iconPtrs = readListFromFile(sizeof(uint) * (1 + itemCount), itemCount, sizeof(uint), BitConverter.ToUInt32);
            //uint[] iconPtrs = readListFromFile(sizeof(uint), itemCount, sizeof(uint) + itemCount * sizeof(uint), BitConverter.ToUInt32);
            _items = new List<string>();
            for (uint i = 0; i < itemCount; i++)
            {
                _items.Add(TEncoding.UTF8.GetString(readObjFromFile(pathPtrs[i])));
            }
        }
        public static ImageSource GetIcon(int itemIndex)
        {
            byte[] imgData = readObjFromFile(_iconPtrs[itemIndex]);
            using (MemoryStream ms = new MemoryStream(imgData))
            {
                return Imaging.CreateBitmapSourceFromHIcon(
                    new Bitmap(ms).GetHicon(),
                    new Int32Rect(0, 0, 0, 0),
                    BitmapSizeOptions.FromWidthAndHeight(32, 32)
                );
            }
        }
        private static byte[] extractIcon(uint itemIndex)
        {
            string PathItem = Items[(int)itemIndex];
            SDIcon icon;
            if (File.Exists(PathItem))
            {
                icon = SDIcon.ExtractAssociatedIcon(PathItem);
            }
            else
            {
                icon = Constants.FolderIcon;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                icon.ToBitmap().Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
        private static void writeBinaryFile(byte[] data, string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create))
            {
                file.Write(data, 0, data.Length);
            }
        }
        private static uint addPtrAndObjData(List<byte> allBytes, byte[] objBytes, uint ptrPos)
        {
            uint ptr = (uint)allBytes.Count;
            // add ptr
            allBytes.ReplaceRange((int)ptrPos, BitConverter.GetBytes(ptr));
            // add string length
            allBytes.AddRange(BitConverter.GetBytes((uint)objBytes.Length));
            // add string data
            allBytes.AddRange(objBytes);
            return ptr;
        }
        private static T readFromFile<T>(uint start, uint bytes, Func<byte[], int, T> convert)
        {
            byte[] data = fileData.SubArray(start, bytes);
            return convert(data, 0);
        }
        private static T[] readListFromFile<T>(uint start, uint listLen, uint bytesItem, Func<byte[], int, T> convert)
        {
            T[] result = new T[listLen];
            for (uint i = 0; i < listLen; i++)
            {
                result[i] = readFromFile(start + i * bytesItem, bytesItem, convert);
            }
            return result;
        }
        private static byte[] readObjFromFile(uint start)
        {
            uint objByteCount = readFromFile(start, sizeof(uint), BitConverter.ToUInt32);
            return readFromFile(start + sizeof(uint), objByteCount, (data, offset) => data);
        }
    }
}
