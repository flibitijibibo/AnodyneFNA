using AnodyneSharp.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AnodyneSharp.Drawing.Effects
{
    public class GrayScale : IFullScreenEffect
    {
        private Effect effect;

        public bool Active()
        {
            return GlobalState.staticEffect.Active();
        }

        public void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            effect = content.Load<Effect>("effects/blackwhite");
        }

        public void Render(SpriteBatch batch, Texture2D screen)
        {

            effect.Parameters["Projection"].SetValue(SpriteDrawer.Projection(new Point(screen.Width, screen.Height)));
            batch.Begin(effect: effect);
            batch.Draw(screen, screen.Bounds, Color.White);
            batch.End();
        }
        public void Deactivate()
        { }

        public void Update()
        { }
    }
}
