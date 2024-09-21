using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WorrywartNEA2.Cell;

namespace WorrywartNEA2
{
    internal class WorldDrawer
    {
        private GraphicsDevice _graphicsDevice;
        private Texture2D cellTexture;

        //Queue containing cell movements to be drawn.
        public Queue<CellMoveInfo> cellMoveInfoQueue = new Queue<CellMoveInfo>();

        //Cell type and colour lookup dictionary.
        public readonly Dictionary<CellType, Color> typeColourDictionary = new Dictionary<CellType, Color>
        {
            {CellType.Empty, new Color(71, 67, 67) },
            {CellType.Student, new Color(240, 225, 225) },
            {CellType.Worrywart, new Color(232, 84, 81) },
            {CellType.Wall, new Color(128, 119, 119) },
            {CellType.Library, new Color(98, 217, 85) }
        };


        public WorldDrawer(GraphicsDevice _graphicsDevice) 
        {
            this._graphicsDevice = _graphicsDevice;
            cellTexture = TextureHelper.RectTexture(_graphicsDevice, MainGame.CELL_WIDTH-2, MainGame.CELL_WIDTH-2);
        }


        //Subroutine to run through cell movements and represent them on screen.
        public void Draw(SpriteBatch _spriteBatch)
        {
            while(cellMoveInfoQueue.Count > 0)
            {
                CellMoveInfo moveInfo = cellMoveInfoQueue.Dequeue();
                //Convert the colour for this cell type to a Vector4 (RGBA)
                Vector4 toColorVector = typeColourDictionary[moveInfo.toType].ToVector4();
                //If this is the highlighted cell, do a component-wise multiplication of the colour
                //with the built in MonoGame colour LightBlue.
                if (moveInfo.highlighted)
                {
                    toColorVector = Vector4.Multiply(toColorVector, Color.LightBlue.ToVector4());
                }
                //Convert this back to a Color from Vector4
                Color toColor = new Color(toColorVector);
                
                //Fill in the old and new positions with whatever info is in moveInfo
                DrawCell(_spriteBatch, typeColourDictionary[moveInfo.fromType], moveInfo.fromPos);
                DrawCell(_spriteBatch, toColor, moveInfo.toPos);
            }
        }

        
        //Subroutine to draw a full grid of empty cells.
        public void DrawEmptyGrid(SpriteBatch _spriteBatch)
        {
            //Draw a cell at every integer vector with 0 < x < columns and 0 < y < rows.
            for(int x = 0; x < MainGame.GRID_COLUMNS; x++)
            {
                for(int y = 0; y < MainGame.GRID_ROWS; y++)
                {
                    //Refer to the look up dictionary for Cell type to colour to know what colour empty Cells are.
                    DrawCell(_spriteBatch, typeColourDictionary[CellType.Empty], new Vector2(x, y));
                }
            }
        }

        //Draw cell at a given position (in grid coordinates)
        public void DrawCell(SpriteBatch _spriteBatch, Color colour, Vector2 position)
        {
            //Initialise sprite drawing object (built into MonoGame).
            _spriteBatch.Begin();
            //Draw the rectangle texture at this cell's grid position multiplied by cell width to get to the screen position.
            _spriteBatch.Draw(cellTexture, position * MainGame.CELL_WIDTH, colour);
            //Apply drawing that we've done.
            _spriteBatch.End();
        }

        public void SignalCellMove(Vector2 fromPos, Vector2 toPos, CellType fromType, CellType toType, bool highlighted)
        {
            var info = new CellMoveInfo(fromPos, toPos, fromType, toType, highlighted);
            cellMoveInfoQueue.Enqueue(info);
        }
    }
}
