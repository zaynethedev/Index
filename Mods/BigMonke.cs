using BepInEx.Configuration;
using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Big Monke", "Makes you big.", "BigMonke", 11)]
    class BigMonke : ModHandler
    {
        public static BigMonke instance;
        public Vector3 originalIndexPanelSize = new Vector3(0.16f, 0.16f, 0.16f);
        public ConfigEntry<float> size;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void SetConfig()
        {
            base.SetConfig();
            size = Plugin.config.Bind(
                section: "Big Monke",
                key: "Size",
                defaultValue: 1.25f,
                description: "Changes your size. 1 = sliQghtly big, 2 = giant"
            );
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GorillaLocomotion.Player.Instance.scale = Mathf.Clamp(size.Value, 1, 2);
        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Plugin.indexPanel.transform.localScale = originalIndexPanelSize;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Plugin.indexPanel.transform.localScale *= Mathf.Clamp(size.Value, 1, 2);
            if (SmallMonke.instance.enabled)
                SmallMonke.instance.OnModDisabled();
        }
    }
}