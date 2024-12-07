using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Index.Resources
{
    internal class NetworkingManager : MonoBehaviour
    {
        public Photon.Realtime.Player[] playerList;
        public NetworkingManager instance;

        public void Start()
        {
            instance = this;
            NetworkSystem.Instance.OnPlayerJoined += OnPlayerJoin;
            NetworkSystem.Instance.OnPlayerLeft += OnPlayerLeave;
        }

        public void SetNetworking(Hashtable hash)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public void OnPlayerLeave(NetPlayer player)
        {
            playerList = PhotonNetwork.PlayerListOthers;
        }

        public void OnPlayerJoin(NetPlayer player)
        {
            playerList = PhotonNetwork.PlayerListOthers;
        }
    }
}
