using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodyneSharp.Resources
{
    public class TextureHandle
    {
        Texture2D tex;

        public Texture2D Tex => tex;

        //These need to be from the original texture for maps to get the correct width/height for calculating the amount of tiles
        public readonly int Width;
        public readonly int Height;

        public TextureHandle(Texture2D baseTex)
        {
            tex = baseTex;
            Width = baseTex.Width;
            Height = baseTex.Height;
        }

        public void SetTex(Texture2D tex)
        {
            this.tex = tex;
        }
    }
}
