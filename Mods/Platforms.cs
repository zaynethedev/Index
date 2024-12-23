using GorillaLocomotion.Climbing;
using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Platforms", "Spawns collidable objects under your hand every time you press grip.", "Platforms", 1)]
    class Platforms : ModHandler
    {
        public static Platforms instance;
        public Vector3 platformOffsetL = new Vector3(0f, -0.025f, 0f), platformOffsetR = new Vector3(0f, -0.025f, 0f);
        public GameObject platformL, platformR;
        public Transform platformTransformL, platformTransformR;
        public static bool platSetR = false, platSetL = false;
        public Vector3 platSize = new Vector3(0.3f, 0.06f, 0.3f);
        public Color platColor = GorillaTagger.Instance.offlineVRRig.playerColor;

        public override void Start()
        {
            base.Start();
            instance = this;
            platformL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platformL.AddComponent<GorillaSurfaceOverride>();
            platformL.GetComponent<MeshRenderer>().material = new Material(Plugin.indexPanel.transform.Find("ShaderInit_Platforms").GetComponent<MeshRenderer>().materials[0]);
            platformL.GetComponent<MeshRenderer>().material.SetFloat("_Fill", 0.075f);
            platformL.name = "GorillaLeftPlatform";
            platformL.transform.position = Vector3.zero;
            platformL.transform.localScale = new Vector3(0.3f, 0.06f, 0.3f);
            var ClimbableL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ClimbableL.name = "PlatformStickyManagerL";
            ClimbableL.transform.localScale = platformR.transform.localScale * 2;
            ClimbableL.transform.SetParent(platformTransformR, false);
            ClimbableL.GetComponent<Renderer>().enabled = false;
            ClimbableL.AddComponent<GorillaClimbable>();
            ClimbableL.layer = LayerMask.NameToLayer("GorillaInteractable");

            platformR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platformR.AddComponent<GorillaSurfaceOverride>();
            platformR.GetComponent<MeshRenderer>().material = new Material(Plugin.indexPanel.transform.Find("ShaderInit_Platforms").GetComponent<MeshRenderer>().materials[0]);
            platformR.GetComponent<MeshRenderer>().material.SetFloat("_Fill", 0.075f);
            platformR.name = "GorillaRightPlatform";
            platformR.transform.position = Vector3.zero;
            platformR.transform.localScale = new Vector3(0.3f, 0.06f, 0.3f);
            var ClimbableR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ClimbableR.name = "PlatformStickyManagerR";
            ClimbableR.transform.localScale = platformR.transform.localScale * 2;
            ClimbableR.transform.SetParent(platformTransformR, false);
            ClimbableR.GetComponent<Renderer>().enabled = false;
            ClimbableR.AddComponent<GorillaClimbable>();
            ClimbableR.layer = LayerMask.NameToLayer("GorillaInteractable");

            platformTransformL = platformL.transform;
            platformTransformR = platformR.transform;
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            platColor = GorillaTagger.Instance.offlineVRRig.playerColor;
            if (ControllerInputPoller.instance.rightControllerGripFloat >= 0.5)
            {
                if (!platSetR)
                {
                    platSetR = true;
                    platformTransformR.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position + new Vector3(0, -0.1f, 0);
                    platformTransformR.rotation = Quaternion.Euler(0, -90, 0);
                    platformR.GetComponent<MeshRenderer>().material.SetColor("_OuterPlatformColor", platColor);
                    platformTransformR.Translate(platformOffsetR);
                }
            }
            else
            {
                platSetR = false;
                platformTransformR.position = Vector3.zero;
                platformTransformR.rotation = Quaternion.identity;
            }
            if (ControllerInputPoller.instance.leftControllerGripFloat >= 0.5)
            {
                if (!platSetL)
                {
                    platSetL = true;
                    platformTransformL.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position + new Vector3(0, -0.1f, 0);
                    platformTransformL.rotation = Quaternion.Euler(0, -90, 0);
                    platformL.GetComponent<MeshRenderer>().material.SetColor("_OuterPlatformColor", platColor);
                    platformTransformL.Translate(platformOffsetL);
                }
            }
            else
            {
                platSetL = false;
                platformTransformL.position = Vector3.zero;
                platformTransformL.rotation = Quaternion.identity;
            }
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            platformL.transform.position = Vector3.zero; platformR.transform.position = Vector3.zero;
            platformL.SetActive(false); platformR.SetActive(false);
            if (NoClip.instance.enabled)
            {
                NoClip.instance.OnModDisabled();
            }
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();
            platformL.SetActive(true); platformR.SetActive(true);
            platSetL = false; platSetR = false;
        }
    }
}