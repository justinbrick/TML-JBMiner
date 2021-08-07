﻿using System;
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
        private static ConcurrentDictionary<Tile, bool> _alreadyMined = new();
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            var self = Main.tile[i, j];
            if (fail || !JBMiner.Instance.VeinKeybind.Current || _alreadyMined.ContainsKey(self)) return;
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
            // Now finally break each block.
            _ = DestroyTiles(blocksToMine);
        }

        public override void Unload()
        {
            _alreadyMined = null;
        }

        private async Task DestroyTiles(List<(int, int)> tiles)
        {
            foreach ((int x, int y) in tiles)
            {
                await Task.Delay(1);
                WorldGen.KillTile(x,y);
            }
        }

        internal IEnumerable<(int, int)> GetNearbyBlocks(Tile match, int i, int j)
        {
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    var tile = Main.tile[i + x, j + y];
                    if (x + y == 0 || tile is null || _alreadyMined.ContainsKey(tile) || !tile.IsTheSameAs(match)) continue;
                    _alreadyMined.TryAdd(tile, true);
                    yield return (i + x, j + y);
                }
            }
        }
    }
}