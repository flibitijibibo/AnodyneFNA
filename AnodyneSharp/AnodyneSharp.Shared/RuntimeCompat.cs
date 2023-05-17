using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

internal static class MathF
{
    public static float PI = MathHelper.Pi;
    public static float Tau = MathHelper.TwoPi;

    public static float Abs(float x)
    {
        return (float) Math.Abs(x);
    }

    public static float Ceiling(float x)
    {
        return (float) Math.Ceiling(x);
    }

    public static float Cos(float x)
    {
        return (float) Math.Cos(x);
    }

    public static float Min(float x, float y)
    {
        return (x < y) ? x : y;
    }

    public static float Round(float x)
    {
        return (float) Math.Round(x);
    }

    public static float Sign(float x)
    {
        return Math.Sign(x);
    }

    public static float Sin(float x)
    {
        return (float) Math.Sin(x);
    }

    public static float Sqrt(float x)
    {
        return (float) Math.Sqrt(x);
    }
}

internal static class MonoGameOverloads
{
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

    public static bool Contains(this Rectangle r, Vector2 v)
    {
        return r.Contains(v.ToPoint());
    }

    public static Point ToPoint(this Vector2 v)
    {
        return new Point((int) v.X, (int) v.Y);
    }

    public static Vector2 ToVector2(this Point p)
    {
        return new Vector2(p.X, p.Y);
    }
}

internal static class DotNetOverloads
{
    public static bool EndsWith(this string s, char c)
    {
        // Sigh, C# pls
        return s.EndsWith(c.ToString());
    }

    public static float NextSingle(this Random random)
    {
        return (float) random.NextDouble();
    }
}
