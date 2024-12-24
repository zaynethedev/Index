using BepInEx.Configuration;
using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Small Monke", "Makes you small.", "SmallMonke", 12)]
    class SmallMonke : ModHandler
    {
        public static SmallMonke instance;
        public Vector3 originalIndexPanelSize = new Vector3(0.16f, 0.16f, 0.16f);
        public ConfigEntry<float> size;

        public override void Start()
        {
            base.Start();
            instance = this;
            size = Plugin.config.Bind(
                section: "Size Changers",
                key: "Small Monkke Size",
                defaultValue: 0.25f,
                description: "Changes your size. 0.1 = very small, 0.9 = slightly small"
            );
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GorillaLocomotion.Player.Instance.scale = Mathf.Clamp(size.Value, 0.1f, 0.9f);
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Plugin.indexPanel.transform.localScale = originalIndexPanelSize;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Plugin.indexPanel.transform.localScale *= Mathf.Clamp(size.Value, 0.1f, 0.9f);
            if (BigMonke.instance.enabled)
                BigMonke.instance.OnModDisabled();
        }
    }
}