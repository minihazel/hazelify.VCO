﻿using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace hazelify.VCO.Patches
{
    public class OnGameDestroyedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnDestroy));
        }

        [PatchPostfix]
        private static void PatchPostfix(ref GameWorld __instance)
        {
            Plugin.hasStartedGame = false;
        }
    }
}
