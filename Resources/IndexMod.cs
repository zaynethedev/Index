using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Index.Resources
{
    public abstract class IndexMod
    {
        public string modName;
        public string modDescription;
        public string modGUID;
        public int modID;
        public static ModType modType;
        public string modTypeString = modType.ToString();

        public virtual bool enabled { get; set; }

        public virtual void Start() { enabled = false; }
        public virtual void OnFixedUpdate() { }

        public virtual void OnUpdate() { }
        public virtual void OnModEnabled() 
        { 
            enabled = true;
            Plugin.indexPanel.transform.Find("IndexPanel/ModInfo").gameObject.GetComponent<TextMeshPro>().text = $"{modName}\n\n{modDescription}\n\n{modTypeString}";
            if (Plugin.indexPanel.transform.Find($"Mods/page1/{modID}"))
            {
                Plugin.indexPanel.transform.Find($"Mods/page1/{modID}").gameObject.GetComponent<MeshRenderer>().material = ButtonManager.selectedMaterial;
            }
            else if (Plugin.indexPanel.transform.Find($"Mods/page2/{modID}"))
            {
                Plugin.indexPanel.transform.Find($"Mods/page2/{modID}").gameObject.GetComponent<MeshRenderer>().material = ButtonManager.selectedMaterial;
            }
        }
        public virtual void OnModDisabled() 
        {
            enabled = false;
            if (Plugin.indexPanel.transform.Find($"Mods/page1/{modID}"))
            {
                Plugin.indexPanel.transform.Find($"Mods/page1/{modID}").gameObject.GetComponent<MeshRenderer>().material = ButtonManager.unselectedMaterial;
            }
            else if (Plugin.indexPanel.transform.Find($"Mods/page2/{modID}"))
            {
                Plugin.indexPanel.transform.Find($"Mods/page2/{modID}").gameObject.GetComponent<MeshRenderer>().material = ButtonManager.unselectedMaterial;
            }
        } 
    }

    public enum ModType
    {
        gameplay,
        miscellaneous
    }
}