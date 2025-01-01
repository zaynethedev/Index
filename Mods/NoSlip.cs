using Index.Resources;

namespace Index.Mods
{
    [IndexMod("No-Slip", "Disables slipping mechanics.", "NoSlip", 5)]
    class NoSlip : ModHandler
    {
        public static NoSlip instance;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            SlipperyMonke.instance.OnModDisabled();
        }
    }
}