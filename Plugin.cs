using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using hazelify.VCO.Events;
using hazelify.VCO.Patches;
using BepInEx.Bootstrap;
using System.Collections.Generic;
using System.IO;
using hazelify.VCO.PresetInfo;
using EFT.UI;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace hazelify.VCO;

[BepInPlugin("hazelify.vco", "Viewmodel Camera Offset", "1.0.6")]
// [BepInDependency("com.samswat.fov", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    // personal build event path: "..\..\..\BepInEx\plugins\hazelify.VCO\"
    // general strings

    // FOV mod detection outphased for the time being until
    // I can figure out better compatibility integration

    public string currentEnv = Environment.CurrentDirectory; // main SPT dir
    public static string weaponsPath = null;
    public static string presetsPath = null;
    public static List<string> weaponsList = new List<string>();
    public static List<string> presetsList = new List<string>();
    public static string currentWeapon = null;
    public static bool isInActiveAim = false;
    public static bool hasStartedGame = false;
    public static bool isEnablingViaUpdate = false;

    public static Dictionary<string, float> currentOffset = new Dictionary<string, float>();

    public static new ManualLogSource Logger;

    private const string Settings = "Settings";
    public static ConfigEntry<bool> _notice;
    // public static ConfigEntry<string> PresetSelection { get; private set; }
    public static ConfigEntry<string> PresetSelection { get; private set; }
    public static ConfigEntry<bool> _DeletePreset;
    public static ConfigEntry<bool> _OffsetStates;
    public static ConfigEntry<bool> _toggleAutomaticWeaponDetection;
    public static ConfigEntry<bool> _refreshWeaponsList;
    public static ConfigEntry<bool> _refreshPresetsList;

    private const string Offsets = "Offsets";
    public static ConfigEntry<float> _ForwardBackwardOffset;
    public static ConfigEntry<float> _UpDownOffset;
    public static ConfigEntry<float> _SidewaysOffset;

    public static ConfigEntry<bool> _EnableActiveAim;
    public static ConfigEntry<KeyboardShortcut> _ActiveAimShortcut;

    private const string Export = "Export";
    public static ConfigEntry<string> _PresetName;
    public static ConfigEntry<bool> _ExportPreset;

    private const string FieldofView = "Field of View";
    public static ConfigEntry<bool> _fovtoggle;

    private const string DoNotModify = "(Do not modify)";
    public static ConfigEntry<int> minRange;
    public static ConfigEntry<int> maxRange;
    public static ConfigEntry<bool> _placeholder;

    public void Awake()
    {
        // Awake logic via Unity
        Logger = base.Logger;
        Logger.LogInfo($"hazelify.VCO has loaded!");

        new FixSliderPatch().Enable();
        new PlayerSpringPatch().Enable();
        new ApplySettingsPatch().Enable();
        new SetItemInHandsPatch().Enable();
        new OnGameStarted().Enable();
        new OnGameDestroyedPatch().Enable();

        weaponsPath = Path.Combine(currentEnv, "BepInEx", "plugins", "hazelify.VCO", "weapons.cfg");
        presetsPath = Path.Combine(currentEnv, "BepInEx", "plugins", "hazelify.VCO", "presets.json");

        checkPaths();
        initPresets();
        readFromWeaponsList();

        // Settings.Bind(Config);
        minRange = Config.Bind(
            DoNotModify,
            "Min FOV Value",
            20,
            new ConfigDescription("Your desired minimum FOV value. Default is 50",
            new AcceptableValueRange<int>(1, 149),
            new ConfigurationManagerAttributes { IsAdvanced = true }));

        maxRange = Config.Bind(
            DoNotModify,
            "Max FOV Value",
            150,
            new ConfigDescription("Your desired maximum FOV value. Default is 75",
            new AcceptableValueRange<int>(1, 150),
            new ConfigurationManagerAttributes { IsAdvanced = true }));

        _fovtoggle = Config.Bind(
            FieldofView,
            "Enable expanded range",
            false,
            new ConfigDescription("Allows increasing the FOV limit to 150",
                null,
                new ConfigurationManagerAttributes { Order = 10 }));

        _SidewaysOffset = Config.Bind(
            Offsets,
            "Camera X Offset",
            0.05f,
            new ConfigDescription("Lower value = Camera goes further left. Higher value = Camera goes further right. Default is 0.05",
            new AcceptableValueRange<float>(-0.5f, 0.5f),
            new ConfigurationManagerAttributes { Order = 9 }));

        _UpDownOffset = Config.Bind(
            Offsets,
            "Camera Y Offset",
            0.05f,
            new ConfigDescription("Lower value = Camera goes further down. Higher value = Camera goes further up Default is 0.05",
            new AcceptableValueRange<float>(-0.5f, 0.5f),
            new ConfigurationManagerAttributes { Order = 8 }));

        _ForwardBackwardOffset = Config.Bind(
            Offsets,
            "Camera Z Offset",
            0.05f,
            new ConfigDescription("Lower value = Camera goes further back. Higher value = Camera goes further front. Default is 0.05",
            new AcceptableValueRange<float>(-0.5f, 0.5f),
            new ConfigurationManagerAttributes { Order = 7 }));

        _EnableActiveAim = Config.Bind(
            Offsets,
            "Enable Active Aim",
            false,
            new ConfigDescription("Allows for using (discount) Active Aim via hotkey.",
            null,
            new ConfigurationManagerAttributes { Order = 6 }));

        _ActiveAimShortcut = Config.Bind(
            Offsets,
            "Active Aim Hotkey",
            new KeyboardShortcut(KeyCode.Z),
            new ConfigDescription("Set what hotkey to toggle Active Aim with",
            null,
            new ConfigurationManagerAttributes { Order = 5 }));



        // set up configuration options for the Bep menu
        PresetSelection = Config.Bind(
            Settings,
            "Select Preset",
            "Default",
            new ConfigDescription("Choose a preset to use for viewmodel offsets.",
            new AcceptableValueList<string>(presetsList.ToArray()),
            new ConfigurationManagerAttributes { Order = 6 }));
        _notice = Config.Bind(
            Settings,
            "Hover cursor here to read",
            true,
            new ConfigDescription("If you are using SamSWAT's FOVFix or IncreasedFOV mod, this mod WILL clash. " +
                                  "I bear no responsibility for any misuse between any of these three mods. Use them at your own caution.",
                null,
                new ConfigurationManagerAttributes { Order = 5 }));
        _OffsetStates = Config.Bind(
            Settings,
            "Toggle the offsets on/off",
            true,
            new ConfigDescription("Choose if the mod should alter the viewmodel.",
                null,
                new ConfigurationManagerAttributes { Order = 4 }));
        _refreshPresetsList = Config.Bind(
            Settings,
            "Refresh preset list",
            false,
            new ConfigDescription("Refresh and fetch new presets from the `presets.json` file.",
                null,
                new ConfigurationManagerAttributes { Order = 3 }));
        _refreshWeaponsList = Config.Bind(
            Settings,
            "Refresh weapons list",
            false,
            new ConfigDescription("If edited, refresh the `weapons.json` list of recognized weapons in realtime for use with the automatic detection system.",
                null,
                new ConfigurationManagerAttributes { Order = 2 }));
        _toggleAutomaticWeaponDetection = Config.Bind(
            Settings,
            "Toggle automatic weapon detection",
            true,
            new ConfigDescription("Choose if the mod should only do its magic when the equipped weapon is recognized by the mod database.",
                null,
                new ConfigurationManagerAttributes { Order = 1 }));

        /*
        _PresetName = Config.Bind(
            Export,
            "Preset Name (for exporting)",
            "My Custom Preset",
            new ConfigDescription("Enter the name of your custom preset before you export it.",
                null,
                new ConfigurationManagerAttributes { Order = 2 }));

        _ExportPreset = Config.Bind(
            Export,
            "Export Preset",
            false,
            new ConfigDescription("Click the toggle to export your preset. It's a toggle instead of a button because Devraccoon is too lazy to make it a button.\n\n" +
                                  "Game restart is required to show the new preset in the dropdown.",
                null,
                new ConfigurationManagerAttributes { Order = 1 }));
        */

        // init events
        OffsetEvents.Initialize(PresetSelection, _ForwardBackwardOffset, _UpDownOffset, _SidewaysOffset, _OffsetStates, _toggleAutomaticWeaponDetection);
        SetItemInHandsPatch.Initialize(_OffsetStates, _toggleAutomaticWeaponDetection, _ForwardBackwardOffset, _UpDownOffset, _SidewaysOffset);

        _refreshPresetsList.SettingChanged += onRefreshPresetsList;
        _refreshWeaponsList.SettingChanged += onRefreshWeaponsList;
        _fovtoggle.SettingChanged += fovSettingChanged;
        // _ExportPreset.SettingChanged += PresetSettingsChanged;
        // _DeletePreset.SettingChanged += PresetDeleted;

        if (currentOffset == null) return;
        if (_SidewaysOffset == null) return;
        if (_SidewaysOffset == null) return;

        currentOffset.Add("X", _SidewaysOffset.Value);
        currentOffset.Add("Y", _UpDownOffset.Value);
    }

    public void Update()
    {
        if (hasStartedGame)
        {
            if (IsKeyPressed(_ActiveAimShortcut.Value) && _EnableActiveAim.Value)
            {
                if (!isInActiveAim)
                {
                    isEnablingViaUpdate = true;
                    isInActiveAim = true;
                    _SidewaysOffset.Value = 0.123f;
                    _UpDownOffset.Value = 0.043f;
                }
                else
                {
                    isEnablingViaUpdate = true;
                    isInActiveAim = false;
                    _SidewaysOffset.Value = currentOffset["X"];
                    _UpDownOffset.Value = currentOffset["Y"];
                }
            }
        }
    }

    public static string[] initPresets()
    {
        PresetManager.Initialize(presetsPath);
        if (PresetManager.LoadedPresets.Count == 0)
        {
            presetsList.Add("Default"); // fallback if no presets
        }

        presetsList.AddRange(PresetManager.LoadedPresets.Select(p => p.Name));
        string defaultValue = presetsList.Count > 0 ? presetsList[0] : "Default";
        return presetsList.ToArray();
    }

    public static void OnOptionToggled(object sender, EventArgs e)
    {
        if (PresetSelection != null)
        {
            if (PresetSelection.Value != null)
            {
                string selected = PresetSelection.Value;
                ConsoleScreen.Log("[VCO] Applying preset: " + selected);

                for (int i = 0; i < PresetManager.LoadedPresets.Count; i++)
                {
                    if (selected == PresetManager.LoadedPresets[i].Name)
                    {
                        _ForwardBackwardOffset.Value = PresetManager.LoadedPresets[i].X;
                        _UpDownOffset.Value = PresetManager.LoadedPresets[i].Y;
                        _SidewaysOffset.Value = PresetManager.LoadedPresets[i].Z;
                    }
                }
            }
        }
    }

    public static void OnAutomaticOptionToggled(object sender, EventArgs e)
    {

    }

    public static void onOffsetToggled(object sender, EventArgs e)
    {
        if (PresetSelection != null)
        {
            PresetSelection.Value = "Default";
        }
    }

    public static void RefreshPresetSelection()
    {
        presetsList.Clear();
        PresetManager.LoadedPresets.Clear();
        PresetManager.Initialize(Plugin.presetsPath);

        if (PresetManager.LoadedPresets.Count == 0)
        {
            Plugin.presetsList.Add("Default"); // fallback if no presets
        }

        for (int i = 0; i < PresetManager.LoadedPresets.Count; i++)
        {
            Plugin.presetsList.Add(PresetManager.LoadedPresets[i].Name);
        }

        string defaultValue = Plugin.presetsList.Count > 0 ? Plugin.presetsList[0] : "Default";
    }

    public static void onRefreshWeaponsList(object sender, EventArgs e)
    {
        if (_refreshWeaponsList == null) return;
        _refreshWeaponsList.Value = false;
        weaponsList.Clear();
        readFromWeaponsList();
    }

    public void onRefreshPresetsList(object sender, EventArgs e)
    {
        if (_refreshPresetsList == null) return;
        rebindPresets();
        _refreshPresetsList.Value = false;
    }

    public void rebindPresets()
    {
        string[] presetOptions = initPresets() ?? new[] { "Default" };
        string currentValue = PresetSelection.Value ?? presetOptions[0];
        if (!presetOptions.Contains(currentValue))
        {
            currentValue = presetOptions[0];
        }

        if (PresetSelection != null)
        {
            Config.Remove(PresetSelection.Definition);
        }

        PresetSelection = Config.Bind(
            "Presets",
            "Selected Preset",
            currentValue,
            new ConfigDescription(
                "Choose a preset",
                new AcceptableValueList<string>(presetOptions),
                new ConfigurationManagerAttributes { IsAdvanced = true }
            )
        );
    }

    public static void PresetDeleted(object sender, EventArgs e)
    {
        if (PresetSelection != null)
        {
            if (PresetSelection.Value != null)
            {
                string selected = PresetSelection.Value;
                if (string.IsNullOrEmpty(selected)) return;

                bool deletedPreset = PresetManager.DeletePreset(selected);
                if (deletedPreset)
                {
                    Logger.LogInfo($"[VCO] Preset " + selected + " successfully deleted");
                }
            }
        }
    }

    public static void PresetSettingsChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_PresetName.Value)) return;

        string presetName = _PresetName.Value;
        if (string.IsNullOrEmpty(presetName)) return;

        try
        {
            PresetManager.AddPreset(presetName, _SidewaysOffset.Value, _UpDownOffset.Value, _ForwardBackwardOffset.Value);
            Logger.LogInfo($"[VCO] Preset " + presetName + " successfully added");
        }
        catch (Exception ex)
        {
            Logger.LogError("[VCO Error] " + ex.Message.ToString());
        }
    }

    public static void fovSettingChanged(object sender, EventArgs e)
    {
        new FixSliderPatch().Enable();
        new ApplySettingsPatch().Enable();
    }

    private static void checkPaths()
    {
        bool doesPresetsPathExist = File.Exists(presetsPath);
        if (!doesPresetsPathExist)
        {
            File.Create(presetsPath);
            PresetManager.AddPreset("Default", 0.05f, 0.06f, -0.01f);
        }
        return;
    }

    private static void generateWeaponsList(string path)
    {
        try
        {
            File.CreateText(path);
            string[] allWeapons = {
                    "HK MP7A1 4.6x30 submachine gun",
                    "HK MP7A2 4.6x30 submachine gun",
                    "FN SCAR-L 5.56x45 assault rifle",
                    "FN SCAR-L 5.56x45 assault rifle (FDE)",
                    "FN SCAR-H X-17 7.62x51 assault rifle",
                    "FN SCAR-H X-17 7.62x51 assault rifle (FDE)",
                    "SIG MCX-SPEAR 6.8x51 assault rifle"
                };

            File.WriteAllLines(path, allWeapons);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError("[VCO ERROR] " + ex.Message.ToString());
        }
    }

    private static void readFromWeaponsList()
    {
        if (!File.Exists(weaponsPath))
        {
            generateWeaponsList(weaponsPath);
            readFromWeaponsList();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(File.ReadAllText(weaponsPath)))
            {
                File.Delete(weaponsPath);
                generateWeaponsList(weaponsPath);
                readFromWeaponsList();
            }
            else
            {
                weaponsList.AddRange(File.ReadAllLines(weaponsPath));
            }
        }
    }

    bool IsKeyPressed(KeyboardShortcut key)
    {
        if (!Input.GetKeyDown(key.MainKey))
        {
            return false;
        }

        foreach (var modifier in key.Modifiers)
        {
            if (!Input.GetKey(modifier))
            {
                return false;
            }
        }

        return true;
    }
}