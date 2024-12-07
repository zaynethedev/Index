using Index.Resources;
using GorillaLocomotion;

namespace Index.Mods
{
    [IndexMod("Bounce Monke", "Makes you bouncy.", "BounceMonke", 10)]
    class BounceMonke : ModHandler
    {
        public static BounceMonke instance;

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
            Player.Instance.bodyCollider.material.bounciness = 1f;
        }
    }
}