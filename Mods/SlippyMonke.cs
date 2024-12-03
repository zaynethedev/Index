using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using HarmonyLib;
using Index.Mods;
using System;
using Index;

namespace Index.Mods
{
    class SlipperyMonke : IndexMod
    {
        public static SlipperyMonke instance;

        public SlipperyMonke()
        {
            modName = "Slippy Monke";
            modDescription = "Makes everything slippery.";
            modGUID = "SlippyMonk";
            modID = 6;
            modType = ModType.gameplay;
        }

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