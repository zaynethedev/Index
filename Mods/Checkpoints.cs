using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using Index.Resources;
using GorillaLocomotion;

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

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Keyboard.current.yKey.isPressed || ControllerInputPoller.instance.leftControllerSecondaryButton)
            {
                if (!isPressingDown)
                {
                    var checkpointPos = GTPlayer.Instance.LastLeftHandPosition;
                    point = CreateCheckpoint();
                    point.transform.position = checkpointPos;
                }
            }
            else if (Keyboard.current.bKey.isPressed || ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                if (!checkpointCreated || isPressingDown) return;
                TeleportPlayer(point.transform.position);
            }
            isPressingDown = ControllerInputPoller.instance.leftControllerSecondaryButton || ControllerInputPoller.instance.rightControllerSecondaryButton;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            UnityEngine.Object.Destroy(point);
            checkpointCreated = false;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            MeshColliders = UnityEngine.Resources.FindObjectsOfTypeAll<MeshCollider>();
        }

        GameObject CreateCheckpoint()
        {
            Vector3 checkpointPos = Vector3.zero;
            if (checkpointCreated) return point;
            checkpointCreated = true;
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
            var checkpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            checkpoint.GetComponent<Renderer>().material = material;
            checkpoint.GetComponent<SphereCollider>().enabled = false;
            checkpoint.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            checkpoint.name = "Checkpoint";
            return checkpoint;
        }

        async void TeleportPlayer(Vector3 checkpointPos)
        {
            GorillaTagger.Instance.rigidbody.maxLinearVelocity = 0;
            foreach (MeshCollider Coliders in MeshColliders) { Coliders.enabled = false; }
            GTPlayer.Instance.headCollider.transform.position = checkpointPos;
            await Task.Delay(5);
            foreach (MeshCollider Coliders in MeshColliders) { Coliders.enabled = true; }
            GorillaTagger.Instance.rigidbody.maxLinearVelocity = 10000000000000000;
            return;
        }
    }
}