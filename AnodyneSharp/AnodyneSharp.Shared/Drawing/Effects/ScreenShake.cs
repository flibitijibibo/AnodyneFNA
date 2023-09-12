using AnodyneSharp.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AnodyneSharp.Drawing.Effects
{
    public class ScreenShake : IFullScreenEffect
    {
        [Flags]
        public enum Directions
        {
            Horizontal = 1,
            Vertical = 2,
            Both = Horizontal | Vertical
        };
        Directions _dir = Directions.Both;
        float _intensity = 0f;
        float _time = 0f;
        float _shakeUpdate = 0f;
        Vector2 _pos = new();
        bool _isPaused = false;

        public bool Active()
        {
            return !_isPaused && _time > 0f;
        }

        public void Shake(float intensity, float time, Directions shakeDir = Directions.Both)
        {
            _time = GlobalState.settings.screenshake ? time : 0f;
            _intensity = intensity;
            _dir = shakeDir;
            _pos = new();
        }

        public void Update()
        {
            _time -= GameTimes.DeltaTime;

            _shakeUpdate += GameTimes.DeltaTime;
            if (_shakeUpdate < 1 / 30f) return;
            
            _shakeUpdate = 0f;
            if((_dir & Directions.Horizontal) == Directions.Horizontal)
            {
                _pos.X = (float)GlobalState.RNG.NextDouble() * 2f * _intensity - _intensity;
            }
            if((_dir & Directions.Vertical) == Directions.Vertical)
            {
                _pos.Y = (float)GlobalState.RNG.NextDouble() * 2f * _intensity - _intensity;
            }
            //Debug.Print(_pos.ToString());
            if (_pos.X < 0.003f && _pos.X > -0.003f) _pos.X = 0;
            if (_pos.Y < 0.003f && _pos.Y > -0.003f) _pos.Y = 0;
        }

        public void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            return;
        }

        public void Render(SpriteBatch batch, Texture2D screen)
        {
            Vector2 pos = new Vector2(screen.Width, screen.Height) * _pos;
            batch.GraphicsDevice.Clear(Color.Black);
            //Clamps the screenshake to pixel intervals, which we don't want
            //batch.Begin(samplerState:SamplerState.PointClamp);
            batch.Begin();
            batch.Draw(screen, pos, Color.White);
            batch.End();
        }

        public void Deactivate()
        {
            _time = 0;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
    }
}
