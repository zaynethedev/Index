using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Index.Scripts
{
    public class XRayHelper : MonoBehaviourPunCallbacks
    {
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            try
            {
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
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            base.OnPlayerEnteredRoom(newPlayer);
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            try
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    rig.skeleton.renderer.enabled = false;
                    rig.skeleton.renderer.material.shader = Shader.Find("GorillaTag/UberShader");
                    rig.skeleton.renderer.material.color = rig.playerColor;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            base.OnPlayerLeftRoom(otherPlayer);
        }
    }

}