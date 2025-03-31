using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hazelify.VCO;
using Newtonsoft.Json;

namespace hazelify.VCO.PresetInfo
{
    public static class PresetManager
    {
        public static List<Preset> LoadedPresets { get; } = [];
        private static string _filePath;

        public static void Initialize(string filePath)
        {
            _filePath = filePath;
            if (File.Exists(_filePath))
            {
                var presetData = PresetLoader.LoadPresets(filePath);
                LoadedPresets.AddRange(presetData.Presets);
            }
        }

        public static void AddPreset(string name, float x, float y, float z)
        {
            if (LoadedPresets.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) return;

            var newPreset = new Preset { Name = name, X = x, Y = y, Z = z };
            LoadedPresets.Add(newPreset);
            SavePresets();
        }

        public static bool DeletePreset(string name)
        {
            var removePreset = LoadedPresets.Find(p => p.Name.Equals(name,StringComparison.OrdinalIgnoreCase));

            if (removePreset != null)
            {
                LoadedPresets.Remove(removePreset);
                SavePresets();
                return true;
            }

            return false;
        }

        private static void SavePresets()
        {
            var presetData = new PresetData { Presets = LoadedPresets };
            string json = JsonConvert.SerializeObject(presetData, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
