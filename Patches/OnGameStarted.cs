using EFT;
using EFT.UI;
using HarmonyLib;
using hazelify.VCO.PresetInfo;
using SPT.Reflection.Patching;
using System.Reflection;

namespace hazelify.VCO.Patches
{
    public class OnGameStarted : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        private static void PatchPostfix(ref GameWorld __instance)
        {
            Plugin.hasStartedGame = true;

            Plugin.currentOffset.Clear();
            Plugin.currentOffset["Y"] = Plugin._UpDownOffset.Value;
            Plugin.currentOffset["Z"] = Plugin._SidewaysOffset.Value;
        }
    }
}
