using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using BepInEx;
using UnityEngine.InputSystem;
using Index.Mods;
using Index.BepInfo;

namespace Index.Scripts
{
    [BepInPlugin(Info_PlatLoader.guid, Info_PlatLoader.name, Info_PlatLoader.version)]
    public class IndexPlatformLoader : BaseUnityPlugin
    {
        private static GameObject indexPanel;
        private static Transform iTransform;
        private static List<GameObject> loadedObjectsL = new List<GameObject>(), loadedObjectsR = new List<GameObject>();
        private static string tempFolder = Path.Combine(Application.dataPath, "Index/Loaders/Platforms/TMP");
        private static string zipFolder = Path.Combine(Application.dataPath, "Index/Loaders/Platforms/Zip");
        private static int indexL = -1, indexR = -1;


        void Awake() { StartCoroutine(Init()); }
        IEnumerator Init()
        {
            while (Plugin.indexPanel == null || Plugin.indexPanel.transform.Find("IndexPanel") == null || Platforms.instance == null)
            {
                yield return new WaitForSeconds(1f);
            }
            indexPanel = Plugin.indexPanel;
            iTransform = indexPanel.transform.Find("IndexPanel");
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
            if (!Directory.Exists(zipFolder)) Directory.CreateDirectory(zipFolder);
            LoadPlatforms();
        }
        void Update()
        {
            if (Keyboard.current.bKey.wasPressedThisFrame) SwitchPlatform(-1);
            if (Keyboard.current.nKey.wasPressedThisFrame) SwitchPlatform(1);
            if (Keyboard.current.mKey.wasPressedThisFrame) RemovePlatforms();
        }

        void RemovePlatforms()
        {
            foreach (GameObject o in loadedObjectsL)
                o.SetActive(false);
            foreach (GameObject o in loadedObjectsR)
                o.SetActive(false);
            Platforms.instance.platformL.GetComponent<MeshRenderer>().enabled = true; Platforms.instance.platformR.GetComponent<MeshRenderer>().enabled = true;
        }

        void LoadPlatforms()
        {
            string platformsPath = Path.Combine(Paths.PluginPath, "Index/Loaders/Platforms");
            if (!Directory.Exists(platformsPath))
                return;
            string[] files = Directory.GetFiles(platformsPath, "*.indexplatform");
            foreach (var file in files)
                LoadPlatform(file);
        }
        void LoadPlatform(string file)
        {
            if (indexPanel == null || iTransform == null)
                return;
            string platformZipPath = Path.Combine(zipFolder, Path.GetFileNameWithoutExtension(file) + ".zip");
            try { File.Copy(file, platformZipPath, true); } catch (Exception e) { Debug.Log("File copy error: " + e.Message); return; }
            if (!File.Exists(platformZipPath))
                return;
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
            try { ZipFile.ExtractToDirectory(platformZipPath, tempFolder, true); }
            catch (Exception e) { Debug.Log("Extraction error: " + e.Message); return; }
            string jsonPath = Path.Combine(tempFolder, "info.json");
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                var info = JsonUtility.FromJson<PlatformInfo>(json);
            }
            string bundlePath = Path.Combine(tempFolder, "platform.bundle");
            if (File.Exists(bundlePath))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
                if (bundle != null)
                    ApplyPlatform(bundle, JsonUtility.FromJson<PlatformInfo>(File.ReadAllText(jsonPath)).Name);
            }
            try { Directory.Delete(tempFolder, true); }
            catch (Exception e) { Debug.Log("Deletion error: " + e.Message); }
        }
        void ApplyPlatform(AssetBundle bundle, string name)
        {
            GameObject[] prefabs = bundle.LoadAllAssets<GameObject>();
            if (prefabs.Length > 0)
            {
                GameObject prefab = prefabs[0];
                GameObject instance = Instantiate(prefab);
                GameObject leftPlat, rightPlat;
                leftPlat = Instantiate(instance);
                rightPlat = Instantiate(instance);
                loadedObjectsL.Add(leftPlat); loadedObjectsR.Add(rightPlat);
                leftPlat.transform.SetParent(Platforms.instance.platformL.transform, false);
                rightPlat.transform.SetParent(Platforms.instance.platformR.transform, false);
                leftPlat.name = $"left{name}"; rightPlat.name = $"right{name}";
                leftPlat.transform.localPosition = Vector3.zero;
                rightPlat.transform.localPosition = Vector3.zero;
            }
            bundle.Unload(false);
        }
        void SwitchPlatform(int direction)
        {
            if (loadedObjectsL.Count == 0 && loadedObjectsR.Count == 1)
                return;
            int newIndexL = (indexL + direction) % loadedObjectsL.Count;
            if (newIndexL < 0)
                newIndexL = loadedObjectsL.Count - 1;
            int newIndexR = (indexR + direction) % loadedObjectsR.Count;
            if (newIndexR < 0)
                newIndexR = loadedObjectsR.Count - 1;
            indexL = newIndexL;
            indexR = newIndexR;
            Platforms.instance.platformL.GetComponent<MeshRenderer>().enabled = false; Platforms.instance.platformR.GetComponent<MeshRenderer>().enabled = false;
            ActivateSelectedPlatform();
        }
        void ActivateSelectedPlatform()
        {
            foreach (var obj in loadedObjectsL)
                obj.SetActive(false);
            foreach (var obj in loadedObjectsR)
                obj.SetActive(false);
            loadedObjectsL[indexL].SetActive(true);
            loadedObjectsR[indexR].SetActive(true);
        }
        void OnDestroy()
        {
            if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
            if (Directory.Exists(zipFolder)) Directory.Delete(zipFolder, true);
        }
    }
    [Serializable]
    public class PlatformInfo
    {
        public string Name;
        public string Description;
        public string Author;
    }
}