using SPT.Reflection.Patching;
using EFT.Animations;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
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
            ConsoleScreen.Log("[VCO] Z: " + ___CameraOffset.z.ToString());
            ConsoleScreen.Log("[VCO] Y: " + ___CameraOffset.y.ToString());
            ConsoleScreen.Log("[VCO] X: " + ___CameraOffset.x.ToString());
            ConsoleScreen.Log("[VCO] PlayerSpringPatch: ");

            if (Plugin._OffsetStates.Value)
            {
                ___CameraOffset = new Vector3(Plugin._SidewaysOffset.Value, Plugin._UpDownOffset.Value, Plugin._ForwardBackwardOffset.Value);
            }
            else
            {
                ___CameraOffset = new Vector3(0.04f, 0.04f, 0.04f);
            }
        }
    }
}
