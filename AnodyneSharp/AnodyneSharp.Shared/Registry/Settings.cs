using AnodyneSharp.Dialogue;
using AnodyneSharp.Drawing;
using AnodyneSharp.Registry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnodyneSharp.Registry
{
    public enum Resolution
    {
        Windowed,
        Scaled,
        Stretch
    }

    public enum FPS
    {
        Fixed,
        VSync,
        Unlocked,
        FixedHalf
    }

    public class Settings
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Language language { get; set; } = Language.EN;
        
        public float music_volume_scale { get; set; } = 1.0f;
        public float sfx_volume_scale { get; set; } = 1.0f;
        
        public bool autosave_on { get; set; } = true;

        public bool pause_on_unfocus { get; set; } = true;
        public bool fast_text { get; set; } = false;
        public bool invincible { get; set; } = false;
        public bool extended_coyote { get; set; } = false;
        public bool guaranteed_health { get; set; } = false;
        
        public Resolution resolution { get; set; } = Resolution.Scaled;
        public int scale { get; set; } = 3;

        public FPS fps { get; set; } = FPS.FixedHalf;

        public float flash_brightness { get; set; } = 1.0f;
        public float flash_easing { get; set; } = 0.0f;
        public bool screenshake { get; set; } = true;

        public static Settings Load()
        {
            try
            {
                string save = Storage.LoadSettings();
                return JsonSerializer.Deserialize<Settings>(save);
            }
            catch
            {
                Settings fresh = new();
                fresh.scale = SpriteDrawer.MaxScale;
                return fresh;
            }
        }

        public void Save()
        {
            Storage.SaveSettings(JsonSerializer.Serialize<Settings>(this));
        }
    }
}
