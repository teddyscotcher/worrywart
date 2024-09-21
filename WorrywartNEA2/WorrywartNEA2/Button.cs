using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorrywartNEA2
{
    internal class Button : UIObject
    {
        //Delegate type to store method that will be called when button is pressed.
        private Delegate pressMethod;
        //Arguments to be put into method that is called when button is pressed.
        private object?[] args;

        private string text;

        public Button(Vector2 pos, Vector2 size, Color colour, string text, Delegate pressMethod, object?[] args) : base(pos, size, colour)
        {
            this.pos = pos;
            this.size = size;
            this.text = text;
            this.pressMethod = pressMethod;
            this.args = args;
        }

        public override void Update(InputManager _inputManager)
        {
            //Is the mouse over the button and mouse is pressed?
            Vector2 mousePos = _inputManager.MousePos();
            if(_inputManager.IsLeftMousePressed() && mousePos.X - pos.X <= size.X && mousePos.X - pos.X > 0 && mousePos.Y - pos.Y <= size.Y && mousePos.Y - pos.Y > 0)
            {
                //If so, call the method
                pressMethod.DynamicInvoke(args);
            }
        }

        public override void Draw(SpriteBatch _spriteBatch, Texture2D _rectTex, SpriteFont font)
        {
            //Start drawing
            _spriteBatch.Begin();
            //Define the bounding rectangle of the button so we can draw it.
            Rectangle rect = new Rectangle(pos.ToPoint(), size.ToPoint());
            _spriteBatch.Draw(_rectTex, rect, colour);
            //Draw the button text on top in the provided font.
            _spriteBatch.DrawString(font, text, new Vector2(rect.X, rect.Y), Color.White);
            //Doen drawing
            _spriteBatch.End();
        }
    }
}
