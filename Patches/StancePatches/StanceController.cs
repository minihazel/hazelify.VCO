using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace hazelify.VCO.Patches.StancePatches
{
    public enum WeaponStance
    {
        Default,
        HighReady,
        LowReady
    }

    public class StanceController : MonoBehaviour
    {
        public static WeaponStance currentStance = WeaponStance.Default;
        public static void cycleCurrentStance()
        {
            currentStance = (WeaponStance)(((int)currentStance + 1) % System.Enum.GetValues(typeof(WeaponStance)).Length);
            Debug.Log($"[VCO] Cycled to: {currentStance}");
        }

        public static float GetStanceSwayMultiplier()
        {
            return currentStance switch
            {
                WeaponStance.HighReady => 0.8f,
                WeaponStance.LowReady => 0.6f,
                _ => 1.0f
            };
        }

        public static float GetStanceErgoMultiplier()
        {
            return currentStance switch
            {
                WeaponStance.HighReady => 1.1f,
                WeaponStance.LowReady => 0.9f,
                _ => 1.0f
            };
        }
    }
}
