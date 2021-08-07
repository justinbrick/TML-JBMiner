using JBMiner.Systems;
using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace JBMiner
{
	public class JBMiner : Mod
	{
		internal ModKeybind VeinKeybind;
		internal static JBMiner Instance;
		
		public override void Load()
		{
			base.Load();
			Instance = this;
			VeinKeybind = KeybindLoader.RegisterKeybind(this as Mod, "Vein Mine", Keys.OemTilde);
		}

		public override void Unload()
		{
			Configuration.instance = null; // Remove the configuration instance.
			Instance = null;
			base.Unload();
		}
	}
}