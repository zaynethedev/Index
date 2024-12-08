using HarmonyLib;
using Index.Mods;
using GorillaLocomotion;

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
}