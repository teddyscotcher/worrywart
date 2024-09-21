using Microsoft.Xna.Framework;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorrywartNEA2
{
    internal class Cell
    {
        //CellType definition! Essentially creating a variable type.
        public enum CellType
        {
            Empty,
            Student,
            Worrywart,
            Wall,
            Library
        }

        //position, type, unique id, and neighbourhood radius (private and public w/ getters and setters)
        protected int id;
        
        public int Id { get { return id; } set { id = value; } }

        protected Vector2 position;
        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }
        protected CellType type { get; set; }
        public CellType Type
        {
            get { return type; }
            set { type = value; }
        }
        protected float neighbourhoodRadius;

        public float NeighbourhoodRadius
        {
            get { return neighbourhoodRadius; }
            set { neighbourhoodRadius = value; }
        }

        public Cell() { }

        //constructor w/ no neighbourhood radius (shorthand for when we don't need neighbourhood radius like with Walls)
        public Cell(int id, Vector2 position, CellType type)
        {
            this.id = id;
            this.position = position;
            this.type = type;
            this.neighbourhoodRadius = 0;
        }

        //Normal constructor
        public Cell(int id, Vector2 position, CellType type, float neighbourhoodRadius)
        {
            this.id = id;
            this.position = position;
            this.type = type;
            this.neighbourhoodRadius = neighbourhoodRadius;
        }

        //Update function ready to be overriden by subclasses if we need it.
        public virtual void Update(List<Cell> cells) { }

        //Calculating all cells within neighbourhoodRadius (other than this cell)
        protected List<Cell> CalculateNeighbourhood(List<Cell> cells)
        {
            List<Cell> neighbours = new List<Cell>();
            foreach(Cell cell in cells)
            {
                if(Vector2.Distance(cell.position, position) < neighbourhoodRadius && cell.id != id)
                {
                    neighbours.Add(cell);
                }
            }
            return neighbours;
        }
    }
}
