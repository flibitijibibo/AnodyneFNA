using AnodyneSharp.Drawing.Spritesheet;
using AnodyneSharp.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodyneSharp.MapData.Tiles
{
    public class AnimatedTile
    {
        private Anim _curAnim;
        public Spritesheet sprite;

        public Rectangle spriteRect;

        public AnimatedTile(int[] frames, int framerate, string texName)
        {
            _curAnim = new Anim("a", frames, framerate);

            sprite = new Spritesheet(ResourceManager.GetTexHandle(texName), 16, 16);
        }

        public void UpdateAnimation()
        {
            _curAnim.Update();

            if (_curAnim.Dirty)
            {
                UpdateRect();
            }
        }

        private void UpdateRect()
        {
            spriteRect = sprite.GetRect(_curAnim.Frame);
        }
    }
}
