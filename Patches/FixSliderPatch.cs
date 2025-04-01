using SPT.Reflection.Patching;
using EFT.UI.Settings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using EFT.UI;
using EFT;

namespace hazelify.VCO.Patches
{
    public class FixSliderPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameSettingsTab), nameof(GameSettingsTab.Show));
        }

        [PatchPostfix]
        private static void PatchPostfix(ref NumberSlider ____fov, GClass1040 ___gclass1040_0)
        {
            if (Plugin._fovtoggle.Value)
            {
                Plugin.minRange.Value = 20;
                Plugin.maxRange.Value = 150;

                SettingsTab.BindNumberSliderToSetting(____fov, ___gclass1040_0.FieldOfView, Plugin.minRange.Value, Plugin.maxRange.Value);
            }
            else
            {
                Plugin.minRange.Value = 20;
                Plugin.maxRange.Value = 75;
            }
        }
    }
}
