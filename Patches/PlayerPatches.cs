using HarmonyLib;
using Index.Mods;
using GorillaLocomotion;
using System.Collections.Generic;
using UnityEngine;

namespace Index.Patches
{
    [HarmonyPatch(typeof(Player), "GetSlidePercentage")]
    class SlipPatch
    {
        static void Postfix(ref float __result)
        {
            if (SlipperyMonke.instance.enabled)
            {
                __result = 1f;
            }
            else if (NoSlip.instance.enabled)
            {
                __result = 0.03f;
            }
        }
    }

    [HarmonyPatch(typeof(ForceVolume))]
    public class ForcePatch
    {
        public static List<ForceVolume> ActiveForces = new List<ForceVolume>();

        [HarmonyPatch(nameof(ForceVolume.OnTriggerEnter)), HarmonyPrefix]
        public static bool ForceEnter(ForceVolume __instance, Collider other)
        {
            if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
            {
                ActiveForces.AddIfNew(__instance);
                return DisableWindBarrier.UseForceMethods;
            }
            return true;
        }

        [HarmonyPatch(nameof(ForceVolume.OnTriggerStay)), HarmonyPrefix]
        public static bool ForceStay(ForceVolume __instance, Collider other)
        {
            if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
            {
                if (ActiveForces.Contains(__instance)) ActiveForces.Remove(__instance);
                return DisableWindBarrier.UseForceMethods;
            }
            return true;
        }

        [HarmonyPatch(nameof(ForceVolume.OnTriggerExit)), HarmonyPrefix]
        public static bool ForceExit(ForceVolume __instance, Collider other)
        {
            if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
            {
                return DisableWindBarrier.UseForceMethods;
            }
            return true;
        }
    }
}