﻿using AnodyneSharp.Drawing;
using AnodyneSharp.Input;
using AnodyneSharp.Registry;
using AnodyneSharp.Resources;
using AnodyneSharp.Sounds;
using AnodyneSharp.States.PauseSubstates.BaseSubstates;
using AnodyneSharp.UI;
using AnodyneSharp.UI.PauseMenu;
using AnodyneSharp.UI.PauseMenu.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodyneSharp.States.PauseSubstates
{
    public class SecretSubstate : GridSubstate
    {
        public SecretSubstate()
            :base("secrets", "secret_trophies", InventoryState.SecretStatus)
        {

        }
    }
}
