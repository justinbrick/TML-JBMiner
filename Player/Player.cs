using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Configuration = JBMiner.Systems.Configuration;

namespace JBMiner.Player
{
    public class Player : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            base.ProcessTriggers(triggersSet);
            if (JBMiner.Instance.WhitelistAdd.JustPressed)
            {
                var position = Main.MouseWorld.ToTileCoordinates();
                var tile = Main.tile[position.X, position.Y];
                if (Configuration.Instance.Whitelist.Contains(tile.type)) return;
                Configuration.Instance.Whitelist.Add(tile.type);
                Main.NewText($"Added tile, ID: {tile.type}");
            }
            else if (JBMiner.Instance.WhitelistRemove.JustPressed)
            {
                var position = Main.MouseWorld.ToTileCoordinates();
                var tile = Main.tile[position.X, position.Y];
                if (!Configuration.Instance.Whitelist.Contains(tile.type)) return;
                Configuration.Instance.Whitelist.Remove(tile.type);
                Main.NewText($"Removed tile, ID: {tile.type}");
            }
        }
    }
}