using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace WorrywartNEA2
{
    internal class Node
    {
        public Vector2 position;
        public int f;
        public int g;
        public int h;
        public Node parent;


    }
}
