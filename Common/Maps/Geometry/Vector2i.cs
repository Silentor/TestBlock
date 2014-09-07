//using UnityEngine;

namespace Silentor.TB.Common.Maps.Geometry
{
    public struct Vector2i 
    {
        public readonly int X, Z;
	
        public static readonly Vector2i Zero = new Vector2i(0,0);
        public static readonly Vector2i One = new Vector2i(1, 1);
        public static readonly Vector2i Forward = new Vector2i(0, 1);
        public static readonly Vector2i Back = new Vector2i(0, -1);
        public static readonly Vector2i Left = new Vector2i(-1, 0);
        public static readonly Vector2i Right = new Vector2i(1, 0);
	
        public static readonly Vector2i[] Directions =
        {
            Forward, Back, Right, Left, 
        };

        /// <summary>
        /// ������� �����
        /// </summary>
        public static readonly Vector2i[] Cardinals = Directions;

        public static readonly Vector2i[] DiagonalDirections = new[] 
        {
            Left + Forward, Right + Forward,
            Left + Back, Right + Back,
        };

        public Vector2i(int x, int z) {
            this.X = x;
            this.Z = z;
        }

        public static int DistanceSquared(Vector2i a, Vector2i b) {
            int dx = b.X-a.X;
            int dz = b.Z-a.Z;
            return dx*dx + dz*dz;
        }

        //public static int RoughDistance(Vector2i a, Vector2i b) {
        //    int dx = Mathf.Abs(b.X - a.X);
        //    int dz = Mathf.Abs(b.Z - a.Z);
        //    return Mathf.Max(dx, dz);
        //}
	
        //public static int LightDistance(Vector2i a, Vector2i b)
        //{
        //    int dx = Mathf.Abs(b.X - a.X);
        //    int dz = Mathf.Abs(b.Z - a.Z);
        //    return dx + dz;
        //}
	
        public int DistanceSquared(Vector2i v) 
        {
            return DistanceSquared(this, v);
        }
	
        public override int GetHashCode () 
        {
            return X.GetHashCode () ^ Z.GetHashCode () << 16;
        }
	
        public override bool Equals(object other)
        {
            if (!(other is Vector2i)) return false;
            var vector = (Vector2i) other;
            return X == vector.X && 
                   Z == vector.Z;
        }
	
        public override string ToString() {
            return "Vector2i("+X+", "+Z+")";
        }

        public static bool operator == (Vector2i a, Vector2i b) {
            return a.X == b.X && 
                   a.Z == b.Z;
        }
	
        public static bool operator != (Vector2i a, Vector2i b) {
            return a.X != b.X ||
                   a.Z != b.Z;
        }
	
        public static Vector2i operator - (Vector2i a, Vector2i b) 
        {
            return new Vector2i(a.X - b.X, a.Z - b.Z);
        }
	
        public static Vector2i operator + (Vector2i a, Vector2i b) 
        {
            return new Vector2i(a.X + b.X, a.Z + b.Z);
        }

        public static Vector2i operator *(Vector2i a, int b)
        {
            return new Vector2i(a.X * b, a.Z * b);
        }

        public static Vector2i operator /(Vector2i a, int b)
        {
            return new Vector2i(a.X / b, a.Z / b);
        }

        //public static explicit operator Vector2(Vector2i v) 
        //{
        //    return new Vector2(v.X, v.Z);
        //}

        //public static explicit operator Vector2i(Vector2 v)
        //{
        //    return new Vector2i((int)v.x, (int)v.y);
        //}

        //public static Vector2i Unity2MapPosition(Vector2 position)
        //{
        //    return new Vector2i(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        //}

        //public static Vector2 Map2UnityPosition(Vector2i position)
        //{
        //    return (Vector2) position;// new Vector2(-position.X, position.Z);
        //}

        //public static explicit operator CardinalDirections(Vector2i direction)
        //{
        //    if(direction == Zero) return CardinalDirections.None;

        //    if (Mathf.Abs(direction.Z) >= Mathf.Abs(direction.X))
        //    {
        //        if (direction.Z >= 0)
        //            return CardinalDirections.North;
        //        else
        //            return CardinalDirections.South;
        //    }
        //    else
        //    {
        //        if (direction.X >= 0)
        //            return CardinalDirections.West;
        //        else
        //            return CardinalDirections.East;
        //    }
        //}

   
    }
}
