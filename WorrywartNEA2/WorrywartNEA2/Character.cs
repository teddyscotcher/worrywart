using Microsoft.Xna.Framework;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace WorrywartNEA2
{
    internal class Character : Cell
    {
        //Where we're going
        protected Vector2 targetPos;

        //Factors that help us decide where to go.
        protected float stopDistance;
        protected float runAwayRadius;
        protected List<CellType> runFrom;
        protected List<CellType> goTowards;

        //How fast we go there
        protected float speed;

        //Stress!!
        protected float stressLevel;

        public float StressLevel { get { return stressLevel; } set { stressLevel = value; } }

        //So we can generate random stuff
        protected Random random;

        //Vector to indicate to wander
        private readonly Vector2 NO_MOVE = new Vector2(-1, -1);

        //Randomly generated name
        public string Name { get { return name; } set { name = value; } }

        private string name;

        //Possible names
        public static readonly List<string> FIRST_NAMES = new List<string>{
            "Sally",
            "James",
            "Jim",
            "Johnny",
            "Brennan",
            "Brandon",
            "Fred",
            "Sandra",
            "Ulysses",
            "Chang",
            "Michelle",
            "Joanne",
            "Fela",
            "Ify",
            "Dan",
            "Tristan",
            "Plort",
            "Robert",
            "Alison",
            "Simon",
            "Samuel",
            "Abraham",
            "Yramb",
            "Elliott",
            "Brian",
            "Carole",
            "Spongebob",
            "Ai",
            "Janja",
            "Adam",
            "Mejdi",
            "Jamie",
            "Anton",
            "Ruth",
            "Martin"
        };

        public static readonly List<string> LAST_NAMES = new List<string>{
            "Jenkins",
            "Grant",
            "Trapp",
            "Jones",
            "Thomas",
            "Kuti",
            "Smith",
            "Gilbert",
            "Taylor",
            "King",
            "Reed",
            "Lincoln",
            "Squarepants",
            "Mori",
            "Vandermeer",
            "Garnbret",
            "Ondra",
            "Schalk",
            "Forster",
            "Kopanitsa",
            "Kemp",
            "Gibbs"
        };


        public Character(int id, Vector2 position, CellType type, float stressLevel, List<CellType> goTowards, List<CellType> runFrom, float neighbourhoodRadius, float stopDistance,float speed, float runAwayRadius) : base(id, position, type)
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

            this.Name = GenerateName();
        }


        //Pick a random first and last name and return it
        private string GenerateName()
        {
            int fNameIndex = (int)random.NextInt64(0, FIRST_NAMES.Count());
            int lNameIndex = (int)random.NextInt64(0, LAST_NAMES.Count());

            return FIRST_NAMES[fNameIndex] + " " + LAST_NAMES[lNameIndex];
        }



        public override void Update(List<Cell> cells)
        {
            //Find neighbourhood and then find target based on this neighbourhood.
            List<Cell> neighbourhood = CalculateNeighbourhood(cells);
            Vector2 targetPos =  CalculateDestination(neighbourhood);
            
            //If the target comes back as the special vector, wander randomly.
            if (targetPos == NO_MOVE)
            {
                position += WanderVector(neighbourhood);
            }
            else
            {
                //Force the targetPos onto the grid and move to it (if it's not occupied!)
                targetPos = Utility.ConfineToGrid(targetPos);
                bool targetOccupied = false;
                foreach(Cell c in cells)
                {
                    if (c.Position == targetPos) targetOccupied = true;
                }
                if(!targetOccupied) position = targetPos;
            }
        }

        protected Vector2 WanderVector(List<Cell> neighbourhood)
        {
            bool flag = false;
            int bailCount = 0;
            Vector2 randomMove = Vector2.Zero;
            while (!flag && bailCount < 30)
            {
                random.Next();
                //Take a random real number between 0 and 1, double it to make it between 0 and 2, round it to get 0, 1, or 2, then subtract 1 to get -1, 0, or 1.
                int x = (int)(Math.Round(random.NextDouble() * 2) - 1);
                int y = (int)(Math.Round(random.NextDouble() * 2) - 1);

                //Check if moving along the random vector will cause us to collide with another cell. If it does, then try again.
                randomMove = new Vector2(x, y);
                Vector2 newPos = position + randomMove;
                if (Utility.InGrid(newPos) == false) continue;
                flag = true;
                foreach (Cell c in neighbourhood)
                {
                    if (c.Position == newPos)
                    {
                        flag = false;
                        break;
                    }
                }
                bailCount++;
            }
            //If we didn't successfully find a place to wander to, move the cell far, far, away (hopefully this only happens
            //when the game is so crowded that the player won't notice a few cheeky missing Characters.
            if(bailCount > 30)
            {
                position = new Vector2(999, 999);
            }
            return randomMove;
        }

        //Calculating the vector to run away from things.
        protected Vector2 CalculateAwayVector(List<Cell> neighbourhood)
        {
            Vector2 awayVector = Vector2.Zero;
            int awayCount = 0;

            
            foreach (Cell c in neighbourhood)
            {
                //Add the vector v between this cell and each angry cell divided by |v|^2 to get a vector to run away along.
                if (runFrom.Contains(c.Type))
                {
                    Vector2 posDifference = position - c.Position;
                    float d = posDifference.Length();
                    posDifference /= d * d;
                    awayVector += posDifference;
                    awayCount++;
                }
            }
            return awayVector;
            
        }

        //This function just finds the closest Cell that has its type in the filter.
        protected Vector2 CalculateClosestVector(List<Cell> neighbourhood, List<CellType> filter)
        {
            //
            Vector2 towardsVector = Vector2.Zero;
            float shortestDistance = float.PositiveInfinity;

            foreach (Cell c in neighbourhood)
            {
                if (filter.Contains(c.Type))
                {
                    Vector2 posDifference = c.Position - position;
                    float d = posDifference.Length();
                    
                    if (d < shortestDistance)
                    {
                        towardsVector = posDifference;
                        shortestDistance = d;
                    }
                } 
                
            }
            return towardsVector;

        }

        //Calculates goal position, this is pumped into the A* class and sent back into the Cell as a route (list of Vector2)
        public Vector2 CalculateDestination(List<Cell> neighbourhood)
        {
            //Calculate vectors to closest Chaser and Target.
            Vector2 closestTargetVector = CalculateClosestVector(neighbourhood, goTowards);
            Vector2 closestChaserVector = CalculateClosestVector(neighbourhood, runFrom);
            //Magnitude of these vectors
            float t = closestTargetVector.Length();
            float a = closestChaserVector.Length();
            //If there's a chaser nearby (and they're closer than runAwayRadius, or if they're just closer than the closest Target, or if there is no Target), then run away.
            if( a > 0 && (a < runAwayRadius || a < t || t == 0) )
            {
                return position + Vector2.Round(Vector2.Normalize(CalculateAwayVector(neighbourhood)) * speed);
            }
            //If we didn't run away before and the nearest Target is farther away than stopDistance (and either there is no Chaser or this target is closer than the nearest Chaser), then run to Target.
            else if( (t < a || (t > 0 && a == 0)) && t > stopDistance)
            {
                Vector2 targetPos = position + Vector2.Normalize(closestTargetVector) * speed;
                return targetPos; 
            }
            else
            {
                return NO_MOVE;
            }
            
        }
    }
}
