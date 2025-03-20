﻿using Index.Resources;
using GorillaLocomotion;
using System.Reflection;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Wall Walk", "Lets you smoothly walk on walls.", "WallWalk", 4)]
    class WallWalk : ModHandler
    {
        public static WallWalk instance;
        public RaycastHit hit;
        public Vector3 originalGravity;
        private Vector3 currentGravity;
        private Vector3 targetGravity;
        private float smoothness = 5.0f;

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

            if (Player.Instance.wasLeftHandColliding || Player.Instance.wasRightHandColliding)
            {
                hit = GetLastHitInfo();
                Vector3 wallNormal = hit.normal;
                Vector3 playerUp = Player.Instance.bodyCollider.transform.up;
                targetGravity = Vector3.Lerp(playerUp * -originalGravity.magnitude, wallNormal * -originalGravity.magnitude, 0.8f);
            }
            else
            {
                targetGravity = originalGravity;
            }
            currentGravity = Vector3.Lerp(currentGravity, targetGravity, Time.deltaTime * smoothness);
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