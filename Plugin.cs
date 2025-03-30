using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using UnityEngine;
using System;
using hazelify.VCO.Events;
using hazelify.VCO.Patches;
using HarmonyLib;
using BepInEx.Bootstrap;
using System.Drawing.Text;
using ConfigurationManager;

namespace hazelify.VCO;

[BepInPlugin("hazelify.vco", "Viewmodel Camera Offset", "1.0.0")]
[BepInDependency("com.samswat.fov", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    private static new ManualLogSource Logger;
    public static string FOVmod = "com.samswat.fov";

    public static bool isFOVinstalled { get; private set; }

    public static ConfigEntry<int> minRange;
    public static ConfigEntry<int> maxRange;

    public static ConfigEntry<bool> _placeholder;
    public static ConfigEntry<float> _ForwardBackwardOffset;
    public static ConfigEntry<float> _UpDownOffset;
    public static ConfigEntry<float> _SidewaysOffset;
    public static ConfigEntry<bool> _fovtoggle;
    public static ConfigEntry<bool> _toggleoffset;

    private void Awake()
    {
        isFOVinstalled = Chainloader.PluginInfos.ContainsKey(FOVmod);

        // Awake logic via Unity
        Logger = base.Logger;
        Logger.LogInfo($"hazelify.VCO has loaded!");

        if (!isFOVinstalled)
        {
            new FixSliderPatch().Enable();
            new PlayerSpringPatch().Enable();
            new ApplySettingsPatch().Enable();
            // new SetOffsetPatch().Enable();

            minRange = Config.Bind(
                "Main Section",
                "Min FOV Value",
                20,
                new ConfigDescription("Your desired minimum FOV value. Default is 50",
                new AcceptableValueRange<int>(1, 149),
                new ConfigurationManagerAttributes { IsAdvanced = true }));

            maxRange = Config.Bind(
                "Main Section",
                "Max FOV Value",
                150,
                new ConfigDescription("Your desired maximum FOV value. Default is 75",
                new AcceptableValueRange<int>(1, 150),
                new ConfigurationManagerAttributes { IsAdvanced = true }));

            // set up configuration options for the Bep menu
            _ForwardBackwardOffset = Config.Bind(
                "Offsets",
                "Camera X Offset",
                0.05f,
                new ConfigDescription("Lower value = Camera goes further left. Higher value = Camera goes further right. Default is 0.05",
                new AcceptableValueRange<float>(-0.5f, 0.5f)));

            _UpDownOffset = Config.Bind(
                "Offsets",
                "Camera Y Offset",
                0.05f,
                new ConfigDescription("Lower value = Camera goes further down. Higher value = Camera goes further up Default is 0.05",
                new AcceptableValueRange<float>(-0.5f, 0.5f)));

            _SidewaysOffset = Config.Bind(
                "Offsets",
                "Camera Z Offset",
                0.05f,
                new ConfigDescription("Lower value = Camera goes further back. Higher value = Camera goes further front. Default is 0.05",
                new AcceptableValueRange<float>(-0.5f, 0.5f)));

            _toggleoffset = Config.Bind(
                "Offsets",
                "Enable auto-offset",
                true,
                "This will automatically apply your offset settings on raid-start/when an offset option is altered. You can still change the offsets on the fly. Default is true");

            _fovtoggle = Config.Bind(
                "Field of View",
                "Enable expanded range",
                false,
                "Allows increasing the FOV limit to 150");

            // init events
            OffsetEvents.Initialize(_ForwardBackwardOffset, _UpDownOffset, _SidewaysOffset, _toggleoffset);
            _fovtoggle.SettingChanged += fovSettingChanged;
        }
        else
        {
            _placeholder = Config.Bind(
                "Detection", 
                "FOV mod detected", 
                false, 
                "SamSWAT's FOV mod detected, all options are disabled.");
        }
    }

    private static void fovSettingChanged(object sender, EventArgs e)
    {
        new FixSliderPatch().Enable();
        new ApplySettingsPatch().Enable();
    }
}
