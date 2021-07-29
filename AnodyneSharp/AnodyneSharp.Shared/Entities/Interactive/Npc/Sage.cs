﻿using AnodyneSharp.Dialogue;
using AnodyneSharp.Drawing;
using AnodyneSharp.GameEvents;
using AnodyneSharp.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using static AnodyneSharp.States.CutsceneState;

namespace AnodyneSharp.Entities
{
    [Collision(typeof(Player))]
    abstract class Sage : Entity, Interactable
    {
        protected Player _player;
        protected EntityPreset _preset;

        int _initDistance;
        int _stopDistance;
        string _scene;

        public Sage(EntityPreset preset, Player p, int initDistance, int stopDistance, string scene) : base(preset.Position, "sage", 16, 16, DrawOrder.ENTITIES)
        {
            _player = p;
            _preset = preset;
            _initDistance = initDistance;
            _stopDistance = stopDistance;
            _scene = scene;

            width = height = 10;
            offset = Vector2.One * 3;
            Position += offset;
            immovable = true;

            AddAnimation("walk_d", CreateAnimFrameArray(0, 1), 6, true);
            AddAnimation("walk_r", CreateAnimFrameArray(2, 3), 6, true);
            AddAnimation("walk_l", CreateAnimFrameArray(2, 3), 6, true);
            AddAnimation("walk_u", CreateAnimFrameArray(4, 5), 6, true);
            AddAnimation("idle_d", CreateAnimFrameArray(6));
            AddAnimation("idle_r", CreateAnimFrameArray(7));
            AddAnimation("idle_l", CreateAnimFrameArray(7));
            AddAnimation("idle_u", CreateAnimFrameArray(8));

            Play("idle_d");
        }

        protected virtual IEnumerator<CutsceneEvent> StateLogic()
        {
            MoveTowards(_player.Position, 20);

            while ((_player.Position - Position).Length() > _stopDistance)
            {
                yield return null;
            }

            velocity = Vector2.Zero;

            yield return new DialogueEvent(DialogueManager.GetDialogue("sage", _scene));

            yield break;
        }

        public override void Update()
        {
            base.Update();
            if (!_preset.Activated && !GlobalState.ScreenTransition && 
                _player.state == PlayerState.GROUND && (_player.Position - Position).Length() < _initDistance)
            {
                _preset.Activated = true;
                GlobalState.StartCutscene = StateLogic();
            }

            FaceTowards(_player.Position);
            PlayFacing(velocity == Vector2.Zero ? "idle" : "walk");
        }

        protected override void AnimationChanged(string name)
        {
            if (name == "walk_l" || name == "idle_l")
            {
                _flip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                _flip = SpriteEffects.None;
            }
        }

        public override void Collided(Entity other)
        {
            Separate(this, other);
        }

        public bool PlayerInteraction(Facing player_direction)
        {
            GlobalState.Dialogue = DialogueManager.GetDialogue("sage", _scene);
            return true;
        }
    }

    [NamedEntity(xmlName: "Sage", map: "NEXUS")]
    class SageNexus : Sage
    {
        public SageNexus(EntityPreset preset, Player p) : base(preset, p, 64, 20, "enter_nexus")
        {
            if (GlobalState.events.BossDefeated.Contains("TERMINAL"))
            {
                preset.Alive = exists = false;
            }
        }
    }

    [NamedEntity(xmlName: "Sage", map: "OVERWORLD")]
    class SageOverworld : Sage
    {
        public SageOverworld(EntityPreset preset, Player p) : base(preset, p, 56, 28, "bedroom_entrance")
        {
            if (GlobalState.events.BossDefeated.Contains("BEDROOM"))
            {
                preset.Alive = exists = false;
            }
        }

        protected override IEnumerator<CutsceneEvent> StateLogic()
        {
            IEnumerator<CutsceneEvent> baseState = base.StateLogic();

            while(baseState.MoveNext())
            {
                yield return baseState.Current;
            }

            while (!_player.broom.exists)
            {
                _player.actions_disabled = false;
                yield return null;
            }

            while (_player.broom.exists)
            {
                yield return null;
            }

            yield return new DialogueEvent(DialogueManager.GetDialogue("sage", "bedroom_entrance"));

            yield break;
        }
    }

    class DungeonSage : Sage
    {
        public DungeonSage(EntityPreset preset, Player p, int initDistance, int stopDistance, string scene)
            : base(preset,p,initDistance,stopDistance,scene)
        {
            if (GlobalState.events.LeftAfterBoss.Contains(GlobalState.CURRENT_MAP_NAME))
            {
                preset.Alive = exists = false;
            }
        }
    }

    [NamedEntity(xmlName: "Sage", map: "BEDROOM")]
    class SageBedroom : DungeonSage
    {
        public SageBedroom(EntityPreset preset, Player p)
            : base(preset, p, 48, 24, "after_boss")
        { }
    }

    [NamedEntity(xmlName: "Sage", map: "REDCAVE")]
    class SageRedCave : DungeonSage
    {
        public SageRedCave(EntityPreset preset, Player p)
            : base(preset, p, 28, 16, "one")
        { }
    }

    [NamedEntity(xmlName: "Sage", map: "CROWD")]
    class SageCrowd : DungeonSage
    {
        public SageCrowd(EntityPreset preset, Player p)
            : base(preset, p, 46, 24, "one")
        { }
    }


    [NamedEntity(xmlName: "Sage", map: "BLANK"), Events(typeof(EndScreenTransition))]
    class SageBlank : Entity
    {
        EntityPreset _preset;
        public SageBlank(EntityPreset preset, Player p) : base(preset.Position, DrawOrder.ENTITIES)
        {
            visible = false;
            _preset = preset;
        }

        public override void OnEvent(GameEvent e)
        {
            GlobalState.Dialogue = DialogueManager.GetDialogue("sage", "intro", _preset.Frame + 1);
            exists = _preset.Alive = false;
        }
    }
}
