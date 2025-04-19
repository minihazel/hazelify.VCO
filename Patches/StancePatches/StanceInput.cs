using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using EFT;
using EFT.Animations;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace hazelify.VCO.Patches.StancePatches
{
    public class StanceInput : MonoBehaviour
    {
        private KeyCode toggleKey = KeyCode.H;

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                StanceController.cycleCurrentStance();
            }
        }
    }
}
