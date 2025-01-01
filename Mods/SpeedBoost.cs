using Index.Resources;
using GorillaLocomotion;
using BepInEx.Configuration;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Speed Boost", "A mod that increases your speed, making it easier to run.", "SpeedBoost", 2)]
    class SpeedBoost : ModHandler
    {
        public static SpeedBoost instance;
        public ConfigEntry<float> speed;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void SetConfig()
        {
            base.SetConfig();
            speed = Plugin.config.Bind(
                section: "Speed Boost",
                key: "Speed Multiplier",
                defaultValue: 1.25f,
                description: "Changes your speed. 1 = normal speed, 3 = very fast."
            );
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Player.Instance.jumpMultiplier = 1.1f * Mathf.Clamp(speed.Value, 1, 3);
            Player.Instance.maxJumpSpeed = 6.5f * Mathf.Clamp(speed.Value, 1, 3);
        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Player.Instance.jumpMultiplier = 1.1f;
            Player.Instance.maxJumpSpeed = 6.5f;
        }
    }
}