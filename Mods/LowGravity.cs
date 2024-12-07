using Index.Resources;
using GorillaLocomotion;
using UnityEngine;

namespace Index.Mods
{
    class LowGravity : IndexMod
    {
        public static LowGravity instance;
        public float gravityScale = .25f;
        Vector3 grav;

        public LowGravity()
        {
            modName = "Low Gravity";
            modDescription = "Lets you float in the air.";
            modGUID = "LowGravity";
            modID = 7;
            modType = ModType.testing;
        }

        public override void Start()
        {
            base.Start();
            instance = this;
            grav = Physics.gravity;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Physics.gravity = grav * gravityScale;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Physics.gravity = grav;
        }
    }
}