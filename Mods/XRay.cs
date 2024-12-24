using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("XRay", "A mod that allows you to see other players through walls.", "XRay", 15)]
    class XRay : ModHandler
    {
        public static XRay instance;

        public override void Start()
        {
            base.Start();
            instance = this;
            //NetworkSystem.Instance.OnPlayerJoined += OnPlayerJoined;
            //NetworkSystem.Instance.OnPlayerLeft += OnPlayerLeft;
            
        }

        private void OnPlayerJoined(NetPlayer player)
        {
            /*
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.OwningNetPlayer == player && !rig.isLocal)
                {
                    rig.skeleton.enabled = true;
                    rig.skeleton.renderer.enabled = true;
                    rig.skeleton.renderer.material.shader = Shader.Find("GUI/Text Shader");
                    rig.skeleton.renderer.material.color = rig.playerColor;
                }
            }
            */
        }

        private void OnPlayerLeft(NetPlayer player)
        {
            /*
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.OwningNetPlayer == player)
                {
                    rig.skeleton.renderer.enabled = false;
                    rig.skeleton.renderer.material.shader = Shader.Find("GorillaTag/UberShader");
                    rig.skeleton.renderer.material.color = rig.playerColor;
                }
            }
            */
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isLocal)
                {
                    rig.skeleton.enabled = true;
                    rig.skeleton.renderer.enabled = true;
                    rig.skeleton.renderer.material.shader = Shader.Find("GUI/Text Shader");
                    rig.skeleton.renderer.material.color = rig.playerColor;
                }
            }
            Plugin.Penis.SetActive(true);
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isLocal)
                {
                    rig.skeleton.renderer.enabled = false;
                    rig.skeleton.renderer.material.shader = Shader.Find("GorillaTag/UberShader");
                }
            }
            Plugin.Penis.SetActive(false);



        }
    }
}