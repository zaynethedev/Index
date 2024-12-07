using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using HarmonyLib;
using Index.Mods;
using BepInEx;
using System.Collections.Generic;
using System;
namespace Index.Mods
{
    class NoClip : IndexMod
    {
        public static NoClip instance;

        public NoClip()
        {
            modName = "NoClip";
            modDescription = "Makes you non-collidable.";
            modGUID = "NoClip";
            modID = 9;
            modType = ModType.testing;

        }

        public override void Start()
        {
            base.Start();
            instance = this;
            if (NoClipHelper.Instance == null)
            {
                GameObject helperObject = new GameObject("NoClipHelper");
                helperObject.AddComponent<NoClipHelper>();
            }
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (ControllerInputPoller.instance.leftControllerIndexFloat >= 0.5 || ControllerInputPoller.instance.rightControllerIndexFloat >= 0.5)
            {
                MeshCollider[] array = NoClipHelper.Instance.FindAllObjectsOfType<MeshCollider>();

                foreach (MeshCollider meshCollider in array)
                {
                    meshCollider.enabled = false;
                }
            }
            else
            {
                MeshCollider[] array = NoClipHelper.Instance.FindAllObjectsOfType<MeshCollider>();

                foreach (MeshCollider meshCollider in array)
                {
                    meshCollider.enabled = true;
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
            Platforms.instance.OnModEnabled();
        }
    }
}