using BepInEx.Configuration;
using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Slow Motion", "Makes everything slow.", "SlowMotion", 16)]
    class SlowMotion : ModHandler
    {
        public static SlowMotion instance;
        public ConfigEntry<float> scale;

        public override void Start()
        {
            base.Start();
            instance = this;
            scale = Plugin.config.Bind(
                section: "Slow Motion",
                key: "Time Scale",
                defaultValue: 0.35f,
                description: "How slow the game runs upon enabling the mod."
            );
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Time.timeScale = 1;
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Time.timeScale = scale.Value;
        }
    }
}