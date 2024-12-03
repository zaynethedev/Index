using BepInEx;
using UnityEngine;
using Index.Resources;
using System.Collections.Generic;
using System;
using Photon.Pun;
using System.IO;
using System.Reflection;
using TMPro;
using DevHoldableEngine;

namespace Index
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static bool inRoom;
        public static List<IndexMod> mods = new List<IndexMod>();
        public static GameObject indexPanel;

        void Start()
        {
            var bundle = LoadAssetBundle("Index.Resources.index");
            indexPanel = bundle.LoadAsset<GameObject>("IndexPanel");
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized()
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            indexPanel = indexPanel.transform.Find("Pivot").gameObject;
            indexPanel = Instantiate(indexPanel);
            indexPanel.AddComponent<DevHoldable>();
            var indexTransform = indexPanel.transform;
            indexTransform.localPosition = new Vector3(-67.3437f, 12f, -81.9055f);
            indexTransform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            indexTransform.rotation = Quaternion.Euler(0f, 335f, 0f);
            var indexPanelMods = indexTransform.Find("Mods");
            indexTransform.Find("Page1").gameObject.AddComponent<ButtonManager>();
            indexTransform.Find("Page2").gameObject.AddComponent<ButtonManager>();
            indexPanel.SetActive(false);
            indexTransform.Find("Mods/page2").gameObject.SetActive(false);
            var modParent = new GameObject("Index.Mods");

            foreach (var type in allTypes)
            {
                if (typeof(IndexMod).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    IndexMod modInstance = (IndexMod)Activator.CreateInstance(type);
                    mods.Add(modInstance);

                    GameObject modGameObject = new GameObject(modInstance.modName);
                    modGameObject.AddComponent(modInstance.GetType());
                    modGameObject.transform.parent = modParent.transform;
                    modInstance.Start();
                    Transform panelMod = indexPanelMods.Find($"{modInstance.modID}");

                    TextMeshPro textComponent = panelMod.Find("Text")?.GetComponent<TextMeshPro>();
                    if (textComponent != null)
                    {
                        textComponent.text = modInstance.modName.ToString();
                    }
                    if (!panelMod.GetComponent<ButtonManager>())
                    {
                        var buttonManager = panelMod.gameObject.AddComponent<ButtonManager>();
                        buttonManager.Start();
                    }

                    Transform page1 = indexPanelMods.Find("page1");
                    Transform page2 = indexPanelMods.Find("page2");
                    if (new HashSet<string> { "1", "2", "3", "4", "5", "6", "7", "8" }.Contains(panelMod.gameObject.name))
                    {
                        if (page1 != null)
                        {
                            panelMod.transform.SetParent(page1, false);
                        }
                    }
                    else if (new HashSet<string> { "9", "10", "11", "12", "13", "14" }.Contains(panelMod.gameObject.name))
                    {
                        if (page2 != null)
                        {
                            panelMod.transform.SetParent(page2, false);
                        }
                    }

                    Debug.Log($"INDEX // {modInstance.modName} initialized correctly.");
                }
            }
            foreach (Transform child in indexPanelMods)
            {
                if (child.name != "page1" && child.name != "page2")
                {
                    Debug.Log($"INDEX // Disabling unused mod. ModID: {child.name}");
                    child.gameObject.SetActive(false);
                }
            }

            Debug.Log("INDEX Initialization complete.");
        }

        void Update()
        {
            if (mods != null)
            {
                if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Contains("MODDED"))
                {
                    foreach (IndexMod index in mods)
                    {
                        if (index.enabled)
                        {
                            index.OnUpdate();
                        }
                    }
                }
            }
        }
        void FixedUpdate()
        {
            if (mods != null)
            {
                if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Contains("MODDED"))
                {
                    if (!inRoom)
                        inRoom = true;

                    if (!indexPanel.activeSelf)
                        indexPanel.SetActive(true);

                    foreach (IndexMod index in mods)
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
                        inRoom = false;

                    if (indexPanel.activeSelf)
                        indexPanel.SetActive(false);

                    foreach (IndexMod index in mods)
                    {
                        if (index.enabled)
                        {
                            index.OnModDisabled();
                        }
                    }

                }
            }
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
    }
}
