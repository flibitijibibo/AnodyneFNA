﻿using AnodyneSharp.Drawing;
using AnodyneSharp.FSM;
using AnodyneSharp.Registry;
using AnodyneSharp.Sounds;
using AnodyneSharp.Utilities;
using Microsoft.Xna.Framework;
using RSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnodyneSharp.Entities.Enemy
{
    [NamedEntity, Enemy, Collision(typeof(Player), typeof(Broom), MapCollision = true, KeepOnScreen = true)]
    public class Slime : HealthDropper
    {
        private enum SlimeType
        {
            Normal,
            //Key, unused
            //Rise, unused
            Bullet = 3
        }

        EntityPreset _preset;

        private SlimeType _type;

        private int _health = 2;

        private IState state;

        private class MoveState : TimerState
        {
            public bool move_frame_sound_sync = false;
            public DateTime exit_time = DateTime.Now;
            public MoveState()
            {
                AddTimer(0.5f, "MoveTimer");
            }
        }

        private float _speed = 20f;


        private EntityPool<Goo> goos;
        private EntityPool<Bullet> bullets;

        private Player target;

        public Slime(EntityPreset preset, Player player)
            : base(preset.Position, "slime", 16, 16, DrawOrder.ENTITIES)
        {
            _preset = preset;

            _type = _preset.Frame == 3 ? SlimeType.Bullet : SlimeType.Normal;

            AddAnimation("Move", CreateAnimFrameArray(0, 1), 3);
            AddAnimation("Hurt", CreateAnimFrameArray(0, 8, 0, 8), 15, false);
            AddAnimation("Dying", CreateAnimFrameArray(0, 8, 0, 8), 12, false);

            goos = new EntityPool<Goo>(8, () => new Goo());
            target = player;

            if (_type == SlimeType.Bullet)
            {
                bullets = new EntityPool<Bullet>(4, () => new Bullet());
                _speed *= 2;
            }

            state = new StateMachineBuilder()
                .State<MoveState>("Move")
                    .Enter((state) =>
                    {
                        Play("Move");
                        state.Advance((float)(DateTime.Now - state.exit_time).TotalSeconds);
                        if (_type == SlimeType.Bullet)
                        {
                            state.AddTimer(1.8f, "ShootTimer");
                        }
                    })
                    .Update((state, time) => SyncSplash(state))
                    .Event("ShootTimer", (state) => bullets.Spawn(b => b.Spawn(this, target)))
                    .Event("MoveTimer", (state) => ChangeDir())
                    .Event<CollisionEvent<Player>>("Player", (state, p) => p.entity.ReceiveDamage(1))
                    .Event<CollisionEvent<Broom>>("Hit", (state, b) => GetHit(b.entity))
                    .Exit((s) =>
                    {
                        s.exit_time = DateTime.Now;
                    })
                .End()
                .State("Hurt")
                    .Enter((state) => Play("Hurt"))
                    .Event<CollisionEvent<Player>>("Player", (state, p) => p.entity.ReceiveDamage(1))
                    .Condition(() => _health <= 0, (state) => state.Parent.ChangeState("Dying"))
                    .Condition(() => _curAnim.Finished, (state) => state.Parent.ChangeState("Move"))
                .End()
                .State("Dying")
                    .Enter((state) => Play("Dying"))
                    .Condition(() => _curAnim.Finished, (state) => { _preset.Alive = false; Die(); })
                .End()
                .Build();
            state.ChangeState("Move");
        }

        private void SyncSplash(MoveState state)
        {
            if (_curAnim.Frame == 1 && !state.move_frame_sound_sync)
            {
                SoundManager.PlaySoundEffect("slime_walk");
                state.move_frame_sound_sync = true;
            }
            else if (_curAnim.Frame == 0)
            {
                state.move_frame_sound_sync = false;
            }
        }

        private void ChangeDir()
        {
            if (_curAnim.Frame == 1)
            {
                //Make it more likely for slimes to stand still periodically
                velocity = Vector2.Zero;
            }
            else
            {
                velocity = new Vector2((float)GlobalState.RNG.NextDouble(), (float)GlobalState.RNG.NextDouble()) - Vector2.One / 2f;
                velocity *= _speed;
            }
        }

        private void GetHit(Broom b)
        {
            SoundManager.PlaySoundEffect("hit_slime");

            goos.Spawn(g => g.Spawn(this), 2);

            _health -= 1;
            velocity = FacingDirection(b.facing) * 100;

            state.ChangeState("Hurt");
        }

        public override void Update()
        {
            state.Update(GameTimes.DeltaTime);
            base.Update();
        }

        public override void Collided(Entity other)
        {
            if (other is Player p)
            {
                state.TriggerEvent("Player", new CollisionEvent<Player>() { entity = p });
            }
            else if (other is Broom b)
            {
                state.TriggerEvent("Hit", new CollisionEvent<Broom>() { entity = b });
            }
        }

        public override IEnumerable<Entity> SubEntities()
        {
            if (_type == SlimeType.Normal)
            {
                return goos.Entities.Concat(base.SubEntities());
            }
            else
            {
                return goos.Entities.Concat(bullets.Entities).Concat(base.SubEntities());
            }

        }

        [Collision(MapCollision = true)]
        private class Goo : Entity
        {
            private IState state;

            private class MoveState : AbstractState
            {
                public Parabola_Thing parabola;
            }

            public Goo() : base(Vector2.Zero, "slime_goo", 6, 6, DrawOrder.PARTICLES)
            {
                AddAnimation("move", CreateAnimFrameArray(0, 1, 2, 3, 1, 3, 1, 2, 1, 0), GlobalState.RNG.Next(5, 10));
                shadow = new Shadow(this, Vector2.Zero, ShadowType.Tiny);

                state = new StateMachineBuilder()
                    .State<MoveState>("Move")
                        .Enter((state) =>
                        {
                            state.parabola = new Parabola_Thing(this, 16, 0.8f + 0.3f * (float)GlobalState.RNG.NextDouble());
                            velocity.X = MathUtilities.OneRandomOf(-1, 1) * (10 + 5 * (float)GlobalState.RNG.NextDouble());
                            velocity.Y = MathUtilities.OneRandomOf(-1, 1) * (10 + 5 * (float)GlobalState.RNG.NextDouble());
                            Play("move");
                            shadow.exists = true;
                            opacity = 1.0f;
                        })
                        .Update((state, time) =>
                        {
                            if (state.parabola.Tick())
                            {
                                state.Parent.ChangeState("Splash");
                            }
                        })
                    .End()
                    .State("Splash")
                        .Enter((state) =>
                        {
                            SoundManager.PlaySoundEffect("slime_splash");
                            shadow.exists = false;
                            SetFrame(_curAnim.Frame);
                            velocity = Vector2.Zero;
                        })
                        .Update((state, time) => opacity -= 0.05f)
                        .Condition(() => opacity <= 0, (state) => exists = false)
                    .End()
                    .Build();
            }

            public void Spawn(Slime parent)
            {
                Position = parent.Position;
                state.ChangeState("Move");
            }

            public override void Update()
            {
                base.Update();
                state.Update(GameTimes.DeltaTime);
            }
        }

        [Collision(typeof(Player), MapCollision = true)]
        private class Bullet : Entity
        {
            public Bullet()
                : base(Vector2.Zero, "slime_bullet", 8, 8, DrawOrder.PARTICLES)
            {
                AddAnimation("move", CreateAnimFrameArray(0, 1), GlobalState.RNG.Next(5, 10));
            }

            public void Spawn(Slime parent, Player target)
            {
                SoundManager.PlaySoundEffect("slime_shoot");

                Position = parent.Position;
                velocity = Vector2.Normalize(target.Position - parent.Position) * 40;

                Play("move");
                opacity = 1.0f;
            }

            public override void Update()
            {
                base.Update();

                opacity -= 0.39f * GameTimes.DeltaTime;

                if (touching != Touching.NONE || opacity < 0.3f)
                {
                    exists = false;
                }
            }

            public override void Collided(Entity other)
            {
                if (other is Player p && p.state != PlayerState.AIR)
                {
                    p.ReceiveDamage(1);
                    exists = false;
                }
            }
        }
    }
}