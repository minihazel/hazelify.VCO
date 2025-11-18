using BepInEx.Configuration;
using BepInEx;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using UnityEngine;
using System;
using System.Collections.Generic;
using hazelify.VCO.Patches;
using EFT.UI;

namespace hazelify.VCO.Events
{
    public static class OffsetEvents
    {
        private static ManualLogSource Logger;
        public static ConfigEntry<string> _PresetSelection { get; private set; }
        public static ConfigEntry<bool> _OffsetStates;
        public static ConfigEntry<bool> _toggleAutomaticWeaponDetection;
        public static ConfigEntry<float> _ForwardBackwardOffset;
        public static ConfigEntry<float> _UpDownOffset;
        public static ConfigEntry<float> _SidewaysOffset;
        public static ConfigEntry<KeyboardShortcut> _ActiveAimShortcut;
        public static ConfigEntry<bool> _EnableActiveAim;
        public static bool isInActiveAim;

        public static string currentEnv = Environment.CurrentDirectory;
        public static List<string> weapons = new List<string>();

        public static void Initialize(
            ConfigEntry<string> PresetSelection,
            ConfigEntry<float> ForwardBackwardOffset,
            ConfigEntry<float> UpDownOffset,
            ConfigEntry<float> SidewaysOffset,
            ConfigEntry<bool> OffsetStates,
            ConfigEntry<bool> ToggleAutomaticWeaponDetection,
            ConfigEntry<KeyboardShortcut> ActiveAimShortcut,
            ConfigEntry<bool> EnableActiveAim,
            bool IsInActiveAim)
        {
            _PresetSelection = PresetSelection;
            _PresetSelection.SettingChanged += Plugin.OnOptionToggled;

            _ForwardBackwardOffset = ForwardBackwardOffset;
            _ForwardBackwardOffset.SettingChanged += OffsetSettingChanged;

            _UpDownOffset = UpDownOffset;
            _UpDownOffset.SettingChanged += OffsetSettingChanged;

            _SidewaysOffset = SidewaysOffset;
            _SidewaysOffset.SettingChanged += OffsetSettingChanged;

            _toggleAutomaticWeaponDetection = ToggleAutomaticWeaponDetection;
            _toggleAutomaticWeaponDetection.SettingChanged += Plugin.OnAutomaticOptionToggled;

            _ActiveAimShortcut = ActiveAimShortcut;
            _ActiveAimShortcut.SettingChanged += OffsetSettingChanged;

            _EnableActiveAim = EnableActiveAim;

            isInActiveAim = IsInActiveAim;

            _OffsetStates = OffsetStates;
            _OffsetStates.SettingChanged += OffsetSettingChanged;
            _OffsetStates.SettingChanged += Plugin.onOffsetToggled;
        }

        public static void OffsetSettingChanged(object sender, EventArgs e)
        {

            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.RegisteredPlayers == null)
            {
                return;
            }

            /*
            Vector3
            Parameter 1: Left, right (currentOffset["Z"])
            Parameter 2: Up, down (currentOffset["Y"])
            Parameter 3: Forward, backward
            */

            if (gameWorld != null)
            {
                if (gameWorld.MainPlayer != null)
                {
                    if (gameWorld.MainPlayer.ProceduralWeaponAnimation != null)
                    {
                        if (gameWorld.MainPlayer.ProceduralWeaponAnimation.HandsContainer != null)
                        {
                            if (gameWorld.MainPlayer.ProceduralWeaponAnimation.HandsContainer.CameraOffset != null)
                            {
                                var __instance = gameWorld.MainPlayer.ProceduralWeaponAnimation;
                                Vector3 newOffsets;

                                if (_OffsetStates.Value)
                                {
                                    newOffsets = new Vector3(_SidewaysOffset.Value, _UpDownOffset.Value, _ForwardBackwardOffset.Value);
                                    __instance.HandsContainer.CameraOffset = newOffsets;
                                }
                                else
                                {
                                    newOffsets = new Vector3(0.04f, 0.04f, 0.04f);
                                }

                                if (_EnableActiveAim.Value && isInActiveAim)
                                {
                                    // Active Aim is ON: OVERRIDE the Y and Z values with the saved currentOffset
                                    // NOTE: Forward/Backward offset (X) usually remains the same, using _ForwardBackwardOffset.Value.

                                    // We overwrite only the Y (Up/Down) and Z (Sideways) components
                                    newOffsets.y = Plugin.currentOffset["Y"]; // Y (Up/Down)
                                    newOffsets.z = Plugin.currentOffset["Z"]; // Z (Sideways)
                                }

                                __instance.HandsContainer.CameraOffset = newOffsets;
                            }
                        }
                    }
                }
            }
        }
    }
}