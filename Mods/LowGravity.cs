using BepInEx.Configuration;
using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Low Gravity", "Lets you float in the air.", "LowGravity", 7)]
    class LowGravity : ModHandler
    {
        public static LowGravity instance;
        public ConfigEntry<float> gravityScale;
        Vector3 grav;

        public override void Start()
        {
            base.Start();
            instance = this;
            grav = Physics.gravity;
            gravityScale = Plugin.config.Bind(
                section: "Gravity",
                key: "Gravity Multiplier",
                defaultValue: 0.25f,
                description: "Changes your gravity. 0 = no gravity, 1 = normal gravity."
            );
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Physics.gravity = grav * Mathf.Clamp(gravityScale.Value, 0, 1);
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Physics.gravity = grav;
        }
    }
}