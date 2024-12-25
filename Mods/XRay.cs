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
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            Plugin.puncallbacks_xray.SetActive(true);
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            Plugin.puncallbacks_xray.SetActive(false);
        }
    }
}