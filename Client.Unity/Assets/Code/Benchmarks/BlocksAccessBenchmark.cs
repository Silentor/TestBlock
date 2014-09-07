using System.Collections;
using System.Diagnostics;
using Silentor.TB.Common.Maps.Blocks;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class BlocksAccessBenchmark : MonoBehaviour 
    {
        private BlockData[] _blockDatas = new BlockData[16*16*256];
        private Block _block = new Block(1);
        public static BlockData result;

        private const int SizeXBits = 4;
        private const int SizeX = 1 << SizeXBits;
        private const int SizeYBits = 8;
        private const int SizeY = 1 << SizeYBits;
        private const int SizeZBits = 4;
        private const int SizeZ = 1 << SizeZBits;

        // Use this for initialization
        void Start () 
        {
            print("Stopwatch: freq " + Stopwatch.Frequency + ", is high resolution " + Stopwatch.IsHighResolution);
            var bd = new BlockData(_block);

            //Warm up
            for (var x = 0; x < SizeX; x++)
                for (var y = 0; y < SizeY; y++)
                    for (var z = 0; z < SizeZ; z++)
                    {
                        SetBlocksShift(x, y, z, bd);
                    }

            UnityEngine.Debug.Log("Start bench");
            StartCoroutine(Bench());
        }

        IEnumerator Bench()
        {
            var block = new BlockData(_block);

            while (true)
            {
                foreach (var p in BenchSet(block)) yield return p;
                foreach (var p in BenchGet()) yield return p;
            }
        }

        private IEnumerable BenchSet(BlockData block)
        {
            var sw = new Stopwatch();

            sw.Start();
            for (var i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            SetBlocksMult(x, y, z, block);
            print("Set mult " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (var i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            SetBlocksShift(x, y, z, block);
            print("Set shift " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (var i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            _blockDatas[GetCoordMult(x, y, z)] = block;
            print("Set coords mult " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (var i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            _blockDatas[GetCoordShift(x, y, z)] = block;
            print("Set coords shift " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (int i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            _blockDatas[y + SizeY*x + SizeY*SizeX*z] = block;
            print("Set directly mult " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (int i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            _blockDatas[y + (x << SizeYBits) + (z << SizeYBits + SizeXBits)] = block;
            print("Set directly shift " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;
        }

        private IEnumerable BenchGet()
        {
            var sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            result = GetBlocksMult(x, y, z);
            print("Get mult " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (int i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            result = GetBlocksShift(x, y, z);
            print("Get shift " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (int i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            result = _blockDatas[GetCoordMult(x, y, z)];
            print("Get coords mult " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (int i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            result = _blockDatas[GetCoordShift(x, y, z)];
            print("Get coords shift " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (int i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            result = _blockDatas[y + SizeY * x + SizeY * SizeX * z];
            print("Get directly mult " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;

            sw.Start();
            for (int i = 0; i < 100; i++)
                for (var x = 0; x < SizeX; x++)
                    for (var y = 0; y < SizeY; y++)
                        for (var z = 0; z < SizeZ; z++)
                            result = _blockDatas[y + (x << SizeYBits) + (z << SizeYBits + SizeXBits)];
            print("Get directly shift " + sw.ElapsedMilliseconds);
            sw.Reset();
            yield return null;
        }

        void SetBlocksMult(int x, int y, int z, BlockData block)
        {
            _blockDatas[y + SizeY*x + SizeY*SizeX*z] = block;
        }

        BlockData GetBlocksMult(int x, int y, int z)
        {
            return _blockDatas[y + SizeY * x + SizeY * SizeX * z];
        }

        void SetBlocksShift(int x, int y, int z, BlockData block)
        {
            _blockDatas[y + (x << SizeYBits) + (z << SizeYBits + SizeXBits)] = block;
        }

        BlockData GetBlocksShift(int x, int y, int z)
        {
            return _blockDatas[y + (x << SizeYBits) + (z << SizeYBits + SizeXBits)];
        }

        int GetCoordMult(int x, int y, int z)
        {
            return y + SizeY*x + SizeY*SizeX*z;
        }

        int GetCoordShift(int x, int y, int z)
        {
            return y + (x << SizeYBits) + (z << SizeYBits + SizeXBits);
        }

    }
}
