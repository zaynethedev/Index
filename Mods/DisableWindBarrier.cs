using Index.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Disable Wind Barrier", "Disables all wind barriers. Credits to defaultuser0 for the help.", "DisableWindBarrier", 14)]
    class DisableWindBarrier : ModHandler
    {
        public static DisableWindBarrier instance;
        public static bool UseForceMethods = true;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/Forest_ForceVolumes/").SetActive(true);
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/Forest_ForceVolumes/").SetActive(false);
        }
    }
}