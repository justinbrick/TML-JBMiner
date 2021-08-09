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
        private static bool _isMining = false;
        private static ConcurrentDictionary<Tile, bool> _alreadyMined = new();
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            var self = Main.tile[i, j];
            if (fail || _isMining ||  !JBMiner.Instance.VeinKeybind.Current || _alreadyMined.ContainsKey(self)) return;
            _isMining = true;
            _alreadyMined.Clear();
            var newBlocks = GetNearbyBlocks(self, i, j).ToList();
            
            List<(int, int)> blocksToMine = new(newBlocks);
            while (newBlocks.Count > 0 && newBlocks.Count < Configuration.instance.BlockLimit && blocksToMine.Count < Configuration.instance.BlockLimit)
            {
                var (x, y) = newBlocks[0];
                var tile = Main.tile[x, y];
                newBlocks.AddRange(GetNearbyBlocks(tile, x, y));
                blocksToMine.Add((x,y));
                newBlocks.RemoveAt(0);
            }

            Task.Delay(1);
            foreach ((int x, int y) in blocksToMine)
            {
                WorldGen.KillTile(x,y);
            }
            _isMining = false;
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
                    if (tile is null || tile == match || _alreadyMined.ContainsKey(tile) || !SameAs(match, tile) ) continue; 
                    _alreadyMined.TryAdd(tile, true);
                    yield return (i + x, j + y);
                }
            }
        }
    }
}