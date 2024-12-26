using Index.Resources;
using GorillaLocomotion;
using UnityEngine;
using HarmonyLib;
using Index.Mods;
namespace Index.Mods
{
    [IndexMod("Monke Pearl", "Its an ender pearl but monke", "MonkePearl", 19)]
    class MonkePearl : ModHandler
    {
        public static MonkePearl instance;

        public override void Start()
        {
            base.Start();
            instance = this;
        }
        public override void OnFixedUpdate()
        {
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