﻿using AnodyneSharp.Dialogue;
using AnodyneSharp.Entities.Events;
using AnodyneSharp.Entities.Gadget;
using AnodyneSharp.MapData;
using AnodyneSharp.Registry;
using AnodyneSharp.Sounds;
using AnodyneSharp.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static AnodyneSharp.States.CutsceneState;

namespace AnodyneSharp.Entities.Enemy.Go
{
    [NamedEntity("Shadow_Briar", map: "GO")]
    public class BriarBossMain : Entity
    {
        Vector2 tl;
        ThornGate gate;
        VolumeEvent volume;

        BriarBossFight fightStage;
        enum State
        {
            Intro,
            Fight,
            Post
        }
        State state = State.Intro;

        Player player;

        public BriarBossMain(EntityPreset preset, Player p) : base(Vector2.Zero, "briar", 16, 16, Drawing.DrawOrder.ENTITIES)
        {
            tl = MapUtilities.GetRoomUpperLeftPos(GlobalState.CurrentMapGrid);
            AddAnimation("idle", CreateAnimFrameArray(0));
            AddAnimation("walk_u", CreateAnimFrameArray(4, 5), 4);
            AddAnimation("oh_no", CreateAnimFrameArray(10));
            Position = tl + new Vector2(80 - width / 2, 32);
            gate = new(tl + Vector2.UnitX * 64);
            volume = new(0, 0.6f);
            player = p;

            if (GlobalState.events.GetEvent("HappyDone") == 0 || GlobalState.events.GetEvent("BlueDone") == 0)
            {
                exists = false;
                volume.exists = false;
            }
        }

        public override void Update()
        {
            base.Update();
            switch (state)
            {
                case State.Intro:
                    if (player.Position.Y < tl.Y + 6 * 16)
                    {
                        state = State.Fight;
                        GlobalState.StartCutscene = Intro();
                    }
                    break;
                case State.Fight:
                    if(!fightStage?.exists ?? false)
                    {
                        state = State.Post;
                    }
                    break;
                case State.Post:
                    player.Position.Y = Math.Min(player.Position.Y, tl.Y + 120);
                    break;
            }
        }

        IEnumerator<CutsceneEvent> Intro()
        {
            yield return new DialogueEvent(DialogueManager.GetDialogue("briar", "before_fight"));

            SoundManager.PlaySoundEffect("stream");
            SoundManager.StopSong();

            var water_anim = CoroutineUtils.OnceEvery(DoWaterAnim(), 0.2f);
            while (water_anim.MoveNext())
            {
                GlobalState.screenShake.Shake(0.01f, 0.1f);
                yield return null;
            }

            Play("oh_no");
            yield return new DialogueEvent(DialogueManager.GetDialogue("briar", "before_fight"));

            bool flash_active = false;
            GlobalState.flash.Flash(1, Color.White, () => flash_active = true);
            while (!flash_active) yield return null;
            
            SoundManager.PlaySong("briar-fight");
            visible = false;
            (GlobalState.Map as Map).offset.X = 160 * 2;

            gate.Position.Y += 9 * 16;

            fightStage = new(player);
            GlobalState.SpawnEntity(fightStage);

            yield break;
        }

        IEnumerator<string> DoWaterAnim()
        {
            var happy_anim = WaterAnim.DoWaterAnim(tl + new Vector2(0, 16 * 4));
            while (happy_anim.MoveNext())
                yield return null;
            var blue_anim = WaterAnim.DoWaterAnim(tl + new Vector2(16 * 9, 16 * 4));
            while (blue_anim.MoveNext())
                yield return null;
            
            Point tl_p = GlobalState.Map.ToMapLoc(tl);
            
            void Set(Point p) => GlobalState.Map.ChangeTile(Layer.BG, tl_p + p, 194);
            
            Set(new Point(5, 1));
            yield return null;
            Set(new Point(5, 0));
            yield return null;
            Set(new Point(4, 1));
            yield return null;
            Set(new Point(4, 0));
            yield break;
        }

        public override IEnumerable<Entity> SubEntities()
        {
            return new List<Entity>() { gate, volume };
        }
    }

    [Collision(typeof(Player))]
    internal class ThornGate : Entity
    {
        public ThornGate(Vector2 pos) : base(pos, "briar_ground_thorn", 32, 16, Drawing.DrawOrder.ENTITIES)
        {
            immovable = true;
            AddAnimation("move", CreateAnimFrameArray(6, 7, 8), 6);
            Play("move");
        }

        public override void Collided(Entity other)
        {
            base.Collided(other);
            Separate(this, other);
            (other as Player).ReceiveDamage(1);
        }
    }
}