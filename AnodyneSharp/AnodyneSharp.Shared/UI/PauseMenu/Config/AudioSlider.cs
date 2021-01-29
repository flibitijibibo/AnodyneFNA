﻿using AnodyneSharp.Drawing;
using AnodyneSharp.Drawing.Spritesheet;
using AnodyneSharp.Registry;
using AnodyneSharp.Resources;
using AnodyneSharp.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodyneSharp.UI.PauseMenu.Config
{
    public class AudioSlider : OptionSelector
    {
        private const float BarOffset = 8f;

        private float min;
        private float max;
        private float stepSize;
        private float current;
        private float start;

        private Spritesheet _slider;
        private Texture2D _sliderInside;
        private Texture2D _sliderBg;

        private bool _useMainMenuFrame;

        public AudioSlider(Vector2 pos, float current, float min, float max, float stepSize, bool useMainMenuFrame)
            : base(pos, 68)
        {
            this.min = min;
            this.max = max;
            this.stepSize = stepSize;
            this.current = current;
            start = current;

            _slider = new Spritesheet(ResourceManager.GetTexture("volume_bar", true), 60, 11);
            _sliderInside = ResourceManager.GetTexture("volume_bar_inside", true);
            _sliderBg = ResourceManager.GetTexture("volume_bar_bg", true);

            _useMainMenuFrame = useMainMenuFrame;
        }

        public override void ResetValue()
        {
            current = start;
            ValueChangedEvent?.Invoke(current.ToString(), -1);
            SoundManager.PlaySoundEffect("menu_select");
        }

        public override void SetValue()
        {
            start = current;
            SoundManager.PlaySoundEffect("menu_select");
        }

        protected override void LeftPressed()
        {
            current -= stepSize;

            if (current <= min)
            {
                current = min;
            }

            SoundManager.PlaySoundEffect("menu_move");

            ValueChangedEvent?.Invoke(current.ToString(), -1);
        }

        protected override void RightPressed()
        {
            current += stepSize;

            if (current >= max)
            {
                current = max;
            }

            SoundManager.PlaySoundEffect("menu_move");

            ValueChangedEvent?.Invoke(current.ToString(), -1);
        }

        public override void DrawUI()
        {
            base.DrawUI();

            SpriteDrawer.DrawGuiSprite(_sliderBg, new Rectangle(
                (int)(position.X + BarOffset - _slider.Width / 2),
                (int)(position.Y - _slider.Height / 2),
                (int)(_slider.Width),
                (int)(_slider.Height)),
                Z: DrawingUtilities.GetDrawingZ(DrawOrder.AUDIO_SLIDER_BG));


            SpriteDrawer.DrawGuiSprite(_slider.Tex, position + new Vector2(BarOffset, 0), _slider.GetRect(_useMainMenuFrame ? 1 : 0), Z: DrawingUtilities.GetDrawingZ(DrawOrder.AUDIO_SLIDER));

            if (current > min)
            {
                int width = (int)(_slider.Width * (current - min) / (max - min));

                SpriteDrawer.DrawGuiSprite(_sliderInside, new Rectangle(
                    (int)(position.X + BarOffset - width / 2),
                    (int)(position.Y - _slider.Height / 2),
                    (int)(width),
                    (int)(_slider.Height)),
                    Z: DrawingUtilities.GetDrawingZ(DrawOrder.AUDIO_SLIDER_BAR));
            }
        }
    }
}
