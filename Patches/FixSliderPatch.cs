using EFT.UI;
using EFT.UI.Settings;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hazelify.VCO.Patches
{
    public class FixSliderPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameSettingsTab), nameof(GameSettingsTab.Show));
        }

        [PatchPostfix]
        private static void PatchPostfix(ref NumberSlider ____fov, GClass1053 ___gclass1053_0)
        {
            if (Plugin._fovtoggle.Value)
            {
                Plugin.minRange.Value = 20;
                Plugin.maxRange.Value = 150;

                SettingsTab.BindNumberSliderToSetting(____fov, ___gclass1053_0.FieldOfView, Plugin.minRange.Value, Plugin.maxRange.Value);
            }
            else
            {
                Plugin.minRange.Value = 20;
                Plugin.maxRange.Value = 75;
            }
        }
    }
}
