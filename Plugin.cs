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
using BepInEx.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Index.BepInfo;
using BoingKit;

namespace Index
{
    [BepInPlugin(Info_Plugin.guid, Info_Plugin.name, Info_Plugin.version)]
    public class Plugin : BaseUnityPlugin
    {
        public static bool inRoom, initialized;
        public static List<ModHandler> initMods = new List<ModHandler>();
        public static GameObject indexPanel;
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
            string onlineVersion = await fetch();
            if (!string.IsNullOrEmpty(onlineVersion) && onlineVersion == Info_Plugin.version)
            {
                indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().text = "INDEX v1.0.3";
            }
            else
            {
                indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().fontSize -= 3;
                indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().color = Color.red;
                indexPanel.transform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().text = $"NEW VERSION AVAILABLE: v{onlineVersion}";
            }
        }

        private async Task<string> fetch()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string content = await client.GetStringAsync("https://raw.githubusercontent.com/zaynethedev/Index/main/ver.txt");
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

                foreach (ModHandler mod in initMods)
                {
                    if (mod.enabled)
                    {
                        mod.OnUpdate();
                    }
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

            if (ControllerInputPoller.instance.leftControllerPrimaryButton && ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                indexPanel.transform.rotation = GorillaTagger.Instance.mainCamera.transform.rotation;
                indexPanel.transform.position = Player.Instance.headCollider.transform.position + Player.Instance.headCollider.transform.forward;
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