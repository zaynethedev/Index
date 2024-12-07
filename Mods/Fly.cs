using Index.Resources;
using GorillaLocomotion;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Fly", "Allows you to glide in the air with your controller buttons.", "Fly", 3)]
    class Fly : ModHandler
    {
        public static Fly instance;

        public override void Start()
        {
            base.Start();
            instance = this;
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (ControllerInputPoller.instance.leftControllerPrimaryButton || ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                var direction = Player.Instance.headCollider.transform.forward;
                var rigidbody = Player.Instance.bodyCollider.attachedRigidbody;
                Vector3 velocity = direction * 7.5f;
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, velocity, 0.25f);
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