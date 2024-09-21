using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorrywartNEA2
{
    internal class InputManager
    {
        private MouseState _mouseState;
        private MouseState _prevMouseState;

        public InputManager() 
        { 
            _mouseState = new MouseState();
        }

        public void Update()
        {
            _prevMouseState = _mouseState;
            _mouseState = Mouse.GetState();
        }

        public Vector2 MousePos()
        {
            return _mouseState.Position.ToVector2();
        }

        public bool IsLeftMousePressed()
        {
            //If left button is pressed and wasn't just pressed last frame, then we have a click!!!
            if(_mouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        public bool IsRightMousePressed()
        {
            //If right button is pressed and wasn't just pressed last frame, then we have a click!!!
            if (_mouseState.RightButton == ButtonState.Pressed && _prevMouseState.RightButton != ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
    }
}
