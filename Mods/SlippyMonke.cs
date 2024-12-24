using Index.Resources;

namespace Index.Mods
{
    [IndexMod("Slippy Monke", "Makes everything slippery.", "SlippyMonk", 6)]
    class SlipperyMonke : ModHandler
    {
        public static SlipperyMonke instance;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();
            NoSlip.instance.OnModDisabled();
        }
    }
}