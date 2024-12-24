using Index.Resources;
using GorillaLocomotion;
using BepInEx.Configuration;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Bounce Monke", "Makes you bouncy.", "BounceMonke", 10)]
    class BounceMonke : ModHandler
    {
        public static BounceMonke instance;
        public ConfigEntry<float> bounceMultiplier;

        public override void Start()
        {
            base.Start();
            instance = this;
            bounceMultiplier = Plugin.config.Bind(
                section: "Bounce Monke",
                key: "Bounce Multiplier",
                defaultValue: 1f,
                description: "Changes your bounce amount. 1f = bouncy, 5f = extremely bouncy."
            );
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Player.Instance.bodyCollider.material.bounciness = 0f;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Player.Instance.bodyCollider.material.bounciness = Mathf.Clamp(bounceMultiplier.Value, 1f, 5f);
        }
    }
}