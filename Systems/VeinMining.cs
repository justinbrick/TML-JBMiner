using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
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
            if (fail || _isMining ||  !JBMiner.Instance.VeinMine.Current || _alreadyMined.ContainsKey((i,j)) || !Configuration.Instance.Whitelist.Contains(self.type)) return;
            _isMining = true;
            var blocksToMine = GetNearbyBlocks(self, i, j).ToList();
            int index = 0;
            while (index < blocksToMine.Count && blocksToMine.Count < Configuration.Instance.BlockLimit)
            {
                var (x, y) = blocksToMine[index];
                var tile = Main.tile[x, y];
                foreach (var block in GetNearbyBlocks(tile, x, y))
                    if (blocksToMine.Count < Configuration.Instance.BlockLimit)
                        blocksToMine.Add(block);
                ++index;
            }

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
        
        internal static IEnumerable<(int, int)> GetNearbyBlocks(Tile match, int i, int j)
        {
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    var tile = Main.tile[i + x, j + y];
                    if (tile is null || tile == match || _alreadyMined.ContainsKey((i+x,j+y)) || !tile.SameAs(match) ) continue; 
                    _alreadyMined.TryAdd((i+x,j+y), true);
                    yield return (i + x, j + y);
                }
            }
        }
    }

    public static class TileExtension
    {
        public static bool SameAs(this Tile first, Tile second)
        {
            return first.IsActive && second.IsActive && first.type == second.type;
        }
    }
}