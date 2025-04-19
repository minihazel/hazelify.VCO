using EFT.Animations;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace hazelify.VCO.Patches.StancePatches
{
    public class WeaponPositionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), "Update");
        }

        [PatchPostfix]
        public static void PatchPostfix(ProceduralWeaponAnimation __instance)
        {
            if (__instance == null) return;
            if (__instance.HandsContainer == null) return;
            if (__instance.HandsContainer.WeaponRoot == null) return;

            Vector3 stanceOffset = Vector3.zero;
            Quaternion stanceRotation = Quaternion.identity;

            switch (StanceController.currentStance)
            {
                case WeaponStance.HighReady:
                    stanceOffset = new Vector3(0f, 0.08f, -0.05f);
                    stanceRotation = Quaternion.Euler(10f, 0f, 0f);
                    break;
                case WeaponStance.LowReady:
                    stanceOffset = new Vector3(0f, -0.05f, -0.02f);
                    stanceRotation = Quaternion.Euler(-5f, 0f, 0f);
                    break;
                case WeaponStance.Default:
                default:
                    return;
            }

            __instance.HandsContainer.WeaponRoot.localPosition += stanceOffset;
            __instance.HandsContainer.WeaponRoot.localRotation *= stanceRotation;
        }
    }
}
