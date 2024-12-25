using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using ReLogic.OS;
using Terraria.ModLoader.Config;

namespace JBMiner.Systems
{
    public class Configuration : ModConfig
    {
        [System.Text.Json.Serialization.JsonIgnore] internal static Configuration Instance;
        [System.Text.Json.Serialization.JsonIgnore] public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(500)] public int BlockLimit; // This is the max amount of blocks we can break at any given time.
        [Newtonsoft.Json.JsonIgnore] private const string WhitelistFileName = "JBMinerWhitelist";

        private static string StorageDirectory = Path.Combine(Platform.Get<IPathService>().GetStoragePath(), "Terraria",
            "tModLoader", ".jbminer");
        private static string FolderPath(string place) => Path.Combine(StorageDirectory, place);

        public override bool Autoload(ref string name)
        {
            name = "Configuration";
            Instance ??= this;

            var configPath = FolderPath(WhitelistFileName);
            
            if (File.Exists(configPath))
            {
                using var str = File.OpenText(FolderPath(WhitelistFileName));
                Whitelist = JsonSerializer.Deserialize<List<int>>(str.ReadToEnd());
            }
            else
            {
                Whitelist = DefaultWhitelist;
                // Create Directory, to ensure it exists before storing.
                Directory.CreateDirectory(StorageDirectory);
                using var writer = new StreamWriter(File.Open(configPath, FileMode.Create));
                writer.Write(JsonSerializer.Serialize(Whitelist)); 
            }
            return base.Autoload(ref name);
        }

        // Unload so that we can properly fix the stream.
        public void Unload()
        {
            try
            {
                using var str = new StreamWriter(File.Open(FolderPath(WhitelistFileName), FileMode.Create));
                str.Write(JsonSerializer.Serialize(Whitelist)); // This makes the whitelist a string.
            }
            catch
            {
                
            }
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