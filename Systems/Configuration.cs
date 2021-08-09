using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using On.Terraria.UI;
using Terraria.ModLoader.Config;

namespace JBMiner.Systems
{
    public class Configuration : ModConfig
    {
        [JsonIgnore] internal static Configuration instance;
        [JsonIgnore]
        public override ConfigScope Mode { get; } = ConfigScope.ClientSide;
        [DefaultValue(500)] public int BlockLimit; // This is the max amount of blocks we can break at any given time.

        public override bool Autoload(ref string name)
        {
            name = "Configuration";
            instance ??= this;
            return base.Autoload(ref name);
        }

        public List<int> Whitelist = DefaultWhitelist;
        
        [JsonIgnore]
        internal static List<int> DefaultWhitelist = new()
        {
            6,
            7,
            8,
            9,
            22,
            107,
            108,
            111,
            166,
            167,
            168,
            169,
            204,
            211,
            221,
            222,
            223
        };
    }
}