using Index.Resources;
using System.Collections.Generic;
using Index.Patches;
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

        public void SetForces(bool useForces)
        {
            Debug.Log($"SetForces ({useForces})");

            if (!useForces)
            {
                List<ForceVolume> activeForces = new List<ForceVolume>(ForcePatch.ActiveForces);
                foreach (ForceVolume force in activeForces)
                {
                    force.OnTriggerExit(GorillaTagger.Instance.headCollider);
                }
            }

            UseForceMethods = useForces;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            SetForces(true);
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            SetForces(false);
        }
    }
}