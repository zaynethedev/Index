using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using BepInEx;
using UnityEngine.InputSystem;

namespace Index.Scripts
{
    [BepInPlugin("indexteam.indexthemeLoader", "Index Theme Loader", "1.0.7")]
    public class IndexThemeLoader : BaseUnityPlugin
    {
        private static GameObject indexPanel;
        private static Transform indexPanelTransform;
        private static List<GameObject> loadedThemeObjects = new List<GameObject>();
        private static string tempThemeFolder = Path.Combine(Application.dataPath, "Index/ThemeTMP");
        private static string tempZipFolder = Path.Combine(Application.dataPath, "Index/TempZips");
        private static int currentThemeIndex = -1;

        void Awake() { StartCoroutine(Init()); }
        IEnumerator Init()
        {
            while (Plugin.indexPanel == null || Plugin.indexPanel.transform.Find("IndexPanel") == null)
            {
                yield return new WaitForSeconds(1f);
            }
            indexPanel = Plugin.indexPanel;
            indexPanelTransform = indexPanel.transform.Find("IndexPanel");
            if (!Directory.Exists(tempThemeFolder)) Directory.CreateDirectory(tempThemeFolder);
            if (!Directory.Exists(tempZipFolder)) Directory.CreateDirectory(tempZipFolder);
            LoadThemes();
        }
        void Update()
        {
            if (Keyboard.current.jKey.wasPressedThisFrame) SwitchTheme(-1);
            if (Keyboard.current.kKey.wasPressedThisFrame) SwitchTheme(1);
            if (Keyboard.current.lKey.wasPressedThisFrame) RemoveThemes();
        }

        void RemoveThemes()
        {
            var mr = indexPanelTransform.GetComponent<MeshRenderer>();
            mr.enabled = true;
            foreach (GameObject o in loadedThemeObjects)
                o.SetActive(false);
        }

        void LoadThemes()
        {
            string themesPath = Path.Combine(Paths.PluginPath, "Index/Themes");
            if (!Directory.Exists(themesPath))
                return;
            string[] files = Directory.GetFiles(themesPath, "*.indextheme");
            foreach (var file in files)
                LoadTheme(file);
        }
        void LoadTheme(string file)
        {
            if (indexPanel == null || indexPanelTransform == null)
                return;
            string themeZipPath = Path.Combine(tempZipFolder, Path.GetFileNameWithoutExtension(file) + ".zip");
            try { File.Copy(file, themeZipPath, true); } catch (Exception e) { Debug.Log("File copy error: " + e.Message); return; }
            if (!File.Exists(themeZipPath))
                return;
            if (!Directory.Exists(tempThemeFolder)) Directory.CreateDirectory(tempThemeFolder);
            try { ZipFile.ExtractToDirectory(themeZipPath, tempThemeFolder, true); }
            catch (Exception e) { Debug.Log("Extraction error: " + e.Message); return; }
            string jsonPath = Path.Combine(tempThemeFolder, "info.json");
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                var info = JsonUtility.FromJson<ThemeInfo>(json);
            }
            string bundlePath = Path.Combine(tempThemeFolder, "theme.bundle");
            if (File.Exists(bundlePath))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
                if (bundle != null)
                    ApplyTheme(bundle);
            }
            try { Directory.Delete(tempThemeFolder, true); }
            catch (Exception e) { Debug.Log("Deletion error: " + e.Message); }
        }
        void ApplyTheme(AssetBundle bundle)
        {
            GameObject[] prefabs = bundle.LoadAllAssets<GameObject>();
            if (prefabs.Length > 0)
            {
                GameObject prefab = prefabs[0];
                GameObject instance = Instantiate(prefab);
                instance.transform.SetParent(indexPanelTransform, false);
                Material[] mats = bundle.LoadAllAssets<Material>();
                Renderer[] rends = instance.GetComponentsInChildren<Renderer>();
                if (mats.Length > 0)
                {
                    foreach (Renderer r in rends)
                        r.material = mats[0];
                }
                instance.SetActive(false);
                loadedThemeObjects.Add(instance);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.Euler(90, 0, 0);
                instance.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            bundle.Unload(false);
        }
        void SwitchTheme(int direction)
        {
            if (loadedThemeObjects.Count == 0)
                return;
            int newIndex = (currentThemeIndex + direction) % loadedThemeObjects.Count;
            if (newIndex < 0)
                newIndex = loadedThemeObjects.Count - 1;
            currentThemeIndex = newIndex;
            ActivateSelectedTheme();
        }
        void ActivateSelectedTheme()
        {
            if (indexPanelTransform == null) return;
            MeshRenderer mr = indexPanelTransform.GetComponent<MeshRenderer>();
            if (mr != null)
                mr.enabled = false;
            foreach (var obj in loadedThemeObjects)
                obj.SetActive(false);
            loadedThemeObjects[currentThemeIndex].SetActive(true);
            loadedThemeObjects[currentThemeIndex].transform.localPosition = Vector3.zero;
            loadedThemeObjects[currentThemeIndex].transform.localRotation = Quaternion.Euler(90, 0, 0);
            loadedThemeObjects[currentThemeIndex].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        void OnDestroy()
        {
            if (Directory.Exists(tempThemeFolder)) Directory.Delete(tempThemeFolder, true);
            if (Directory.Exists(tempZipFolder)) Directory.Delete(tempZipFolder, true);
        }
    }
    [Serializable]
    public class ThemeInfo
    {
        public string Name;
        public string Description;
        public string Author;
    }
}