using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using System.Threading.Tasks;

namespace Index.Mods
{
    class ModTemplate : IndexMod
    {
        public static ModTemplate instance;

        MeshCollider[] MeshColliders;
        GameObject point;
        bool isPressingDown;

        public ModTemplate()
        {
            modName = "Checkpoint Mod";
            modDescription = "Lets you create a checkpoint and teleport to it.";
            modGUID = "Checkpoints";
            modID = 13;
            modType = ModType.gameplay;
        }

        public override void Start()
        {
            base.Start();
            instance = this;
            MeshColliders = UnityEngine.Resources.FindObjectsOfTypeAll<MeshCollider>();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (ControllerInputPoller.instance.leftControllerSecondaryButton)
            {
                if (isPressingDown) return;
                point.transform.position = Player.Instance.LastLeftHandPosition;
            }

            else if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {

                if (point == null || isPressingDown) return;
                TeleportPlayer(point.transform.position);
            }

            isPressingDown = ControllerInputPoller.instance.leftControllerSecondaryButton || ControllerInputPoller.instance.rightControllerSecondaryButton;
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Object.Destroy(point);
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = GorillaTagger.Instance.offlineVRRig.playerColor
            };
            point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.GetComponent<Renderer>().material = material;
            point.GetComponent<SphereCollider>().enabled = false;
            point.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
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
    }
}