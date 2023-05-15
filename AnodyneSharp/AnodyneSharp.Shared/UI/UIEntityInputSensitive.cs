using Microsoft.Xna.Framework;
using AnodyneSharp.Drawing;
using AnodyneSharp.Input;

namespace AnodyneSharp.UI
{
    class UIEntityInputSensitive : UIEntity
    {
        string textureBaseName;
        string currentTexture;

        //  pressEnter = new UIEntity(new Vector2((GameConstants.SCREEN_WIDTH_IN_PIXELS - 96) / 2, GameConstants.SCREEN_HEIGHT_IN_PIXELS), "press_enter", 96, 16, DrawOrder.MENUTEXT)
        public UIEntityInputSensitive(Vector2 pos, string textureName, int frameWidth, int frameHeight, DrawOrder layer)
            : base(pos, textureName, frameWidth, frameHeight, layer)
        {
            textureBaseName = textureName;
            currentTexture = textureName;
        }

        public override void Update()
        {
            string next = textureBaseName;
            if (KeyInput.ControllerMode)
            {
                if (KeyInput.ControllerButtonOffset == 26)
                {
                    next += "_nintendo";
                }
                else if (KeyInput.ControllerButtonOffset == 52)
                {
                    next += "_sony";
                }
                else
                {
                    next += "_xbox";
                }
            }
            if (next != currentTexture)
            {
                SetTexture(next, width, height);
                currentTexture = next;
            }
        }
    }
}
