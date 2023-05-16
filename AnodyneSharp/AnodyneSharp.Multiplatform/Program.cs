using AnodyneSharp.Logging;
using AnodyneSharp.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
static class MathF
{
    public static float PI = MathHelper.Pi;
    public static float Tau = MathHelper.TwoPi;

    public static float Abs(float i)
    {
        return (float) Math.Abs(i);
    }

    public static float Sin(float i)
    {
        return (float) Math.Sin(i);
    }

    public static float Cos(float i)
    {
        return (float) Math.Cos(i);
    }

    public static float Min(float x, float y)
    {
        return (x < y) ? x : y;
    }

    public static float Ceiling(float x)
    {
        return (float) Math.Ceiling(x);
    }

    public static float Sign(float x)
    {
        return Math.Sign(x);
    }

    public static float Sqrt(float x)
    {
        return (float) Math.Sqrt(x);
    }
}
static class StaticOverloads
{
    public static bool EndsWith(this string s, char c)
    {
        // Sigh, C# pls
        return s.EndsWith(c.ToString());
    }

    public static bool Contains(this Rectangle r, Vector2 v)
    {
        return r.Contains(v.ToPoint());
    }

    public static float NextSingle(this Random random)
    {
        return (float) random.NextDouble();
    }

    public static Vector2 ToVector2(this Point p)
    {
        return new Vector2(p.X, p.Y);
    }

    public static Point ToPoint(this Vector2 v)
    {
        return new Point((int) v.X, (int) v.Y);
    }

    public static void Begin(
        this SpriteBatch batch,
        SpriteSortMode sortMode = SpriteSortMode.Deferred,
        BlendState blendState = null,
        SamplerState samplerState = null,
        DepthStencilState depthStencilState = null,
        RasterizerState rasterizerState = null,
        Effect effect = null,
        Matrix? transformMatrix = null
    ) {
        if (transformMatrix.HasValue)
        {
            batch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix.Value);
        }
        else
        {
            batch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect);
        }
    }
}

namespace AnodyneSharp.Multiplatform
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
#if !DEBUG
            try
            {
#endif
                DebugLogger.Init();

                ResourceManager.GetDirectories = GetDirectories;
                ResourceManager.GetFiles = GetFiles;

                using AnodyneGame game = new AnodyneGame();
                game.Run();
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
