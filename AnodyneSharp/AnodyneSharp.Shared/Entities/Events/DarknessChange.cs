﻿using AnodyneSharp.Drawing;
using AnodyneSharp.Registry;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodyneSharp.Entities.Events
{
    [NamedEntity("Event",null,0)]
    public class DarknessChange : Entity
    {
        public DarknessChange(EntityPreset preset, Player p) : base(preset.Position, DrawOrder.ENTITIES)
        {
            GlobalState.darkness.TargetAlpha(float.Parse(preset.TypeValue));
            exists = false;
        }
    }
}
