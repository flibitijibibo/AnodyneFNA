using AnodyneSharp.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodyneSharp.Drawing.Spritesheet
{
    public class Spritesheet
    {
        TextureHandle tex;

        public Texture2D Tex => tex?.Tex;
        public readonly int Width;
        public readonly int Height;

        public int NumFrames => tex.Width / Width * tex.Height / Height; //Using original size for tilesheet

        public Spritesheet(TextureHandle tex, int width, int height)
        {
            this.tex = tex;
            Width = width;
            Height = height;
        }

        public Rectangle GetRect(int frame)
        {
            int indexX = frame * Width;
            int indexY = 0;

            if (indexX >= Tex.Width)
            {
                indexY = indexX / Tex.Width * Height;
                indexX %= Tex.Width;
            }

            return new Rectangle(indexX, indexY, Width, Height);
        }
    }
}
