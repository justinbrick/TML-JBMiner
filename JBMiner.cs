using JBMiner.Systems;
using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace JBMiner
{
	public class JBMiner : Mod
	{
		internal ModKeybind VeinMine;
		internal ModKeybind WhitelistAdd;
		internal ModKeybind WhitelistRemove;
		
		internal static JBMiner Instance;
		
		public override void Load()
		{
			base.Load();
			Instance = this;
			VeinMine = KeybindLoader.RegisterKeybind(this, "Vein Mine", Keys.OemTilde);
			WhitelistAdd = KeybindLoader.RegisterKeybind(this, "Add to Whitelist", Keys.NumPad0);
			WhitelistRemove = KeybindLoader.RegisterKeybind(this, "Remove from Whitelist", Keys.NumPad1);
		}
		
		public override void Unload()
		{
			Configuration.Instance = null; // Remove the configuration instance.
			Instance = null;
			base.Unload();
		}
	}
}