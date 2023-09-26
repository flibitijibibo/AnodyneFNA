﻿using AnodyneSharp.Dialogue;
using AnodyneSharp.Input;
using AnodyneSharp.Registry;
using AnodyneSharp.Sounds;
using AnodyneSharp.States.MenuSubstates;
using AnodyneSharp.States.MenuSubstates.MainMenu;
using AnodyneSharp.UI;
using AnodyneSharp.UI.PauseMenu;
using Microsoft.Xna.Framework;
using RSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnodyneSharp.States.MainMenu
{
    public class MainMenuState : State
    {
        private enum MenuState
        {
            Save1,
            Save2,
            Save3,
            Settings,
            Quit,
            Anodyne2
        }

        private static MenuState _menuState;

        private UILabel _versionLabel;
        private UILabel _save1Label;
        private UILabel _save2Label;
        private UILabel _save3Label;
        private UILabel _settingsLabel;
        private UILabel _quitLabel;
        private UILabel _anodyne2Label;

        private UILabel _inputLabel;

        private MenuSelector _selector;
        private MenuState _lastState;

        private FileSubstate[] files;

        private bool _inSubstate;

        private Substate _substate;

        private IState _state;

        private bool isNewSave;

        private float xOffset;

        public MainMenuState()
        {
            files = Enumerable.Range(0, 3).Select((i) => new FileSubstate(i)).ToArray();

            _selector = new MenuSelector();
            _selector.Play("enabledRight");

            _lastState = _menuState;

            UpdateEntities = false;

            _state = new StateMachineBuilder()
                .State("Normal")
                    .Enter((state) =>
                    {
                        SetLabels();
                        StateChanged();
                    })
                    .Update((state, t) =>
                    {
                        if (!_inSubstate && KeyInput.JustPressedRebindableKey(KeyFunctions.Cancel))
                        {
                            SoundManager.PlayPitchedSoundEffect("pause_sound", -0.1f);
                            ChangeStateEvent(AnodyneGame.GameState.TitleScreen);
                        }
                        else if (!_inSubstate)
                        {
                            BrowseInput();
                        }
                        else
                        {
                            _substate.HandleInput();
                        }

                        if (_lastState != _menuState)
                        {
                            StateChanged();
                        }
                    })
                    .Condition(() => _substate.Exit, (state) =>
                    {
                        if (_substate is FileSubstate s)
                        {
                            if (s.LoadedSave)
                            {
                                SoundManager.PlaySoundEffect("menu_select");
                                isNewSave = s.NewSave;
                                _state.ChangeState("FadeOut");

                                return;
                            }
                            else if (s.RefreshSaves)
                            {
                                files = Enumerable.Range(0, 3).Select((i) => new FileSubstate(i)).ToArray();
                            }

                        }

                        _inSubstate = false;
                        _substate.Exit = false;
                        _selector.Play("enabledRight");

                    })
                    .End()
                    .State("FadeOut")
                        .Update((state, t) => GlobalState.black_overlay.ChangeAlpha(0.72f))
                        .Condition(() => GlobalState.black_overlay.alpha == 1, (state) => ChangeStateEvent(isNewSave ? AnodyneGame.GameState.Intro : AnodyneGame.GameState.Game))
                    .End()
                .Build();

            _state.ChangeState("Normal");

        }

        public override void Update()
        {
            base.Update();

            if (GlobalState.RefreshLabels)
            {
                GlobalState.RefreshLabels = false;
                SetLabels();

                foreach (var state in files)
                {
                    state.SetLabels();
                }
            }

            if (KeyInput.ControllerModeChanged)
            {
                _inputLabel.SetText($"{DialogueManager.GetDialogue("misc", "any", "secrets", 14)} {DialogueManager.GetDialogue("misc", "any", "secrets", 15)}");
            }

            _selector.Update();
            _selector.PostUpdate();

            _substate.Update();

            _state.Update(GameTimes.DeltaTime);
        }


        public override void DrawUI()
        {
            base.DrawUI();

            _selector.Draw();

            _versionLabel.Draw();
            _save1Label.Draw();
            _save2Label.Draw();
            _save3Label.Draw();
            _settingsLabel.Draw();
            _quitLabel.Draw();
            _anodyne2Label?.Draw();

            _inputLabel.Draw();

            _substate.DrawUI();
        }

        private void BrowseInput()
        {
            if (KeyInput.JustPressedRebindableKey(KeyFunctions.Accept) || (KeyInput.JustPressedRebindableKey(KeyFunctions.Right) && Achievements.WasInit))
            {
                SoundManager.PlaySoundEffect("menu_select");
                if (_menuState == MenuState.Quit)
                {
                    // Special case: Right on Quit goes to Anodyne2
                    if (Achievements.WasInit && KeyInput.JustPressedRebindableKey(KeyFunctions.Right))
                    {
                        _menuState++;
                    }
                    else
                    {
                        GlobalState.ClosingGame = true;
                    }
                }
                else if (_menuState == MenuState.Anodyne2)
                {
                    Achievements.OpenSequelStorePage();
                }
                else
                {
                    _inSubstate = true;
                    _selector.Play("disabledRight");
                    _substate.GetControl();
                }
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Left))
            {
                if (_menuState == MenuState.Anodyne2)
                {
                    _menuState--;
                }
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Up))
            {
                SoundManager.PlaySoundEffect("menu_move");
                if (_menuState != MenuState.Save1)
                {
                    _menuState--;
                }
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Down))
            {
                SoundManager.PlaySoundEffect("menu_move");
                if (_menuState < MenuState.Quit)
                {
                    _menuState++;
                }
                else if (_menuState == MenuState.Quit && Achievements.WasInit)
                {
                    _menuState++;
                }
            }
        }


        private void StateChanged()
        {
            _lastState = _menuState;
            _selector.Position = new Vector2(2 + xOffset, 34 + (int)_menuState * 16);
            if (_menuState == MenuState.Quit)
            {
                _selector.Position.Y += 8 * 7; // This matches the position set in SetLabels below
            }
            else if (_menuState == MenuState.Anodyne2)
            {
                _selector.Position.X = 80;
                _selector.Position.Y += 8 * 5;
            }

            _substate = _menuState switch
            {
                MenuState.Save1 => files[0],
                MenuState.Save2 => files[1],
                MenuState.Save3 => files[2],
                MenuState.Settings => new ConfigSubstate(true),
                _ => new Substate(),
            };
        }

        private void SetLabels()
        {
            xOffset = GlobalState.CurrentLanguage switch
            {
                Language.ES => -2,
                Language.IT => -2,
                Language.PT_BR => -2,
                _ => 0,
            };

            float x = 10f + xOffset;
            float startY = GameConstants.HEADER_HEIGHT - GameConstants.LineOffset + 11 + (GlobalState.CurrentLanguage == Language.ZH_CN ? 1 : 0);
            float yStep = (GameConstants.FONT_LINE_HEIGHT - GameConstants.LineOffset) * 2;

            Color color = new Color(116, 140, 144);

            string save = "";  /*DialogueManager.GetDialogue("misc", "any", "title", 24);*/


            _versionLabel = new UILabel(new Vector2(x, startY - yStep), false, "v0.95", color);
            _save1Label = new UILabel(new Vector2(x, startY), false, save + 1, color);
            _save2Label = new UILabel(new Vector2(x, startY + yStep), false, save + 2, color);
            _save3Label = new UILabel(new Vector2(x, startY + yStep * 2), false, save + 3, color);
            _settingsLabel = new UILabel(new Vector2(x, startY + yStep * 3), false, DialogueManager.GetDialogue("misc", "any", "config", 0), color);
            _quitLabel = new UILabel(new Vector2(x, startY + yStep * 7.5f), false, DialogueManager.GetDialogue("misc", "any", "save", 6), color);
            if (Achievements.WasInit)
            {
                _anodyne2Label = new UILabel(new Vector2(87f, startY + yStep * 7.5f), false, "Anodyne 2", color);
                if (GlobalState.CurrentLanguage == Language.IT || GlobalState.CurrentLanguage == Language.ES || GlobalState.CurrentLanguage == Language.ZH_CN)
                {
                    _anodyne2Label = new UILabel(new Vector2(100f, startY + yStep * 7.5f), false, "Ano2", color);
                }
            }

            Vector2 inputPos = Vector2.Zero;
            if (GlobalState.CurrentLanguage == Language.ZH_CN)
            {
                inputPos = new Vector2(2, 168 - GameConstants.LineOffset + 1);
            }
            else
            {
                inputPos = new Vector2(2, 168);
                if (GlobalState.CurrentLanguage == Language.KR)
                {
                    inputPos.Y -= 1;
                }
            }
            _inputLabel = new UILabel(inputPos, false, $"{DialogueManager.GetDialogue("misc", "any", "secrets", 14)} {DialogueManager.GetDialogue("misc", "any", "secrets", 15)}", color);
        }
    }
}
