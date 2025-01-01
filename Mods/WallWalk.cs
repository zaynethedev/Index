using Index.Resources;
using GorillaLocomotion;
using System.Reflection;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Wall Walk", "Lets you stick onto the walls.", "WallWalk", 4)]
    class WallWalk : ModHandler
    {
        public static WallWalk instance;
        public RaycastHit hit;
        public Vector3 grav;

        public override void Start()
        {
            base.Start();
            instance = this;
            grav = Physics.gravity;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Player.Instance.wasLeftHandTouching || Player.Instance.wasRightHandTouching)
            {
                hit = GetLastHitInfo();
                Physics.gravity = hit.normal * -grav.magnitude * 1.175f;
            }
            else
            {
                if (Vector3.Distance(Player.Instance.bodyCollider.transform.position, hit.point) > 1.5f * Player.Instance.scale)
                {
                    Physics.gravity = grav * 1.5f;
                }
            }
        }

        private RaycastHit GetLastHitInfo()
        {
            FieldInfo fieldInfo = typeof(Player).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
            return (RaycastHit)fieldInfo.GetValue(Player.Instance);
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Physics.gravity = grav;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
        }
    }
}