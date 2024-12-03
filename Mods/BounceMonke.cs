using Index.Resources;
using UnityEngine;
using GorillaLocomotion;

namespace Index.Mods
{
    class BounceMonke : IndexMod
    {
        public static BounceMonke instance;

        public BounceMonke()
        {
            modName = "Bounce Monke";
            modDescription = "Makes you bouncy.";
            modGUID = "BounceMonke";
            modID = 10;
            modType = ModType.gameplay;
        }

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Player.Instance.bodyCollider.material.bounciness = 0f;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Player.Instance.bodyCollider.material.bounciness = 0.75f;
        }
    }
}