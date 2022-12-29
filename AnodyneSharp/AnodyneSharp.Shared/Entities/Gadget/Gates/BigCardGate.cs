﻿using AnodyneSharp.Dialogue;
using AnodyneSharp.Entities.Base.Rendering;
using AnodyneSharp.GameEvents;
using AnodyneSharp.Registry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnodyneSharp.Entities.Gadget
{
    [NamedEntity("CardGate"), Events(typeof(ChangeCardCount))]
    public class BigCardGate : BigGate
    {
        Entity keyhole;
        Entity[] digits;

        public BigCardGate(EntityPreset preset, Player p) : base(preset,p)
        {
            _sentinel.OpensOnInteract = true;

            keyhole = new(Position, "gate_green_slots", 32, 16, new RefLayer(layer_def, 1));
            keyhole.SetFrame(3);

            digits = new Entity[2]
            {
                new(Position + new Vector2(12,6),"gate_green_digits",3,5,new RefLayer(layer_def, 2)),
                new(Position + new Vector2(17,6),"gate_green_digits",3,5,new RefLayer(layer_def, 2))
            };
            digits[0].SetFrame(_preset.Frame / 10);
            digits[1].SetFrame(_preset.Frame % 10);
        }

        public override bool TryUnlock()
        {
            if (GlobalState.inventory.CardCount >= _preset.Frame)
            {
                GlobalState.Dialogue = _preset.Frame switch
                {
                    4 => DialogueManager.GetDialogue("misc", "any", "keyblockgate", 1),
                    < 36 => DialogueManager.GetDialogue("misc","any","keyblockgate",4),
                    36 => DialogueManager.GetDialogue("misc", "any", "keyblockgate", 3),
                    47 => DialogueManager.GetDialogue("misc", "any", "keyblockgate", 5),
                    49 => "!!!",
                    _ => null
                };
                GlobalState.StartCutscene = OpeningSequence();
                return true;
            }
            else
            {
                GlobalState.Dialogue = _preset.Frame switch
                {
                    4 => DialogueManager.GetDialogue("misc", "any", "keyblockgate", 0),
                    49 => "....",
                    50 => DialogueManager.GetDialogue("misc", "any", "keyblockgate", 6),
                    _ => DialogueManager.GetDialogue("misc", "any", "keyblockgate", 2)
                };
                return false;
            }
        }

        public override IEnumerable<Entity> SubEntities()
        {
            return base.SubEntities().Concat(new List<Entity>() { keyhole, digits[0], digits[1] });
        }

        protected override void BreakLock()
        {
            keyhole.exists = digits[0].exists = digits[1].exists = false;
        }

        public override void OnEvent(GameEvent e)
        {
            GlobalState.SpawnEntity(new Explosion(keyhole) { Position = keyhole.Position + new Vector2(5,-4) });
            int newCount = ((ChangeCardCount)e).Count;
            digits[0].SetFrame(newCount / 10);
            digits[1].SetFrame(newCount % 10);
        }
    }
}
