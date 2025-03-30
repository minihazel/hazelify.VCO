using BepInEx.Configuration;
using BepInEx;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hazelify.VCO;
using EFT.InventoryLogic;
using System.IO;
using JetBrains.Annotations;
using System.Security.Cryptography.X509Certificates;

namespace hazelify.VCO.Events
{
    public static class OffsetEvents
    {
        private static ManualLogSource Logger;
        public static ConfigEntry<float> _ForwardBackwardOffset;
        public static ConfigEntry<float> _UpDownOffset;
        public static ConfigEntry<float> _SidewaysOffset;
        public static ConfigEntry<bool> _toggleoffset;

        public static string currentEnv = Environment.CurrentDirectory;
        public static List<string> weapons = new List<string>();

        public static void Initialize(ConfigEntry<float> ForwardBackwardOffset, ConfigEntry<float> UpDownOffset, ConfigEntry<float> SidewaysOffset, ConfigEntry<bool> Toggleoffset)
        {
            _ForwardBackwardOffset = ForwardBackwardOffset;
            _ForwardBackwardOffset.SettingChanged += OffsetSettingChanged;

            _UpDownOffset = UpDownOffset;
            _UpDownOffset.SettingChanged += OffsetSettingChanged;

            _SidewaysOffset = SidewaysOffset;
            _SidewaysOffset.SettingChanged += OffsetSettingChanged;

            _toggleoffset = Toggleoffset;
            _toggleoffset.SettingChanged += OffsetSettingChanged;
        }

        private static void OffsetSettingChanged(object sender, EventArgs e)
        {

            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.RegisteredPlayers == null)
            {
                return;
            }

            /*
            Vector3
            Parameter 1: Left, right
            Parameter 2: Up, down
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
                                if (_toggleoffset.Value)
                                {
                                    Vector3 newOffsets = new Vector3(_SidewaysOffset.Value, _UpDownOffset.Value, _ForwardBackwardOffset.Value);
                                    gameWorld.MainPlayer.ProceduralWeaponAnimation.HandsContainer.CameraOffset = newOffsets;
                                    ConsoleScreen.Log("[VCO] Activated viewmodel offset");
                                }
                                else
                                {
                                    Vector3 newOffsets = new Vector3(0.04f, 0.04f, 0.04f);
                                    gameWorld.MainPlayer.ProceduralWeaponAnimation.HandsContainer.CameraOffset = newOffsets;
                                    ConsoleScreen.Log("[VCO] Deactivated viewmodel offsets");
                                }
                            }
                        }
                    }
                }
            }

            /*
            if (gameWorld.MainPlayer.HandsController != null)
            {
                if (gameWorld.MainPlayer.HandsController.Item != null)
                {
                    if (gameWorld.MainPlayer.HandsController.Item.LocalizedName().ToString() != "")
                    {
                        string shortName = gameWorld.MainPlayer.HandsController.Item.LocalizedShortName().ToString();
                        string full = gameWorld.MainPlayer.HandsController.Item.LocalizedName().ToString();

                        if (weapons.Contains(full))
                        {
                            // Vector3 newOffsets = new Vector3(_SidewaysOffset.Value, _UpDownOffset.Value, _ForwardBackwardOffset.Value);
                            // gameWorld.MainPlayer.ProceduralWeaponAnimation.HandsContainer.CameraOffset = newOffsets;

                            ConsoleScreen.Log("[VCO] Applied camera offset for: " + shortName);
                        }
                    }
                }
            }
            */
        }
    }
}
