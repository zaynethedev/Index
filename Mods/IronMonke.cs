using GorillaLocomotion;
using Index.Resources;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Index.Mods
{
    class IronMonke : IndexMod
    {
        public static IronMonke instance;

        public IronMonke()
        {
            modName = "Iron Monke";
            modDescription = "Pushes you in the direction you want to go. (Using your hands)";
            modGUID = "IronMonke";
            modID = 8;
            modType = ModType.testing;
        }

        public override void Start()
        {
            base.Start();
            instance = this;
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (ControllerInputPoller.instance.leftControllerGripFloat >= 0.5f)
            {
                Player.Instance.bodyCollider.attachedRigidbody.AddForce(10 * -Player.Instance.leftControllerTransform.right, ForceMode.Acceleration);
                var gtins = GorillaTagger.Instance;
                gtins.StartVibration(true, gtins.tapHapticStrength / 50f * gtins.bodyCollider.attachedRigidbody.velocity.magnitude, gtins.tapHapticDuration);
            }
            if (ControllerInputPoller.instance.rightControllerGripFloat >= 0.5f)
            {
                Player.Instance.bodyCollider.attachedRigidbody.AddForce(10 * Player.Instance.rightControllerTransform.right, ForceMode.Acceleration);
                var gtins = GorillaTagger.Instance;
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