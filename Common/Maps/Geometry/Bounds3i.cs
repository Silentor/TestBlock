using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Silentor.TB.Common.Maps.Geometry
{
    public struct Bounds3i : IEnumerable<Vector3i>
    {
        private Vector3i _min;
        private Vector3i _max;
        //private static readonly Vector3 BoundsVector = new Vector3(0.5f, -0.5f, -0.5f);

        public Bounds3i(Vector3i min, Vector3i max) : this()
        {
            _min = min;
            _max = max;

            TestMinMax();
        }

        public Bounds3i(Vector3i center, int extends) : this(center - Vector3i.One * extends, center + Vector3i.One * extends)
        {
        }

        public Vector3i Max
        {
            get { return _max; }
            set
            {
                _max = value; TestMinMax();
            }
        }

        public Vector3i Min
        {
            get { return _min; }
            set
            {
                _min = value; TestMinMax();
            }
        }

        public bool Contains(Vector3i pos)
        {
            return pos.X >= Min.X && pos.X <= Max.X &&
                   pos.Y >= Min.Y && pos.Y <= Max.Y &&
                   pos.Z >= Min.Z && pos.Z <= Max.Z 
                ;
        }

        /// <summary>
        /// Inclusive enumeration of bound
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Vector3i> GetEnumerator()
        {
            for (var x = Min.X; x <= Max.X; x++)
                for (var y = Min.Y; y <= Max.Y; y++)          
                    for (var z = Min.Z; z <= Max.Z; z++)
                        yield return new Vector3i(x, y, z);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Bounds3i(min = {0}, max = {1})", Min, Max);
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() ^ (Max.GetHashCode() << 2);
        }

        public override bool Equals(object other)
        {
            if (!(other is Bounds3i)) return false;
            var b = (Bounds3i)other;
            return Min == b.Min && Max == b.Max;
        }

        public static bool operator ==(Bounds3i a, Bounds3i b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(Bounds3i a, Bounds3i b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        /// <summary>
        /// Translate bounds by vector
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bounds3i operator +(Bounds3i a, Vector3i b)
        {
            return new Bounds3i(a.Min + b, a.Max + b);
        }

        /// <summary>
        /// Translate bounds by vector
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bounds3i operator -(Bounds3i a, Vector3i b)
        {
            return new Bounds3i(a.Min - b, a.Max - b);
        }

        public IEnumerable<Vector3i> Substract(Bounds3i b)
        {
            return this.Where(v => !b.Contains(v));
        }

        private void TestMinMax()
        {
            if (Min.X > Max.X || Min.Y > Max.Y || Min.Z > Max.Z) throw new ArgumentException(String.Format("Min {0} greater then Max {1}", Min, Max));
        }
    }
}
