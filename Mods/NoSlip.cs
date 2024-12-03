using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using HarmonyLib;
using Index.Mods;
using System;
using Index;

namespace Index.Mods
{
    class NoSlip : IndexMod
    {
        public static NoSlip instance;

        public NoSlip()
        {
            modName = "No Slip";
            modDescription = "Disables slipping mechanics.";
            modGUID = "NoSlip";
            modID = 5;
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
            SlipperyMonke.instance.OnModDisabled();
        }
    }
}

[HarmonyPatch(typeof(Player), "GetSlidePercentage")]
class NoSlipPatch
{
    static void Postfix(ref float __result)
    {
        if (NoSlip.instance.enabled)
        {
            __result = 0f;
        }
    }
}