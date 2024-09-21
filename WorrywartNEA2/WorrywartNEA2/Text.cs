using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace WorrywartNEA2
{
    internal class Text : UIObject
    {
        public string text;

        public Text(Vector2 pos, Vector2 size, Color colour, string text) : base(pos, size, colour)
        {
            this.pos = pos;
            this.size = size;
            this.text = text;
        }

       

        public override void Draw(SpriteBatch _spriteBatch, Texture2D _rectTex, SpriteFont font)
        {
            _spriteBatch.Begin();
            //Create the rectangle that bounds the Text
            Rectangle rect = new Rectangle(pos.ToPoint(), size.ToPoint());
            //Draw the background rectangle.
            _spriteBatch.Draw(_rectTex, rect, colour);
            //Draw the text on top.
            _spriteBatch.DrawString(font, text, new Vector2(rect.X, rect.Y), Color.White);
            _spriteBatch.End();
        }
    }
}
