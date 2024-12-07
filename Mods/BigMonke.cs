using ExitGames.Client.Photon;
using GorillaNetworking;
using Index.Resources;
using Photon.Pun;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Big Monke", "Makes you big.", "BigMonke", 11)]
    class BigMonke : ModHandler
    {
        public static BigMonke instance;
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
            GorillaLocomotion.Player.Instance.scale = 2f;
        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
            hash.AddOrUpdate("indexSize", 1f);
            PhotonNetwork.SetPlayerCustomProperties(hash);
            Plugin.indexPanel.transform.localScale = originalIndexPanelSize;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            hash.AddOrUpdate("indexSize", 2f);
            PhotonNetwork.SetPlayerCustomProperties(hash);
            Plugin.indexPanel.transform.localScale *= 1.5f;
            SmallMonke.instance.OnModDisabled();
        }
    }
}