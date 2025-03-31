using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hazelify.VCO;

namespace hazelify.VCO.PresetInfo
{
    public class PresetLoader
    {
        public static PresetData LoadPresets(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // Create an empty JSON file if missing
                File.WriteAllText(filePath, "{\"presets\": []}");
                return new PresetData { Presets = new List<Preset>() };
            }

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<PresetData>(json) ?? new PresetData { Presets = new List<Preset>() };
        }
    }
}
