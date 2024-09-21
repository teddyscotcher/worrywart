using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WorrywartNEA2.Cell;
using Microsoft.Xna.Framework;

namespace WorrywartNEA2
{
    internal class Library : Cell
    {
        private float effectRadius;
        private float effectStrength;
        
        public float EffectRadius { get { return effectRadius; } set { effectRadius = value; } }
        public float EffectStrength { get { return effectStrength; } set { effectStrength = value; } }

        public Library(int id, Vector2 position, CellType type, float effectRadius, float effectStrength) : base(id, position, type)
        {
            this.id = id;
            this.position = position;
            this.type = type;
            this.effectRadius = effectRadius;
            this.effectStrength = effectStrength;
        }

        public override void Update(List<Cell> cells)
        {
            base.Update(cells);
            foreach (Cell cell in cells)
            {
                //If we've got a character that's closer than effectRadius
                if ((cell.Type == CellType.Student || cell.Type == CellType.Worrywart) && Vector2.Distance(position, cell.Position) < effectRadius)
                {
                    Character ch = (Character)cell;
                    //Decrease their stress (not going past 0)
                    if(ch.StressLevel > 0)
                    {
                        ch.StressLevel -= effectStrength;
                    } else if(ch.StressLevel < 0)
                    {
                        ch.StressLevel = 0;
                    }
                    
                }
            }
        }

    }
}
