using BepInEx;
using UnityEngine;
using Index.Resources;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using TMPro;
using GorillaLocomotion;
using HarmonyLib;
using BepInEx.Configuration;
using System.Linq;
using Index.Scripts;
using System.Net.Http;
using System.Threading.Tasks;
using Index.BepInfo;
using UnityEngine.InputSystem;
using Photon.Voice;

namespace Index
{
    [BepInPlugin(Info_Plugin.guid, Info_Plugin.name, Info_Plugin.version)]
    public class Plugin : BaseUnityPlugin
    {
        public static bool inRoom, initialized;
        public static List<ModHandler> initMods = new List<ModHandler>();
        public static GameObject indexPanel, thrusterL, thrusterR;
        public static Harmony harmony;
        public static ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "Index.cfg"), true);
        public ConfigEntry<Vector3> panelColorOuter;
        public ConfigEntry<Vector3> panelColorInner;

        void Start()
        {
            harmony = Harmony.CreateAndPatchAll(GetType().Assembly, "indexteam.Index");
            preInit();
            GorillaTagger.OnPlayerSpawned(init);
        }

        private async void version()
        {
            string warn = await fetch("https://raw.githubusercontent.com/zaynethedev/Index/main/warn.txt");
            if (!warn.Contains("none"))
            {
                if (!warn.Contains("version=none"))
                {
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().enableAutoSizing = true;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().fontSizeMin = 4;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().fontSizeMax = 12;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().color = Color.yellow;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().text = warn.ToUpper();
                }
                else
                {
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().enableAutoSizing = true;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().fontSizeMin = 4;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().fontSizeMax = 12;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().color = Color.yellow;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().text = warn.ToUpper();
                }
            }
            else
            {
                string onlineVersion = await fetch("https://raw.githubusercontent.com/zaynethedev/Index/main/ver.txt");
                if (!string.IsNullOrEmpty(onlineVersion) && onlineVersion == Info_Plugin.version)
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().text = $"INDEX v{Info_Plugin.version}";
                else
                {
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().enableAutoSizing = true;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().fontSizeMin = 4;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().fontSizeMax = 12;
                    indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().text = $"NEW VERSION AVAILABLE: v{onlineVersion}";
                }
            }
        }

        private async Task<string> fetch(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string content = await client.GetStringAsync(url);
                    return content.Trim().Replace('_', '.');
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return null;
            }
        }

        void preInit()
        {
            var bundle = LoadAssetBundle("Index.Resources.index");
            indexPanel = bundle.LoadAsset<GameObject>("IndexPanel");
            thrusterL = bundle.LoadAsset<GameObject>("MonkeThruster");
            thrusterR = bundle.LoadAsset<GameObject>("MonkeThruster");
            panelColorOuter = config.Bind(
                section: "Index Panel",
                key: "Outer Color",
                defaultValue: new Vector3(0.439f, 0f, 1f),
                description: "The outer panel color."
            );
            panelColorInner = config.Bind(
                section: "Index Panel",
                key: "Inner Color",
                defaultValue: new Vector3(0f, 0.961f, 1f),
                description: "The inner panel color."
            );
        }

        void init()
        {
            try
            {
                thrusterL = Instantiate(thrusterL); thrusterR = Instantiate(thrusterR);
                thrusterL.transform.SetParent(GorillaTagger.Instance.offlineVRRig.leftHandTransform, false);
                thrusterR.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
                thrusterL.transform.localPosition = new Vector3(0.0372000001f, -0.0414999984f, 0.0480999984f);
                thrusterL.transform.localRotation = Quaternion.Euler(336.230774f, 82.4238434f, 269.163513f);
                thrusterL.transform.localScale = new Vector3(1.19494653f, 1.47576821f, 1.33444989f);
                thrusterR.transform.localPosition = new Vector3(-0.038189102f, -0.0489270277f, 0.0503906533f);
                thrusterR.transform.localRotation = Quaternion.Euler(338.277008f, 273.67511f, 273.216217f);
                thrusterR.transform.localScale = new Vector3(1.19494653f, -1.47576797f, 1.33444989f);
                thrusterL.SetActive(false); thrusterR.SetActive(false);
                var prtclL = thrusterL.transform.Find("Particle System").GetComponent<ParticleSystem>().main; prtclL.simulationSpeed = 10; thrusterL.transform.Find("Particle System").gameObject.SetActive(false);
                var prtclR = thrusterR.transform.Find("Particle System").GetComponent<ParticleSystem>().main; prtclR.simulationSpeed = 10; thrusterR.transform.Find("Particle System").gameObject.SetActive(false);
                thrusterL.transform.Find("Particle System").transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                thrusterR.transform.Find("Particle System").transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                var allTypes = Assembly.GetExecutingAssembly().GetTypes();
                indexPanel = Instantiate(indexPanel.transform.Find("Pivot").gameObject);
                indexPanel.AddComponent<HoldableEngine>();
                indexPanel.SetActive(false);
                indexPanel.transform.Find("IndexPanel").GetComponent<MeshRenderer>().material.SetColor("_OuterPlatformColor", new Color(panelColorOuter.Value.x, panelColorOuter.Value.y, panelColorOuter.Value.z));
                indexPanel.transform.Find("IndexPanel").GetComponent<MeshRenderer>().material.SetColor("_MainPlatformColor", new Color(panelColorInner.Value.x, panelColorInner.Value.y, panelColorInner.Value.z));
                var modsTransform = indexPanel.transform.Find("Mods");
                modsTransform.Find("page1").gameObject.SetActive(true);
                modsTransform.Find("page2").gameObject.SetActive(false);
                indexPanel.transform.Find("Page1").AddComponent<ButtonManager>();
                indexPanel.transform.Find("Page2").AddComponent<ButtonManager>();
                foreach (var modType in allTypes.Where(modType => typeof(ModHandler).IsAssignableFrom(modType) && !modType.IsAbstract))
                {
                    ModHandler modInstance = ModHandler.CreateInstance(modType);
                    if (modInstance != null)
                    {
                        initMods.Add(modInstance);
                        modInstance.SetConfig();
                        modInstance.Start();
                        var modAttrib = (IndexMod)Attribute.GetCustomAttribute(modType, typeof(IndexMod));
                        if (modAttrib != null)
                        {
                            var modPanel = indexPanel.transform.Find($"Mods/{modAttrib.ModID}");
                            if (modPanel != null)
                            {
                                modPanel.AddComponent<ButtonManager>();
                                modPanel.Find("Text").GetComponent<TextMeshPro>().text = modAttrib.ModName;
                            }
                        }
                    }
                }
                foreach (ModHandler mod in initMods)
                {
                    var attrib = (IndexMod)Attribute.GetCustomAttribute(mod.GetType(), typeof(IndexMod));
                    if (attrib != null)
                    {
                        var page1HashSet = new HashSet<string> { "1", "2", "3", "4", "5", "6", "7", "8" };
                        var page2HashSet = new HashSet<string> { "9", "10", "11", "12", "13", "14", "15", "16" };
                        if (page1HashSet.Contains(attrib.ModID.ToString()))
                            indexPanel.transform.Find($"Mods/{attrib.ModID}").SetParent(indexPanel.transform.Find("Mods/page1"), false);
                        else if (page2HashSet.Contains(attrib.ModID.ToString()))
                            indexPanel.transform.Find($"Mods/{attrib.ModID}").SetParent(indexPanel.transform.Find("Mods/page2"), false);
                    }
                }
                foreach (Transform child in indexPanel.transform.Find("Mods"))
                {
                    if (!new HashSet<string> { "page1", "page2" }.Contains(child.name))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                initialized = true;
                NetworkSystem.Instance.OnJoinedRoomEvent += LоbbyChеck;
                NetworkSystem.Instance.OnReturnedToSinglePlayer += LоbbyChеck;
                version();
                indexPanel.transform.Find("IndexPanel/ModInfo").GetComponent<TextMeshPro>().text = "No mod selected\n\nNo mod selected";
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        private void LоbbyChеck()
        {
            if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                inRoom = true;
                if (!indexPanel.activeSelf)
                {
                    indexPanel.SetActive(true);
                }
            }
            else
            {
                inRoom = false;
                if (indexPanel.activeSelf)
                {
                    indexPanel.SetActive(false);
                }

                foreach (ModHandler mod in initMods)
                {
                    if (mod.enabled)
                    {
                        mod.OnModDisabled();
                    }
                }
            }
        }

        void Update()
        {
            if (!initialized || !inRoom) return;

            foreach (ModHandler mod in initMods)
            {
                if (mod.enabled)
                {
                    mod.OnUpdate();
                }
            }

            if (ControllerInputPoller.instance.leftControllerPrimaryButton && ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                indexPanel.transform.rotation = GorillaTagger.Instance.mainCamera.transform.rotation;
                indexPanel.transform.position = GTPlayer.Instance.headCollider.transform.position + GTPlayer.Instance.headCollider.transform.forward;
            }
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }
    }
}