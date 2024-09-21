using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace WorrywartNEA2
{
    internal static class Utility
    {
        public static Vector2 ConfineToGrid(Vector2 v)
        {
            //Round coords to integers.
            v.X = MathF.Round(v.X);
            v.Y = MathF.Round(v.Y);

            //Constrict X and Y of the vector so they're on the grid.
            if (v.X >= MainGame.GRID_COLUMNS) v.X = MainGame.GRID_COLUMNS - 1;
            if (v.X < 0) v.X = 0;
            if (v.Y >= MainGame.GRID_ROWS) v.Y = MainGame.GRID_ROWS - 1;
            if (v.Y < 0) v.Y = 0;
            return v;
        }
        public static bool InGrid(Vector2 v)
        {
            //Check if the vector is within the bounds of the game grid or not.
            if (v.X >= MainGame.GRID_COLUMNS || v.X < 0 || v.Y >= MainGame.GRID_ROWS || v.Y < 0) return false;
            else return true;
        }
    }
}
