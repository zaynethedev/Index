using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using HarmonyLib;
using Index.Mods;
namespace Index.Mods
{
    [IndexMod("Dash Monke", "Allows you to dash. Press any face button to dash.", "DashMonke", 16)]
    class DashMonke : ModHandler
    {
        public static DashMonke instance;
        private Rigidbody rb;
        private Vector3 force = new Vector3(5, 0, 0);

        public override void Start()
        {
            base.Start();
            instance = this;
            rb = Player.Instance.GetComponent<Rigidbody>();
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (ControllerInputPoller.instance.rightControllerPrimaryButton || ControllerInputPoller.instance.rightControllerSecondaryButton || ControllerInputPoller.instance.leftControllerPrimaryButton || ControllerInputPoller.instance.leftControllerSecondaryButton)
            {
                // RigidBody Movement
                if(rb != null)
                {
                    rb.AddForce(force, ForceMode.VelocityChange);
                }
                else if (rb == null)
                {
                    Debug.Log("RigidBody dont exist i think from what i know atleast...");
                }
            }
        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();
        }
    }
}