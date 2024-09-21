using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorrywartNEA2
{
    internal static class TextureHelper
    {
        //Create white rect texture of given dimensions
        public static Texture2D RectTexture(GraphicsDevice _graphicsDevice, int width, int height)
        {
            int numPixels = width * height;
            Texture2D tex = new Texture2D(_graphicsDevice, width, height);
            Color[] data = new Color[numPixels];
            for(int i = 0; i < numPixels; i++)
            {
                data[i] = Color.White;
            }
            tex.SetData(data);
            return tex;
        }
    }
}
