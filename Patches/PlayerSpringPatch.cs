using SPT.Reflection.Patching;
using EFT.Animations;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using EFT.UI;

namespace hazelify.VCO.Patches
{
    public class PlayerSpringPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PlayerSpring), nameof(PlayerSpring.Start));
        }

        [PatchPostfix]
        private static void PatchPostfix(ref Vector3 ___CameraOffset)
        {
            if (Plugin._toggleoffset.Value)
            {
                ___CameraOffset = new Vector3(Plugin._SidewaysOffset.Value, Plugin._UpDownOffset.Value, Plugin._ForwardBackwardOffset.Value);
                ConsoleScreen.Log("[VCO] Activated PlayerSpring offset");
            }
            else
            {
                ___CameraOffset = new Vector3(0.04f, 0.04f, 0.04f);
                ConsoleScreen.Log("[VCO] Deactivated PlayerSpring offset");
            }
        }
    }
}
