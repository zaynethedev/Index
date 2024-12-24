using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using BepInEx.Configuration;

namespace Index.Mods
{
    [IndexMod("Fly", "Allows you to glide in the air with your controller buttons.", "Fly", 3)]
    class Fly : ModHandler
    {
        public static Fly instance;
        public ConfigEntry<float> flySpeed;

        public override void Start()
        {
            base.Start();
            instance = this;
            flySpeed = Plugin.config.Bind(
                section: "Fly",
                key: "Speed Multiplier",
                defaultValue: 7.5f,
                description: "Changes your fly speed. 7.5f = slow, 22.5f = fast."
            );
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (ControllerInputPoller.instance.leftControllerPrimaryButton || ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                var direction = Player.Instance.headCollider.transform.forward;
                var rigidbody = Player.Instance.bodyCollider.attachedRigidbody;
                Vector3 velocity = direction * Mathf.Clamp(flySpeed.Value, 7.5f, 22.5f);
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