using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Low Gravity", "Lets you float in the air.", "LowGravity", 7)]
    class LowGravity : ModHandler
    {
        public static LowGravity instance;
        public float gravityScale = .25f;
        Vector3 grav;

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