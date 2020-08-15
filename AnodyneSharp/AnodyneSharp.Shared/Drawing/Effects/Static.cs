﻿using AnodyneSharp.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AnodyneSharp.Drawing.Effects
{
    public class Static : IFullScreenEffect
    {
        private Effect effect;
        private static float static_timer = 0;
        private static int static_step = 0;

        public bool Active()
        {
            string map = GlobalState.CURRENT_MAP_NAME;
            return map == "SUBURB" || map == "DRAWER";
        }

        public void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            effect = content.Load<Effect>("effects/static");
        }

        public void Render(SpriteBatch batch, Texture2D screen)
        {
            batch.Begin(effect: effect);
            batch.Draw(screen, new Rectangle(0, 0, screen.Width, screen.Height), Color.White);
            batch.End();
        }

        public void Update()
        {
            static_timer += GameTimes.DeltaTime;
            if (static_timer > 1.0f / 8.0f)
            {
                static_timer = 0;
                static_step = (static_step + 1) % 4;
                effect.Parameters["step"].SetValue(static_step);
            }
        }
    }
}