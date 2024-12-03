using ExitGames.Client.Photon;
using GorillaNetworking;
using Index.Resources;
using Photon.Pun;

namespace Index.Mods
{
    class BigMonke : IndexMod
    {
        public static BigMonke instance;
        public Hashtable hash = new Hashtable();

        public BigMonke()
        {
            modName = "Big Monke";
            modDescription = "Makes you big.";
            modGUID = "BigMonke";
            modID = 11;
            modType = ModType.gameplay;
        }

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
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            hash.AddOrUpdate("indexSize", 2f);
            PhotonNetwork.SetPlayerCustomProperties(hash);
        }
    }
}