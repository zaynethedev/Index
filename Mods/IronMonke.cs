using GorillaLocomotion;
using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Iron Monke", "Pushes you in the direction you want to go. (Using your hands)", "BigMonke", 8)]
    class IronMonke : ModHandler
    {
        public static IronMonke instance;

        public override void Start()
        {
            base.Start();
            instance = this;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (ControllerInputPoller.instance.leftControllerGripFloat >= 0.5f && ControllerInputPoller.instance.rightControllerGripFloat >= 0.5f)
            {
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(32 * GTPlayer.Instance.rightControllerTransform.right, ForceMode.Acceleration);
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(32 * -GTPlayer.Instance.leftControllerTransform.right, ForceMode.Acceleration);
                Plugin.thrusterL.transform.Find("Particle System").gameObject.SetActive(true);
                Plugin.thrusterR.transform.Find("Particle System").gameObject.SetActive(true);
            }
            else
            {
                Plugin.thrusterL.transform.Find("Particle System").gameObject.SetActive(false);
                Plugin.thrusterR.transform.Find("Particle System").gameObject.SetActive(false);
            }
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Plugin.thrusterL.SetActive(false);
            Plugin.thrusterR.SetActive(false);
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Plugin.thrusterL.SetActive(true);
            Plugin.thrusterR.SetActive(true);
        }
    }
}