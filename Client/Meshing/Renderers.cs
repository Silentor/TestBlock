using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Silentor.TB.Common.Maps.Blocks;

namespace Silentor.TB.Client.Meshing
{
    public interface IBlockMeshers : IEnumerable<SimplestBlockMesher>
    {    
    }

    /// <summary>
    /// Map blocks from blockset to block BlockMeshers
    /// </summary>
    public class BlockMeshers : IBlockMeshers
    {
        private readonly List<SimplestBlockMesher> _renderers = new List<SimplestBlockMesher>();

        public BlockMeshers(IBlocksAtlas atlas, IBlockSet blockSet)
        {
            Add(new SimplestBlockMesher(atlas, blockSet[BlockSet.StoneID], "stone"));
            Add(new SimplestBlockMesher(atlas, blockSet[BlockSet.DirtID], "dirt"));
            Add(new SimplestBlockMesher(atlas, blockSet[BlockSet.DirtWithGrassID], "grass_side"));
            //Add(new SymmetricBlockRenderer(atlas, blockSet[BlockSet.DirtWithGrassID], "grass_side", "grass_top", "dirt"));
            //_renderers[BlockSet.REEDS_ID] = new BlockRenderer(map, BlockSet.Instance[BlockSet.DIRT_WITH_GRASS_ID], "reeds", isTransparent: true);
        }

        private void Add(SimplestBlockMesher mesher)
        {
            if (mesher == null) throw new ArgumentNullException("mesher");

            if (_renderers.Any(r => r.Block == mesher.Block))
                throw new InvalidOperationException("BlockMeshers for block id" + mesher.Block.Id + "already added");

            _renderers.Add(mesher);
        }

        public IEnumerator<SimplestBlockMesher> GetEnumerator()
        {
            return _renderers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
