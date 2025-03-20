using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using BepInEx;
using UnityEngine.InputSystem;
using Index.BepInfo;

namespace Index.Scripts
{
    [BepInPlugin(Info_ThemeLoader.guid, Info_ThemeLoader.name, Info_ThemeLoader.version)]
    public class IndexThemeLoader : BaseUnityPlugin
    {
        private static GameObject indexPanel;
        private static Transform iTransform;
        private static List<GameObject> loadedObjects = new List<GameObject>();
        private static string tempFolder = Path.Combine(Application.dataPath, "Index/Loaders/Themes/TMP");
        private static string zipFolder = Path.Combine(Application.dataPath, "Index/Loaders/Themes/Zip");
        private static int index = -1;

        void Awake() { StartCoroutine(Init()); }
        IEnumerator Init()
        {
            while (Plugin.indexPanel == null || Plugin.indexPanel.transform.Find("IndexPanel") == null)
            {
                yield return new WaitForSeconds(1f);
            }
            indexPanel = Plugin.indexPanel;
            iTransform = indexPanel.transform.Find("IndexPanel");
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
            if (!Directory.Exists(zipFolder)) Directory.CreateDirectory(zipFolder);
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
            var mr = iTransform.GetComponent<MeshRenderer>();
            mr.enabled = true;
            foreach (GameObject o in loadedObjects)
                o.SetActive(false);
        }

        void LoadThemes()
        {
            string themesPath = Path.Combine(Paths.PluginPath, "Index/Loaders/Themes");
            if (!Directory.Exists(themesPath))
                return;
            string[] files = Directory.GetFiles(themesPath, "*.indextheme");
            foreach (var file in files)
                LoadTheme(file);
        }
        void LoadTheme(string file)
        {
            if (indexPanel == null || iTransform == null)
                return;
            string themeZipPath = Path.Combine(zipFolder, Path.GetFileNameWithoutExtension(file) + ".zip");
            try { File.Copy(file, themeZipPath, true); } catch (Exception e) { Debug.Log("File copy error: " + e.Message); return; }
            if (!File.Exists(themeZipPath))
                return;
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
            try { ZipFile.ExtractToDirectory(themeZipPath, tempFolder, true); }
            catch (Exception e) { Debug.Log("Extraction error: " + e.Message); return; }
            string jsonPath = Path.Combine(tempFolder, "info.json");
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                var info = JsonUtility.FromJson<ThemeInfo>(json);
            }
            string bundlePath = Path.Combine(tempFolder, "theme.bundle");
            if (File.Exists(bundlePath))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
                if (bundle != null)
                    ApplyTheme(bundle, JsonUtility.FromJson<ThemeInfo>(File.ReadAllText(jsonPath)).Name);
            }
            try { Directory.Delete(tempFolder, true); }
            catch (Exception e) { Debug.Log("Deletion error: " + e.Message); }
        }
        void ApplyTheme(AssetBundle bundle, string name)
        {
            GameObject[] prefabs = bundle.LoadAllAssets<GameObject>();
            if (prefabs.Length > 0)
            {
                GameObject prefab = prefabs[0];
                GameObject instance = Instantiate(prefab);
                instance.transform.SetParent(iTransform, false);
                Material[] mats = bundle.LoadAllAssets<Material>();
                Renderer[] rends = instance.GetComponentsInChildren<Renderer>();
                if (mats.Length > 0)
                {
                    foreach (Renderer r in rends)
                        r.material = mats[0];
                }
                instance.SetActive(false);
                loadedObjects.Add(instance);
                instance.name = name;
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.Euler(90, 0, 0);
                instance.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            bundle.Unload(false);
        }
        void SwitchTheme(int direction)
        {
            if (loadedObjects.Count == 0)
                return;
            int newIndex = (index + direction) % loadedObjects.Count;
            if (newIndex < 0)
                newIndex = loadedObjects.Count - 1;
            index = newIndex;
            ActivateSelectedTheme();
        }
        void ActivateSelectedTheme()
        {
            if (iTransform == null) return;
            MeshRenderer mr = iTransform.GetComponent<MeshRenderer>();
            if (mr != null)
                mr.enabled = false;
            foreach (var obj in loadedObjects)
                obj.SetActive(false);
            loadedObjects[index].SetActive(true);
            loadedObjects[index].transform.localPosition = Vector3.zero;
            loadedObjects[index].transform.localRotation = Quaternion.Euler(90, 0, 0);
            loadedObjects[index].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        void OnDestroy()
        {
            if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
            if (Directory.Exists(zipFolder)) Directory.Delete(zipFolder, true);
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