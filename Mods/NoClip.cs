using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("No-Clip", "Makes you non-collidable.", "NoClip", 9)]
    class NoClip : ModHandler
    {
        public static NoClip instance;

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
        public override void OnUpdate()
        {
            base.OnUpdate();
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
            Platforms.instance.OnModDisabled();
            base.OnModDisabled();
            MeshCollider[] array = NoClipHelper.Instance.FindAllObjectsOfType<MeshCollider>();
            foreach (MeshCollider meshCollider in array)
            {
                meshCollider.enabled = true;
            }
        }
        public override void OnModEnabled()
        {
            Platforms.instance.OnModEnabled();
            base.OnModEnabled();
        }
    }
}