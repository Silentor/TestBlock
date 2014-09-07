using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Silentor.TB.Common.Maps.Geometry
{
    /// <summary>
    /// 2d array sliding window. Size is immutable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid<T> : IEnumerable<T>
    {
        public Bounds2i Bounds
        {
            get { return _bounds; }
            private set
            {
                _bounds = value;
                MinX = _bounds.Min.X;
                MaxX = _bounds.Max.X;
                MinZ = _bounds.Min.Z;
                MaxZ = _bounds.Max.Z;
            }
        }

        public int MinX { get; private set; }

        public int MinZ { get; private set; }

        public int MaxX { get; private set; }

        public int MaxZ { get; private set; }

        public readonly int SizeX;

        public readonly int SizeZ;

        public T this[int x, int z]
        {
            get { return Get(x, z); }
            set { Set(value, x, z);}
        }

        public T this[Vector2i coords]
        {
            get { return Get(coords); }
            set { Set(value, coords); }
        }


        public Grid(Bounds2i bounds)
        {
            CheckBounds(bounds);

            Bounds = bounds;

            SizeX = bounds.Size.X;
            SizeZ = bounds.Size.Z;

            _grid = new T[SizeX * SizeZ];
        }

        public void Set(T obj, Vector2i pos)
        {
            Set(obj, pos.X, pos.Z);
        }

        public void Set(T obj, int x, int z)
        {
#if DEBUG
            if(x < MinX || x > MaxX || z < MinZ || z > MaxZ)
                throw new IndexOutOfRangeException(string.Format("Some argument {0}, {1} out of grid range {2}", x, z, Bounds));
#endif

            x = (x + ZeroOffset)%SizeX;
            z = (z + ZeroOffset)%SizeZ;
            
            _grid[x * SizeZ + z] = obj;
        }

        public T Get(Vector2i pos)
        {
            return Get(pos.X, pos.Z);
        }

        public T Get(int x, int z)
        {
#if DEBUG
            if (x < MinX || x > MaxX || z < MinZ || z > MaxZ)
                throw new IndexOutOfRangeException(string.Format("Some argument {0}, {1} out of grid range {2}", x, z, Bounds));
#endif

            x = (x + ZeroOffset) % SizeX;
            z = (z + ZeroOffset) % SizeZ;

            return _grid[x * SizeZ + z];
        }

        public bool IsCorrectIndex(Vector2i pos)
        {
            return IsCorrectIndex(pos.X, pos.Z);
        }

        public bool IsCorrectIndex(int x, int z)
        {
            if (x < MinX || z < MinZ) return false;
            if (x > MaxX || z > MaxZ) return false;
            return true;
        }

        /// <summary>
        /// Slide grid, discard slided out data, slide in data set to default
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="initialize">Is initialize new elements to default?</param>
        public void Slide(Vector2i offset, bool initialize = true)
        {
            var newBounds = Bounds.Translate(offset);
            CheckBounds(newBounds);

            var oldBound = Bounds;
            Bounds = newBounds;

            if(initialize)
                foreach (var undefined in Bounds.Substract(oldBound))
                    Set(default(T), undefined);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _grid.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _grid.GetEnumerator();
        }

        private static void CheckBounds(Bounds2i bounds)
        {
            if (bounds.Min.X < MinCoord || bounds.Min.Z < MinCoord ||
               bounds.Max.X > MaxCoord || bounds.Max.Z > MaxCoord)
                throw new ArgumentOutOfRangeException("bounds", bounds, "Grid bounds out of range");
        }

        private readonly T[] _grid;     //2D array
        private Bounds2i _bounds;

        private const int ZeroOffset = int.MaxValue/2;
        private const int MinCoord = -ZeroOffset;
        private const int MaxCoord = ZeroOffset;
    }
}