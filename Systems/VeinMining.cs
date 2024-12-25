using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace JBMiner.Systems
{
    public class VeinMining : GlobalTile
    {
        private static bool _isMining;
        private static ConcurrentDictionary<(int, int), bool> _alreadyMined = new();
        
        public override void KillTile(
            int i,
            int j,
            int type,
            ref bool fail,
            ref bool effectOnly,
            ref bool noItem)
        {
            var tile = Main.tile[i, j];
            if (fail || _isMining || _alreadyMined.ContainsKey((i, j)) || Main.worldName == string.Empty)
                return;
            switch (Main.netMode)
            {
                case 0:
                    if (!JBMiner.Instance.VeinMine.Current || !Configuration.Instance.Whitelist.Contains(tile.TileType))
                        break;
                    DestroyNear(i, j);
                    break;
                case 1:
                    if (!JBMiner.Instance.VeinMine.Current || !Configuration.Instance.Whitelist.Contains(tile.TileType))
                        break;
                    var packet = JBMiner.Instance.GetPacket();
                    packet.Write((byte) 0);
                    packet.WriteVector2(new Vector2(i, j));
                    packet.Send();
                    break;
            }
        }
        public static void DestroyNear(int i, int j)
        {
            Tile match = Main.tile[i, j];
            _alreadyMined.TryAdd((i, j), true);
            _isMining = true;
            int i1 = i;
            int j1 = j;
            List<(int, int)> list = GetNearbyBlocks(match, i1, j1).ToList();
            for (int index = 0; index < list.Count && list.Count < Configuration.Instance.BlockLimit; ++index)
            {
                (int i2, int j2) = list[index];
                foreach ((int, int) nearbyBlock in GetNearbyBlocks(Main.tile[i2, j2], i2, j2))
                {
                    if (list.Count < Configuration.Instance.BlockLimit)
                        list.Add(nearbyBlock);
                }
            }
            foreach ((int num1, int num2) in list)
            {
                WorldGen.KillTile(num1, num2);
                if (Main.netMode != 0)
                    NetMessage.SendData(17, number2: (float) num1, number3: (float) num2);
            }
            VeinMining._isMining = false;
            VeinMining._alreadyMined.Clear();
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
                    if (tile == match || _alreadyMined.ContainsKey((i+x,j+y)) || !tile.SameAs(match) ) continue; 
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
            // Previous: IsActive, HasTile likely candidate to replace.
            return first.HasTile && second.HasTile && first.TileType == second.TileType;
        }
    }
}