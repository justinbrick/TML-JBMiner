using System.IO;
using JBMiner.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;

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

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (reader.ReadByte() != 0)
                return;
            var vector2 = reader.ReadVector2();
            VeinMining.DestroyNear((int) vector2.X, (int) vector2.Y);
            NetMessage.SendData(7);
        }

        public override void Unload()
        {
            Configuration.Instance.Unload(); 
            Configuration.Instance = null; // Remove the configuration instance.
            Instance = null;
            base.Unload();
        }
    }
}