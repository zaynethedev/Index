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

namespace Index
{
    [BepInPlugin("zaynethedev.Index", "Index", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static bool inRoom, initialized;
        public static List<ModHandler> mods = new List<ModHandler>();
        public static GameObject indexPanel;
        private static Harmony instance;
        public static bool IsPatched { get; private set; }
        public const string InstanceId = "zaynethedev.Index";

        void Start()
        {
            preInit();
            GorillaTagger.OnPlayerSpawned(init);
        }

        void preInit()
        {
            var bundle = LoadAssetBundle("Index.Resources.index");
            indexPanel = bundle.LoadAsset<GameObject>("IndexPanel");
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
            Debug.Log("INDEX Initialization complete.");

            if (!IsPatched)
            {
                if (instance == null)
                {
                    instance = new Harmony(InstanceId);
                }

                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        void InitializePanelTransform()
        {
            var indexTransform = indexPanel.transform;
            indexTransform.localPosition = new Vector3(-67.3437f, 12f, -81.9055f);
            indexTransform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            indexTransform.rotation = Quaternion.Euler(0f, 335f, 0f);
        }

        void SetupModPanel()
        {
            var indexTransform = indexPanel.transform;
            var indexPanelMods = indexTransform.Find("Mods");
            indexTransform.Find("Page1").gameObject.AddComponent<ButtonManager>();
            indexTransform.Find("Page2").gameObject.AddComponent<ButtonManager>();
            indexTransform.Find("Mods/page2").gameObject.SetActive(false);
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
            GameObject modGameObject = new GameObject(modInstance.modName);
            modGameObject.AddComponent(modType);
            modGameObject.transform.parent = indexPanel.transform.Find("Mods").transform;
            modInstance.Start();
            SetupModUI(modInstance);

            Debug.Log($"INDEX // {modInstance.modName} initialized correctly.");
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
            else if (new HashSet<string> { "9", "10", "11", "12", "13", "14" }.Contains(modName))
                modPanel.SetParent(page2, false);
        }

        void DisableUnusedMods()
        {
            var indexPanelMods = indexPanel.transform.Find("Mods");

            foreach (Transform child in indexPanelMods)
            {
                if (child.name != "page1" && child.name != "page2")
                {
                    Debug.Log($"INDEX // Disabling unused mod. ModID: {child.name}");
                    child.gameObject.SetActive(false);
                }
            }
        }

        void Update()
        {
            if (!initialized) return;

            /*if (Keyboard.current.jKey.wasPressedThisFrame)
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("F6", JoinType.Solo);*/

            if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED"))
                foreach (ModHandler index in mods)
                    if (index.enabled)
                        index.OnUpdate();
            else
                    if (instance != null && IsPatched)
                    {
                        instance.UnpatchSelf();
                        IsPatched = false;
                    }
        }

        void FixedUpdate()
        {
            if (!initialized) return;

            if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                HandleModPanelVisibility();
                HandleModUpdates();
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
                var offset = new Vector3(0.4f, 0, 0.4f);
                indexPanel.transform.position = Player.Instance.headCollider.transform.position + offset;
                indexPanel.transform.rotation = Player.Instance.headCollider.transform.rotation;
            }

            if (!indexPanel.activeSelf)
            {
                indexPanel.SetActive(true);
            }
        }

        void HandleModUpdates()
        {
            foreach (ModHandler index in mods)
            {
                if (index.enabled)
                {
                    index.OnFixedUpdate();
                }
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