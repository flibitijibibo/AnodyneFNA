using AnodyneSharp.Drawing;
using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Base.Rendering;
using AnodyneSharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodyneSharp.UI
{
    public class UIEntity : Entity
    {

        public UIEntity(Vector2 pos, string textureName, int frameWidth, int frameHeight, DrawOrder layer)
            : this(pos, textureName, 0, frameWidth, frameHeight, layer)
        {
        }

        public UIEntity(Vector2 pos, string textureName, int frameWidth, int frameHeight, ILayerType layer)
            : this(pos, textureName, 0, frameWidth, frameHeight, layer)
        {
        }

        public UIEntity(Vector2 pos, ISpriteRenderer sprite, DrawOrder layer, SpriteEffects flip)
            : base(pos, sprite, layer)
        {
            _flip = flip;
            HasVisibleHitbox = false;
        }

        public UIEntity(Vector2 pos, ISpriteRenderer sprite, DrawOrder layer) : base(pos, sprite, layer) { HasVisibleHitbox = false; }

        public UIEntity(Vector2 pos, string textureName, int frame, int frameWidth, int frameHeight, DrawOrder layer)
            : base(pos, new StaticSpriteRenderer(textureName, frameWidth, frameHeight, frame, ignoreChaos: true), layer)
        {
            HasVisibleHitbox = false;
        }

        public UIEntity(Vector2 pos, string textureName, int frame, int frameWidth, int frameHeight, ILayerType layer)
            : base(pos, new StaticSpriteRenderer(textureName, frameWidth, frameHeight, frame, layer: layer, ignoreChaos: true))
        {
            HasVisibleHitbox = false;
        }

        protected override bool SetTexture(string textureName, int width, int height, bool ignoreChaos = false, bool allowFailure = false)
        {
            return base.SetTexture(textureName, width, height, true, allowFailure);
        }
    }
}
