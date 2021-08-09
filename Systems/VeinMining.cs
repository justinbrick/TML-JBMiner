using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace JBMiner.Systems
{
    public class VeinMining : GlobalTile
    {
        private static bool _isMining;
        private static ConcurrentDictionary<(int, int), bool> _alreadyMined = new();
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            var self = Main.tile[i, j];
            if (fail || _isMining ||  !JBMiner.Instance.VeinKeybind.Current || _alreadyMined.ContainsKey((i,j))) return;
            _isMining = true;
            var blocksToMine = GetNearbyBlocks(self, i, j).ToList();
            int index = 0;
            while (index < blocksToMine.Count && blocksToMine.Count < Configuration.instance.BlockLimit)
            {
                var (x, y) = blocksToMine[index];
                var tile = Main.tile[x, y];
                foreach (var block in GetNearbyBlocks(tile, x, y))
                    if (blocksToMine.Count < Configuration.instance.BlockLimit)
                        blocksToMine.Add(block);
                ++index;
            }

            Main.NewText(blocksToMine.Count);
            foreach ((int x, int y) in blocksToMine)
            {
                WorldGen.KillTile(x,y);
            }
            _isMining = false;
            _alreadyMined.Clear();
        }

        public override void Unload()
        {
            _alreadyMined = null;
        }

        private static bool SameAs(Tile first, Tile second)
        {
            return first.IsActive && second.IsActive && first.type == second.type;
        }

        internal IEnumerable<(int, int)> GetNearbyBlocks(Tile match, int i, int j)
        {
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    var tile = Main.tile[i + x, j + y];
                    if (tile is null || tile == match || _alreadyMined.ContainsKey((i+x,j+y)) || !SameAs(match, tile) ) continue; 
                    _alreadyMined.TryAdd((i+x,j+y), true);
                    yield return (i + x, j + y);
                }
            }
        }
    }
}