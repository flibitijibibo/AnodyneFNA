using System;
using System.IO;
using System.Text;
using SDL3;

namespace AnodyneSharp.Registry
{
    public static class Storage
    {
        public static bool Exists(string path)
        {
            IntPtr storage = SDL.SDL_OpenUserStorage(null, "AnodyneFNA", 0);
            if (!SDL.SDL_StorageReady(storage))
            {
                throw new NotImplementedException("Bother flibit about async storage support");
            }

            bool result = SDL.SDL_GetStorageFileSize(storage, "Settings.json", out ulong length);

            SDL.SDL_CloseStorage(storage);

            return result;
        }

        #region Game Save I/O

        public static unsafe string LoadGame(int slot)
        {
            string path = $"Saves/Save_{slot + 1}.dat";

            IntPtr storage = SDL.SDL_OpenUserStorage(null, "AnodyneFNA", 0);
            if (!SDL.SDL_StorageReady(storage))
            {
                throw new NotImplementedException("Bother flibit about async storage support");
            }

            if (!SDL.SDL_GetStorageFileSize(storage, path, out ulong length))
            {
                return string.Empty;
            }

            byte[] file = new byte[length];
            fixed (byte* f = &file[0])
            {
                SDL.SDL_ReadStorageFile(storage, path, (IntPtr) f, length);
            }

            SDL.SDL_CloseStorage(storage);

            using MemoryStream ms = new MemoryStream(file);
            using StreamReader sr = new StreamReader(
                ms,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true
            );
            return sr.ReadToEnd();
        }

        public static unsafe void SaveGame(int slot, string text)
        {
            string path = $"Saves/Save_{slot + 1}.dat";

            IntPtr storage = SDL.SDL_OpenUserStorage(null, "AnodyneFNA", 0);
            if (!SDL.SDL_StorageReady(storage))
            {
                throw new NotImplementedException("Bother flibit about async storage support");
            }

            SDL.SDL_CreateStorageDirectory(storage, "Saves");

            byte[] file = Encoding.UTF8.GetBytes(text);
            fixed (byte* f = &file[0])
            {
                SDL.SDL_WriteStorageFile(storage, path, (IntPtr) f, (ulong) file.Length);
            }

            SDL.SDL_CloseStorage(storage);
        }

        public static void DeleteGame(int slot)
        {
            string path = $"Saves/Save_{slot + 1}.dat";

            IntPtr storage = SDL.SDL_OpenUserStorage(null, "AnodyneFNA", 0);
            if (!SDL.SDL_StorageReady(storage))
            {
                throw new NotImplementedException("Bother flibit about async storage support");
            }

            SDL.SDL_RemoveStoragePath(storage, path);

            SDL.SDL_CloseStorage(storage);
        }

        #endregion

        #region Settings.json I/O

        public static unsafe string LoadSettings()
        {
            IntPtr storage = SDL.SDL_OpenUserStorage(null, "AnodyneFNA", 0);
            if (!SDL.SDL_StorageReady(storage))
            {
                throw new NotImplementedException("Bother flibit about async storage support");
            }

            if (!SDL.SDL_GetStorageFileSize(storage, "Settings.json", out ulong length))
            {
                return string.Empty;
            }

            byte[] file = new byte[length];
            fixed (byte* f = &file[0])
            {
                SDL.SDL_ReadStorageFile(storage, "Settings.json", (IntPtr) f, length);
            }

            SDL.SDL_CloseStorage(storage);

            using MemoryStream ms = new MemoryStream(file);
            using StreamReader sr = new StreamReader(
                ms,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true
            );
            return sr.ReadToEnd();
        }

        public static unsafe void SaveSettings(string text)
        {
            IntPtr storage = SDL.SDL_OpenUserStorage(null, "AnodyneFNA", 0);
            if (!SDL.SDL_StorageReady(storage))
            {
                throw new NotImplementedException("Bother flibit about async storage support");
            }

            byte[] file = Encoding.UTF8.GetBytes(text);
            fixed (byte* f = &file[0])
            {
                SDL.SDL_WriteStorageFile(storage, "Settings.json", (IntPtr) f, (ulong) file.Length);
            }

            SDL.SDL_CloseStorage(storage);
        }

        #endregion

        #region Generic File I/O

        public static unsafe byte[] LoadFile(string path)
        {
            IntPtr storage = SDL.SDL_OpenUserStorage(null, "AnodyneFNA", 0);
            if (!SDL.SDL_StorageReady(storage))
            {
                throw new NotImplementedException("Bother flibit about async storage support");
            }

            if (!SDL.SDL_GetStorageFileSize(storage, path, out ulong length))
            {
                return new byte[0];
            }

            byte[] file = new byte[length];
            fixed (byte* f = &file[0])
            {
                SDL.SDL_ReadStorageFile(storage, path, (IntPtr) f, length);
            }

            SDL.SDL_CloseStorage(storage);

            return file;
        }

        public static unsafe void SaveFile(string path, byte[] file)
        {
            IntPtr storage = SDL.SDL_OpenUserStorage(null, "AnodyneFNA", 0);
            if (!SDL.SDL_StorageReady(storage))
            {
                throw new NotImplementedException("Bother flibit about async storage support");
            }

            fixed (byte* f = &file[0])
            {
                SDL.SDL_WriteStorageFile(storage, path, (IntPtr) f, (ulong) file.Length);
            }

            SDL.SDL_CloseStorage(storage);
        }

        #endregion
    }
}
