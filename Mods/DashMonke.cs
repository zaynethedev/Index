using BepInEx.Configuration;
using GorillaLocomotion;
using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Dash Monke", "Allows you to fling yourself into the direction you are looking. MOD TRIGGER: X", "DashMonke", 16)]
    class DashMonke : ModHandler
    {
        public static DashMonke instance;
        public ConfigEntry<float> dashSpeed;
        public bool isDash;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void SetConfig()
        {
            base.SetConfig();
            dashSpeed = Plugin.config.Bind(
                section: "Dash",
                key: "Speed Multiplier",
                defaultValue: 12.5f,
                description: "Changes your fly speed. 12.5f = default, 25f = extreme."
            );
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (ControllerInputPoller.instance.leftControllerPrimaryButton && !isDash)
            {
                isDash = true;
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(GTPlayer.Instance.headCollider.transform.forward * dashSpeed.Value, ForceMode.VelocityChange);
            }
            else if (!ControllerInputPoller.instance.leftControllerPrimaryButton && isDash)
            {
                isDash = false;
            }
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
        }
    }
}
