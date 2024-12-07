using Index.Resources;
using GorillaLocomotion;
using HarmonyLib;
using Index.Mods;

namespace Index.Mods
{
    [IndexMod("Slippy Monke", "Makes everything slippery.", "SlippyMonk", 6)]
    class SlipperyMonke : ModHandler
    {
        public static SlipperyMonke instance;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();
            NoSlip.instance.OnModDisabled();
        }
    }
}

[HarmonyPatch(typeof(Player), "GetSlidePercentage")]
class SlipPatch
{
    static void Postfix(ref float __result)
    {
        if (SlipperyMonke.instance.enabled)
        {
            __result = 1f;
        }
    }
}