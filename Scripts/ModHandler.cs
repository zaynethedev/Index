using System;
using TMPro;
using UnityEngine;

namespace Index.Resources
{
    public abstract class ModHandler
    {
        public string modName;
        public string modDescription;
        public string modGUID;
        public int modID;
        public bool enabled;

        public virtual void Start() { enabled = false; }
        public virtual void OnUpdate() { }
        public virtual void SetConfig() { }
        public virtual void OnModEnabled()
        {
            enabled = true;
            Plugin.indexPanel.transform.Find("IndexPanel/ModInfo").gameObject.GetComponent<TextMeshPro>().text = $"{modName}\n\n{modDescription}";
            foreach (Transform page in Plugin.indexPanel.transform.Find("Mods"))
            {
                if (page.name.Contains("page"))
                {
                    foreach (Transform modButton in page)
                    {
                        if (modButton.name == modID.ToString())
                        {
                            modButton.gameObject.GetComponent<MeshRenderer>().material = ButtonManager.selectedMaterial;
                        }
                    }
                }
            }

        }
        public virtual void OnModDisabled()
        {
            enabled = false;
            foreach (Transform page in Plugin.indexPanel.transform.Find("Mods"))
            {
                if (page.name.Contains("page"))
                {
                    foreach (Transform modButton in page)
                    {
                        if (modButton.name == modID.ToString())
                        {
                            modButton.gameObject.GetComponent<MeshRenderer>().material = ButtonManager.unselectedMaterial;
                        }
                    }
                }
            }
        }

        public static ModHandler CreateInstance(Type modType)
        {
            var attribute = (IndexMod)Attribute.GetCustomAttribute(modType, typeof(IndexMod));

            if (attribute != null)
            {
                ModHandler instance = (ModHandler)Activator.CreateInstance(modType);

                instance.modName = attribute.ModName;
                instance.modDescription = attribute.ModDescription;
                instance.modGUID = attribute.ModGUID;
                instance.modID = attribute.ModID;
                Debug.Log($"Mod Loaded: {instance.modName} (ID: {instance.modID})");

                return instance;
            }
            else
            {
                Debug.LogError("No IndexMod attribute found on class " + modType.Name);
            }
            return null;
        }
    }
}