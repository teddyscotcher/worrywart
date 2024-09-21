using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace WorrywartNEA2
{
    internal class Worrywart : Character
    {

        private float stressIncrease;
        private float attackRadius;

        //Do everything the same as with Character (except for the cheeky 2 variables at the end).
        public Worrywart(int id, Vector2 position, CellType type, float stressLevel, List<CellType> goTowards, List<CellType> runFrom, float neighbourhoodRadius, float stopDistance, float speed, float runAwayRadius, float stressIncrease, float attackRadius) : base(id, position, type, stressLevel, goTowards, runFrom, neighbourhoodRadius, stopDistance, speed, runAwayRadius)
        {
            this.id = id;
            this.Position = position;
            this.Type = type;
            this.stressLevel = stressLevel;
            this.runFrom = runFrom;
            this.goTowards = goTowards;
            this.NeighbourhoodRadius = neighbourhoodRadius;
            this.stopDistance = stopDistance;
            this.speed = speed;
            this.runAwayRadius = runAwayRadius;
            random = new Random();
            this.stressIncrease = stressIncrease;
            this.attackRadius = attackRadius;
        }

        public override void Update(List<Cell> cells)
        {
            //Do all the stuff a normal Character does when updating.
            base.Update(cells);
            //Find neighbourhood
            List<Cell> neighbourhood = CalculateNeighbourhood(cells);
            foreach(Cell cell in neighbourhood)
            {
                //If we've found a Student that is closer than attackRadius, give them stress!!!
                if (cell.Type == CellType.Student && Vector2.Distance(position, cell.Position) < attackRadius)
                {
                    Character ch = (Character)cell;
                    ch.StressLevel += stressIncrease;
                }
            }
        }
    }
}
