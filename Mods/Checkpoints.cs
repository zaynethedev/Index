using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Photon.Pun;

namespace Index.Mods
{
    [IndexMod("Checkpoint Mod", "Lets you create a checkpoint and teleport to it.", "Checkpoints", 13)]
    class Checkpoints : ModHandler
    {
        public static Checkpoints instance;

        MeshCollider[] MeshColliders;
        GameObject point;
        bool isPressingDown;

        bool checkpointCreated;

        Dictionary<String, GameObject> checkpoints = new Dictionary<String, GameObject>();
        Photon.Realtime.Player[] playerList;

        public override void Start()
        {
            base.Start();

            instance = this;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();

            foreach (var player in playerList)
            {
                CreateCheckpoint(player, false, UnityEngine.Color.black);
            }

            if (Keyboard.current.yKey.isPressed || ControllerInputPoller.instance.leftControllerSecondaryButton)
            {
                if (isPressingDown) return;

                var playerColor = GorillaTagger.Instance.offlineVRRig.playerColor;
                var checkpointPos = Player.Instance.LastLeftHandPosition;

                Console.WriteLine("creating checkpoint");

                point = CreateCheckpoint(PhotonNetwork.LocalPlayer, true, playerColor);

                point.transform.position = checkpointPos;

                SetNetworking(playerColor, checkpointPos);
            }

            else if (Keyboard.current.bKey.isPressed || ControllerInputPoller.instance.rightControllerSecondaryButton)
            {

                if (!checkpointCreated || isPressingDown) return;
                TeleportPlayer(point.transform.position);
            }

            isPressingDown = Keyboard.current.yKey.isPressed || Keyboard.current.bKey.isPressed ||
                                ControllerInputPoller.instance.leftControllerSecondaryButton || ControllerInputPoller.instance.rightControllerSecondaryButton;
        }



        public override void OnModDisabled()
        {
            base.OnModDisabled();

            NetworkSystem.Instance.OnPlayerJoined -= OnPlayerJoined;
            NetworkSystem.Instance.OnPlayerLeft -= OnPlayerLeft;

            foreach (var checkpoint in checkpoints.Values)
            {
                UnityEngine.Object.Destroy(checkpoint);
            }
            SetNetworking(UnityEngine.Color.black, Vector3.zero);
            checkpoints.Clear();
            point = null;
            checkpointCreated = false;
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();

            playerList = PhotonNetwork.PlayerListOthers;

            MeshColliders = UnityEngine.Resources.FindObjectsOfTypeAll<MeshCollider>();

            NetworkSystem.Instance.OnPlayerJoined += OnPlayerJoined;
            NetworkSystem.Instance.OnPlayerLeft += OnPlayerLeft;
        }

        void OnPlayerJoined(NetPlayer player)
        {
            playerList = PhotonNetwork.PlayerListOthers;
        }

        void OnPlayerLeft(NetPlayer player)
        {
            playerList = PhotonNetwork.PlayerListOthers;
            UnityEngine.Object.Destroy(checkpoints[player.UserId]);
            checkpoints.Remove(player.UserId);
        }
        GameObject CreateCheckpoint(Photon.Realtime.Player player, bool isSelf, UnityEngine.Color color)
        {

            Vector3 checkpointPos = Vector3.zero;

            var playerUserID = player.UserId;

            if (isSelf && checkpointCreated) return point;

            if (!isSelf)
            {

                if (!player.CustomProperties.ContainsKey("cPos")) { return point; }
                checkpointPos = (Vector3)player.CustomProperties["cPos"];
                var colorArray = (float[])player.CustomProperties["cColor"];
                color = new UnityEngine.Color(colorArray[0], colorArray[1], colorArray[2]);

                if (checkpoints.ContainsKey(playerUserID))
                {
                    checkpoints[playerUserID].transform.position = checkpointPos;
                    return point;
                }

                if (checkpointPos == Vector3.zero)
                {
                    try
                    {
                        UnityEngine.Object.Destroy(checkpoints[playerUserID]);
                        checkpoints.Remove(playerUserID);
                    }
                    catch { }
                    return null;
                }
            }
            else { checkpointCreated = true; }


            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = color
            };

            var checkpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            checkpoint.GetComponent<Renderer>().material = material;
            checkpoint.GetComponent<SphereCollider>().enabled = false;

            checkpoint.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            checkpoint.name = playerUserID;

            if (!isSelf) { checkpoint.transform.position = checkpointPos; }

            try
            {
                checkpoints.Add(playerUserID, checkpoint);
            }
            catch { }
            return checkpoint;
        }
        async void TeleportPlayer(Vector3 checkpointPos)
        {
            GorillaTagger.Instance.rigidbody.maxLinearVelocity = 0;

            foreach (MeshCollider Coliders in MeshColliders) { Coliders.enabled = false; }

            Player.Instance.headCollider.transform.position = checkpointPos;
            await Task.Delay(5);

            foreach (MeshCollider Coliders in MeshColliders) { Coliders.enabled = true; }

            GorillaTagger.Instance.rigidbody.maxLinearVelocity = 10000000000000000;

            return;
        }

        void SetNetworking(UnityEngine.Color checkpointColor, Vector3 checkpointPos)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
            {
                {"cPos", checkpointPos },
                {"cColor", new float[] { checkpointColor.r, checkpointColor.g, checkpointColor.b } }
            });
        }
    }
}