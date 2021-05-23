﻿using AnodyneSharp.Dialogue;
using AnodyneSharp.Input;
using AnodyneSharp.Registry;
using AnodyneSharp.Sounds;
using AnodyneSharp.UI;
using AnodyneSharp.UI.PauseMenu;
using AnodyneSharp.UI.PauseMenu.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static AnodyneSharp.Utilities.TextUtilities;

namespace AnodyneSharp.States.MenuSubstates
{
    public class ControlsSubstate : Substate
    {
        private List<List<(UILabel function, UILabel keyboard, UILabel controller, KeyFunctions keyFunction)>> _keyBindPages;

        private UIEntity _bgBox;

        private UILabel pageLabel;

        private TextSelector _pageSetter;

        int _page;
        int _selectorPos;

        public ControlsSubstate()
        {
            _page = 0;

            _keyBindPages = new();

            SetLabels();

            _pageSetter = new TextSelector(new Vector2(61, 156), 32, 0, true, Drawing.DrawOrder.TEXT, "1/4", "2/4", "3/4", "4/4")
            {
                noConfirm = true,
                noLoop = true
            };
        }

        public override void GetControl()
        {
            base.GetControl();

            _page = 0;

            SetCursorPos(0);
        }

        public override void Update()
        {
            base.Update();

            bool moved = false;

            if (KeyInput.JustPressedRebindableKey(KeyFunctions.Up))
            {
                SetCursorPos(_selectorPos - 1);
                moved = true;
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Down))
            {
                SetCursorPos(_selectorPos + 1);
                moved = true;
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Right))
            {
                if (_page == 3)
                {
                    return;
                }

                moved = true;

                _page++;

                _pageSetter.SetValue(_page);
                PageValueChanged();
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Left))
            {
                if (_page == 0)
                {
                    return;
                }

                moved = true;

                _page--;

                _pageSetter.SetValue(_page);
                PageValueChanged();
            }


            if (moved)
            {
                SoundManager.PlaySoundEffect("menu_move");
            }
        }


        public override void DrawUI()
        {
            base.DrawUI();

            _pageSetter.Draw();

            pageLabel.Draw();

            foreach (var (function, keyboard, controller, _) in _keyBindPages[_page])
            {
                function.Draw();
                keyboard.Draw();
                controller.Draw();
            }

            _bgBox.Draw();
        }

        public override void HandleInput()
        {
            if (KeyInput.JustPressedRebindableKey(KeyFunctions.Cancel) || KeyInput.JustPressedRebindableKey(KeyFunctions.Pause))
            {
                ExitSubState();
            }
        }

        private void SetCursorPos(int newSelectorPos)
        {
            if (newSelectorPos < 0 || newSelectorPos > _keyBindPages[_page].Count - 1)
            {
                return;
            }

            _selectorPos = newSelectorPos;


            selector.Position = _keyBindPages[_page][_selectorPos].function.Position - new Vector2(selector.sprite.Width, -2);
            selector.Position.Y += CursorOffset;
        }

        private void PageValueChanged()
        {
            pageLabel.SetText(DialogueManager.GetDialogue("misc", "any", "controls", 15 + _page));

            if (_selectorPos > _keyBindPages[_page].Count - 1)
            {
                SetCursorPos(_keyBindPages[_page].Count - 1);
            }
        }

        private void SetLabels()
        {
            int menuWidth = 140;

            float x = GameConstants.SCREEN_WIDTH_IN_PIXELS / 2 - menuWidth / 2;
            float y = 8;
            float yStep = GameConstants.FONT_LINE_HEIGHT - GameConstants.LineOffset + 8;

            float yStart = (GameConstants.FONT_LINE_HEIGHT - GameConstants.LineOffset + 8) / 2;

            _bgBox = new UIEntity(new Vector2(x, y), "controls", menuWidth, 160, Drawing.DrawOrder.TEXTBOX);

            pageLabel = new UILabel(new Vector2(x + 10, y + yStart), true, DialogueManager.GetDialogue("misc", "any", "controls", 15), layer: Drawing.DrawOrder.TEXT);

            var keys = KeyInput.RebindableKeys;

            (UILabel function, UILabel keyboard, UILabel controller, KeyFunctions keyFunction) CreateTup(KeyFunctions key, bool isSecond, int num, int pos) => (
                new UILabel(new Vector2(x + 10, y + yStart + yStep * pos), true, !isSecond ? DialogueManager.GetDialogue("misc", "any", "controls", 1 + num) : "",
                    layer: Drawing.DrawOrder.TEXT),
                new UILabel(new Vector2(x + 74, y + yStart + yStep * pos), true,
                    !isSecond
                        ? (keys[key].Keys.Any() ? GetKeyBoardString(keys[key].Keys.FirstOrDefault()) : "")
                        : (keys[key].Keys.Count > 1 ? GetKeyBoardString(keys[key].Keys.ElementAtOrDefault(1)) : ""),
                    layer: Drawing.DrawOrder.TEXT),
                new UILabel(new Vector2(x + 118, y + yStart + yStep * pos), true,
                    !isSecond
                        ? (keys[key].Buttons.Any() ? GetButtonString(keys[key].Buttons.FirstOrDefault()) : "")
                        : (keys[key].Buttons.Count > 1 ? GetButtonString(keys[key].Buttons.ElementAtOrDefault(1)) : ""),
                    layer: Drawing.DrawOrder.TEXT),
                key);

            _keyBindPages.Add(new()
            {
                CreateTup(KeyFunctions.Up, false, 1, 1),
                CreateTup(KeyFunctions.Up, true, 1, 2),
                CreateTup(KeyFunctions.Right, false, 4, 3),
                CreateTup(KeyFunctions.Right, true, 4, 4),
                CreateTup(KeyFunctions.Down, false, 2, 5),
                CreateTup(KeyFunctions.Down, true, 2, 6),
                CreateTup(KeyFunctions.Left, false, 3, 7),
                CreateTup(KeyFunctions.Left, true, 3, 8),
            });

            _keyBindPages.Add(new()
            {
                CreateTup(KeyFunctions.Accept, false, 5, 1),
                CreateTup(KeyFunctions.Accept, true, 5, 2),
                CreateTup(KeyFunctions.Cancel, false, 6, 3),
                CreateTup(KeyFunctions.Cancel, true, 6, 4),
                CreateTup(KeyFunctions.Pause, false, 7, 5),
                CreateTup(KeyFunctions.Pause, true, 7, 6),
            });

            _keyBindPages.Add(new()
            {
                CreateTup(KeyFunctions.Broom1, false, 8, 1),
                CreateTup(KeyFunctions.Broom1, true, 8, 2),
                CreateTup(KeyFunctions.Broom2, false, 9, 3),
                CreateTup(KeyFunctions.Broom2, true, 9, 4),
                CreateTup(KeyFunctions.Broom3, false, 10, 5),
                CreateTup(KeyFunctions.Broom3, true, 10, 6),
                CreateTup(KeyFunctions.Broom4, false, 11, 7),
                CreateTup(KeyFunctions.Broom4, true, 11, 8),
            });

            _keyBindPages.Add(new()
            {
                CreateTup(KeyFunctions.NextPage, false, 12, 1),
                CreateTup(KeyFunctions.NextPage, true, 12, 2),
                CreateTup(KeyFunctions.PreviousPage, false, 13, 3),
                CreateTup(KeyFunctions.PreviousPage, true, 13, 4),
            });
        }
    }
}
