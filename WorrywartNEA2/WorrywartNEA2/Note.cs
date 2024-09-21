using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace WorrywartNEA2
{
    
    internal class Note : Cell
    {
        private Vector2 target;
        private bool deleteMe;

        public Note(int id, Vector2 position, CellType type, float speed, Vector2 target, bool deleteMe) : base(id, position, type)
        {
            this.id = id;
            this.position = position;
            this.type = type;
            this.neighbourhoodRadius = 0;
        }

        public override void Update(List<Cell> cells)
        {
            base.Update(cells);
            if (position == target) deleteMe = true;
            foreach(Cell c in cells)
            {
                if (c.Position == position)
                {
                    deleteMe = true;
                    break;
                }
            }
        }
    }
}
