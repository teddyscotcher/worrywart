using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorrywartNEA2
{
    internal class UIObject
    {
        public Vector2 pos;
        public Vector2 size;
        public Color colour;

        public UIObject(Vector2 pos, Vector2 size, Color colour)
        {
            this.pos = pos;
            this.size = size;
            this.colour = colour;
        }
        public virtual void Update(InputManager _inputManager) { }

        public virtual void Draw(SpriteBatch _spriteBatch, Texture2D _rectTex, SpriteFont font)
        {

        }
    }
}
