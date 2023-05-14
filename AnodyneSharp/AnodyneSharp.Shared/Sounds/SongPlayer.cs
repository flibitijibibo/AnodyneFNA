using Microsoft.Xna.Framework.Audio;
using System;

namespace AnodyneSharp.Sounds
{
    internal class SongPlayer
    {
        const int BufferMs = 128;
        
        DynamicSoundEffectInstance player;
        IntPtr vorbis;
        uint loopStart;

        float[] vorbis_samples = new float[SoundEffect.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(BufferMs),44100,AudioChannels.Stereo)];

        public SongPlayer()
        {
            ResetPlayer();
        }

        ~SongPlayer()
        {
            player?.Dispose();
            if (vorbis != IntPtr.Zero)
            {
                FAudio.stb_vorbis_close(vorbis);
            }
        }

        private void ResetPlayer()
        {
            loopStart = 0;
            float volume = player?.Volume ?? 1f;
            player?.Stop();
            player?.Dispose();
            player = new(44100, AudioChannels.Stereo);
            player.Volume = volume;
            player.BufferNeeded += BufferNeeded;
        }

        private void BufferNeeded(object sender, EventArgs e)
        {
            int decoded = FAudio.stb_vorbis_get_samples_float_interleaved(vorbis, 2, vorbis_samples, vorbis_samples.Length / 2);
            if (decoded == 0)
            {
                FAudio.stb_vorbis_seek(vorbis, loopStart);
                BufferNeeded(sender, e);
                return;
            }

            player.SubmitFloatBufferEXT(vorbis_samples, 0, decoded * 2);
        }

        internal float GetVolume()
        {
            return player.Volume;
        }

        public void Play(string song)
        {
            ResetPlayer();
            int err;
            if (vorbis != IntPtr.Zero)
            {
                FAudio.stb_vorbis_close(vorbis);
            }
            vorbis = FAudio.stb_vorbis_open_filename(song, out err, IntPtr.Zero);
            FAudio.stb_vorbis_comment c = FAudio.stb_vorbis_get_comment(vorbis);
            for (int i = 0; i < c.comment_list_length; i += 1) unsafe
            {
                IntPtr* p = (IntPtr*) c.comment_list;
                string comment = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(p[i]);
                if (comment.StartsWith("LOOPSTART") && comment.Contains("="))
                {
                    if (uint.TryParse(comment.Substring(comment.IndexOf("=") + 1), out loopStart))
                    {
                        break;
                    }
                }
            }
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
