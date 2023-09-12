﻿using AnodyneSharp.Logging;
using AnodyneSharp.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnodyneSharp.Multiplatform
{
    public static class Program
    {
#if NETCOREAPP
        static void Main(string[] args)
        {
            /* Set up custom logging because stdout and stderr are not redirected to the Visual Studio console.
             * Use FNALoggerEXT or Debug.WriteLine instead of Console.WriteLine in your application.
             * Note that this only applies to Debug mode, since Debug.WriteLine is compiled out for Release builds.
             */
#if DEBUG
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                System.Diagnostics.Debug.WriteLine(e.ExceptionObject);
            }
#endif

            if (SDL2.SDL.SDL_GetPlatform().StartsWith("Xbox"))
            {
                SDL2.SDL.SDL_GDKRunApp(FakeMain, IntPtr.Zero);
            }
            else
            {
                RealMain(args);
            }
        }

        static int FakeMain(int argc, IntPtr argv)
        {
            RealMain(new string[] { });
            return 0;
        }

        static void RealMain(string[] args)
#else
        [STAThread]
        static void Main(string[] args)
#endif
        {
            foreach (string arg in args)
            {
                if (arg.Equals("--steamdebug"))
                {
                    Achievements.DebugMode = true;
                }
            }
#if !DEBUG
            try
            {
#endif
                DebugLogger.Init();
                Achievements.Init();

                ResourceManager.GetDirectories = GetDirectories;
                ResourceManager.GetFiles = GetFiles;

                using AnodyneGame game = new AnodyneGame();
                game.Run();
                Achievements.Quit();
#if !DEBUG
            }
            catch (Exception ex)
            {
                DebugLogger.AddException(ex);
            }
#endif
        }

        public static DirectoryInfo[] GetDirectories(string fullPath)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fullPath);

            DirectoryInfo dir = new(path);

            if (!dir.Exists)
            {
                DebugLogger.AddCritical($"Tried loading from {dir.FullName} but failed!", false);
                return Array.Empty<DirectoryInfo>();
            }

            return dir.GetDirectories();
        }

        public static List<FileInfo> GetFiles(string fullPath)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fullPath);

            DirectoryInfo dir = new(path);

            if (!dir.Exists)
            {
                DebugLogger.AddCritical($"Tried loading from {dir.FullName} but failed!", false);
                return new List<FileInfo>();
            }

            return dir.GetFiles().ToList();
        }
    }
}
