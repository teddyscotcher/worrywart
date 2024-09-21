using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorrywartNEA2
{
    internal class Nerd : Character
    {
        public Nerd(int id, Vector2 position, CellType type, float stressLevel, List<CellType> goTowards, List<CellType> runFrom, float neighbourhoodRadius, float stopDistance, float thinkRadius, float runAwayRadius, float timeBetweenThrows) : base(id, position, type, stressLevel, goTowards, runFrom, neighbourhoodRadius, stopDistance, thinkRadius, runAwayRadius)
        {
            this.id = id;
        }
    }
}
