using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using HarmonyLib;
using Index.Mods;
using Index.Scripts;
namespace Index.Mods
{
    [IndexMod("Frozone", "Spawns 'slides' under your hands", "Frozone", 0)]
    class ModTemplate : ModHandler
    {
        public static ModTemplate instance;
        public GameObject slide;

        public override void Start()
        {
            base.Start();
            instance = this;
            slide = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GorillaSurfaceOverride gso = slide.AddComponent<GorillaSurfaceOverride>();
            gso.slidePercentageOverride = 1f;
            gso.transform.localScale = new Vector3(1, 0.025f, 1);
            slide.SetActive(false);
        }
        public override void OnFixedUpdate()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat >= 0.5)
            {
                GameObject tempSlide = GameObject.Instantiate(slide);
                tempSlide.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                tempSlide.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                tempSlide.AddComponent<SlideScript>();
            }
            if (ControllerInputPoller.instance.leftControllerGripFloat >= 0.5)
            {
                GameObject tempSlide = GameObject.Instantiate(slide);
                tempSlide.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                tempSlide.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                tempSlide.AddComponent<SlideScript>();
            }
            base.OnFixedUpdate();

        }
        public override void OnModDisabled()
        {
            base.OnModDisabled();
        }
        public override void OnModEnabled()
        {
            base.OnModEnabled();
        }
    }
}