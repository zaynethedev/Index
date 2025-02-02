using Index.Resources;
using GorillaLocomotion;
using System.Reflection;
using UnityEngine;

namespace Index.Mods
{
<<<<<<< Updated upstream
    [IndexMod("Wall Walk", "Lets you smoothly walk on walls.", "WallWalk", 4)]
=======
    [IndexMod("Wall Walk", "Lets you smoothly walk on walls and ceilings.", "WallWalk", 4)]
>>>>>>> Stashed changes
    class WallWalk : ModHandler
    {
        public static WallWalk instance;
        public RaycastHit hit;
        public Vector3 originalGravity;
        private Vector3 currentGravity;
        private Vector3 targetGravity;
<<<<<<< Updated upstream
        private float smoothness = 5.0f;
=======
        private float lerp = 6.5f;
        private float connect = 1.5f;
>>>>>>> Stashed changes

        public override void Start()
        {
            base.Start();
            instance = this;
            originalGravity = Physics.gravity;
            currentGravity = originalGravity;
            targetGravity = originalGravity;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Player.Instance.wasLeftHandTouching || Player.Instance.wasRightHandTouching)
            {
                hit = GetLastHitInfo();
<<<<<<< Updated upstream
                Vector3 wallNormal = hit.normal;
                Vector3 playerUp = Player.Instance.bodyCollider.transform.up;
                targetGravity = Vector3.Lerp(playerUp * -originalGravity.magnitude, wallNormal * -originalGravity.magnitude, 0.8f);
=======
                if (hit.distance <= connect)
                {
                    Vector3 surfaceNormal = hit.normal;
                    Vector3 playerUp = Player.Instance.bodyCollider.transform.up;
                    targetGravity = Vector3.Lerp(playerUp * -originalGravity.magnitude, surfaceNormal * -originalGravity.magnitude, 3f);
                }
>>>>>>> Stashed changes
            }
            else
            {
                targetGravity = originalGravity;
            }
<<<<<<< Updated upstream
            currentGravity = Vector3.Lerp(currentGravity, targetGravity, Time.deltaTime * smoothness);
=======
            currentGravity = Vector3.Lerp(currentGravity, targetGravity, Time.deltaTime * lerp);
>>>>>>> Stashed changes
            Physics.gravity = currentGravity;
        }

        private RaycastHit GetLastHitInfo()
        {
            FieldInfo fieldInfo = typeof(Player).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
            return (RaycastHit)fieldInfo.GetValue(Player.Instance);
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Physics.gravity = originalGravity;
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
        }
    }
}