using Index.Resources;
using GorillaLocomotion;

namespace Index.Mods
{
    class SpeedBoost : IndexMod
    {
        public static SpeedBoost instance;

        public SpeedBoost()
        {
            modName = "Speed Boost";
            modDescription = "A mod that increases your speed, making it easier to run.";
            modGUID = "SpeedBoost";
            modID = 2;
            modType = ModType.gameplay;
        }

        public override void Start()
        {
            base.Start();
            instance = this;
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            Player.Instance.jumpMultiplier = 1.3f;
            Player.Instance.maxJumpSpeed = 8.5f;
        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Player.Instance.jumpMultiplier = 1.1f;
            Player.Instance.maxJumpSpeed = 6.5f;
        }
    }
}