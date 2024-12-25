using TMPro;
using UnityEngine;

namespace Index.Scripts
{
    public class IndexConfig
    {
        private enum Mods
        {
            BigMonke,
            BombMonke,
            BounceMonke,
            CarMonke,
            Checkpoints,
            Fly,
            LowGravity,
            Platforms,
            SmallMonke,
            SpeedBoost,
            MenuColor
        }
        private enum config
        {

        }

        private Mods currentMod;
        private TextMeshPro selmodtext;
        private TextMeshPro selmodtext2;
        private TextMeshPro selmodtext3;

        public void Start()
        {
            GorillaTagger.OnPlayerSpawned(Init);
        }
        public void Init()
        {
            GameObject thingding = GameObject.Find("SettingsPage/SelectedMod/SelectedModPanel/Text");
            GameObject thingding2 = GameObject.Find("SettingsPage/ModOption/ConfigOptionPanel/Text");
            GameObject thingding3 = GameObject.Find("SettingsPage/ModConfig/ModConfigPanel/Text");
            selmodtext = thingding.GetComponent<TextMeshPro>();
            selmodtext2 = thingding2.GetComponent<TextMeshPro>();
            selmodtext3 = thingding3.GetComponent<TextMeshPro>();
            currentMod = Mods.Platforms;
            TextStuff();
        }

        public void ChangeOption(bool dnwMod)
        {
            if (dnwMod)
            {
                if ((int)currentMod > 0)
                {
                    currentMod = (Mods)((int)currentMod - 1);
                }
                else
                {
                    currentMod = (Mods)(System.Enum.GetValues(typeof(Mods)).Length - 1);
                }
                TextStuff();
            }
            else
            {
                int totalMods = System.Enum.GetValues(typeof(Mods)).Length;
                currentMod = (Mods)(((int)currentMod + 1) % totalMods);
                TextStuff();
            }
        }

        private void TextStuff()
        {
            selmodtext.text = currentMod.ToString();

            selmodtext2.text = currentConfigOpt.ToString();

            selmodtext3.text = currentConfig.ToString();

        }
    }
}
