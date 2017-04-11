using Evertop.Soccer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvertopSoccerAI
{
    /// <summary>
    /// Same as Position but with additional overriden methods - Equals and GetHashCode, needed for Dictionary operations
    /// </summary>
    public class Position2
    {
        public Position2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Position2))
                return false;

            return ((Position2)obj).X == X && ((Position2)obj).Y == Y;
        }

        public override int GetHashCode()
        {
            return (X + Y).GetHashCode();
        }
    }
}