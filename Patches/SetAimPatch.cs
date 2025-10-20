using EFT;
using EFT.Animations;
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
    public class SetAimPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player.FirearmController), nameof(Player.FirearmController.SetAim), new[] { typeof(bool) });
        }

        [PatchPostfix]
        public static void PatchPostfix(Player.FirearmController __instance, bool value)
        {
            if (__instance == null) return;

            Player player = __instance.GetComponent<Player>();
            bool yourPlayer = player.IsYourPlayer;
            ProceduralWeaponAnimation pwa = player.ProceduralWeaponAnimation;
            EPointOfView pov = pwa.PointOfView;

            if (yourPlayer && pov == EPointOfView.FirstPerson)
            {
                player.MovementContext.PlayerAnimator.SetAiming(true);
            }
        }
    }
}
