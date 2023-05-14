using Microsoft.Xna.Framework.Audio;
using System;

namespace AnodyneSharp.Sounds
{
    internal class SongPlayer
    {
        const int BufferMs = 128;
        
        DynamicSoundEffectInstance player;

        float[] vorbis_samples = new float[SoundEffect.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(BufferMs),44100,AudioChannels.Stereo)];

        public SongPlayer()
        {
            ResetPlayer();
        }

        private void ResetPlayer()
        {
            float volume = player?.Volume ?? 1f;
            player?.Stop();
            player?.Dispose();
            player = new(44100, AudioChannels.Stereo);
            player.Volume = volume;
            player.BufferNeeded += BufferNeeded;
        }

        private void BufferNeeded(object sender, EventArgs e)
        {
        }

        internal float GetVolume()
        {
            return player.Volume;
        }

        public void Play(string song)
        {
            ResetPlayer();
            BufferNeeded(null, null);
            if(player.State != SoundState.Playing)
            {
                player.Play();
            }
        }

        public void Stop() => player.Stop();

        public void SetVolume(float v) => player.Volume = v;

    }
}
