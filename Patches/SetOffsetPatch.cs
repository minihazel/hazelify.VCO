using Comfort.Common;
using EFT;
using EFT.Animations;
using EFT.InventoryLogic;
using EFT.UI;
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
    // DO NOT USE, WILL BREAK THE SETTINGS MENU
    public class SetOffsetPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player.FirearmController), nameof(Player.FirearmController.smethod_8));
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.RegisteredPlayers == null)
            {
                return;
            }

            if (__instance != null)
            {
                if (gameWorld != null)
                {
                    if (gameWorld.MainPlayer != null)
                    {
                        string fullItem = (string)__instance.HandsController.Item.LocalizedName().ToString();
                        ConsoleScreen.Log("[VCO] Equipped weapon: " + fullItem);
                        ConsoleScreen.Log("");
                    }
                }
            }
        }
    }
}
