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

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GTPlayer.Instance.jumpMultiplier = 1.3f * 1.5f;
            GTPlayer.Instance.maxJumpSpeed = 8.5f * 1.5f;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            GTPlayer.Instance.jumpMultiplier = 1.1f;
            GTPlayer.Instance.maxJumpSpeed = 6.5f;
        }
    }
}