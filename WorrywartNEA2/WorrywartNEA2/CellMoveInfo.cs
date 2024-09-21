using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WorrywartNEA2.Cell;

namespace WorrywartNEA2
{
    internal class CellMoveInfo
    {
        //Object to give information to WorldDrawer about a cell movement to be drawn.
        public Vector2 fromPos;
        public Vector2 toPos;
        public CellType fromType;
        public CellType toType;
        public bool highlighted;

        public CellMoveInfo(Vector2 fromPos, Vector2 toPos, CellType fromType, CellType toType, bool highlighted)
        {
            this.fromPos = fromPos;
            this.toPos = toPos;
            this.fromType = fromType;
            this.toType = toType;
            this.highlighted = highlighted;
        }
    }
}
