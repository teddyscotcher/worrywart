using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorrywartNEA2
{
    internal class UIManager
    {
        private InputManager _inputManager;
        private List<UIObject> _uiObjects;
        private Texture2D _buttonTexture;

        public SpriteFont font;


        public UIManager(GraphicsDevice _graphicsDevice, InputManager _inputManager, SpriteFont font)
        {
            //List of all instantiated UIObjects.
            this._uiObjects = new List<UIObject>();
            //So we can use mouse info:
            this._inputManager = _inputManager;
            //Blank rectangle texture for the background of buttons.
            this._buttonTexture = TextureHelper.RectTexture(_graphicsDevice, 32, 32);
            
        }

        

        public void Add(UIObject obj)
        {
            _uiObjects.Add(obj);
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            foreach (UIObject obj in _uiObjects)
            {
                obj.Draw(_spriteBatch, _buttonTexture, font);
            }
        }

        public void Update()
        {
            foreach(UIObject obj in _uiObjects)
            {
                obj.Update(_inputManager);
            }
        }
    }
}
