using BepInEx;
using UnityEngine;
using Index.Resources;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using TMPro;
using DevHoldableEngine;
using GorillaLocomotion;
using HarmonyLib;
using BepInEx.Configuration;
using Index.Scripts;

namespace Index
{
    [BepInPlugin("IndexTeam.Index", "Index", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static bool inRoom, initialized;
        public static List<ModHandler> mods = new List<ModHandler>();
        public static GameObject indexPanel;
        public List<GameObject> buttons = new List<GameObject>();
        public static Harmony harmony;
        public static ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "Index.cfg"), true);
        public static GameObject puncallbacks_xray;
        public ConfigEntry<Vector3> panelColorOuter;
        public ConfigEntry<Vector3> panelColorInner;

        void Start()
        {
            harmony = Harmony.CreateAndPatchAll(GetType().Assembly, "zaynethedev.Index");
            preInit();
            puncallbacks_xray = new GameObject("puncallbacks_xray");
            puncallbacks_xray.AddComponent<XRayPUNCallbacks>();
            puncallbacks_xray.SetActive(false);
            GorillaTagger.OnPlayerSpawned(init);
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
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();

            indexPanel = Instantiate(indexPanel.transform.Find("Pivot").gameObject);
            indexPanel.AddComponent<DevHoldable>();
            InitializePanelTransform();
            SetupModPanel();
            foreach (var type in allTypes)
            {
                InitializeMod(type);
            }
            DisableUnusedMods();
            initialized = true;
            indexPanel.transform.Find("IndexPanel/ModInfo").gameObject.GetComponent<TextMeshPro>().text = $"No mod selected\n\nNo mod selected";
        }

        void InitializePanelTransform()
        {
            var indexTransform = indexPanel.transform;
            indexTransform.localPosition = new Vector3(-67.3437f, 11.6f, -81.9055f);
            indexTransform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            indexTransform.rotation = Quaternion.Euler(0f, 335f, 0f);
        }

        void SetupModPanel()
        {
            var indexTransform = indexPanel.transform;
            var indexPanelMods = indexTransform.Find("Mods");
            buttons.Add(indexTransform.Find("Page1").gameObject);
            buttons.Add(indexTransform.Find("Page2").gameObject);
            foreach (var btn in buttons)
            {
                btn.AddComponent<ButtonManager>();
            }
            indexTransform.Find("Mods/page1").gameObject.SetActive(true);
            indexTransform.Find("Mods/page2").gameObject.SetActive(false);
            indexTransform.Find("IndexPanel/IndexInfo").GetComponent<TextMeshPro>().text = "INDEX v1.0.0";
            indexTransform.Find("IndexPanel").gameObject.GetComponent<MeshRenderer>().material.SetColor("_OuterPlatformColor", new Color(panelColorOuter.Value.x, panelColorOuter.Value.y, panelColorOuter.Value.z));
            indexTransform.Find("IndexPanel").gameObject.GetComponent<MeshRenderer>().material.SetColor("_MainPlatformColor", new Color(panelColorInner.Value.x, panelColorInner.Value.y, panelColorInner.Value.z));
            indexPanel.SetActive(false);
        }

        void InitializeMod(Type modType)
        {
            if (!typeof(ModHandler).IsAssignableFrom(modType) || modType.IsAbstract)
            {
                return;
            }
            ModHandler modInstance = ModHandler.CreateInstance(modType);
            if (modInstance == null) return;

            mods.Add(modInstance);
            modInstance.Start();
            SetupModUI(modInstance);
        }

        void SetupModUI(ModHandler modInstance)
        {
            var modType = modInstance.GetType();
            var indexModAttribute = (IndexMod)Attribute.GetCustomAttribute(modType, typeof(IndexMod));

            if (indexModAttribute != null)
            {
                int modID = indexModAttribute.ModID;
                var modPanel = indexPanel.transform.Find($"Mods/{modID}");
                if (modPanel == null)
                {
                    Debug.LogError($"Mod panel for {modInstance.modName} (ID: {modID}) not found.");
                    return;
                }

                TextMeshPro textComponent = modPanel.Find("Text")?.GetComponent<TextMeshPro>();
                if (textComponent != null)
                {
                    textComponent.text = modInstance.modName;
                }
                if (!modPanel.GetComponent<ButtonManager>())
                {
                    var buttonManager = modPanel.gameObject.AddComponent<ButtonManager>();
                    buttonManager.Start();
                }
                DistributeModToPage(modPanel);
            }
            else
            {
                Debug.LogError($"IndexMod attribute not found for {modInstance.modName}");
            }
        }

        void DistributeModToPage(Transform modPanel)
        {
            var modName = modPanel.gameObject.name;
            var page1 = indexPanel.transform.Find("Mods/page1");
            var page2 = indexPanel.transform.Find("Mods/page2");
            if (new HashSet<string> { "1", "2", "3", "4", "5", "6", "7", "8" }.Contains(modName))
                modPanel.SetParent(page1, false);
            else if (new HashSet<string> { "9", "10", "11", "12", "13", "14", "15", "16" }.Contains(modName))
                modPanel.SetParent(page2, false);
        }

        void DisableUnusedMods()
        {
            var indexPanelMods = indexPanel.transform.Find("Mods");

            foreach (Transform child in indexPanelMods)
            {
                if (!new HashSet<string> { "page1", "page2" }.Contains(child.name))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        void Update()
        {
            if (!initialized) return;

            if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                indexPanel.transform.Find("IndexPanel").gameObject.GetComponent<MeshRenderer>().material.SetColor("_OuterPlatformColor", new Color(panelColorOuter.Value.x, panelColorOuter.Value.y, panelColorOuter.Value.z));
                indexPanel.transform.Find("IndexPanel").gameObject.GetComponent<MeshRenderer>().material.SetColor("_MainPlatformColor", new Color(panelColorInner.Value.x, panelColorInner.Value.y, panelColorInner.Value.z));
                foreach (ModHandler index in mods)
                    if (index.enabled)
                        index.OnUpdate();
            }
        }

        void FixedUpdate()
        {
            if (!initialized) return;

            if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                HandleModPanelVisibility();
                foreach (ModHandler index in mods)
                {
                    if (index.enabled)
                    {
                        index.OnFixedUpdate();
                    }
                }
            }
            else
            {
                if (inRoom)
                {
                    inRoom = false;
                }
                if (indexPanel.activeSelf)
                {
                    indexPanel.SetActive(false);
                }
                foreach (ModHandler index in mods)
                {
                    if (index.enabled)
                    {
                        index.OnModDisabled();
                    }
                }
            }
        }

        void HandleModPanelVisibility()
        {
            if (!inRoom) inRoom = true;

            if (ControllerInputPoller.instance.leftControllerPrimaryButton && ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                indexPanel.transform.rotation = GorillaTagger.Instance.mainCamera.transform.transform.rotation;
                indexPanel.transform.position = Player.Instance.headCollider.transform.position + Player.Instance.headCollider.transform.forward;
            }

            if (!indexPanel.activeSelf)
            {
                indexPanel.SetActive(true);
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
