using Index.Resources;
using UnityEngine;

namespace Index.Mods
{
    [IndexMod("Flood", "Use your triggers to control how high the water rises! Left Trigger: Lower, Right Trigger: Rise", "Flood", 17)]
    class Flood : ModHandler
    {
        public static Flood instance;
        public GameObject floodWater;

        public override void Start()
        {
            base.Start();
            instance = this;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (ControllerInputPoller.instance.leftControllerIndexFloat >= 0.5f)
            {
                floodWater.transform.position = new Vector3(floodWater.transform.position.x, floodWater.transform.position.y - 0.035f, floodWater.transform.position.z);
            }
            if (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.5f)
            {
                floodWater.transform.position = new Vector3(floodWater.transform.position.x, floodWater.transform.position.y + 0.035f, floodWater.transform.position.z);
            }
        }

        public override void OnModDisabled()
        {
            base.OnModDisabled();
            GameObject.Destroy(floodWater);
        }

        public override void OnModEnabled()
        {
            base.OnModEnabled();
            floodWater = GameObject.Instantiate(GameObject.Find("CaveWaterVolume"));
            floodWater.transform.localScale = new Vector3(3f, 500, 5f);
            floodWater.transform.position = new Vector3(-50, -5f, -60f);
            floodWater.transform.rotation = Quaternion.identity;
        }
    }
}