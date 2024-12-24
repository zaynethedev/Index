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
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (ControllerInputPoller.instance.leftControllerGripFloat >= 0.5f && ControllerInputPoller.instance.rightControllerGripFloat >= 0.5f)
            {
                Player.Instance.bodyCollider.attachedRigidbody.AddForce(10 * Player.Instance.rightControllerTransform.right, ForceMode.Acceleration);
                Player.Instance.bodyCollider.attachedRigidbody.AddForce(10 * -Player.Instance.leftControllerTransform.right, ForceMode.Acceleration);
                var gtins = GorillaTagger.Instance;
                gtins.StartVibration(true, gtins.tapHapticStrength / 50f * gtins.bodyCollider.attachedRigidbody.velocity.magnitude, gtins.tapHapticDuration);
                gtins.StartVibration(false, gtins.tapHapticStrength / 50f * gtins.bodyCollider.attachedRigidbody.velocity.magnitude, gtins.tapHapticDuration);
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