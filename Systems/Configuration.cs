using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using ReLogic.OS;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;


namespace JBMiner.Systems
{
    public class Configuration : ModConfig
    {
        [System.Text.Json.Serialization.JsonIgnore] internal static Configuration Instance;
        [System.Text.Json.Serialization.JsonIgnore] public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(500)] public int BlockLimit; // This is the max amount of blocks we can break at any given time.
        [Newtonsoft.Json.JsonIgnore] private const string WhitelistFileName = "JBMinerWhitelist";
        public static string FolderPath(string place) => Path.Combine(Platform.Get<IPathService>().GetStoragePath(), "Terraria", "ModLoader", "Beta", place);

        public override bool Autoload(ref string name)
        {
            name = "Configuration";
            Instance ??= this;
            if (File.Exists(FolderPath(WhitelistFileName)))
            {
                using var str = File.OpenText(FolderPath(WhitelistFileName));
                Whitelist = JsonSerializer.Deserialize<List<int>>(str.ReadToEnd());
            }
            else
            {
                Whitelist = DefaultWhitelist;
                using var writer = new StreamWriter(File.Open(FolderPath(WhitelistFileName), FileMode.Create));
                writer.Write(JsonSerializer.Serialize(Whitelist)); // For some reason it glitches if we don't serialize and save as soon as we get this? And then it turns into null which isn't good.
            }
            return base.Autoload(ref name);
        }

        // Unload so that we can properly fix the stream.
        public void Unload()
        {
            using var str = new StreamWriter(File.Open(FolderPath(WhitelistFileName), FileMode.Create));
            str.Write(JsonSerializer.Serialize(Whitelist)); // This makes the whitelist a string.
        }

        // This whitelist contains all the blocks that we're currently looking for while mining.
        [Newtonsoft.Json.JsonIgnore]
        public List<int> Whitelist;
        
        [Newtonsoft.Json.JsonIgnore]
        internal static readonly List<int> DefaultWhitelist = new()
        {
            6, 7, 8,
            9, 22, 62,
            63, 64, 65,
            66, 67, 68,
            107, 108, 111,
            123, 166, 167,
            168, 169, 204,
            211, 221, 222,
            223
        };
    }
}