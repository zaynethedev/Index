using System.Linq;
using UnityEngine;
using System.Collections;
using TMPro;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

namespace Index.Resources
{
    internal class ButtonManager : GorillaPressableButton
    {
        public static ButtonManager instance;
        public static bool isCooldown = false;
        public float cooldownTime = 0.1f;
        public int modIDSettings = 1;
        public int configIDSettings = 0;
        public static Material unselectedMaterial = new Material(Plugin.indexPanel.transform.Find("ShaderInit_UnselectedButton").GetComponent<MeshRenderer>().materials[0]);
        public static Material selectedMaterial = new Material(Plugin.indexPanel.transform.Find("ShaderInit_SelectedButton").GetComponent<MeshRenderer>().materials[0]);

        public override void Start()
        {
            instance = this;
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.layer = 18;
            gameObject.GetComponent<MeshRenderer>().material = unselectedMaterial;
        }

        public override void ButtonActivation()
        {
            string buttonName = gameObject.name;
            if (isCooldown)
            {
                return;
            }

            if (int.TryParse(buttonName, out int modID))
            {
                ModHandler mod = Plugin.mods.FirstOrDefault(m => m.modID == modID);

                if (mod != null)
                {
                    if (mod.enabled)
                    {
                        mod.OnModDisabled();
                        StartCoroutine(Cooldown());
                    }
                    else
                    {
                        mod.OnModEnabled();
                        StartCoroutine(Cooldown());
                    }
                }
            }
            else
            {
                switch (gameObject.name)
                {
                    case "Page1":
                        if (!Plugin.indexPanel.transform.Find("Mods/page1").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(true);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(false);
                            StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
                        }
                        break;
                    case "Page2":
                        if (!Plugin.indexPanel.transform.Find("Mods/page2").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(true);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(false);
                            StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
                        }
                        break;
                    case "Settings":
                        if (!Plugin.indexPanel.transform.Find("SettingsPage").gameObject.activeSelf)
                        {
                            Plugin.indexPanel.transform.Find("Mods/page1").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("Mods/page2").gameObject.SetActive(false);
                            Plugin.indexPanel.transform.Find("SettingsPage").gameObject.SetActive(true);
                            StartCoroutine(ChangeMaterialWithDelay(gameObject, selectedMaterial, unselectedMaterial, 0.1f));
                        }
                        break;
                    case "NextMod":
                        modIDSettings = modIDSettings < 16 ? modIDSettings + 1 : 1;
                        UpdateModSelection();
                        break;
                    case "PreviousMod":
                        modIDSettings = modIDSettings > 1 ? modIDSettings - 1 : Plugin.mods.Count;
                        UpdateModSelection();
                        break;
                    case "NextConfig":
                        configIDSettings = (configIDSettings + 1) % Plugin.mods.Count;
                        UpdateConfigSelection();
                        break;
                    case "PreviousConfig":
                        configIDSettings = configIDSettings > 0 ? configIDSettings - 1 : Plugin.mods.Count - 1;
                        UpdateConfigSelection();
                        break;
                }
            }
        }

        ConfigEntryBase GetEntry(string modName, string key)
        {
            foreach (var definition in Plugin.config.Keys)
            {
                if (definition.Section == modName && definition.Key == key)
                {
                    return Plugin.config[definition];
                }
            }
            throw new Exception($"Could not find config entry for {modName} with key {key}");
        }

        List<string> GetConfigKeys(string modName)
        {
            List<string> configKeys = new List<string>();
            foreach (var definition in Plugin.config.Keys)
            {
                if (definition.Section == modName)
                {
                    configKeys.Add(Plugin.config[definition].Definition.Key);
                }
            }
            return configKeys;
        }

        private IEnumerator ChangeMaterialWithDelay(GameObject obj, Material selected, Material unselected, float delaySeconds)
        {
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.material = selected;
            yield return new WaitForSeconds(delaySeconds);
            meshRenderer.material = unselected;
        }

        public IEnumerator Cooldown()
        {
            isCooldown = true;
            yield return new WaitForSeconds(cooldownTime);
            isCooldown = false;
        }

        private void UpdateModSelection()
        {
            string modName = Plugin.mods[modIDSettings - 1].modName;
            Plugin.indexPanel.transform.Find("SettingsPage/SelectedMod/SelectedModPanel/Text").GetComponent<TextMeshPro>().text = modName;
        }

        private void UpdateConfigSelection()
        {
            string modName = Plugin.mods[modIDSettings - 1].modName;
            string configName = GetConfigKeys(modName)[configIDSettings];
            Plugin.indexPanel.transform.Find("SettingsPage/ModConfig/ModConfigPanel/Text").GetComponent<TextMeshPro>().text = configName;
        }
    }
}
