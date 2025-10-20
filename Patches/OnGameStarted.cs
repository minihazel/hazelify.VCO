using EFT;
using HarmonyLib;
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
        }
    }
}
