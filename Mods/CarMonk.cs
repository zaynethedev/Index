using Index.Resources;
using UnityEngine;
using GorillaLocomotion;

namespace Index.Mods
{
    [IndexMod("Car Monk", "Lets you drive. Triggers to move", "CarMonk", 16)]
    class CarMonk : ModHandler
    {
        public static CarMonk instance;
        private float acceleration = 0.1f;
        private float maxSpeed = 30f;
        private float currentSpeed = 0f;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnFixedUpdate()
        {
            var rigidbody = Player.Instance.bodyCollider.attachedRigidbody;
            base.OnFixedUpdate();

            if (rigidbody != null)
            {
                Vector3 headForward = Player.Instance.headCollider.transform.forward;
                var gtins = GorillaTagger.Instance;
                Vector3 forwardDirection = Vector3.ProjectOnPlane(headForward, Vector3.up).normalized;

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    currentSpeed = Mathf.Clamp(currentSpeed + acceleration, 0, maxSpeed);
                    Vector3 velocity = forwardDirection * currentSpeed;
                    rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, velocity, 0.1f);
                    gtins.StartVibration(false, gtins.tapHapticStrength / 50f * rigidbody.velocity.magnitude, gtins.tapHapticDuration);
                }
                else if (ControllerInputPoller.instance.leftControllerIndexFloat > 0.1f)
                {
                    currentSpeed = Mathf.Clamp(currentSpeed + acceleration, 0, maxSpeed);
                    Vector3 velocity = forwardDirection * -currentSpeed;
                    rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, velocity, 0.1f);
                    gtins.StartVibration(true, gtins.tapHapticStrength / 50f * rigidbody.velocity.magnitude, gtins.tapHapticDuration);
                }
                else
                {
                    currentSpeed = Mathf.Max(0, currentSpeed - acceleration * 2);
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
            Platforms.instance.OnModDisabled();
        }
    }
}
