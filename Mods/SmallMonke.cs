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
        public Hashtable hash = new Hashtable();
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
            hash.AddOrUpdate("indexSize", 1f);
            Plugin.indexPanel.transform.localScale = originalIndexPanelSize;
            PhotonNetwork.SetPlayerCustomProperties(hash);
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            hash.AddOrUpdate("indexSize", 0.25f);
            PhotonNetwork.SetPlayerCustomProperties(hash);
            Plugin.indexPanel.transform.localScale *= 0.25f;
            if (BigMonke.instance.enabled)
                BigMonke.instance.OnModDisabled();
        }
    }
}