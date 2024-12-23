using GorillaNetworking;
using Index.Resources;
using System.Collections;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Index Room", "Joins a room dedicated to Index users.", "IndexRoom", 16)]
    class IndexRoom : ModHandler
    {
        public static IndexRoom instance;

        public override void Start()
        {
            base.Start();
            instance = this;
            if (IndexRoomHelper.instance == null)
            {
                GameObject helperObject = new GameObject("IndexRoomHelper");
                helperObject.AddComponent<IndexRoomHelper>();
            }
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            IndexRoomHelper.instance.JoinRoom("_INDEXLOBBY");
        }
    }

    public class IndexRoomHelper : MonoBehaviour
    {
        public static IndexRoomHelper instance;
        public int roomId = 1;

        public void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void JoinRoom(string lobbyName)
        {
            StartCoroutine(JoinLobby(lobbyName));
        }

        IEnumerator JoinLobby(string lobbyName)
        {
            NetworkSystem.Instance.ReturnToSinglePlayer();
            if (NetworkSystem.Instance.InRoom)
            {
                yield return new WaitForSeconds(0.5f);
            }
            string mode = GorillaComputer.instance.currentGameMode.Value;
            GorillaComputer.instance.currentGameMode.Value = "MODDED_MODDED_CASUALCASUAL";
            try
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom($"{lobbyName}_{roomId}", JoinType.Solo);
            }
            catch
            {
                if (GorillaComputer.instance.roomFull)
                {
                    roomId++;
                    JoinRoom(lobbyName);
                }
            }
            if(!NetworkSystem.Instance.InRoom)
            {
                yield return new WaitForSeconds(0.5f);
            }
            GorillaComputer.instance.currentGameMode.Value = mode;
            IndexRoom.instance.OnModDisabled();
        }
    }
}