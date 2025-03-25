using BepInEx.Configuration;
using GorillaLocomotion;
using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Slide Control", "Increases your slide control on slippery surfaces.", "SlideControl", 18)]
    class SlideControl : ModHandler
    {
        public static SlideControl instance;
        public ConfigEntry<float> slideControlMultiplier;
        private float slideControl;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void SetConfig()
        {
            base.SetConfig();
            slideControlMultiplier = Plugin.config.Bind(
                section: "Slide Control",
                key: "Slide Control Multiplier",
                defaultValue: 3.25f,
                description: "Changes your slide control. 2.5 = weak, 5 = extreme."
            );
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            GTPlayer.Instance.slideControl = slideControl;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            slideControl = GTPlayer.Instance.slideControl;
            GTPlayer.Instance.slideControl = slideControl * Mathf.Clamp(slideControlMultiplier.Value, 2.5f, 5f);
        }
    }
}