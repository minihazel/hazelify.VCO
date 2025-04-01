using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using EFT.UI;
using EFT;
using Comfort.Common;
using EFT.InventoryLogic;
using BepInEx.Configuration;
using hazelify.VCO.Events;

namespace hazelify.VCO.Patches
{
    public class SetItemInHandsPatch : ModulePatch
    {
        public static bool isFinished = false;
        public static ConfigEntry<bool> _OffsetStates;
        public static ConfigEntry<bool> _toggleAutomaticWeaponDetection;
        public static ConfigEntry<float> _ForwardBackwardOffset;
        public static ConfigEntry<float> _UpDownOffset;
        public static ConfigEntry<float> _SidewaysOffset;

        public static void Initialize(
            ConfigEntry<bool> OffsetStates,
            ConfigEntry<bool> ToggleAutomaticWeaponDetection,
            ConfigEntry<float> ForwardBackwardOffset,
            ConfigEntry<float> UpDownOffset,
            ConfigEntry<float> SidewaysOffset)
        {
            _ForwardBackwardOffset = ForwardBackwardOffset;
            _ForwardBackwardOffset.SettingChanged += OffsetEvents.OffsetSettingChanged;

            _UpDownOffset = UpDownOffset;
            _UpDownOffset.SettingChanged += OffsetEvents.OffsetSettingChanged;

            _SidewaysOffset = SidewaysOffset;
            _SidewaysOffset.SettingChanged += OffsetEvents.OffsetSettingChanged;

            _toggleAutomaticWeaponDetection = ToggleAutomaticWeaponDetection;
            _toggleAutomaticWeaponDetection.SettingChanged += Plugin.OnAutomaticOptionToggled;

            _OffsetStates = OffsetStates;
            _OffsetStates.SettingChanged += OffsetEvents.OffsetSettingChanged;
        }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.method_82));
        }

        [PatchPostfix]
        private static void PatchPostfix(Item item)
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.RegisteredPlayers == null)
            {
                return;
            }

            if (item != null)
            {
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
                                    if (item is not Weapon weapon) return;
                                    string weapon_name = weapon.LocalizedName().ToString();
                                    ConsoleScreen.Log("[VCO] Equipped weapon: " + weapon_name);
                                    ConsoleScreen.Log("");

                                    if (!_toggleAutomaticWeaponDetection.Value) return;
                                    for (int i = 0; i < Plugin.weaponsList.Count; i++)
                                    {
                                        if (weapon_name.Contains(Plugin.weaponsList[i]))
                                        {
                                            Plugin._OffsetStates.Value = true;
                                            isFinished = false;
                                            break;
                                        }
                                        else
                                        {
                                            isFinished = true;
                                            continue;
                                        }
                                    }

                                    if (isFinished)
                                    {
                                        Plugin._OffsetStates.Value = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
