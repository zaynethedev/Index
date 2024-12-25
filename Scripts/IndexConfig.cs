using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Index.Scripts
{
    public class IndexConfig
    {
        // only change this when adding new mods + make sure you add it to the dictionary
        private enum Mods
        {
            BigMonke,
            BombMonke,
            BounceMonke,
            CarMonke,
            Fly,
            LowGravity,
            Platforms,
            SmallMonke,
            SpeedBoost,
            MenuColor
        }

        // dont touch this stuff
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
            // mod slectection text
            selmodtext.text = currentMod.ToString();

            // config text
            Dictionary<Mods, string> modDescriptions = new Dictionary<Mods, string>()
            {
                { Mods.Platforms, "Sticky" },
                { Mods.SmallMonke, "0.9 = not that small, 0.1 = very small" },
                { Mods.SpeedBoost, "1 = normal speed, 3 = very fast" },
                { Mods.MenuColor, "Color" },
                { Mods.BigMonke, "1 = big, 2 = very big" },
                { Mods.BombMonke, "N/A for now" },
                { Mods.BounceMonke, "1 = normal bounce, 5 = SUPER BOUNCE" },
                { Mods.CarMonke, "N/A for now" },
                { Mods.Fly, "1 = slow, 2 = fast" },
                { Mods.LowGravity, "0 = no gravity, 1 = normal" }
            };

            if (modDescriptions.TryGetValue(currentMod, out string description))
            {
                selmodtext3.text = description;
            }
            else
            {
                selmodtext3.text = "ERROR";
            }

            // config option text


        }
    }
}
