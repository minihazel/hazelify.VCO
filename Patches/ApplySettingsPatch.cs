using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using hazelify.VCO;

namespace hazelify.VCO.Patches
{
    public class ApplySettingsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass1085.Class1841), nameof(GClass1085.Class1841.method_0));
        }

        [PatchPostfix]
        public static void PatchPostfix(int x, ref int __result)
        {
            __result = Mathf.Clamp(x, Plugin.minRange.Value, Plugin.maxRange.Value);
        }
    }
}