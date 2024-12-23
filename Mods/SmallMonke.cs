using ExitGames.Client.Photon;
using GorillaNetworking;
using Index.Resources;
using Photon.Pun;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Small Monke", "Makes you small.", "SmallMonke", 12)]
    class SmallMonke : ModHandler
    {
        public static SmallMonke instance;
        public Vector3 originalIndexPanelSize = new Vector3(0.16f, 0.16f, 0.16f);

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GorillaLocomotion.Player.Instance.scale = 0.25f;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Plugin.indexPanel.transform.localScale = originalIndexPanelSize;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Plugin.indexPanel.transform.localScale *= 0.25f;
            if (BigMonke.instance.enabled)
                BigMonke.instance.OnModDisabled();
        }
    }
}